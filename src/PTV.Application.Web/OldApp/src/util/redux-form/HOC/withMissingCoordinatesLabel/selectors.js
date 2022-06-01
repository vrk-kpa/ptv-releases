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
  getAddressId,
  getStreetName,
  getAddressStreetNumber,
  getAddressPostalCode,
  getAddressMainCoordinateState
} from 'selectors/addresses'
import { getCoordinates } from 'util/redux-form/sections/Coordinates/selectors'
import { coordinateTypesEnum } from 'enums'

export const getCoordinatesNotFoundForAddress = createSelector(
  getStreetName,
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
  (coordinates, addressId) => coordinates.get(addressId) || Map()
)

export const getIsFetchingCoordinates = createSelector(
  getCoordinatesForAddressId,
  coordinatesForAddressId => coordinatesForAddressId.get('isFetching') || false
)
