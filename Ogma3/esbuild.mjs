import * as esbuild from "esbuild";
import { minifyTemplates, writeFiles } from "esbuild-minify-templates";

const start = process.hrtime.bigint();

const baseSettings = {
	minify: true,
	bundle: true,
	sourcemap: true,
	color: true,
	logLevel: "info"
};

const bundleContext = await esbuild.context({
	...baseSettings,
	entryPoints: ["./wwwroot/js/src/**/*.ts", "./wwwroot/js/src/**/*.js"],
	outdir: "./wwwroot/js/dist"
});

const litContext = await esbuild.context({
	...baseSettings,
	entryPoints: ["./wwwroot/js/src-webcomponents/**/*.ts"],
	plugins: [minifyTemplates(), writeFiles()],
	outdir: "./wwwroot/js/bundle",
	write: false
})

// perform build
const ctx = process.argv.includes('--wc') ? litContext : bundleContext;

if (process.argv.includes("--watch")) {
	await ctx.watch();
	console.log("Watching files...");
} else {
	await ctx.rebuild();
	await ctx.dispose();
	console.log(`Build completed in ${(process.hrtime.bigint() - start) / BigInt(1_000_000)}ms`);
	process.exit();
}