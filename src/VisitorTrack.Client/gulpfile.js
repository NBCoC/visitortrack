var gulp = require('gulp'),
  cleanCSS = require('gulp-clean-css'),
  rename = require('gulp-rename'),
  replace = require('gulp-replace'),
  concat = require('gulp-concat'),
  changedInPlace = require('gulp-changed-in-place');

gulp.task('copy-font', () => {
  const directory = 'node_modules/material-design-icons/iconfont/';
  return gulp
    .src([
      `${directory}/MaterialIcons-Regular.eot`,
      `${directory}/MaterialIcons-Regular.ijmap`,
      `${directory}/MaterialIcons-Regular.svg`,
      `${directory}/MaterialIcons-Regular.ttf`,
      `${directory}/MaterialIcons-Regular.woff`,
      `${directory}/MaterialIcons-Regular.woff2`
    ])
    .pipe(changedInPlace({ firstPass: true }))
    .pipe(gulp.dest('dist/css'));
});

gulp.task('copy-css', ['copy-font'], () => {
  return gulp
    .src([
      'node_modules/material-design-icons/iconfont/material-icons.css',
      'node_modules/material-design-lite/dist/material.css',
      'node_modules/dialog-polyfill/dialog-polyfill.css',
      'node_modules/mdl-selectfield/dist/mdl-selectfield.css',
      //'src/styles/dashboard.css',
      'src/styles/visitor-track.css'
    ])
    .pipe(concat('css'))
    .pipe(cleanCSS({ compatibility: 'ie8' }))
    .pipe(rename('styles.css'))
    .pipe(gulp.dest('dist/css'));
});

gulp.task('copy-systemjs', () => {
  return gulp
    .src('node_modules/systemjs/dist/system.js')
    .pipe(gulp.dest('dist'));
});

gulp.task('copy-config-dev', () => {
  return gulp
    .src('build/config-dev.js')
    .pipe(rename('config.js'))
    .pipe(gulp.dest('dist'));
});

gulp.task('copy-config-prod', () => {
  const timestamp = new Date().getTime();

  return gulp
    .src('build/config-prod.js')
    .pipe(replace('dist/bundle.js', 'dist/bundle.js?v=' + timestamp))
    .pipe(rename('config.js'))
    .pipe(gulp.dest('dist'));
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
