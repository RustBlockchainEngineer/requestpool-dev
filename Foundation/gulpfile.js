/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/
'use strict';

var gulp = require('gulp'),
    rimraf = require('gulp-rimraf'),
    concat = require('gulp-concat'),
    cssmin = require('gulp-cssmin'),
    uglify = require('gulp-uglify'),
    sass = require('gulp-sass'),
    less = require('gulp-less'),
    sort = require('gulp-sort'),
    rename = require('gulp-rename'),
    util = require('gulp-util'),
    postcss = require('gulp-postcss'),
    autoprefixer = require('autoprefixer'),
    rtlcss = require('rtlcss'),
    fs = require('fs');

var webroot = './wwwroot/';
var paths = {
    js: webroot + 'js/',
    css: webroot + 'css/',
    jsFiles: webroot + 'js/**/*.js',
    minJsFiles: webroot + 'js/**/*.min.js',
    cssFiles: webroot + 'css/**/*.css',
    minCssFiles: webroot + 'css/**/*.min.css',
    ignoreFiles: webroot + '**/x*.*',
    dev: webroot + 'dev',
    bower: webroot + 'dev/bower',
    themes: webroot + 'dev/themes',

};

var devConfig = JSON.parse(JSON.stringify(fs.readFileSync(paths.dev + '/config.json', 'utf8')));

gulp.task('clean', ['clean:js', 'clean:css']);

gulp.task('min:js', function () {
    return gulp.src([paths.jsFiles, '!' + paths.minJsFiles, '!' + paths.ignoreFiles])
    .pipe(sort({ asc: true }))
    .pipe(concat('site.js'))
    .pipe(gulp.dest(paths.js))
    .pipe(uglify())
    .pipe(rename('site.min.js'))
    .pipe(gulp.dest(paths.js));
});


gulp.task('css:public:clean', function () {
    var cssFolder = paths.dev + '/_public/css/',
        cssFiles = cssFolder + '**/*.css',
        overrideCssFiles = cssFolder + '**/*-rtl-override.css';
    return gulp.src([
        cssFolder + 'public-std.css',
        cssFolder + 'public-std-rtl.css',
        cssFolder + 'public-std-rtl-override.css',
        cssFolder + 'public.css',
        cssFolder + 'public-rtl.css',
        cssFolder + 'public-override-rtl.css',
        paths.css + 'public.min.css',
        paths.css + 'public-rtl.min.css'
    ])
    .pipe(rimraf());
    console.log('clean');
});

gulp.task('css:public', ['css:public:clean'], function () {
    /*
    public-std.css: standard css concatenated
    public-std-rtl.css: automatic creation of standard css for rtl display concatenated
    public-std-rtl-override.css: manually override css for rtl display concatenated
    public.css: vendor prefixed for public-std.cs
    public-rtl.css:vendor prefixed css for public-std-rtl.css
    public-override-rtl.css: vendor prefixed css for public-std-rtl-override.css 
    public.min.css: 
    public-rtl.min.css:
    public-override-rtl.min.css:
    */

    var cssFolder = paths.dev + '/_public/css/',
        cssFiles = cssFolder + '**/*.css',
        overrideCssFiles = cssFolder + '**/*-rtl-override.css';

    gulp.src([cssFiles, '!' + overrideCssFiles, '!' + paths.ignoreFiles,'!**/public*.css'])
    .pipe(sort({ asc: true }))
    .pipe(concat('public-std-ltr.css'))
    .pipe(gulp.dest(cssFolder))
    .pipe(postcss([rtlcss()]))
    .pipe(rename('public-std-rtl.css'))
    .pipe(gulp.dest(cssFolder))
    .on('end', function () {
        console.log('1');
        gulp.src([overrideCssFiles, '!' + paths.ignoreFiles])
        .pipe(sort({ asc: true }))
        .pipe(concat('public-std-rtl-override.css'))
        .pipe(gulp.dest(cssFolder))
        .on('end', function () {
            console.log('2');
            gulp.src([cssFolder + 'public*.css'])
            .pipe(postcss([autoprefixer({ browsers: devConfig.autoprefixerBrowsers })]))
            .pipe(rename(function (path) {
                path.basename = path.basename.replace('-std', '');
            }))
            .pipe(gulp.dest(cssFolder))
            .pipe(cssmin())
            .pipe(rename(function (path) {
                path.basename += '.min';
            }))
            .pipe(gulp.dest(paths.css))
            .on('end', function () {
                console.log('3');
                gulp.src([paths.css + 'public-rtl*.min.css'])
                .pipe(concat('public-rtl.min.css'))
                .pipe(gulp.dest(paths.css))
                .on('end', function () {
                    gulp.src([paths.css + 'public-rtl-override.min.css',cssFolder+'*-std-*.css'])
                    .pipe(rimraf());
                });
            });
        });
    });
});

