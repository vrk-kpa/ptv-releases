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

import {
  Set
} from 'immutable'

const keyIn = keys => {
  const keySet = Set(keys)
  return (v, k) => keySet.has(k)
}

const servicePropertyNames = {
  isAlternateNameUsed: 'isAlternateNameUsedAsDisplayName'
}

export const serviceLocationChannelTransformer = values => {
  values = values.update(x => x.mapKeys(key => servicePropertyNames[key] || key))
  return values
    .update('name', Mappers.languageMap(editorStatePlainTextTransformer))
    .update('shortDescription', Mappers.languageMap(editorStatePlainTextTransformer))
    .update('description', Mappers.languageMap(editorStateTransformer))
    .update('emails', Mappers.languageMap(Mappers.getFilteredList))
    .update('phoneNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('faxNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('webPages', Mappers.languageMap(Mappers.getFilteredList))
    .update('isAlternateNameUsedAsDisplayName', x => x && x.filter(y => y).keySeq().toList())
}
export const accessibilityRegisterTransformer = values => {
  return values
    .update('visitingAddresses', visitingAddresses => {
      return visitingAddresses
        .map(visitingAddress => visitingAddress.update('accessibilityRegister', accessibilityRegister => {
          if (!accessibilityRegister) return null
          return accessibilityRegister.filter(keyIn([
            'id',
            'entranceId',
            'isMainEntrance'
          ]))
        }))
    })
}
