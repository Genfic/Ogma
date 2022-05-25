// @ts-ignore
import { listenAndServe } from "https://deno.land/std@0.110.0/http/server.ts";

const base = new URL("../Ogma3/wwwroot/css/dist/", import.meta.url);

const addr = ':4001';

const dark = await Deno.readTextFile(new URL('dark.css', base));
const light = await Deno.readTextFile(new URL('light.css', base));

const parseCss = (css: string): { key: string; val: string }[] =>
	css
		.split('\n')
		.map((s) => s.trim())
		.filter((s) => s.startsWith('--'))
		.map((s) => {
			let split = s.split(':').map((s) => s.trim());
			return { key: split[0], val: split[1] };
		});

const body = new TextEncoder().encode(`
<style>${dark.replace(':root', '#dark')}</style>
<style>${light.replace(':root', '#light')}</style>
<style>
* {
	box-sizing: border-box;
	padding: 0;
	margin: 0;
}
main {
	display: flex;
	flex-direction: row;
}
section {
	width: 100%;
}
.color {
	padding: 1rem;
}
</style>
<main>
	<section id="dark" style="background-color: var(--background)">
		${parseCss(dark).map((c) => `<div class="color" style="background-color: ${c.val}">${c.key}</div>`).join('')}
	</section>
	<section id="light" style="background-color: var(--background)">
		${parseCss(light).map((c) => `<div class="color" style="background-color: ${c.val}">${c.key}</div>`).join('')}
	</section>
</main>`);

const handler = (request: Request): Response => {
	return new Response(body, {
		status: 200,
		headers: { 'content-type': 'text/html; charset=utf-8' },
	});
};

console.log(`HTTP webserver running. Access it at: http://localhost${addr}/`);
await listenAndServe(addr, handler);
