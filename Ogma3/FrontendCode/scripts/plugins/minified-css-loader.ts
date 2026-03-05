import type { BunPlugin } from "bun";

export const cssMinifyPlugin = async (): Promise<BunPlugin> => {
	const { transform } = await import("lightningcss");
	const { cssTargets } = await import("../helpers/css-targets");
	const { basename } = await import("node:path");

	const decoder = new TextDecoder();
	return {
		name: "minified-css",
		setup(build) {
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
						errors: [
							{
								text: `Failed to minify CSS ${args.path}: ${e instanceof Error ? e.message : String(e)}`,
							},
						],
						loader: "text",
					};
				}
			});
		},
	};
};
