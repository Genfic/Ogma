import { dirname, join } from "node:path";
import ct from "chalk-template";
import * as cheerio from "cheerio";
import ejs from "ejs";
import { lowerCase, pascalCase } from "es-toolkit";
import { fetchIcon } from "./helpers/icons";
import { findAllTemplates } from "./helpers/template-helpers";

const _root = dirname(Bun.main);
const templates = await findAllTemplates();

console.log(ct`{bold Enter icon names to download}`);

for await (const line of console) {
	const res = await fetchIcon(line);

	if (!res) {
		console.log(ct`{red Icon not found: ${line}}`);
		continue;
	}

	const name = pascalCase(res.name).replace(":", "");
	const title = lowerCase(res.name).replace(":", "");

	const ico = ejs.render(templates.get("icon"), {
		icon: cheerio.load(res.svg, { xml: true })("svg").html() as string,
		name,
		title,
	});

	console.log(ct`{green Downloaded ${name}}`);

	await Bun.write(join(_root, "..", "typescript", "src", "icons", `${name}.tsx`), ico);
}
