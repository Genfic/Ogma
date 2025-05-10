import path, { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { dirsize } from "./helpers/dirsize";
import { log } from "./helpers/logger";
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
const _source = join(_root, "..", "typescript", "src");
const _dest = join(_root, "..", "..", "wwwroot", "js");

if (values.clean) {
	await rm(_dest, { recursive: true, force: true });
}

const compileFile = async (file: string) => {
	const start = Bun.nanoseconds();
	const { base } = path.parse(file);

	const { success, logs } = await Bun.build({
		entrypoints: [file],
		outdir: _dest,
		root: _source,
		minify: true,
		sourcemap: "external",
		splitting: true,
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
	});

	const time = convert(Bun.nanoseconds() - start, "ns")
		.to("best")
		.toString(3);
	if (success) {
		console.log(ct`{dim File {reset.bold ${base}} compiled in {reset.bold {underline ${time}}}}`);
	} else {
		console.error(ct`{red Build of {reset.bold ${base}} failed after {reset.bold {underline ${time}}}}`);
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

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${_source}/**/*.{js,ts}`).scanSync()];
	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const tasks = [];
	for (const file of files) {
		log.verbose(`Compiling ${file}`);
		tasks.push(compileFile(file));
	}
	const res = await Promise.allSettled(tasks);

	log.verbose(res.map((r) => r.status));

	const time = convert(Bun.nanoseconds() - start, "ns")
		.to("best")
		.toString(3);

	const fulfilled = res.filter((r) => r.status === "fulfilled").length;
	const color = fulfilled === files.length ? c.green : c.red;
	console.log(ct`{bold compiled ${color(ct`{underline ${fulfilled}} of {underline ${files.length}}`)} files}`);

	if (fulfilled !== files.length) {
		console.log(ct`{bold.yellow Run again with {dim --verbose} for more info}`);
	}
	console.log(ct`{bold Total compilation took {green {underline ${time}}}}\n`);

	const size = await dirsize(`${_dest}/**/[!_]*.js`);
	console.log(ct`{green Total size: {bold.underline ${convert(size, "bytes").to("best").toString(3)}}}`);
};

await compileAll();

if (values.watch) {
	await watch(_source, ["update"], {
		transformer: (events) => events.filter(({ path }) => hasExtension(path, "ts", "js")).map((e) => e.path),
		predicate: (files) => files.length > 0,
		action: async (files) => {
			for (const p of files) {
				const { base } = path.parse(p);
				console.log(ct`{blueBright 🔔 File {bold ${base}} changed, recompiling...}`);
				await compileFile(p);
			}
		},
	});
}
