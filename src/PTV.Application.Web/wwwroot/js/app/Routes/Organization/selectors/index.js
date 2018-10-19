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
import { Map, List } from 'immutable'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getEntitiesForIds } from 'selectors/base'
// import { getContentLanguageCode } from 'selectors/selections'
import { EditorState, convertFromRaw } from 'draft-js'
import { getEntityLanguageAvailabilities } from 'selectors/common'
import {
  getAreaInformationWithDefaultWithoutOrganization
} from 'selectors/areaInformation'
import { getUserOrganization } from 'selectors/userInfo'

export const getOrganizationDescription = createSelector(
  EntitySelectors.getEntity,
  entity => {
    try {
      return entity.get('description') &&
      entity.get('description').map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

export const getOrganizationShortDescription = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('shortDescription') || Map()
)

const getAlternateNameUsed = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('isAlternateNameUsedAsDisplayName') || List()
)

export const getIsAlternateNameUsed = createSelector(
  getAlternateNameUsed,
  alters => alters.size > 0 ? alters.reduce((acc, curr) => acc.set(curr, true), Map()) : null
)

export const getOrganizationEmails = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('emails') || Map()
)

export const getOrganizationEmailsInitial = createSelector(
  [getOrganizationEmails, EntitySelectors.emails.getEntities],
  (emails, entities) => emails.size > 0 &&
      emails.map(v => getEntitiesForIds(entities, v, List())) ||
      Map()
)

export const getElectronicInvoicingAddresses = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('electronicInvoicingAddresses') || List()
)

export const getElectronicInvoicingAddressesInitial = createSelector(
  [getElectronicInvoicingAddresses, EntitySelectors.electronicInvoicingAddresses.getEntities],
  (addresses, entities) => {
    return getEntitiesForIds(entities, addresses, List())
  }
)

export const getOrganizationWebPages = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('webPages') || Map()
)

export const getOrganizationWebPagesInitial = createSelector(
  [getOrganizationWebPages, EntitySelectors.webPages.getEntities],
  (webPages, entities) => webPages.size > 0 &&
      webPages.map(v => getEntitiesForIds(entities, v, List())) ||
      Map()
)

export const getOrganizationPhoneNumbers = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('phoneNumbers') || Map()
)

export const getChargedChargeType = createSelector(
  EnumsSelectors.chargeTypes.getEntities,
  chargeTypes => (
    chargeTypes.filter(aT => aT.get('code').toLowerCase() === 'charged')
      .first() || Map()).get('id') || ''
)

export const getDefaultPhoneNumber = createSelector(
  getChargedChargeType,
  chargeType => Map({ chargeType: chargeType, isLocalNumber: true })
)

export const getOrganizationPhoneNumbersInitial = createSelector(
  [getOrganizationPhoneNumbers, EntitySelectors.phoneNumbers.getEntities, getChargedChargeType],
  (phoneNumbers, entities, chargeType) => phoneNumbers.size > 0 &&
      phoneNumbers.map(v => getEntitiesForIds(entities, v, List())) ||
      Map()
  )

export const getBusiness = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('business') || null
)

export const getBusinessEntity = createSelector(
  [EntitySelectors.business.getEntities, getBusiness],
  (businessEntities, id) => businessEntities.get(id) || null
)

export const getOrganizationPostalAddresses = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('postalAddresses') || Map()
)

export const getOrganizationVisitingAddresses = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('visitingAddresses') || Map()
)

export const getOrganizationPostalAddressesInitial = createSelector(
  [getOrganizationPostalAddresses, EntitySelectors.addresses.getEntities],
  (addresses, entities) => {
    return addresses.size > 0 &&
      getEntitiesForIds(entities, addresses, List()) ||
      List()
  }
  )
export const getOrganizationVisitingAddressesInitial = createSelector(
  [getOrganizationVisitingAddresses, EntitySelectors.addresses.getEntities],
  (addresses, entities) => {
    return addresses.size > 0 &&
      getEntitiesForIds(entities, addresses, List()) ||
      List()
  }
  )

const getOrganization = createSelector(
    [EntitySelectors.getEntity, getUserOrganization],
    (entity, userInfo) => entity.size ? entity.get('parentId') : userInfo
  )

export const getOrganizationFormInitialValues = createSelector(
  [
    EntitySelectors.getEntity,
    getOrganizationDescription,
    getOrganizationShortDescription,
    getIsAlternateNameUsed,
    getOrganizationEmailsInitial,
    getOrganizationWebPagesInitial,
    getOrganizationPhoneNumbersInitial,
    getAreaInformationWithDefaultWithoutOrganization,
    getBusinessEntity,
    getOrganizationPostalAddressesInitial,
    getOrganizationVisitingAddressesInitial,
    getEntityLanguageAvailabilities,
    getOrganization,
    getElectronicInvoicingAddressesInitial
  ], (
    entity,
    description,
    shortDescription,
    isAlternateNameUsed,
    emails,
    webPages,
    phoneNumbers,
    areaInformation,
    business,
    postalAddresses,
    visitingAddresses,
    languagesAvailabilities,
    organization,
    electronicInvoicingAddresses
  ) => ({
    id: entity.get('id') || null,
    name: entity.get('name') || Map(),
    description,
    shortDescription,
    alternateName: entity.get('alternateName') || Map(),
    groupLevel:  !entity.get('isMainOrganization'),
    isAlternateNameUsed,
    business,
    oid: entity.get('oid') || null,
    organization,
    organizationType: entity.get('organizationType') || null,
    municipality: entity.get('municipality') || null,
    areaInformation,
    emails,
    phoneNumbers,
    webPages,
    postalAddresses,
    visitingAddresses,
    languagesAvailabilities,
    electronicInvoicingAddresses
  }
  )
)
