import { Option, program } from "@commander-js/extra-typings";
import multi from "@rollup/plugin-multi-entry";
import resolve from "@rollup/plugin-node-resolve";
import { Glob } from "bun";
import ct from "chalk-template";
import convert from "convert";
import { rollup } from "rollup";
import esbuild from "rollup-plugin-esbuild";
import minifyHTML from "rollup-plugin-html-literals";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-f, --full", "Output full, unminified file", false)
	.option("-r, --release", "Build in release mode", false)
	.parse(Bun.argv)
	.opts();

const base = "./wwwroot/js";
const source = `${base}/src-webcomponents`;
const dest = `${base}/bundle`;

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${source}/**/[!_]*.ts`).scanSync()];
	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	await using bundle = await rollup({
		input: files,
		output: {
			file: `${dest}/components.js`,
			format: "esm",
			sourcemap: true,
		},
		plugins: [
			multi(),
			resolve(),
			!values.full && minifyHTML(),
			esbuild({
				tsconfig: `${base}/tsconfig.json`,
				treeShaking: true,
				minify: !values.full,
				legalComments: "eof",
			}),
		],
	});

	await bundle.write({
		file: `${dest}/components.js`,
		format: "umd",
		name: "components",
		sourcemap: true,
	});

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);
};

await compileAll();

if (values.watch) {
	await watch(source, {
		verbose: values.verbose ?? false,
		transformer: (events) =>
			events
				.filter((e) => e.type === "update")
				.filter((e) => hasExtension(e.path, "ts"))
				.filter((e) => !e.path.endsWith("~"))
				.map((e) => e.path),
		predicate: (files) => files.length > 0,
		action: async (_) => {
			await compileAll();
		},
	});
}
