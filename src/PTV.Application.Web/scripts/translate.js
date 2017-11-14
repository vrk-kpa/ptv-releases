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
import * as fs from 'fs'
import { sync as globSync } from 'glob'
// import {sync as mkdirpSync} from 'mkdirp';
// import Translator from './lib/translator';

const MESSAGES_PATTERN = './wwwroot/js/build/messages/**/*.json'
const LANG_DIR = './wwwroot/localization/'

// Aggregates the default messages that were extracted from the example app's
// React components via the React Intl Babel plugin. An error will be thrown if
// there are messages in different components that use the same `id`. The result
// is a flat collection of `id: message` pairs for the app's default locale.
let duplicates = []
let noMessages = {}
let keyChanged = {}

const defaultMessages = globSync(MESSAGES_PATTERN)
  .map((filename) => fs.readFileSync(filename, 'utf8'))
  .map((file) => JSON.parse(file))
  .reduce((collection, descriptors) => {
    descriptors.forEach(({ id, defaultMessage, description }) => {
      if (collection.hasOwnProperty(id)) {
        if (collection[id] !== defaultMessage) {
          // throw new Error(`Duplicate message id: ${id} with different defaultMessages ${defaultMessage} and ${collection[id]}.`)
          console.error(`Duplicate message id: ${id} with different defaultMessages ${defaultMessage} and ${collection[id]}.`)
          duplicates.push({ id, current: defaultMessage, existing: collection[id] })
        }
      } else {
        collection[id] = defaultMessage
      }
      noMessages[id] = id
      if (description) {
        keyChanged[id] = description
      }
    })

    return collection
  }, {})

if (duplicates.length > 0) {
  const ids = duplicates.map(x => x.id).join(', ')
  throw new Error(`Duplicate messages with ids: ${ids}.`)
}

// let noMessages = globSync(MESSAGES_PATTERN)
//   .map((filename) => fs.readFileSync(filename, 'utf8'))
//   .map((file) => JSON.parse(file))
//   .reduce((collection, descriptors) => {
//     descriptors.forEach(({ id }) => {
//       collection[id] = id
//     })

//     return collection
//   }, {})

// let keyChanged = globSync(MESSAGES_PATTERN)
//   .map((filename) => fs.readFileSync(filename, 'utf8'))
//   .map((file) => JSON.parse(file))
//   .reduce((collection, descriptors) => {
//     descriptors.forEach(({ id, description }) => {
//       collection[id] = description
//     })

//     return collection
//   }, {})

// For the purpose of this example app a fake locale: `en-UPPER` is created and
// the app's default messages are "translated" into this new "locale" by simply
// UPPERCASING all of the message text. In a real app this would be through some
// offline process to get the app's messages translated by machine or
// processional translators.
// let uppercaseTranslator = new Translator((text) => text.toUpperCase());
// let uppercaseMessages = Object.keys(defaultMessages)
//     .map((id) => [id, defaultMessages[id]])
//     .reduce((collection, [id, defaultMessage]) => {
//         collection[id] = uppercaseTranslator.translate(defaultMessage);
//         return collection;
//     }, {});

const sort = messages => Object.keys(messages).sort((a, b) => a.toLowerCase().localeCompare(b.toLowerCase()))

// mkdirpSync(LANG_DIR);
const filename = LANG_DIR + 'default.json'
const noTranslation = LANG_DIR + 'noTranslation.json'
const keyChanges = LANG_DIR + 'keyChanges.json'
console.log('Creating ' + filename)
fs.writeFileSync(filename, JSON.stringify(defaultMessages, sort(defaultMessages), 2))
console.log('Creating ' + noTranslation)
fs.writeFileSync(noTranslation, JSON.stringify(noMessages, sort(noMessages), 2))
console.log('Creating ' + keyChanges)
fs.writeFileSync(keyChanges, JSON.stringify(keyChanged, sort(keyChanged), 2))
// fs.writeFileSync(LANG_DIR + 'en-UPPER.json', JSON.stringify(uppercaseMessages, null, 2));

