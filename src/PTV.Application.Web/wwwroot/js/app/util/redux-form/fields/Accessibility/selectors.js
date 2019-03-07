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
import { List, Map } from 'immutable'
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getApiCalls } from 'selectors/base'
import moment from 'moment'
import { formValueSelector } from 'redux-form/immutable'
import { DEFAULT_GUID_EMPTY_VALUE } from 'enums'

const getAddressesIds = createSelector(
  state => formValueSelector('serviceLocationForm')(state, 'visitingAddresses') || List(),
  addresses => {
    const addressesIds = addresses
      .map(address => address.get('id'))
      .filter(x => !!x)
    return addressesIds || List()
  }
)
const getAddresses = createSelector(
  [getAddressesIds, EntitySelectors.addresses.getEntities],
  (addressesIds, addressesEntities) => {
    const addresses = addressesIds.map(addressId => addressesEntities.get(addressId)).filter(x => !!x)
    return addresses || List()
  }
)
const getAccessibilityRegisterIds = createSelector(
  getAddresses,
  addresses => {
    const accessibilityRegisterIds = addresses
      .filter(address => address.get('accessibilityRegister'))
      .map(address => address.get('accessibilityRegister'))
    return accessibilityRegisterIds || List()
  }
)

export const getAccessibilityRegisters = createSelector(
  [
    getAccessibilityRegisterIds,
    EntitySelectors.accessibilityRegisters.getEntities
  ],
  (
    accessibilityRegisterIds,
    accessibilityRegisterEntities
  ) => {
    const accessibilityRegisters = accessibilityRegisterIds
      .map(accessibilityRegisterId => accessibilityRegisterEntities.get(accessibilityRegisterId))
    return accessibilityRegisters || List()
  }
)
export const getMainAccessibilityRegister = createSelector(
  getAccessibilityRegisters,
  accessibilityRegisters => {
    return accessibilityRegisters
      // .filter(accessibilityRegister => accessibilityRegister.get('isMainEntrance'))
      .first() || Map()
  }
)

export const getIsAccessibilityRegisterValid = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => {
    // Intentional true comparision //
    const defaultGuidEmptyValue = DEFAULT_GUID_EMPTY_VALUE
    return (
      accessibilityRegister.get('isValid') === true &&
      accessibilityRegister.get('entranceId') !== defaultGuidEmptyValue
    )
  }
)

export const getIsAccessibilityRegisterSet = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => {
    return !!accessibilityRegister.get('url')
  }
)
export const getAccessibilityRegisterSetAt = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => {
    const setAt = accessibilityRegister.get('setAt')
    return setAt && moment(setAt).format('HH:mm:ss')
  }
)
export const getAccessibilityRegisterUrl = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => {
    return accessibilityRegister.get('url')
  }
)
export const getAccessibilityRegisterAddressLanguage = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => accessibilityRegister.get('addressLanguage')
)
export const getAccessibilityRegisterSentenceGroups = createSelector(
  [getAccessibilityRegisters, (_, { index }) => index],
  (accessibilityRegisters, index) => {
    const accessibilityRegister = accessibilityRegisters.get(index) || Map()
    return accessibilityRegister.get('groups') || List()
  }
)
export const getAccessibilityRegisterId = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => accessibilityRegister.get('id')
)
export const getCanSetAccessibility = createSelector(
  getAddresses,
  addresses => {
    const address = addresses.first()
    if (
      address &&
      !address.get('invalidAddress') &&
      address.get('postalCode') &&
      address.get('streetNumber') &&
      address.get('street') &&
      address.has('coordinates') &&
      address.get('coordinates').size !== 0 &&
      address.get('coordinates').every(coordinate => {
        const coordinateState = coordinate.get('coordinateState').toLowerCase()
        return coordinateState === 'ok' || coordinateState === 'enteredbyuser' || coordinateState === 'enteredbyar'
      })
    ) {
      return true
    }
    return false
  }
)
export const getIsLoadingSentences = createSelector(
  getApiCalls,
  apiCalls => !!apiCalls.getIn(['accessibility', 'load', 'isFetching'])
)

export const getContactEmail = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => accessibilityRegister.get('contactEmail') || ''
)

export const getContactPhone = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => accessibilityRegister.get('contactPhone') || ''
)

export const getContactUrl = createSelector(
  getMainAccessibilityRegister,
  accessibilityRegister => accessibilityRegister.get('contactUrl') || ''
)
