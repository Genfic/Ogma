import { $, Glob } from "bun";
import c from "chalk";

const files = new Glob("./wwwroot/css/*.scss").scan();

const migrators = ["calc-interpolation", "color", "division", "module", "namespace", "strict-unary"];

for await (const file of files) {
	console.log(c.bold.green(`Migrating file: ${file}`));
	for (const migrator of migrators) {
		console.log(c.blue(`With migrator: ${migrator}`));
		const res = await $`bunx sass-migrator ${migrator} ${file} --migrate-deps`;
		console.log(res.text());
	}
}
