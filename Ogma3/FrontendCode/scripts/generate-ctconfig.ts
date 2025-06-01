import { dirname, join, resolve } from "node:path";
import ct from "chalk-template";
import convert from "convert";
import { parse, stringify } from "json5";

const _root = dirname(Bun.main);
const _path = join(_root, "..", "..", "Data", "CTConfig.cs");
const _outputTS = join(_root, "..", "typescript", "generated", "ctconfig.ts");
const _symbols = ["public", "static", "class", "const", "int", "string"];

const start = Bun.nanoseconds();
console.log(ct`Parsing {bold ${resolve(_path)}}`);

const file = await Bun.file(_path).text();
const cleaned = file
	.split("\n")
	.map((l) => l.trim())
	.filter((l) => !l.startsWith("//"))
	.filter((l) => !l.startsWith("namespace"))
	.filter((l) => l.length > 0)
	.map((l) => {
		let nl = l;
		for (const symbol of _symbols) {
			nl = nl.replaceAll(symbol, "");
		}
		return nl.trim();
	})
	.map((l) => {
		if (["{", "}"].includes(l)) return l;

		if (!l.includes("=")) return `"${l}":`;

		const [name, value] = l.split("=").map((s) => s.trim());
		const f = new Function(`return ${value}`);

		const v = f();
		const newValue = typeof v === "number" ? v : `"${v}"`;

		return `"${name}": ${newValue},`;
	})
	.map((l) => (l === "}" ? "}," : l))
	.join("\n");

const json = `{${cleaned}}`;
const obj = parse(json).CTConfig;

const ts = Object.entries(obj)
	.map(([k, v]) => `export const ${k} = ${stringify(v, null, 4)} as const;`)
	.join("\n\n");

await Bun.write(_outputTS, ts);

const finish = convert(Bun.nanoseconds() - start, "ns")
	.to("best")
	.toString(3);

console.log(ct`Generated file {bold ${resolve(_outputTS)}}`);
console.log(ct`{green Completed in {bold ${finish}}}`);
