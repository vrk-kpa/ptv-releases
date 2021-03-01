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
import {
  // filterAttachment,
  editorStateTransformer,
  Mappers
  // filteredListMap
} from 'util/redux-form/submitFilters'

export const generalDescriptionBasicTransformer = values => {
  values = values.update('description', Mappers.languageMap(editorStateTransformer))
  values = values.update('conditionOfServiceUsage', Mappers.languageMap(editorStateTransformer))
  values = values.update('userInstruction', Mappers.languageMap(editorStateTransformer))
  values = values.update('laws', Mappers.getFilteredList)
  values = values.update('backgroundDescription', Mappers.languageMap(editorStateTransformer))
  values = values.update('keywords', Mappers.languageMap((value, key) =>
  value.subtract((values.get('newKeywords') || OrderedSet()).get(key))))  
  .delete('organizationProducerWarnning')
  values = values.update('deadLineInformation', Mappers.languageMap(editorStateTransformer))
  values = values.update('processingTimeInformation', Mappers.languageMap(editorStateTransformer))
  values = values.update('validityTimeInformation', Mappers.languageMap(editorStateTransformer))
  values = values.updateIn(['chargeType','additionalInformation'], Mappers.languageMap(editorStateTransformer))
  
  return values
}