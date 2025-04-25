import { Glob } from "bun";
import { dirname, join, parse } from "node:path";

const _root = dirname(Bun.main);

export const findAllTemplates = async (): Promise<TemplateCollectionInstance> => {
	const files = new Glob(join(_root, "templates", "*.ejs")).scan();

	const map: Record<string, string> = {};
	for await (const file of files) {
		map[parse(file).name] = await Bun.file(file).text();
	}

	return new TemplateCollection(map);
};

export type TemplateCollectionInstance = InstanceType<typeof TemplateCollection>;

class TemplateCollection {
	private readonly templates: Record<string, string> = {};

	public get(name: string) {
		if (!(name in this.templates)) {
			throw new Error(`Template "${name}" not found`);
		}
		return this.templates[name];
	}

	constructor(templates: Record<string, string>) {
		this.templates = templates;
	}
}
