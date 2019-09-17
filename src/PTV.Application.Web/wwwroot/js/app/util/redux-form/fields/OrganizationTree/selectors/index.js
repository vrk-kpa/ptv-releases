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
import { Map, List, OrderedSet } from 'immutable'
import {
  getApiCalls,
  getEntitiesForIds,
  getIdFromProps,
  getFormValue,
  getParameterFromProps
} from 'selectors/base'
import {
  createListLabelValueSelectorJS,
  createEntityListWithDefault
} from 'selectors/factory'
import { getUserOrganization } from 'selectors/userInfo'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

export const getOrganization = createSelector(
  EntitySelectors.organizations.getEntity,
  entity => {
    // console.log(entity && entity.toJS())
    return entity.merge({
      name: entity.get('displayName')
      // areChildrenLoaded: true
      // isLeaf: children.size === 0
    })
  }
)

const getApiCall = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (apiCalls, searchValue) => apiCalls.getIn(['Organization', searchValue]) || Map()
)

const getSearch = createSelector(
  getApiCall,
  api => api.get('search') || Map()
)

export const getIsFetching = createSelector(
  getSearch,
  api => api.get('isFetching') || false
)

const getFilteredIds = createSelector(
  getSearch,
  search => search.get('result') || List()
)

const getMainOrganizationRoles = createSelector(
  EntitySelectors.organizationRoles.getEntities,
  getUserOrganization,
  (entities, userOrg) => entities.filter(entity => entity.get('isMain') && entity.get('organizationId') === userOrg).toOrderedSet() || OrderedSet()
)

const getMainOrganizationRoleIds = createSelector(
  getMainOrganizationRoles,
  orgRoles => orgRoles.map(orgRole => orgRole.get('organizationId'))
)

const getRootIds = createSelector(
  [getFilteredIds,
    getMainOrganizationRoleIds,
    getParameterFromProps('value'),
    getParameterFromProps('showSelected'),
    getParameterFromProps('onlySelected')],
  (searchedIds, orgRoleIds, selected, showSelected, onlySelected) => {
    if (showSelected) {
      if (onlySelected) {
        return selected.toList()
      }
      const mergedList = orgRoleIds.concat(selected.toList())
      return mergedList.toSet().toList()
    }

    return (searchedIds.filter(id => !selected.has(id))) || List()
  }
)

const getChildren = createSelector(
  getOrganization,
  entity => {
    return entity.get('children') || List()
  }
)

const getOrganizationIds = createSelector(
  [getRootIds, getChildren, getIsIdDefined],
  (top, children, returnChildren) => {
    return (returnChildren ? children : top).map(org => ({ id: org }))
  }
)

const getTranslatedOrganizations = createTranslatedListSelector(
  getOrganizationIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

const polyfillNames = (models, entities) => {
  return models.filter(model => model && !!model.id).map(model => {
    if (!model.name) {
      const entity = entities.get(model.id)
      return { id: model.id, name: entity.get('displayName') }
    }

    return model
  })
}

export const getOrderedOrganizations = createSelector(
  [getTranslatedOrganizations, EntitySelectors.organizations.getEntities],
  (tranlsations, entities) => {
    const organizations = polyfillNames(tranlsations, entities)
    return organizations.sort((a, b) => {
      if (!a || !b) return 0
      return a.name.localeCompare(b.name)
    })
  }
)

const getOrganizations = createSelector(
  EntitySelectors.organizations.getEntities,
  organizations => organizations
    .map(organization => organization.set('name', organization.get('displayName')))
)

const getIds = getParameterFromProps('ids')

const getOrganizationsForIds = createEntityListWithDefault(getOrganizations, getIds)

export const getOrganizationForIdsJs = createListLabelValueSelectorJS(
  getOrganizationsForIds,
  getParameterFromProps('disabledAll')
)

const getFormSelectedOrganizations = createSelector(
  getFormValue('organizationIds'),
  organizations => organizations || OrderedSet()
)

const getFormSelectedOrganizationsList = createSelector(
  getFormSelectedOrganizations,
  organizations => organizations.toList()
)

export const getSelectedOrganizations = createSelector(
  [EntitySelectors.organizations.getEntities, getFormValue('organizations')],
  (organizations, ids) => getEntitiesForIds(organizations, List(ids), List())
)

const getFilteredWithoutSelectedIds = createSelector(
  [getFilteredIds, getFormSelectedOrganizations],
  (filtered, selected) => {
    return filtered.toOrderedSet().subtract(selected).toList()
  }
)

export const getSelectedTreeOrganizationEntities = createEntityListWithDefault(getOrganizations, getFormSelectedOrganizationsList)

export const getSuggestedTreeOrganizationEntities = createEntityListWithDefault(getOrganizations, getFilteredWithoutSelectedIds)
