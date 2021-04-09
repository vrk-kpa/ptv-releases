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
import { Map } from 'immutable'
import { EntitySelectors } from 'selectors'
import { formTypesEnum } from 'enums'
import { getContentLanguageCode } from 'selectors/selections'
import { getAddressSearch } from 'appComponents/ChannelAddressSearch/selectors'
import { getValuesByFormName } from 'selectors/base'

const getStreet = createSelector(
  state => getAddressSearch(state, { formName:formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM }),
  address => address.get('street')
)

const getStreetName = createSelector(
  state => getAddressSearch(state, { formName:formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM }),
  address => address.get('streetName')
)

const getStreetNumber = createSelector(
  state => getAddressSearch(state, { formName:formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM }),
  address => address.get('streetNumber')
)

const getPostalCode = createSelector(
  state => getAddressSearch(state, { formName:formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM }),
  address => address.get('postalCode')
)

const getPostalCodeEntity = createSelector(
  [getPostalCode, EntitySelectors.postalCodes.getEntities],
  (postalCode, postalCodes) => postalCodes.get(postalCode) || Map()
)

const getMunicipalityId = createSelector(
  getPostalCodeEntity,
  postalCode => postalCode.get('municipalityId')
)

const getSearchStreetNumberRange = createSelector(
  getValuesByFormName,
  values => values && values.get('streetNumberRange')
)

export const getGetDefaultSearchedAddress = createSelector(
  [
    getStreet,
    getStreetName,
    getStreetNumber,
    getPostalCode,
    getMunicipalityId,
    getContentLanguageCode,
    getSearchStreetNumberRange
  ],
  (
    street,
    streetName,
    streetNumber,
    postalCode,
    municipality,
    language,
    streetNumberRange
  ) => Map({
    streetType: 'Street',
    streetName,
    street,
    streetNumber,
    postalCode,
    municipality,
    language,
    streetNumberRange
  })
)

export const isDefaultAddressExists = createSelector(
  [getStreetName,
    getStreetNumber
  ],
  (streetName,
    streetNumber
  ) => streetNumber && streetName
)
