const priorities= new Map<string, number>([
	['woff2', 1],
	['woff', 2],
	['otf', 3],
	['ttf', 4]
]);

let tags: { tag: string, priority: number }[] = [];

const path = new URL("../Ogma3/wwwroot/fonts/", import.meta.url);
for await (const f of Deno.readDir(path)) {
	const name = f.name.split('.')[1];
	tags.push({ tag: f.name, priority: priorities.get(name) ?? 0 });
}

tags.sort((a, b) => a.priority - b.priority);
tags.sort((a,b) => (a.tag > b.tag) ? 1 : ((b.tag > a.tag) ? -1 : 0)).reverse();

for (const t of tags) {
	console.log(`url('/fonts/${t.tag}') format('${t.tag.split('.')[1]}'),`);
}