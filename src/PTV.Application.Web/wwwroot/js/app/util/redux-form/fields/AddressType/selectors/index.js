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
import { formValueSelector } from 'redux-form/immutable'
import { Map } from 'immutable'

const getAddress = (formName, path) => createSelector(
  state => formValueSelector(formName)(state, path),
  formValues => formValues || Map()
)

export const getIsAccessibilityRegisterAddresss = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.hasIn(['accessibilityRegister', 'id'])
)

export const getInvalidAddress = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.get('invalidAddress')
)

export const getCoordinates = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.get('coordinates')
)

export const getPostalCode = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.get('postalCode')
)

export const getMunicipalityId = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.get('municipality')
)

export const getStreetId = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.get('street')
)

export const getStreetNumberRangeId = (formName, path) => createSelector(
  getAddress(formName, path),
  address => address.get('streetNumberRange')
)
