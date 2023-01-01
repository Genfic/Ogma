"use strict";
import { pipeline } from "stream";
import gulp from "gulp";
import sourcemaps from "gulp-sourcemaps";
import run from "gulp-run";

// CSS processors
import postcss from "gulp-postcss";
import { sass } from "@mr-hope/gulp-sass";
import autoprefixer from "autoprefixer";
import mqpacker from "@hail2u/css-mqpacker";
import csso from "postcss-csso";

// JS processors
import terser from "gulp-terser";
import gulpEsbuild from "gulp-esbuild";

// Rollup
import * as rollup from "rollup";
import resolve from "@rollup/plugin-node-resolve";
import multi from "@rollup/plugin-multi-entry";
import esbuild from "rollup-plugin-esbuild";
import minifyHTML from "rollup-plugin-html-literals";

// Dirs
const root = "./wwwroot";
const roots = {
	css: `${root}/css`,
	js: `${root}/js`
};

// Watch globs
const watchGlobs = {
	sass: [ // Avoid `**` because gulp-sass shits itself otherwise and compilation takes >5s on any change
		`${roots.css}/*.{sass,scss}`,
		`${roots.css}/src/*.{sass,scss}`,
		`${roots.css}/src/elements/*.{sass,scss}`,
		`${roots.css}/src/admin-elements/*.{sass,scss}`,
		`${roots.css}/src/mixins/*.{sass,scss}`,
		`${roots.css}/src/base/*.{sass,scss}`,
		`${roots.css}/src/pages/*.{sass,scss}`

		// `${roots.css}/**/*.sass`,
		// `${roots.css}/**/*.scss`,
	],
	js: [
		`${roots.js}/src/**/*.js`
	],
	ts: [
		`${roots.js}/src/**/*.ts`,
		`!${roots.js}/src/wcomps/**/*.ts`
	]
};

// CSS tasks
export const css = () => pipeline(gulp.src(`${roots.css}/*.sass`),
	sourcemaps.init({}),                   // Init maps
	sass(),                              // Compile SASS
	postcss([                    // Postprocess it
		autoprefixer,
		mqpacker,
		csso({ comments: false })
	]),
	sourcemaps.write("./", {}),     // Write maps
	gulp.dest(`${roots.css}/dist`),      // Output minified CSS
	errorHandler);

export const watchCss = () => gulp.watch(watchGlobs.sass, css);

// JS tasks
export const js = () => pipeline(gulp.src([`${roots.js}/src/**/*.js`]),
	sourcemaps.init({}),
	terser(),
	sourcemaps.write("./", {}),
	gulp.dest(`${roots.js}/dist`),
	errorHandler);

export const watchJs = () => gulp.watch(watchGlobs.js, js);

// TS tasks
export const ts = () => pipeline(gulp.src([`${roots.js}/src/**/*.ts`]),
	gulpEsbuild({
		outdir: ".",
		minify: true,
		sourcemap: true,
		tsconfig: `${roots.js}/tsconfig.json`
	}),
	gulp.dest(`${roots.js}/dist`),
	errorHandler
);

export const watchTs = () => gulp.watch(watchGlobs.ts, ts);

// Component bundle
export const components = async () => pipeline(gulp.src(`${roots.js}/src/wcomps/**/*.ts`),
	async () => {
		const out = `${roots.js}/bundle/components.js`;
		const bundle = await rollup.rollup({
			input: `${roots.js}/src/wcomps/**/*.ts`,
			output: {
				file: out,
				format: "es",
				sourcemap: true
			},
			plugins: [
				multi(),
				resolve(),
				minifyHTML(),
				esbuild({
					tsconfig: `${roots.js}/tsconfig.json`,
					minify: true,
					legalComments: "eof"
				})
			]
		});	
		return await bundle.write({
			file: out,
			format: "umd",
			name: "components",
			sourcemap: true
		});
	},
	errorHandler);
const webtypes = () => run('npm run t:webtypes', {}).exec();
export const watchComponents = () => gulp.watch(`${roots.js}/src/wcomps/**/*.ts`, gulp.series(components, webtypes));


// All tasks
export const all = gulp.parallel(css, js, ts, components);
export const watchAll = gulp.parallel(watchCss, watchJs, watchTs, watchComponents, all);


// Error handler
function errorHandler(err) {
	if (err) {
		console.error(err);
	}
}
