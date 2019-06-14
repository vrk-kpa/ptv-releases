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
import { Map, List, OrderedSet } from 'immutable'
import { EntitySelectors } from 'selectors'
import { getApiCalls, getEntitiesForIdsJS, getParameterFromProps } from 'selectors/base'
import { isUndefined } from 'lodash'

export const getChannelCalls = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('channel') || Map()
)

export const getAddress = createSelector(
  getChannelCalls,
  channelCalls => channelCalls.get('addresses') || Map()
)

export const getAddressSearch = createSelector(
  [getAddress, getParameterFromProps('formName')],
  (address, key) => address.get(key) || Map()
)

export const isSearchExists = createSelector(
  [getAddress, getParameterFromProps('formName')],
  (address, key) => address.get(key) || false
)

export const getAddressSearchResult = createSelector(
  getAddressSearch,
  search => search.get('result') || Map()
)

export const getPreviousAddressSearchResultIds = createSelector(
  getAddressSearch,
  search => search.get('prevEntities') || List()
)
export const getAddressSearchPageNumber = createSelector(
  getAddressSearchResult,
  search => {
    const pageNumber = search.get('pageNumber')
    return isUndefined(pageNumber)
      ? 0
      : pageNumber
  }
)
export const getAddressSearchIsMoreAvailable = createSelector(
  getAddressSearchResult,
  search => !!search.get('moreAvailable')
)

export const getAddressSearchResultsIds = createSelector(
  [
    getAddressSearchResult,
    getPreviousAddressSearchResultIds
  ],
  (search, previousIds) => {
    const currentIds = search.get('data') || List()
    return OrderedSet(previousIds.concat(currentIds)).toList()
  }
)

export const isAddressSearchResultExists = createSelector(
  [getAddressSearchResultsIds, isSearchExists],
  (result, resultExists) => resultExists && result && result.size > 0
)

export const getAddressSearchTotal = createSelector(
  getAddressSearchResult,
  addressSearch => addressSearch.get('count') || 0
)
export const getAddressSearchCount = createSelector(
  getAddressSearchResultsIds,
  addressSearchIds => (addressSearchIds && addressSearchIds.count()) || 0
)

export const getAddressSearchIsFetching = createSelector(
  [getAddressSearch, getAddressSearchResultsIds],
  (search, resultIds) => {
    const isFetching = !!search.get('isFetching')
    const isEmpty = resultIds.size === 0
    return isFetching && isEmpty
  }
)
export const getAddressSearchIsMoreFetching = createSelector(
  [getAddressSearchIsFetching, getAddressSearchPageNumber],
  (isFetching, pageNumber) => !!pageNumber && isFetching
)

export const getAddressSearchResults = createSelector(
  [EntitySelectors.addresses.getEntities, getAddressSearchResultsIds],
  (entities, results) => getEntitiesForIdsJS(entities, results, [])
)

