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
import { CodeListSchemas } from '../codeLists'
import { FintoSchemas } from '../finto'

const brokenLink = new schema.Entity('brokenLinks')
const webPage = new schema.Entity('webPages')
const phoneNumber = new schema.Entity('phoneNumbers', {
  dialCode: CodeListSchemas.DIAL_CODE
})
const email = new schema.Entity('emails')
const coordinate = new schema.Entity('coordinates')
const accessibilityRegister = new schema.Entity('accessibilityRegisters', {}, { idAttribute: 'entranceId' })
const address = new schema.Entity('addresses', {
  postalCode: CodeListSchemas.POSTAL_CODE,
  country: CodeListSchemas.COUNTRY,
  street: CodeListSchemas.STREET,
  streetNumberRange: CodeListSchemas.STREET_NUMBER,
  accessibilityRegister
})
const keyword = new schema.Entity('keywords')
const business = new schema.Entity('business')
const law = new schema.Entity('laws')
const electronicInvoicingAddress = new schema.Entity('electronicInvoicingAddresses')
const accessibilityClassification = new schema.Entity('accessibilityClassifications')
const sahaMapping = new schema.Entity('sahaMappings')

const openingHour = new schema.Entity('openingHours', {
})

const connection = new schema.Entity('connections', {
  digitalAuthorization: new schema.Object({
    digitalAuthorizations : FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY
  }),
  contactDetails: new schema.Object({
    emails: new schema.Values(new schema.Array(email)),
    phoneNumbers: new schema.Values(new schema.Array(phoneNumber)),
    faxNumbers: new schema.Values(new schema.Array(phoneNumber)),
    postalAddresses: new schema.Array(address),
    webPages: new schema.Values(new schema.Array(webPage))
  }),
  openingHours: new schema.Values(new schema.Array(openingHour))
},
{ idAttribute : 'connectionId' }
)

const organizationAreaInformation = new schema.Entity('organizationAreaInformations', {}, {
  idAttribute: entity => entity.organizationId
})

const organizationRole = new schema.Entity('organizationRoles', {}, {
  idAttribute: entity => entity.organizationId
})
const connectionOperation = new schema.Entity('connectionOperations')

const latestEntityInfo = new schema.Entity('latestEntityInfos', {}, {
  idAttribute: entity => entity.unificRootId
})

const languagesAvailability = new schema.Entity('languagesAvailabilities', {}, {
  idAttribute: (language, parent) => parent.id,
  mergeStrategy: (a, b) => console.log(a, b) || { ...a, ...b, languages: [...a.languages, ...b.languages] },
  processStrategy: language => ({
    [language.languageId]: language,
    languages: [language.languageId]
  })
})

export default {
  ACCESSIBILITY_CLASSIFICATION: accessibilityClassification,
  ACCESSIBILITY_CLASSIFICATION_ARRAY: new schema.Array(accessibilityClassification),
  ACCESSIBILITY_REGISTER: accessibilityRegister,
  ACCESSIBILITY_REGISTER_ARRAY: new schema.Array(accessibilityRegister),
  CONNECTION_OPERATION: connectionOperation,
  CONNECTION_OPERATION_ARRAY: new schema.Array(connectionOperation),
  WEB_PAGE: webPage,
  WEB_PAGE_ARRAY: new schema.Array(webPage),
  PHONE_NUMBER: phoneNumber,
  PHONE_NUMBER_ARRAY: new schema.Array(phoneNumber),
  ELECTRONIC_INVOICING_ADDRESS: electronicInvoicingAddress,
  ELECTRONIC_INVOICING_ADDRESS_ARRAY: new schema.Array(electronicInvoicingAddress),
  EMAIL: email,
  EMAIL_ARRAY: new schema.Array(email),
  COORDINATE: coordinate,
  COORDINATE_ARRAY: new schema.Array(coordinate),
  ADDRESS: address,
  ADDRESS_ARRAY: new schema.Array(address),
  KEYWORD: keyword,
  KEYWORD_ARRAY: new schema.Array(keyword),
  BUSINESS: business,
  BUSINESS_ARRAY: new schema.Array(business),
  CONNECTION: connection,
  CONNECTION_ARRAY: new schema.Array(connection),
  LAW: law,
  LAW_ARRAY: new schema.Array(law),
  OPENING_HOUR: openingHour,
  OPENING_HOUR_ARRAY: new schema.Array(openingHour),
  ORGANIZATION_AREA_INFORMATION: organizationAreaInformation,
  ORGANIZATION_AREA_INFORMATION_ARRAY: new schema.Array(organizationAreaInformation),
  ORGANIZATION_ROLE: organizationRole,
  ORGANIZATION_ROLE_ARRAY: new schema.Array(organizationRole),
  LATEST_ENTITY_INFO: latestEntityInfo,
  LANGUAGES_AVAILABILITY: languagesAvailability,
  SAHA_MAPPING: sahaMapping,
  SAHA_MAPPING_ARRAY: new schema.Array(sahaMapping),
  BROKEN_LINK: brokenLink,
  BROKEN_LINK_ARRAY: new schema.Array(brokenLink)
}
