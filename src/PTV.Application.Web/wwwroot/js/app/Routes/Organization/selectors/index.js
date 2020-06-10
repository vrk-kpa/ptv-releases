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
import { Map, List, OrderedMap } from 'immutable'
import { EntitySelectors, EnumsSelectors, BaseSelectors } from 'selectors'
import createCachedSelector from 're-reselect'
import { getEntitiesForIds } from 'selectors/base'
// import { getContentLanguageCode } from 'selectors/selections'
import { EditorState, convertFromRaw, ContentState } from 'draft-js'
import { getEntityLanguageAvailabilities } from 'selectors/common'
import {
  getAreaInformationWithDefaultWithoutOrganization
} from 'selectors/areaInformation'
import { getUserOrganization } from 'selectors/userInfo'
import {
  getMappedCollectionsData,
  getAddressCollectionsData,
  getLocalizedCollectionsData,
  getDescriptionsForQa
} from 'selectors/qualityAgent'

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

export const getOrganizationEntity = createSelector(
  EntitySelectors.getEntity,
  organization => organization
)

export const getOrganizationProperty = (propertyName, defaultValue = null) => createSelector(
  getOrganizationEntity,
  organization => organization.get(propertyName) != null ? organization.get(propertyName) : defaultValue
)

export const getOrganizationShortDescription = createCachedSelector(
  getOrganizationProperty('shortDescription', Map()),
  field => {
    try {
      return field.map(l =>
        l && EditorState.createWithContent(ContentState.createFromText(l))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)(
  (state, props) => EntitySelectors.getEntity(state, props).get('id') || ''
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
      emails.map(v => getEntitiesForIds(entities, v, List()).map((email, order) => email.set('order', order))) ||
      Map()
)

export const getElectronicInvoicingAddresses = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('electronicInvoicingAddresses') || List()
)

export const getElectronicInvoicingAddressesInitial = createSelector(
  [getElectronicInvoicingAddresses, EntitySelectors.electronicInvoicingAddresses.getEntities],
  (addresses, entities) => {
    return getEntitiesForIds(entities, addresses, List()).map((address, order) => address.set('order', order))
  }
)

export const getOrganizationWebPages = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('webPages') || Map()
)

export const getOrganizationWebPagesInitial = createSelector(
  [getOrganizationWebPages, EntitySelectors.webPages.getEntities],
  (webPages, entities) => webPages.size > 0 &&
      webPages.map(v => getEntitiesForIds(entities, v, List()).map((web, order) => web.set('order', order))) ||
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
  [getOrganizationPhoneNumbers, EntitySelectors.phoneNumbers.getEntities, EntitySelectors.dialCodes.getEntities],
  (phoneNumbers, entities, dialCodes) => phoneNumbers.size > 0 &&
      phoneNumbers.map(v => getEntitiesForIds(entities, v, List())
        .map((phone, order) => phone.set('order', order))
        .map((phone) => {
          const dial = dialCodes.get(phone.get('dialCode'))
          const dialCode = dial && dial.get('code') || ''
          return phone.set('wholePhoneNumber', dialCode + phone.get('phoneNumber'))
            .set('isLocalNumberParsed', phone.get('isLocalNumber'))
        })) ||
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
      getEntitiesForIds(entities, addresses, List()).map((address, order) => address.set('order', order)) ||
      List()
  }
)
export const getOrganizationVisitingAddressesInitial = createSelector(
  [getOrganizationVisitingAddresses, EntitySelectors.addresses.getEntities],
  (addresses, entities) => {
    return addresses.size > 0 &&
      getEntitiesForIds(entities, addresses, List()).map((address, order) => address.set('order', order)) ||
      List()
  }
)

const getOrganization = createSelector(
  [EntitySelectors.getEntity, getUserOrganization],
  (entity, userInfo) => entity.size ? entity.get('parentId') : userInfo
)

