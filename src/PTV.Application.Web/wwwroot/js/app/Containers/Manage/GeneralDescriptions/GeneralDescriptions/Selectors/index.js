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
import { getLanguageParameter, getLaws,
         getEntitiesForIds, getIdFromProps,
         getParameterFromProps, getIdSelector } from '../../../../Common/Selectors'
import { getGeneralDescriptions, getGeneralDescriptionId } from '../../Common/Selectors'
import { getServiceTypes, getTargetGroups,
         getServiceClasses, getIndustrialClasses,
         getLifeEvents, getServiceTypeSelector, getTargetGroupSelector } from '../../../../Services/Common/Selectors'

export const getGeneralDescription = createSelector(
    [getGeneralDescriptions, getGeneralDescriptionId],
    (generalGescriptions, generalDescriptionId) =>
    generalGescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionNameMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('name') || Map()
)

export const getGeneralDescriptionName = createSelector(
    [getGeneralDescriptionNameMap, getLanguageParameter],
    (name, language) => name.get(language) || ''
)

export const getGeneralDescriptionShortDescriptionMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('shortDescription') || Map()
)

export const getGeneralDescriptionShortDescription = createSelector(
    [getGeneralDescriptionShortDescriptionMap, getLanguageParameter],
    (shortDescription, language) => shortDescription.get(language) || ''
)

export const getGeneralDescriptionDescriptionMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('description') || Map()
)

export const getGeneralDescriptionDescription = createSelector(
    [getGeneralDescriptionDescriptionMap, getLanguageParameter],
    (description, language) => description.get(language) || ''
)

export const getGeneralDescriptionServiceUsageMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceUsage') || Map()
)

export const getGeneralDescriptionServiceUsage = createSelector(
    [getGeneralDescriptionServiceUsageMap, getLanguageParameter],
    (serviceUsage, language) => serviceUsage.get(language) || ''
)

export const getGeneralDescriptionUserInstructionMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('userInstruction') || Map()
)

export const getGeneralDescriptionUserInstruction = createSelector(
    [getGeneralDescriptionUserInstructionMap, getLanguageParameter],
    (userInstruction, language) => userInstruction.get(language) || ''
)

export const getGeneralDescriptionBackgroundDescriptionMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('backgroundDescription') || Map()
)

export const getGeneralDescriptionBackgroundDescription = createSelector(
    [getGeneralDescriptionBackgroundDescriptionMap, getLanguageParameter],
    (backgroundDescription, language) => backgroundDescription.get(language) || ''
)

export const getGeneralDescriptionAdditionalInformationMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformation') || Map()
)

export const getGeneralDescriptionAdditionalInformation = createSelector(
    [getGeneralDescriptionAdditionalInformationMap, getLanguageParameter],
    (additionalInformation, language) => additionalInformation.get(language) || ''
)

export const getGeneralDescriptionAdditionalInformationDeadLineMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformationDeadLine') || Map()
)

export const getGeneralDescriptionAdditionalInformationDeadLine = createSelector(
    [getGeneralDescriptionAdditionalInformationDeadLineMap, getLanguageParameter],
    (additionalInformationDeadLine, language) => additionalInformationDeadLine.get(language) || ''
)

export const getGeneralDescriptionAdditionalInformationProcessingTimeMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformationProcessingTime') || Map()
)

export const getGeneralDescriptionAdditionalInformationProcessingTime = createSelector(
    [getGeneralDescriptionAdditionalInformationProcessingTimeMap, getLanguageParameter],
    (additionalInformationProcessingTime, language) => additionalInformationProcessingTime.get(language) || ''
)

export const getGeneralDescriptionAdditionalInformationValidityTimeMap = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('additionalInformationValidityTime') || Map()
)

export const getGeneralDescriptionAdditionalInformationValidityTime = createSelector(
    [getGeneralDescriptionAdditionalInformationValidityTimeMap, getLanguageParameter],
    (additionalInformationValidityTime, language) => additionalInformationValidityTime.get(language) || ''
)

