import { SolidPlugin } from "@atulin/bun-plugin-solid";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { rm } from "node:fs/promises";
import { dirname, join, relative } from "node:path";
import solidLabels from "solid-labels/babel";

import { log } from "../typescript/src-helpers/logger";
import { Logger } from "./helpers/logger";
import { hasExtension } from "./helpers/path";
import { SizeHistory } from "./helpers/size-history";
import { Stopwatch } from "./helpers/stopwatch";
import { watch } from "./helpers/watcher";
import { manifestPlugin } from "./plugins/manifest-plugin";
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

const clean = async () => {
	console.log(ct`{bold.red 🗑️{dim Cleaning} ${_dest}}`);
	await rm(_dest, { recursive: true, force: true });
};

if (values.clean) {
	await clean();
}

const compile = async (from: Glob, to: string, root: string) => {
	const timer = new Stopwatch();
	const logger = new Logger();

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
					plugins: [[solidLabels, { dev: false }]],
				},
			}),
			cssMinifyPlugin,
			manifestPlugin(),
		],
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
		define: {
			"import.meta.env.DEV": values.release ? "false" : "true",
		},
	});

	const chunks = outputs
		.filter((o) => o.kind === "chunk")
		.map((c) => relative(join(_root, "..", "..", "wwwroot"), c.path).replaceAll("\\", "/"));

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

	const results = outputs.filter((c) => c.kind !== "sourcemap");

	const size = results.reduce((a, b) => a + b.size, 0);

	return { chunks, size };
};

const sizeHistory = new SizeHistory("JS");
const compileAll = async () => {
	const timer = new Stopwatch();
	const logger = new Logger();

	if (values.cleanAlways) {
		await clean();
	}

	const { chunks: jsChunks, size } = await compile(
		new Glob(`${_source}/src/**/[^_]*.{ts,js,tsx}`),
		join(_dest, "/"),
		"src",
	);

	const chunks = jsChunks.map(
		(p) => `<link rel="modulepreload" href="~/${p}" as="script" asp-append-version="true" />`,
	);
	logger.log(ct`{dim Writing _ModulePreloads.cshtml with {reset.bold.underline ${chunks.length}} chunks}`);
	await Bun.write(join(_root, "..", "..", "Pages", "Shared", "_ModulePreloads.cshtml"), chunks.join("\n"));

	const best = (size: number) => convert(size, "bytes").to("best").toString(3);

	logger.log(ct`{green Total size: {bold.underline ${best(size)}}}`);

	const [prev] = await sizeHistory.at(-1);
	const [first] = await sizeHistory.at(0);
	if (prev) {
		const text = (x: number) => (x > 0 ? c.red : c.green)((x < 0 ? "" : "+") + best(x));
		logger.log(`Size difference: ${text(size - prev)} (total: ${text(size - first)})`);
	}
	await sizeHistory.push(size);

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