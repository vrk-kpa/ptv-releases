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
import { Map, List, OrderedSet, fromJS } from 'immutable'
import { getApiCalls,
  getEntitiesForIdsJS,
  getEntitiesForIds,
  getFormValue,
  getValuesByFormName,
  getParameterFromProps
} from 'selectors/base'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { EditorState, convertFromRaw } from 'draft-js'
import {
  getOrganization,
  getLocalizedOrganizationsJS,
  createEntityPropertySelector
} from 'selectors/common'
import { getAreaInformationWithDefault } from 'selectors/areaInformation'
import {
  createFilteredField,
  getSelectedOrCopyEntity,
  getLanguageAvailabilities
} from 'selectors/copyEntity'
import { getUserOrganization, getUserRoleName } from 'selectors/userInfo'
import Entities from 'selectors/entities/entities'

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

export const getPreviousGeneralDescriptionSearchResultIds = createSelector(
  getGneralDescriptionSearch,
  search => search.get('previousData') || List()
)

export const getGneralDescriptionSearchResult = createSelector(
  getGneralDescriptionSearch,
  search => search.get('result') || Map()
)
1
export const getGneralDescriptionSearchResultsIds = createSelector(
  [
    getGneralDescriptionSearchResult,
    getPreviousGeneralDescriptionSearchResultIds
  ],
  (search, previousIds) => {
    const currentIds = search.get('data') || List()
    return OrderedSet(previousIds.concat(currentIds)).toList()
  }
)