const getServiceTypeServiceId = getIdSelector(getServiceTypeSelector('service'))

export const getGeneralDescriptionServiceTypeId = createSelector(
    [getGeneralDescription, getServiceTypes, getServiceTypeServiceId],
    (generalDescription, serviceTypes, defaultId) =>
        serviceTypes.has(generalDescription.get('typeId')) ? generalDescription.get('typeId') : defaultId
)

export const getSelectedGeneralDescriptionServiceType = createSelector(
    [getServiceTypes, getGeneralDescriptionServiceTypeId],
    (serviceTypes, typeId) => serviceTypes.get(typeId) || Map()
)

export const getSelectedServiceTypeCode = createSelector(
    getSelectedGeneralDescriptionServiceType,
    serviceType => serviceType.get('code') || ''
)

export const getGeneralDescriptionChargeTypeId = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('chargeTypeId') || null
)

export const getPublishingStatus = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('publishingStatusId') || null
)

export const getUnificRootId = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('unificRootId') || null
)

export const getGeneralDescriptionLaws = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('laws') || List()
)

export const getGeneralDescriptionLawsObjects = createSelector(
    [getGeneralDescriptionLaws, getLaws],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getSelectedTargetGroups = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('targetGroups') || List()
)

export const getIsSelectedTargetGroup = createSelector(
    [getSelectedTargetGroups, getIdFromProps],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getIsAnyTargetGroupsSelected = createSelector(
    getSelectedTargetGroups,
    targetGroups => targetGroups.size > 0
)

const getTagetGroupKR2Id = getIdSelector(getTargetGroupSelector('kr2'))
const getTagetGroupKR1Id = getIdSelector(getTargetGroupSelector('kr1'))

export const getIsSelectedTargetGroupKR1 = createSelector(
    [getSelectedTargetGroups, getTagetGroupKR1Id],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getIsSelectedTargetGroupKR2 = createSelector(
    [getSelectedTargetGroups, getTagetGroupKR2Id],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getSelectedServiceClasses = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceClasses') || List()
)

export const getIsSelectedServiceClass = createSelector(
    [getSelectedServiceClasses, getParameterFromProps('nodeId')],
    (selectedServiceClass, id) => selectedServiceClass.includes(id)
)

export const getSelectedLifeEvents = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('lifeEvents') || List()
)

export const getIsSelectedLifeEvent = createSelector(
    [getSelectedLifeEvents, getParameterFromProps('nodeId')],
    (selectedLifeEvent, id) => selectedLifeEvent.includes(id)
)

export const getSelectedIndustrialClasses = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('industrialClasses') || List()
)

export const getIsSelectedIndustrialClass = createSelector(
    [getSelectedIndustrialClasses, getParameterFromProps('nodeId')],
    (selectedIndustrialClass, id) => selectedIndustrialClass.includes(id)
)

// OT
export const getSelectedOntologyTerms = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('ontologyTerms') || List()
)
// AT
export const getAnnotationOntologyTerms = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('annotationOntologyTerms') || List()
)

// OT and AT
export const getSelectedWithAnnotationOntologyTerms = createSelector(
    [getAnnotationOntologyTerms, getSelectedOntologyTerms],
    (annotations, ontologyTerms) => Set(annotations).union(ontologyTerms)
)

// OT - AT
export const getSelectedWithoutAnnotationOntologyTerms = createSelector(
    [getSelectedOntologyTerms, getAnnotationOntologyTerms],
    (ontologyTerms, annotations) => ontologyTerms.filter(x => !annotations.includes(x))
)

// is selected Ontology term
export const getIsSelectedOntologyTerms = createSelector(
    [getSelectedWithAnnotationOntologyTerms, getParameterFromProps('nodeId')],
    (selectedOntologyTerms, id) => {
      return selectedOntologyTerms.includes(id)
    }
)

