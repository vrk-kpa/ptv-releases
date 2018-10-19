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
import { EntitySelectors } from 'selectors'
import { createTranslatedIdSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

const getItems = (_, { items }) => items
const getIndex = (_, { index }) => index

// PHONE
const getPhone = createSelector(
  [
    getIndex,
    getItems
  ],
  (index, items) => items && items.get(index) || Map()
)
// DIAL CODE
const getDialCodeId = createSelector(
  getPhone,
  phone => phone.get('dialCode')
)
const getDialCodeEntity = createSelector(
  [
    getDialCodeId,
    EntitySelectors.dialCodes.getEntities
  ],
  (id, items) => items.get(id) || Map()
)
const getDialCode = createSelector(
  getDialCodeEntity,
  dialCode => dialCode.get('code')
)
// PHONE TYPE
const getPhoneTypeId = createSelector(
  getPhone,
  phone => phone.get('type')
)
export const getPhoneType = createTranslatedIdSelector(
  getPhoneTypeId, {
    languageTranslationType: languageTranslationTypes.both
  }
)
// PHONE NUMBER
export const getPhoneNumber = createSelector(
  getPhone,
  phone => phone.get('phoneNumber') || ''
)
// IS LOCAL NUMBER
export const getIsLocalNumber = createSelector(
  getPhone,
  phone => phone.get('isLocalNumber') || false
)
// ADDITIONAL INFORMATION
const getAdditionalInformation = createSelector(
  getPhone,
  phone => phone.get('additionalInformation') || ''
)
// CHARGE TYPE
const getChargeTypeId = createSelector(
  getPhone,
  phone => phone.get('chargeType')
)
const getChargeType = createTranslatedIdSelector(
  getChargeTypeId, {
    languageTranslationType: languageTranslationTypes.both
  }
)
// CHARGE DESCRIPTION
const getChargeDescription = createSelector(
  getPhone,
  phone => phone.get('chargeDescription') || ''
)

export const getPhoneNumberTitle = createSelector(
  [
    getDialCode,
    getPhoneNumber,
    getIsLocalNumber,
    getAdditionalInformation,
    getChargeType,
    getChargeDescription
  ],
  (dialCode, phoneNumber, isLocalNumber, additionalInformation, chargeType, chargeDescription) => {
    const phone = !isLocalNumber && [dialCode, phoneNumber].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const infoString = [phone, additionalInformation, chargeType, chargeDescription].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)
