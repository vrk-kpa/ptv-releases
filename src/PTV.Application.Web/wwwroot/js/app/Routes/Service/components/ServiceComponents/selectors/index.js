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
import { Map } from 'immutable'
import { getFormValue } from 'selectors/base'
import {
  getContentLanguageCode,
  getContentLanguageId,
  getSelectedComparisionLanguageCode,
  getSelectedComparisionLanguageId
} from 'selectors/selections'
import { EntitySelectors } from 'selectors'
import { getPublishingStatusPublishedId } from 'selectors/common'

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
    return gdLanguages && gdLanguages.filter(l => l.get('statusId') === publishedId).map((v, k) => v.get('languageId')) || Map()
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
  (generalDescription, contentLanguageCode) =>
    generalDescription.get('name') && generalDescription.get('name').get(contentLanguageCode) || ''
)

export const getGeneralDescriptionCompareName = createSelector(
  [getGeneralDescription, getSelectedComparisionLanguageCode],
  (generalDescription, compareLanguageCode) =>
    generalDescription.get('name') && generalDescription.get('name').get(compareLanguageCode) || ''
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