export const getSelectedTargetGroupsObjects = createSelector(
    [getSelectedTargetGroups, getTargetGroups],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getSelectedServiceClassesObjects = createSelector(
    [getSelectedServiceClasses, getServiceClasses],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getSelectedLifeEventsObjects = createSelector(
    [getSelectedLifeEvents, getLifeEvents],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getSelectedIndustrialClassesObjects = createSelector(
    [getSelectedIndustrialClasses, getIndustrialClasses],
    (ids, entities) => getEntitiesForIds(entities, ids) || List()
)

export const getTranslationLanguageSaveCode = createSelector(
    getParameterFromProps('language'),
    code => code || 'fi',
)

export const getGeneralDescriptionInfo = createSelector(
  [
    getGeneralDescriptionId,
    getGeneralDescriptionName,
    getGeneralDescriptionShortDescription,
    getGeneralDescriptionDescription,
    getSelectedTargetGroupsObjects,
    getSelectedServiceClassesObjects,
    getSelectedLifeEventsObjects,
    getSelectedIndustrialClassesObjects,
    getTranslationLanguageSaveCode,
    getIsSelectedTargetGroupKR1,
    getIsSelectedTargetGroupKR2
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
        languageCode,
        useLifeEvents,
        useIndustrialClasses
    ) => (
      {
        id,
        name : serviceName,
        shortDescriptions,
        description,
        targetGroups: targetGroupsObjects.map(item => item.get('name')),
        serviceClasses: serviceClassesObjects.map(item => item.get('name')),
        lifeEvents: useLifeEvents ? lifeEventsObjects.map(item => item.get('name')) : [],
        industrialClasses: useIndustrialClasses ? industrialClassesObjects.map(item => item.get('name')) : [],
        languageCode
      }
    )
)

export const getSaveStep1Model = createSelector(
  [
    getGeneralDescriptionId,
    getGeneralDescriptionNameMap,
    getGeneralDescriptionShortDescriptionMap,
    getGeneralDescriptionDescriptionMap,
    getGeneralDescriptionServiceUsageMap,
    getGeneralDescriptionUserInstructionMap,
    getPublishingStatus,
    getGeneralDescriptionServiceTypeId,
    getGeneralDescriptionAdditionalInformationMap,
    getGeneralDescriptionAdditionalInformationDeadLineMap,
    getGeneralDescriptionAdditionalInformationProcessingTimeMap,
    getGeneralDescriptionAdditionalInformationValidityTimeMap,
    getGeneralDescriptionBackgroundDescriptionMap,
    getGeneralDescriptionLawsObjects,
    getGeneralDescriptionChargeTypeId,
    getSelectedTargetGroups,
    getSelectedServiceClasses,
    getSelectedWithAnnotationOntologyTerms,
    getSelectedLifeEvents,
    getSelectedIndustrialClasses,
    getTranslationLanguageSaveCode,
    getIsSelectedTargetGroupKR1,
    getIsSelectedTargetGroupKR2
  ],
 (id,
    name,
    shortDescription,
    description,
    serviceUsage,
    userInstruction,
    publishingStatus,
    typeId,
    additionalInformation,
    additionalInformationDeadLine,
    AdditionalInformationProcessingTime,
    additionalInformationValidityTime,
    backgroundDescription,
    laws,
    chargeTypeId,
    targetGroups,
    serviceClasses,
    ontologyTerms,
    lifeEvents,
    industrialClasses,
    Language,
    saveLifeEvents,
    saveIndustrialClasses
    ) => (
   {
     id,
     name,
     shortDescription,
     description,
     serviceUsage,
     userInstruction,
     publishingStatus,
     typeId,
     additionalInformation,
     additionalInformationDeadLine,
     AdditionalInformationProcessingTime,
     additionalInformationValidityTime,
     backgroundDescription,
     laws,
     chargeTypeId,
     targetGroups,
     serviceClasses,
     ontologyTerms,
     lifeEvents : saveLifeEvents ? lifeEvents : [],
     industrialClasses: saveIndustrialClasses ? industrialClasses : [],
     Language
   })

)
