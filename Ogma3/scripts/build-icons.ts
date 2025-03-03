import { parseArgs } from "node:util";
import { render } from "@lit-labs/ssr";
import { collectResultSync } from "@lit-labs/ssr/lib/render-result";
import { type BunFile, Glob } from "bun";
import ct from "chalk-template";
import * as cheerio from "cheerio";
import convert from "convert";
import { uniq } from "es-toolkit";
import { parse } from "json5";
import { unsafeSVG } from "lit/directives/unsafe-svg.js";
import { html } from "lit";

const { values } = parseArgs({
	args: Bun.argv,
	options: {
		verbose: {
			type: "boolean",
		},
	},
	allowPositionals: true,
});

const log = (...data: unknown[]) => values.verbose && console.log(data);

const start = Bun.nanoseconds();

const handlers: BunFile[] = [...new Glob("../**/*.cshtml").scanSync()].map((f) => Bun.file(f));

const json = await Bun.file("./seed.json5").text();
const seed = parse(json) as { Icons: string[]; AdditionalIcons: string[] };

const icons: string[] = [...seed.Icons, ...seed.AdditionalIcons];

for (const handler of handlers) {
	const content = await handler.text();
	const dom = cheerio.load(content);
	const found = dom.extract({
		icons: [
			{
				selector: "icon[icon]",
				value: "icon",
			},
		],
	});
	icons.push(...found.icons);
}

const svgs: { svg: string; name: string }[] = [];

for (const icon of uniq(icons)) {
	log(ct`{dim Fetching icon {reset.bold ${icon}}}`);
	const [set, name] = icon.split(":");
	const res = await fetch(`https://api.iconify.design/${set}/${name}.svg`);
	if (res.ok) {
		const svg = await res.text();
		svgs.push({ svg, name: icon });
		console.log(ct`{green Fetched icon: ${icon}}`);
	} else {
		console.error(ct`{red Failed to fetch icon: ${icon}}`);
	}
}

console.log(ct`{green Fetched {bold ${svgs.length}}} icons`);

const rewriter = new HTMLRewriter().on("*", {
	comments(comment: HTMLRewriterTypes.Comment): void | Promise<void> {
		comment.remove();
	},
	element(element: HTMLRewriterTypes.Element): void | Promise<void> {
		if (element.tagName !== "svg" || element.getAttribute("id") === "spritesheet") {
			return;
		}
		element.removeAndKeepContent();
	},
});

const tpl /* lang=svg */ = html`
	<svg xmlns="http://www.w3.org/2000/svg" id="spritesheet" style="display: none">
		${svgs.map(
			({ svg, name }) => html`
			<symbol id="${name}" viewBox="0 0 24 24">${unsafeSVG(svg)}</symbol>
		`,
		)}
	</svg>
	`;

const spritesheet = rewriter.transform(collectResultSync(render(tpl)));

await Bun.write("./wwwroot/svg/spritesheet.svg", spritesheet);
await Bun.write("./Pages/Shared/_IconSheet.cshtml", spritesheet);

console.log(ct`{green Created {reset.bold ./wwwroot/svg/spritesheet.svg}} and {bold ./Pages/Shared/IconSheet.cshtml}`);

const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}`);
