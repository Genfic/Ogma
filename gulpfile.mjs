'use strict';
import { pipeline } from "stream";
import { pipeline as asyncPipeline } from "stream/promises";

import gulp from 'gulp';
import postcss from "gulp-postcss";
import { sass } from "@mr-hope/gulp-sass";
import rename from "gulp-rename";
import sourcemaps from "gulp-sourcemaps";
import cond from "gulp-if";

// CSS processors
import autoprefixer from "autoprefixer";
import mqpacker from "@hail2u/css-mqpacker";
import csso from "postcss-csso";

// JS processors
import typescript from "gulp-typescript";
const tsProject = typescript.createProject('./Ogma3/wwwroot/js/tsconfig.json');
import terser from "gulp-terser";

// Rollup
import * as rollup from "rollup";
import resolve from "@rollup/plugin-node-resolve";
import rollupTs from "@rollup/plugin-typescript";
import multi from "@rollup/plugin-multi-entry";
import esbuild from 'rollup-plugin-esbuild'

// Dirs
const root = './Ogma3/wwwroot';
const roots = {
	css: `${root}/css`,
	js: `${root}/js`,
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
		`${roots.css}/src/pages/*.{sass,scss}`,

		// `${roots.css}/**/*.sass`,
		// `${roots.css}/**/*.scss`,
	],
	js: [
		`${roots.js}/src/**/*.js`
	],
	ts: [
		`${roots.js}/src/**/*.ts`
	]
};

// CSS tasks
export const css = () => pipeline(gulp.src(`${roots.css}/*.sass`),
	sourcemaps.init(),                   // Init maps
	sass(),                              // Compile SASS
	gulp.dest(`${roots.css}/dist`),      // Output the raw CSS
	postcss([                    // Postprocess it
		autoprefixer,
		mqpacker,
		csso({comments: false})
	]),
	sourcemaps.write('./'),     // Write maps
	cond('**/*.css',           // If it's a css file and not a map file
		rename({suffix: '.min'}),   // Add .min suffix
	),
	gulp.dest(`${roots.css}/dist`),      // Output minified CSS
	errorHandler);

export const watchCss = () => gulp.watch(watchGlobs.sass, css);

// JS tasks
export const js = () => pipeline(gulp.src([`${roots.js}/src/**/*.js`]),
	rename({suffix: '.min'}),
	sourcemaps.init(),
	terser(),
	sourcemaps.write('./'),
	gulp.dest(`${roots.js}/dist`),
	errorHandler);

export const watchJs = () => gulp.watch(watchGlobs.js, js);

// TS tasks
export const ts = () => pipeline(gulp.src([`${roots.js}/src/**/*.ts`]),
	sourcemaps.init(),
	tsProject(),
	gulp.dest(`${roots.js}/dist`),
	rename({suffix: '.min'}),
	terser(),
	sourcemaps.write('./'),
	gulp.dest(`${roots.js}/dist`),
	errorHandler);

export const watchTs = () => gulp.watch(watchGlobs.ts, ts);

// Component bundle
export const components = async () => pipeline(gulp.src(`${roots.js}/src/wcomps/**/*.ts`),
    async () => {
    	const out = `${roots.js}/bundle/components.js`;
        const bundle = await rollup.rollup({
            input: `${roots.js}/src/wcomps/**/*.ts`,
            output: {
                file: out,
                format: 'es',
                sourcemap: true
            },
            plugins: [
                multi(),
                resolve(),
                esbuild({
					tsconfig: './Ogma3/wwwroot/js/tsconfig.json',
					minify: true
				}),
            ]
        });
        return bundle.write({
            file: out,
            format: 'umd',
            name: 'components',
            sourcemap: true
        })
    },
	// terser(),
    errorHandler);

// All tasks
export const all = gulp.parallel(css, js, ts);
export const watchAll = gulp.parallel(watchCss, watchJs, watchTs, all);


// Error handler
function errorHandler(err) {
	if (err) {
		console.error(err);
	}
}
