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
import { List, Map, Set } from 'immutable'
import { getEntities, getJS, getObjectArray, getServiceReducers, getEntitiesForIdsJS, getLaws,
    getLanguages, getChannelTypes, getMunicipalities, getResults, getPageModeState, getTranslationLanguages, getPageModeStateForKey, getLanguageFromCode,
    getParameterFromProps, getIdFromProps, getParameters, areStepDataValid, getStepInnerSearchModel, getEntitiesForIds, getTranslationLanguageId, getLanguageParameter } from '../../../Common/Selectors'
import { getServiceId, getProvisionTypes, getKeyWords, getServiceTypes, getTargetGroups, getServiceClasses, getIndustrialClasses, getLifeEvents } from '../../Common/Selectors'
import { getOrganizationEntities, getFilteredOrganizations } from '../../../Manage/Organizations/Common/Selectors'
import { getGeneralDescriptions } from '../../../Manage/GeneralDescriptions/Common/Selectors'
import shortid from 'shortid'
import { merger } from '../../../../Middleware'

export const getServicesEntities = createSelector(
    getEntities,
    search => search.get('services') || new Map()
)

export const getServices = createSelector(
    [getServicesEntities, getLanguageParameter],
    (services, language) => services.map((service) => service.get(language))
)

export const getService = createSelector(
    [getServices, getServiceId],
    (services, serviceId) => services.get(serviceId) || Map()
)
// Step1
export const getServiceName = createSelector(
    getService,
    service => service.get('serviceName') || ''
)

export const getAlternateServiceName = createSelector(
    getService,
    service => service.get('alternateServiceName') || ''
)

export const getShortDescriptions = createSelector(
    getService,
    service => service.get('shortDescriptions') || ''
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
    getService,
    service => service.get('publishingStatusId') || null
)

export const getUnificRootId = createSelector(
    getService,
    service => service.get('unificRootId') || null
)

export const getChargeType = createSelector(
    getService,
    service => service.get('chargeType') || null
)

export const getServiceType = createSelector(
    getService,
    service => service.get('serviceType') || null
)

export const getServiceTypeId = createSelector(
    getService,
    service => service.get('serviceTypeId') || null
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
    (isOptionSelected, generalDescriptionIdAttached) => (isOptionSelected == null) ? generalDescriptionIdAttached : isOptionSelected || false
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
    [getServiceNameFromGeneralDescription, getLanguageParameter],
    (name, language) => name.get(language) || ''
)

export const getDescriptionFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('description') || Map()
)

export const getDescriptionFromGeneralDescriptionLocale = createSelector(
    [getDescriptionFromGeneralDescription, getLanguageParameter],
    (description, language) => description.get(language) || ''
)

export const getServiceUsageFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('conditionOfServiceUsage') || Map()
)

export const getServiceUsageFromGeneralDescriptionLocale = createSelector(
    [getServiceUsageFromGeneralDescription, getLanguageParameter],
    (conditionOfServiceUsage, language) => conditionOfServiceUsage.get(language) || ''
)

export const getUserInstructionFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceUserInstruction') || Map()
)

export const getUserInstructionFromGeneralDescriptionLocale = createSelector(
    [getUserInstructionFromGeneralDescription, getLanguageParameter],
    (serviceUserInstruction, language) => serviceUserInstruction.get(language) || ''
)

export const getChargeTypeInfoFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('chargeTypeAdditionalInfo') || Map()
)

export const getChargeTypeInfoFromGeneralDescriptionLocale = createSelector(
    [getChargeTypeInfoFromGeneralDescription, getLanguageParameter],
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
    generalDescription => generalDescription.get('deadLineAdditionalInfo') || Map()
)

export const getDeadLineFromGeneralDescriptionLocale = createSelector(
    [getDeadLineFromGeneralDescription, getLanguageParameter],
    (deadLineAdditionalInfo, language) => deadLineAdditionalInfo.get(language) || ''
)

export const getProcessingTimeFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('processingTimeAdditionalInfo') || Map()
)

