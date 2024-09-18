import { Glob } from "bun";
import { parseArgs } from "util";
import watcher from "@parcel/watcher";
import c from "ansi-colors";
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

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${source}/**/*.{js,ts}`).scanSync()];
	console.log(c.green(`⚙ Compiling ${c.bold(files.length.toString())} files`));

	const result = await Bun.build({
		entrypoints: files,
		outdir: dest,
		root: source,
		minify: true,
		sourcemap: "linked",
		splitting: false,
	});

	const unit = convert(Bun.nanoseconds() - start, "ns").to("best");
	if (result.success) {
		console.log(c.bold(`Total compilation took ${c.bold.green(unit.quantity.toFixed(2))} ${c.bold.green(unit.unit)}\n`));
	} else {
		console.log(c.bold.red(`Compilation failed! (${c.dim(`${c.bold.green(unit.quantity.toFixed(2))} ${c.bold.green(unit.unit)}`)})`));
		for (const log of result.logs.filter(l => ["error", "warning"].includes(l.level))) {
			const color = log.level === "error" ? c.red : c.yellow;
			if (log.position) {
				console.log(color(`[${log.level}]: ${log.position.file} (${log.position.line}:${log.position.column}) ${log.message}`));
			} else {
				console.log(color(`[${log.level}]: ${log.message}`));
			}
		}
	}
};

await compileAll();

if (values.watch) {
	console.log(c.blue("👀 Watching..."));

	const subscription = await watcher.subscribe(source, async (err, events) => {
		if (values.verbose) {
			for (const { type, path } of events) {
				console.info(c.bgYellow(`${type}: ${path}`));
			}
		}

		if (err) {
			console.error(c.bgRed(err.message));
			values.verbose && console.error(err);
			return;
		}

		if (events.find(({ type, path }) => type === "update" && hasExtension(path, "ts", "js"))) {
			console.log(c.blueBright("🔔 Files changed, recompiling!"));
			await compileAll();
		}
	});

	process.on("SIGINT", async () => {
		// close watcher when Ctrl-C is pressed
		console.log("🚪 Closing watcher...");
		await subscription.unsubscribe();
		process.exit(0);
	});
}
