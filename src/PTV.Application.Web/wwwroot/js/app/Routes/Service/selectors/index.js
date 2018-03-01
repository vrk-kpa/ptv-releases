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
  getEntitiesForIdsJS,
  getEntitiesForIds,
  getFormValue
} from 'selectors/base'
// import { getContentLanguageCode } from 'selectors/selections'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { EditorState, convertFromRaw } from 'draft-js'
import {
  getOrganization,
  getEntityLanguageAvailabilities,
  getLocalizedOrganizationsJS
} from 'selectors/common'
import { getAreaInformationWithDefault } from 'selectors/areaInformation'

export const getServiceCalls = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('service') || Map()
)

export const getGneralDescription = createSelector(
  getServiceCalls,
  serviceCalls => serviceCalls.get('generalDescription') || Map()
)

export const getGneralDescriptionSearch = createSelector(
  getGneralDescription,
  generalDescription => generalDescription.get('search') || Map()
)

export const getServiceTypeService = createSelector(
  EnumsSelectors.serviceTypes.getEntities,
  serviceTypes => serviceTypes.filter(st => st.get('code').toLowerCase() === 'service').first() || Map()
)

export const getServiceTypeServiceId = createSelector(
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
  search => search.get('data') || List()
)

export const getGneralDescriptionSearchResults = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getGneralDescriptionSearchResultsIds],
  (entities, results) => getEntitiesForIdsJS(entities, results, [])
)

export const getServiceChargeTypeMap = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('chargeType') || Map()
)

export const getServiceChargeTypeType = createSelector(
  getServiceChargeTypeMap,
  chargeType => chargeType.get('chargeType') || null
)

export const getServiceChargeTypeAdditionalInformation = createSelector(
  getServiceChargeTypeMap,
  chargeType => chargeType.get('additionalInformation') || Map()
)

export const getServiceChargeType = createSelector(
  [getServiceChargeTypeType, getServiceChargeTypeAdditionalInformation],
  (chargeType, additionalInformation) => ({
    chargeType,
    additionalInformation
  })
)

export const getServiceDescription = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('description') &&
      entity.get('description').map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getServiceConditionOfServiceUsage = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('conditionOfServiceUsage') &&
        entity.get('conditionOfServiceUsage').map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getServiceUserInstruction = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('userInstruction') &&
      entity.get('userInstruction').map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
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

export const getOverrideTargetGroups = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('overrideTargetGroups') || List()
)

export const getOverrideTargetGroupsOS = createSelector(
  getOverrideTargetGroups,
  targetGroups => targetGroups.toOrderedSet()
)

export const getServiceProducersIds = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('serviceProducers') || List()
)

export const getServiceProducers = createSelector(
  [getServiceProducersIds, EntitySelectors.serviceProducers.getEntities],
  (entityIds, entities) => getEntitiesForIds(entities, entityIds, List())
)

export const getServiceVouchers = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('serviceVouchers') || Map()
)

export const getServiceVoucherInUse = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('serviceVoucherInUse') || false
)

export const getServiceVouchersInitial = createSelector(
  [getServiceVouchers, EntitySelectors.serviceVouchers.getEntities, getServiceVoucherInUse],
  (serviceVouchers, entities, serviceVoucherInUse) => Map({
    serviceVoucherInUse: serviceVoucherInUse,
    serviceVouchers: serviceVouchers.size > 0 &&
      serviceVouchers.map(v => getEntitiesForIds(entities, v, List())) ||
      Map()
  })
)

export const getServiceLaws = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('laws') || List()
)

export const getService = createSelector(
  [EntitySelectors.getEntity,
    getServiceDescription,
    getServiceConditionOfServiceUsage,
    getServiceUserInstruction,
    getServiceChargeType,
    getServiceTypeServiceId,
    getServiceClassesOS,
    getIndustrialClassesOS,
    getOntologyTermsOS,
    getLifeEventsOS,
    getTargetGroupsOS,
    getOverrideTargetGroupsOS,
    getServiceProducers,
    getServiceVouchersInitial,
    getAreaInformationWithDefault,
    getServiceLaws,
    getEntityLanguageAvailabilities,
    getOrganization
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
    overrideTargetGroups,
    serviceProducers,
    serviceVouchers,
    areaInformation,
    laws,
    languagesAvailabilities,
    organization
  ) => ({
    name: entity.get('name') || Map(),
    generalDescriptionId: entity.get('generalDescriptionId') || null,
    alternateName: entity.get('alternateName') || Map(),
    shortDescription: entity.get('shortDescription') || Map(),
    deadLineInformation: entity.get('deadLineInformation') || Map(),
    processingTimeInformation: entity.get('processingTimeInformation') || Map(),
    validityTimeInformation: entity.get('validityTimeInformation') || Map(),
    id: entity.get('id') || null,
    serviceClasses,
    industrialClasses,
    ontologyTerms,
    annotationTerms: OrderedSet(),
    lifeEvents,
    targetGroups,
    overrideTargetGroups,
    description,
    conditionOfServiceUsage,
    userInstruction,
    serviceProducers,
    serviceVouchers,
    areaInformation,
    laws,
    responsibleOrganizations: (entity.get('responsibleOrganizations') || List()).toOrderedSet(),
    languages: entity.get('languages') || List(),
    languagesAvailabilities,
    keywords: (entity.get('keywords') || Map()).map(OrderedSet),
    organization,
    fundingType: entity.get('fundingType') || null,
    serviceType: entity.get('serviceType') || serviceTypeServiceId,
    chargeType
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

export const getGeneralDescriptionServiceType = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('serviceType') || ''
)

export const getFormServiceTypeId = createSelector(
  getFormValue('serviceType'),
  values => values || ''
)

export const getServiceTypeQualification = createSelector(
  EntitySelectors.serviceTypes.getEntities,
  serviceTypes => serviceTypes.find(st => st.get('code').toLowerCase() === 'professionalqualifications') || Map()
)

export const getServiceTypeQualificationId = createSelector(
  getServiceTypeQualification,
  serviceTypeQualification => serviceTypeQualification.get('id') || ''
)

export const getServiceTypeObligation = createSelector(
EntitySelectors.serviceTypes.getEntities,
serviceTypes => serviceTypes.find(st => st.get('code').toLowerCase() === 'permissionandobligation') || Map()
)

export const getServiceTypeObligationId = createSelector(
getServiceTypeObligation,
serviceTypeObligation => serviceTypeObligation.get('id') || ''
)

export const getServiceTypeCodeSelected = createSelector(
  [
    getIsGeneralDescriptionAttached,
    getGeneralDescriptionServiceType,
    getFormServiceTypeId,
    getServiceTypeQualificationId,
    getServiceTypeObligationId
  ], (
  isGDAttached,
  gdServiceType,
  serviceServiceType,
  qualificationServiceTypeId,
  obligationServiceTypeId
) => {
  if (isGDAttached) {
    if (gdServiceType === qualificationServiceTypeId) {
      return 'qualification'
    } else if (gdServiceType === obligationServiceTypeId) {
      return 'obligation'
    }
  } else {
    if (serviceServiceType === qualificationServiceTypeId) {
      return 'qualification'
    } else if (serviceServiceType === obligationServiceTypeId) {
      return 'obligation'
    }
  }
  return 'service'
})

export const getResponsibleOrganizations = createSelector(
 [getLocalizedOrganizationsJS, getFormValue('organization')],
  (organizations, organizationOnFormId) => {
    return organizations.filter(o => o.value !== organizationOnFormId)
  }
)