export const getProcessingTimeFromGeneralDescriptionLocale = createSelector(
    [getProcessingTimeFromGeneralDescription, getLanguageParameter],
    (processingTimeAdditionalInfo, language) => processingTimeAdditionalInfo.get(language) || ''
)

export const getValidityTimeFromGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('validityTimeAdditionalInfo') || Map()
)

export const getValidityTimeFromGeneralDescriptionLocale = createSelector(
    [getValidityTimeFromGeneralDescription, getLanguageParameter],
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
    [getLanguages, getSelectedLanguages],
    (allLanguages, selectedLanguages) => getEntitiesForIdsJS(allLanguages, selectedLanguages)
//    selectedLanguages && selectedLanguages.size > 0 ? getJS(selectedLanguages.map(id => allLanguages.get(id))) : null
)

export const getCoverageTypeId = createSelector(
    getService,
    service => service.get('serviceCoverageTypeId') || ''
)
export const getCoverageType = createSelector(
    getService,
    service => service.get('serviceCoverageType') || List()
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
    [getServiceLaws, getLaws],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getTranslationLanguageSaveCode = createSelector(
    getParameterFromProps('language'),
    code => code || 'fi',
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
    getGeneralDescription,
    getSelectedLanguages,
    getAdditionalInformation,
    getAdditionalInformationDeadLine,
    getAdditionalInformationProcessingTime,
    getAdditionalInformationValidityTime,
    getCoverageTypeId,
    getSelectedMunicipalities,
    getTranslationLanguageSaveCode,
    getServiceLawsObjects
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
    generalDescription,
    languages,
    additionalInformation,
    additionalInformationDeadLine,
    additionalInformationProcessingTime,
    additionalInformationValidityTime,
    serviceCoverageTypeId,
    municipalities,
    language,
    laws
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
     generalDescription: { id: generalDescription.get('id'), typeId: generalDescription.get('typeId') },
     languages,
     additionalInformation,
     additionalInformationDeadLine,
     additionalInformationProcessingTime,
     additionalInformationValidityTime,
     serviceCoverageTypeId,
     municipalities,
     language,
     laws
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
    [getSelectedTargetGroups, getIdFromProps],
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
    [getOverrideTargetGroupsFormGeneralDescription, getIdFromProps],
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

export const getSelectedServiceClasses = createSelector(
    getService,
    service => service.get('serviceClasses') || List()
)

export const getIsSelectedServiceClass = createSelector(
    [getSelectedServiceClasses, getParameterFromProps('nodeId')],
    (selectedServiceClass, id) => selectedServiceClass.includes(id)
)

export const getSelectedServiceClassesFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceClasses') || List()
)

export const getSelectedServiceClassesWithGeneralDescription = createSelector(
    [getSelectedServiceClasses, getSelectedServiceClassesFormGeneralDescription],
    (serviceClasses, generalDescSc) => Set(generalDescSc).union(serviceClasses)
)

export const getIsSelectedServiceClassFormGeneralDescription = createSelector(
    [getSelectedServiceClassesFormGeneralDescription, getParameterFromProps('nodeId')],
    (selectedServiceClasses, id) => selectedServiceClasses.includes(id)
)

export const getIsSelectedServiceClassWithGeneralDescription = createSelector(
    [getSelectedServiceClassesWithGeneralDescription, getParameterFromProps('nodeId')],
    (serviceClasses, id) => serviceClasses.includes(id)
)

export const getSelectedIndustrialClasses = createSelector(
    getService,
    service => service.get('industrialClasses') || List()
)

export const getIsSelectedIndustrialClass = createSelector(
    [getSelectedIndustrialClasses, getIdFromProps],
    (selectedIndustrialClass, id) => selectedIndustrialClass.includes(id)
)

export const getSelectedIndustrialClassesFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('industrialClasses') || List()
)

export const getSelectedIndustrialClassesWithGeneralDescription = createSelector(
    [getSelectedIndustrialClasses, getSelectedIndustrialClassesFormGeneralDescription],
    (industrialClasses, generalDescSc) => Set(generalDescSc).union(industrialClasses)
)

export const getIsSelectedIndustrialClassFormGeneralDescription = createSelector(
    [getSelectedIndustrialClassesFormGeneralDescription, getParameterFromProps('nodeId')],
    (selectedIndustrialClasses, id) => selectedIndustrialClasses.includes(id)
)

