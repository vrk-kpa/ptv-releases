/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
/// <binding AfterBuild='clean, mocha, set-prod-node-env, build-webPack, translateText, min, clean:build' Clean='clean' />
"use strict";

var gulp = require("gulp"),
    del = require("del"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    cleancss = require("gulp-clean-css"),
    uglify = require("gulp-uglify"),
    gutil = require("gulp-util");
var babel = require("babel-core/register");
var sourcemaps = require("gulp-sourcemaps");
var browserify = require('browserify');
var babelify = require('babelify');
var sassify = require('sassify');
var stringify = require('stringify');
var runSequence = require('run-sequence');
var mochaRunCreator = require('./Scripts/test/mochaRunCreator.js');
var translateTexts = require('./Scripts/translate.js');
var webpack = require('webpack');
var gutil = require('gulp-util');

process.env.NODE_ENV = 'production';
var webPackConfig = require('./webpack_external/webpack.config.js');

var paths = {
    webroot: "./wwwroot/"
};

paths.build = paths.webroot + 'js/build/';
paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.bundleJsDest = paths.webroot + "js/bundle/";
paths.concatCssDest = paths.webroot + "css/style.min.css";

gulp.task('mocha', function () {
    mochaRunCreator.default('process')();
});

gulp.task('translateText', function () {
    translateTexts.default();
});

// Continuous test running
gulp.task('mocha-watch', function () {
    gulp.watch(
      ['./wwwroot/js/App/**'],
      mochaRunCreator.default('log')
    );
});

gulp.task("clean:js", function () {
    del(paths.bundleJsDest);
});

gulp.task("clean:css", function () {
    del(paths.concatCssDest);
});

gulp.task("clean:build", function () {
    del(paths.build);
});

gulp.task("clean", (done) => {
    runSequence("clean:js", "clean:css", "clean:build", function () { done() })
});

gulp.task("min:js", function () {
    return gulp.src([paths.build + '*.js'], { base: "." })
        .pipe(concat(paths.bundleJsDest + 'bundle.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, '!' + paths.concatCssDest])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", function (done) {
    runSequence("min:js", "min:css", function () { done() });
});

gulp.task('set-prod-node-env', function () {
    return process.env.NODE_ENV = 'production';
});

gulp.task('build-webPack', ['set-prod-node-env'], function (callback) {
    webpack(webPackConfig, (fatalError, stats) => {
        var jsonStats = stats.toJson();

        // We can save jsonStats to be analyzed with
        // http://webpack.github.io/analyse or
        // https://github.com/robertknight/webpack-bundle-size-analyzer.
        // const fs = require('fs');
        // fs.writeFileSync('./bundle-stats.json', JSON.stringify(jsonStats));

        var buildError = fatalError || jsonStats.errors[0] || jsonStats.warnings[0];

        if (buildError) {
            throw new gutil.PluginError('webpack', buildError);
        }

        gutil.log('[webpack]', stats.toString({
            colors: true,
            version: false,
            hash: false,
            timings: false,
            chunks: false,
            chunkModules: false
        }));

        callback();
    });
});

gulp.task('default', function (done) {
    runSequence('clean', 'mocha', 'build', 'translateText', 'min', 'clean:temp', function () { done() });
});
