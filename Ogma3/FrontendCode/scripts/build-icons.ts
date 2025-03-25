import { render } from "@lit-labs/ssr";
import { type RenderResult, collectResultSync } from "@lit-labs/ssr/lib/render-result";
import { Glob } from "bun";
import ct from "chalk-template";
import * as cheerio from "cheerio";
import convert from "convert";
import { compact, flow, uniq } from "es-toolkit";
import format from "html-format";
import { parse } from "json5";
import { type TemplateResult, html } from "lit";
import { unsafeSVG } from "lit/directives/unsafe-svg.js";
import { dirname, join } from "node:path";

const start = Bun.nanoseconds();

const _root = dirname(Bun.main);
const json = await Bun.file(join(_root, "..", "..", "seed.json5")).text();
const seed = parse(json) as { Icons: string[]; AdditionalIcons: string[] };

const icons: string[] = [...seed.Icons, ...seed.AdditionalIcons];

for await (const file of new Glob(join(_root, "..", "..", "**", "*.cshtml")).scan()) {
	const content = await Bun.file(file).text();
	const dom = cheerio.load(content);
	const found = dom.extract({
		icons: [
			{
				selector: "icon[icon]:not([dynamic])",
				value: "icon",
			},
		],
	});
	icons.push(...found.icons);
}

const fetchIcon = async (icon: string) => {
	const [set, name] = icon.split(":");
	const res = await fetch(`https://api.iconify.design/${set}/${name}.svg`);

	if (res.ok) {
		const svg = await res.text();
		console.log(ct`{green Fetched icon: ${icon}}`);
		return { svg, name: icon };
	}

	console.error(ct`{red Failed to fetch icon: ${icon}}`);
	return null;
};

const res = await Promise.all(uniq(icons).map((icon) => fetchIcon(icon)));

const svgs = compact(res);

console.log(ct`{green Fetched {bold ${svgs.length}}} icons`);

const rewriter = new HTMLRewriter()
	.on("*", {
		comments(comment: HTMLRewriterTypes.Comment): void | Promise<void> {
			comment.remove();
		},
	})
	.on('svg:not([id="spritesheet"])', {
		element(element: HTMLRewriterTypes.Element): void | Promise<void> {
			element.removeAndKeepContent();
		},
	});

const tpl /* lang=svg */ = html`
	<svg xmlns="http://www.w3.org/2000/svg" id="spritesheet" style="display: none">
		${svgs
			.toSorted((a, b) => a.name.localeCompare(b.name))
			.map(
				({ svg, name }) => html`
					<symbol id="${name}" viewBox="0 0 24 24">${unsafeSVG(svg)}</symbol>
				`,
			)}
	</svg>
`;

const spritesheet = flow(
	(x: TemplateResult<1>) => render(x),
	(x: RenderResult) => collectResultSync(x),
	(x: string) => rewriter.transform(x),
	(x: string) => format(x, "\t", Number.MAX_VALUE),
	(x: string) => x.replaceAll(/<!--(.+)-->/g, ""),
	(x: string) => x.replaceAll("\n\n", "\n"),
	(x: string) => x.trim(),
)(tpl);

await Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg"), spritesheet);
await Bun.write(join(_root, "..", "..", "Pages", "Shared", "_IconSheet.cshtml"), spritesheet);

console.log(ct`{green Created {reset.bold ./wwwroot/svg/spritesheet.svg}} and {bold ./Pages/Shared/IconSheet.cshtml}`);

const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}`);
