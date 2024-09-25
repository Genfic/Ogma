import path from "node:path";
import { parseArgs } from "util";
import watcher from "@parcel/watcher";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { hasExtension } from "./helpers/path";

const { values } = parseArgs({
	args: Bun.argv,
	options: {
		watch: {
			type: "boolean",
		},
		verbose: {
			type: "boolean",
		},
	},
	strict: true,
	allowPositionals: true,
});
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
	});

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	if (success) {
		console.log(ct`{dim File {reset.bold ${base}} compiled in {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`);
	} else {
		console.error(ct`{red Build of {reset.bold ${base}} failed after {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`);
		for (const log of logs.filter((l) => ["error", "warning"].includes(l.level))) {
			const color = log.level === "error" ? c.red : c.yellow;
			if (log.position) {
				console.log(color(`[${log.level}]: ${log.position.file} (${log.position.line}:${log.position.column}) ${log.message}`));
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
		values.verbose && console.info(`Compiling ${file}`);
		tasks.push(compileFile(file));
	}
	const res = await Promise.allSettled(tasks);

	values.verbose && console.log(res.map((r) => r.status));

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);
};

await compileAll();

if (values.watch) {
	console.log(c.blue("👀 Watching..."));

	const subscription = await watcher.subscribe(source, async (err, events) => {
		if (values.verbose) {
			for (const { type, path } of events) {
				console.info(c.yellow(`${type}: ${path}`));
			}
		}

		if (err) {
			console.error(c.bgRed(err.message));
			values.verbose && console.error(err);
			return;
		}

		const triggerPaths = events
			.filter((e) => e.type === "update")
			.filter((e) => hasExtension(e.path, "ts", "js"))
			.filter((e) => !e.path.endsWith("~"))
			.map((e) => e.path);

		if (triggerPaths.length > 0) {
			values.verbose && console.info(c.yellow(`Changed files: ${triggerPaths.join(", ")}`));

			for (const p of triggerPaths) {
				const { base } = path.parse(p);
				console.log(ct`{blueBright 🔔 File {bold ${base}} changed, recompiling...}`);
				await compileFile(p);
			}
		}
	});

	process.on("SIGINT", async () => {
		// close watcher when Ctrl-C is pressed
		console.log("🚪 Closing watcher...");
		await subscription.unsubscribe();
		process.exit(0);
	});
}
