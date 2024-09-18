import { Glob } from "bun";
import { parseArgs } from "util";
import watcher from "@parcel/watcher";
import c from "ansi-colors";
import convert from "convert";
import { compile } from "sass";
import { browserslistToTargets, transform } from "lightningcss";
import browserslist from "browserslist";
import * as path from "node:path";

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

const encoder = new TextEncoder();

const base = "./wwwroot/css";
const dest = `${base}/dist`;

const compileSass = async (file: string) => {
	const start = Bun.nanoseconds();

	const { name: filename, base } = path.parse(file);

	const { css, sourceMap } = compile(file, {
		sourceMap: true,
	});

	const { code, map, warnings } = transform({
		code: encoder.encode(css),
		inputSourceMap: JSON.stringify(sourceMap),
		sourceMap: true,
		filename: file,
		targets: browserslistToTargets(browserslist("defaults")),
		minify: true,
	});

	for (const warning of warnings) {
		console.warn(c.yellow(`[${c.bold(filename)}] WRN: ${warning}`));
	}

	await Bun.write(path.join(dest, `${filename}.css`), code);
	if (map) {
		await Bun.write(path.join(dest, `${filename}.map.css`), map);
	}

	const unit = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(`${c.dim("File")} ${c.bold(base)} ${c.dim("compiled in")} ${c.bold(unit.quantity.toFixed(2))} ${c.bold(unit.unit)}`);
};

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${base}/*.scss`).scanSync()];
	console.log(c.green(`⚙ Compiling ${c.bold(files.length.toString())} files`));

	const tasks = [];
	for (const file of files) {
		tasks.push(compileSass(file));
	}
	await Promise.allSettled(tasks);

	const unit = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(c.bold(`Total compilation took ${c.bold.green(unit.quantity.toFixed(2))} ${c.bold.green(unit.unit)}\n`));
};

await compileAll();

if (values.watch) {
	console.log(c.blue("👀 Watching..."));

	const subscription = await watcher.subscribe(base, async (err, events) => {
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

		if (events.find(({ type, path }) => type === "update" && path.endsWith("scss"))) {
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
