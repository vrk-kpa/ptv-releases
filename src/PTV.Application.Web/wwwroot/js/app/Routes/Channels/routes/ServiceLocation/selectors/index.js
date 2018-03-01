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
import { fromJS, List, Map } from 'immutable'
import {
  // getChannel,
  getChannelProperty,
  getDescription,
  getPhones,
  getEmails,
  getWebPages,
  getConnectionType,
  getId,
  getOpeningHours
} from 'Routes/Channels/selectors'
import { getOrganization } from 'selectors/common'
import { getEntityLanguageAvailabilities } from 'selectors/common'
import { getAreaInformationOrDefaultEmpty } from 'selectors/areaInformation'
import { BaseSelectors, EntitySelectors } from 'selectors'

const getFaxNumbers = createSelector(
  [getChannelProperty('faxNumbers'), EntitySelectors.phoneNumbers.getEntities],
  (phones, entities) =>
    phones &&
    phones.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

const getVisitingAddresses = createSelector(
  [getChannelProperty('visitingAddresses'), EntitySelectors.addresses.getEntities],
  (addresses, entities) => BaseSelectors.getEntitiesForIds(entities, addresses, List())
)

const getPostalAddresses = createSelector(
  [getChannelProperty('postalAddresses'), EntitySelectors.addresses.getEntities],
  (addresses, entities) => BaseSelectors.getEntitiesForIds(entities, addresses, List())
)

export const getServiceLocationChannel = createSelector(
  [
    getId,
    getChannelProperty('name'),
    getOrganization,
    getChannelProperty('shortDescription'),
    getDescription,
    getWebPages,
    getConnectionType,
    getAreaInformationOrDefaultEmpty,
    getChannelProperty('languages'),
    getPhones,
    getFaxNumbers,
    getEmails,
    getPostalAddresses,
    getVisitingAddresses,
    getEntityLanguageAvailabilities,
    getOpeningHours
  ], (
    id,
    name,
    organization,
    shortDescription,
    description,
    webPages,
    connectionType,
    areaInformation,
    languages,
    phoneNumbers,
    faxNumbers,
    emails,
    postalAddresses,
    visitingAddresses,
    languagesAvailabilities,
    openingHours
  ) => fromJS({
    id,
    name,
    organization,
    shortDescription,
    webPages,
    description,
    connectionType,
    areaInformation,
    languages,
    phoneNumbers,
    faxNumbers,
    emails,
    postalAddresses,
    visitingAddresses,
    languagesAvailabilities,
    openingHours
  })
)
