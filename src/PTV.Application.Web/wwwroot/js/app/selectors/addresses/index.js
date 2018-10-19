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
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode,
  getCodesOfTranslationLanguages
} from 'selectors/selections'
import { EntitySelectors } from 'selectors'
import { Map } from 'immutable'
import { createTranslatedIdSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { getParameterFromProps } from 'selectors/base'

const getItems = (_, { items }) => items
const getIndex = (_, { index }) => index
const getIsInCompareMode = (_, { compare }) => !!compare

const getLanguageCode = createSelector(
  [
    getContentLanguageCode,
    getSelectedComparisionLanguageCode,
    getIsInCompareMode
  ],
  (language, comparisionLanguageCode, compare) => {
    return compare && comparisionLanguageCode || language || 'fi'
  }
)

// ADDRESS ENTiTY
const getAddress = createSelector(
  [
    getIndex,
    getItems
  ],
  (index, items) => items && items.get(index) || Map()
)
// ADDRESS ID
export const getAddressId = createSelector(
  getAddress,
  address => address.get('id') || null
)
// ACCESSIBILITY REGISTER
export const getHasAccessibilityRegisterId = createSelector(
  getAddress,
  address => !!address.get('accessibilityRegister')
)
export const getIsMainEntrance = createSelector(
  getAddress,
  address => {
    return !!address.getIn(['accessibilityRegister', 'isMainEntrance'])
  }
)
export const getIsAdditionalEntrance = createSelector(
  getAddress,
  address => {
    return address.getIn(['accessibilityRegister', 'isMainEntrance']) === false
  }
)
// ADDRESS TYPE (street, pobox, foreign, other, noaddress)
export const getAddressType = createSelector(
  getAddress,
  address => address.get('streetType')
)
// ADDRESS POSTAL CODE
const getAddressPostalCodeId = createSelector(
  getAddress,
  address => address.get('postalCode')
)
const getAddressPostalCodeEntity = createSelector(
  [
    getAddressPostalCodeId,
    EntitySelectors.postalCodes.getEntities
  ],
  (id, entities) => entities.get(id) || Map()
)
export const getAddressPostalCode = createSelector(
  getAddressPostalCodeEntity,
  postalCode => postalCode.get('code')
)
export const getAddressPostalCodeTranslatedPostOffice = createTranslatedIdSelector(
  getAddressPostalCodeId, {
    languageTranslationType: languageTranslationTypes.both
  }
)

const findDefaultValue = (allowedCodes, street) => {
  let defaultValue
  allowedCodes.forEach(lC => {
    defaultValue = street.get(lC)
    return !defaultValue
  }
  )
  return defaultValue || ''
}

// ADDRESS STREET
const getAddressStreet = createSelector(
  getAddress,
  address => address.get('street') || Map()
)
export const getLocalizedAddressStreet = createSelector(
  [
    getAddressStreet,
    getLanguageCode,
    getParameterFromProps('defaultStreetTitle'),
    getCodesOfTranslationLanguages
  ],
  (street, language, defaultTitle, allowedCodes) =>
    street.get(language) ||
    (defaultTitle && findDefaultValue(allowedCodes.filter(l => l !== language), street)) ||
    ''
)
// ADDRESS STREET NUMBER
export const getAddressStreetNumber = createSelector(
  getAddress,
  address => address.get('streetNumber') || ''
)
// ADDRESS ADDiTiONAL iNFORMATiON
const getAddressAdditionalInformation = createSelector(
  getAddress,
  address => address.get('additionalInformation') || Map()
)
export const getLocalizedAddressAdditionalInformation = createSelector(
  [
    getAddressAdditionalInformation,
    getLanguageCode
  ],
  (additionalInformation, language) => additionalInformation.get(language) || ''
)
// ADDRESS POBOX
const getAddressPoBox = createSelector(
  getAddress,
  address => address.get('poBox') || Map()
)
export const getLocalizedAddressPoBox = createSelector(
  [
    getAddressPoBox,
    getLanguageCode
  ],
  (pobox, language) => pobox.get(language) || ''
)
// ADDRESS DELiVERY iNSTRUCTiONS
const getAddressText = createSelector(
  getAddress,
  address => address.get('noAddressAdditionalInformation') || Map()
)
export const getLocalizedAddressText = createSelector(
  [
    getAddressText,
    getLanguageCode
  ],
  (text, language) => text.get(language) || ''
)
// ADDRESS RECiPiENT
const getAddressRecipient = createSelector(
  getAddress,
  address => address.get('receivers') || Map()
)
export const getLocalizedAddressRecipient = createSelector(
  [
    getAddressRecipient,
    getLanguageCode
  ],
  (recipient, language) => recipient.get(language) || ''
)
// ADDRESS COORDiNATES
const getAddressCoordinates = createSelector(
  getAddress,
  address => address.get('coordinates') || Map()
)
const getAddressFirstCoordinate = createSelector(
  getAddressCoordinates,
  coordinates => coordinates.first() || Map()
)
export const getAddressLatitude = createSelector(
  getAddressFirstCoordinate,
  coordinate => coordinate.get('latitude') || ''
)
export const getAddressLongitude = createSelector(
  getAddressFirstCoordinate,
  coordinate => coordinate.get('longitude') || ''
)
// ADDRESS FOREIGN TEXT
const getAddressForeignText = createSelector(
  getAddress,
  address => address.get('foreignAddressText') || Map()
)
export const getLocalizedAddressForeignText = createSelector(
  [
    getAddressForeignText,
    getLanguageCode
  ],
  (text, language) => text.get(language) || ''
)
// ADDRESS COUNTRY
const getAddressCountryId = createSelector(
  getAddress,
  address => address.get('country')
)
export const getAddressTranslatedCountry = createTranslatedIdSelector(
  getAddressCountryId, {
    languageTranslationType: languageTranslationTypes.both
  }
)
// ADDRESS MUNiCiPALiTY
const getAddressMunicipalityId = createSelector(
  getAddress,
  address => address.get('municipality')
)
export const getAddressTranslatedMunicipality = createTranslatedIdSelector(
  getAddressMunicipalityId, {
    languageTranslationType: languageTranslationTypes.both
  }
)
