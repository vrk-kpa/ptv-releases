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
import { EntitySelectors } from 'selectors'
import { Map, List, Set } from 'immutable'

import { getFormValue } from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'

import {
  createListLabelValueSelectorDisabledJS,
  createEntityListWithDefault
} from 'selectors/factory'

export const getKeywordsJS = createSelector(
  EntitySelectors.keywords.getEntities,
    keywords => keywords.map(keyword => {
      const entity = keyword.get('entity')
      return {
        value: keyword.get('id'),
        label: keyword.get('name'),
        entity: typeof entity === 'boolean' ? entity : true
      }
    })
)

const getFormGeneralDescriptionId = createSelector(
  getFormValue('generalDescriptionId'),
  values => values || ''
)

const getGeneralDescription = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

const getGDKeywordIds = createSelector(
  [getGeneralDescription, getContentLanguageCode],
  (generalDescription, languageCode) => {
    let keywords = generalDescription.get('keywords') || Map()
    let languageKeywords = keywords.get(languageCode)
    if (languageKeywords) {
      return List(languageKeywords)
    }
    return List()
  } 
)

export const hasGDKeywords = createSelector(
  getGDKeywordIds,
  ids => ids.size > 0
)

export const getGDKeywords = createEntityListWithDefault(
  EntitySelectors.keywords.getEntities,
  getGDKeywordIds
)

export const getGDKeywordLabelValues = createListLabelValueSelectorDisabledJS(
  getGDKeywords
)

export const getGDKeywordsJS = createSelector(
  [getKeywordsJS, getGDKeywordIds],
  (keywords, keywordIds) => {
    let gdKeywords = keywords.filter((k,v) => keywordIds.contains(k.value))
    return Map(gdKeywords)
  }
)

export const getAllKeywordsJS = createSelector(
  [getKeywordsJS, getGDKeywordsJS],
  (keywords, gdKeywords) => {
    return keywords
  }
)

const getKeywordsMapForServiceAndGD = createSelector(
  [getFormValue('keywords'), getGeneralDescription],
  (keywords, gd) => {
    const gdKeywords = gd && gd.get('keywords') || Map()
    const merged = keywords.mergeDeep(gdKeywords)
    return merged
  }
)

export const getLocalizedKeywords = createSelector(
  [EntitySelectors.keywords.getEntities,
    getKeywordsMapForServiceAndGD,
    getContentLanguageCode],
  (allKeywords, formIds, languageCode) => {
    const languageIds = formIds.get(languageCode) || Map()
    const selectedKeywords = languageIds.map(id => allKeywords.get(id))
    return selectedKeywords.toJS()
  }
)
