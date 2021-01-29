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
import {
  getAddressType,
  getStreetName,
  getAddressStreetNumber,
  getAddressPostalCode,
  getAddressPostalCodeTranslatedPostOffice,
  getAddressTranslatedMunicipality,
  getAddressLatitude,
  getAddressLongitude,
  getLocalizedAddressForeignText,
  getAddressTranslatedCountry
} from 'selectors/addresses'

const getStreetAddressTitle = createSelector(
  [
    getStreetName,
    getAddressStreetNumber,
    getAddressPostalCode,
    getAddressPostalCodeTranslatedPostOffice,
    getAddressTranslatedMunicipality
  ],
  (streetName, number, code, office, municipality) => {
    const streetAndNumber = [streetName, number].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const postalCode = [code, office].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const infoString = [streetAndNumber, postalCode, municipality].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)

const getOtherAddressTitle = createSelector(
  [
    getAddressLatitude,
    getAddressLongitude,
    getAddressPostalCode,
    getAddressPostalCodeTranslatedPostOffice
  ],
  (latitude, longitude, code, office) => {
    const postalCode = [code, office].reduce(
      (acc, curr, index) => curr && (index === 0 ? acc + curr : acc + ' ' + curr) || acc
      , ''
    )
    const infoString = [latitude, longitude, postalCode].filter(x => x)
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

export const getAddressTitle = createSelector(
  [
    getAddressType,
    getStreetAddressTitle,
    getOtherAddressTitle,
    getForeignAddressTitle
  ],
  (type, streetAddress, otherAddress, foreignAddress) => {
    switch (type) {
      case 'Street':
        return streetAddress
      case 'Other':
        return otherAddress
      case 'Foreign':
        return foreignAddress
      default:
        return streetAddress
    }
  }
)
