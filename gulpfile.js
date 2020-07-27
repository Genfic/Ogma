'use strict';

const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
sass.compiler = require('sass');
const rename = require("gulp-rename");
const sourcemaps = require("gulp-sourcemaps");
const Fiber = require('fibers');

// CSS processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('css-mqpacker');
const nano = require('cssnano');

// JS processors
const terser = require('gulp-terser-js');

// Dirs
const root    = './Ogma3/wwwroot';
const cssroot = `${root}/css`;
const jsroot  = `${root}/js`;

// CSS tasks
gulp.task('css', () => {
    const processors = [
        autoprefixer,
        discard({ removeAll: true }),
        mqpacker,
        nano({ preset: 'default' })
    ];

    return gulp.src(`${cssroot}/*.sass`)
        .pipe(sourcemaps.init())                // Init maps
        .pipe(sass({fiber: Fiber}))     // Compile SASS
        .pipe(gulp.dest(cssroot))               // Output the raw CSS
        .pipe(postcss(processors))              // Postprocess it
        .pipe(sourcemaps.write(`./maps`))       // Write maps
        .pipe(rename({ suffix: '.min' }))  // Add .min suffix
        .pipe(gulp.dest(cssroot))               // Output minified CSS
});

gulp.task('watch:css', () => gulp.watch(`${cssroot}/**/*.sass`, gulp.series('css')));

// JS tasks
gulp.task('js', () => {
    return gulp.src([`${jsroot}/src/**/*.js`])
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.init())
        .pipe(terser({ mangle: { toplevel: true } }))
        .on('error', err => {
            console.error(err)
            this.emit('end')
        })
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest(`${jsroot}/dist`));
});

gulp.task('watch:js', () => gulp.watch([`${jsroot}/src/**/*.js`], gulp.series('js')));

// All tasks
gulp.task('all', gulp.parallel(['css', 'js']));
gulp.task('watch:all', gulp.parallel(['watch:css', 'watch:js', 'all']));
