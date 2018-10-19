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
import { set } from 'lodash'
import { defineMessages } from '../node_modules/react-intl'
import { ENGINE_METHOD_ALL } from 'constants'
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
let noMessagesNodots = {}
let keyChanged = {}
let defualtByLang = {}
let newKeys = {}

const filename = LANG_DIR + 'default.json'
const noTranslation = LANG_DIR + 'noTranslation.json'
const noTranslationDisplay = LANG_DIR + 'noTranslationDisplay.json'
const keyChanges = LANG_DIR + 'keyChanges.json'

const noTranslationOld = JSON.parse(fs.readFileSync(noTranslation, 'utf8'))

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
      noMessagesNodots[id] = id.replace(/\./g, ' ')
      if (typeof description === 'string') {
        keyChanged[id] = description
      }
      if (typeof description === 'object') {
        Object.keys(description).map(key => set(defualtByLang, [key, id], description[key]))
        set(defualtByLang, ['fi', id], defaultMessage)
      }

      if (!noTranslationOld[id]) {
        newKeys[id] = defaultMessage
      }
    })

    return collection
  }, {})

if (duplicates.length > 0) {
  const ids = duplicates.map(x => x.id).join(', ')
  throw new Error(`Duplicate messages with ids: ${ids}.`)
}

const sort = messages => Object.keys(messages).sort((a, b) => a.toLowerCase().localeCompare(b.toLowerCase()))

// mkdirpSync(LANG_DIR);
const filterNewKeys = (all, newKeys) =>
  Object.keys(all).reduce(
    (acc, curr) => {
      if (newKeys[curr]) {
        acc[curr] = all[curr]
      }
      return acc
    }, {}
  )

console.log('Creating ' + filename)
fs.writeFileSync(filename, JSON.stringify(defaultMessages, sort(defaultMessages), 2))
console.log('Creating ' + noTranslation)
fs.writeFileSync(noTranslation, JSON.stringify(noMessages, sort(noMessages), 2))
console.log('Creating ' + noTranslationDisplay)
fs.writeFileSync(noTranslationDisplay, JSON.stringify(noMessagesNodots, sort(noMessagesNodots), 2))
console.log('Creating ' + keyChanges)
fs.writeFileSync(keyChanges, JSON.stringify(keyChanged, sort(keyChanged), 2))
fs.writeFileSync(
  `${LANG_DIR}keyChangedNew.json`,
  JSON.stringify(filterNewKeys(keyChanged, newKeys), sort(keyChanged), 2)
)

const defaults = JSON.stringify(defualtByLang, null, 2)
console.log('Creating default language messages', defaults)
Object.keys(defualtByLang).forEach(lang => {
  // filter default text for new keys
  // const newDefault = Object.keys(defualtByLang[lang]).reduce(
  //   (acc, curr) => {
  //     if (newKeys[curr]) {
  //       acc[curr] = defualtByLang[lang][curr]
  //     }
  //     return acc
  //   }, {}
  // )
  const newDefault = filterNewKeys(defualtByLang[lang], newKeys)
  fs.writeFileSync(`${LANG_DIR}${lang}_default.json`, JSON.stringify(newDefault, null, 2))
  // all defaluts
  fs.writeFileSync(`${LANG_DIR}${lang}_defaultAll.json`, JSON.stringify(defualtByLang[lang], null, 2))
})
console.log('Creating difference reports')
fs.writeFileSync(`${LANG_DIR}newKeys.json`, JSON.stringify(newKeys, null, 2))
