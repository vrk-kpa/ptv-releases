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
import createCachedSelector from 're-reselect'
import { Map, List } from 'immutable'
import { EntitySelectors, BaseSelectors, EnumsSelectors } from 'selectors'
import { EditorState, ContentState, convertFromRaw } from 'draft-js'
import { createEntityPropertySelector, getOrganization } from 'selectors/common'
import { createFilteredField, getSelectedOrCopyEntity } from 'selectors/copyEntity'
import { getUserOrganization } from 'selectors/userInfo'
import { formChannelTypes, formTypesEnum } from 'enums'
import { getMappedCollectionsData,
  getAddressCollectionsData,
  getLocalizedCollectionsData,
  getNamesForQa,
  getDescriptionsForQa
} from 'selectors/qualityAgent'
import { getEntitiesForIds } from 'selectors/base'

export const getSafeId = (state, props) => EntitySelectors.getEntity(state, props).get('id') || ''

export const getChannel = createSelector(
  EntitySelectors.getEntity,
  channel => channel
)

export const getChannelProperty = (propertyName, defaultValue = null) => createCachedSelector(
  getChannel,
  channel => channel.get(propertyName) != null ? channel.get(propertyName) : defaultValue
)(
  (state, props) => `${getSafeId(state, props)}${propertyName}`
)

export const getTemplateId = createSelector(
  BaseSelectors.getParameterFromProps('templateId'),
  templateId => templateId
)

export const getTemplateOrganizationId = createCachedSelector(
  BaseSelectors.getParameterFromProps('templateId'),
  getSelectedOrCopyEntity,
  (templateId, entity) => templateId && entity.get('organizationId') || null
)(
  getSafeId
)

export const getChannelOrganization = createSelector(
  [getOrganization, getUserOrganization, getTemplateId],
  (organization, userOrganization, templateId) => templateId ? userOrganization : organization
)

export const getName = createCachedSelector(
  getChannelProperty('name', Map()),
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
  getSafeId
)

export const getShortDescription = createCachedSelector(
  getChannelProperty('shortDescription', Map()),
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
  getSafeId
)

