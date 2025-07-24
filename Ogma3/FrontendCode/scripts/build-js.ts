import { rm } from "node:fs/promises";
import { dirname, join, relative } from "node:path";
import { SolidPlugin } from "@atulin/bun-plugin-solid";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import solidLabels from "solid-labels/babel";
import { log } from "../typescript/src-helpers/logger";
import { alphaBy } from "./helpers/function-helpers";
import { getHash } from "./helpers/hash";
import { Logger } from "./helpers/logger";
import { hasExtension } from "./helpers/path";
import { Parallel } from "./helpers/promises";
import { Stopwatch } from "./helpers/stopwatch";
import { watch } from "./helpers/watcher";
import { cssMinifyPlugin } from "./plugins/minified-css-loader";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-r, --release", "Build in release mode", false)
	.option("--no-minify", "Don't minify code", true)
	.option("-c, --clean", "Clean output directory", false)
	.option("-C --clean-always", "Clean output directory before each compilation", false)
	.parse(Bun.argv)
	.opts();

const _root = dirname(Bun.main);
const _source = join(_root, "..", "typescript");
const _dest = join(_root, "..", "..", "wwwroot", "js");

const prefixWidth = "Javascript".length;

const clean = async () => {
	console.log(ct`{bold.red 🗑️{dim Cleaning} ${_dest}}`);
	await rm(_dest, { recursive: true, force: true });
};

if (values.clean) {
	await clean();
}

const compile = async (from: Glob, to: string, root: string, name: string) => {
	const timer = new Stopwatch();
	const logger = new Logger(`[${name}]`, prefixWidth);

	const files = [...from.scanSync()];
	logger.log(ct`Found {bold.underline ${files.length}} files to compile.`);
	logger.verbose(`Compiling \n\t${files.join("\n\t")}`);

	const { success, logs, outputs } = await Bun.build({
		entrypoints: files,
		outdir: to,
		root: join(_source, root),
		minify: values.minify,
		sourcemap: "linked",
		splitting: true,
		plugins: [
			SolidPlugin({
				babelOptions: {
					plugins: [[solidLabels, {}]],
				},
			}),
			cssMinifyPlugin,
		],
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
	});

	const chunks = outputs
		.filter((o) => o.kind === "chunk")
		.map((c) => relative(join(_root, "..", "..", "wwwroot"), c.path).replaceAll("\\", "/"))
		.map((p) => `<link rel="modulepreload" href="~/${p}" as="script" />`);

	if (success) {
		logger.log(ct`{dim Files compiled in}`, timer);
	} else {
		logger.error(ct`{red Build of files failed after}`, timer);
		for (const log of logs.filter((l) => ["error", "warning"].includes(l.level))) {
			const color = log.level === "error" ? c.red : c.yellow;
			if (log.position) {
				const msg = `[${log.level}]: ${log.position.file} (${log.position.line}:${log.position.column}) ${log.message}`;
				logger.log(color(msg));
			} else {
				logger.log(color(`[${log.level}]: ${log.message}`));
			}
		}
	}

	const size = outputs
		.filter((c) => (["chunk", "asset", "entry-point"] as (typeof c.kind)[]).includes(c.kind))
		.reduce((a, b) => a + b.size, 0);

	return { chunks, size };
};

const ext = ".js";
const generateManifest = async () => {
	const timer = new Stopwatch();
	const files = new Glob(`${_dest}/**/[!_]*.js`).scan();

	const hashed = await Parallel.forEach(files, async (file) => {
		const hash = await getHash(Bun.file(file), "sha256");
		return {
			hash,
			path: relative(_dest, file).replaceAll("\\", "/").replace(ext, ""),
		};
	});

	const manifest = {
		generated: new Date().toISOString(),
		files: hashed.toSorted(alphaBy((d) => d.path)).map(({ path, hash }) => `${path}:${hash}`),
	};

	await Bun.write(join(_source, "generated", "manifest.json"), JSON.stringify(manifest, null, 2));

	const { time, unit } = timer.lap(3);
	console.log(ct`{dim Generated manifest.json in {reset.bold {underline ${time}} ${unit}}}`);
};

const sizeHistory: number[] = [];
const compileAll = async () => {
	const timer = new Stopwatch();
	const logger = new Logger();

	if (values.cleanAlways) {
		await clean();
	}

	const { chunks: jsChunks, size: jsSize } = await compile(
		new Glob(`${_source}/src/**/[^_]*.{ts,js,tsx}`),
		join(_dest, "/"),
		"src",
		"Javascript",
	);

	logger.log(ct`{dim Generating manifest.json}`);
	await generateManifest();
	const { chunks: workersChunks, size: workersSize } = await compile(
		new Glob(`${_source}/src-workers/**/[^_]*.ts`),
		join(_dest, "workers"),
		"src-workers",
		"Workers",
	);

	const chunks = [...jsChunks, ...workersChunks];
	logger.log(ct`{dim Writing _ModulePreloads.cshtml with {reset.bold.underline ${chunks.length}} chunks}`);
	await Bun.write(join(_root, "..", "..", "Pages", "Shared", "_ModulePreloads.cshtml"), chunks.join("\n"));

	const best = (size: number) => convert(size, "bytes").to("best").toString();

	const currentSize = jsSize + workersSize;
	logger.log(ct`{green Total size: {bold.underline ${best(currentSize)}}}`);

	const prev = sizeHistory.at(-1);
	if (prev) {
		const text = (x: number) => (x > 0 ? c.red : c.green)((x < 0 ? "" : "+") + best(x));
		logger.log(`Size difference: ${text(currentSize - prev)} (total: ${text(currentSize - sizeHistory[0])})`);
	}
	sizeHistory.push(currentSize);

	await Bun.write(join(_dest, ".gitkeep"), "");

	logger.log(ct`{dim Files compiled in}`, timer);
};

await compileAll();

if (values.watch) {
	await watch(_source, ["update"], {
		transformer: (events) =>
			events.filter(({ path }) => hasExtension(path, "tsx", "ts", "js", "css")).map((e) => e.path),
		predicate: (files) => files.length > 0,
		action: async (_) => await compileAll(),
	});
}
