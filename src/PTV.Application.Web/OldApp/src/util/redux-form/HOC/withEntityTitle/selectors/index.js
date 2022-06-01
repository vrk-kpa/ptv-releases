/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { getForm, getParameterFromProps } from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import { getPlainText } from 'util/helpers'

export const getFormNameFieldValue = createSelector(
  [getForm, getContentLanguageCode, getParameterFromProps('formName')],
  (formStates, contentLanguageCode, formName) =>
    getPlainText(formStates.getIn([formName, 'values', 'name', contentLanguageCode]))
)

export const getFormAlterNameFieldValue = createSelector(
  [getForm, getContentLanguageCode, getParameterFromProps('formName')],
  (formStates, contentLanguageCode, formName) =>
    formStates.getIn([formName, 'values', 'alternateName', contentLanguageCode]) || ''
)

export const getFormUseAlterNameFieldValue = createSelector(
  [getForm, getContentLanguageCode, getParameterFromProps('formName')],
  (formStates, contentLanguageCode, formName) =>
    formStates.getIn([formName, 'values', 'isAlternateNameUsed', contentLanguageCode]) || false
)

// export const getIsContentLanguageInEntityLanguages = createSelector(
//   [getLanguages, getContentLanguageId],
//   (entityLanguages, contentLanguage) => {
//     // const findLanguage = entityLanguages => entityLanguages.id === contentLanguage
//     return entityLanguages.includes(contentLanguage)
//   }
// )
