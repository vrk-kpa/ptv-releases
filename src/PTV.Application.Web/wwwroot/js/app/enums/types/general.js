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
import { stringUpperEnumFactory } from '../factory'
import { formTypesEnum } from '../form'

export const breakPointsDefinition = ['xl', 'lg', 'md', 'sm', 'xs']
export const breakPointsEnum = stringUpperEnumFactory(breakPointsDefinition)

export const publishingStatusCodesDefinition = ['Draft', 'Published', 'Modified', 'Deleted', 'OldPublished', 'Removed']
export const publishingStatusCodesEnum = stringUpperEnumFactory(publishingStatusCodesDefinition)

export const mainNavigationSlugs = ['search', 'connections', 'tasks', 'admin']
export const mainNavigationSlugsEnum = stringUpperEnumFactory(mainNavigationSlugs)

const sortDirectionTypes = ['asc', 'desc']
export const sortDirectionTypesEnum = stringUpperEnumFactory(sortDirectionTypes)

export const mainAccordionKeys = [
  'generalDescriptionAccordion',
  'serviceAccordion',
  'serviceCollectionAccordion',
  'organizationAccordion',
  'electronicAccordion',
  'webPageAccordion',
  'printableFormAccordion',
  'phoneAccordion',
  'serviceLocationAccordion',
  'connectionAccordion',
  'astiConnectionAccordion'
]
export const mainAccordionKeysEnum = stringUpperEnumFactory(mainAccordionKeys)
export const mainAccordionKeysByForm = {
  [formTypesEnum.GENERALDESCRIPTIONFORM]: mainAccordionKeysEnum.GENERALDESCRIPTIONACCORDION,
  [formTypesEnum.SERVICEFORM]: mainAccordionKeysEnum.SERVICEACCORDION,
  [formTypesEnum.SERVICECOLLECTIONFORM]: mainAccordionKeysEnum.SERVICECOLLECTIONACCORDION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: mainAccordionKeysEnum.ELECTRONICACCORDION,
  [formTypesEnum.ORGANIZATIONFORM]: mainAccordionKeysEnum.ORGANIZATIONACCORDION,
  [formTypesEnum.WEBPAGEFORM]: mainAccordionKeysEnum.WEBPAGEACCORDION,
  [formTypesEnum.PRINTABLEFORM]: mainAccordionKeysEnum.PRINTABLEFORMACCORDION,
  [formTypesEnum.PHONECHANNELFORM]: mainAccordionKeysEnum.PHONEACCORDION,
  [formTypesEnum.SERVICELOCATIONFORM]: mainAccordionKeysEnum.SERVICELOCATIONACCORDION,
  [formTypesEnum.CONNECTIONS]: mainAccordionKeysEnum.CONNECTIONS,
  [formTypesEnum.ASTICONNECTIONS]: mainAccordionKeysEnum.ASTICONNECTIONS
}
export const mainAccordionItemCountByForm = {
  [formTypesEnum.GENERALDESCRIPTIONFORM]: 2,
  [formTypesEnum.SERVICEFORM]: 3,
  [formTypesEnum.SERVICECOLLECTIONFORM]: 1,
  [formTypesEnum.ELECTRONICCHANNELFORM]: 2,
  [formTypesEnum.ORGANIZATIONFORM]: 1,
  [formTypesEnum.WEBPAGEFORM]: 1,
  [formTypesEnum.PRINTABLEFORM]:1,
  [formTypesEnum.PHONECHANNELFORM]: 2,
  [formTypesEnum.SERVICELOCATIONFORM]: 2,
  [formTypesEnum.CONNECTIONS]: 1,
  [formTypesEnum.ASTICONNECTIONS]: 1
}
