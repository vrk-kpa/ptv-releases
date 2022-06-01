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
import { schema } from 'normalizr'
import { defineChildrenSchema } from 'schemas/finto'
import { IntlSchemas } from 'Intl/Schemas'
const { translation } = IntlSchemas

const publishingStatus = new schema.Entity('publishingStatuses', { translation })
const chargeType = new schema.Entity('chargeTypes', { translation })
const channelType = new schema.Entity('channelTypes', { translation })
const phoneNumberType = new schema.Entity('phoneNumberTypes', { translation })
const webPageType = new schema.Entity('webPageTypes', { translation })
const printableFormUrlType = new schema.Entity('printableFormUrlTypes', { translation })
const serviceHourType = new schema.Entity('serviceHourType', { translation })
const areaInformationType = new schema.Entity('areaInformationTypes', { translation })
const areaType = new schema.Entity('areaTypes', { translation })
const keyword = new schema.Entity('keywords', { translation })
const serviceChannelConnectionType = new schema.Entity('serviceChannelConnectionTypes', { translation })
const serviceType = new schema.Entity('serviceTypes', { translation })
const generalDescriptionType = new schema.Entity('generalDescriptionTypes', { translation })
const userAccessRightsGroupType = new schema.Entity('userAccessRightsGroups', { translation })
const fundingType = new schema.Entity('fundingTypes', { translation })
const provisionType = new schema.Entity('provisionTypes', { translation })
const organizationType = new schema.Entity('organizationTypes', { translation })
const astiType = new schema.Entity('astiTypes', { translation })
const translationStateType = new schema.Entity('translationStateTypes', { translation })
const extraType = new schema.Entity('extraTypes', { translation })
const serverConstant = new schema.Entity('serverConstants', {}, { idAttribute: e => e.code })
const accessibilityClassificationLevelType = new schema.Entity('accessibilityClassificationLevelTypes', { translation })
const wcagLevelType = new schema.Entity('wcagLevelTypes', { translation })
const holidays = new schema.Entity('holidays', { translation })
const holidayDates = new schema.Entity('holidayDates', {}, { idAttribute: e => e.id })
const serviceNumbers = new schema.Entity('serviceNumbers', {}, { idAttribute: e => e.number })
const voucherType = new schema.Entity('voucherTypes', { translation })
defineChildrenSchema(organizationType)

export const TypeSchemas = {
  PUBLISHING_STATUS: publishingStatus,
  PUBLISHING_STATUS_ARRAY: new schema.Array(publishingStatus),
  CHARGE_TYPE:chargeType,
  CHARGE_TYPE_ARRAY: new schema.Array(chargeType),
  AREA_INFORMATION_TYPE: areaInformationType,
  AREA_INFORMATION_TYPE_ARRAY: new schema.Array(areaInformationType),
  AREA_TYPE: areaType,
  AREA_TYPE_ARRAY: new schema.Array(areaType),
  KEYWORD_TYPE: keyword,
  KEYWORD_ARRAY: new schema.Array(keyword),
  SERVICE_CHANNEL_CONNECTION_TYPE: serviceChannelConnectionType,
  SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY: new schema.Array(serviceChannelConnectionType),
  PHONE_NUMBER_TYPE: phoneNumberType,
  PHONE_NUMBER_TYPE_ARRAY: new schema.Array(phoneNumberType),
  CHANNEL_TYPE: channelType,
  CHANNEL_TYPE_ARRAY: new schema.Array(channelType),
  WEB_PAGE_TYPE: webPageType,
  WEB_PAGE_TYPE_ARRAY: new schema.Array(webPageType),
  PRINTABLE_FORM_TYPE: printableFormUrlType,
  PRINTABLE_FORM_TYPE_ARRAY: new schema.Array(printableFormUrlType),
  SERVICE_HOUR_TYPE: serviceHourType,
  SERVICE_HOUR_TYPE_ARRAY: new schema.Array(serviceHourType),
  SERVICE_TYPE: serviceType,
  SERVICE_TYPE_ARRAY: new schema.Array(serviceType),
  GENERAL_DESCRIPTION_TYPE: generalDescriptionType,
  GENERAL_DESCRIPTION_TYPE_ARRAY: new schema.Array(generalDescriptionType),
  USER_ACCESS_RIGHTS_GROUP_TYPE: userAccessRightsGroupType,
  USER_ACCESS_RIGHTS_GROUP_TYPE_ARRAY: new schema.Array(userAccessRightsGroupType),
  PROVISION_TYPE: provisionType,
  PROVISION_TYPE_ARRAY: new schema.Array(provisionType),
  FUNDING_TYPE: fundingType,
  FUNDING_TYPE_ARRAY: new schema.Array(fundingType),
  ORGANIZATION_TYPE: organizationType,
  ORGANIZATION_TYPE_ARRAY: new schema.Array(organizationType),
  ASTI_TYPE: astiType,
  ASTI_TYPE_ARRAY: new schema.Array(astiType),
  TRANSLATION_STATE_TYPE: translationStateType,
  TRANSLATION_STATE_TYPE_ARRAY:  new schema.Array(translationStateType),
  EXTRA_TYPE: extraType,
  EXTRA_TYPE_ARRAY:  new schema.Array(extraType),
  SERVER_CONSTANTS: serverConstant,
  SERVER_CONSTANTS_ARRAY:  new schema.Array(serverConstant),
  ACCESSIBILITY_CLASSIFICATION_LEVEL_TYPE: accessibilityClassificationLevelType,
  ACCESSIBILITY_CLASSIFICATION_LEVEL_TYPE_ARRAY: new schema.Array(accessibilityClassificationLevelType),
  WCAG_LEVEL_TYPE: wcagLevelType,
  WCAG_LEVEL_TYPE_ARRAY: new schema.Array(wcagLevelType),
  HOLIDAYS: holidays,
  HOLIDAYS_TYPE_ARRAY: new schema.Array(holidays),
  HOLIDAY_DATES: holidayDates,
  HOLIDAY_DATES_ARRAY: new schema.Array(holidayDates),
  SERVICE_NUMBERS: holidayDates,
  SERVICE_NUMBERS_ARRAY: new schema.Array(serviceNumbers),
  VOUCHER_TYPE: voucherType,
  VOUCHER_TYPE_ARRAY: new schema.Array(voucherType)
}
