'use strict';
const {pipeline} = require('stream');

const gulp = require('gulp');
const postcss = require('gulp-postcss');
// const sass = require('gulp-sass');
// sass.compiler = require('sass');
const { sass } = require("@mr-hope/gulp-sass");
const rename = require("gulp-rename");
const sourcemaps = require("gulp-sourcemaps");
const fiber = require('fibers');
const cond = require('gulp-if');

// CSS processors
const autoprefixer = require('autoprefixer');
// const discard = require('postcss-discard-comments');
const mqpacker = require('@hail2u/css-mqpacker')
// const nano = require('cssnano');
const csso = require('postcss-csso');

// JS processors
const ts = require('gulp-typescript');
const tsProject = ts.createProject('tsconfig.json');
const terser = require('gulp-terser');

// Dirs
const root = './Ogma3/wwwroot';
const roots = {
    css: `${root}/css`,
    js: `${root}/js`,
}

// Watch globs
const watchGlobs = {
    sass: [ // Avoid `**` because gulp-sass shits itself otherwise and compilation takes >5s on any change
        `${roots.css}/*.sass`,
        `${roots.css}/src/*.sass`,
        `${roots.css}/src/elements/*.sass`,
        `${roots.css}/src/admin-elements/*.sass`,
        `${roots.css}/src/mixins/*.sass`,

        `${roots.css}/*.scss`,
        `${roots.css}/src/*.scss`,
        `${roots.css}/src/elements/*.scss`,
        `${roots.css}/src/admin-elements/*.scss`,
        `${roots.css}/src/mixins/*.scss`,
    ],
    js: [
        `${roots.js}/src/**/*.js`
    ],
    ts: [
        `${roots.js}/src/**/*.ts`
    ]
}

// CSS tasks
gulp.task('css', () => {
    return pipeline(gulp.src(`${roots.css}/*.sass`),
        sourcemaps.init(),                   // Init maps
        sass({ fiber }),        // Compile SASS
        gulp.dest(roots.css),                // Output the raw CSS
        postcss([                    // Postprocess it
            autoprefixer,
            mqpacker,
            csso({comments: false})
        ]),
        sourcemaps.write(`./`),     // Write maps
        cond('**/*.css',           // If it's a css file and not a map file
            rename({suffix: '.min'}),   // Add .min suffix
        ),
        gulp.dest(`${roots.css}/dist`),    // Output minified CSS
        errorHandler);
});

gulp.task('watch:css', () => gulp.watch(watchGlobs.sass, gulp.series('css')));

// JS tasks
gulp.task('js', () => {
    return pipeline(gulp.src([`${roots.js}/src/**/*.js`]),
        rename({suffix: '.min'}),
        sourcemaps.init(),
        terser({
            mangle: {
                toplevel: false
            }
        }),
        sourcemaps.write('./'),
        gulp.dest(`${roots.js}/dist`),
        errorHandler);
});

gulp.task('watch:js', () => gulp.watch(watchGlobs.js, gulp.series('js')));

// TS tasks
gulp.task('ts', () => {
    return pipeline(gulp.src([`${roots.js}/src/**/*.ts`]),
        sourcemaps.init(),
        tsProject(),
        gulp.dest(`${roots.js}/dist`),
        rename({suffix: '.min'}),
        terser({
            mangle: {
                toplevel: true
            }
        }),
        sourcemaps.write('./'),
        gulp.dest(`${roots.js}/dist`),
        errorHandler);
});

gulp.task('watch:ts', () => gulp.watch(watchGlobs.ts, gulp.series('ts')))

// All tasks
gulp.task('all', gulp.parallel(['css', 'js', 'ts']));
gulp.task('watch:all', gulp.parallel(['watch:css', 'watch:js', 'watch:ts', 'all']));


function errorHandler(err) {
    if (err) {
        console.error(err);
    }
}
