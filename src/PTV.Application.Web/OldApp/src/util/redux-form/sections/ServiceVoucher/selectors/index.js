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
import { EntitySelectors } from 'selectors'

export const getServiceVoucherTitle = (_, { data }) => data || Map()

export const getVoucherTypeUrl = createSelector(
  EntitySelectors.voucherTypes.getEntities,
  voucherTypes => voucherTypes.find(st => st.get('code').toLowerCase() === 'url') || Map()
)
export const getVoucherTypeUrlId = createSelector(
  getVoucherTypeUrl,
  voucherTypeUrl => voucherTypeUrl.get('id') || ''
)

export const getVoucherTypeNoUrl = createSelector(
  EntitySelectors.voucherTypes.getEntities,
  voucherTypes => voucherTypes.find(st => st.get('code').toLowerCase() === 'nourl') || Map()
)
export const getVoucherTypeNoUrlId = createSelector(
  getVoucherTypeNoUrl,
  voucherTypeNoUrl => voucherTypeNoUrl.get('id') || ''
)