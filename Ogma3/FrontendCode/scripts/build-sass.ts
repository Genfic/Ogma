import * as path from "node:path";
import { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import browserslist from "browserslist";
import { Glob } from "bun";
import ct from "chalk-template";
import c from "chalk";
import convert from "convert";
import { browserslistToTargets, transform } from "lightningcss";
import { initAsyncCompiler } from "sass-embedded";
import { log } from "./helpers/logger";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";
import { readdir } from "node:fs/promises";
import { attempt, attemptSync } from "./helpers/function-helpers";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.parse(Bun.argv)
	.opts();

const encoder = new TextEncoder();

const root = dirname(Bun.main);
const _base = join(root, "..", "styles");
const _dest = join(root, "..", "..", "wwwroot", "css");

const compiler = await initAsyncCompiler();
const targets = browserslistToTargets(browserslist("last 2 years and > 0.1% and not dead"));

const compileSass = async (file: string) => {
	const start = Bun.nanoseconds();

	const { name: filename, base } = path.parse(file);

	const fileContent = await Bun.file(file).text();

	log.verbose(`file ${fileContent.length} bytes`);

	const extraDirs = (await readdir(_base, { withFileTypes: true }))
		.filter((v) => v.isDirectory())
		.map((v) => path.join(_base, v.name));

	const compileResult = await attempt(
		async () => {
			return await compiler.compileStringAsync(fileContent, {
				sourceMap: true,
				loadPaths: [_base, ...extraDirs],
			});
		},
		(error) => log.verbose(error),
	);

	if (!compileResult) {
		console.log("Compilation error");
		return;
	}

	const { css, sourceMap } = compileResult;

	log.verbose(css.length);

	const transformResult = attemptSync(
		() => {
			return transform({
				code: encoder.encode(css),
				inputSourceMap: JSON.stringify(sourceMap),
				sourceMap: true,
				filename: file,
				targets,
				minify: true,
			});
		},
		(error) => log.verbose(error),
	);

	if (!transformResult) {
		console.log("Transformation error");
		return;
	}

	const { code, map, warnings } = transformResult;

	log.verbose(code.length);

	for (const warning of warnings) {
		console.warn(
			ct`{yellow [{bold ${filename}}] WRN: ${warning.message} at ${warning.loc.filename} : ${warning.loc.line}:${warning.loc.column}}`,
		);
	}

	await Bun.write(path.join(_dest, `${filename}.css`), code);
	if (map) {
		await Bun.write(path.join(_dest, `${filename}.map.css`), map);
	}

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(
		ct`{dim File {reset.bold ${base}} compiled in {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`,
	);
};

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${_base}/[!_]*.scss`).scanSync()];

	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const tasks = [];
	for (const file of files) {
		values.verbose && console.info(`Compiling ${file}`);
		tasks.push(compileSass(file));
	}
	const res = await Promise.allSettled(tasks);

	log.verbose(
		res.map((r) => {
			if (r.status === "fulfilled") {
				return "ok";
			}

			return {
				status: r.status,
				file: r.reason.fileName,
				loc: `line ${r.reason?.loc?.line}, column ${r.reason?.loc?.column}`,
				type: r.reason?.data?.type,
				r: JSON.stringify(r, null, 4),
			};
		}),
	);

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");

	const fulfilled = res.filter((r) => r.status === "fulfilled").length;
	const color = fulfilled === files.length ? c.green : c.red;
	console.log(ct`{bold compiled ${color(ct`{underline ${fulfilled}} of {underline ${files.length}}`)} files}`);
	if (fulfilled !== files.length) {
		console.log(ct`{bold.yellow Run again with {dim --verbose} for more info}`);
	}
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);
};

await compileAll();

if (values.watch) {
	await watch(_base, {
		transformer: (events) => events.find(({ type, path }) => type === "update" && hasExtension(path, "scss")),
		predicate: (event) => !!event,
		action: async (_) => {
			await compileAll();
		},
	});
}

process.on("SIGINT", async () => {
	await compiler.dispose();
	process.exit(0);
});
