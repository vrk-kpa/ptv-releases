/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { EntitySelectors } from 'selectors'
import {
  getParameterFromProps,
  getFormValueWithPath
} from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import { getSelectedLanguage } from 'Intl/Selectors'
import {
  getFilteredList,
  languageMap
} from 'util/redux-form/submitFilters/Mappers'
import { EditorState, convertFromRaw } from 'draft-js'

const getPostalCodeEntity = createSelector(
  EntitySelectors.postalCodes.getEntity,
  postalCode => postalCode || Map()
)

export const getPostalCode = createSelector(
  getPostalCodeEntity,
  postalCode => postalCode.get('code') || ''
)

export const getCountryEntity = createSelector(
  [EntitySelectors.countries.getEntities, getParameterFromProps('address')],
  (countries, address) => address && countries.get(address.get('country')) || Map()
)

export const getCountry = createSelector(
  getCountryEntity,
  country => country.get('countryName') || ''
)

export const getDialCodeEntity = createSelector(
  [EntitySelectors.dialCodes.getEntities, getParameterFromProps('number')],
  (dialCodes, number) => number && dialCodes.get(number.get('dialCode')) || Map()
)

export const getDialCode = createSelector(
  getDialCodeEntity,
  dialCode => dialCode.get('code') || ''
)

const getFieldPath = ({ field }) => field

const getLocalizedValue = (value, code) => {
  if (!value) return ''
  if (value.toJS) {
    value = value.toJS()
  }

  return value[code] || value[Object.keys(value)[0]]
}

export const getFieldValue = createSelector(
  getFormValueWithPath(getFieldPath),
  value => value || Map()
)

const getLanguageCode = createSelector(
  getContentLanguageCode,
  getSelectedLanguage,
  (contentLanguage, uiLanguage) => contentLanguage || uiLanguage
)

export const getDescription = createSelector(
  getFieldValue,
  description => description.get('description') || Map()
)

export const getBasicInformationDescription = createSelector(
  getDescription,
  getLanguageCode,
  (description, languageCode) => {
    const editorState = description && description.size > 0 &&
      description || Map()
    return editorState.get(languageCode) || null
  }
)

export const getBasicInformationChargeType = createSelector(
  getFieldValue,
  value => value.get('chargeType')
)

export const getAdditionalInformation = createSelector(
  getFieldValue,
  addInfo => addInfo.get('additionalInformation') || Map()
)

export const getBasicInformationAdditionalInformation = createSelector(
  getAdditionalInformation,
  getLanguageCode,
  (addInfo, languageCode) => {
    const editorState = addInfo && addInfo.size > 0 &&
      addInfo || Map()
    return editorState.get(languageCode) || null
  }
)

export const getHasBasicInformation = createSelector(
  getBasicInformationDescription,
  getBasicInformationChargeType,
  getBasicInformationAdditionalInformation,
  (description, chargeType, additionalInformation) => {
    const hasBasicInfo = description || chargeType || additionalInformation
    return !!hasBasicInfo
  }
)

const getAstiTypes = createSelector(
  getFieldValue,
  value => value.get('astiTypeInfos') || List()
)
export const getHasAstiInformation = createSelector(
  getAstiTypes,
  astiTypes => astiTypes.some(type => type.get('astiDescription').size > 0)
)

const getNormalOpeningHours = createSelector(
  getFieldValue,
  value => value.get('normalOpeningHours') || List()
)
const getSpecialOpeningHours = createSelector(
  getFieldValue,
  value => value.get('specialOpeningHours') || List()
)
const getExceptionalOpeningHours = createSelector(
  getFieldValue,
  value => value.get('exceptionalOpeningHours') || List()
)
const getHolidayHours = createSelector(
  getFieldValue,
  value => value.get('holidayHours') || List()
)
export const getHasOpeningHours = createSelector(
  getNormalOpeningHours,
  getSpecialOpeningHours,
  getExceptionalOpeningHours,
  getHolidayHours,
  (normal, special, exceptional, holidays) => {
    const hasOpeningHours = normal.size > 0 || special.size > 0 || exceptional.size > 0 || holidays.size > 0
    return !!hasOpeningHours
  }
)

const getEmails = createSelector(
  getFieldValue,
  value => languageMap(getFilteredList)(value.get('emails')) || Map()
)
export const getEmailsCollection = createSelector(
  getEmails,
  getLanguageCode,
  (value, languageCode) => value.size && (value.get(languageCode) || value.first()) || List()
)
const getFaxNumbers = createSelector(
  getFieldValue,
  value => languageMap(getFilteredList)(value.get('faxNumbers')) || Map()
)
export const getFaxNumbersCollection = createSelector(
  getFaxNumbers,
  getLanguageCode,
  (value, languageCode) => value.size && (value.get(languageCode) || value.first()) || List()
)
const getPhoneNumbers = createSelector(
  getFieldValue,
  value => languageMap(getFilteredList)(value.get('phoneNumbers')) || Map()
)
export const getPhoneNumbersCollection = createSelector(
  getPhoneNumbers,
  getLanguageCode,
  (value, languageCode) => value.size && (value.get(languageCode) || value.first()) || List()
)
export const getPostalAddressesCollection = createSelector(
  getFieldValue,
  value => value.size && getFilteredList(value.get('postalAddresses')) || List()
)
const getWebPages = createSelector(
  getFieldValue,
  value => languageMap(getFilteredList)(value.get('webPages')) || Map()
)
export const getWebPagesCollection = createSelector(
  getWebPages,
  getLanguageCode,
  (value, languageCode) => value.size && (value.get(languageCode) || value.first()) || List()
)
export const getHasContactDetails = createSelector(
  getEmailsCollection,
  getFaxNumbersCollection,
  getPhoneNumbersCollection,
  getPostalAddressesCollection,
  getWebPagesCollection,
  (emails, faxNumbers, phoneNumbers, postalAddresses, webPages) => {
    const hasContactDetails = emails.size > 0 || faxNumbers.size > 0 || phoneNumbers.size > 0 ||
      postalAddresses.size > 0 || webPages.size > 0
    return !!hasContactDetails
  }
)

const getDigitalAuthorization = createSelector(
  getFieldValue,
  value => value.get('digitalAuthorizations')
)
export const getHasDigitalAuthorization = createSelector(
  getDigitalAuthorization,
  digitalAuthorizations => digitalAuthorizations && digitalAuthorizations.size > 0
)
