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

import { createSelector } from 'reselect'
import { formValueSelector } from 'redux-form/immutable'
import { Map } from 'immutable'
import { getCodesOfTranslationLanguages } from 'selectors/selections'

const getAddressStreet = createSelector(
  (state, { formName, path }) => formValueSelector(formName)(state, path),
  formValues => ((formValues || Map()).get('street') || Map())
)

export const getDefaultValue = createSelector(
  [getAddressStreet, (state, { language }) => language, getCodesOfTranslationLanguages],
  (street, language, allowedCodes) => {
    const streetForLanguage = street.get(language) || ''
    return !!streetForLanguage && streetForLanguage ||
        findDefaultValue(allowedCodes.filter(l => l !== language), street)
  }
)

const findDefaultValue = (allowedCodes, street) => {
  let defaultValue
  allowedCodes.forEach(lC => {
    defaultValue = street.get(lC)
    return !defaultValue
  }
  )
  return defaultValue || ''
}
