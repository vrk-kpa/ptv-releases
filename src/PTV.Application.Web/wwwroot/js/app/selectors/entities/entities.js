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
import { getKey, formEntityConcreteTypes } from 'enums'

export const getSelectedEntity = createSelector(
  BaseSelectors.getSelections,
  selections => selections.get('entity') || Map()
)
export const getSelectedEntityId = createSelector(
  getSelectedEntity,
  selectedEntity => selectedEntity.get('id') || ''
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

export const getFormName = createSelector(
  getSelectedEntityConcreteType,
  entityConcreteType => entityConcreteType && getKey(formEntityConcreteTypes, entityConcreteType.toLowerCase()) || null
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
export const getTranslationAvailability = createSelector(
  getEntity,
  entity => {
    return entity.get('translationAvailability') || Map()
  }
)

export const getInTranslationLanguageCodes = createSelector(
  getTranslationAvailability,
  traslations => traslations.filter(trans => trans.get('isInTranslation')).map((_, code) => code).toSet()
)

export const getLanguageTranslationAvailability = createSelector(
  [getTranslationAvailability, BaseSelectors.getParameterFromProps('languageCode')],
  (translationAvaliability, language) => {
    return translationAvaliability.get(language) || Map()
  }
)

export const getIsInTranslation = createSelector(
  getLanguageTranslationAvailability,
  translationProgress => {
    return translationProgress.get('isInTranslation') || false
  }
)

export const getIsAnyInTranslation = createSelector(
  getTranslationAvailability,
  translationProgress => {
    return translationProgress.some(x => x.get('isInTranslation')) || false
  }
)

export const getCanBeTranslated = createSelector(
  getLanguageTranslationAvailability,
  translationProgress => {
    return translationProgress.get('canBeTranslated') || false
  }
)

export const getIsTranslationDelivered = createSelector(
  getLanguageTranslationAvailability,
  translationProgress => {
    return translationProgress.get('isTranslationDelivered') || false
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

const createGetNotLoadedEntityIds = entitiesSelector => createSelector(
  [entitiesSelector, BaseSelectors.getParameterFromProps],
  (entities, ids) => ids && ids.filter(id => !entities.get(id)) || List()
)

const getApiCalls = createSelector(
  BaseSelectors.getCommon,
  common => common.get('apiCalls') || Map()
)

const createGetEntityIsFetching = (name, key, subKeyselector = () => {}) => createSelector(
  [getApiCalls, subKeyselector],
  (apiCalls, subKey) => subKey && apiCalls.getIn(Array.isArray(subKey)
    ? [name].concat(subKey).concat(['isFetching'])
    : [name, subKey, key, 'isFetching']) || apiCalls.getIn([name, key, 'isFetching']) || false
)

export const getEntityIsFetching = createSelector(
  [getApiCalls, getSelectedEntityType],
  (apiCalls, type) => apiCalls.getIn([type, 'load', 'isFetching']) || false
)

const entityNames = [
  'accessibilityRegisters',
  'addresses',
  'areaInformationTypes',
  'areaTypes',
  'astiTypes',
  'business',
  'businessRegions',
  'channels',
  'channelTypes',
  'chargeTypes',
  'connectionOperations',
  'connections',
  'coordinates',
  'countries',
  'dialCodes',
  'digitalAuthorizations',
  'electronicInvoicingAddresses',
  'emails',
  'entityOperations',
  'extraTypes',
  'fundingTypes',
  'generalDescriptions',
  'generalDescriptionTypes',
  'hospitalRegions',
  'industrialClasses',
  'instructions',
  'keywords',
  'languages',
  'languagesAvailabilities',
  'laws',
  'lifeEvents',
  'municipalities',
  'notifications',
  'notificationsGroup',
  'ontologyTerms',
  'openingHours',
  'organizationAreaInformations',
  'organizationRoles',
  'organizations',
  'organizationTypes',
  'phoneNumbers',
  'phoneNumberTypes',
  'postalCodes',
  'previousInfos',
  'printableFormUrlTypes',
  'provinces',
  'provisionTypes',
  'publishingStatuses',
  'qualityErrors',
  'serverConstants',
  'serviceChannelConnectionTypes',
  'serviceClasses',
  'serviceCollections',
  'serviceProducers',
  'services',
  'serviceTypes',
  'serviceVouchers',
  'streets',
  'streetNumbers',
  'targetGroups',
  'tasks',
  'translatedItems',
  'translationCompanies',
  'translationLanguages',
  'translationOrderLanguages',
  'translationOrders',
  'translationOrderStates',
  'translations',
  'translationStateTypes',
  'userAccessRightsGroups',
  'userOrganizations',
  'webPages',
  'webPageTypes',
  'accessibilityClassifications',
  'accessibilityClassificationLevelTypes',
  'wcagLevelTypes'
]

const entitySelectors = {}

entityNames.map(name => {
  const getEntities = createGetEntitiesSelector(name)
  const getEntity = createGetEntitySelector(getEntities)
  const getIsEntityLoaded = createGetIsEntityLoaded(getEntities)
  const getNotLoadedIds = createGetNotLoadedEntityIds(getEntities)
  const getEntityIsFetching = createGetEntityIsFetching(name, 'load')
  const getPreviewEntityIsFetching = createGetEntityIsFetching(name, 'loadPreview')
  const getEntityIsFetchingKeyPath = (selector) =>
    createGetEntityIsFetching(name, 'load', selector)
  const getEntityIsLocking = createGetEntityIsFetching(name, 'lock')
  const getEntityIsUnLocking = createGetEntityIsFetching(name, 'unLock')
  const getEntityDialogIsFetching = createGetEntityIsFetching(name, 'loadDialog')
  const getEntityIsLoading = createGetEntityIsFetching(name, 'loadEntity')
  const getEntityDialogIsPublishing = createGetEntityIsFetching(name, 'publishEntity')

  entitySelectors[name] = {
    getEntities,
    getEntity,
    getIsEntityLoaded,
    getNotLoadedIds,
    getEntityIsLocking,
    getEntityIsUnLocking,
    getEntityIsFetching,
    getPreviewEntityIsFetching,
    getEntityIsFetchingKeyPath,
    getEntityDialogIsFetching,
    getEntityDialogIsPublishing,
    getEntityIsLoading
  }
})

export const getPreviousInfo = createSelector(
  getEntity,
  entitySelectors.previousInfos.getEntities,
  (entity, previousInfos) => previousInfos.get(entity.get('previousInfo')) || Map()
)

export const getPreviousInfoVersion = createSelector(
  getEntity,
  entitySelectors.previousInfos.getEntities,
  (entity, previousInfos) => previousInfos.getIn([entity.get('previousInfo'), 'versions', entity.get('id')]) || Map()
)

// for normalized language availabilities
// export const getEntityAvailableLanguages = createSelector(
//   getEntity,
//   entitySelectors.languagesAvailabilities.getEntities,
//   (entity, languages) => {
//     const id = entity.get('id')
//     const laIds = languages.getIn([id, 'languages']) || List()
//     return laIds.map(l => languages.getIn([id, l]))
//   }
// )

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

export default entitySelectors
