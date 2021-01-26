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
import { getValuesByFormName, getParameterFromProps } from 'selectors/base'

const getAddress = createSelector(
  [
    getValuesByFormName,
    getParameterFromProps('collectionName'),
    getParameterFromProps('index')
  ],
  (formValues, path, index) => formValues && formValues.getIn([path, index]) || Map()
)

export const getIsAccessibilityRegisterAddresss = createSelector(
  getAddress,
  address => address.hasIn(['accessibilityRegister', 'id'])
)

export const getInvalidAddress = createSelector(
  getAddress,
  address => address.get('invalidAddress')
)

export const getCoordinates = createSelector(
  getAddress,
  address => address.get('coordinates')
)

export const getPostalCode = createSelector(
  getAddress,
  address => address.get('postalCode')
)

export const getMunicipalityId = createSelector(
  getAddress,
  address => address.get('municipality')
)

export const getStreetId = createSelector(
  getAddress,
  address => address.get('street')
)

export const getStreetNumberRangeId = createSelector(
  getAddress,
  address => address.get('streetNumberRange')
)

export const getAddressType = createSelector(
  getAddress,
  address => address.get('streetType')
)
