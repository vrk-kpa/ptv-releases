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
import { createError } from './isValid'
import { getContentLanguageCode } from 'selectors/selections'
import { List, Map } from 'immutable'
import messages from './messages'

const isValidAddress = label => (value, formValues, props) => {
  const { dispatch } = props
  if (!dispatch) {
    throw new Error(
      `dispatch not provided for isValidServiceProducer(${label}) validator`
    )
  }
  let languageCode
  dispatch(({ getState }) => {
    languageCode = getContentLanguageCode(getState())
  })
  const street = value.get('street')
  const streetNumber = value.get('streetNumber')
  const postalCode = value.get('postalCode')
  const coordinatesState = ((
    value.get('coordinates') || List()
  ).first() || Map()).get('coordinateState')
  const additionalInformation = value.getIn([
    'additionalInformation',
    languageCode
  ])
  const foreignAddress = value.getIn(['foreignAddressText', languageCode])
  const streetType = value.get('streetType')
  let valid = false
  switch (streetType) {
    case 'Street':
      valid = !!(street && streetNumber && postalCode)
      break
    case 'Other':
      valid = !!(coordinatesState !== 'NotOk' && additionalInformation)
      break
    case 'Foreign':
      valid = !!foreignAddress
      break
  }
  if (!valid) {
    return createError(label, messages.isRequired)
  }
}

export default isValidAddress
