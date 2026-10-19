Bun.plugin({
	name: "raw-import",
	setup(build) {
		build.onLoad({ filter: /\?raw$/ }, async (args) => {
			const contents = await Bun.file(args.path.replace(/\?raw$/, "")).text();
			return {
				contents: `export default ${JSON.stringify(contents)};`,
				loader: "js",
			};
		});
	},
});