gulp.task('css:admin:clean', function () {
    var cssFolder = paths.dev + '/_admin/css/',
        cssFiles = cssFolder + '**/*.css',
        overrideCssFiles = cssFolder + '**/*-rtl-override.css';
    return gulp.src([
        cssFolder + 'admin-std.css',
        cssFolder + 'admin-std-rtl.css',
        cssFolder + 'admin-std-rtl-override.css',
        cssFolder + 'admin.css',
        cssFolder + 'admin-rtl.css',
        cssFolder + 'admin-override-rtl.css',
        paths.css + 'admin.min.css',
        paths.css + 'admin-rtl.min.css'
    ])
    .pipe(rimraf());
    console.log('cleaned');
});

gulp.task('css:admin', ['css:admin:clean'], function () {
    /*
    public-std.css: standard css concatenated
    public-std-rtl.css: automatic creation of standard css for rtl display concatenated
    public-std-rtl-override.css: manually override css for rtl display concatenated
    public.css: vendor prefixed for public-std.cs
    public-rtl.css:vendor prefixed css for public-std-rtl.css
    public-override-rtl.css: vendor prefixed css for public-std-rtl-override.css 
    public.min.css: 
    public-rtl.min.css:
    public-override-rtl.min.css:
    */

    var cssFolder = paths.dev + '/_admin/css/',
        cssFiles = cssFolder + '**/*.css',
        overrideCssFiles = cssFolder + '**/*-rtl-override.css';

    gulp.src([cssFiles, '!' + overrideCssFiles, '!' + paths.ignoreFiles])
    .pipe(sort({ asc: true }))
    .pipe(concat('admin-std.css'))
    .pipe(gulp.dest(cssFolder))
    .pipe(postcss([rtlcss()]))
    .pipe(rename('admin-std-rtl.css'))
    .pipe(gulp.dest(cssFolder))
    .on('end', function () {
        console.log('1');
        gulp.src([overrideCssFiles, '!' + paths.ignoreFiles])
        .pipe(sort({ asc: true }))
        .pipe(concat('admin-std-rtl-override.css'))
        .pipe(gulp.dest(cssFolder))
        .on('end', function () {
            console.log('2');
            gulp.src([cssFolder + 'admin*.css'])
            .pipe(postcss([autoprefixer({ browsers: devConfig.autoprefixerBrowsers })]))
            .pipe(rename(function (path) {
                path.basename = path.basename.replace('-std', '');
            }))
            .pipe(gulp.dest(cssFolder))
            .pipe(cssmin())
            .pipe(rename(function (path) {
                path.basename += '.min';
            }))
            .pipe(gulp.dest(paths.css))
            .on('end', function () {
                console.log('3');
                gulp.src([paths.css + 'admin-rtl*.min.css'])
                .pipe(concat('admin-rtl.min.css'))
                .pipe(gulp.dest(paths.css))
                .on('end', function () {
                    gulp.src([paths.css + 'admin-rtl-override.min.css', cssFolder + '*-std-*.css'])
                    .pipe(rimraf());
                });
            });
        });
    });
});

gulp.task('css', ['css:public', 'css:admin']);


gulp.task('less:theme', function () {
    // ex: less:theme --name paper
    var prefix = util.env.prefix;
    var themeName = util.env.name;
    if (!themeName)
        themeName = 'default'
    console.log('building theme: ' + themeName);
    var themePath = paths.themes + '/' + themeName + '/';

    console.log('clean');
    gulp.src(themePath + '*.css')
    .pipe(rimraf());
    console.log('build less files');
    gulp.src(themePath + 'build.less')
      .pipe(less(
          {
              //paths: [path.join(__dirname, 'less', 'includes')]
          }
      ))
      .pipe(rename(themeName + '-ltr.css'))
      .pipe(gulp.dest(themePath))
      .pipe(postcss([rtlcss()]))
      .pipe(rename(themeName + '-rtl.css'))
      .pipe(gulp.dest(themePath))
      .on('end', function () {
          console.log('rtl version created');
          gulp.src([themePath + '*.css'])
          .pipe(postcss([autoprefixer({ browsers: devConfig.autoprefixerBrowsers })]))
          .pipe(rename(function (path) {
              //path.basename = path.basename.replace('-std', '');
          }))
          .pipe(gulp.dest(themePath))
          /*.pipe(cssmin())
          .pipe(rename(function (path) {
              path.basename += '.min';
          }))
          .pipe(gulp.dest(themePath))*/
           .on('end', function () {
               console.log('vendor prefixes added');
               gulp.src(themePath + '*.css')//dirname 
                .pipe(rename(function (path) {
                    path.basename = prefix ? (prefix + path.basename) : path.basename;
                    path.dirname = '';
                }))
              .pipe(gulp.dest(paths.css))
              .on('end', function () {
                  console.log('Success: final files copied to destination');
              });
           });
      });
});

gulp.task('default', function () {
    // place code for your default task here
});