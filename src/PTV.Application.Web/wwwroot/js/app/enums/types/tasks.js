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

export const taskTypesDefinition = [
  'outdatedDraftServices',
  'outdatedPublishedServices',
  'outdatedDraftChannels',
  'outdatedPublishedChannels',
  'servicesWithoutChannels',
  'channelsWithoutServices',
  'translationArrived',
  'missingLanguageOrganizations'
]
export const taskTypesEnum = stringUpperEnumFactory(taskTypesDefinition)
export const taskTypes = {
  [taskTypesEnum.OUTDATEDDRAFTSERVICES]: 'outdatedDraftServices',
  [taskTypesEnum.OUTDATEDPUBLISHEDSERVICES]: 'outdatedPublishedServices',
  [taskTypesEnum.OUTDATEDDRAFTCHANNELS]: 'outdatedDraftChannels',
  [taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS]: 'outdatedPublishedChannels',
  [taskTypesEnum.SERVICESWITHOUTCHANNELS]: 'servicesWithoutChannels',
  [taskTypesEnum.CHANNELSWITHOUTSERVICES]: 'channelsWithoutServices',
  [taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS]: 'missingLanguageOrganizations',
  [taskTypesEnum.TRANSLATIONARRIVED]: 'translationArrived'
}

export const notificationTypesDefinition = [
  'serviceChannelInCommonUse',
  'contentUpdated',
  'contentArchived',
  'generalDescriptionCreated',
  'generalDescriptionUpdated',
  'connectionChanges'
]
export const notificationTypesEnum = stringUpperEnumFactory(notificationTypesDefinition)
export const notificationTypes = {
  [notificationTypesDefinition.SERVICECHANNELINCOMMONUSE]: 'serviceChannelInCommonUse',
  [notificationTypesDefinition.CONTENTUPDATED]: 'contentUpdated',
  [notificationTypesDefinition.CONTENTARCHIVED]: 'contentArchived',
  [notificationTypesDefinition.GENERALDESCRIPTIONCREATED]: 'generalDescriptionCreated',
  [notificationTypesDefinition.GENERALDESCRIPTIONUPDATED]: 'generalDescriptionUpdated',
  [notificationTypesDefinition.CONNECTIONCHANGES]:'connectionChanges'
}
