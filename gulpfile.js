'use strict';
const {pipeline} = require('stream');

const gulp = require('gulp');
const postcss = require('gulp-postcss');
const {sass} = require('@mr-hope/gulp-sass');
const rename = require('gulp-rename');
const sourcemaps = require('gulp-sourcemaps');
const cond = require('gulp-if');

// CSS processors
const autoprefixer = require('autoprefixer');
const mqpacker = require('@hail2u/css-mqpacker');
const csso = require('postcss-csso');

// JS processors
const typescript = require('gulp-typescript');
const tsProject = typescript.createProject('./Ogma3/wwwroot/js/tsconfig.json');
const terser = require('gulp-terser');

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
const css = () => pipeline(gulp.src(`${roots.css}/*.sass`),
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
exports.css = css;

const watchCss = () => gulp.watch(watchGlobs.sass, css);
exports.watchCss = watchCss;

// JS tasks
const js = () => pipeline(gulp.src([`${roots.js}/src/**/*.js`]),
	rename({suffix: '.min'}),
	sourcemaps.init(),
	terser(),
	sourcemaps.write('./'),
	gulp.dest(`${roots.js}/dist`),
	errorHandler);
exports.js = js;

const watchJs = () => gulp.watch(watchGlobs.js, js);
exports.watchJs = watchJs;

// TS tasks
const ts = () => pipeline(gulp.src([`${roots.js}/src/**/*.ts`]),
	sourcemaps.init(),
	tsProject(),
	gulp.dest(`${roots.js}/dist`),
	rename({suffix: '.min'}),
	terser(),
	sourcemaps.write('./'),
	gulp.dest(`${roots.js}/dist`),
	errorHandler);
exports.ts = ts;

const watchTs = () => gulp.watch(watchGlobs.ts, ts);
exports.watchTs = watchTs;

// All tasks
const all = gulp.parallel(css, js, ts);
exports.all = all;

const watchAll = gulp.parallel(watchCss, watchJs, watchTs, all);
exports.watchAll = watchAll;


// Error handler
function errorHandler(err) {
	if (err) {
		console.error(err);
	}
}
