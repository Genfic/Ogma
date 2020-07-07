const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
const rename = require("gulp-rename");
const sourcemaps = require("gulp-sourcemaps");
// const purge = require("gulp-purgecss");

// CSS processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('css-mqpacker');
const nano = require('cssnano');

// JS processors
const terser = require('gulp-terser-js');

// CSS tasks
gulp.task('css', () => {
    const processors = [
        autoprefixer,
        discard({ removeAll: true }),
        mqpacker,
        nano({ preset: 'default' })
    ];

    return gulp.src('./Ogma3/wwwroot/css/*.sass')
        .pipe(sass())                           // Compile SASS
        .pipe(gulp.dest('./Ogma3/wwwroot/css')) // Output the raw CSS
        .pipe(postcss(processors))              // Postprocess it
        .pipe(rename({ suffix: '.min' }))       // Add .min suffix
        .pipe(gulp.dest('./Ogma3/wwwroot/css')) // Output minified CSS
});

gulp.task('css:purged', () => {
    return gulp.src(['./Ogma3/wwwroot/css/*.css', '!./**/*.min.css'])
        .pipe(rename({
            suffix: '.rejected'
        }))
        .pipe(purge({
            content: ['./Ogma3/**/*.cshtml'],
            rejected: true
        }))
        .pipe(gulp.dest('./Ogma3/wwwroot/css'))
});

gulp.task('watch:css', () => gulp.watch('**/*.sass', gulp.series('css')));

// JS tasks
gulp.task('js', () => {
    return gulp.src(['./Ogma3/wwwroot/js/src/**/*.js'])
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.init())
        // .pipe(uglify({mangle: true}))
        .pipe(terser({ mangle: { toplevel: true } }))
        .on('error', err => {
            console.error(err)
            this.emit('end')
        })
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./Ogma3/wwwroot/js/dist'));
});

gulp.task('watch:js', () => gulp.watch(['./Ogma3/wwwroot/js/src/**/*.js'], gulp.series('js')));

// All tasks
gulp.task('all', gulp.parallel(['css', 'js']));
gulp.task('watch:all', gulp.parallel(['watch:css', 'watch:js']));
