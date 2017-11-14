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
import { schema } from 'normalizr'

// schemas
import EntitySchemas from './entities'
import { CodeListSchemas } from '../codeLists'
import { FintoSchemas } from '../finto'
import { TypeSchemas } from '../types'
import { addressTypesEnum } from 'enums'
import SECURITY from './security'
import { IntlSchemas } from 'Intl/Schemas'
const { translation } = IntlSchemas
// import { TypeSchemas, FintoSchemas, CodeListSchemas, EntitySchemas } from 'schemas'

const channel = new schema.Entity('channels', {
  attachments: new schema.Values(EntitySchemas.WEB_PAGE_ARRAY),
  webPages: new schema.Values(EntitySchemas.WEB_PAGE_ARRAY),
  formFiles: new schema.Values(EntitySchemas.WEB_PAGE_ARRAY),
  phoneNumbers: new schema.Values(EntitySchemas.PHONE_NUMBER_ARRAY),
  faxNumbers: new schema.Values(EntitySchemas.PHONE_NUMBER_ARRAY),
  emails: new schema.Values(EntitySchemas.EMAIL_ARRAY),
  // services: new schema.Array(servicesSchema),
  connections: EntitySchemas.CONNECTION_ARRAY,
  openingHours: new schema.Values(EntitySchemas.OPENING_HOUR_ARRAY),
  // normalOpeningHours: EntitySchemas.OPENING_HOUR_ARRAY,
  // specialOpeningHours: EntitySchemas.OPENING_HOUR_ARRAY,
  // exceptionalOpeningHours: EntitySchemas.OPENING_HOUR_ARRAY,
  [addressTypesEnum.POSTALADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  [addressTypesEnum.VISITINGADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  deliveryAddress: EntitySchemas.ADDRESS,
  security: SECURITY
})

const generalDescription = new schema.Entity('generalDescriptions', {
  ontologyTerms: FintoSchemas.ONTOLOGY_TERM_ARRAY,
  serviceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  lifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  industrialClasses: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
  connections: EntitySchemas.CONNECTION_ARRAY,
  annotationOntologyTerms: FintoSchemas.ANNOTATION_ONTOLOGY_TERM_ARRAY
})

const currentIssuesSchema = new schema.Entity('instructions')
const serviceProducer = new schema.Entity('serviceProducers')
const serviceVoucher = new schema.Entity('serviceVouchers')

const service = new schema.Entity('services', {
  serviceProducers: new schema.Array(serviceProducer),
  serviceVouchers: new schema.Values(new schema.Array(serviceVoucher)),
  // attachedChannels: CommonChannels.CHANNEL_ARRAY,
  // newKeyWords: EntitySchemas.KEYWORD_ARRAY,
  keywords: new schema.Values(EntitySchemas.KEYWORD_ARRAY),
  generalDescriptionOutput: generalDescription,
  ontologyTerms: FintoSchemas.ONTOLOGY_TERM_ARRAY,
  annotationOntologyTerms: FintoSchemas.ANNOTATION_ONTOLOGY_TERM_ARRAY,
  serviceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  lifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  industrialClasses: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
  connections: EntitySchemas.CONNECTION_ARRAY,
  // laws: EntitySchemas.LAW_ARRAY,
  security: SECURITY
})

const organization = new schema.Entity('organizations', {
  children: new schema.Array(organization),
  municipality: CodeListSchemas.MUNICIPALITY,
  emails: new schema.Values(EntitySchemas.EMAIL_ARRAY),
  phoneNumbers: new schema.Values(EntitySchemas.PHONE_NUMBER_ARRAY),
  webPages: new schema.Values(EntitySchemas.WEB_PAGE_ARRAY),
  business: EntitySchemas.BUSINESS,
  [addressTypesEnum.POSTALADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  [addressTypesEnum.VISITINGADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  translation,
  security: SECURITY
})

const enumDefinitionSchema = new schema.Entity('enums', {
  areaInformationTypes: TypeSchemas.AREA_INFORMATION_TYPE_ARRAY,
  areaTypes: TypeSchemas.AREA_TYPE_ARRAY,
  keywords: TypeSchemas.KEYWORD_ARRAY,
  businessRegions: CodeListSchemas.BUSINESS_REGION_ARRAY,
  chargeTypes: TypeSchemas.CHARGE_TYPE_ARRAY,
  serviceChannelConnectionTypes: TypeSchemas.SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY,
  dialCodes: CodeListSchemas.DIAL_CODE_ARRAY,
  hospitalRegions: CodeListSchemas.HOSPITAL_REGION_ARRAY,
  industrialClasses: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
  digitalAuthorizations: FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY,
  phoneNumberTypes: TypeSchemas.PHONE_NUMBER_TYPE_ARRAY,
  channelTypes: TypeSchemas.CHANNEL_TYPE_ARRAY,
  languages: CodeListSchemas.LANGUAGE_ARRAY,
  translationLanguages: CodeListSchemas.LANGUAGE_ARRAY,
  laws: EntitySchemas.LAW_ARRAY,
  lifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  municipalities: CodeListSchemas.MUNICIPALITY_ARRAY,
  organizations: new schema.Array(organization),
  provinces: CodeListSchemas.PROVINCE_ARRAY,
  publishingStatuses: TypeSchemas.PUBLISHING_STATUS_ARRAY,
  serviceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  printableFormUrlTypes: TypeSchemas.PRINTABLE_FORM_TYPE_ARRAY,
  serviceTypes: TypeSchemas.SERVICE_TYPE_ARRAY,
  fundingTypes: TypeSchemas.FUNDING_TYPE_ARRAY,
  targetGroups: FintoSchemas.TARGET_GROUP_ARRAY,
  topLifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  provisionTypes: TypeSchemas.PROVISION_TYPE_ARRAY,
  topServiceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  topTargetGroups: FintoSchemas.TARGET_GROUP_ARRAY,
  topDigitalAuthorizations: FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY,
  organizationTypes: TypeSchemas.ORGANIZATION_TYPE_ARRAY,
  organizationAreaInformations: EntitySchemas.ORGANIZATION_AREA_INFORMATION_ARRAY,
  userOrganizations: new schema.Array(organization),
  organizationRoles: EntitySchemas.ORGANIZATION_ROLE_ARRAY
}, {
  idAttribute: enums => 'enums'
})

const electronicChannelForm = new schema.Object({
  basicInfo: channel,
  openingHours: channel,
  // enums
  enumCollection: enumDefinitionSchema
})

const searchResult = new schema.Object({
  services: new schema.Array(service),
  channels: new schema.Array(channel),
  generalDescriptions: new schema.Array(generalDescription),
  organizations: new schema.Array(organization)
})
const getSearch = searchSchema => new schema.Object({
  data: new schema.Array(searchSchema)
})

export default {
  ENUM: enumDefinitionSchema,
  ENUM_COLLECTION: new schema.Object({ enumCollection: enumDefinitionSchema }),
  CHANNEL: channel,
  CHANNEL_ARRAY: new schema.Array(channel),
  SERVICE: service,
  SERVICE_ARRAY: new schema.Array(service),
  ELECTRONIC_FORM: electronicChannelForm,
  ELECTRONIC_FORM_ARRAY: new schema.Array(electronicChannelForm),
  SERVICE_FORM: service,
  SERVICE_FORM_ARRAY: new schema.Array(service),
  GENERAL_DESCRIPTION: generalDescription,
  GENERAL_DESCRIPTION_ARRAY: new schema.Array(generalDescription),
  ORGANIZATION: organization,
  ORGANIZATION_ARRAY: new schema.Array(organization),
  CURRENT_ISSUE: currentIssuesSchema,
  CURRENT_ISSUE_ARRAY: new schema.Array(currentIssuesSchema),
  SEARCH: searchResult,
  GET_SEARCH: getSearch
}
