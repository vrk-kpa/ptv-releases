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
import {
  hasMaxBulletCountDraftJS,
  isRequired,
  isUrl,
  isEqual,
  isRequiredDraftJs,
  isNotEmpty,
  arrayValidator,
  isValidAddress
} from 'util/redux-form/validators'
import { DRAFTJS_MAX_BULLET_COUNT } from 'enums'
import { validationMessageTypes } from 'util/redux-form/validators/types'

export const commonChannelValidators = [
  {
    path: 'name',
    validate: isEqual('shortDescription')('name'),
    type: validationMessageTypes.asErrorVisible
  },
  {
    path: 'description',
    validate: isRequiredDraftJs('description')
  },
  {
    path: 'description',
    validate: hasMaxBulletCountDraftJS(DRAFTJS_MAX_BULLET_COUNT)('description'),
    type: validationMessageTypes.visible
  },
  { path: 'shortDescription', validate: isRequired('shortDescription') },
  {
    path: 'shortDescription',
    validate: isEqual('name')('shortDescription'),
    type: validationMessageTypes.asErrorVisible
  }
]

export const urlValidators = [
  { path: 'urlAddress', validate: isUrl('urlAddress') },
  { path: 'urlAddress', validate: isRequired('urlAddress') }
]

export const languageValidators = [
  { path: 'languages', validate: isNotEmpty('languages') }
]

export const visitingAddressValidators = [
  {
    path: 'visitingAddresses',
    validate: arrayValidator(isValidAddress('visitingAddresses'))
  },
  {
    path: 'visitingAddresses',
    validate: isNotEmpty('visitingAddresses')
  }
]
