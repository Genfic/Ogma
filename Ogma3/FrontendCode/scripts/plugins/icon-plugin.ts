import { exists, mkdir } from "node:fs/promises";
import { join } from "node:path";
import type { BunPlugin } from "bun";
import dedent from "dedent";
import { pascalCase } from "es-toolkit";

export interface IconOptions {
	cacheDir: string;
}

const BASE_FILE = "icon-base.tsx";

const BASE_CONTENT = dedent(`
	import type { JSX } from "solid-js";

	export function createIcon(viewBox: string, innerHTML: string) {
		return function Icon(props: JSX.SvgSVGAttributes<SVGSVGElement>) {
			return (
				<svg
					viewBox={viewBox}
					width="1.5em"
					height="1.5em"
					part="icon"
					{...props}
					innerHTML={innerHTML}
				/>
			);
		};
	}
`);

export function iconPlugin(options: IconOptions): BunPlugin {
	return {
		name: "icon-plugin",
		async setup(build) {
			const { cacheDir } = options;

			if (!(await exists(cacheDir))) {
				await mkdir(cacheDir);
			}

			const basePath = join(cacheDir, BASE_FILE);
			if (!(await Bun.file(basePath).exists())) {
				await Bun.write(basePath, BASE_CONTENT);
			}

			build.onResolve({ filter: /^icon:/ }, async (args) => {
				const [_, collection, icon] = args.path.split(":");
				const path = join(cacheDir, `${pascalCase(collection)}${pascalCase(icon)}.tsx`);

				const file = Bun.file(path);
				if (await file.exists()) {
					return { path };
				}

				const res = await fetch(`https://api.iconify.design/${collection}/${icon}.svg`);
				if (!res.ok) {
					throw new Error(`Icon ${args.path} not found`);
				}
				const svg = await res.text();

				const viewBox = svg.match(/viewBox="([^"]+)"/)?.[1] ?? "0 0 24 24";
				const body = svg
					.replace(/<svg[^>]*>/, "")
					.replace(/<\/svg>\s*$/, "")
					.trim();
				const innerHTML = `<title>${collection} ${icon}</title>${body}`;

				const tsx = dedent(`
					import { createIcon } from "./${BASE_FILE.replace(".tsx", "")}";
					export default createIcon("${viewBox}", \`${innerHTML}\`);
				`);

				await file.write(tsx);
				return { path };
			});
		},
	};
}
