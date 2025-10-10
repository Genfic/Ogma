import { $, serve } from "bun";
import ejs from "ejs";
import { compact } from "es-toolkit";
import { generateDiffHtml } from "./helpers/diff-parser";
import { findAllTemplates } from "./helpers/template-helpers";
import type { BiomeDiag } from "./types/biome-diag";

await $`bunx biome check . --reporter=json > biome.diag.json`.quiet().nothrow();

const diag: BiomeDiag = await Bun.file("biome.diag.json").json();
const templates = await findAllTemplates();

const page = ejs.render(templates.get("biome-report"), { diag, compact, generateDiffHtml });

const server = serve({
	routes: {
		"/": () => new Response(page, { headers: { "Content-Type": "text/html" } }),
	},
});

console.log(`Running on ${server.url.href}`);
