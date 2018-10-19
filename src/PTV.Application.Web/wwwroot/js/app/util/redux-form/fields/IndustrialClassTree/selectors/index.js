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
  getParameterFromProps,
  getEntitiesForIds,
  getFormValue
} from 'selectors/base'
import {
  createListLabelValueSelectorJS,
  createListLabelValueSelectorDisabledJS,
  createEntityListWithDefault
} from 'selectors/factory'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

const getIndustrialClasses = createSelector(
  EnumsSelectors.industrialClasses.getEnums,
  industrialClasses => industrialClasses || List()
)

export const getIndustrialClass = createSelector(
  [EntitySelectors.industrialClasses.getEntities, getIdFromProps],
  (entities, id) => entities.get(id) || Map()
)

// const getIndustrialClassChildren = createSelector(
//   getIndustrialClass,
//   entity => entity.get('children') || List()
// )

const getRootIds = createSelector([
  getIndustrialClasses,
  getParameterFromProps('value'),
  getParameterFromProps('showSelected'),
  getParameterFromProps('filterSelected')
],
(top, selected, showSelected, filterSelected) =>
  showSelected
    ? selected.toList()
    : (filterSelected && top.filter(id => !selected.has(id)) || top) || List()
)

export const getIndustrialClassIds = createSelector(
  [getRootIds, getIsIdDefined],
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

const getFormGeneralDescriptionId = createSelector(
  getFormValue('generalDescriptionId'),
  values => values || ''
)

// const getIsGeneralDescriptionAttached = createSelector(
//   getFormGeneralDescriptionId,
//   generalDescriptionId => !!generalDescriptionId || false
// )

const getGeneralDescription = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionIndustrialClassesIds = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('industrialClasses') || List()
)

export const getGeneralDescriptionIndustrialClasses = createEntityListWithDefault(
  EntitySelectors.industrialClasses.getEntities,
  getGeneralDescriptionIndustrialClassesIds
)

const getIndustrialClassesIds = createSelector([
  getParameterFromProps('ids'),
  getGeneralDescriptionIndustrialClassesIds
], (ids, idsFromGD) =>
  ids.filter(id => !idsFromGD.includes(id))
)

const getIndustrialClassesForIds = createEntityListWithDefault(
  EntitySelectors.industrialClasses.getEntities,
  getIndustrialClassesIds
)

// export const getIndustrialClassesForIdsJs = createSelector(
//   [getIndustrialClassesForIds, getGeneralDescriptionIndustrialClasses, getParameterFromProps('disabledAll')],
//   (list, fromGD, disabledAll) =>
//     fromGD.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:false })).concat(
//       list.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:!disabledAll }))
//     ).toArray()
// )

export const getIndustrialClassesForIdsJs = createListLabelValueSelectorJS(
  getIndustrialClassesForIds,
  getParameterFromProps('disabledAll')
)

export const getGDIndustrialClassesForIdsJs = createListLabelValueSelectorDisabledJS(
  getGeneralDescriptionIndustrialClasses
)

export const isAnyIndustrialClass = createSelector(
  [getFormValue('industrialClasses'), getGeneralDescriptionIndustrialClassesIds],
  (sc, gdsc) => sc && sc.filter(x => !gdsc.includes(x)).size > 0
)

export const isAnyGDIndustrialClass = createSelector(
  getGeneralDescriptionIndustrialClassesIds,
  ids => ids.size > 0
)

export const getSelectedIndustrialClasses = createSelector(
  [EntitySelectors.industrialClasses.getEntities, getFormValue('industrialClasses')],
  (industrialClasses, ids) => getEntitiesForIds(industrialClasses, List(ids), List())
)
