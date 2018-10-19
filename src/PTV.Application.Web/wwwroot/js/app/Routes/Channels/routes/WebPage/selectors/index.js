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
import { fromJS } from 'immutable'
import {
  getChannelOrganization,
  getChannelProperty,
  getDescription,
  getPhones,
  getEmails,
  getConnectionType,
  getId
} from 'Routes/Channels/selectors'
import { getLanguageAvailabilities } from 'selectors/copyEntity'

export const getWebPageChannel = createSelector(
  [
    getId,
    getLanguageAvailabilities,
    getChannelProperty('name'),
    getChannelOrganization,
    getChannelProperty('shortDescription'),
    getDescription,
    getChannelProperty('urlAddress'),
    getConnectionType,
    getChannelProperty('areaInformation'),
    getChannelProperty('languages'),
    getPhones,
    getEmails
  ], (
    id,
    languagesAvailabilities,
    name,
    organization,
    shortDescription,
    description,
    urlAddress,
    connectionType,
    areaInformation,
    languages,
    phoneNumbers,
    emails
  ) => fromJS({
    id,
    name,
    organization,
    shortDescription,
    urlAddress,
    description,
    connectionType,
    areaInformation,
    languages,
    phoneNumbers,
    emails,
    languagesAvailabilities
  })
)
