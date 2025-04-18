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

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-r, --release", "Build in release mode", false)
	.option("-c, --clean", "Clean output directory", false)
	.parse(Bun.argv)
	.opts();

const _root = dirname(Bun.main);
const _source = join(_root, "..", "typescript", "src-solid");
const _dest = join(_root, "..", "..", "wwwroot", "js", "comp");

if (values.clean) {
	await rm(_dest, { recursive: true, force: true });
}

const compileAll = async () => {
	const start = Bun.nanoseconds();

	const { success, logs, outputs } = await Bun.build({
		entrypoints: [...new Glob(`${_source}/**/[!_]*.tsx`).scanSync()],
		outdir: _dest,
		root: _source,
		minify: true,
		sourcemap: "external",
		splitting: true,
		plugins: [SolidPlugin()],
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
	});

	const chunks = outputs
		.filter((o) => o.kind === "chunk")
		.map((c) => c.path.split("wwwroot").at(-1)?.replaceAll("\\", "/"))
		.filter((s) => typeof s === "string")
		.map((p) => `<link rel="modulepreload" href="~${p}" as="script" />`);

	await Bun.write(join(_root, "..", "..", "Pages", "Shared", "_ModulePreloads.cshtml"), chunks.join("\n"));

	const time = convert(Bun.nanoseconds() - start, "ns")
		.to("best")
		.toString(3);
	if (success) {
		console.log(ct`{dim Files compiled in {reset.bold {underline ${time}}}}`);
	} else {
		console.error(ct`{red Build of files failed after {reset.bold {underline ${time}}}}`);
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

	const size = await dirsize(`${_dest}/**/[!_]*.js`);
	console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best").toString(3)}}}`);
};

await compileAll();

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
