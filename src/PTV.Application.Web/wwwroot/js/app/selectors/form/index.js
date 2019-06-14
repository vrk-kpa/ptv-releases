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

import { getFormValue } from 'selectors/base'
import { List } from 'immutable'
import { createSelector } from 'reselect'
import { getPublishingStatusPublishedId } from 'selectors/common'
import { getTranslationLanguageCodes } from 'selectors/entities/entities'

export const getFormOrganization = createSelector(
  getFormValue('organization'),
  organization => organization || null

)

export const getVisitingAddresses = createSelector(
  getFormValue('visitingAddresses'),
  visitingAddresses => visitingAddresses || List()
)

export const getAreAllEnabled = createSelector(
  getVisitingAddresses,
  visitingAddresses => visitingAddresses.size <= 1
)

export const getAreStreetOtherEnabled = createSelector(
  getVisitingAddresses,
  visitingAddresses => !visitingAddresses.some(address => {
    const streetType = (address && address.get('streetType') || '').toLowerCase()
    return streetType === 'foreign'
  })
)

export const getFormLanguagesAvailabilities = createSelector(
  getFormValue('languagesAvailabilities'),
  languagesAvailabilities => languagesAvailabilities || List()
)

export const getFormLanguageToPublish = createSelector(
  [getFormLanguagesAvailabilities, getPublishingStatusPublishedId],
  (languagesAvailabilities, publishingStatusPublishedId) =>
    languagesAvailabilities.filter(x => x.get('newStatusId') === publishingStatusPublishedId && x.get('statusId') !== publishingStatusPublishedId && !x.get('validFrom'))
)

export const getFormLanguageToSchedule = createSelector(
  [getFormLanguagesAvailabilities, getPublishingStatusPublishedId],
  (languagesAvailabilities, publishingStatusPublishedId) =>
    languagesAvailabilities.filter(x => x.get('newStatusId') === publishingStatusPublishedId && (x.get('validFrom') || x.get('validTo')))
)

export const getFormIsAnyLanguageToPublish = createSelector(
  getFormLanguageToPublish,
  languagesAvailabilities => !!languagesAvailabilities.size
)

export const getFormIsAnyLanguageToSchedule = createSelector(
  getFormLanguageToSchedule,
  languagesAvailabilities => !!languagesAvailabilities.size
)

export const getFormLanguagesAvailabilitiesTransformed = createSelector(
  [getFormLanguagesAvailabilities, getPublishingStatusPublishedId],
  (languagesAvailabilities, publishedId) => languagesAvailabilities.map(languageAvailability => {
    return languageAvailability.update(language => {
      return language.has('newStatusId')
        ? language
          .set('validFrom', language.get('newStatusId') !== publishedId ? null : language.get('validFrom'))
          .set('validTo', language.get('newStatusId') !== publishedId ? null : language.get('validTo'))
          .set('statusId', language.get('newStatusId'))
          .delete('newStatusId')
        : language
    })
  })
)
