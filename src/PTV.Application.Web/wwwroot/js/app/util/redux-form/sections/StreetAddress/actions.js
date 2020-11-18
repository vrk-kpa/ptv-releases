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
import { getContentLanguageCode } from 'selectors/selections'
import { change, formValueSelector } from 'redux-form/immutable'
import { fromJS, Map } from 'immutable'
import { apiCall3 } from 'actions'
import {
  getAddressStreetNumber,
  getAddressId,
  getAddressPostalCode
} from './selectors'
import { combinePathAndField } from 'util/redux-form/util'

import {
  loadAddresses
} from 'appComponents/ChannelAddressSearch/actions'
import shortId from 'shortid'

const getAddressSuccess = (formName, path, data, addressId) => ({ dispatch, getState }) => {
  const id = formValueSelector(formName)(getState(), combinePathAndField(path, 'id'))
  if (id === addressId) {
    dispatch(change(formName, combinePathAndField(path, 'coordinates'), fromJS(data.response.result.coordinates)))
  }
}

export const getCoordinates = props => ({ dispatch, getState }) => {
  const state = getState()
  const streetNumber = props.streetNumber || getAddressStreetNumber(state, props)
  const language = props.language || getContentLanguageCode(state, props) || 'fi'
  const streetName = getLocalizedName(props.streetName, language)
  const street = props.streetId
  let addressId = getAddressId(state, props)
  const postalCode = props.pCode || getAddressPostalCode(state, props)
  const municipalityCode = props.mCode

  if (!addressId) {
    addressId = shortId.generate()
    dispatch(change(props.formName, combinePathAndField(props.path, 'id'), addressId))
  }
  dispatch(apiCall3({
    keys: ['application', 'coordinates', addressId],
    payload: {
      endpoint: 'common/GetCoordinatesForAddress',
      data: { id: addressId, streetName, streetNumber, municipalityCode, language }
    },
    successNextAction: (data) => dispatch(getAddressSuccess(props.formName, props.path, data, addressId))
  }))
  if (!props.disableLoad) {
    dispatch(loadAddresses(state, dispatch, Map({ street, streetNumber, postalCode }), props.formName))
  }
}

/**
 * Attempts to select a street ID which corresponds to given street name and postal code ID
 * @param {?string} language - the code of the content language
 * @param {?Map} streets - immutable Map of stored street entities
 * @param {?Map} streetNumbers - immutable Map of stored street number entities
 * @param {?string} streetName - object containing street names in individual languages
 * @param {?string} streetId - street ID value in current form
 * @param {?string} postalCodeId - postal code ID value in current form
 * @param {?Map} translatedItems - immutable Map of stored translations
 */
export const getSuitableStreet = props => {
  const { language, streets, streetNumbers, streetName, streetId, postalCodeId, translatedItems } = props
  const localizedName = getLocalizedName(streetName, language)
  // This should work only when user selects a postal code and types a street name,
  // but does not select anything from the dropdown.
  if (!postalCodeId || !localizedName || streetId) {
    return null
  }

  const suitableStreets = []
  streets.forEach(street => {
    const id = street && street.get('id')
    const suitableStreetNumbers = streetNumbers && streetNumbers.filter(sn =>
      sn.get('postalCode') === postalCodeId && sn.get('streetId') === id)
    const translation = translatedItems && translatedItems.get(id)
    const polyfilledName = translation && polyfillStreetName(translation.get('texts'))
    const languageText = polyfilledName && polyfilledName.get(language) || ''

    if (suitableStreetNumbers && suitableStreetNumbers.size > 0 &&
      languageText.toLowerCase() === localizedName.toLowerCase()) {
      suitableStreets.push({
        id,
        streetName: polyfilledName
      })
    }
  })

  const suitableStreet = suitableStreets && suitableStreets[0]
  return suitableStreet
}

const polyfillStreetName = texts => {
  const fi = texts.get('fi')
  const sv = texts.get('sv')
  const en = texts.get('en')
  const def = texts.first()

  // For Finnish use Finnish or Åland Swedish
  // For Swedish use Swedish or default Finnish
  // For English use English or default Finnish or Åland Swedish
  return Map().set('fi', fi || sv || def).set('sv', sv || fi || def).set('en', en || fi || sv || def)
}

const getLocalizedName = (streetNames, languageCode) => {
  // Language fallback: try content language, then default Finnish, then default for Åland islands
  return streetNames && (streetNames.get(languageCode) || streetNames.get('fi') || streetNames.first())
}
