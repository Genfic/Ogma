'use strict';

const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
sass.compiler = require('sass');
const rename = require("gulp-rename");
const sourcemaps = require("gulp-sourcemaps");
const Fiber = require('fibers');
const browserSync = require('browser-sync').create();
const cond = require('gulp-if');

// CSS processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('@hail2u/css-mqpacker')
const nano = require('cssnano');

// JS processors
const ts = require('gulp-typescript');
const tsProject = ts.createProject('./tsconfig.json');
const terser = require('gulp-terser');

// Dirs
const root = './Ogma3/wwwroot';
const dir = {
    cssroot : `${root}/css`,
    jsroot  : `${root}/js`,
}

// Watch globs
const watch = {
    sass: [ // Avoid `**` because gulp-sass shits itself otherwise and compilation takes >5s on any change
        `${dir.cssroot}/*.sass`, 
        `${dir.cssroot}/src/*.sass`, 
        `${dir.cssroot}/src/elements/*.sass`, 
        `${dir.cssroot}/src/mixins/*.sass`
    ],
    js: [
        `${dir.jsroot}/src/**/*.js`
    ],
    ts: [
        `${dir.jsroot}/src/**/*.ts`
    ]
}

// CSS tasks
gulp.task('css', () => {
    const processors = [
        autoprefixer,
        discard({ removeAll: true }),
        mqpacker(),
        nano({ preset: 'default' })
    ];
    
    return gulp.src(`${dir.cssroot}/*.sass`)
        .pipe(sourcemaps.init())                   // Init maps
        .pipe(sass({fiber: Fiber}))        // Compile SASS
        .pipe(gulp.dest(dir.cssroot))              // Output the raw CSS
        .pipe(postcss(processors))                 // Postprocess it
        .pipe(sourcemaps.write(`./`))     // Write maps
        .pipe(cond('**/*.css',           // If it's a css file and not a map file
            rename({ suffix: '.min' })        // Add .min suffix
        ))     
        .pipe(gulp.dest(`${dir.cssroot}/dist`))    // Output minified CSS
});

gulp.task('watch:css', () => gulp.watch(watch.sass, gulp.series('css')));

// JS tasks
gulp.task('js', () => {
    return gulp.src([`${dir.jsroot}/src/**/*.js`])
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.init())
        .pipe(terser({ 
            mangle: { 
                toplevel: false 
            } 
        }))
        .on('error', err => {
            console.error(err)
            this.emit('end')
        })
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest(`${dir.jsroot}/dist`));
});

gulp.task('watch:js', () => gulp.watch(watch.js, gulp.series('js')));

// TS tasks
gulp.task('ts', () => {
    return gulp.src([`${dir.jsroot}/src/**/*.ts`])
        .pipe(sourcemaps.init())
        .pipe(tsProject())
        .pipe(gulp.dest(`${dir.jsroot}/dist`))
        .pipe(rename({ suffix: '.min' }))
        .pipe(terser({ mangle: { toplevel: true } }))
        .on('error', err => {
            console.error(err)
            this.emit('end')
        })
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest(`${dir.jsroot}/dist`));
});

gulp.task('watch:ts', () => gulp.watch(watch.ts, gulp.series('ts')))

// Browser sync
gulp.task('sync', function () {
    browserSync.init({
        proxy: 'http://localhost:5001',
        files: [`${dir.cssroot}/**/*.css`, `${dir.jsroot}/**/*.js`, `${dir.jsroot}/**/*.ts`]
    })
})

// All tasks
gulp.task('all', gulp.parallel(['css', 'js']));
gulp.task('watch:all', gulp.parallel(['watch:css', 'watch:js', 'all']));
