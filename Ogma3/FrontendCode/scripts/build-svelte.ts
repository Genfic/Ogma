import { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import ct from "chalk-template";
import convert from "convert";
import { SveltePlugin } from "bun-plugin-svelte";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";
import { dirsize } from "./helpers/dirsize";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-f, --full", "Output full, unminified file", false)
	.option("-r, --release", "Build in release mode", false)
	.parse(Bun.argv)
	.opts();

const _root = dirname(Bun.main);
const _source = join(_root, "..", "typescript", "svelte-webcomponents");
const _dest = join(_root, "..", "..", "wwwroot", "js", "bundle2");

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${_source}/**/[!_]*.svelte`).scanSync()];
	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const res = await Bun.build({
		entrypoints: files,
		root: _source,
		outdir: _dest,
		format: "esm",
		sourcemap: "linked",
		packages: "bundle",
		splitting: true,
		minify: !values.full,
		plugins: [SveltePlugin()],
	});

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);
};

await compileAll();

const size = await dirsize(`${_dest}/**/[!_]*.{js,css}`);
console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best")}}}`);

if (values.watch) {
	await watch(_source, {
		transformer: (events) =>
			events
				.filter((e) => e.type === "update")
				.filter((e) => hasExtension(e.path, "svelte"))
				.filter((e) => !e.path.endsWith("~"))
				.map((e) => e.path),
		predicate: (files) => files.length > 0,
		action: async (_) => {
			await compileAll();
		},
	});
}
