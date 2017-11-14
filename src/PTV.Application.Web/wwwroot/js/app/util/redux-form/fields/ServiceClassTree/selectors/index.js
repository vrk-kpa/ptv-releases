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
  getEntitiesForIds,
  getIdFromProps,
  getFormValue,
  getParameterFromProps
} from 'selectors/base'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

export const getTopServiceClasses = createSelector(
    EnumsSelectors.topServiceClasses.getEnums,
    topServiceClasses => topServiceClasses || List()
)

const getServiceClasses = createSelector(
  [EntitySelectors.serviceClasses.getEntities, EntitySelectors.translatedItems.getEntities],
  (serviceClasses, translations) => serviceClasses.map((sc, id) => sc.set('label', translations.getIn([id, 'texts'])))
)

export const getServiceClass = createSelector(
    [getServiceClasses, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

export const getServiceClassChildren = createSelector(
    getServiceClass,
    entity => entity.get('children') || List()
)

export const getServiceClassIds = createSelector(
    [getTopServiceClasses, getServiceClassChildren, getIsIdDefined],
    (top, children, returnChildren) => returnChildren ? children : top
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) => api.getIn(['ServiceClass', searchValue, 'search']) || Map()
)

export const getIsFetching = createIsFetchingSelector(getSearch)

const getFilteredIds = createSelector(
  getSearch,
  search => search.get('result') || List()
)

const getFilteredChildren = createSelector(
    getServiceClass,
    entity => entity.get('filteredChildren') || List()
)

export const getFilteredServiceClassIds = createSelector(
    [getFilteredIds, getFilteredChildren, getIsIdDefined],
    (top, children, returnChildren) => returnChildren ? children : top
)

export const getFilteredServiceClass = createSelector(
    [getServiceClass, getFilteredChildren],
    (entity, children) => entity.merge({
      areChildrenLoaded: true,
      isLeaf: children.size === 0
    })
)

export const getFormGeneralDescriptionId = createSelector(
    getFormValue('generalDescriptionId'),
    values => values || ''
)

export const getIsGeneralDescriptionAttached = createSelector(
    getFormGeneralDescriptionId,
    generalDescriptionId => !!generalDescriptionId || false
)

export const getGeneralDescription = createSelector(
    [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
    (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionServiceClassesIds = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceClasses') || List()
)

export const getGeneralDescriptionServiceClasses = createSelector(
    [EntitySelectors.serviceClasses.getEntities, getGeneralDescriptionServiceClassesIds],
    (serviceClasses, ids) => getEntitiesForIds(serviceClasses, ids, Map())
)

export const getServiceClassesForIds = createSelector(
    [EntitySelectors.serviceClasses.getEntities, getParameterFromProps('ids'), getGeneralDescriptionServiceClassesIds],
    (serviceClasses, ids, idsFromGD) =>
      getEntitiesForIds(serviceClasses, List(ids.filter(id => !idsFromGD.includes(id))), List())
)

export const getServiceClassesForIdsJs = createSelector(
  [getServiceClassesForIds, getGeneralDescriptionServiceClasses, getParameterFromProps('disabledAll')],
  (list, fromGD, disabledAll) =>
    list.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:!disabledAll })).concat(
      fromGD.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:false }))
    ).toArray()
  )
