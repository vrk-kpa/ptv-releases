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
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { Map } from 'immutable'

const getItems = (_, { items }) => items
const getIndex = (_, { index }) => index
const getIsInCompareMode = (_, { compare }) => !!compare

const getLanguageCode = createSelector(
  [
    getContentLanguageCode,
    getSelectedComparisionLanguageCode,
    getIsInCompareMode
  ],
  (language, comparisionLanguageCode, compare) => {
    return compare && comparisionLanguageCode || language || 'fi'
  }
)

const getEInvoicing = createSelector(
  [
    getIndex,
    getItems
  ],
  (index, items) => items && items.get(index) || Map()
)

const getEInvoicingAddress = createSelector(
  getEInvoicing,
  eInvoicing => eInvoicing.get('electronicInvoicingAddress') || ''
)

const getEInvoicingOperatorCode = createSelector(
  getEInvoicing,
  eInvoicing => eInvoicing.get('operatorCode') || ''
)

const getEInvoicingInfo = createSelector(
  getEInvoicing,
  eInvoicing => eInvoicing.get('additionalInformation') || Map()
)
const getLocalizedEInvoicingInfo = createSelector(
  [
    getEInvoicingInfo,
    getLanguageCode
  ],
  (additionalInformation, language) => additionalInformation.get(language) || ''
)

export const getEInvoicingAddressTitle = createSelector(
  [
    getEInvoicingAddress,
    getEInvoicingOperatorCode,
    getLocalizedEInvoicingInfo
  ],
  (name, description, urlAddress) => {
    const infoString = [name, urlAddress, description].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)
