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
  editorStateTransformer,
  editorStatePlainTextTransformer,
  Mappers
} from 'util/redux-form/submitFilters'

export const printableFormTransformer = values => {
  const result = values
    .update('name', Mappers.languageMap(editorStatePlainTextTransformer))
    .update('shortDescription', Mappers.languageMap(editorStatePlainTextTransformer))
    .update('description', Mappers.languageMap(editorStateTransformer))
    .update('formFiles', Mappers.languageMap(Mappers.getFilteredList))
    .update('attachments', Mappers.languageMap(Mappers.getFilteredList))
    .update('emails', Mappers.languageMap(Mappers.getFilteredList))
    .update('phoneNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('deliveryAddress', da => da && da.size && da || null)

  return result
}

export const addressTransformer = values => {
  // if (values.hasIn(['deliveryAddress', 'address'])) {
  //   // Flattening deliveryAddress structure //
  //   let flatDeliveryAddress = values
  //     .getIn(['deliveryAddress'])
  //     .flatten(true)
  //   values = values.set('deliveryAddress', flatDeliveryAddress)
  //   // Renaming some properties //
  //   const newAddressPropertyNames = {
  //     municipality: 'municipalityId'
  //   }
  //   values = values
  //     .update('deliveryAddress', address => address
  //         .mapKeys(key => newAddressPropertyNames[key] || key))
  // }
  return values
}
