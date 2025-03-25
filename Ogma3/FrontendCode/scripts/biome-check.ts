import { render } from "@lit-labs/ssr";
import { type RenderResult, collectResultSync } from "@lit-labs/ssr/lib/render-result";
import { $, serve } from "bun";
import { compact, flow } from "es-toolkit";
import { type TemplateResult, html } from "lit";
import type { BiomeDiag, Diff } from "./types/biome-diag";

await $`bunx @biomejs/biome check . --reporter=json > biome.diag.json`.quiet().nothrow();

const diag: BiomeDiag = await Bun.file("biome.diag.json").json();

const processDiff = ({ dictionary, ops }: Diff) => {
	for (const op of ops) {
	}
};

const tpl /* lang=svg */ = html`
	<!doctype html>
	<html lang="en">
	<head>
		<meta charset="UTF-8">
		 <meta name="viewport" content="width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">
		 <meta http-equiv="X-UA-Compatible" content="ie=edge">
		 <title>Document</title>
		<style>
			body {
				font-family: sans-serif;
			}
			main {
				display: flex;
				flex-direction: column;
				gap: 1rem;
			}
			.diag {
				display: flex;
				flex-direction: column;
				border: 1px solid black;
			}
			.emphasis {
				font-weight: bold;
				background-color: lightblue;
			}
		</style>
	</head>
	<body>
		<main>
			${diag.diagnostics.map(
				(d) => html`
				<div class="diag">
					<h2>${d.severity.toUpperCase()} ${d.category}</h2>
					<div>${d.location.path.file} | ${d.location.span?.join(", ")}</div>
					<h3>${d.description}</h3>
					<ul>${d.advices.advices.map(
						(a) =>
							html`
							<li>${compact(a.log ?? []).map((f) =>
								typeof f === "string"
									? f
									: html`
										<span>
											${f.map(
												(m) => html`
												<span class="${m.elements.join(" ").toLowerCase()}">${m.content}</span>
											`,
											)}
										</span>`,
							)}
							
								${a.diff && html`<pre>${a.diff.dictionary}</pre>`}
							</li>
						`,
					)}
					</ul>
				</div>
			`,
			)}
		</main>
	</body>
	</html>
`;

const page = flow(
	(x: TemplateResult<1>) => render(x),
	(x: RenderResult) => collectResultSync(x),
	(x: string) => x.trim(),
)(tpl);

const server = serve({
	routes: {
		"/": () => new Response(page, { headers: { "Content-Type": "text/html" } }),
	},
});

console.log(server.url.href);
