import * as esbuild from "esbuild";

const start = process.hrtime.bigint();

const ctx = await esbuild.context({
	entryPoints: ["./wwwroot/js/src/**/*.ts", "./wwwroot/js/src/**/*.js"],
	minify: true,
	bundle: true,
	sourcemap: true,
	outdir: "./wwwroot/js/dist",
	color: true,
	logLevel: "info"
});

if (process.argv.includes("--watch")) {
	await ctx.watch();
	console.log("Watching files...");
} else {
	await ctx.rebuild();
	await ctx.dispose();
	console.log(`Build completed in ${(process.hrtime.bigint() - start) / BigInt(1_000_000)}ms`);
}