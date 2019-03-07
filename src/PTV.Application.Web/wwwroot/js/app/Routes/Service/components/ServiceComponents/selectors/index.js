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
import { Map, List } from 'immutable'
import { getFormValue } from 'selectors/base'
import {
  getContentLanguageCode,
  getContentLanguageId,
  getSelectedComparisionLanguageCode,
  getSelectedComparisionLanguageId
} from 'selectors/selections'
import { EntitySelectors } from 'selectors'
import { getPublishingStatusPublishedId } from 'selectors/common'
import { EditorState, convertFromRaw } from 'draft-js'

// form selectors
export const getFormGeneralDescriptionId = createSelector(
  getFormValue('generalDescriptionId'),
  values => values || ''
)

// GeneralDescription selectors
export const getIsGeneralDescriptionAttached = createSelector(
  getFormGeneralDescriptionId,
  generalDescriptionId => !!generalDescriptionId || false
)

export const getGeneralDescription = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionlanguagesAvailabilities = createSelector(
  getGeneralDescription, gd => gd.get('languagesAvailabilities') || Map()
)

export const getIsGDAvailableInContentLanguage = createSelector(
  [getGeneralDescriptionlanguagesAvailabilities, getContentLanguageId, getPublishingStatusPublishedId],
  (gdLanguages, contentLanguage, publishedId) => {
    return gdLanguages.some(l => l.get('languageId') === contentLanguage && l.get('statusId') === publishedId)
  }
)

export const getGDPublishedLanguagesIds = createSelector(
  [getGeneralDescriptionlanguagesAvailabilities, getPublishingStatusPublishedId],
  (gdLanguages, publishedId) => {
    return gdLanguages && gdLanguages
      .filter(l => l.get('statusId') === publishedId)
      .map((v, k) => v.get('languageId')) || Map()
  }
)

export const getGDPublishedLanguages = createSelector(
  [getGDPublishedLanguagesIds, EntitySelectors.languages.getEntities],
  (ids, languages) => {
    const langs = languages.filter(lang => ids.contains(lang.get('id'))).toList()
    return langs.map((v, k) => v.get('code')) || Map()
  }
)

export const getIsGDAvailableInCompareLanguage = createSelector(
  [getGeneralDescriptionlanguagesAvailabilities, getSelectedComparisionLanguageId, getPublishingStatusPublishedId],
  (gdLanguages, contentLanguage, publishedId) =>
    gdLanguages.some(l => l.get('languageId') === contentLanguage && l.get('statusId') === publishedId)
)

export const getGeneralDescriptionName = createSelector(
  [getGeneralDescription, getContentLanguageCode],
  (generalDescription, contentLanguageCode) => generalDescription.get('name') &&
    generalDescription.get('name').get(contentLanguageCode) || ''
)

export const getGeneralDescriptionCompareName = createSelector(
  [getGeneralDescription, getSelectedComparisionLanguageCode],
  (generalDescription, compareLanguageCode) => generalDescription.get('name') &&
    generalDescription.get('name').get(compareLanguageCode) || ''
)

// service type selectors
export const getGeneralDescriptionServiceType = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('serviceType') || ''
)

// ChargeType selectors
export const getGeneralDescriptionChargeType = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('chargeType') || Map()
)

export const getGeneralDescriptionChargeTypeType = createSelector(
  getGeneralDescriptionChargeType,
  chargeType => chargeType.get('chargeType') || ''
)

export const getGeneralDescriptionChargeTypeAdditionalInformation = createSelector(
  getGeneralDescriptionChargeType,
  chargeType => chargeType.get('additionalInformation') || Map({ fi: '' })
)

