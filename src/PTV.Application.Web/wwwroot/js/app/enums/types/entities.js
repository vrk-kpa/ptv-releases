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
import { stringUpperEnumFactory } from '../factory'

export const entityTypes = ['services', 'channels', 'generalDescriptions', 'organizations', 'serviceCollections']
export const entityTypesEnum = stringUpperEnumFactory(entityTypes)

export const serviceContentTypes = [
  'serviceService',
  'serviceProfessional',
  'servicePermit'
]

export const channelsTypes = [
  'electronicChannel',
  'phoneChannel',
  'printableFormChannel',
  'serviceLocationChannel',
  'webPageChannel'
]
export const channelTypesEnum = stringUpperEnumFactory(channelsTypes)

export const channelContentTypes = [
  'eChannel',
  'webPage',
  'printableForm',
  'phone',
  'serviceLocation'
]

export const entityConcreteTypes = [
  'service',
  'generalDescription',
  'organization',
  'serviceCollection',
  ...channelsTypes
]
export const entityConcreteTypesEnum = stringUpperEnumFactory(entityConcreteTypes)

export const entityActionTypes = [
  'archive',
  'withdraw',
  'restore',
  'translate',
  'copy',
  'linkToModified',
  'linkToPublished',
  'createService'
]
export const entityActionTypesEnum = stringUpperEnumFactory(entityActionTypes)

export const entityCoverageTypes = [
  'entity',
  'language'
]
export const entityCoverageTypesEnum = stringUpperEnumFactory(entityCoverageTypes)

export const extraTypes = ['asti', 'sote']
export const extraTypesEnum = stringUpperEnumFactory(extraTypes)
