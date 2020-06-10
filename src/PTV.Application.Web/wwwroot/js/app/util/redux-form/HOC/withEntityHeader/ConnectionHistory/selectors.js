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
import { List, OrderedSet, Map } from 'immutable'
import { getEntityUnificRoot, getSelectedEntityType } from 'selectors/entities/entities'

export const getIsAlreadyLoaded = createSelector(
  [getApiCalls, getSelectedEntityType, getEntityUnificRoot],
  (apiCalls, entityType, unificRootId) => !!apiCalls.getIn([
    'connectionHistory',
    entityType,
    unificRootId,
    'connection',
    'result'
  ])
)
export const getIsLoading = createSelector(
  [getApiCalls, getSelectedEntityType, getEntityUnificRoot],
  (apiCalls, entityType, unificRootId) => !!apiCalls.getIn([
    'connectionHistory',
    entityType,
    unificRootId,
    'connection',
    'isFetching'
  ])
)

export const getSearchResult = createSelector(
  [getApiCalls, getSelectedEntityType, getEntityUnificRoot],
  (apiCalls, entityType, versioningId) => apiCalls.getIn([
    'connectionHistory',
    entityType,
    versioningId,
    'connection',
    'result'
  ]) || List()

)
export const getSearchRequest = createSelector(
  [getApiCalls, getSelectedEntityType, getEntityUnificRoot],
  (apiCalls, entityType, versioningId) => apiCalls.getIn([
    'connectionHistory',
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

export const getIsMoreAvailable = createSelector(
  getSearchResult,
  result => result.get('moreAvailable')
)

export const getPageNumber = createSelector(
  getSearchResult,
  result => result.get('pageNumber') || 0
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

export const getRows = createSelector(
  [getOperationIds, EntitySelectors.connectionOperations.getEntities],
  (ids, entities) => getEntitiesForIds(entities, ids, List()).toJS()
)
