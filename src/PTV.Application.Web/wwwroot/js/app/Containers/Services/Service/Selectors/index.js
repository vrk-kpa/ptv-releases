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
import { List, Map, OrderedSet } from 'immutable'
import * as CommonSelectors from '../../../Common/Selectors'
import { getServiceId, getProvisionTypes, getKeyWords, getServiceTypes, getTargetGroups,
         getServiceClasses, getIndustrialClasses, getLifeEvents, getServiceTypeSelector, getProvisionTypesSelector } from '../../Common/Selectors'
import { getOrganizationEntities, getFilteredOrganizations } from '../../../Manage/Organizations/Common/Selectors'
import { getGeneralDescriptions } from '../../../Manage/GeneralDescriptions/Common/Selectors'
// import shortid from 'shortid'
import { merger } from '../../../../Middleware'

export const getServicesEntities = createSelector(
    CommonSelectors.getEntities,
    search => search.get('services') || new Map()
)

export const getServices = createSelector(
    [getServicesEntities, CommonSelectors.getLanguageParameter],
    (services, language) => services.map((service) => service.get(language))
)

export const getService = createSelector(
    [getServices, getServiceId],
    (services, serviceId) => services.get(serviceId) || Map()
)

export const getServiceEntity = createSelector(
    [getServicesEntities, getServiceId],
    (serviceEntities, serviceId) => serviceEntities.get(serviceId) || Map()
)
// Step1
export const getServiceName = createSelector(
    getService,
    service => service.get('serviceName') || ''
)

export const getGeneralServiceName = createSelector(
    getService,
    service => service.get('generalServiceName') || ''
)

export const getAlternateServiceName = createSelector(
    getService,
    service => service.get('alternateServiceName') || ''
)

export const getShortDescriptions = createSelector(
    getService,
    service => service.get('shortDescriptions') || ''
)

export const getAreaInformationType = createSelector(
    getService,
    service => service.get('areaInformationType') || ''
)

export const getAvailableLanguages = createSelector(
    getService,
    service => service.get('languagesAvailabilities') || Map()
)

export const getAreaInformationTypeId = createSelector(
    [getService, CommonSelectors.getAreaInformationTypeId],
    (service, defaultAreaInformationType) => service.get('areaInformationTypeId') || defaultAreaInformationType
)

export const getAreaTypeId = createSelector(
    getService,
    service => service.get('areaTypeId') || null
)

export const getDescription = createSelector(
    getService,
    service => service.get('description') || ''
)

export const getServiceUsage = createSelector(
    getService,
    service => service.get('serviceUsage') || ''
)

export const getUserInstruction = createSelector(
    getService,
    service => service.get('userInstruction') || ''
)

export const getPublishingStatus = createSelector(
    getServiceEntity,
    serviceEntity => serviceEntity.get('publishingStatusId') || null
)

export const getUnificRootId = createSelector(
    getService,
    service => service.get('unificRootId') || null
)

export const getMainOrganizationId = createSelector(
    getService,
    service => service.get('organizationId') || null
)

export const getChargeType = createSelector(
    getService,
    service => service.get('chargeType') || null
)

export const getServiceType = createSelector(
    getService,
    service => service.get('serviceType') || null
)

const getServiceTypeServiceId = CommonSelectors.getIdSelector(getServiceTypeSelector('service'))

export const getServiceTypeId = createSelector(
    [getService, getServiceTypeServiceId],
    (service, defaultId) => service.get('serviceTypeId') || defaultId
)

export const getFundingTypeId = createSelector(
    getService,
    service => service.get('fundingTypeId') || null
)

export const getSelectedServiceType = createSelector(
    [getServiceTypes, getServiceTypeId],
    (serviceTypes, serviceTypeId) => serviceTypes.get(serviceTypeId) || Map()
)

// export const getSelctedServiceTypeCode = createSelector(
//     getSelectedServiceType,
//     serviceType => serviceType.get('code') || ''
// )

export const getSelectedLanguages = createSelector(
    getService,
    service => service.get('languages') || null
)

export const getAdditionalInformation = createSelector(
    getService,
    service => service.get('additionalInformation') || ''
)

export const getAdditionalInformationDeadLine = createSelector(
    getService,
    service => service.get('additionalInformationDeadLine') || ''
)

export const getAdditionalInformationProcessingTime = createSelector(
    getService,
    service => service.get('additionalInformationProcessingTime') || ''
)

export const getAdditionalInformationValidityTime = createSelector(
    getService,
    service => service.get('additionalInformationValidityTime') || ''
)

// general description
export const getGeneralDescriptionId = createSelector(
    getService,
    service => service.get('generalDescription') || null
)

