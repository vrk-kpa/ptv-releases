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
// import en from './en.jsonx';
var data = require('json!./En.json');

// import * as fs from 'fs';
// import {sync as globSync} from 'glob';
// import {sync as mkdirpSync} from 'mkdirp';
// import Translator from './lib/translator';

// const MESSAGES_PATTERN = './wwwroot/js/build/messages/**/*.json';
// const LANG_DIR         = './wwwroot/js/build/';

// Aggregates the default messages that were extracted from the example app's
// React components via the React Intl Babel plugin. An error will be thrown if
// there are messages in different components that use the same `id`. The result
// is a flat collection of `id: message` pairs for the app's default locale.
// let defaultMessages = globSync(MESSAGES_PATTERN)
//     .map((filename) => fs.readFileSync(filename, 'utf8'))
//     .map((file) => JSON.parse(file))
//     .reduce((collection, descriptors) => {
//         descriptors.forEach(({id, defaultMessage}) => {
//             if (collection.hasOwnProperty(id)) {
//                 if (collection[id] !== defaultMessage) {
//                     throw new Error(`Duplicate message id: ${id} with different defaultMessages {defaultMessage} and {collection[id]}.`);
//                 }
//             }
//             else{
//                 collection[id] = defaultMessage;
//             }
//         });

//         return collection;
//     }, {});

// const messages = fs.readFile('.en.jsonx', (err, file) => {
	// return JSON.parse(file);
// });


// export default{
    // test: "Test message"
// }

export default data;

