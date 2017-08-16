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
import { Schema, arrayOf } from 'normalizr'

// schemas
import { CommonSchemas } from '../../../Common/Schemas'

// types
import { addressTypes } from '../../../Common/Helpers/types'

const channelsSchema = new Schema('channels', { idAttribute: channel => channel.id, meta: { localizable: true } })
const channelsSchema_v2 = new Schema('channels', { idAttribute: channel => channel.id })
const urlAttachmentSchema = new Schema('urlAttachments',
  { idAttribute: urlAttachment => urlAttachment.id, meta: { localizable: true } })
const openingHoursSchema = new Schema('openingHours', { idAttribute: openingHours => openingHours.id, meta: { localizable: true } })
const dailyOpeningHoursSchema = new Schema('dailyOpeningHours',
  { idAttribute: dailyOpeningHours => dailyOpeningHours.id })
const servicesSchema = new Schema('services', { idAttribute: service => service.id, meta: { localizable: true } })

channelsSchema.define({
  urlAttachments: arrayOf(urlAttachmentSchema),
  // webPage: CommonSchemas.WEB_PAGE,
  webPages: CommonSchemas.WEB_PAGE_ARRAY,
  phoneNumber: CommonSchemas.PHONE_NUMBER,
  phoneNumbers: CommonSchemas.PHONE_NUMBER_ARRAY,
  fax: CommonSchemas.PHONE_NUMBER,
  faxNumbers: CommonSchemas.PHONE_NUMBER_ARRAY,
  email: CommonSchemas.EMAIL,
  emails: CommonSchemas.EMAIL_ARRAY,
  services: arrayOf(servicesSchema),
  connections: CommonSchemas.CONNECTION_ARRAY,
  openingHoursNormal: arrayOf(openingHoursSchema),
  openingHoursSpecial: arrayOf(openingHoursSchema),
  openingHoursExceptional: arrayOf(openingHoursSchema),
  [addressTypes.POSTAL]: CommonSchemas.ADDRESS_ARRAY,
  [addressTypes.VISITING]: CommonSchemas.ADDRESS_ARRAY,
  [addressTypes.DELIVERY]: CommonSchemas.ADDRESS,
  security: CommonSchemas.SECURITY
})

openingHoursSchema.define({
  openingHours: arrayOf(openingHoursSchema),
  dailyOpeningHours: arrayOf(dailyOpeningHoursSchema)
})

export const CommonChannelsSchemas = {
  CHANNEL: channelsSchema,
  CHANNEL_ARRAY: arrayOf(channelsSchema),
  CHANNEL_V2: channelsSchema_v2,
  CHANNEL_ARRAY_V2: arrayOf(channelsSchema_v2),
  URL_ATTACHMENT: urlAttachmentSchema,
  URL_ATTACHMENT_ARRAY: arrayOf(urlAttachmentSchema),
  OPENING_HOUR: openingHoursSchema,
  OPENING_HOUR_ARRAY: arrayOf(openingHoursSchema)
}