const getIsGeneralDescriptionIdValid = createSelector(
    getGeneralDescriptionId,
    generalDescriptionId => generalDescriptionId != null
)

const getServiceIsGeneralDescriptionAttached = createSelector(
    getService,
    service => service.get('isGeneralDescriptionAttached')
)

export const getIsGeneralDescriptionSelected = createSelector(
    [getServiceIsGeneralDescriptionAttached, getIsGeneralDescriptionIdValid],
    (isOptionSelected, generalDescriptionIdAttached) => (isOptionSelected == null)
        ? generalDescriptionIdAttached
        : isOptionSelected || false
)

export const getIsGeneralDescriptionSelectedAndAttached = createSelector(
    [getIsGeneralDescriptionSelected, getIsGeneralDescriptionIdValid],
    (isSelected, idAttached) => isSelected && idAttached
)

export const getGeneralDescriptionIdIfSelected = createSelector(
    [getIsGeneralDescriptionSelectedAndAttached, getGeneralDescriptionId],
    (isSelected, id) => isSelected ? id : null
)

export const getGeneralDescription = createSelector(
    [getGeneralDescriptionIdIfSelected, getGeneralDescriptions],
    (id, generalDescriptions) => generalDescriptions.get(id) || Map()
)

export const getServiceNameFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('name') || Map()
)

export const getServiceNameFromGeneralDescriptionLocale = createSelector(
    [getServiceNameFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (name, language) => name.get(language) || ''
)

export const getDescriptionFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('description') || Map()
)

export const getBackgroundDescriptionFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('backgroundDescription') || Map()
)

export const getDescriptionFromGeneralDescriptionLocale = createSelector(
    [getDescriptionFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (description, language) => description.get(language) || ''
)

export const getBackgroundDescriptionFromGeneralDescriptionLocale = createSelector(
    [getBackgroundDescriptionFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (description, language) => description.get(language) || ''
)

export const getServiceUsageFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceUsage') || Map()
)

export const getServiceUsageFromGeneralDescriptionLocale = createSelector(
    [getServiceUsageFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (conditionOfServiceUsage, language) => conditionOfServiceUsage.get(language) || ''
)

export const getUserInstructionFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('userInstruction') || Map()
)

export const getUserInstructionFromGeneralDescriptionLocale = createSelector(
    [getUserInstructionFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (serviceUserInstruction, language) => serviceUserInstruction.get(language) || ''
)

export const getChargeTypeInfoFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformation') || Map()
)

export const getChargeTypeInfoFromGeneralDescriptionLocale = createSelector(
    [getChargeTypeInfoFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (chargeTypeAdditionalInfo, language) => chargeTypeAdditionalInfo.get(language) || ''
)

export const getChargeTypeGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('chargeTypeId') || ''
)

export const getServiceTypeIdFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('typeId') || ''
)

export const getSelectedServiceTypeFromGeneralDescription = createSelector(
    [getServiceTypes, getServiceTypeIdFromGeneralDescription],
    (serviceTypes, serviceTypeId) => serviceTypes.get(serviceTypeId) || Map()
)

export const getDeadLineFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformationDeadLine') || Map()
)

export const getDeadLineFromGeneralDescriptionLocale = createSelector(
    [getDeadLineFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (deadLineAdditionalInfo, language) => deadLineAdditionalInfo.get(language) || ''
)

export const getProcessingTimeFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformationProcessingTime') || Map()
)

export const getProcessingTimeFromGeneralDescriptionLocale = createSelector(
    [getProcessingTimeFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (processingTimeAdditionalInfo, language) => processingTimeAdditionalInfo.get(language) || ''
)

export const getValidityTimeFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformationValidityTime') || Map()
)

export const getValidityTimeFromGeneralDescriptionLocale = createSelector(
    [getValidityTimeFromGeneralDescription, CommonSelectors.getLanguageParameter],
    (validityTimeAdditionalInfo, language) => validityTimeAdditionalInfo.get(language) || ''
)

export const getSelctedServiceTypeCode = createSelector(
    [getSelectedServiceType, getSelectedServiceTypeFromGeneralDescription],
    (serviceType, gdServiceType) => gdServiceType.get('code') || serviceType.get('code') || ''
)

export const getLawsFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('laws') || Map()
)

export const getSelectedLanguagesItemsJS = createSelector(
    [CommonSelectors.getLanguages, getSelectedLanguages],
    (allLanguages, selectedLanguages) => CommonSelectors.getEntitiesForIdsJS(allLanguages, selectedLanguages)
    // selectedLanguages && selectedLanguages.size > 0 ? CommonSelectors.getJS(selectedLanguages.map(id => allLanguages.get(id))) : null
)


export const getSelectedMunicipalities = createSelector(
    getService,
    service => service.get('municipalities') || List()
)

