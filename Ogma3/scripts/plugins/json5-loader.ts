import { plugin } from "bun";

await plugin({
	name: "Json5",
	async setup(build) {
		const { parse } = await import("json5");

		build.onLoad({ filter: /\.json5$/ }, async (args) => {
			// read and parse the file
			const text = await Bun.file(args.path).text();
			const exports = parse(text) as Record<string, unknown>;

			// and return it as a module
			return {
				exports,
				loader: "object",
			};
		});
	},
});
