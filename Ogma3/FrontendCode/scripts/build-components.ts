import { deserialize, serialize } from "bun:jsc";
import { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import multi from "@rollup/plugin-multi-entry";
import resolve from "@rollup/plugin-node-resolve";
import { Glob } from "bun";
import ct from "chalk-template";
import convert from "convert";
import { type RollupCache, rollup } from "rollup";
import esbuild from "rollup-plugin-esbuild";
import minifyHTML from "rollup-plugin-html-literals";
import tsConfigPaths from "rollup-plugin-tsconfig-paths";
import { dirsize } from "./helpers/dirsize";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-f, --full", "Output full, unminified file", false)
	.option("-r, --release", "Build in release mode", false)
	.parse(Bun.argv)
	.opts();

const _root = dirname(Bun.main);
const _source = join(_root, "..", "typescript", "src-webcomponents");
const _dest = join(_root, "..", "..", "wwwroot", "js", "bundle");
const _cacheFile = join(_root, ".cache", "components.cache");

const saveCache = async (cache: RollupCache | undefined) => {
	if (!cache) {
		return;
	}
	const bin = serialize(cache);
	await Bun.write(_cacheFile, bin);
};

const loadCache = async () => {
	if (!(await Bun.file(_cacheFile).exists())) {
		return undefined;
	}
	const bin = await Bun.file(_cacheFile).arrayBuffer();
	return deserialize(bin);
};

let cache: RollupCache | undefined = await loadCache();

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${_source}/**/[!_]*.ts`).scanSync()];
	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	await using bundle = await rollup({
		input: files,
		cache,
		plugins: [
			tsConfigPaths({
				tsConfigPath: join(_root, "..", "typescript", "tsconfig.json"),
			}),
			multi(),
			resolve(),
			!values.full && minifyHTML(),
			esbuild({
				tsconfig: join(_root, "..", "typescript", "tsconfig.json"),
				minify: !values.full,
				legalComments: "eof",
			}),
		],
	});

	cache = bundle.cache;
	await saveCache(cache);

	await bundle.write({
		file: join(_dest, "components.js"),
		format: "esm",
		name: "components",
		sourcemap: true,
	});

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);
};

await compileAll();

const size = await dirsize(`${_dest}/**/[!_]*.js`);
console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best")}}}`);

if (values.watch) {
	await watch(_source, {
		transformer: (events) =>
			events.filter(({ type, path }) => type === "update" && hasExtension(path, "ts")).map((e) => e.path),
		predicate: (files) => files.length > 0,
		action: async (_) => {
			await compileAll();
		},
	});
}
