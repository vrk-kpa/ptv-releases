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
/* eslint-disable no-undef */

// Because root .babelrc is configured for react-native (browser users webpack
// and server has own .babelrc file), we have to require regenerator explicitly.
//import 'regenerator/runtime';

import gulp from 'gulp';
import gutil from 'gulp-util';
import mocha from 'gulp-mocha';
import path from 'path';

// To ignore webpack custom loaders on server.
['css', 'less', 'sass', 'scss', 'styl'].forEach(ext => {
  require.extensions['.' + ext] = () => {};
});

function reportError(errorReporter) {
  return errorReporter === 'process' ? process.exit.bind(process, 1) : gutil.log;
}

export default function mochaRunCreator(errorReporter = 'process') {
  return (file) => {
    let source = './wwwroot/js/App/**/__test__/**/*.js';
    if (file) {
      // Do not run tests when changed something not JS
      if (!/\.(js|jsx)?$/.test(file.path)) {
        console.log(`Change happend on '${file.path}' but it is not valid JS file`); // eslint-disable-line no-console
        return null;
      }

      if (file.path.indexOf('__test__') !== -1)
        source = file.path;
      else {
        const parts = file.path.split(path.sep);
        const filename = parts.pop(1);
        const dir = parts.join(path.sep);
        source = `${dir}/__test__/${filename.split('.')[0]}*.js`;
      }
    }

    console.log(`Running ${source}`); // eslint-disable-line no-console
    gulp.src(source, {read: false})
      .pipe(mocha({
        require: ['./Scripts/test/mochaSetup.js'],
        reporter: 'spec'
      }))
      .on('error', reportError(errorReporter));
  };
}
