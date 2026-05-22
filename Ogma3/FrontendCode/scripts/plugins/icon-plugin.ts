import { exists, mkdir } from "node:fs/promises";
import { join } from "node:path";
import type { BunPlugin } from "bun";
import { pascalCase } from "es-toolkit";
import { dedent } from "../helpers/string-helpers";
import baseIcon from "../templates/base-icon.tsx?raw";

export interface IconOptions {
	cacheDir: string;
}

interface Icon {
	prefix: string;
	lastModified: number;
	width: number;
	height: number;
	icons: Record<string, { body: string }>;
}

const BASE_FILE = "icon-base.tsx";

export function iconPlugin(options: IconOptions): BunPlugin {
	return {
		name: "icon-plugin",
		async setup(build) {
			const { cacheDir } = options;
			const inflight = new Map<string, Promise<void>>();

			if (!(await exists(cacheDir))) {
				await mkdir(cacheDir);
			}

			const basePath = join(cacheDir, BASE_FILE);
			if (!(await Bun.file(basePath).exists())) {
				await Bun.write(basePath, baseIcon);
			}

			build.onResolve({ filter: /^icon:/ }, async (args) => {
				const [_, collection, icon] = args.path.toLowerCase().split(":");
				const path = join(cacheDir, `${pascalCase(collection)}${pascalCase(icon)}.tsx`);

				const file = Bun.file(path);

				if (inflight.has(path)) {
					await inflight.get(path);
					return { path };
				}

				if (await file.exists()) {
					return { path };
				}

				const write = (async () => {
					const res = await fetch(`https://api.iconify.design/${collection}.json?icons=${icon}`);
					if (!res.ok) {
						throw new Error(`Icon ${args.path} not found`);
					}

					const data: Icon = await res.json();

					const icons = new Map(Object.entries(data.icons));
					const body = icons.size > 1 ? icons.get(icon)?.body : icons.values().next().value?.body;

					if (!body) {
						console.warn(`Icon ${icon} returned no body`);
					}

					const tsx = dedent`
						import { createIcon } from "./${BASE_FILE.replace(".tsx", "")}";
						export default createIcon(${data.width}, ${data.height}, \`${body}\`);
					`;

					await file.write(tsx);
				})();

				inflight.set(path, write);
				try {
					await write;
				} finally {
					inflight.delete(path);
				}

				return { path };
			});
		},
	};
}
