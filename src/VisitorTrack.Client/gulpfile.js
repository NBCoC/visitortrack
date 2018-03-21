var gulp = require('gulp'),
  cleanCSS = require('gulp-clean-css'),
  sass = require('gulp-sass'),
  rename = require('gulp-rename'),
  replace = require('gulp-replace');

gulp.task('copy-fonts', () => {
  var directory = './node_modules/font-awesome/fonts/';
  return gulp
    .src([
      directory + 'fontawesome-webfont.otf',
      directory + 'fontawesome-webfont.eot',
      directory + 'fontawesome-webfont.svg',
      directory + 'fontawesome-webfont.ttf',
      directory + 'fontawesome-webfont.woff',
      directory + 'fontawesome-webfont.woff2'
    ])
    .pipe(gulp.dest('./dist/fonts'));
});

gulp.task('copy-css', ['copy-fonts'], () => {
  return gulp
    .src(['./src/styles.scss'])
    .pipe(sass())
    .pipe(cleanCSS({ compatibility: 'ie8' }))
    .pipe(rename('styles.css'))
    .pipe(gulp.dest('./dist/css'));
});

gulp.task('copy-systemjs', () => {
  return gulp
    .src('./node_modules/systemjs/dist/system.js')
    .pipe(gulp.dest('./dist'));
});

gulp.task('copy-config-dev', () => {
  return gulp
    .src('./build/config-dev.js')
    .pipe(rename('config.js'))
    .pipe(gulp.dest('./dist'));
});

gulp.task('copy-config-prod', () => {
  const timestamp = new Date().getTime();

  return gulp
    .src('./build/config-prod.js')
    .pipe(replace('dist/bundle.js', 'dist/bundle.js?v=' + timestamp))
    .pipe(rename('config.js'))
    .pipe(gulp.dest('./dist'));
});

gulp.task(
  'build',
  ['copy-css', 'copy-systemjs', 'copy-config-dev'],
  () => true
);
gulp.task(
  'build:prod',
  ['copy-css', 'copy-systemjs', 'copy-config-prod'],
  () => true
);
