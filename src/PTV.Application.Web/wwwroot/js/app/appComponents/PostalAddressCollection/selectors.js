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
  getAddressType,
  getLocalizedAddressStreet,
  getAddressStreetNumber,
  getAddressPostalCode,
  getAddressPostalCodeTranslatedPostOffice,
  getLocalizedAddressPoBox,
  getLocalizedAddressForeignText,
  getAddressTranslatedCountry,
  getLocalizedAddressText
} from 'selectors/addresses'

const getStreetAddressTitle = createSelector(
  [
    getLocalizedAddressStreet,
    getAddressStreetNumber,
    getAddressPostalCode,
    getAddressPostalCodeTranslatedPostOffice
  ],
  (street, number, code, office) => {
    const streetAndNumber = [street, number].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const postalCode = [code, office].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const infoString = [streetAndNumber, postalCode].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)

const getPoBoxAddressTitle = createSelector(
  [
    getLocalizedAddressPoBox,
    getAddressPostalCode,
    getAddressPostalCodeTranslatedPostOffice
  ],
  (pobox, code, office) => {
    const postalCode = [code, office].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const infoString = [pobox, postalCode].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)

const getForeignAddressTitle = createSelector(
  [
    getLocalizedAddressForeignText,
    getAddressTranslatedCountry
  ],
  (text, country) => {
    const infoString = [text, country].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)

const getTextAddressTitle = createSelector(
  getLocalizedAddressText,
  text => {
    const infoString = [text].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)

export const getAddressTitle = createSelector(
  [
    getAddressType,
    getStreetAddressTitle,
    getPoBoxAddressTitle,
    getForeignAddressTitle,
    getTextAddressTitle
  ],
  (type, streetAddres, poBoxAddress, foreignAddress, textAddress) => {
    switch (type) {
      case 'Street':
        return streetAddres
      case 'PostOfficeBox':
        return poBoxAddress
      case 'Foreign':
        return foreignAddress
      case 'NoAddress':
        return textAddress
      default:
        return streetAddres
    }
  }
)
