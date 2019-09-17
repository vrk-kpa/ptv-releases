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
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { List, Map } from 'immutable'
import { getEntitiesForIds } from 'selectors/base'
import { getEntityAvailableLanguages, getTranslationAvailability } from 'selectors/entities/entities'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

export const getIsOrganizationLanguageWarningVisible = createSelector(
  EntitySelectors.getEntity,
  entity =>
    (entity.get('missingLanguages') && entity.get('missingLanguages').size > 0) || false
)

const getMissingLanguagesIds = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('missingLanguages') || List()
)

export const getMissingLanguages = createSelector(
  getMissingLanguagesIds, EntitySelectors.languages.getEntities,
  (missingLanguagesIds, languagesEntities) => getEntitiesForIds(languagesEntities, missingLanguagesIds)
)

export const getIsRemoveConnectionWarningVisible = createSelector(
  EntitySelectors.getEntity,
  entity =>
    (entity.get('disconnectedConnections') && entity.get('disconnectedConnections').size > 0) || false
)

export const getWarningChannels = createSelector(
  EntitySelectors.getEntity,
  entity =>
    entity.get('disconnectedConnections') || List()
)

export const getNotificationIds = createSelector(
  getWarningChannels,
  channels =>
    channels.map((channel) => channel.get('notificationId')).toJS()
)

export const getIsExpireWarningVisible = createSelector(
  EntitySelectors.getEntity,
  entity =>
    entity.get('isWarningVisible') || false
)

export const getIsTimedPublishInfoVisible = createSelector(
  getEntityAvailableLanguages,
  languagesAvailabilities => languagesAvailabilities.some(la => la.get('validFrom'))
)

export const getPublishOn = createSelector(
  getEntityAvailableLanguages,
  languagesAvailabilities => {
    const sorted = languagesAvailabilities
      .map(la => la.get('validFrom'))
      .filter(x => x)
      .sort() || List()
    return sorted.get(0)
  }
)

export const getIsTranslationDeliveredInfoVisible = createSelector(
  getTranslationAvailability,
  translationAvailabilities => translationAvailabilities.some(tA => tA.get('isTranslationDelivered'))
)

const getTranslationArrivedLanguageCodes = createSelector(
  getTranslationAvailability,
  translationAvailabilities => translationAvailabilities
    .filter(tA => tA.get('isTranslationDelivered'))
    .keySeq()
)

const getTranslationArrivedLanguagesIds = createSelector(
  getTranslationArrivedLanguageCodes,
  EnumsSelectors.translationLanguages.getEntities,
  (languageCodes, translationLanguages) => translationLanguages
    .reduce((acc, curr) => (
      languageCodes.includes(curr.get('code')) ? acc.push(curr.get('id')) : acc
    ), List())
    .map(id => Map({ id }))
)

const getTranslatedTranslationArrivedLanguages = createTranslatedListSelector(
  getTranslationArrivedLanguagesIds, {
    languageTranslationType: languageTranslationTypes.locale
  }
)

export const getTranslatedTranslationArrivedLanguagesTexts = createSelector(
  getTranslatedTranslationArrivedLanguages,
  languages => languages.map(lang => lang.get('name')).join(', ')
)

export const getHasQualityError = createSelector(
  EntitySelectors.qualityErrors.getEntity,
  entity =>
    entity.some(field => field.get('rules').some(rule => !rule.get('passed')))
)

export const getIsTranslationOrderedInfoVisible = createSelector(
  getTranslationAvailability,
  translationAvailabilities => translationAvailabilities.some(tA => tA.get('isInTranslation'))
)

const getTranslationOrderedLanguageCodes = createSelector(
  getTranslationAvailability,
  translationAvailabilities => translationAvailabilities
    .filter(tA => tA.get('isInTranslation'))
    .keySeq()
)

const getTranslationOrderedLanguagesIds = createSelector(
  getTranslationOrderedLanguageCodes,
  EnumsSelectors.translationLanguages.getEntities,
  (languageCodes, translationLanguages) => translationLanguages
    .reduce((acc, curr) => (
      languageCodes.includes(curr.get('code')) ? acc.push(curr.get('id')) : acc
    ), List())
    .map(id => Map({ id }))
)

const getTranslatedTranslationOrderedLanguages = createTranslatedListSelector(
  getTranslationOrderedLanguagesIds, {
    languageTranslationType: languageTranslationTypes.locale
  }
)

export const getTranslatedTranslationOrderedLanguagesTexts = createSelector(
  getTranslatedTranslationOrderedLanguages,
  languages => languages.map(lang => lang.get('name')).join(', ')
)
