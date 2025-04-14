import path, { dirname, join } from "node:path";
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

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-r, --release", "Build in release mode", false)
	.parse(Bun.argv)
	.opts();

const _root = dirname(Bun.main);
const _source = join(_root, "..", "typescript", "src-solid");
const _dest = join(_root, "..", "..", "wwwroot", "js", "comp");

const compileAll = async () => {
	const start = Bun.nanoseconds();

	const { success, logs } = await Bun.build({
		entrypoints: [...new Glob(`${_source}/**/[!_]*.tsx`).scanSync()],
		outdir: _dest,
		root: _source,
		minify: true,
		sourcemap: "external",
		splitting: true,
		plugins: [SolidPlugin()],
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
	});

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	if (success) {
		console.log(ct`{dim Files compiled in {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`);
	} else {
		console.error(ct`{red Build of files failed after {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`);
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
};

await compileAll();

const size = await dirsize(`${_dest}/**/[!_]*.js`);
console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best")}}}`);

if (values.watch) {
	await watch(_source, {
		transformer: (events) =>
			events.filter(({ type, path }) => type === "update" && hasExtension(path, "tsx")).map((e) => e.path),
		predicate: (files) => files.length > 0,
		action: async (_) => {
			await compileAll();
		},
	});
}