const getResponsibleOrganization = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('responsibleOrganizationRegionId')
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
    getElectronicInvoicingAddressesInitial,
    getResponsibleOrganization
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
    electronicInvoicingAddresses,
    responsibleOrganization
  ) => OrderedMap().mergeDeep({
    id: entity.get('id') || null,
    languagesAvailabilities,
    oid: entity.get('oid') || null,
    unificRootId: entity.get('unificRootId') || null,
    // basic info
    groupLevel: !entity.get('isMainOrganization'),
    organization,
    organizationType: entity.get('organizationType') || null,
    municipality: entity.get('municipality') || null,
    responsibleOrganization,
    areaInformation,
    name: entity.get('name') || Map(),
    alternateName: entity.get('alternateName') || Map(),
    isAlternateNameUsed,
    business,
    shortDescription,
    description,
    emails,
    phoneNumbers,
    webPages,
    postalAddresses,
    visitingAddresses,
    electronicInvoicingAddresses
  })
)

const descriptionFields = {
  shortDescription: { json: true, type: 'Summary' },
  description: { json: true, type: 'Description' }
}

const aditionalInformationFields = {
  emails : {
    additionalInformation: 'description'
  },
  phoneNumbers : {
    additionalInformation: 'additionalInformation',
    chargeDescription: 'chargeDescription' },
  webPages : {
    name : 'value'
  }
}

const localizedAditionalInformationFields = {
  electronicInvoicingAddresses : {
    property : 'value',
    value : 'additionalInformation',
    collection : 'electronicInvoicings' }
}

const addressFields = {
  postalAddresses : {
    property : 'value',
    value : 'additionalInformation',
    collection : 'addresses',
    type : 'Postal' },
  visitingAddresses : {
    property : 'value',
    value : 'additionalInformation',
    collection : 'addresses',
    type : 'Visiting' }
}

const additionalKeys = () => Object.keys(aditionalInformationFields)
  .map(key => aditionalInformationFields[key].collection || key)
  .filter((x, i, a) => a.indexOf(x) === i)
const additionalLocalizedKeys = () => Object.keys(localizedAditionalInformationFields)
  .map(key => localizedAditionalInformationFields[key].collection || key)
  .filter((x, i, a) => a.indexOf(x) === i)
const addressKeys = () => Object.keys(addressFields)
  .map(key => addressFields[key].collection || key)
  .filter((x, i, a) => a.indexOf(x) === i)

const getCollections = (organization, getFields, keys, keyFields) => {
  const fields = getFields(organization, keyFields)
  let result = []
  fields.forEach(field => {
    keys().forEach(coll => {
      if (result[coll]) {
        field[coll] && (result[coll] = result[coll].concat(field[coll]))
      } else {
        result[coll] = field[coll]
      }
    })
  })
  return result
}

const getOrganizationAditionalInformations = createSelector(
  BaseSelectors.getValuesByFormName,
  organization => organization && getCollections(organization, getMappedCollectionsData, additionalKeys, aditionalInformationFields)
)

const getOrganizationAddressAditionalInformations = createSelector(
  BaseSelectors.getValuesByFormName,
  organization => organization && getCollections(organization, getAddressCollectionsData, addressKeys, addressFields)
)

const getOrganizationLocalizedAditionalInformations = createSelector(
  BaseSelectors.getValuesByFormName,
  organization => organization && getCollections(organization, getLocalizedCollectionsData, additionalLocalizedKeys, localizedAditionalInformationFields)
)

const getOrganizationDescriptions = createSelector(
  BaseSelectors.getValuesByFormName,
  organization => organization && getDescriptionsForQa(
    organization,
    value => value.getCurrentContent(),
    descriptionFields,
    'organizationDescriptions')
)

export const getAdditionalQualityCheckData = createSelector(
  BaseSelectors.getFormValue('unificRootId'),
  BaseSelectors.getFormValue('alternativeId'),
  getOrganizationDescriptions,
  getOrganizationAditionalInformations,
  getOrganizationAddressAditionalInformations,
  getOrganizationLocalizedAditionalInformations,
  (id, alternativeId, organizationDescriptions, collections, addresses, localizedCollections) => ({
    id,
    alternativeId,
    organizationDescriptions,
    ...collections,
    ...addresses,
    ...localizedCollections
  })
)