export const getDescription = createCachedSelector(
  createFilteredField(createEntityPropertySelector(getSelectedOrCopyEntity, 'description', Map())),
  field => {
    try {
      return field.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)(
  getSafeId
)

export const getAttachments = createSelector(
  [getChannelProperty('attachments'), EntitySelectors.webPages.getEntities],
  (attachments, entities) =>
    attachments &&
    attachments.map(x => BaseSelectors.getEntitiesForIds(entities, x, List()).map((att, order) => att.set('order', order))) ||
    Map()
)

export const getFormFiles = createSelector(
  [getChannelProperty('formFiles'), EntitySelectors.webPages.getEntities],
  (formFiles, entities) =>
    formFiles &&
    formFiles.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

export const getWebPages = createCachedSelector(
  [getChannelProperty('webPages'), EntitySelectors.webPages.getEntities],
  (formFiles, entities) =>
    formFiles && formFiles.map(x =>
      BaseSelectors.getEntitiesForIds(entities, x, List()).map((web, order) =>
        web.set('order', order))) || Map()
)(
  getSafeId
)

export const getPhoneNumberTypePhoneType = createSelector(
  EnumsSelectors.phoneNumberTypes.getEntities,
  chargeTypes => (
    chargeTypes.filter(aT => aT.get('code').toLowerCase() === 'phone')
      .first() || Map()).get('id') || ''
)

export const getChargedChargeType = createSelector(
  EnumsSelectors.chargeTypes.getEntities,
  chargeTypes => (
    chargeTypes.filter(aT => aT.get('code').toLowerCase() === 'charged')
      .first() || Map()).get('id') || ''
)

export const getDefaultPhoneNumber = createSelector(
  [getChargedChargeType, getPhoneNumberTypePhoneType],
  (chargeType, phoneType) => Map({ chargeType: chargeType, isLocalNumber: false, type: phoneType })
)

export const getPhones = createCachedSelector(
  [getChannelProperty('phoneNumbers'), EntitySelectors.phoneNumbers.getEntities, EntitySelectors.dialCodes.getEntities, EnumsSelectors.serviceNumbers.getEnums],
  (phones, entities, dialCodes, serviceNumbers) => {
    return phones &&
    phones.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())
      .map((phone, order) => phone.set('order', order))
      .map((phone) => {
        const dial = dialCodes.get(phone.get('dialCode'))
        const dialCode = dial && dial.get('code') || ''
        const phoneNumber = phone && phone.get('phoneNumber') || null  
        const servicePrefixies = phoneNumber && serviceNumbers && serviceNumbers.filter(serviceNumber => phoneNumber.startsWith(serviceNumber))
        const isLocalNumberParsed = phone.get('isLocalNumber') && servicePrefixies && servicePrefixies.size > 0 
        return phone.set('wholePhoneNumber', dialCode + phone.get('phoneNumber'))
          .set('isLocalNumberParsed', isLocalNumberParsed)
      })) ||
    Map()
  }
)(
  getSafeId
)

export const getDeliveryAddresses = createSelector(
  [getChannelProperty('deliveryAddresses'), EntitySelectors.addresses.getEntities],
  (addresses, entities) =>
    BaseSelectors.getEntitiesForIds(entities, addresses, List()).map((addr, order) =>
      addr.set('order', order))
)

export const getEmails = createCachedSelector(
  [getChannelProperty('emails'), EntitySelectors.emails.getEntities],
  (emails, entities) =>
    emails &&
    emails.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)(
  getSafeId
)

export const getAccessibilityClassifications = createSelector(
  [getChannelProperty('accessibilityClassifications'),
    EntitySelectors.accessibilityClassifications.getEntities
  ],
  (accessibilityClassifications, entities) =>
    accessibilityClassifications &&
    accessibilityClassifications.map(x => entities.get(x) || Map()) || Map()
)

export const getDefaultConnectionType = createSelector(
  EnumsSelectors.serviceChannelConnectionTypes.getEntities,
  connectionTypes => (
    connectionTypes.filter(cT => cT.get('code').toLowerCase() === 'commonforall')
      .first() || Map()).get('id') || ''
)

export const getConnectionType = createSelector(
  [getChannelProperty('connectionType'), getDefaultConnectionType, getTemplateId],
  (connectionType, defaultConnectionType, templateId) => templateId
    ? defaultConnectionType
    : connectionType || defaultConnectionType
)

export const getId = createCachedSelector(
  getChannel,
  entity => entity.get('id') || null
)(
  getSafeId
)

export const getUnificRootId = createCachedSelector(
  getChannel,
  entity => entity.get('unificRootId') || null
)(
  getSafeId
)

const getOpeningHoursProperty = property => createCachedSelector(
  getChannelProperty('openingHours', Map()),
  hours => hours.get(property)
)(
  (state, props) => `${getSafeId(state, props)}${property}`
)

const getNormalHours = createCachedSelector(
  [getOpeningHoursProperty('normalOpeningHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => BaseSelectors.getEntitiesForIds(entities, hours, List())
    .map(oh => oh.update('dailyOpeningHours', days => days.filter(x => x)))
    .map((hour, order) => hour.set('order', order))
)(
  getSafeId
)

const getSpecialHours = createCachedSelector(
  [getOpeningHoursProperty('specialOpeningHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => BaseSelectors.getEntitiesForIds(entities, hours, List())
    .map((hour, order) => hour.set('order', order))
)(
  getSafeId
)

const getExceptionalHours = createCachedSelector(
  [getOpeningHoursProperty('exceptionalOpeningHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => BaseSelectors.getEntitiesForIds(entities, hours, List())
    .map((hour, order) => hour.set('order', order))
)(
  getSafeId
)

const getHolidayHours = createCachedSelector(
  [getOpeningHoursProperty('holidayHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => {
    const holidays = BaseSelectors.getEntitiesForIds(entities, hours, List())
      .map((hour, order) => hour.set('order', order))
    let result = Map()
    holidays.forEach(holiday => {
      result = result.set(holiday.get('code'), Map({
        active : true,
        intervals: holiday.get('intervals'),
        type: holiday.get('isClosed') ? 'close' : 'open'
      }))
    })
    return result
  }
)(
  getSafeId
)

export const getOpeningHours = createCachedSelector(
  [
    getNormalHours,
    getSpecialHours,
    getExceptionalHours,
    getHolidayHours
  ], (
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours,
    holidayHours
  ) => Map().mergeDeep({
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours,
    holidayHours
  })
)(
  getSafeId
)

const descriptionFields = {
  shortDescription: { json: true, type: 'Summary' },
  description: { json: true, type: 'Description' }
}

const nameFields = {
  name: { json: true, type: 'Name' }
}

const getChannelNames = createSelector(
  BaseSelectors.getValuesByFormName,
  (channel) => channel && getNamesForQa(channel, nameFields, 'serviceChannelNames')
)

const getServiceChannelDescriptions = createSelector(
  BaseSelectors.getValuesByFormName,
  channel => channel && getDescriptionsForQa(
    channel,
    value => value.getCurrentContent(),
    descriptionFields,
    'serviceChannelDescriptions'
  )
)

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

const phoneNumbers = {
  additionalInformation: 'additionalInformation',
  chargeDescription: 'chargeDescription'
}

const attachments = {
  name : 'name',
  description: 'description'
}

const aditionalInformationFields = {
  [formTypesEnum.ELECTRONICCHANNELFORM] : {
    phoneNumbers,
    attachments
  },
  [formTypesEnum.PHONECHANNELFORM]: {
    phoneNumbers
  },
  [formTypesEnum.PRINTABLEFORM]: {
    phoneNumbers,
    attachments
  },
  [formTypesEnum.SERVICELOCATIONFORM]: {
    phoneNumbers,
    webPages : {
      name : 'value'
    }
  },
  [formTypesEnum.WEBPAGEFORM]: {
    phoneNumbers
  }
}

const openingHourAttributes = {
  property : 'value',
  value : 'title',
  collection : 'serviceHours'
}

const normalOpeningHours = {
  ...openingHourAttributes,
  rest: { serviceHourType : 'DaysOfTheWeek' }
}

const specialOpeningHours = {
  ...openingHourAttributes,
  rest: { serviceHourType : 'OverMidnight' }
}

const exceptionalOpeningHours = {
  ...openingHourAttributes,
  rest: { serviceHourType : 'Exceptional' }
}

const openingHoursFields = {
  [formTypesEnum.ELECTRONICCHANNELFORM] : {
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours
  },
  [formTypesEnum.PHONECHANNELFORM]: {
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours
  },
  [formTypesEnum.SERVICELOCATIONFORM]: {
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours
  },
  [formTypesEnum.WEBPAGEFORM]: {},
  [formTypesEnum.PRINTABLEFORM]: {}
}

const addressFields = {
  [formTypesEnum.PRINTABLEFORM]: {
    deliveryAddresses : {
      property : 'value',
      value : (type) => type === 'NoAddress' ? 'noAddressAdditionalInformation' : 'additionalInformation',
      type : 'Delivery'
    }
  },
  [formTypesEnum.SERVICELOCATIONFORM]: {
    postalAddresses : {
      property : 'value',
      value : 'additionalInformation',
      collection : 'addresses',
      type : 'Postal' },
    visitingAddresses : {
      property : 'value',
      value : 'additionalInformation',
      collection : 'addresses',
      type : 'Location' }
  },
  [formTypesEnum.WEBPAGEFORM]: {},
  [formTypesEnum.ELECTRONICCHANNELFORM] : {},
  [formTypesEnum.PHONECHANNELFORM]: {}
}

const additionalKeys = (type) => Object.keys(aditionalInformationFields[type])
  .map(key => aditionalInformationFields[type][key].collection || key)
  .filter((x, i, a) => a.indexOf(x) === i)

const addressKeys = (type) => Object.keys(addressFields[type])
  .map(key => addressFields[type][key].collection || key)
  .filter((x, i, a) => a.indexOf(x) === i)

const openningHoursKeys = (type) => Object.keys(openingHoursFields[type])
  .map(key => openingHoursFields[type][key].collection || key)
  .filter((x, i, a) => a.indexOf(x) === i)

const getChannelAditionalInformations = createSelector(
  BaseSelectors.getValuesByFormName,
  BaseSelectors.getParameterFromProps('formName'),
  (channel, formName) => channel && getCollections(channel, getMappedCollectionsData, () => additionalKeys(formName), aditionalInformationFields[formName])
)

const getChannelAddressAditionalInformations = createSelector(
  BaseSelectors.getValuesByFormName,
  BaseSelectors.getParameterFromProps('formName'),
  (channel, formName) => channel && getCollections(channel, getAddressCollectionsData, () => addressKeys(formName), addressFields[formName])
)

const getChannelOpenningHours = createSelector(
  BaseSelectors.getValuesByFormName,
  BaseSelectors.getParameterFromProps('formName'),
  (channel, formName) => channel && getCollections(channel.get('openingHours') || Map(), getLocalizedCollectionsData, () => openningHoursKeys(formName), openingHoursFields[formName])
)

export const getChannelAdditionalQualityCheckData = createSelector(
  BaseSelectors.getParameterFromProps('formName'),
  BaseSelectors.getFormValue('unificRootId'),
  BaseSelectors.getFormValue('alternativeId'),
  getChannelNames,
  getServiceChannelDescriptions,
  getChannelAditionalInformations,
  getChannelAddressAditionalInformations,
  getChannelOpenningHours,
  (formName, id, alternativeId, serviceChannelNames, serviceChannelDescriptions, collections, addresses, openingHours) => ({
    id,
    alternativeId,
    type: formChannelTypes[formName],
    serviceChannelNames,
    serviceChannelDescriptions,
    ...collections,
    ...addresses,
    ...openingHours
  })
)

const getServiceCollectionsIds = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('serviceCollections') || List()
)

export const getServiceCollections = createSelector(
  [getServiceCollectionsIds, EntitySelectors.connections.getEntities],
  (entityIds, entities) => getEntitiesForIds(entities, entityIds, List())
)
