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
  getChannelOrganization,
  getChannelProperty,
  getDescription,
  getPhones,
  getEmails,
  getWebPages,
  getConnectionType,
  getId,
  getUnificRootId,
  getOpeningHours
} from 'Routes/Channels/selectors'
import { getAreaInformationOrDefaultEmpty } from 'selectors/areaInformation'
import { BaseSelectors, EntitySelectors } from 'selectors'
import { getLanguageAvailabilities } from 'selectors/copyEntity'

const getFaxNumbers = createSelector(
  [getChannelProperty('faxNumbers'), EntitySelectors.phoneNumbers.getEntities],
  (phones, entities) =>
    phones &&
    phones.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

const getVisitingAddresses = createSelector(
  [
    getChannelProperty('visitingAddresses'),
    EntitySelectors.addresses.getEntities,
    EntitySelectors.accessibilityRegisters.getEntities
  ],
  (addresses, addressEntities, accessibilityRegisterEntities) => {
    if (!addresses) return List()
    return addresses
      .map(addressId => addressEntities.get(addressId))
      .map(address => address.update(
        'accessibilityRegister',
        accessibilityRegisterId => accessibilityRegisterEntities.get(accessibilityRegisterId)
      ))
  }
)
const getPostalAddresses = createSelector(
  [getChannelProperty('postalAddresses'), EntitySelectors.addresses.getEntities],
  (addresses, entities) => BaseSelectors.getEntitiesForIds(entities, addresses, List())
)
const getAccessibilityRegisterId = createSelector(
  getVisitingAddresses,
  addresses => {
    return addresses.getIn([0, 'accessibilityRegister', 'id'])
  }
)
const getIsAccessibilityRegisterSet = createSelector(
  getVisitingAddresses,
  addresses => {
    return !!addresses.getIn([0, 'accessibilityRegister', 'id'])
  }
)

const getAccessibilityRegisterMeta = createSelector(
  [
    getAccessibilityRegisterId,
    getIsAccessibilityRegisterSet,
    getChannelProperty('accessibilityMeta')
  ],
  (
    accessibilityRegisterId,
    isAccessibilityRegisterSet,
    accessibilityMeta
  ) => isAccessibilityRegisterSet
    ? Map({
      id: accessibilityRegisterId,
      isChanged: false,
      isDeleted: false
    })
    : Map({
      id: null,
      isChanged: false,
      isDeleted: false
    })
)

const getAlternateNameUsed = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('isAlternateNameUsedAsDisplayName') || List()
)

export const getIsAlternateNameUsed = createSelector(
  getAlternateNameUsed,
  alters => alters.size > 0 ? alters.reduce((acc, curr) => acc.set(curr, true), Map()) : null
)

export const getServiceLocationChannel = createSelector(
  [
    getId,
    getUnificRootId,
    getChannelProperty('name'),
    getChannelProperty('alternateName'),
    getIsAlternateNameUsed,
    getChannelOrganization,
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
    getLanguageAvailabilities,
    getOpeningHours,
    getAccessibilityRegisterMeta,
    getChannelProperty('oid')
  ], (
    id,
    unificRootId,
    name,
    alternateName,
    isAlternateNameUsed,
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
    openingHours,
    accessibilityMeta,
    oid
  ) => {
    const result = fromJS({
      id,
      unificRootId,
      name,
      alternateName,
      isAlternateNameUsed,
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
      openingHours,
      accessibilityMeta,
      oid
    })
    if (!unificRootId) {
      return result.delete('unificRootId')
    }
    return result
  }
)
