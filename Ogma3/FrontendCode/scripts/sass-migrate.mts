import { program } from "@commander-js/extra-typings";
import { $, Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import { compact } from "es-toolkit";
import { dirname, join, basename } from "node:path";

const values = program
	.option("--migrate", "Migrate SCSS files", false)
	.option("--vars", "Migrate SCSS variables", false)
	.parse(Bun.argv)
	.opts();

console.log(values);

if (values.migrate) {
	const files = new Glob(join(dirname(Bun.main), "..", "styles", "**", "*.scss")).scan();

	const migrators = ["calc-interpolation", "color", "division", "module", "namespace", "strict-unary"];

	for await (const file of files) {
		console.log(c.bold.green(`Migrating file: ${file}`));
		for (const migrator of migrators) {
			console.log(c.blue(`With migrator: ${migrator}`));
			const res = await $`bunx sass-migrator ${migrator} ${file} --migrate-deps`;
			console.log(res.text());
		}
	}

	process.exit();
}

if (values.vars) {
	const source = await Bun.file(join(dirname(Bun.main), "..", "styles", "src", "vars.scss")).text();
	const pairs = new Map(
		source
			.split("\n")
			.filter((l) => l.includes("var(--") && l.startsWith("$"))
			.map((l) => l.replace(";", "").split(":"))
			.filter((l) => l.length === 2)
			.map((l) => [l[0].trim(), l[1].trim()]),
	);

	console.log(pairs);

	console.log(ct`{green Found {bold ${pairs.size}} source variables}`);

	const files = new Glob(join(dirname(Bun.main), "..", "styles", "**", "*.scss")).scan();

	for await (const file of files) {
		console.log(ct`Migrating file: {bold.blue ${file}}`);
		const contents = await Bun.file(file).text();

		const vars = [...contents.matchAll(/v\.(?<name>\$[\w_-]+)/gi)].flatMap((v) =>
			Object.entries(v.groups ?? {})
				.filter(([k, _]) => k === "name")
				.map(([_, v]) => v),
		);

		if (vars.length === 0) {
			console.log(ct`{yellow No variables found}`);
			continue;
		}
		console.log(ct`{green Found {bold ${vars.length}} variables}`);

		let out = contents;
		for (const match of compact(vars)) {
			if (pairs.has(match)) {
				const name = match;
				const value = pairs.get(name);

				if (!value) continue;

				console.log(
					ct`Replacing {bold.blue ${name}} with {bold.blue ${value}} in {bold.yellow ${basename(file)}}`,
				);
				out = out.replaceAll(name, value);
			} else {
				console.log(ct`Variable {bold.red ${match}} not found`);
			}
		}

		await Bun.write(file, out.replaceAll("v.var", "var"));
	}

	process.exit();
}