export const getGneralDescriptionSearchIsFetching = createSelector(
  [getGneralDescriptionSearch, getGneralDescriptionSearchResultsIds],
  (search, resultIds) => {
    const isFetching = !!search.get('isFetching')
    const isEmpty = resultIds.size === 0
    return isFetching && isEmpty
  }
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
  createFilteredField(createEntityPropertySelector(getSelectedOrCopyEntity, 'description', Map())),
  field => {
    try {
      return field.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getServiceConditionOfServiceUsage = createSelector(
  createFilteredField(createEntityPropertySelector(getSelectedOrCopyEntity, 'conditionOfServiceUsage', Map())),
  field => {
    try {
      return field.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getServiceUserInstruction = createSelector(
  createFilteredField(createEntityPropertySelector(getSelectedOrCopyEntity, 'userInstruction', Map())),
  field => {
    try {
      return field.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getServiceClasses = createEntityPropertySelector(getSelectedOrCopyEntity, 'serviceClasses', List())

export const getServiceClassesOS = createSelector(
  getServiceClasses,
  serviceClasses => serviceClasses.toOrderedSet()
)

export const getIndustrialClasses = createEntityPropertySelector(getSelectedOrCopyEntity, 'industrialClasses', List())

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

export const getLifeEvents = createEntityPropertySelector(getSelectedOrCopyEntity, 'lifeEvents', List())

export const getLifeEventsOS = createSelector(
  getLifeEvents,
  lifeEvents => lifeEvents.toOrderedSet()
)

export const getTargetGroups = createEntityPropertySelector(getSelectedOrCopyEntity, 'targetGroups', List())

export const getTargetGroupsOS = createSelector(
  getTargetGroups,
  targetGroups => targetGroups.toOrderedSet()
)

export const getOverrideTargetGroups = createEntityPropertySelector(
  getSelectedOrCopyEntity, 'overrideTargetGroups', List()
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

const getServiceType = createSelector(
  [createEntityPropertySelector(getSelectedOrCopyEntity, 'serviceType'), getServiceTypeServiceId],
  (serviceType, defaultServiceType) => serviceType || defaultServiceType
)
const getFundingType = createEntityPropertySelector(getSelectedOrCopyEntity, 'fundingType')
const getLanguages = createEntityPropertySelector(getSelectedOrCopyEntity, 'languages', List())
const getServiceOrganization = createSelector(
  [getOrganization, getUserOrganization, getParameterFromProps('copyId')],
  (organization, userOrganization, copyId) => copyId ? userOrganization : organization
)

export const getServiceCollectionsIds = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('serviceCollections') || List()
)
export const getServiceCollections = createSelector(
  [getServiceCollectionsIds, EntitySelectors.connections.getEntities],
  (entityIds, entities) => getEntitiesForIds(entities, entityIds, List())
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
    getLanguageAvailabilities,
    getServiceOrganization,
    getServiceType,
    getFundingType,
    getLanguages,
    getServiceCollections
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
    organization,
    serviceType,
    fundingType,
    languages,
    serviceCollections
  ) => fromJS({
    name: entity.get('name') || Map(),
    generalDescriptionId: entity.get('generalDescriptionId') || null,
    alternateName: entity.get('alternateName') || Map(),
    shortDescription: entity.get('shortDescription') || Map(),
    deadLineInformation: entity.get('deadLineInformation') || Map(),
    processingTimeInformation: entity.get('processingTimeInformation') || Map(),
    validityTimeInformation: entity.get('validityTimeInformation') || Map(),
    id: entity.get('id') || null,
    unificRootId: entity.get('unificRootId') || null,
    expireOn: entity.get('expireOn') || 0,
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
    languages,
    languagesAvailabilities,
    keywords: (entity.get('keywords') || Map()).map(OrderedSet),
    organization,
    fundingType,
    serviceType,
    chargeType,
    serviceCollections
  })
)

const getGeneralDescriptionForService = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getParameterFromProps('gdId')],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

const getServiceLanguagesAvailabilities = createSelector(
  [getGeneralDescriptionForService, Entities.languages.getEntities],
  (gd, languages) => {
    return gd.size && gd.get('languagesAvailabilities').map(lang => Map({
      languageId: lang.get('languageId'),
      code: languages.getIn([lang.get('languageId'), 'code'])
    }))
  }
)

export const getServiceByGeneralDescription = createSelector(
  getService,
  getGeneralDescriptionForService,
  getServiceLanguagesAvailabilities,
  (service, gd, languagesAvailabilities) =>
    gd.size
      ? service.merge({
        generalDescriptionId: gd.get('unificRootId') || null,
        languagesAvailabilities: languagesAvailabilities,
        name: gd.get('name') || Map()
      })
      : service
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
  }
)

const getType = typeValueSelector => createSelector(
  typeValueSelector,
  EntitySelectors.serviceTypes.getEntities,
  (typeId, types) => types.getIn([typeId, 'code']) || 'Service'
)

const descriptionFields = {
  shortDescription: { json: false, type: 'Summary' },
  description: { json: true, type: 'Description' },
  processingTimeInformation: { json: false, type: 'ProcessingTime' },
  userInstruction: { json: true, type: 'UserInstruction' },
  validityTimeInformation: { json: false, type: 'ValidityTime' },
  deadLineInformation: { json: false, type: 'DeadLine' },
  backgroundDescription: { json: true, type: 'BackgroundDescription' }
}

const getDescriptions = (generalDescription, convertValue = (value) => value) =>
  generalDescription
    .filter((x, key) => Object.keys(descriptionFields).includes(key))
    .reduce((descriptions, x, key) => {
      if (x) {
        const isJSON = descriptionFields[key].json
        return descriptions.concat(
          x.map((value, language) => ({
            language,
            type: descriptionFields[key].type,
            value: !isJSON
              ? value
              : value && convertValue(value).getPlainText(' ')
          })).toArray()
        )
      }
      return descriptions
    }, [])

const getPropertyForQa = (propertyValue, getObject = (value, language) => ({ value, language })) =>
  (propertyValue || Map())
    .reduce((names, value, language) => {
      if (value) {
        names.push(getObject(value, language))
      }
      return names
    }, [])

const getNames = entity =>
  getPropertyForQa(
    entity.get('name'),
    (value, language) => ({
      language,
      type: 'Name',
      value
    })
  )

const getGeneralDescriptionType = createSelector(
  getGeneralDescription,
  gd => gd.get('type')
)

const getGeneralDescriptionForQA = createSelector(
  getGeneralDescription,
  getType(getGeneralDescriptionType),
  (generalDescription, type) => generalDescription && {
    id: generalDescription.get('id'),
    publishingStatus: 'Published',
    type,
    descriptions: getDescriptions(generalDescription, value => convertFromRaw(JSON.parse(value))),
    names: getNames(generalDescription)
  }
)

const getServiceDescriptions = createSelector(
  getValuesByFormName,
  service => service && getDescriptions(service, value => value.getCurrentContent())
)

export const getAdditionalQualityCheckData = createSelector(
  getType(getFormValue('serviceType')),
  getFormValue('unificRootId'),
  getGeneralDescriptionForQA,
  getFormValue('alternativeId'),
  getFormValue('conditionOfServiceUsage'),
  getServiceDescriptions,
  (type, id, GeneralServiceDescription, alternativeId, requirements, serviceDescriptions) => ({
    id,
    alternativeId,
    type,
    GeneralServiceDescription,
    serviceDescriptions,
    requirements: getPropertyForQa(
      requirements,
      (value, language) => ({
        language,
        value: value && value.getCurrentContent().getPlainText(' ')
      })
    )
  })
)

const isAstiConnection = createSelector(
  [EntitySelectors.getEntity,
    EntitySelectors.connections.getEntities],
  (entity, connections) => {
    const cons = entity.get('connections').map(id => connections.get(id))
    return cons.some(con => con.getIn(['astiDetails', 'isASTIConnection']))
  }
)
export const canArchiveAstiEntity = createSelector(
  [isAstiConnection, getUserRoleName],
  (isAsti, role) => {
    return !isAsti || role === 'Eeva'
  }
)
