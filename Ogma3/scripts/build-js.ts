import path from "node:path";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { hasExtension } from "./helpers/path";
import { watch } from "./helpers/watcher";
import { log } from "./helpers/logger";
import { program } from "@commander-js/extra-typings";

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-w, --watch", "Watch mode", false)
	.option("-r, --release", "Build in release mode", false)
	.parse(Bun.argv)
	.opts();

const base = "./wwwroot/js";
const source = `${base}/src`;
const dest = `${base}/dist`;

const compileFile = async (file: string) => {
	const start = Bun.nanoseconds();
	const { base } = path.parse(file);

	const { success, logs } = await Bun.build({
		entrypoints: [file],
		outdir: dest,
		root: source,
		minify: true,
		sourcemap: "linked",
		splitting: false,
		drop: values.release ? ["console", ...Object.keys(log).map((k) => `log.${k}`)] : undefined,
	});

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	if (success) {
		console.log(
			ct`{dim File {reset.bold ${base}} compiled in {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`,
		);
	} else {
		console.error(
			ct`{red Build of {reset.bold ${base}} failed after {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`,
		);
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
	const files = [...new Glob(`${source}/**/*.{js,ts}`).scanSync()];
	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const tasks = [];
	for (const file of files) {
		log.verbose(`Compiling ${file}`);
		tasks.push(compileFile(file));
	}
	const res = await Promise.allSettled(tasks);

	log.verbose(res.map((r) => r.status));

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
				.filter((e) => hasExtension(e.path, "ts", "js"))
				.filter((e) => !e.path.endsWith("~"))
				.map((e) => e.path),
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
