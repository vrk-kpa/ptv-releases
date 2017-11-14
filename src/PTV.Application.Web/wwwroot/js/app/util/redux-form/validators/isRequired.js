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
import { defineMessages } from 'react-intl'
import { Map, List, Set, Iterable } from 'immutable'

const message = defineMessages({
  isRequired: {
    id: 'Components.Validators.IsRequired.Message',
    defaultMessage: 'Täytä kentän tiedot.'
  }
})

const checkText = text => (text == null || (typeof text === 'string' && text.trim().length === 0))

const localizedValueValidation = (value, allValues, label) => {
  const la = allValues.get('languagesAvailabilities')
  const laCodes = la.map((v, k) => v.get('code'))
  const filledNames = value.reduce((p, c, k) => !checkText(c) && p.add(k) || p, Set())
  const laFilledNames = filledNames.filter(name => laCodes.contains(name))
  if (laFilledNames.size < la.size) {
    const languages = la.map(l => l.get('code')).reduce((p, c) => !laFilledNames.get(c) ? p.push(c) : p, List())
    return createError(label, languages)
  }
  return undefined
}

const createError = (label, languages) => Map({
  message: message.isRequired,
  label: label,
  languages
})

const isRequired = (label) => (value, all) => {
  if (Iterable.isIterable(value)) {
    return localizedValueValidation(value, all, label)
  }

  return checkText(value)
    ? createError(label)
    : undefined
}

export default isRequired
