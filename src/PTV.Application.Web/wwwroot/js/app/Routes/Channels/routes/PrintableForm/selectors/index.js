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
import { OrderedMap } from 'immutable'
import {
  getId,
  getUnificRootId,
  getPhones,
  getAttachments,
  getDeliveryAddresses,
  getEmails,
  getName,
  getShortDescription,
  getDescription,
  getFormFiles,
  getConnectionType,
  getChannelProperty,
  getChannelOrganization,
  getTemplateId,
  getTemplateOrganizationId
} from 'Routes/Channels/selectors'
import { getAreaInformationOrDefaultEmpty } from 'selectors/areaInformation'
import { getLanguageAvailabilities } from 'selectors/copyEntity'

export const getPrintableForm = createSelector([
  getId,
  getUnificRootId,
  getName,
  getChannelOrganization,
  getShortDescription,
  getChannelProperty('languages'),
  getDescription,
  getPhones,
  getAttachments,
  getEmails,
  getFormFiles,
  getAreaInformationOrDefaultEmpty,
  getDeliveryAddresses,
  getLanguageAvailabilities,
  getConnectionType,
  getChannelProperty('formIdentifier'),
  getTemplateId,
  getTemplateOrganizationId
], (
  id,
  unificRootId,
  name,
  organization,
  shortDescription,
  languages,
  description,
  phoneNumbers,
  attachments,
  emails,
  formFiles,
  areaInformation,
  deliveryAddresses,
  languagesAvailabilities,
  connectionType,
  formIdentifier,
  templateId,
  templateOrganizationId
) => OrderedMap().mergeDeep({
  // general fields
  id,
  unificRootId,
  languagesAvailabilities,
  templateId,
  templateOrganizationId,

  // basic info
  name,
  organization,
  shortDescription,
  description,
  formIdentifier,
  formFiles,
  deliveryAddresses,
  languages,
  phoneNumbers,
  emails,
  attachments,
  areaInformation,
  connectionType
}))
