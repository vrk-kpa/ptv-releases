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
import { getApiCalls, getEntitiesForIds } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { List, Map, OrderedSet } from 'immutable'
import { getSelectedEntityType, getEntity } from 'selectors/entities/entities'
import { languageOrder } from 'enums'

export const getVersioningId = createSelector(
  getEntity,
  entity => entity.getIn(['version', 'id']) || null
)
export const getIsAlreadyLoaded = createSelector(
  [getApiCalls, getSelectedEntityType, getVersioningId],
  (apiCalls, entityType, versioningId) => !!apiCalls.getIn([
    'entityHistory',
    entityType,
    versioningId,
    'connection',
    'result'
  ])
)
export const getIsLoading = createSelector(
  [getApiCalls, getSelectedEntityType, getVersioningId],
  (apiCalls, entityType, versioningId) => !!apiCalls.getIn([
    'entityHistory',
    entityType,
    versioningId,
    'connection',
    'isFetching'
  ])
)
export const getSearchResult = createSelector(
  [getApiCalls, getSelectedEntityType, getVersioningId],
  (apiCalls, entityType, versioningId) => apiCalls.getIn([
    'entityHistory',
    entityType,
    versioningId,
    'connection',
    'result'
  ]) || List()

)
export const getSearchRequest = createSelector(
  [getApiCalls, getSelectedEntityType, getVersioningId],
  (apiCalls, entityType, versioningId) => apiCalls.getIn([
    'entityHistory',
    entityType,
    versioningId,
    'connection'
  ]) || Map()

)

export const getOperationSearchIds = createSelector(
  getSearchResult,
  result => result.get('data') || Map()
)
export const getOperationSearchPreviousIds = createSelector(
  getSearchRequest,
  result => result.get('prevEntities') || List()
)

export const getOperationIds = createSelector(
  [
    getOperationSearchIds,
    getOperationSearchPreviousIds
  ],
  (currentIds, previousIds) => {
    return OrderedSet(previousIds.concat(currentIds)).toList()
  }
)

export const getIsMoreAvailable = createSelector(
  getSearchResult,
  result => result.get('moreAvailable')
)

export const getPageNumber = createSelector(
  getSearchResult,
  result => result.get('pageNumber') || 0
)

export const getRows = createSelector(
  [
    getOperationIds,
    EntitySelectors.entityOperations.getEntities,
    EntitySelectors.publishingStatuses.getEntities
  ],
  (ids, entityOperations, publishingStatuses) => {
    const mainRows = getEntitiesForIds(entityOperations, ids, List())
    let result = List()
    mainRows.forEach(mainRow => {
      if (mainRow.get('subOperations') && mainRow.get('subOperations').size) {
        const subRows = getEntitiesForIds(entityOperations, mainRow.get('subOperations'), List())
        result = result.concat(subRows)
      }
      result = result.push(mainRow)
    })
    return result.toJS()
  }
)

export const getSourceLanguageCode = createSelector(
  EntitySelectors.languages.getEntity,
  language => {
    const code = language.get('code') || ''
    return code.toUpperCase()
  }
)

export const getTargetLanguageCodes = languageIds => createSelector(
  EntitySelectors.languages.getEntities,
  langs => {
    const languages = getEntitiesForIds(langs, List(languageIds), List())
    return languages.sort((a, b) => {
      if (languageOrder[a.get('code')] > languageOrder[b.get('code')]) return 1
      else if (languageOrder[a.get('code')] < languageOrder[b.get('code')]) return -1
      return 0
    }).map(lang => lang.get('code').toUpperCase()).toArray()
  }
)
