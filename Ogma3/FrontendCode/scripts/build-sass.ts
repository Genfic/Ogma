import fs, { rm } from "node:fs/promises";
import { dirname, join, parse } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { attempt, attemptAsync } from "es-toolkit";
import { transform } from "lightningcss";
import { initAsyncCompiler } from "sass-embedded";
import { cssTargets } from "./helpers/css-targets";
import { dirsize } from "./helpers/dirsize";
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

const extraDirs = (await fs.readdir(_base, { withFileTypes: true }))
	.filter((v) => v.isDirectory())
	.map((v) => join(_base, v.name));

const compileSass = async (file: string) => {
	const timer = new Stopwatch();
	const logger = new Logger();

	const { name: filename, base } = parse(file);
	logger.verbose(`Compiling ${file}`);

	const size = Bun.file(file).size;
	logger.verbose(`File size: ${size} bytes`);

	const [compileError, compileResult] = await attemptAsync(async () => {
		return await compiler.compileAsync(file, {
			sourceMap: true,
			sourceMapIncludeSources: true,
			loadPaths: [_base, ...extraDirs],
		});
	});

	if (!compileResult) {
		logger.log(`Sass compilation error: ${compileError}`);
		return;
	}

	const { css, sourceMap } = compileResult;

	if (values.raw) {
		await Bun.write(join(_dest, `${filename}.raw.css`), css);
	}

	logger.verbose(`Sass compiled: ${css.length} bytes`);

	const outFile = `${filename}.css`;

	const [error, result] = attempt(() =>
		transform({
			projectRoot: projectRoot,
			code: encoder.encode(css),
			inputSourceMap: sourceMap ? JSON.stringify(sourceMap) : undefined,
			sourceMap: true,
			filename: outFile,
			targets: cssTargets,
			minify: true,
		}),
	);

	if (!result) {
		logger.log(`LightningCSS transformation error: ${error}`);
		return;
	}

	const { code, map, warnings } = result;

	logger.verbose(`LightningCSS output: ${code.length} bytes`);
	logger.verbose(`Warnings: ${warnings.length}`);
	for (const { message, loc } of warnings) {
		logger.warn(ct`{yellow [{bold ${filename}}] WRN: ${message} at ${loc.filename}:${loc.line}:${loc.column}}`);
	}

	const mapPath = join(_dest, `${outFile}.map`);
	const fullCode = Buffer.concat([code, encoder.encode(`\n/*# sourceMappingURL=/css/${outFile}.map */`)]);

	await Bun.write(join(_dest, outFile), fullCode);

	if (map) {
		await Bun.write(mapPath, map);
	}

	await Bun.write(join(_dest, ".gitkeep"), "");

	logger.log(ct`{dim File {reset.bold ${base}} compiled in}`, timer);
};

const compileAll = async () => {
	const timer = new Stopwatch();
	const logger = new Logger();
	const files = [...new Glob(`${_base}/[!_]*.scss`).scanSync(), ...new Glob(`${_base}/themes/[!_]*.scss`).scanSync()];

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
