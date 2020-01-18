const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
const rename = require("gulp-rename");

// Processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('css-mqpacker');
const nano = require('gulp-cssnano');

gulp.task('css', () => {
    const processors = [
        autoprefixer,
        discard({ removeAll: true }),
        mqpacker
    ];

    return gulp.src('./Ogma3/wwwroot/css/*.sass')
        .pipe(sass())                           // Compile SASS
        .pipe(postcss(processors))              // Postprocess it
        .pipe(gulp.dest('./Ogma3/wwwroot/css')) // Output the raw CSS
        .pipe(nano())                           // Minify CSS
        .pipe(rename({ suffix: '.min' }))       // Add .min suffix
        .pipe(gulp.dest('./Ogma3/wwwroot/css')) // Output minified CSS
});

gulp.task('watch:css', () => {
    gulp.watch('**/*.sass', gulp.series('css'));
});
