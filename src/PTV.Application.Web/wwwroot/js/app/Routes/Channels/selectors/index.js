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
import createCachedSelector from 're-reselect'
import { Map, List } from 'immutable'
import { EntitySelectors, BaseSelectors, EnumsSelectors } from 'selectors'
import { EditorState, ContentState, convertFromRaw } from 'draft-js'
import { createEntityPropertySelector, getOrganization } from 'selectors/common'
import { createFilteredField, getSelectedOrCopyEntity } from 'selectors/copyEntity'
import { getUserOrganization } from 'selectors/userInfo'
import { formChannelTypes } from 'enums'

export const getChannel = createSelector(
  EntitySelectors.getEntity,
  channel => channel
)

export const getChannelProperty = (propertyName, defaultValue = null) => createSelector(
  getChannel,
  channel => channel.get(propertyName) != null ? channel.get(propertyName) : defaultValue
)

export const getTemplateId = createSelector(
  BaseSelectors.getParameterFromProps('templateId'),
  templateId => templateId
)

export const getTemplateOrganizationId = createSelector(
  BaseSelectors.getParameterFromProps('templateId'),
  getSelectedOrCopyEntity,
  (templateId, entity) => templateId && entity.get('organizationId') || null
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
  (state, props) => EntitySelectors.getEntity(state, props).get('id') || ''
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
  (state, props) => EntitySelectors.getEntity(state, props).get('id') || ''
)

export const getDescription = createSelector(
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

export const getDeliveryAddresses = createSelector(
  [getChannelProperty('deliveryAddresses'), EntitySelectors.addresses.getEntities],
  (addresses, entities) => BaseSelectors.getEntitiesForIds(entities, addresses, List())
)

export const getEmails = createSelector(
  [getChannelProperty('emails'), EntitySelectors.emails.getEntities],
  (emails, entities) =>
    emails &&
    emails.map(x => BaseSelectors.getEntitiesForIds(entities, x, List())) ||
    Map()
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

export const getId = createSelector(
  getChannel,
  entity => entity.get('id') || null
)

export const getUnificRootId = createSelector(
  getChannel,
  entity => entity.get('unificRootId') || null
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

const descriptionFields = {
  shortDescription: { json: true, type: 'Summary' },
  description: { json: true, type: 'Description' }
}

const getDescriptions = (generalDescription) =>
  generalDescription
    .filter((x, key) => Object.keys(descriptionFields).includes(key))
    .reduce((descriptions, x, key) => {
      if (x) {
        const isJSON = descriptionFields[key].json
        return descriptions.concat(
          x.map((value, language) => ({
            language,
            type: descriptionFields[key].type,
            value: !isJSON
              ? value
              : value && value.getCurrentContent().getPlainText(' ')
          })).toArray()
        )
      }
      return descriptions
    }, [])

const getServiceChannelDescriptions = createSelector(
  BaseSelectors.getValuesByFormName,
  service => service && getDescriptions(service)
)

export const getAdditionalQualityCheckData = createSelector(
  BaseSelectors.getParameterFromProps('formName'),
  BaseSelectors.getFormValue('unificRootId'),
  BaseSelectors.getFormValue('alternativeId'),
  getServiceChannelDescriptions,
  (formName, id, alternativeId, serviceChannelDescriptions) => ({
    id,
    alternativeId,
    type: formChannelTypes[formName],
    serviceChannelDescriptions
  })
)