export const getServiceLaws = createSelector(
    getService,
    service => service.get('laws') || List()
)

export const getServiceLawsObjects = createSelector(
    [getServiceLaws, CommonSelectors.getLaws],
    (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids) || List()
)

export const getTranslationLanguageSaveCode = createSelector(
    CommonSelectors.getParameterFromProps('language'),
    code => code || 'fi',
)

export const getSelectedAreaMunicipality = createSelector(
    getService,
    service => service.get('areaMunicipality') || List()
)

export const getSelectedAreaBusinessRegions = createSelector(
    getService,
    service => service.get('areaBusinessRegions') || List()
)

export const getSelectedAreaHospitalRegions = createSelector(
    getService,
    service => service.get('areaHospitalRegions') || List()
)

export const getSelectedAreaProvince = createSelector(
    getService,
    service => service.get('areaProvince') || List()
)

export const getOrganizers = createSelector(
    getService,
    service => service.get('organizers') || List()
)

export const getProducers = createSelector(
    getService,
    service => service.get('serviceProducers') || List()
)

export const getServiceProducersEntities = createSelector(
    CommonSelectors.getEntities,
    search => search.get('serviceProducers') || Map()
)

export const getServiceProducers = createSelector(
    [getServiceProducersEntities, CommonSelectors.getLanguageParameter],
    (producers, language) => producers.map((producer) => producer.get(language))
)

export const getSelectedAreaCount = createSelector(
  [getSelectedAreaMunicipality,
    getSelectedAreaBusinessRegions,
    getSelectedAreaHospitalRegions,
    getSelectedAreaProvince],
    (municipality, business, hospital, provicies) => municipality.size + business.size + hospital.size + provicies.size
)

// Service vouchers
export const getVouchers = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('serviceVouchers') || Map()
)

export const getServiceVouchers = createSelector(
    getService,
    entities => entities.get('serviceVouchers') || Map()
)

export const getIsActiveVoucher = createSelector(
    getService,
    entities => entities.get('isActiveVoucher') || false
)


export const getServiceVouchersEntities = createSelector(
    [getVouchers, getServiceVouchers],
    (vouchers, serviceVouchers) => CommonSelectors.getEntitiesForIds(vouchers, serviceVouchers) || List()
)

export const getSortedServiceVouchers = createSelector(
    getServiceVouchersEntities,
    vouchers => vouchers.sortBy(voucher => voucher.get('orderNumber')).map(voucher => voucher.get('id')) || List()
)

export const getSaveStep1Model = createSelector(
  [
    getServiceId,
    getServiceName,
    getAlternateServiceName,
    getShortDescriptions,
    getDescription,
    getServiceUsage,
    getUserInstruction,
    getPublishingStatus,
    getChargeType,
    getServiceTypeId,
    getAreaInformationTypeId,
    getGeneralDescription,
    getSelectedLanguages,
    getAdditionalInformation,
    getAdditionalInformationDeadLine,
    getAdditionalInformationProcessingTime,
    getAdditionalInformationValidityTime,
    getTranslationLanguageSaveCode,
    getServiceLawsObjects,
    getSelectedAreaMunicipality,
    getSelectedAreaBusinessRegions,
    getSelectedAreaHospitalRegions,
    getSelectedAreaProvince,
    getOrganizers,
    getMainOrganizationId,
    getProducers,
    getServiceProducers,
    getFundingTypeId,
    getServiceVouchersEntities,
    getIsActiveVoucher
  ],
 (id,
    serviceName,
    alternateServiceName,
    shortDescriptions,
    description,
    serviceUsage,
    userInstruction,
    publishingStatus,
    chargeType,
    serviceTypeId,
    areaInformationTypeId,
    generalDescription,
    languages,
    additionalInformation,
    additionalInformationDeadLine,
    additionalInformationProcessingTime,
    additionalInformationValidityTime,
    language,
    laws,
    areaMunicipality,
    areaBusinessRegions,
    areaHospitalRegions,
    areaProvince,
    organizers,
    organizationId,
    producers,
    producerEntities,
    fundingTypeId,
    serviceVouchers,
    isActiveVoucher
    ) => (
   {
     id,
     serviceName,
     alternateServiceName,
     shortDescriptions,
     description,
     serviceUsage,
     userInstruction,
     publishingStatus,
     chargeType,
     serviceTypeId,
     areaInformationTypeId,
     generalDescription: { unificRootId: generalDescription.get('unificRootId'), typeId: generalDescription.get('typeId') },
     languages,
     additionalInformation,
     additionalInformationDeadLine,
     additionalInformationProcessingTime,
     additionalInformationValidityTime,
     language,
     laws,
     areaMunicipality,
     areaBusinessRegions,
     areaHospitalRegions,
     areaProvince,
     organizers,
     organizationId,
     serviceProducers: producers.map(p => producerEntities.get(p)),
     fundingTypeId,
     serviceVouchers: isActiveVoucher ? (serviceVouchers.size === 0
                                            ? [new Map({ orderNumber: 1 })]
                                            : serviceVouchers)
                                      : []
   })
)

