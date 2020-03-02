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
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map, List, Set } from 'immutable'
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
import { getOrderedIndustrialClasses as getOrderedIndustrialClassIdsFiltered } from './filterSelectors'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

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

export const getIndustrialClassesChildren = createSelector(
  getIndustrialClass,
  entity => entity.get('children') || Map()
)

const getIndustrialClassIds = createSelector(
  [getRootIds, getIsIdDefined, getIndustrialClassesChildren],
  (top, isDefined, children) => (isDefined ? children : top).map(id => ({ id }))
)

const getTranslatedIndustrialClasses = createTranslatedListSelector(
  getIndustrialClassIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getOrderedIndustrialClasses = createSelector(
  getTranslatedIndustrialClasses,
  classes => classes.sort((a, b) => {
    if (!a || !b) return 0
    return a.name.localeCompare(b.name)
  })
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) => api.getIn(['IndustrialClass', searchValue, 'search']) || Map()
)

export const getIsFetching = createIsFetchingSelector(getSearch)

const getFilteredChildren = createSelector(
  getIndustrialClass,
  entity => entity.get('filteredChildren') || List()
)

const getFilteredIndustrialClassIdsTemp = createSelector(
  [getOrderedIndustrialClassIdsFiltered, EntitySelectors.industrialClasses.getEntities],
  (filteredIds, industrialClasses, id) => buildTree(industrialClasses, filteredIds.map(item => item.id))
)

const getFilteredIndustrialClassIds = createSelector(
  [getFilteredIndustrialClassIdsTemp, getParameterFromProps('id')],
  (tempIds, id) => {
    const ids = !id && tempIds.get('topParents') || tempIds.get(id)
    return (ids && ids.toList() || List()).map(id => ({ id }))
  }
)

const getTranslatedFilteredIndustrialClasses = createTranslatedListSelector(
  getFilteredIndustrialClassIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getOrderedFilteredIndustrialClasses = createSelector(
  getTranslatedFilteredIndustrialClasses,
  classes => classes.sort((a, b) => {
    if (!a || !b) return 0
    return a.name.localeCompare(b.name)
  })
)

const buildTree = (entities, ids, map) => {
  return ids.reduce((acc, currentId) => {
    const parentId = entities.get(currentId).get('parentId')
    if (parentId !== null) {
      if (!acc.has(parentId)) {
        acc = acc.set(parentId, Set([currentId]))
      } else {
        acc = acc.update(parentId, list => list.add(currentId))
      }
      return buildTree(entities, List([parentId]), acc)
    }

    return acc.update('topParents', list => list.add(currentId))
  }, map || Map({ 'topParents': Set() }))
}

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
