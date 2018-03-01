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
  getParameterFromProps,
  getFormValue
} from 'selectors/base'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

export const getIndustrialClasses = createSelector(
    EnumsSelectors.industrialClasses.getEnums,
    industrialClasses => industrialClasses || List()
)

export const getIndustrialClass = createSelector(
    [EntitySelectors.industrialClasses.getEntities, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

export const getIndustrialClassChildren = createSelector(
    getIndustrialClass,
    entity => entity.get('children') || List()
)

export const getIndustrialClassIds = createSelector(
    [getIndustrialClasses, getIsIdDefined],
    (top, isDefined) => isDefined ? List() : top
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) => api.getIn(['IndustrialClass', searchValue, 'search']) || Map()
)

export const getIsFetching = createIsFetchingSelector(getSearch)

const getFilteredIds = createSelector(
  getSearch,
  search => search.get('result') || List()
)

const getFilteredChildren = createSelector(
    getIndustrialClass,
    entity => entity.get('filteredChildren') || List()
)

export const getFilteredIndustrialClassIds = createSelector(
    [getFilteredIds, getIsIdDefined],
    (top, isDefined) => isDefined ? List() : top
)

export const getFilteredIndustrialClass = createSelector(
    [getIndustrialClass, getFilteredChildren],
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

export const getGeneralDescriptionIndustrialClassesIds = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('industrialClasses') || List()
)

export const getGeneralDescriptionIndustrialClasses = createSelector(
    [EntitySelectors.industrialClasses.getEntities,
    getGeneralDescriptionIndustrialClassesIds],
    (industrialClasses, ids) => getEntitiesForIds(industrialClasses, ids, List())
)

export const getIndustrialClassesForIds = createSelector(
  [EntitySelectors.industrialClasses.getEntities,
    getParameterFromProps('ids'),
    getGeneralDescriptionIndustrialClassesIds],
  (industrialClasses, ids, idsFromGD) =>
    getEntitiesForIds(industrialClasses, List(ids.filter(id => !idsFromGD.includes(id))), List())
)

export const getIndustrialClassesForIdsJs = createSelector(
  [getIndustrialClassesForIds, getGeneralDescriptionIndustrialClasses, getParameterFromProps('disabledAll')],
  (list, fromGD, disabledAll) =>
    fromGD.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:false })).concat(
      list.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:!disabledAll }))
    ).toArray()
)
