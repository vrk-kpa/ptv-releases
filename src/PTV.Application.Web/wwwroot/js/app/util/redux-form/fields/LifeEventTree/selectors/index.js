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
  getFormValue,
  getEntitiesForIds
} from 'selectors/base'
import {
  createListLabelValueSelectorJS,
  createListLabelValueSelectorDisabledJS,
  createEntityListWithDefault
} from 'selectors/factory'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

const getTopLifeEvents = createSelector(
  EnumsSelectors.topLifeEvents.getEnums,
  topLifeEvents => topLifeEvents || List()
)

export const getLifeEvent = createSelector(
  [EntitySelectors.lifeEvents.getEntities, getIdFromProps],
  (entities, id) => entities.get(id) || Map()
)

const getLifeEventChildren = createSelector(
  getLifeEvent,
  (entity) => entity.get('children') || List()
)

const getRootIds = createSelector([
  getTopLifeEvents,
  getParameterFromProps('value'),
  getParameterFromProps('showSelected'),
  getParameterFromProps('filterSelected')
],
(top, selected, showSelected, filterSelected) =>
  showSelected
    ? selected.toList()
    : (filterSelected && top.filter(id => !selected.has(id)) || top) || List()
)

const getLifeEventIds = createSelector(
  [getRootIds, getLifeEventChildren, getIsIdDefined],
  (top, children, returnChildren) => (returnChildren ? children : top).map(event => ({ id: event }))
)

const getTranslatedLifeEvents = createTranslatedListSelector(
  getLifeEventIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getOrderedLifeEvents = createSelector(
  getTranslatedLifeEvents,
  events => events.sort((a, b) => {
    if (!a || !b) return 0
    return a.name.localeCompare(b.name)
  })
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) => api.getIn(['LifeEvent', searchValue, 'search']) || Map()
)

export const getIsFetching = createIsFetchingSelector(getSearch)

const getFilteredIds = createSelector(
  getSearch,
  search => search.get('result') || List()
)

const getFilteredChildren = createSelector(
  getLifeEvent,
  entity => entity.get('filteredChildren') || List()
)

const getFilteredLifeEventIds = createSelector(
  [getFilteredIds, getFilteredChildren, getIsIdDefined],
  (top, children, returnChildren) => (returnChildren ? children : top).map(event => ({ id: event }))
)

const getTranslatedFilteredLifeEvents = createTranslatedListSelector(
  getFilteredLifeEventIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getOrderedFilteredLifeEvents = createSelector(
  getTranslatedFilteredLifeEvents,
  events => events.sort((a, b) => {
    if (!a || !b) return 0
    return a.name.localeCompare(b.name)
  })
)

export const getFilteredLifeEvent = createSelector(
  [getLifeEvent, getFilteredChildren],
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

export const getGeneralDescriptionLifeEventsIds = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('lifeEvents') || List()
)

const getGeneralDescriptionLifeEvents = createEntityListWithDefault(
  EntitySelectors.lifeEvents.getEntities,
  getGeneralDescriptionLifeEventsIds
)

const getLifeEventsIds = createSelector([
  getParameterFromProps('ids'),
  getGeneralDescriptionLifeEventsIds
],
(ids, idsFromGD) =>
  ids.filter(id => !idsFromGD.includes(id))
)

export const getLifeEventsForIds = createEntityListWithDefault(
  EntitySelectors.lifeEvents.getEntities,
  getLifeEventsIds
)

export const getLifeEventsForIdsJs = createListLabelValueSelectorJS(
  getLifeEventsForIds,
  getParameterFromProps('disabledAll')
)

export const getGDLifeEventsForIdsJs = createListLabelValueSelectorDisabledJS(getGeneralDescriptionLifeEvents)

export const isAnyLifeEvent = createSelector(
  [getFormValue('lifeEvents'), getGeneralDescriptionLifeEventsIds],
  (sc, gdsc) => sc && sc.filter(x => !gdsc.includes(x)).size > 0
)

export const isAnyGDLifeEvent = createSelector(
  getGeneralDescriptionLifeEventsIds,
  ids => ids.size > 0
)

export const getSelectedLifeEvents = createSelector(
  [EntitySelectors.lifeEvents.getEntities, getFormValue('lifeEvents')],
  (lifeEvents, ids) => getEntitiesForIds(lifeEvents, List(ids), List())
)

