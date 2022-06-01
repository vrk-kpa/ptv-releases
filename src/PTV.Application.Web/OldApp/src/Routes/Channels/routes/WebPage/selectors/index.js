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
  getChannelOrganization,
  getChannelProperty,
  getName,
  getShortDescription,
  getDescription,
  getPhones,
  getEmails,
  getConnectionType,
  getId,
  getUnificRootId,
  getAccessibilityClassifications,
  getTemplateId,
  getTemplateOrganizationId,
  getServiceCollections
} from 'Routes/Channels/selectors'
import { getLanguageAvailabilities } from 'selectors/copyEntity'
import { getAreaInformationOrDefaultEmpty } from 'selectors/areaInformation'

export const getWebPageChannel = createSelector(
  [
    getId,
    getUnificRootId,
    getLanguageAvailabilities,
    getName,
    getChannelOrganization,
    getShortDescription,
    getDescription,
    getChannelProperty('urlAddress'),
    getConnectionType,
    getChannelProperty('languages'),
    getPhones,
    getEmails,
    getAreaInformationOrDefaultEmpty,
    getAccessibilityClassifications,
    getTemplateId,
    getTemplateOrganizationId,
    getServiceCollections
  ], (
    id,
    unificRootId,
    languagesAvailabilities,
    name,
    organization,
    shortDescription,
    description,
    urlAddress,
    connectionType,
    languages,
    phoneNumbers,
    emails,
    areaInformation,
    accessibilityClassifications,
    templateId,
    templateOrganizationId,
    serviceCollections
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
    urlAddress,
    languages,
    phoneNumbers,
    emails,
    areaInformation,
    connectionType,
    accessibilityClassifications,
    serviceCollections
  })
)
