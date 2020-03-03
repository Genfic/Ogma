const gulp = require('gulp');
const postcss = require('gulp-postcss');
const sass = require('gulp-sass');
const rename = require("gulp-rename");
const sourcemaps = require("gulp-sourcemaps");

// CSS processors
const autoprefixer = require('autoprefixer');
const discard = require('postcss-discard-comments');
const mqpacker = require('css-mqpacker');
const nano = require('cssnano');

// JS processors
const uglify = require('gulp-uglify-es').default;
const closureCompiler = require('google-closure-compiler').gulp();

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

gulp.task('watch:css', () => gulp.watch('**/*.sass', gulp.series('css')));

// JS tasks
gulp.task('js', () => {
    return gulp.src(['./Ogma3/wwwroot/js/**/*.js', '!./**/*.min.js'])
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.init())
        .pipe(uglify({mangle: true}))
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./Ogma3/wwwroot/js'));
        // .pipe(closureCompiler({
        //     compilation_level: 'SIMPLE',
        //     warning_level: 'QUIET',
        //     language_in: 'ECMASCRIPT6_STRICT',
        //     language_out: 'ECMASCRIPT5_STRICT',
        //     output_wrapper: '(function(){\n%output%\n}).call(this)',
        // }, {
        //     platform: ['native', 'java', 'javascript']
        // }))
        // .pipe(sourcemaps.write('./'))
        // .pipe(gulp.dest('./Ogma3/wwwroot/js'));
});

gulp.task('watch:js', () => gulp.watch('**/*.js', gulp.series('js')));

// All tasks
gulp.task('all', gulp.parallel(['css', 'js']));
gulp.task('watch:all', gulp.parallel(['watch:css', 'watch:js']));