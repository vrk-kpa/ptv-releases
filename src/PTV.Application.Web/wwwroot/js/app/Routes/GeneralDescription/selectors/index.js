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
import { getApiCalls,
  getEntitiesForIdsJS } from 'selectors/base'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { EditorState, convertFromRaw } from 'draft-js'
import { getEntityLanguageAvailabilities } from 'Routes/selectors'

export const getGeneralDescriptionCalls = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('generalDescription') || Map()
)

export const getGneralDescription = createSelector(
  getGeneralDescriptionCalls,
  generalDescriptionCalls => generalDescriptionCalls.get('generalDescription') || Map()
)

export const getGneralDescriptionSearch = createSelector(
  getGneralDescription,
  generalDescription => generalDescription.get('search') || Map()
)

export const getServiceTypeService = createSelector(
  EnumsSelectors.serviceTypes.getEntities,
  serviceTypes => serviceTypes.filter(st => st.get('code').toLowerCase() === 'service').first() || Map()
)

export const getGeneralDescriptionTypeServiceId = createSelector(
  getServiceTypeService,
  serviceTypeService => serviceTypeService.get('id') || ''
)

export const getGneralDescriptionSearchIsFetching = createSelector(
  getGneralDescriptionSearch,
  search => search.get('isFetching') || false
)

export const getGneralDescriptionSearchResult = createSelector(
  getGneralDescriptionSearch,
  search => search.get('result') || Map()
)

export const getGneralDescriptionSearchResultsIds = createSelector(
  getGneralDescriptionSearchResult,
  search => search.get('generalDescriptions') || List()
)

export const getGneralDescriptionSearchResults = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getGneralDescriptionSearchResultsIds],
  (entities, results) => getEntitiesForIdsJS(entities, results, [])
)

export const getGeneralDescriptionChargeTypeMap = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('chargeType') || Map()
)

export const getGeneralDescriptionChargeTypeType = createSelector(
  getGeneralDescriptionChargeTypeMap,
  chargeType => chargeType.get('chargeType') || null
)

export const getGeneralDescriptionChargeTypeAdditionalInformation = createSelector(
  getGeneralDescriptionChargeTypeMap,
  chargeType => chargeType.get('additionalInformation') || Map()
)

export const getGeneralDescriptionChargeType = createSelector(
  [getGeneralDescriptionChargeTypeType, getGeneralDescriptionChargeTypeAdditionalInformation],
  (chargeType, additionalInformation) => ({
    chargeType,
    additionalInformation
  })
)

export const getGeneralDescriptionDescription = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('description') &&
      entity.get('description').map(l =>
        EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getGeneralDescriptionConditionOfServiceUsage = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('conditionOfServiceUsage') &&
        entity.get('conditionOfServiceUsage').map(l =>
          EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getGeneralDescriptionBackgroundDescription = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('backgroundDescription') &&
        entity.get('backgroundDescription').map(l =>
          EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getGeneralDescriptionUserInstruction = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('userInstruction') &&
      entity.get('userInstruction').map(l =>
        EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getServiceClasses = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('serviceClasses') || List()
)

export const getServiceClassesOS = createSelector(
  getServiceClasses,
  serviceClasses => serviceClasses.toOrderedSet()
)

export const getIndustrialClasses = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('industrialClasses') || List()
)

export const getIndustrialClassesOS = createSelector(
  getIndustrialClasses,
  industrialClasses => industrialClasses.toOrderedSet()
)

export const getOntologyTerms = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('ontologyTerms') || List()
)

export const getOntologyTermsOS = createSelector(
  getOntologyTerms,
  ontologyTerms => ontologyTerms.toOrderedSet()
)

export const getLifeEvents = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('lifeEvents') || List()
)

export const getLifeEventsOS = createSelector(
  getLifeEvents,
  lifeEvents => lifeEvents.toOrderedSet()
)

export const getTargetGroups = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('targetGroups') || List()
)

export const getTargetGroupsOS = createSelector(
  getTargetGroups,
  targetGroups => targetGroups.toOrderedSet()
)

export const getGeneralDescriptionLaws = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('laws') || List()
)

export const getConnections = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('connections') || Map()
)

export const getGeneralDescription = createSelector(
  [EntitySelectors.getEntity,
    getGeneralDescriptionDescription,
    getGeneralDescriptionConditionOfServiceUsage,
    getGeneralDescriptionUserInstruction,
    getGeneralDescriptionChargeType,
    getGeneralDescriptionTypeServiceId,
    getServiceClassesOS,
    getIndustrialClassesOS,
    getOntologyTermsOS,
    getLifeEventsOS,
    getTargetGroupsOS,
    getGeneralDescriptionLaws,
    getGeneralDescriptionBackgroundDescription,
    getEntityLanguageAvailabilities
  ],
  (entity,
    description,
    conditionOfServiceUsage,
    userInstruction,
    chargeType,
    serviceTypeServiceId,
    serviceClasses,
    industrialClasses,
    ontologyTerms,
    lifeEvents,
    targetGroups,
    laws,
    backgroundDescription,
    languagesAvailabilities
  ) => ({ name: entity.get('name') || Map(),
    shortDescription: entity.get('shortDescription') || Map(),
    deadLineInformation: entity.get('deadLineInformation') || Map(),
    processingTimeInformation: entity.get('processingTimeInformation') || Map(),
    validityTimeInformation: entity.get('validityTimeInformation') || Map(),
    languagesAvailabilities,
    id: entity.get('id') || null,
    serviceClasses,
    industrialClasses,
    ontologyTerms,
    annotationTerms: OrderedSet(),
    lifeEvents,
    targetGroups,
    description,
    conditionOfServiceUsage,
    userInstruction,
    laws,
    backgroundDescription,
    serviceType: entity.get('serviceType') || serviceTypeServiceId,
    chargeType
  })
)
