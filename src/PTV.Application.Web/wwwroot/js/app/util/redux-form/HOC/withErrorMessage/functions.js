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
import React from 'react'
import { Iterable, List } from 'immutable'
import { validationMessageTypes } from 'util/redux-form/validators/types'

export const formatOptions = (options, formatMessage) => {
  return Object.keys(options).reduce((acc, curr) => {
    if (typeof options[curr] === 'object') {
      acc[curr] = localizeMessage(options[curr], null, formatMessage)
    } else {
      acc[curr] = options[curr]
    }
    return acc
  }, {})
}

export const localizeMessage = (item, options, formatMessage) =>
  item && item.id && (
    options
      ? formatMessage(item, formatOptions(options, formatMessage))
      : formatMessage(item)
  ) || item

export const getMessage = (error, formatMessage) => {
  if (Array.isArray(error)) {
    error = error[0]
  }
  const x = error && (
    Iterable.isIterable(error)
      ? localizeMessage(
        Iterable.isIterable(error.get('message')) && error.get('message').toJS() || error.get('message'),
        Iterable.isIterable(error.get('options')) && error.get('options').toJS() || error.get('options'),
        formatMessage
      )
      : localizeMessage(error.message, error.options, formatMessage)
  ) || ''
  // console.log('loc error', error, x)
  return x
}

export const isLanguageError = (error, language) => {
  if (!language) {
    return error
  }
  const languages = Iterable.isIterable(error) ? error.get('languages') : error.languages
  return !languages || languages.includes(language)
}

export const getType = error =>
  (Iterable.isIterable(error) ? error.get('type') : error.type) || validationMessageTypes.visible

export const getByType = (warning, language, formatMessage, type) =>
  (!type || getType(warning) === type) &&
  isLanguageError(warning, language) &&
  getMessage(warning, formatMessage) || null

export const getMessages = (error, language, formatMessage, type) =>
  error && ((Iterable.isIterable(error) ? error.get('errors') : List(error.errors)) || List())
    .map(w => getByType(w, language, formatMessage, type))
    .filter(w => w) || List()

export const isErrorWarning = (meta, props) => {
  return typeof props.isErrorWarning === 'function' ? props.isErrorWarning(meta, props) : !meta.touched && !meta.dirty
}