// DescriptionGD ---> START
export const getGeneralDescriptionDescriptionData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('description') || Map()
)
export const getIsGDDescription = createSelector(
  [getGeneralDescriptionDescriptionData, getContentLanguageCode],
  (description, lang) => {
    const langContent = description.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getIsGDDescriptionCompare = createSelector(
  [getGeneralDescriptionDescriptionData, getSelectedComparisionLanguageCode],
  (description, lang) => {
    const langContent = description.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getGDDescriptionValue = createSelector(
  [getGeneralDescriptionDescriptionData, getContentLanguageCode, getIsGDDescription],
  (description, lang, hasValue) => {
    if (hasValue) {
      const editorState = description && description.size > 0 &&
        description.map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
export const getGDDescriptionCompareValue = createSelector(
  [getGeneralDescriptionDescriptionData, getSelectedComparisionLanguageCode, getIsGDDescriptionCompare],
  (description, lang, hasValue) => {
    if (hasValue) {
      const editorState = description && description.size > 0 &&
        description.map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
// DescriptionGD ---> END

// UserInstructionGD ---> START
export const getGDgetGDUserInstructionValueData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('userInstruction') || Map()
)
export const getIsGDUserInstruction = createSelector(
  [getGDgetGDUserInstructionValueData, getContentLanguageCode],
  (userInstruction, lang) => {
    const langContent = userInstruction.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getIsGDUserInstructionCompare = createSelector(
  [getGDgetGDUserInstructionValueData, getSelectedComparisionLanguageCode],
  (userInstruction, lang) => {
    const langContent = userInstruction.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getGDUserInstructionValue = createSelector(
  [getGDgetGDUserInstructionValueData, getContentLanguageCode, getIsGDUserInstruction],
  (userInstruction, lang, hasValue) => {
    if (hasValue) {
      const editorState = userInstruction && userInstruction.size > 0 &&
      userInstruction.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
export const getGDUserInstructionCompareValue = createSelector(
  [getGDgetGDUserInstructionValueData, getSelectedComparisionLanguageCode, getIsGDUserInstructionCompare],
  (userInstruction, lang, hasValue) => {
    if (hasValue) {
      const editorState = userInstruction && userInstruction.size > 0 &&
        userInstruction.map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
// UserInstructionGD ---> END

// ConditionOfServiceUsageGD ---> START
export const getGDgetGDConditionOfServiceUsageValueData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('conditionOfServiceUsage') || Map()
)
export const getIsGDConditionOfServiceUsage = createSelector(
  [getGDgetGDConditionOfServiceUsageValueData, getContentLanguageCode],
  (condition, lang) => {
    const langContent = condition.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getIsGDConditionOfServiceUsageCompare = createSelector(
  [getGDgetGDConditionOfServiceUsageValueData, getSelectedComparisionLanguageCode],
  (condition, lang) => {
    const langContent = condition.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getGDConditionOfServiceUsageValue = createSelector(
  [getGDgetGDConditionOfServiceUsageValueData, getContentLanguageCode, getIsGDConditionOfServiceUsage],
  (condition, lang, hasValue) => {
    if (hasValue) {
      const editorState = condition && condition.size > 0 &&
      condition.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
export const getGDConditionOfServiceUsageCompareValue = createSelector(
  [
    getGDgetGDConditionOfServiceUsageValueData,
    getSelectedComparisionLanguageCode,
    getIsGDConditionOfServiceUsageCompare
  ],
  (condition, lang, hasValue) => {
    if (hasValue) {
      const editorState = condition && condition.size > 0 &&
        condition.map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
// ConditionOfServiceUsageGD ---> END

// ChargeTypeAdditionalInfoGD ---> START
export const getGDChargeTypeAdditionalInformation = createSelector(
  getGeneralDescriptionChargeType,
  chargeType => chargeType.get('additionalInformation') || Map()
)
export const getIsGDChargeTypeAdditionalInformation = createSelector(
  getGDChargeTypeAdditionalInformation,
  info => {
    const editorState = info && info.size > 0 &&
    info || Map({ fi: '' })
    return editorState
  }
)
export const getGDChargeTypeAdditionalInformationValue = createSelector(
  [getIsGDChargeTypeAdditionalInformation, getContentLanguageCode],
  (info, languageCode) => info.get(languageCode)
)
export const getGDChargeTypeAdditionalInformationCompareValue = createSelector(
  [getIsGDChargeTypeAdditionalInformation, getSelectedComparisionLanguageCode],
  (info, languageCode) => info.get(languageCode)
)
// ChargeTypeAdditionalInfoGD ---> END

// BackgroundInfoAndLawsGD ---> START
export const getGeneralDescriptionBackgroundDescriptionData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('backgroundDescription') || Map()
)
export const getIsGDBackgroundDescription = createSelector(
  [getGeneralDescriptionBackgroundDescriptionData, getContentLanguageCode],
  (description, lang) => {
    const langContent = description.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getIsGDBackgroundDescriptionCompare = createSelector(
  [getGeneralDescriptionBackgroundDescriptionData, getSelectedComparisionLanguageCode],
  (description, lang) => {
    const langContent = description.get(lang) || null
    const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
    return (raw && raw.getCurrentContent().hasText()) || false
  }
)
export const getGDBackgroundDescriptionValue = createSelector(
  [getGeneralDescriptionBackgroundDescriptionData, getContentLanguageCode, getIsGDBackgroundDescription],
  (description, lang, hasValue) => {
    if (hasValue) {
      const editorState = description && description.size > 0 &&
        description.map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
export const getGDBackgroundDescriptionCompareValue = createSelector(
  [
    getGeneralDescriptionBackgroundDescriptionData,
    getSelectedComparisionLanguageCode,
    getIsGDBackgroundDescriptionCompare
  ],
  (description, lang, hasValue) => {
    if (hasValue) {
      const editorState = description && description.size > 0 &&
        description.map(l =>
          l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)
export const getGeneralDescriptionLaws = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('laws') || List()
)
export const getIsAnyGDLawInContentLanguage = createSelector(
  [getGeneralDescriptionLaws, getContentLanguageCode],
  (laws, lang) => laws.some(law => {
    const url = law.get('urlAddress') || Map()
    return url.get(lang)
  })
)
export const getIsAnyGDLawInCompareLanguage = createSelector(
  [getGeneralDescriptionLaws, getSelectedComparisionLanguageCode],
  (laws, lang) => laws.some(law => {
    const url = law.get('urlAddress') || Map()
    return url.get(lang)
  })
)
// BackgroundInfoAndLawsGD ---> END

// DeadlineAdditionalInfoGD ---> START
export const getGeneralDeadLineAdditionalInfoData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('deadLineInformation') || Map()
)

export const getIsGeneralDeadLineAdditionalInfo = createSelector(
  getGeneralDeadLineAdditionalInfoData,
  deadLine => {
    const editorState = deadLine && deadLine.size > 0 &&
    deadLine || Map({ fi: '' })
    return editorState
  }
)

export const getGDDeadLineAdditionalInfoValue = createSelector(
  [getIsGeneralDeadLineAdditionalInfo, getContentLanguageCode],
  (deadLine, languageCode) => deadLine.get(languageCode) || ''
)

export const getGDDeadLineAdditionalInfoCompareValue = createSelector(
  [getIsGeneralDeadLineAdditionalInfo, getSelectedComparisionLanguageCode],
  (deadLine, languageCode) => deadLine.get(languageCode) || ''
)
// DeadlineAdditionalInfoGD ---> END

// ProcessingTimeAdditionalInfoGD ---> START
export const getGeneralDescriptionProcessingTimeAdditionalInfoData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('processingTimeInformation') || Map()
)

export const getIsGeneralDescriptionProcessingTimeAdditionalInfo = createSelector(
  getGeneralDescriptionProcessingTimeAdditionalInfoData,
  processingTime => {
    const editorState = processingTime && processingTime.size > 0 &&
    processingTime || Map({ fi: '' })
    return editorState
  }
)

export const getGDProcessingTimeAdditionalInfoValue = createSelector(
  [getIsGeneralDescriptionProcessingTimeAdditionalInfo, getContentLanguageCode],
  (processingTime, languageCode) => processingTime.get(languageCode) || ''
)

export const getGDProcessingTimeAdditionalInfoCompareValue = createSelector(
  [getIsGeneralDescriptionProcessingTimeAdditionalInfo, getSelectedComparisionLanguageCode],
  (processingTime, languageCode) => processingTime.get(languageCode) || ''
)
// ProcessingTimeAdditionalInfoGD ---> END

// ValidityTimeAdditionalInfoGD ---> START
export const getGeneralDescriptionValidityTimeAdditionalInfoData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('validityTimeInformation') || Map()
)

export const getGeneralDescriptionValidityTimeAdditionalInfo = createSelector(
  getGeneralDescriptionValidityTimeAdditionalInfoData,
  validity => {
    const editorState = validity && validity.size > 0 &&
      validity || Map({ fi: '' })
    return editorState
  }
)

export const getGDValidityTimeAdditionalInfoValue = createSelector(
  [getGeneralDescriptionValidityTimeAdditionalInfo, getContentLanguageCode],
  (validity, languageCode) => validity.get(languageCode) || ''
)

export const getGDValidityTimeAdditionalInfoCompareValue = createSelector(
  [getGeneralDescriptionValidityTimeAdditionalInfo, getSelectedComparisionLanguageCode],
  (validity, languageCode) => validity.get(languageCode) || ''
)
// ValidityTimeAdditionalInfoGD ---> END
