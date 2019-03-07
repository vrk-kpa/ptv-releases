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
import { EntitySelectors } from 'selectors'
import { getParameterFromProps } from 'selectors/base'

export const getPostalCodeEntity = createSelector(
  [EntitySelectors.postalCodes.getEntities, getParameterFromProps('address')],
  (postalCodes, address) => address && postalCodes.get(address.get('postalCode')) || Map()
)

export const getPostalCode = createSelector(
  getPostalCodeEntity,
  postalCode => postalCode.get('code') || ''
)

export const getPostOffice = createSelector(
  getPostalCodeEntity,
  postalCode => postalCode.get('postOffice') || ''
)

export const getCountryEntity = createSelector(
  [EntitySelectors.countries.getEntities, getParameterFromProps('address')],
  (countries, address) => address && countries.get(address.get('country')) || Map()
)

export const getCountry = createSelector(
  getCountryEntity,
  country => country.get('countryName') || ''
)

export const getDialCodeEntity = createSelector(
  [EntitySelectors.dialCodes.getEntities, getParameterFromProps('number')],
  (dialCodes, number) => number && dialCodes.get(number.get('dialCode')) || Map()
)

export const getDialCode = createSelector(
  getDialCodeEntity,
  dialCode => dialCode.get('code') || ''
)
