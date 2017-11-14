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
import { Map, Set, List } from 'immutable'
import * as BaseSelectors from '../base'

export const getSelectedEntity = createSelector(
  BaseSelectors.getSelections,
  selections => selections.get('entity') || Map()
)
export const getSelectedEntityId = createSelector(
  getSelectedEntity,
  selectedEntity => selectedEntity.get('id') || null
)
export const getSelectedEntityType = createSelector(
  getSelectedEntity,
  selectedEntity => selectedEntity.get('type') || null
)

export const getSelectedEntityConcreteType = createSelector(
  getSelectedEntity,
  selectedEntity => selectedEntity.get('concreteType') || null
)

export const getEntities = createSelector(
    BaseSelectors.getCommon,
    common => common.get('entities') || Map()
)

// TODO: refactor
export const getEntitiesWithGivenType = createSelector(
    [getEntities, getSelectedEntityType, (state, ownProps) => ownProps && ownProps.type && ownProps.type || ''],
    (entities, type, typeProps) => {
      return typeProps && entities.get(typeProps) || entities.get(type) || Map()
    }
)

// TODO: refactor
export const getEntity = createSelector(
    [getEntitiesWithGivenType, getSelectedEntityId, (state, ownProps) => ownProps && ownProps.id && ownProps.id || ''],
    (entities, id, idProps) => {
      return idProps && entities.get(idProps) || entities.get(id) || Map()
    }
)

export const getEntitySubType = createSelector(
  getEntity,
  entity => entity.get('subEntityType') || null
)

export const getEntityUnificRoot = createSelector(
  getEntity,
  entity => entity.get('unificRootId') || null
)

export const getIsSelectedSelector = listSelector => createSelector(
  [listSelector, getSelectedEntityId],
  (list, id) => Set(list).has(id)
)

export const getEntityName = createSelector(
    getEntity,
    entity => entity.get('name') || Map()
)

export const getEntityAlternateName = createSelector(
  getEntity,
  entity => entity.get('alternateName') || Map()
)

export const getEntityUseAlternateName = createSelector(
  getEntity,
  entity => entity.get('isAlternateNameUsedAsDisplayName') || false
)

export const getEntityAvailableLanguages = createSelector(
    getEntity,
    entity => {
      return entity.get('languagesAvailabilities') || List()
    }
)

export const getEntityCanBeAnyPublished = createSelector(
  getEntityAvailableLanguages,
  languages => {
    return languages.some(x => x.get('canBePublished')) || false
  }
)

// entities
const createGetEntitiesSelector = entityName => createSelector(
  getEntities,
  entities => entities.get(entityName) || Map()
)

const createGetEntitySelector = entitiesSelector => createSelector(
  [entitiesSelector, BaseSelectors.getIdFromProps],
  (entities, id) => entities.get(id) || Map()
)

const createGetIsEntityLoaded = entitiesSelector => createSelector(
  [entitiesSelector, BaseSelectors.getIdFromProps],
  (entities, id) => entities.get(id) != null || id == null
)

const getApiCalls = createSelector(
    BaseSelectors.getCommon,
    common => common.get('apiCalls') || Map()
)

const createGetEntityIsFetching = (name, key) => createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([name, key, 'isFetching']) || false
)

const entityNames = [
  'addresses',
  'areaInformationTypes',
  'areaTypes',
  'business',
  'businessRegions',
  'channels',
  'channelTypes',
  'chargeTypes',
  'connections',
  'coordinates',
  'countries',
  'dialCodes',
  'digitalAuthorizations',
  'emails',
  'fundingTypes',
  'generalDescriptions',
  'hospitalRegions',
  'industrialClasses',
  'instructions',
  'keywords',
  'languages',
  'laws',
  'lifeEvents',
  'municipalities',
  'ontologyTerms',
  'openingHours',
  'organizationAreaInformations',
  'organizationRoles',
  'organizations',
  'organizationTypes',
  'phoneNumbers',
  'phoneNumberTypes',
  'postalCodes',
  'printableFormUrlTypes',
  'provinces',
  'provisionTypes',
  'publishingStatuses',
  'securityInfo',
  'serviceChannelConnectionTypes',
  'serviceClasses',
  'serviceProducers',
  'services',
  'serviceTypes',
  'serviceVouchers',
  'targetGroups',
  'translatedItems',
  'translationLanguages',
  'userOrganizations',
  'webPages',
  'webPageTypes'
]

const entitySelectors = {}

entityNames.map(name => {
  const getEntities = createGetEntitiesSelector(name)
  const getEntity = createGetEntitySelector(getEntities)
  const getIsEntityLoaded = createGetIsEntityLoaded(getEntities)
  const getEntityIsFetching = createGetEntityIsFetching(name, 'load')
  const getEntityIsLocking = createGetEntityIsFetching(name, 'lock')
  const getEntityIsUnLocking = createGetEntityIsFetching(name, 'unLock')
  const getEntityDialogIsFetching = createGetEntityIsFetching(name, 'loadDialog')
  const getEntityIsLoading = createGetEntityIsFetching(name, 'loadEntity')
  const getEntityDialogIsPublishing = createGetEntityIsFetching(name, 'publishEntity')

  entitySelectors[name] = {
    getEntities,
    getEntity,
    getIsEntityLoaded,
    getEntityIsLocking,
    getEntityIsUnLocking,
    getEntityIsFetching,
    getEntityDialogIsFetching,
    getEntityDialogIsPublishing,
    getEntityIsLoading
  }
})

export default entitySelectors
