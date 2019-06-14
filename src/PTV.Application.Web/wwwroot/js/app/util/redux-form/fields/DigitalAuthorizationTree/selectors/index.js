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
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map, List } from 'immutable'
import {
  createIsFetchingSelector,
  getApiCalls,
  getIdFromProps,
  getParameterFromProps
} from 'selectors/base'
import {
  createListLabelValueSelectorJS,
  createEntityListWithDefault
} from 'selectors/factory'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

const getTopDigitalAuthorizations = createSelector(
  EnumsSelectors.topDigitalAuthorizations.getEnums,
  topDigitalAuthorizations => {
    return topDigitalAuthorizations || List()
  }
)

export const getDigitalAuthorization = createSelector(
  [EntitySelectors.digitalAuthorizations.getEntities, getIdFromProps],
  (entities, id) => {
    return entities.get(id) || Map()
  }
)

const getDigitalAuthorizationChildren = createSelector(
  getDigitalAuthorization,
  entity => {
    return entity.get('children') || List()
  }
)

export const getDigitalAuthorizationIds = createSelector(
  [getTopDigitalAuthorizations, getDigitalAuthorizationChildren, getIsIdDefined],
  (top, children, returnChildren) => {
    return returnChildren
      ? children : top
  }
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) => api.getIn(['DigitalAuthorization', searchValue, 'search']) || Map()
)

export const getIsFetching = createIsFetchingSelector(getSearch)

const getFilteredIds = createSelector(
  getSearch,
  search => search.get('result') || List()
)

const getFilteredChildren = createSelector(
  getDigitalAuthorization,
  entity => entity.get('filteredChildren') || List()
)

export const getFilteredDigitalAuthorizationIds = createSelector(
  [getFilteredIds, getFilteredChildren, getIsIdDefined],
  (top, children, returnChildren) => returnChildren ? children : top
)

export const getFilteredDigitalAuthorization = createSelector(
  [getDigitalAuthorization, getFilteredChildren],
  (entity, children) => entity.merge({
    areChildrenLoaded: true,
    isLeaf: children.size === 0
  })
)

const getDigitalAuthorizationsForIds = createEntityListWithDefault(
  EntitySelectors.digitalAuthorizations.getEntities,
  getParameterFromProps('ids')
)

export const getDigitalAuthorizationsForIdsJs = createListLabelValueSelectorJS(
  getDigitalAuthorizationsForIds,
  getParameterFromProps('disabledAll')
)
