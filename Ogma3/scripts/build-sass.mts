import * as path from "node:path";
import { parseArgs } from "node:util";
import watcher from "@parcel/watcher";
import browserslist from "browserslist";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import { browserslistToTargets, transform } from "lightningcss";
import { compile } from "sass";
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
		console.warn(ct`{yellow [{bold ${filename}}] WRN: ${warning}}`);
	}

	await Bun.write(path.join(dest, `${filename}.css`), code);
	if (map) {
		await Bun.write(path.join(dest, `${filename}.map.css`), map);
	}

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{dim File {reset.bold ${base}} compiled in {reset.bold {underline ${quantity.toFixed(2)}} ${unit}}}`);
};

const compileAll = async () => {
	const start = Bun.nanoseconds();
	const files = [...new Glob(`${base}/*.scss`).scanSync()];
	console.log(ct`{green ⚙ Compiling {bold.underline ${files.length}} files}`);

	const tasks = [];
	for (const file of files) {
		values.verbose && console.info(`Compiling ${file}`);
		tasks.push(compileSass(file));
	}
	const res = await Promise.allSettled(tasks);

	values.verbose && console.log(res);

	const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
	console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}\n`);
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

		if (events.find(({ type, path }) => type === "update" && hasExtension(path, "scss"))) {
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
