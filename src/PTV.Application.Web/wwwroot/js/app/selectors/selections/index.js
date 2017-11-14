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
import { getSelections } from 'selectors/base'
import EntitySelectors, { getEntityAvailableLanguages } from 'selectors/entities/entities'
import { Map } from 'immutable'

// Content language //
// const getLanguageAvailabilities = createSelector(
//   // getFormValue('languagesAvailabilities'),
//   getEntityAvailableLanguages,
//   languages => languages || fromJS([{ code: 'fi'}])
// )

export const getLanguageAvailabilityCodes = createSelector(
  [getEntityAvailableLanguages, EntitySelectors.languages.getEntities],
  (languagesAvailabilities, languages) => languagesAvailabilities
    .map(la => la.get('code') || languages.getIn([la.get('languageId'), 'code']))
    .filter(code => code && code !== '?')
)

const getLanguageIdsByCode = createSelector(
  EntitySelectors.languages.getEntities,
  languages => languages
    .reduce((prev, curr, key) => prev.set(curr.get('code'), key), Map())
)

const getDefaultSelectedLanguage = createSelector(
  getLanguageAvailabilityCodes,
  codes => codes.first()
)

export const getContentLanguage = createSelector(
  [getSelections],
  (selections) => {
    // !formName && console.error('form name is missing', formName)
    return selections.get('contentLanguage') || Map()
  }
)
export const getContentLanguageCode = createSelector(
  [getContentLanguage, getDefaultSelectedLanguage],
  (contentLanguage, defaultSelectedLanguage) =>
    contentLanguage.get('code') || defaultSelectedLanguage
)
// should use content language selector
export const getContentLanguageId = createSelector(
  [getContentLanguageCode, getLanguageIdsByCode],
  (code, languages) => languages.get(code) || null
)
// Comparision language //
export const getSelectedComparisionLanguage = createSelector(
  getSelections,
  selections => selections.get('comparisionLanguage') || Map()
)
export const getSelectedComparisionLanguageCode = createSelector(
  getSelectedComparisionLanguage,
  comparisionLanguage => comparisionLanguage.get('code') || null
)
export const getSelectedComparisionLanguageId = createSelector(
  getSelectedComparisionLanguage,
  comparisionLanguage => comparisionLanguage.get('id') || null
)
// Preview language //
export const getPreviewDialogLanguage = createSelector(
  getSelections,
  selections => selections.get('previewDialogLanguage') || Map()
)
export const getSelectedPreviewDialogLanguageId = createSelector(
  getPreviewDialogLanguage,
  contentLanguage => contentLanguage.get('id') || null
)
export const getSelectedPreviewDialogLanguageCode = createSelector(
  getPreviewDialogLanguage,
  contentLanguage => contentLanguage.get('code') || null
)
// Connection language //
export const getSelectedConnectionLanguage = createSelector(
  getSelections,
  selections => selections.get('connectionLanguage') || null
)
export const getSelectedConnectionLanguageCode = createSelector(
  getSelectedConnectionLanguage,
  connectionLanguage => connectionLanguage.get('code') || null
)
export const getSelectedConnectionLanguageId = createSelector(
  getSelectedConnectionLanguage,
  connectionLanguage => connectionLanguage.get('id') || null
)
