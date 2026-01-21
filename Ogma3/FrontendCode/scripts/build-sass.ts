import fs, { rm } from "node:fs/promises";
import { dirname, join, parse } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { attemptAsync } from "es-toolkit";
import { transform, type TransformResult } from "lightningcss";
import { initAsyncCompiler } from "sass-embedded";
import { cssTargets } from "./helpers/css-targets";
import { dirsize } from "./helpers/dirsize";
import { $try } from "./helpers/function-helpers";
import { Logger } from "./helpers/logger";
import { hasExtension } from "./helpers/path";
import { Parallel } from "./helpers/promises";
import { Stopwatch } from "./helpers/stopwatch";
import { watch } from "./helpers/watcher";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-c, --clean", "Clean output directory", false)
	.option("-r, --raw", "Output raw css", false)
	.parse(Bun.argv)
	.opts();

const encoder = new TextEncoder();

const root = dirname(Bun.main);
const _base = join(root, "..", "styles");
const _dest = join(root, "..", "..", "wwwroot", "css");

const projectRoot = join(root, "..", "..");

console.log(projectRoot);

const clean = async () => {
	console.log(ct`{bold.red 🗑️ {dim Cleaning} ${_dest}}`);
	await rm(_dest, { recursive: true, force: true });
};

if (values.clean) {
	await clean();
}

const compiler = await initAsyncCompiler();

process.on("SIGINT", async () => {
	await compiler.dispose();
	process.exit(0);
});

const compileSass = async (file: string) => {
	const timer = new Stopwatch();
	const logger = new Logger();

	const { name: filename, base } = parse(file);
	logger.verbose(`Compiling ${file}`);

	const fileContent = await Bun.file(file).stat();
	logger.verbose(`File size: ${fileContent.size} bytes`);

	const extraDirs = (await fs.readdir(_base, { withFileTypes: true }))
		.filter((v) => v.isDirectory())
		.map((v) => join(_base, v.name));

	const compileResult = await $try(async () => {
		return await compiler.compileAsync(file, {
			sourceMap: true,
			sourceMapIncludeSources: true,
			loadPaths: [_base, ...extraDirs],
		});
	}, logger.verbose);

	if (!compileResult) {
		logger.log("Sass compilation error");
		return;
	}

	const { css, sourceMap } = compileResult;

	logger.verbose(`Sass compiled: ${css.length} bytes`);

	const outFile = `${filename}.css`;

	const transformResult: Pick<TransformResult, "code" | "map" | "warnings"> | undefined = values.raw
		? { code: encoder.encode(css), map: encoder.encode(JSON.stringify(sourceMap)), warnings: [] }
		: $try(
				() =>
					transform({
						projectRoot: projectRoot,
						code: encoder.encode(css),
						inputSourceMap: sourceMap ? JSON.stringify(sourceMap) : undefined,
						sourceMap: true,
						filename: outFile,
						targets: cssTargets,
						minify: true,
					}),
				logger.verbose,
			);

	if (!transformResult) {
		logger.log("LightningCSS transformation error");
		return;
	}

	const { code, map, warnings } = transformResult;

	logger.verbose(`LightningCSS output: ${code.length} bytes`);
	logger.verbose(`Warnings: ${warnings.length}`);
	for (const { message, loc } of warnings) {
		logger.warn(ct`{yellow [{bold ${filename}}] WRN: ${message} at ${loc.filename}:${loc.line}:${loc.column}}`);
	}

	await Bun.write(join(_dest, outFile), code);

	if (map) {
		await Bun.write(join(_dest, `${outFile}.map`), map);
	}

	await Bun.write(join(_dest, ".gitkeep"), "");

	logger.log(ct`{dim File {reset.bold ${base}} compiled in}`, timer);
};

const compileAll = async () => {
	const timer = new Stopwatch();
	const logger = new Logger();
	const files = [...new Glob(`${_base}/[!_]*.scss`).scanSync()];

	logger.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const res = await Parallel.forEach(files, async (f) => {
		return await attemptAsync(async () => await compileSass(f));
	});

	logger.verbose(JSON.stringify(res, null, 4));

	const fulfilled = res.filter(([err, _]) => !err).length;
	const color = fulfilled === files.length ? c.green : c.red;
	logger.log(ct`{bold compiled ${color(ct`{underline ${fulfilled}} of {underline ${files.length}}`)} files}`);
	logger.log(ct`{bold Total compilation took}`, timer);
};

await compileAll();

const size = await dirsize(`${_dest}/**/[!_]*.css`);
const best = (num: number) => convert(num, "bytes").to("best").toString(3);
console.log(ct`{green Total size: {bold.underline ${best(size)}}}`);

if (values.watch) {
	await watch(_base, ["update"], {
		transformer: (events) => events.find(({ path }) => hasExtension(path, "scss")),
		predicate: (event) => !!event,
		action: async (_) => await compileAll(),
	});
} else {
	process.exit(0);
}