// Step2
export const getKeyWordsInput = createSelector(
    getService,
    service => service.get('keyWordsInput') || ''
)

export const getKeyWordsId = createSelector(
    getService,
    service => service.get('keyWordsId') || 0
)

export const getSelectedTargetGroups = createSelector(
    getService,
    service => service.get('targetGroups') || List()
)

export const getOverrideTargetGroups = createSelector(
    getService,
    service => service.get('overrideTargetGroups') || List()
)

export const getIsSelectedTargetGroup = createSelector(
    [getSelectedTargetGroups, CommonSelectors.getIdFromProps],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getSelectedTargetGroupsFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('targetGroups') || List()
)

export const getOverrideTargetGroupsFormGeneralDescription = createSelector(
    [getSelectedTargetGroupsFormGeneralDescription, getOverrideTargetGroups],
    (gdTargetGroups, overTargetGroups) => gdTargetGroups.filter(gd => !overTargetGroups.includes(gd))
)

export const getIsAnyTargetGroupsSelected = createSelector(
    [getSelectedTargetGroups, getOverrideTargetGroupsFormGeneralDescription],
    (targetGroups, generalDescTg) => generalDescTg.size > 0 || targetGroups.size > 0
)

export const getIsSelectedTargetGroupFormGeneralDescription = createSelector(
    [getOverrideTargetGroupsFormGeneralDescription, CommonSelectors.getIdFromProps],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getIsSelectedTargetGroupWithGeneralDescription = createSelector(
    [getIsSelectedTargetGroup, getIsSelectedTargetGroupFormGeneralDescription],
    (selected, generalDescriptionSelected) => selected || generalDescriptionSelected
)

export const getTargetGrougKR2 = createSelector(
  getTargetGroups,
  targetGroups => targetGroups.find(tg => tg.get('code') === 'KR2') || Map()
)

export const getTargetGrougKR2Id = createSelector(
  getTargetGrougKR2,
  targetGroup => targetGroup.get('id')
)

export const getTargetGrougKR1 = createSelector(
  getTargetGroups,
  targetGroups => targetGroups.find(tg => tg.get('code') === 'KR1') || Map()
)

export const getTargetGrougKR1Id = createSelector(
  getTargetGrougKR1,
  targetGroup => targetGroup.get('id')
)

export const getSelectedServiceClasses = createSelector(
    getService,
    service => service.get('serviceClasses') || List()
)

export const getIsSelectedServiceClass = createSelector(
    [getSelectedServiceClasses, CommonSelectors.getParameterFromProps('nodeId')],
    (selectedServiceClass, id) => selectedServiceClass.includes(id)
)

export const getSelectedServiceClassesFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceClasses') || List()
)

export const getSelectedServiceClassesWithGeneralDescription = createSelector(
    [getSelectedServiceClasses, getSelectedServiceClassesFormGeneralDescription],
    (serviceClasses, generalDescSc) => OrderedSet(generalDescSc).union(serviceClasses)
)

