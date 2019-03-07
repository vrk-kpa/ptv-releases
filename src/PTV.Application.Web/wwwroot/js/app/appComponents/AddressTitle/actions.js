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
import { change } from 'redux-form/immutable'
import { combinePathAndField } from 'util/redux-form/util'
import { addressTypesEnum } from 'enums'

const shouldStreetAddressBeValid = ({ mCode, pCode, streetId, streetNumberRange }) => {
  return !!mCode && !!pCode && !!streetId && !!streetNumberRange
}

const shouldOtherAddressBeValid = ({ coordinates, postalCodeId }) => {
  const firstCoordinate = coordinates && coordinates.length > 0 && coordinates[0]
  const firstCoordinateState = firstCoordinate &&
    firstCoordinate.coordinateState && firstCoordinate.coordinateState.toLowerCase()
  const isStateValid = (firstCoordinateState === 'ok' ||
    firstCoordinateState === 'enteredbyuser' ||
    firstCoordinateState === 'enteredbyar')
  return !!isStateValid && !!postalCodeId
}

export const checkAddressTitleValidity = props => {
  const isCurrentlyInvalid = props.invalidAddress
  const addressType = props.addressType
  let shouldBeInvalid = isCurrentlyInvalid

  switch (addressType) {
    case addressTypesEnum.STREET:
      shouldBeInvalid = !shouldStreetAddressBeValid(props)
      break
    case addressTypesEnum.OTHER:
      shouldBeInvalid = !shouldOtherAddressBeValid(props)
      break
    default:
      shouldBeInvalid = false
      break
  }

  if (shouldBeInvalid !== isCurrentlyInvalid) {
    props.dispatch(change(
      props.formName,
      combinePathAndField(props.path, 'invalidAddress'),
      shouldBeInvalid
    ))
  }
}
