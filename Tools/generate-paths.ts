import { ensureDir } from "https://deno.land/std@0.140.0/fs/mod.ts";

const paths: { [key: string]: string } = {
	public: "https://localhost:5001/swagger/public/swagger.json",
	internal: "https://localhost:5001/swagger/internal/swagger.json",
};

const replaceType = (type: string): string => {
	return (
		{
			integer: "number",
			decimal: "number",
			double: "number",
			float: "number",
			bool: "boolean",
			string: "string",
		}[type] ?? "any"
	);
};

const buildParams = (parameters: Map<number, Parameter>): string => {
	const params = [];
	for (const [_, param] of Object.entries(parameters)) {
		if (param.in !== "path" && param.in !== "query") continue;
		let p = param.name;
		p += ": ";
		p += replaceType(param.schema.type);
		params.push(p);
	}
	return params.join(", ");
};

const buildQuery = (parameters: Map<number, Parameter>): string => {
	const params = [];
	for (const [_, param] of Object.entries(parameters)) {
		if (param.in !== "query") continue;
		let p = param.name;
		p += "=${";
		p += param.name;
		p += "}";
		params.push(p);
	}
	return params.length > 0 ? `?${params.join("&")}` : "";
};

const buildFunction = (path: string, method: string, meta: Route) => {
	let fun = "export const ";
	fun += meta.operationId;
	fun += " = (";
	fun += meta.parameters ? buildParams(meta.parameters).toLowerCase() : "";
	fun += ") => `";
	fun += path.replaceAll("{", "${").toLowerCase();
	fun += meta.parameters ? buildQuery(meta.parameters).toLowerCase() : "";
	fun += "`;";
	return fun;
};

const generatePaths = async (path: string): Promise<string[]> => {
	const res = await fetch(path);
	const data: SwaggerResponse = await res.json();

	const paths = [];

	for (const [k, v] of Object.entries(data.paths)) {
		for (const [pathKey, pathVal] of Object.entries(v)) {
			const fun = buildFunction(k, pathKey, pathVal as Route);
			paths.push(fun);
		}
	}

	return paths;
};

const outDir = Deno.args[0];
ensureDir(outDir);
for (const [key, val] of Object.entries(paths)) {
	console.log(`Generating paths for: ${key}`);
	const paths = await generatePaths(val);
	await Deno.writeTextFile(`${outDir}/paths-${key}.ts`, paths.join("\n"));
}

interface SwaggerResponse {
	"x-generator": string;
	openapi: string;
	info: { title: string; version: string };
	paths: Map<string, Path>;
}

type Path = Map<string, Route>;

interface Route {
	tags: string[];
	operationId: string;
	parameters?: Map<number, Parameter>;
}

interface Parameter {
	name: string;
	in: string;
	schema: { type: string; nullable: boolean };
}
