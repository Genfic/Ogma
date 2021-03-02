'use strict';
const req = require('stream');
const pipeline = req.pipeline;

const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
sass.compiler = require('sass');
const rename = require("gulp-rename");
const sourcemaps = require("gulp-sourcemaps");
const Fiber = require('fibers');
const cond = require('gulp-if');

// CSS processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('@hail2u/css-mqpacker')
const nano = require('cssnano');

// JS processors
const ts = require('gulp-typescript');
const tsProject = ts.createProject('tsconfig.json');
const terser = require('gulp-terser');

// Dirs
const root = './Ogma3/wwwroot';
const dir = {
    cssroot: `${root}/css`,
    jsroot: `${root}/js`,
}

// Watch globs
const watch = {
    sass: [ // Avoid `**` because gulp-sass shits itself otherwise and compilation takes >5s on any change
        `${dir.cssroot}/*.sass`,
        `${dir.cssroot}/src/*.sass`,
        `${dir.cssroot}/src/elements/*.sass`,
        `${dir.cssroot}/src/admin-elements/*.sass`,
        `${dir.cssroot}/src/mixins/*.sass`,

        `${dir.cssroot}/*.scss`,
        `${dir.cssroot}/src/*.scss`,
        `${dir.cssroot}/src/elements/*.scss`,
        `${dir.cssroot}/src/admin-elements/*.scss`,
        `${dir.cssroot}/src/mixins/*.scss`
    ],
    js: [
        `${dir.jsroot}/src/**/*.js`
    ],
    ts: [
        `${dir.jsroot}/src/**/*.ts`
    ]
}

// CSS tasks
gulp.task('css', (cb) => {
    const processors = [
        autoprefixer,
        discard({removeAll: true}),
        mqpacker,
        nano({preset: 'default'})
    ];

    pipeline(gulp.src(`${dir.cssroot}/*.sass`),
        sourcemaps.init(),                   // Init maps
        sass({fiber: Fiber}),        // Compile SASS
        gulp.dest(dir.cssroot),              // Output the raw CSS
        postcss(processors),                 // Postprocess it
        sourcemaps.write(`./`),     // Write maps
        cond('**/*.css',           // If it's a css file and not a map file
            rename({suffix: '.min'}), // Add .min suffix
        ),
        gulp.dest(`${dir.cssroot}/dist`),    // Output minified CSS
        errorHandler);
    cb();
});

gulp.task('watch:css', () => gulp.watch(watch.sass, gulp.series('css')));

// JS tasks
gulp.task('js', (cb) => {
    pipeline(gulp.src([`${dir.jsroot}/src/**/*.js`]),
        rename({suffix: '.min'}),
        sourcemaps.init(),
        terser({
            mangle: {
                toplevel: false
            }
        }),
        sourcemaps.write('./'),
        gulp.dest(`${dir.jsroot}/dist`),
        errorHandler);
    cb();
});

gulp.task('watch:js', () => gulp.watch(watch.js, gulp.series('js')));

// TS tasks
gulp.task('ts', (cb) => {
    pipeline(gulp.src([`${dir.jsroot}/src/**/*.ts`]),
        sourcemaps.init(),
        tsProject(),
        gulp.dest(`${dir.jsroot}/dist`),
        rename({suffix: '.min'}),
        terser({
            mangle: {
                toplevel: true
            }
        }),
        sourcemaps.write('./'),
        gulp.dest(`${dir.jsroot}/dist`),
        errorHandler);
    cb();
});

gulp.task('watch:ts', () => gulp.watch(watch.ts, gulp.series('ts')))

// All tasks
gulp.task('all', gulp.parallel(['css', 'js', 'ts']));
gulp.task('watch:all', gulp.parallel(['watch:css', 'watch:js', 'watch:ts', 'all']));


function errorHandler(err) {
    if (err) {
        console.error(err);
    }
}
