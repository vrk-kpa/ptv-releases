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
import { Map, List } from 'immutable'
import {
  getFormValueWithPath,
  getParameterFromProps
} from 'selectors/base'
import { EnumsSelectors } from 'selectors'
import { getApplication } from 'selectors/base'

const getAddressPath = props => `${props.addressUseCase}Addresses`
// form selectors
const getFormAddressesByType = createSelector(
  getFormValueWithPath(getAddressPath),
  values => values || Map()
)

export const getAddressCoordinates = createSelector(
  [
    getFormAddressesByType,
    getParameterFromProps('index', 0)
  ],
  (addresses, index) => Number.isNaN(index)
      ? (addresses.get('coordinates') || List())
      : (addresses.getIn([index, 'coordinates']) || List())
)

export const getAddressMainCoordinate = createSelector(
  getAddressCoordinates,
  coordinates => coordinates.filter(coordinate =>
    coordinate &&
    coordinate.get('isMain') &&
    (coordinate.get('coordinateState').toLowerCase() === 'ok' ||
      coordinate.get('coordinateState').toLowerCase() === 'notreceived')
  ).first() || null
)

export const getAddressId = createSelector(
  [
    getFormAddressesByType,
    getParameterFromProps('index', 0)
  ],
  (addresses, index) => Number.isNaN(index)
      ? addresses.get('id')
      : addresses.getIn([index, 'id']) || null
)

export const getCoordinates  = createSelector(
  getApplication,
  application => application.get('coordinates') || Map()
)

export const getCoordinatesForId  = createSelector(
  [getAddressId, getCoordinates],
  (addressId, coordinates) => coordinates.get(addressId) || Map()
)

export const getIsCoordinatesFetching  = createSelector(
  getCoordinatesForId,
  coordinatesForId => coordinatesForId.get('isFetching') || false
)
