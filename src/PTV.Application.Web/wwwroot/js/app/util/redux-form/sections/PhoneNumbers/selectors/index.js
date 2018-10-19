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
import { List, Map } from 'immutable'
import {
  getFormValue,
  getFormValueWithPath
} from 'selectors/base'
import { EntitySelectors } from 'selectors'

export const getFormPhoneNumbers = createSelector(
    getFormValue('phoneNumbers'),
    values => values || List()
)
const getFiledPath = fieldName => ({ path }) => path && `${path}.${fieldName}` || fieldName

export const getIsLocalNumberSelectedForIndex = createSelector(
  getFormValueWithPath(getFiledPath('isLocalNumber')),
  isLocalNumber => isLocalNumber || false
)

export const getChargeTypeSelectedForIndex = createSelector(
  getFormValueWithPath(getFiledPath('chargeType')),
  chargeType => chargeType || false
)

export const getChargeTypeOther = createSelector(
  EntitySelectors.chargeTypes.getEntities,
  chargeTypes => chargeTypes.find(st => st.get('code').toLowerCase() === 'other') || Map()
)

export const getChargeTypeOtherId = createSelector(
  getChargeTypeOther,
  chargeTypeOther => chargeTypeOther.get('id') || ''
)

export const getIsChargeTypeOtherSelectedForIndex = createSelector(
  [getChargeTypeSelectedForIndex, getChargeTypeOtherId],
  (selectedChargeTypeId, chargeTypeOtherId) => selectedChargeTypeId === chargeTypeOtherId
)
