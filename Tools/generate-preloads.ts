const path = new URL("../Ogma3/wwwroot/fonts/", import.meta.url);
for await (const f of Deno.readDir(path)) {
	console.log(`<link rel="preload" as="font" crossorigin="anonymous" href="~/fonts/${f.name}">`);
}