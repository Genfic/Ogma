import convert from "convert";

const text = await Bun.file("../.sizehist").text();
const lines = text.split("\n");
const entries = lines
	.map((l) => l.split(":"))
	.filter((e) => e.length === 3)
	.map((e) => ({
		name: e[0],
		time: Temporal.Instant.fromEpochMilliseconds(Number.parseInt(e[1])),
		size: Number.parseInt(e[2]),
	}));

const maxSize = entries.reduce((max, entry) => Math.max(max, entry.size), 0);

let graph = "<div class='graph'>\n";

for (const entry of entries) {
	const size = convert(entry.size, "bytes").to("best");

	graph += `\t<div class="bar" style="height: ${(entry.size / maxSize) * 100}%" title="${size.toString()}"></div>\n`;
	graph += `<div class="label">${entry.time.toLocaleString()}</div>`;
}

graph += "</div>\n";
graph += `
<style>
* {
  	box-sizing: border-box;
  	padding: 0;
  	margin: 0;
}
.graph {
	--scale: 0.01;

	display: grid;
	grid-template-rows: 1fr auto;
	gap: 1px;
	align-items: end;
	height: 100vh;
	padding: 1rem;

	font-family: monospace;
}
.bar {
  	background-color: orangered;
  	border: 1px solid #3a3a3a;
  	width: 100%;
  	grid-row: 1;
}
.label {
	writing-mode: vertical-lr;
 	text-orientation: sideways;
 	grid-row: 2;
}
</style>
`;

const server = Bun.serve({
	routes: {
		"/": () => new Response(graph, { headers: { "Content-Type": "text/html" } }),
	},
});

console.log(`Server running at ${server.url}`);

export { server };
