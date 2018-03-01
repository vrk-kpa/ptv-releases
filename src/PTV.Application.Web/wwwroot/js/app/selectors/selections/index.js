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
import { getSelections, getParameterFromProps, getFormValue } from 'selectors/base'
import EntitySelectors, { getEntityAvailableLanguages } from 'selectors/entities/entities'
import { Map, List } from 'immutable'

const getFormDefaultLanguage = createSelector(
  getFormValue('languagesAvailabilities', true),
  languages => languages && languages.map(la => la.get('code')).first()
)

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

// Content language
const getContentLanguage = createSelector(
  [getSelections, getParameterFromProps('languageKey')],
  (selections, languageKey) => {
    // !formName && console.error('form name is missing', formName)
    // console.log(selections.get('contentLanguage').toJS(), languageKey)
    return (
      languageKey
      ? selections.getIn(['contentLanguage', 'keys', languageKey])
      : selections.get('contentLanguage')
    ) || Map()
  }
)

export const getContentLanguageCode = createSelector(
  [getContentLanguage, getDefaultSelectedLanguage, getFormDefaultLanguage],
  (contentLanguage, defaultSelectedLanguage, formDefault) =>
  contentLanguage.get('code') || formDefault || defaultSelectedLanguage
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
// Connections //
export const getConnectionsMainEntity = createSelector(
  getSelections,
  selections => selections.get('connectionsMainEntity') || null
)
export const getConnectionsEntity = createSelector(
  getSelections,
  selections => selections.get('connectionsEntity') || null
)
export const getConnectionsActiveEntities = createSelector(
  getSelections,
  selections => selections.get('connectionsActiveEntities') || List()
)
export const getConnectionsAddToAllEntities = createSelector(
  getSelections,
  selections => !!selections.get('shouldAddChildToAllEntities')
)
export const isConnectionsPreview = createSelector(
  getSelections,
  selections => selections.get('connectionsPreview') || false
)
export const isConnectionOpen = createSelector(
  [getSelections,
    getParameterFromProps('connectionIndex'),
    getParameterFromProps('childIndex'),
    getParameterFromProps('isAsti')],
  (selections, pIdx, cIdx, isAsti) => {
    const setup = selections.get('connectionOpenIndex') || [-1, -1, -1]
    return setup[0] === pIdx && ((!isAsti && setup[1] === cIdx) || (isAsti && setup[2] === cIdx))
  }
)
export const hasConnectionOpen = createSelector(
  [getSelections, getParameterFromProps('connectionIndex')],
  (selections, pIdx) => {
    const setup = selections.get('connectionOpenIndex') || [-1, -1]
    return setup[0] === pIdx
  }
)