export const getIsSelectedIndustrialClassWithGeneralDescription = createSelector(
    [getSelectedIndustrialClassesWithGeneralDescription, getParameterFromProps('nodeId')],
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
    (ontologyTerms, generalDescSc) => Set(generalDescSc).union(ontologyTerms)
)

// OT and GD and AT
export const getSelectedOntologyTermsWithGeneralDescriptionAnAnnotation = createSelector(
    [getSelectedOntologyTermsWithGeneralDescription, getAnnotationOntologyTerms],
    (ontologyTerms, annotations) => Set(ontologyTerms).union(annotations)
)

// AT and GD
export const getAnnotationOntologyTermsWithGeneralDescription = createSelector(
    [getAnnotationOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (annotations, generalDescSc) => Set(annotations).union(generalDescSc)
)

// AT - GD
export const getAnnotationWithoutGeneralDescription = createSelector(
    [getAnnotationOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (annotations, generalDescSc) => annotations.filter(x => !generalDescSc.includes(x))
)

// OT - GD - AT
export const getSelectedWithoutAnnotationOntologyTermsAndGeneralDescription = createSelector(
    [getSelectedOntologyTerms, getAnnotationOntologyTermsWithGeneralDescription],
    (ontologyTerms, annotationAndgeneralDescSc) => ontologyTerms.filter(x=> !annotationAndgeneralDescSc.includes(x))
)

// OT + AT - GD
export const getSelectedWithAnnotationOntologyTermsWithoutGeneralDescription = createSelector(
    [getSelectedOntologyTerms, getAnnotationOntologyTerms, getSelectedOntologyTermsFormGeneralDescription],
    (ontologyTerms, annotation, generalDescription) => Set(ontologyTerms).union(annotation).subtract(generalDescription)
)

// selected AT and GD
export const getIsSelectedAnnotationAndGeneralDescription = createSelector(
    [getAnnotationOntologyTermsWithGeneralDescription, getParameterFromProps('nodeId')],
    (selectedOntologyTerms, id) => selectedOntologyTerms.includes(id)
)

// selected GD
export const getIsSelectedOntologyGeneralDescription = createSelector(
    [getSelectedOntologyTermsFormGeneralDescription, getParameterFromProps('id')],
    (selectedOntologyTerms, id) => selectedOntologyTerms.includes(id)
)

// selected ALL
export const getIsSelectedOntologyTermsGeneralDescriptionAndAnnotation = createSelector(
    [getSelectedOntologyTermsWithGeneralDescriptionAnAnnotation, getParameterFromProps('nodeId')],
    (selectedOntologyTerms, id) => {
      return selectedOntologyTerms.includes(id)
    }
)

export const getSelectedLifeEvents = createSelector(
    getService,
    service => service.get('lifeEvents') || List()
)

export const getIsSelectedLifeEvent = createSelector(
    [getSelectedLifeEvents, getIdFromProps],
    (selectedLifeEvent, id) => selectedLifeEvent.includes(id)
)

export const getSelectedLifeEventsFormGeneralDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('lifeEvents') || List()
)

export const getSelectedLifeEventsWithGeneralDescription = createSelector(
    [getSelectedLifeEvents, getSelectedLifeEventsFormGeneralDescription],
    (lifeEvents, generalDescSc) => Set(generalDescSc).union(lifeEvents)
)

export const getIsSelectedLifeEventsFormGeneralDescription = createSelector(
    [getSelectedLifeEventsFormGeneralDescription, getParameterFromProps('nodeId')],
    (selectedLifeEvents, id) => selectedLifeEvents.includes(id)
)

export const getIsSelectedLifeEventsWithGeneralDescription = createSelector(
    [getSelectedLifeEventsWithGeneralDescription, getParameterFromProps('nodeId')],
    (lifeEvents, id) => lifeEvents.includes(id)
)

export const getNewKeyWords = createSelector(
    getService,
    service => service.get('newKeyWords') || List()
)

export const getNewKeyWordsObjects = createSelector(
    [getNewKeyWords, getKeyWords],
    (newKeyWords, keyWords) => getEntitiesForIds(keyWords, newKeyWords) || List()
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
    [ getSelectedTargetGroups, getOverrideTargetGroups, getTargetGroups],
    (ids, exIds, entities) => getEntitiesForIds(entities, filterList(ids, exIds)) || List()
)

export const getSelectedServiceClassesObjects = createSelector(
    [ getSelectedServiceClasses, getServiceClasses],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getSelectedLifeEventsObjects = createSelector(
    [ getSelectedLifeEvents, getLifeEvents],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getSelectedIndustrialClassesObjects = createSelector(
    [ getSelectedIndustrialClasses, getIndustrialClasses],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
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
export const getServiceProducersEntities = createSelector(
    getEntities,
    search => search.get('serviceProducers') || Map()
)

export const getServiceProducers = createSelector(
    [getServiceProducersEntities, getLanguageParameter],
    (producers, language) => producers.map((producer) => producer.get(language))
)

export const getFromServiceProducers = createSelector(
    [getServiceProducersEntities, getLanguageFromCode],
    (producers, language) => producers.map((producer) => producer.get(language))
)

export const getProducers = createSelector(
    getService,
    service => service.get('serviceProducers') || List()
)

export const getProducer = createSelector(
    [getServiceProducers, getParameterFromProps('id')],
    (producers, id) => producers.get(id) || List()
)

export const getSelectedProvisionType = createSelector(
    [getProducer, getProvisionTypes],
    (producer, provisionTypes) => provisionTypes.get(producer.get('provisionTypeId')) || Map()
)

export const getSelectedProvisionTypeCode = createSelector(
    getSelectedProvisionType,
    provisionType => provisionType.get('code')
)

export const getOrganizers = createSelector(
    getService,
    service => service.get('organizers') || List()
)

export const getServiceProducersDetail = createSelector(
    [ getServiceProducers, getIdFromProps, getParameterFromProps('type')],
    (detail, id, type) => detail.get(id).get(type) || Map()

)

export const getFromServiceProducersDetail = createSelector(
    [ getFromServiceProducers, getIdFromProps, getParameterFromProps('type')],
    (detail, id, type) => (detail.get(id) ? detail.get(id).get(type) : null) || Map()
)

export const getSelectedOrganizers = createSelector(
    getService,
    service => service.get('organizers') || List()
)

export const getIsSelectedOrganizer = createSelector(
    [getSelectedOrganizers, getParameterFromProps('nodeId')],
    (organizers, id) => organizers.includes(id)
)

export const getProducersObjectArray = createSelector(
    getProducers,
    producers => getObjectArray(producers)
)

export const getSelectedMunicipalitiesJS = createSelector(
    [getMunicipalities, getSelectedMunicipalities],
    (allMunicipalities, selectedMunicipalites) => selectedMunicipalites
      ? getJS(selectedMunicipalites.map(id => allMunicipalities.get(id)))
      : null
)
export const getOrderedSelectedMunicipalitiesJS = createSelector(
    [getMunicipalities, getSelectedMunicipalities],
    (allMunicipalities, selectedMunicipalites) => selectedMunicipalites
      ? getJS(
          selectedMunicipalites.map(
            id => allMunicipalities.get(id)
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
    getServiceReducers,
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
    channelTypes => getJS(channelTypes, [])
)

export const getSelectedChannelTypesItemsJS = createSelector(
    [getChannelTypes, getSelectedChannelTypes],
    (allChannelTypes, selectedChannelTypes) => selectedChannelTypes ? getJS(selectedChannelTypes.map(id => allChannelTypes.get(id))) : null
)
// Step4 Search result
export const getStep4SearchedChannels = createSelector(
    getStepInnerSearchModel,
    search => search.get('channels') || List()
)
export const getStep4SearchedIsMoreThanMax = createSelector(
    getStepInnerSearchModel,
    innerSearchModel => innerSearchModel.get('isMoreThanMax') || false
)

export const getStep4SearchedIsNumberOfAllItems = createSelector(
    getStepInnerSearchModel,
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
    attachedChannel => getJS(attachedChannel, [])
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
