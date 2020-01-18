const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
const rename = require("gulp-rename");

// Processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('css-mqpacker');
const nano = require('cssnano');

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

gulp.task('watch:css', () => {
    gulp.watch('**/*.sass', gulp.series('css'));
});
