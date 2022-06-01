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
import {
  getFormValueWithPath
} from 'selectors/base'
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { combinePathAndField } from 'util/redux-form/util'
import { getCoordinates } from 'util/redux-form/sections/Coordinates/selectors'
import { getContentLanguageCode } from 'selectors/selections'
import { coordinateTypesEnum } from 'enums'
import { List } from 'immutable'

export const getAddressPostalCode = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'postalCode')),
  (postalCode) => postalCode || null
)

export const getAddressPostalCodeEntity = createSelector(
  [getAddressPostalCode, EntitySelectors.postalCodes.getEntities],
  (postalCode, postalCodes) => postalCodes.get(postalCode) || Map()
)

export const getStreetId = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'street')),
  (street) => street || null
)

export const getStreetEntity = createSelector(
  [getStreetId, EntitySelectors.streets.getEntities],
  (streetId, streets) => streets.get(streetId) || Map()
)

export const getStreetName = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'streetName')),
  streetName => streetName || null
)

export const getStreetNameByLanguageCode = createSelector(
  [getStreetName, getContentLanguageCode],
  (names, language) => {
    return names && (names.get(language) || names.get('fi') || names.first())
  }
)

export const getMunicipality = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'municipality')),
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
  getFormValueWithPath((props) => combinePathAndField(props.path, 'streetNumber')),
  (streetNumber) => streetNumber || null
)

export const getAddressStreetNumberRange = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'streetNumberRange')),
  (streetNumber) => streetNumber || null
)

export const getAddressId = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'id')),
  (id) => id || null
)

export const getIsAddressInvalid = createSelector(
  getFormValueWithPath(props => combinePathAndField(props.path, 'invalidAddress')),
  invalid => invalid
)

const getAddressCoordinates = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'coordinates')),
  (coordinates) => coordinates || List()
)
const getAddressMainCoordinate = createSelector(
  getAddressCoordinates,
  coordinates => coordinates.find(coordinate => coordinate.get('isMain')) || Map()
)
export const getAddressMainCoordinateState = createSelector(
  getAddressMainCoordinate,
  coordinate => coordinate && coordinate.get('coordinateState') || ''
)

export const getCoordinatesNotFoundForAddress = createSelector(
  getStreetNameByLanguageCode,
  getAddressStreetNumber,
  getAddressPostalCode,
  getAddressMainCoordinateState,
  (streetName, streetNumber, postalCode, mainCoordinateState) => {
    if (streetName && streetNumber && postalCode &&
      mainCoordinateState && mainCoordinateState !== coordinateTypesEnum.OK) {
      return true
    }
    return false
  }
)

const getCoordinatesForAddressId = createSelector(
  getCoordinates,
  getAddressId,
  (coordinates, addressId) => {
    return  coordinates.get(addressId) || Map()
  }
)

export const getIsFetchingCoordinates = createSelector(
  getCoordinatesForAddressId,
  coordinatesForAddressId => coordinatesForAddressId.get('isFetching') || false
)