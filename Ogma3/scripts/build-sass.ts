import * as path from "node:path";
import browserslist from "browserslist";
import { Glob } from "bun";
import ct from "chalk-template";
import convert from "convert";
import { browserslistToTargets, transform } from "lightningcss";
import { initAsyncCompiler } from "sass-embedded";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";
import { program } from "@commander-js/extra-typings";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.parse(Bun.argv)
	.opts();

const log = (...data: unknown[]) => values.verbose && console.log(data);

const encoder = new TextEncoder();

const _base = "./wwwroot/css";
const _dest = `${_base}/dist`;

const compiler = await initAsyncCompiler();

const compileSass = async (file: string) => {
	const start = Bun.nanoseconds();

	const { name: filename, base } = path.parse(file);

	const fileContent = await Bun.file(file).text();

	const { css, sourceMap } = await compiler.compileStringAsync(fileContent, {
		sourceMap: true,
		loadPaths: [_base, `${_base}/src/`]
	});

	log(css.length);

	const { code, map, warnings } = transform({
		code: encoder.encode(css),
		inputSourceMap: JSON.stringify(sourceMap),
		sourceMap: true,
		filename: file,
		targets: browserslistToTargets(browserslist("defaults")),
		minify: true
	});

	log(code.length);

	for (const warning of warnings) {
		console.warn(ct`{yellow [{bold ${filename}}] WRN: ${warning}}`);
	}

	await Bun.write(path.join(_dest, `${filename}.css`), code);
	if (map) {
		await Bun.write(path.join(_dest, `${filename}.map.css`), map);
	}

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{dim File {reset.bold ${base}} compiled in {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`);
};

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${_base}/*.scss`).scanSync()];

	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const tasks = [];
	for (const file of files) {
		values.verbose && console.info(`Compiling ${file}`);
		tasks.push(compileSass(file));
	}
	const res = await Promise.allSettled(tasks);

	values.verbose && console.log(res);

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);

	await compiler.dispose();
};

await compileAll();

if (values.watch) {
	await watch(_base, {
		verbose: values.verbose ?? false,
		transformer: (events) => events.find(({ type, path }) => type === "update" && hasExtension(path, "scss")),
		predicate: (event) => !!event,
		action: async (_) => {
			await compileAll();
		}
	});
}
