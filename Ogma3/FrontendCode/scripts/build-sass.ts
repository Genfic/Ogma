import fs, { rm } from "node:fs/promises";
import * as path from "node:path";
import { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { transform } from "lightningcss";
import { initAsyncCompiler } from "sass-embedded";
import { cssTargets } from "./helpers/css-targets";
import { dirsize } from "./helpers/dirsize";
import { attempt, attemptSync } from "./helpers/function-helpers";
import { Logger } from "./helpers/logger";
import { hasExtension } from "./helpers/path";
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

const clean = async () => {
	console.log(ct`{bold.red 🗑️{dim Cleaning} ${_dest}}`);
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

	const { name: filename, base } = path.parse(file);

	const fileContent = await Bun.file(file).text();

	logger.verbose(`file ${fileContent.length} bytes`);

	const extraDirs = (await fs.readdir(_base, { withFileTypes: true }))
		.filter((v) => v.isDirectory())
		.map((v) => path.join(_base, v.name));

	const compileResult = await attempt(
		async () => {
			return await compiler.compileStringAsync(fileContent, {
				sourceMap: true,
				loadPaths: [_base, ...extraDirs],
			});
		},
		(error) => logger.verbose(error),
	);

	if (!compileResult) {
		logger.log("Compilation error");
		return;
	}

	const { css, sourceMap } = compileResult;

	logger.verbose(css.length);

	const transformResult = values.raw
		? { code: css, map: JSON.stringify(sourceMap), warnings: [] }
		: attemptSync(
				() => {
					return transform({
						projectRoot: _dest,
						code: encoder.encode(css),
						inputSourceMap: JSON.stringify(sourceMap),
						sourceMap: true,
						filename: file,
						targets: cssTargets,
						minify: true,
					});
				},
				(error) => logger.verbose(error),
			);

	if (!transformResult) {
		logger.log("Transformation error");
		return;
	}

	const { code, map, warnings } = transformResult;

	logger.verbose(code.length);

	for (const { message, loc } of warnings) {
		logger.warn(ct`{yellow [{bold ${filename}}] WRN: ${message} at ${loc.filename} : ${loc.line}:${loc.column}}`);
	}

	await Bun.write(path.join(_dest, `${filename}.css`), code);
	if (map) {
		await Bun.write(path.join(_dest, `${filename}.map.css`), map);
	}

	logger.log(ct`{dim File {reset.bold ${base}} compiled in}`, timer);
};

const compileAll = async () => {
	const timer = new Stopwatch();
	const logger = new Logger();
	const files = [...new Glob(`${_base}/[!_]*.scss`).scanSync()];

	logger.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const tasks = [];
	for (const file of files) {
		logger.verbose(`Compiling ${file}`);
		tasks.push(compileSass(file));
	}
	const res = await Promise.allSettled(tasks);

	logger.verbose(res.map((r) => (r.status === "fulfilled" ? "ok" : JSON.stringify(r, null, 4))));

	const fulfilled = res.filter((r) => r.status === "fulfilled").length;
	const color = fulfilled === files.length ? c.green : c.red;
	logger.log(ct`{bold compiled ${color(ct`{underline ${fulfilled}} of {underline ${files.length}}`)} files}`);
	logger.log(ct`{bold Total compilation took}`, timer);
};

await compileAll();

const size = await dirsize(`${_dest}/**/[!_]*.css`);
console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best").toString(3)}}}`);

if (values.watch) {
	await watch(_base, ["update"], {
		transformer: (events) => events.find(({ path }) => hasExtension(path, "scss")),
		predicate: (event) => !!event,
		action: async (_) => await compileAll(),
	});
} else {
	process.exit(0);
}
