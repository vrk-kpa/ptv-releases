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
import { EntitySelectors, BaseSelectors, EnumsSelectors } from 'selectors'
import { EditorState, convertFromRaw } from 'draft-js'

export const getChannel = createSelector(
  EntitySelectors.getEntity,
  channel => channel
)

export const getChannelProperty = (propertyName, defaultValue = null) => createSelector(
  getChannel,
  channel => channel.get(propertyName) != null ? channel.get(propertyName) : defaultValue
)

export const getDescription = createSelector(
  getChannelProperty('description'),
  desc => desc &&
    desc.map(l => l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))) ||
    Map()
)

export const getAttachments = createSelector(
  [getChannelProperty('attachments'), EntitySelectors.webPages.getEntities],
  (attachments, entities) =>
    attachments &&
    attachments.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

export const getFormFiles = createSelector(
  [getChannelProperty('formFiles'), EntitySelectors.webPages.getEntities],
  (formFiles, entities) =>
    formFiles &&
    formFiles.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

export const getWebPages = createSelector(
  [getChannelProperty('webPages'), EntitySelectors.webPages.getEntities],
  (formFiles, entities) =>
    formFiles &&
    formFiles.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
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

export const getPhones = createSelector(
  [getChannelProperty('phoneNumbers'), EntitySelectors.phoneNumbers.getEntities, getChargedChargeType,
    getPhoneNumberTypePhoneType],
  (phones, entities, chargeType, phoneType) =>
    phones &&
    phones.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

export const getDeliveryAddress = createSelector(
  [getChannelProperty('deliveryAddress'), EntitySelectors.addresses.getEntities],
  (addressId, addresses) => {
    const result = addresses.get(addressId) || Map()
    return result
  }
)

export const getEmails = createSelector(
  [getChannelProperty('emails'), EntitySelectors.emails.getEntities],
  (emails, entities) =>
    emails &&
    emails.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
)

export const getDefaultConnectionType = createSelector(
  EnumsSelectors.serviceChannelConnectionTypes.getEntities,
  connectionTypes => (
    connectionTypes.filter(cT => cT.get('code').toLowerCase() === 'commonforall')
      .first() || Map()).get('id') || ''
)

export const getConnectionType = createSelector(
  [getChannelProperty('connectionType'), getDefaultConnectionType],
  (connectionType, defaultConnectionType) => connectionType || defaultConnectionType
)

export const getId = createSelector(
  getChannel,
  entity => entity.get('id') || null
)

const getOpeningHoursProperty = property => createSelector(
  getChannelProperty('openingHours', Map()),
  hours => hours.get(property)
)

const getNormalHours = createSelector(
  [getOpeningHoursProperty('normalOpeningHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => BaseSelectors.getEntitiesForIds(entities, hours, List())
    .map(oh => oh.update('dailyOpeningHours', days => days.filter(x => x)))
)

const getSpecialHours = createSelector(
  [getOpeningHoursProperty('specialOpeningHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => BaseSelectors.getEntitiesForIds(entities, hours, List())
)

const getExceptionalHours = createSelector(
  [getOpeningHoursProperty('exceptionalOpeningHours'), EntitySelectors.openingHours.getEntities],
  (hours, entities) => BaseSelectors.getEntitiesForIds(entities, hours, List())
)

export const getOpeningHours = createSelector(
  [
    getNormalHours,
    getSpecialHours,
    getExceptionalHours
  ], (
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours
  ) => Map().mergeDeep({
    normalOpeningHours,
    specialOpeningHours,
    exceptionalOpeningHours
  })
)
