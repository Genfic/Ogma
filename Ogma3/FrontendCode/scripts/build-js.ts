import { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { dirsize } from "./helpers/dirsize";
import { log } from "./helpers/logger";
import { SolidPlugin } from "@atulin/bun-plugin-solid";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";
import { rm } from "node:fs/promises";
import { cssMinifyPlugin } from "./plugins/minified-css-loader";
import { Stopwatch } from "./helpers/stopwatch";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-r, --release", "Build in release mode", false)
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
	const prefix = c.bold.dim(`[${name}]`.padEnd(prefixWidth + 2));

	const files = [...from.scanSync()];
	console.log(ct`${prefix} Found {bold.underline ${files.length}} files to compile.`);

	log.verbose(`${prefix} Compiling \n\t${files.join("\n\t")}`);

	const { success, logs, outputs } = await Bun.build({
		entrypoints: files,
		outdir: to,
		root: join(_source, root),
		minify: true,
		sourcemap: "external",
		splitting: true,
		plugins: [SolidPlugin(), cssMinifyPlugin],
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
	});

	const chunks = outputs
		.filter((o) => o.kind === "chunk")
		.map((c) => c.path.split("wwwroot").at(-1)?.replaceAll("\\", "/"))
		.filter((s) => typeof s === "string")
		.map((p) => `<link rel="modulepreload" href="~${p}" as="script" />`);

	const { time, unit } = timer.lap(3);
	if (success) {
		console.log(ct`${prefix} {dim Files compiled in {reset.bold {underline ${time}} ${unit}}}`);
	} else {
		console.error(ct`${prefix} {red Build of files failed after {reset.bold {underline ${time}} ${unit}}`);
		for (const log of logs.filter((l) => ["error", "warning"].includes(l.level))) {
			const color = log.level === "error" ? c.red : c.yellow;
			if (log.position) {
				console.log(
					color(
						`[${log.level}]: ${log.position.file} (${log.position.line}:${log.position.column}) ${log.message}`,
					),
				);
			} else {
				console.log(color(`[${log.level}]: ${log.message}`));
			}
		}
	}

	return chunks;
};

const getHash = async (filePath: string): Promise<string> => {
	const file = Bun.file(filePath);
	const hasher = new Bun.CryptoHasher("sha256");

	const stream = file.stream();
	for await (const chunk of stream) {
		hasher.update(chunk);
	}
	const hashBuffer = hasher.digest();
	return Buffer.from(hashBuffer).toString("base64url");
};

type HashedEntry = { hash: string; path: string | undefined };
type FullHashedEntry = { hash: string; path: string };
const entryHasPath = (e: HashedEntry): e is FullHashedEntry => !!e.path;
const generateManifest = async () => {
	const timer = new Stopwatch();
	const files = new Glob(`${_dest}/**/[!_]*.js`).scan();

	const tasks = [];
	for await (const file of files) {
		tasks.push(
			(async () => {
				const hash = await getHash(file);
				return {
					hash,
					path: file.split("wwwroot").at(-1)?.replaceAll("\\", "/"),
				};
			})(),
		);
	}
	const hashed = await Promise.all(tasks);
	const manifest = {
		GeneratedAt: new Date().toISOString(),
		Files: hashed
			.filter(entryHasPath)
			.toSorted((a, b) => a.path.localeCompare(b.path))
			.reduce(
				(acc, { path, hash }) => {
					acc[path] = hash;
					return acc;
				},
				{} as Record<string, string>,
			),
	};

	await Bun.write(join(_source, "generated", "manifest.json"), JSON.stringify(manifest, null, 2));

	const { time, unit } = timer.lap(3);
	console.log(ct`{dim Generated manifest.json in {reset.bold {underline ${time}} ${unit}}}`);
};

const compileAll = async () => {
	const timer = new Stopwatch();

	if (values.cleanAlways) {
		await clean();
	}

	const jsChunks = await compile(
		new Glob(`${_source}/src/**/[^_]*.{ts,js,tsx}`),
		join(_dest, "/"),
		"src",
		"Javascript",
	);

	console.log(ct`{dim Generating manifest.json}`);
	await generateManifest();
	const workersChunks = await compile(
		new Glob(`${_source}/src-workers/**/[^_]*.ts`),
		join(_dest, "workers"),
		"src-workers",
		"Workers",
	);

	const chunks = [...jsChunks, ...workersChunks];
	console.log(ct`{dim Writing _ModulePreloads.cshtml with {reset.bold.underline ${chunks.length}} chunks}`);
	await Bun.write(join(_root, "..", "..", "Pages", "Shared", "_ModulePreloads.cshtml"), chunks.join("\n"));

	const size = await dirsize(`${_dest}/**/[!_]*.js`);
	console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best").toString(3)}}}`);

	await Bun.write(join(_dest, ".gitkeep"), "");

	const { time, unit } = timer.lap(3);
	console.log(ct`{dim Files compiled in {reset.bold {underline ${time}} ${unit}}}`);
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
