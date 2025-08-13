import { dirname, join } from "node:path";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import ejs from "ejs";
import { compact, uniq } from "es-toolkit";
import { parse } from "json5";
import { brotliCompressSync } from "zlib";
import { alphaBy } from "./helpers/function-helpers";
import { Logger } from "./helpers/logger";
import { Parallel } from "./helpers/promises";
import { Stopwatch } from "./helpers/stopwatch";
import { findAllTemplates } from "./helpers/template-helpers";

const timer = new Stopwatch();
const logger = new Logger();

const values = program
	.option("-v, --verbose", "Verbose mode", false)
	.option("-s, --serve", "Serve the icon index", false)
	.option("-p, --port <port>", "Port on which to serve", (v) => Number.parseInt(v), 3000)
	.parse(Bun.argv)
	.opts();

const _root = dirname(Bun.main);
const json = await Bun.file(join(_root, "..", "..", "seed.json5")).text();
const seed = parse(json) as { Icons: string[]; AdditionalIcons: string[] };

const templates = await findAllTemplates();

const foundIcons: string[] = [];
const files: Record<string, string> = {};

const extractor = (file: string) =>
	new HTMLRewriter().on("icon[icon]:not([dynamic])", {
		element(el) {
			const ico = el.getAttribute("icon");
			if (!ico) {
				return;
			}
			foundIcons.push(ico);
			files[ico] = file;
		},
	});

for await (const file of new Glob(join(_root, "..", "..", "**", "*.cshtml")).scan()) {
	const content = await Bun.file(file).text();
	extractor(file).transform(content);
}

const icons: string[] = compact(uniq([...seed.Icons, ...seed.AdditionalIcons, ...foundIcons]));

interface Svg {
	svg: string;
	name: string;
}

const fetchIcon = async (icon: string): Promise<Svg | null> => {
	const [set, name] = icon.split(":");
	const res = await fetch(`https://api.iconify.design/${set}/${name}.svg`);

	if (res.ok) {
		const svg = await res.text();
		logger.log(ct`{green Fetched icon: ${icon}}`);
		return { svg: svg.trim(), name: icon };
	}

	logger.error(
		ct`{red Failed to fetch icon: ${icon}}` + (files[icon] ? ct` in file {underline ${files[icon]}}` : ""),
	);
	return null;
};

const res = await Parallel.forEach(icons, fetchIcon);

logger.verbose(res);

const svgs = compact(res).toSorted(alphaBy((s) => s.name));

logger.verbose(svgs);

const color = svgs.length === icons.length ? c.green : c.red;
logger.log(ct`{bold ${color(ct`Fetched {underline ${svgs.length}} of {underline ${icons.length}}`)} icons}`);

const rewriter = new HTMLRewriter().on('svg:not([id="icon-spritesheet"])', {
	element(element: HTMLRewriterTypes.Element): void | Promise<void> {
		element.removeAndKeepContent();
	},
});

const tpl = ejs.render(templates.get("spritesheet"), { svgs });
const spritesheet = rewriter.transform(tpl);

const index = ejs.render(templates.get("icon-index"), { svgs });
await Promise.allSettled([
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg"), spritesheet),
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg.gz"), Bun.gzipSync(spritesheet)),
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg.br"), brotliCompressSync(spritesheet)),
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg.zst"), Bun.zstdCompressSync(spritesheet)),
	Bun.write(join(_root, "..", "..", "Pages", "Shared", "_IconSheet.cshtml"), spritesheet),
	Bun.write(join(_root, "..", "..", "wwwroot", "icon-index.html"), index),
]);

logger.log(ct`{green Created {reset.bold ./wwwroot/svg/spritesheet.svg}} and {bold ./Pages/Shared/IconSheet.cshtml}`);

logger.log(ct`{bold Total compilation took}`, timer);

if (values.serve) {
	const server = Bun.serve({
		port: values.port,
		fetch(_) {
			return new Response(index, { headers: { "Content-Type": "text/html" } });
		},
	});
	logger.log(ct`{green Serving icons at {bold ${server.url}}}`);
}