export const getIsSelectedServiceClassFormGeneralDescription = createSelector(
    [getSelectedServiceClassesFormGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (selectedServiceClasses, id) => selectedServiceClasses.includes(id)
)

export const getIsSelectedServiceClassWithGeneralDescription = createSelector(
    [getSelectedServiceClassesWithGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (serviceClasses, id) => serviceClasses.includes(id)
)

export const getSelectedIndustrialClasses = createSelector(
    getService,
    service => service.get('industrialClasses') || List()
)

export const getIsSelectedIndustrialClass = createSelector(
    [getSelectedIndustrialClasses, CommonSelectors.getIdFromProps],
    (selectedIndustrialClass, id) => selectedIndustrialClass.includes(id)
)

export const getSelectedIndustrialClassesFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('industrialClasses') || List()
)

export const getSelectedIndustrialClassesWithGeneralDescription = createSelector(
    [getSelectedIndustrialClasses, getSelectedIndustrialClassesFormGeneralDescription],
    (industrialClasses, generalDescSc) => OrderedSet(generalDescSc).union(industrialClasses)
)

export const getIsSelectedIndustrialClassFormGeneralDescription = createSelector(
    [getSelectedIndustrialClassesFormGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (selectedIndustrialClasses, id) => selectedIndustrialClasses.includes(id)
)

export const getIsSelectedIndustrialClassWithGeneralDescription = createSelector(
    [getSelectedIndustrialClassesWithGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (industrialClasses, id) => industrialClasses.includes(id)
)

// OT
export const getSelectedOntologyTerms = createSelector(
    getService,
    service => service.get('ontologyTerms') || List()
)
// AT
export const getAnnotationOntologyTerms = createSelector(
    getService,
    service => service.get('annotationOntologyTerms') || List()
)

// GD
export const getSelectedOntologyTermsFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('ontologyTerms') || List()
)

// OT and GD
export const getSelectedOntologyTermsWithGeneralDescription = createSelector(
    [getSelectedOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (ontologyTerms, generalDescSc) => OrderedSet(generalDescSc).union(ontologyTerms)
)

// OT and GD and AT
export const getSelectedOntologyTermsWithGeneralDescriptionAnAnnotation = createSelector(
    [getSelectedOntologyTermsWithGeneralDescription, getAnnotationOntologyTerms],
    (ontologyTerms, annotations) => OrderedSet(ontologyTerms).union(annotations)
)

// AT and GD
export const getAnnotationOntologyTermsWithGeneralDescription = createSelector(
    [getAnnotationOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (annotations, generalDescSc) => OrderedSet(annotations).union(generalDescSc)
)

// AT - GD
export const getAnnotationWithoutGeneralDescription = createSelector(
    [getAnnotationOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (annotations, generalDescSc) => annotations.filter(x => !generalDescSc.includes(x))
)

// OT - GD - AT
export const getSelectedWithoutAnnotationOntologyTermsAndGeneralDescription = createSelector(
    [getSelectedOntologyTerms, getAnnotationOntologyTermsWithGeneralDescription],
    (ontologyTerms, annotationAndgeneralDescSc) => ontologyTerms.filter(x => !annotationAndgeneralDescSc.includes(x))
)

// OT + AT - GD
export const getSelectedWithAnnotationOntologyTermsWithoutGeneralDescription = createSelector(
    [getSelectedOntologyTerms, getAnnotationOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (ontologyTerms, annotation, generalDescription) => OrderedSet(ontologyTerms).union(annotation).subtract(generalDescription)
)

// selected AT and GD
export const getIsSelectedAnnotationAndGeneralDescription = createSelector(
    [getAnnotationOntologyTermsWithGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (selectedOntologyTerms, id) => selectedOntologyTerms.includes(id)
)

// selected GD
export const getIsSelectedOntologyGeneralDescription = createSelector(
    [getSelectedOntologyTermsFormGeneralDescription, CommonSelectors.getParameterFromProps('id')],
    (selectedOntologyTerms, id) => selectedOntologyTerms.includes(id)
)

// selected ALL
export const getIsSelectedOntologyTermsGeneralDescriptionAndAnnotation = createSelector(
    [getSelectedOntologyTermsWithGeneralDescriptionAnAnnotation, CommonSelectors.getParameterFromProps('nodeId')],
    (selectedOntologyTerms, id) => {
      return selectedOntologyTerms.includes(id)
    }
)

export const getSelectedLifeEvents = createSelector(
    getService,
    service => service.get('lifeEvents') || List()
)

export const getIsSelectedLifeEvent = createSelector(
    [getSelectedLifeEvents, CommonSelectors.getIdFromProps],
    (selectedLifeEvent, id) => selectedLifeEvent.includes(id)
)

export const getSelectedLifeEventsFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('lifeEvents') || List()
)

export const getSelectedLifeEventsWithGeneralDescription = createSelector(
    [getSelectedLifeEvents, getSelectedLifeEventsFormGeneralDescription],
    (lifeEvents, generalDescSc) => OrderedSet(generalDescSc).union(lifeEvents)
)

export const getIsSelectedLifeEventsFormGeneralDescription = createSelector(
    [getSelectedLifeEventsFormGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (selectedLifeEvents, id) => selectedLifeEvents.includes(id)
)

export const getIsSelectedLifeEventsWithGeneralDescription = createSelector(
    [getSelectedLifeEventsWithGeneralDescription, CommonSelectors.getParameterFromProps('nodeId')],
    (lifeEvents, id) => lifeEvents.includes(id)
)

export const getNewKeyWords = createSelector(
    getService,
    service => service.get('newKeyWords') || List()
)

export const getNewKeyWordsObjects = createSelector(
    [getNewKeyWords, getKeyWords],
    (newKeyWords, keyWords) => CommonSelectors.getEntitiesForIds(keyWords, newKeyWords) || List()
)

export const getSelectedKeyWords = createSelector(
    getService,
    service => service.get('keyWords') || List()
)

export const getSelectedKeyWordsWithNew = createSelector(
    [getSelectedKeyWords, getNewKeyWords],
    (selectedKeywords, newKeywords) => selectedKeywords.concat(newKeywords) || List()
)

const filterList = (list, exclude) => exclude.size > 0 ? list.filter(x => !exclude.includes(x)) : list

export const getSaveStep2Model = createSelector(
  [
    getSelectedTargetGroups,
    getOverrideTargetGroups,
    getSelectedServiceClasses,
    getSelectedWithAnnotationOntologyTermsWithoutGeneralDescription,
    getSelectedLifeEvents,
    getSelectedIndustrialClasses,
    getSelectedKeyWords,
    getNewKeyWordsObjects,
    getTranslationLanguageSaveCode,
    getSelectedTargetGroupsFormGeneralDescription,
    getSelectedServiceClassesFormGeneralDescription,
    getSelectedLifeEventsFormGeneralDescription,
    getSelectedIndustrialClassesFormGeneralDescription
  ],
    (
        targetGroups,
        overrideTargetGroups,
        serviceClasses,
        ontologyTerms,
        lifeEvents,
        industrialClasses,
        keyWords,
        newKeyWords,
        language,
        gdTargetGroups,
        gdServiceClasses,
        gdLifeEvents,
        gdIndustrialClasses
    ) => (
      {
        targetGroups: filterList(targetGroups, gdTargetGroups),
        overrideTargetGroups,
        serviceClasses: filterList(serviceClasses, gdServiceClasses),
        ontologyTerms: ontologyTerms,
        lifeEvents: filterList(lifeEvents, gdLifeEvents),
        industrialClasses: filterList(industrialClasses, gdIndustrialClasses),
        keyWords,
        newKeyWords,
        language
      }
    )
)

export const getSelectedTargetGroupsObjects = createSelector(
    [getSelectedTargetGroups, getOverrideTargetGroups, getTargetGroups],
    (ids, exIds, entities) => CommonSelectors.getEntitiesForIds(entities, filterList(ids, exIds)) || List()
)

export const getSelectedServiceClassesObjects = createSelector(
    [getSelectedServiceClasses, getServiceClasses],
    (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids) || List()
)

export const getSelectedLifeEventsObjects = createSelector(
    [getSelectedLifeEvents, getLifeEvents],
    (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids) || List()
)

export const getSelectedIndustrialClassesObjects = createSelector(
    [getSelectedIndustrialClasses, getIndustrialClasses],
    (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids) || List()
)

export const getServiceInfo = createSelector(
  [
    getServiceId,
    getServiceName,
    getShortDescriptions,
    getDescription,
    getSelectedTargetGroupsObjects,
    getSelectedServiceClassesObjects,
    getSelectedLifeEventsObjects,
    getSelectedIndustrialClassesObjects,
    getTranslationLanguageSaveCode
  ],
    (
        id,
        serviceName,
        shortDescriptions,
        description,
        targetGroupsObjects,
        serviceClassesObjects,
        lifeEventsObjects,
        industrialClassesObjects,
        languageCode
    ) => (
      {
        id,
        name : serviceName,
        shortDescriptions,
        description,
        targetGroups: targetGroupsObjects.map(item => item.get('name')),
        serviceClasses: serviceClassesObjects.map(item => item.get('name')),
        lifeEvents: lifeEventsObjects.map(item => item.get('name')),
        industrialClasses: industrialClassesObjects.map(item => item.get('name')),
        languageCode
      }
    )
)

// Step3

export const getFromServiceProducers = createSelector(
    [getServiceProducersEntities, CommonSelectors.getLanguageFromCode],
    (producers, language) => producers.map((producer) => producer.get(language))
)

export const getProducer = createSelector(
    [getServiceProducers, CommonSelectors.getParameterFromProps('id')],
    (producers, id) => producers.get(id) || List()
)

export const getSelectedProvisionType = createSelector(
    [getProducer, getProvisionTypes],
    (producer, provisionTypes) => provisionTypes.get(producer.get('provisionTypeId')) || Map()
)

export const getSelectedProducerOrganizationId = createSelector(
    getProducer,
    producer => producer.get('organizationId') || ''
)

export const getSelectedProducerAdditionalInformation = createSelector(
    getProducer,
    producer => producer.get('additionalInformation') || ''
)

export const getSelectedProvisionTypeCode = createSelector(
    getSelectedProvisionType,
    provisionType => provisionType.get('code')
)

export const getProducersEntities = createSelector(
    [getProducers, getServiceProducers],
    (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids) || List()
)

export const getSelectedProvisionTypes = createSelector(
    [getProducersEntities, getProvisionTypes],
    (producers, provisionTypes) => producers.map(producer => provisionTypes.get(producer.get('provisionTypeId'))) || Map()
)

export const isSelfProducerSelected = createSelector(
    getSelectedProvisionTypes,
    selectedTypes => selectedTypes.some(type => type && type.get('code').toLowerCase() === 'selfproduced')
)

export const getProvisionTypesSelfProducedId = CommonSelectors.getIdSelector(getProvisionTypesSelector('selfproduced'))

export const getServiceSelfProducer = createSelector(
    [getProducersEntities, getProvisionTypesSelfProducedId],
    (producers, selfId) => producers.first(producer => producer && producer.get('provisionTypeId') === selfId) || Map()
)

export const getServiceSelfProducerOrganizers = createSelector(
    getServiceSelfProducer,
    producer => producer.get('organizers') || List()
)

export const getServiceProducersDetail = createSelector(
    [getServiceProducers, CommonSelectors.getIdFromProps, CommonSelectors.getParameterFromProps('type')],
    (detail, id, type) => detail.get(id).get(type) || Map()

)

export const getFromServiceProducersDetail = createSelector(
    [getFromServiceProducers, CommonSelectors.getIdFromProps, CommonSelectors.getParameterFromProps('type')],
    (detail, id, type) => (detail.get(id) ? detail.get(id).get(type) : null) || Map()
)

export const getSelectedOrganizers = createSelector(
    getService,
    service => service.get('organizers') || List()
)

export const getSelectedOrganizersWithMainOrganization = createSelector(
    [getSelectedOrganizers, getMainOrganizationId],
    (selectedOrganizations, mainOrganizationId) => {
      return (mainOrganizationId && !selectedOrganizations.includes(mainOrganizationId))
         ? (List([mainOrganizationId]).toSet().union(selectedOrganizations.toSet())).toList()
         : selectedOrganizations
    }
)

export const getSelectedOrganizersItemsJS = createSelector(
    [getOrganizationEntities, getSelectedOrganizers, CommonSelectors.getParameterFromProps('withoutOrganizationId')],
    (organizations, selectedOrganizations, wId) => CommonSelectors.getObjectArray(selectedOrganizations
        .filter(id => id !== wId)
        .map(id => organizations.get(id))
        .sortBy(x => x.get('name'))
    )
)

export const getSelectedOrganizersItemsJSWithMainOrganization = createSelector(
    [getOrganizationEntities, getSelectedOrganizersWithMainOrganization],
    (organizations, selectedOrganizationsWithMainOrganization) => {
      return CommonSelectors.getEntitiesForIdsJS(organizations, selectedOrganizationsWithMainOrganization)
    }
)

export const getSelectedOrganizersItemsWithMainOrganizationJS = createSelector(
    getSelectedOrganizersItemsJSWithMainOrganization,
    selectedOrganizations => selectedOrganizations.filter(i => i != undefined)
)

export const getIsSelectedOrganizer = createSelector(
    [getSelectedOrganizers, CommonSelectors.getParameterFromProps('nodeId')],
    (organizers, id) => organizers.includes(id)
)

export const getProducersObjectArray = createSelector(
    getProducers,
    producers => CommonSelectors.getObjectArray(producers)
)

export const getSelectedServiceProducerOrganizers = createSelector(
    getProducer,
    (producer) => producer.get('organizers') || Map()
)

export const getSelectedServiceProducerOrganizersItemsJSNotFiltered = createSelector(
    [getOrganizationEntities, getSelectedServiceProducerOrganizers],
    (organizations, selectedOrganizations) => CommonSelectors.getEntitiesForIdsJS(organizations, selectedOrganizations)
)

export const getSelectedServiceProducerOrganizersItemsJS = createSelector(
    getSelectedServiceProducerOrganizersItemsJSNotFiltered,
    selectedOrganizations => selectedOrganizations.filter(i => i !== undefined)
)

// areas
export const getSelectedAreaMunicipalitiesJS = createSelector(
    [CommonSelectors.getMunicipalities, getSelectedAreaMunicipality],
    (allAreaMunicipalities, selectedAreaMunicipalities) => selectedAreaMunicipalities
      ? CommonSelectors.getJS(selectedAreaMunicipalities.map(id => allAreaMunicipalities.get(id)))
      : null
)
export const getOrderedSelectedAreaMunicipalitiesJS = createSelector(
    [CommonSelectors.getMunicipalities, getSelectedAreaMunicipality],
    (allAreaMunicipalities, selectedAreaMunicipalities) => selectedAreaMunicipalities
      ? CommonSelectors.getJS(
          selectedAreaMunicipalities.map(
            id => allAreaMunicipalities.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedProvinciesJS = createSelector(
    [CommonSelectors.getProvincies, getSelectedAreaProvince],
    (allProvincies, selectedProvincies) => selectedProvincies
      ? CommonSelectors.getJS(selectedProvincies.map(id => allProvincies.get(id)))
      : null
)
export const getOrderedSelectedProvinciesJS = createSelector(
    [CommonSelectors.getProvincies, getSelectedAreaProvince],
    (allProvincies, selectedProvincies) => selectedProvincies
      ? CommonSelectors.getJS(
          selectedProvincies.map(
            id => allProvincies.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedBusinessRegionsJS = createSelector(
    [CommonSelectors.getBusinessRegions, getSelectedAreaBusinessRegions],
    (allBusinessRegions, selectedBusinessRegions) => selectedBusinessRegions
      ? CommonSelectors.getJS(selectedBusinessRegions.map(id => allBusinessRegions.get(id)))
      : null
)
export const getOrderedSelectedBusinessRegionsJS = createSelector(
    [CommonSelectors.getBusinessRegions, getSelectedAreaBusinessRegions],
    (allBusinessRegions, selectedBusinessRegions) => selectedBusinessRegions
      ? CommonSelectors.getJS(
          selectedBusinessRegions.map(
            id => allBusinessRegions.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedHospitalRegionsJS = createSelector(
    [CommonSelectors.getHospitalRegions, getSelectedAreaHospitalRegions],
    (allHospitalRegions, selectedHospitalRegions) => selectedHospitalRegions
      ? CommonSelectors.getJS(selectedHospitalRegions.map(id => allHospitalRegions.get(id)))
      : null
)
export const getOrderedSelectedHospitalRegionsJS = createSelector(
    [CommonSelectors.getHospitalRegions, getSelectedAreaHospitalRegions],
    (allHospitalRegions, selectedHospitalRegions) => selectedHospitalRegions
      ? CommonSelectors.getJS(
          selectedHospitalRegions.map(
            id => allHospitalRegions.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedOrganizersMap = createSelector(
    [getOrganizationEntities, getFilteredOrganizations, getSelectedOrganizers],
    (allOrganizations, filteredOrganizations, selectedOrganizers) => {
      const mergedOrganziation = allOrganizations.mergeWith(merger, filteredOrganizations)
      return selectedOrganizers ? selectedOrganizers.map(id => mergedOrganziation.get(id)) : null
    }
)

export const getSaveStep3Model = createSelector(
  [
    getOrganizers,
    getProducers,
    getServiceProducers,
    getTranslationLanguageSaveCode
  ],
    (
        organizers,
        producers,
        producerEntities,
        language
    ) => (
      {
        organizers,
        serviceProducers: producers.map(p => producerEntities.get(p)),
        language: language
      }
    )

)

// Step4

// Step4 search box
export const getServiceChannelSearch = createSelector(
    CommonSelectors.getServiceReducers,
    services => Map()
)

export const getOrganizationId = createSelector(
    getService,
    search => search.get('serachOrganizationId') || ''
)

export const getChannelName = createSelector(
    getService,
    search => search.get('searchChannelName') || ''
)

export const getSelectedChannelTypes = createSelector(
    getService,
    search => search.get('searchChannelTypes') || new List()
)

export const getSelectedChannelTypesJS = createSelector(
    getSelectedChannelTypes,
    channelTypes => CommonSelectors.getJS(channelTypes, [])
)

export const getSelectedChannelTypesItemsJS = createSelector(
    [CommonSelectors.getChannelTypes, getSelectedChannelTypes],
    (allChannelTypes, selectedChannelTypes) => selectedChannelTypes
        ? CommonSelectors.getJS(selectedChannelTypes.map(id => allChannelTypes.get(id)))
        : null
)
// Step4 Search result
export const getStep4SearchedChannels = createSelector(
    CommonSelectors.getStepInnerSearchModel,
    search => search.get('channels') || List()
)
export const getStep4SearchedIsMoreThanMax = createSelector(
    CommonSelectors.getStepInnerSearchModel,
    innerSearchModel => innerSearchModel.get('isMoreThanMax') || false
)

export const getStep4SearchedIsNumberOfAllItems = createSelector(
    CommonSelectors.getStepInnerSearchModel,
    innerSearchModel => innerSearchModel.get('numberOfAllItems') || false
)

// Step4 channel data
// Step4 attached channes
export const getSelectedAttachedChannels = createSelector(
    getService,
    service => service.get('attachedChannels') || new List()
)

export const getSelectedAttachedChannelsJS = createSelector(
    getSelectedAttachedChannels,
    attachedChannel => CommonSelectors.getJS(attachedChannel, [])
)

export const getModelToSearch = createSelector(
    [getChannelName, getOrganizationId, getSelectedChannelTypes],
    (channelName, organizationId, selectedChannelTypes) => ({ channelName, organizationId, selectedChannelTypes })
)

export const getServiceConnections = createSelector(
    getService,
    service => service.get('connections') || List()
)

// Step4
