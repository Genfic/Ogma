import { dirname, join } from "node:path";
import { brotliCompressSync } from "node:zlib";
import { program } from "@commander-js/extra-typings";
import { Glob } from "bun";
import c from "chalk";
import ct from "chalk-template";
import * as cheerio from "cheerio";
import ejs from "ejs";
import { compact, uniqBy } from "es-toolkit";
import { parse } from "json5";
import { alphaBy } from "./helpers/function-helpers";
import { fetchIcon } from "./helpers/icons";
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

const brand = (icons: string[], brand: string) => icons.map((i) => ({ value: i, type: brand }));

const icons = compact([
	...brand(seed.Icons, "base"),
	...brand(seed.AdditionalIcons, "additional"),
	...brand(foundIcons, "found"),
]);

interface Svg {
	svg: string;
	name: string;
}

const res = await Parallel.forEach(icons, async ({ value: i, type }) => {
	const res = await fetchIcon(i);
	if (res) {
		logger.log(ct`{green Fetched icon: ${i}}`);
		return { ...res, type };
	}
	logger.error(ct`{red Failed to fetch icon: ${i}}` + (files[i] ? ct` in file {underline ${files[i]}}` : ""));
	return null;
});

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

const tpl = ejs.render(templates.get("spritesheet"), { svgs: svgs.filter((s) => s.type !== "found") });
const spritesheet = rewriter.transform(tpl);

const paths = svgs.map(
	(s): Svg => ({
		name: s.name,
		svg: cheerio.load(s.svg, { xmlMode: true })("svg").html() as string,
	}),
);
logger.verbose(paths);
const csharp = ejs.render(templates.get("icons-csharp"), { svgs: uniqBy(paths, (s) => s.name) });

const index = ejs.render(templates.get("icon-index"), { svgs: uniqBy(svgs, (s) => s.name) });
await Promise.allSettled([
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg"), spritesheet),
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg.gz"), Bun.gzipSync(spritesheet)),
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg.br"), brotliCompressSync(spritesheet)),
	Bun.write(join(_root, "..", "..", "wwwroot", "svg", "spritesheet.svg.zst"), Bun.zstdCompressSync(spritesheet)),
	Bun.write(join(_root, "..", "..", "Data", "Icons.g.cs"), csharp),
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
