import type { BunPlugin } from "bun";
import { basename } from "node:path";

export const cssMinifyPlugin: BunPlugin = {
	name: "minified-css", // A unique name for your loader
	async setup(build) {
		const { transform } = await import("lightningcss");
		const decoder = new TextDecoder();

		build.onLoad({ filter: /\.css$/ }, async (args) => {
			try {
				const content = await Bun.file(args.path).bytes();
				const result = transform({
					code: content,
					filename: basename(args.path),
					minify: true,
				});

				const minified = decoder.decode(result.code);

				return {
					contents: minified,
					loader: "text",
				};
			} catch (e: unknown) {
				console.error(`Error processing CSS file ${args.path}`, e);

				return {
					contents: "",
					errors: [{ text: `Failed to minify CSS ${args.path}: ${(e as { message: string }).message}` }],
					loader: "text",
				};
			}
		});
	},
};
