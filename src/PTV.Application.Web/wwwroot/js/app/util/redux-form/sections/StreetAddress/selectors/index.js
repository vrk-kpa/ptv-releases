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
import {
  getFormValueWithPath
} from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import { EnumsSelectors, EntitySelectors } from 'selectors'

export const getAddressPostalCode = createSelector(
  getFormValueWithPath((props) => `${props.path}.postalCode`),
  (postalCode) => postalCode || null
)

export const getAddressPostalCodeEntity = createSelector(
  [getAddressPostalCode, EntitySelectors.postalCodes.getEntities],
  (postalCode, postalCodes) => postalCodes.get(postalCode) || Map()
)

export const getAddressStreet = createSelector(
  [
    getFormValueWithPath((props) => `${props.path}.street`),
    getContentLanguageCode
  ],
  (street, language) => (street || Map()).get(language) || null
)

export const getMunicipality = createSelector(
  getFormValueWithPath((props) => `${props.path}.municipality`),
  (municipality) => municipality || null
)

export const getMunicipalityEntity = createSelector(
  [getMunicipality, EnumsSelectors.municipalities.getEntitiesMap],
  (municipality, municipalities) => municipalities.get(municipality) || Map()
)

export const getMunicipalityCode = createSelector(
  getMunicipalityEntity,
  municipality => municipality.get('code') || null
)

export const getAddressStreetNumber = createSelector(
  getFormValueWithPath((props) => `${props.path}.streetNumber`),
  (streetNumber) => streetNumber || null
)

export const getAddressId = createSelector(
  getFormValueWithPath((props) => `${props.path}.id`),
  (id) => id || null
)
