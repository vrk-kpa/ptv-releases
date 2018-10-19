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
import { Map, List, Set, Iterable } from 'immutable'

const localizedValueValidation = (value, allValues, label, validationFunc, validationMessage) => {
  const la = allValues.get('languagesAvailabilities')
  const laCodes = la.map((v, k) => v.get('code'))
  const filledNames = value.reduce((p, c, k) => !validationFunc(c) && p.add(k) || p, Set())
  const laFilledNames = filledNames.filter(name => laCodes.contains(name))
  if (laFilledNames.size < la.size) {
    const languages = la.map(l => l.get('code')).reduce((p, c) => !laFilledNames.get(c) ? p.push(c) : p, List())
    return createError(label, validationMessage, languages)
  }
  return undefined
}

const localizedSimpleValueValidation = (value, label, validationFunc, validationMessage) => {
  const languages = value.reduce((p, c, k) => validationFunc(c) && p.push(k) || p, List())
  if (languages.size > 0) {
    return createError(label, validationMessage, languages)
  }
  return undefined
}

const createError = (label, validationMessage, languages) => Map({
  message: validationMessage,
  label: label,
  languages
})

export const isValidWithNullCheck = (validationFunc, validationMessage) => label => (value, all) => {
  if (Iterable.isIterable(value)) {
    return localizedValueValidation(value, all, label, validationFunc, validationMessage)
  }

  return validationFunc(value)
    ? createError(label, validationMessage)
    : undefined
}

export const isValid = (validationFunc, validationMessage) => label => value => {
  if (Iterable.isIterable(value)) {
    return localizedSimpleValueValidation(value, label, validationFunc, validationMessage)
  }

  return validationFunc(value)
    ? createError(label, validationMessage)
    : undefined
}
