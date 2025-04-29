import type { BunPlugin } from "bun";

export const cssMinifyPlugin: BunPlugin = {
	name: "minified-css",
	async setup(build) {
		const { transform } = await import("lightningcss");
		const { cssTargets } = await import("../helpers/css-targets");
		const { basename } = await import("node:path");

		const decoder = new TextDecoder();

		build.onLoad({ filter: /\.css$/ }, async (args) => {
			try {
				const content = await Bun.file(args.path).bytes();
				const result = transform({
					code: content,
					filename: basename(args.path),
					minify: true,
					targets: cssTargets,
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
