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
import { EntitySelectors, EnumsSelectors } from 'selectors'

const getItems = (_, { items }) => items
const getIndex = (_, { index }) => index

// FAX
const getFax = createSelector(
  [
    getIndex,
    getItems
  ],
  (index, items) => items && items.get(index) || Map()
)
// DIAL CODE
const getDialCodeId = createSelector(
  getFax,
  fax => fax.get('dialCode')
)
const getDialCodeEntity = createSelector(
  [
    getDialCodeId,
    EntitySelectors.dialCodes.getEntities
  ],
  (id, items) => items.get(id) || Map()
)
const getDialCode = createSelector(
  getDialCodeEntity,
  dialCode => dialCode.get('code')
)
// FAX NUMBER
export const getFaxNumber = createSelector(
  getFax,
  phone => phone.get('phoneNumber') || ''
)

export const getDefaultCodeId = createSelector(
  EnumsSelectors.serverConstants.getEntities,
  serverConstants => (serverConstants.filter(x => x.get('code').toLowerCase() === 'defaultdialcodeid')
    .first() || Map()).get('pattern') || ''
)

const getDefaultDialCode = createSelector(
  EntitySelectors.dialCodes.getEntities,
  getDefaultCodeId,
  (entityDialCodes, defId) => entityDialCodes.getIn([defId, 'code']) || null
)

export const getFaxNumberTitle = createSelector(
  [
    getDialCode,
    getFaxNumber,
    getDefaultDialCode
  ],
  (dialCode, faxNumber, defDial) => {
    const infoString = [dialCode || defDial, faxNumber].filter(x => x)
    return infoString.length > 0 && infoString.join(' ') || null
  }
)
