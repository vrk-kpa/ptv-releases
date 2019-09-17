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
import { Map, OrderedMap } from 'immutable'
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
  getOpeningHours,
  getTemplateId,
  getTemplateOrganizationId
} from 'Routes/Channels/selectors'
import { getAreaInformationOrDefaultEmpty } from 'selectors/areaInformation'
import { getLanguageAvailabilities } from 'selectors/copyEntity'

export const getPhoneChannel = createSelector(
  [
    getId,
    getUnificRootId,
    getName,
    getChannelOrganization,
    getShortDescription,
    getDescription,
    getChannelProperty('webPage', Map()),
    getConnectionType,
    getAreaInformationOrDefaultEmpty,
    getChannelProperty('languages'),
    getPhones,
    getEmails,
    getLanguageAvailabilities,
    getOpeningHours,
    getTemplateId,
    getTemplateOrganizationId
  ], (
    id,
    unificRootId,
    name,
    organization,
    shortDescription,
    description,
    webPage,
    connectionType,
    areaInformation,
    languages,
    phoneNumbers,
    emails,
    languagesAvailabilities,
    openingHours,
    templateId,
    templateOrganizationId
  ) => OrderedMap().mergeDeep({
    // general fields
    id,
    unificRootId,
    templateId,
    templateOrganizationId,
    languagesAvailabilities,

    // basic ionfo
    name,
    organization,
    shortDescription,
    description,
    phoneNumbers,
    webPage,
    languages,
    emails,
    areaInformation,
    connectionType,

    // opening hours
    openingHours
  })
)
