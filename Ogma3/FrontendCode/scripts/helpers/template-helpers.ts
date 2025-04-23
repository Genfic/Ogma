import { Glob } from "bun";
import { parse } from "node:path";

export const findAllTemplates = async (glob: string) => {
	const files = new Glob(glob).scan();

	const map: Record<string, string> = {};
	for await (const file of files) {
		map[parse(file).name] = await Bun.file(file).text();
	}

	return map;
};
