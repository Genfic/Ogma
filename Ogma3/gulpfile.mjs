"use strict";
import { pipeline } from "stream";
import { dest, lastRun, parallel, src, watch } from "gulp";
import sourcemaps from "gulp-sourcemaps";

// CSS processors
import postcss from "gulp-postcss";
import { sass } from "@mr-hope/gulp-sass";
import autoprefixer from "autoprefixer";
import mqpacker from "@hail2u/css-mqpacker";
import csso from "postcss-csso";

// JS processors
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
const paths = {
	styles: {
		src: [`${roots.css}/*.{sass,scss}`, `${roots.css}/src/**/*.{sass,scss}`],
		dest: `${roots.css}/dist`
	},
	js: {
		src: `${roots.js}/src/**/*.{js,ts}`,
		dest: `${roots.js}/dist`
	},
	wc: {
		src: `${roots.js}/src-webcomponents/**/*.ts`,
		dest: `${roots.js}/bundle`
	}
};

// CSS tasks
export const css = () => pipeline(src(paths.styles.src, { since: lastRun(css) }),
	sourcemaps.init({}),                   // Init maps
	sass(),                              // Compile SASS
	postcss([                    // Postprocess it
		autoprefixer,
		mqpacker,
		csso({ comments: false })
	]),
	sourcemaps.write("./", {}),     // Write maps
	dest(paths.styles.dest),      // Output minified CSS
	errorHandler);

export const watchCss = () => watch(paths.styles.src, css);

// JS tasks
export const js = () => pipeline(src(paths.js.src, { since: lastRun(js) }),
	gulpEsbuild({
		outdir: '.',
		minify: true,
		sourcemap: true,
		tsconfig: `${roots.js}/tsconfig.json`,
		bundle: true,
	}),
	dest(paths.js.dest),
	errorHandler);

export const watchJs = () => watch(paths.js.src, js);

// Component bundle
export const components = async () => pipeline(src(paths.wc.src, { since: lastRun(components) }),
	async () => {
		const out = `${paths.wc.dest}/components.js`;
		const bundle = await rollup.rollup({
			input: paths.wc.src,
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
					legalComments: "eof",
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
export const watchComponents = () => watch(paths.wc.src, components);


// All tasks
export const all = parallel(css, js, components);
export const watchAll = parallel(watchCss, watchJs, watchComponents);


// Error handler
function errorHandler(err) {
	if (err) {
		console.error(err);
	}
}
