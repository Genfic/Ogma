import { dirname, join } from "node:path";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import convert from "convert";
import dedent from "dedent";
import { compact, uniq } from "es-toolkit";
import { parse } from "json5";

const start = Bun.nanoseconds();

const _root = dirname(Bun.main);
const json = await Bun.file(join(_root, "..", "..", "seed.json5")).text();
const seed = parse(json) as { Icons: string[]; AdditionalIcons: string[] };

const foundIcons: (string | null)[] = [];

const extractor = new HTMLRewriter().on("icon[icon]:not([dynamic])", {
	element(el) {
		foundIcons.push(el.getAttribute("icon"));
	},
});

for await (const file of new Glob(join(_root, "..", "..", "**", "*.cshtml")).scan()) {
	const content = await Bun.file(file).text();
	extractor.transform(content);
}

const icons: string[] = uniq([...seed.Icons, ...seed.AdditionalIcons, ...compact(foundIcons)]);

interface Svg {
	svg: string;
	name: string;
}

const fetchIcon = async (icon: string): Promise<Svg | null> => {
	const [set, name] = icon.split(":");
	const res = await fetch(`https://api.iconify.design/${set}/${name}.svg`);

	if (res.ok) {
		const svg = await res.text();
		console.log(ct`{green Fetched icon: ${icon}}`);
		return { svg: svg.trim(), name: icon };
	}

	console.error(ct`{red Failed to fetch icon: ${icon}}`);
	return null;
};

const res = await Promise.all(icons.map((icon) => fetchIcon(icon)));

const svgs = compact(res).toSorted((a, b) => a.name.localeCompare(b.name));

const color = svgs.length === icons.length ? c.green : c.red;
console.log(ct`{bold ${color(ct`Fetched {underline ${svgs.length}} of {underline ${icons.length}}`)} icons}`);

const rewriter = new HTMLRewriter().on('svg:not([id="spritesheet"])', {
	element(element: HTMLRewriterTypes.Element): void | Promise<void> {
		element.removeAndKeepContent();
	},
});

// language=svg
const tpl = dedent`
	<svg xmlns="http://www.w3.org/2000/svg" id="spritesheet" style="display: none">
		${svgs.map(({ svg, name }) => `<symbol id="${name}" viewBox="0 0 24 24">${svg}</symbol>`).join("\n\t\t")}
	</svg>
	`.trim();

const spritesheet = rewriter.transform(tpl);

await Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg"), spritesheet);
await Bun.write(join(_root, "..", "..", "Pages", "Shared", "_IconSheet.cshtml"), spritesheet);

console.log(ct`{green Created {reset.bold ./wwwroot/svg/spritesheet.svg}} and {bold ./Pages/Shared/IconSheet.cshtml}`);

const { quantity, unit } = convert(Bun.nanoseconds() - start, "ns").to("best");
console.log(ct`{bold Total compilation took {green {underline ${quantity.toFixed(2)}} ${unit}}}`);
