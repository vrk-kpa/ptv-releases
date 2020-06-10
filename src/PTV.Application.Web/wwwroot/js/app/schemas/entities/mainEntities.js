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

// schemas
import EntitySchemas from './entities'
import { CodeListSchemas } from '../codeLists'
import { FintoSchemas } from '../finto'
import { TypeSchemas } from '../types'
import { addressCharactersEnum } from 'enums'
import { IntlSchemas } from 'Intl/Schemas'
const { translation } = IntlSchemas

const previousInfo = new schema.Entity('previousInfos', {}, {
  idAttribute: entity => entity.unificRootId,
  processStrategy: (value, parent, key) => {
    // console.log(value, parent, key)
    parent.previousInfo = value
    const { lastModifiedId,
      lastPublishedId,
      modifiedOfLastModified,
      modifiedOfLastPublished,
      lastPublishedLanguages
    } = value
    // console.log(parent)
    return {
      lastModifiedId,
      lastPublishedId,
      modifiedOfLastModified,
      modifiedOfLastPublished,
      lastPublishedLanguages,
      versions: {
        [value.id] : value
      }
    }
  }
})

const lastModifiedInfoSchema = new schema.Entity('previousInfos', {}, {
  idAttribute: entity => entity.unificRootId,
  processStrategy: (value) => {
    return {
      lastModifiedId : value.lastModifiedId
    }
  }
})

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
  [addressCharactersEnum.POSTALADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  [addressCharactersEnum.VISITINGADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  [addressCharactersEnum.DELIVERYADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  accessibilityClassifications: new schema.Values(EntitySchemas.ACCESSIBILITY_CLASSIFICATION),
  previousInfo
  // languagesAvailabilities: new schema.Array(EntitySchemas.LANGUAGES_AVAILABILITY)
})

const generalDescription = new schema.Entity('generalDescriptions', {
  ontologyTerms: FintoSchemas.ONTOLOGY_TERM_ARRAY,
  serviceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  lifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  industrialClasses: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
  connections: EntitySchemas.CONNECTION_ARRAY,
  previousInfo
  // languagesAvailabilities: new schema.Array(EntitySchemas.LANGUAGES_AVAILABILITY)
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
  serviceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  lifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  industrialClasses: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
  connections: EntitySchemas.CONNECTION_ARRAY,
  serviceCollections: EntitySchemas.CONNECTION_ARRAY,
  // laws: EntitySchemas.LAW_ARRAY
  previousInfo
  // languagesAvailabilities: new schema.Array(EntitySchemas.LANGUAGES_AVAILABILITY)
})

const serviceCollection = new schema.Entity('serviceCollections', {
  connections: EntitySchemas.CONNECTION_ARRAY,
  previousInfo
  // languagesAvailabilities: new schema.Array(EntitySchemas.LANGUAGES_AVAILABILITY)
})

const translationOrder = new schema.Entity('translationOrders')
const translationOrderStates = new schema.Entity('translationOrderStates', {
  translationOrder: translationOrder
})
const translationSchema = new schema.Entity('translations', {
  translationOrderStates: new schema.Array(translationOrderStates)
})

const organization = new schema.Entity('organizations', {
  municipality: CodeListSchemas.MUNICIPALITY,
  electronicInvoicingAddresses: EntitySchemas.ELECTRONIC_INVOICING_ADDRESS_ARRAY,
  emails: new schema.Values(EntitySchemas.EMAIL_ARRAY),
  phoneNumbers: new schema.Values(EntitySchemas.PHONE_NUMBER_ARRAY),
  webPages: new schema.Values(EntitySchemas.WEB_PAGE_ARRAY),
  business: EntitySchemas.BUSINESS,
  [addressCharactersEnum.POSTALADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  [addressCharactersEnum.VISITINGADDRESSES]: EntitySchemas.ADDRESS_ARRAY,
  translation,
  previousInfo
  // languagesAvailabilities: new schema.Array(EntitySchemas.LANGUAGES_AVAILABILITY)
}, {
  processStrategy: (entity, parent, key) => {
    const { name, ...rest } = entity
    if (typeof name === 'string') {
      return {
        ...rest,
        displayName: name
      }
    }
    return entity
  }
})

organization.define({
  children: new schema.Array(organization),
  filteredChildren: new schema.Array(organization)
})

const entityOperation = new schema.Entity('entityOperations', {
  subOperations: new schema.Array(new schema.Entity('entityOperations')),
  templateOrganization: new schema.Values(EntitySchemas.ORGANIZATION)
})

const createEnumSchema = (schemaDef, schemaAttribute = 'allowed') => new schema.Array(new schema.Union({
  true: schemaDef,
  false: schemaDef
}, schemaAttribute))

const createEnumSchema2 = (schemaDef, schemaAttribute = 'access') => new schema.Array(new schema.Union({
  0: schemaDef,
  1: schemaDef,
  2: schemaDef
}, schemaAttribute))

const enumDefinitionSchema = new schema.Entity('enums', {
  areaInformationTypes: createEnumSchema(TypeSchemas.AREA_INFORMATION_TYPE),
  areaTypes: createEnumSchema(TypeSchemas.AREA_TYPE),
  astiTypes: createEnumSchema(TypeSchemas.ASTI_TYPE),
  keywords: createEnumSchema(TypeSchemas.KEYWORD),
  businessRegions: createEnumSchema(CodeListSchemas.BUSINESS_REGION, value => !value.invalid),
  chargeTypes: createEnumSchema(TypeSchemas.CHARGE_TYPE),
  serviceChannelConnectionTypes: createEnumSchema(TypeSchemas.SERVICE_CHANNEL_CONNECTION_TYPE),
  dialCodes: CodeListSchemas.DIAL_CODE_ARRAY,
  entityOperations: EntitySchemas.ENTITY_OPERATION_ARRAY,
  extraTypes: createEnumSchema(TypeSchemas.EXTRA_TYPE),
  hospitalRegions: createEnumSchema(CodeListSchemas.HOSPITAL_REGION, value => !value.invalid),
  industrialClasses: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
  digitalAuthorizations: FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY,
  phoneNumberTypes: createEnumSchema(TypeSchemas.PHONE_NUMBER_TYPE),
  channelTypes: createEnumSchema(TypeSchemas.CHANNEL_TYPE),
  languages: CodeListSchemas.LANGUAGE_ARRAY,
  translationCompanies: CodeListSchemas.TRANSLATION_COMPANY_ARRAY,
  translationLanguages: CodeListSchemas.LANGUAGE_ARRAY,
  translationOrderLanguages: CodeListSchemas.LANGUAGE_ARRAY,
  laws: EntitySchemas.LAW_ARRAY,
  lifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  municipalities: CodeListSchemas.MUNICIPALITY_ARRAY,
  organizations: new schema.Array(organization),
  provinces: createEnumSchema(CodeListSchemas.PROVINCE, value => !value.invalid),
  publishingStatuses: createEnumSchema(TypeSchemas.PUBLISHING_STATUS),
  serviceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  printableFormUrlTypes: createEnumSchema(TypeSchemas.PRINTABLE_FORM_TYPE),
  serviceTypes: createEnumSchema(TypeSchemas.SERVICE_TYPE),
  generalDescriptionTypes: createEnumSchema2(TypeSchemas.GENERAL_DESCRIPTION_TYPE),
  fundingTypes: createEnumSchema(TypeSchemas.FUNDING_TYPE),
  targetGroups: FintoSchemas.TARGET_GROUP_ARRAY,
  topLifeEvents: FintoSchemas.LIFE_EVENT_ARRAY,
  provisionTypes: TypeSchemas.PROVISION_TYPE_ARRAY,
  topServiceClasses: FintoSchemas.SERVICE_CLASS_ARRAY,
  topTargetGroups: FintoSchemas.TARGET_GROUP_ARRAY,
  translationStateTypes: createEnumSchema(TypeSchemas.TRANSLATION_STATE_TYPE),
  topDigitalAuthorizations: FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY,
  organizationTypes: TypeSchemas.ORGANIZATION_TYPE_ARRAY,
  organizationAreaInformations: EntitySchemas.ORGANIZATION_AREA_INFORMATION_ARRAY,
  userAccessRightsGroups: TypeSchemas.USER_ACCESS_RIGHTS_GROUP_TYPE_ARRAY,
  userOrganizations: new schema.Array(organization),
  organizationRoles: EntitySchemas.ORGANIZATION_ROLE_ARRAY,
  serverConstants: TypeSchemas.SERVER_CONSTANTS_ARRAY,
  accessibilityClassificationLevelTypes: TypeSchemas.ACCESSIBILITY_CLASSIFICATION_LEVEL_TYPE_ARRAY,
  wcagLevelTypes: TypeSchemas.WCAG_LEVEL_TYPE_ARRAY,
  holidays: createEnumSchema(TypeSchemas.HOLIDAYS),
  holidayDates: TypeSchemas.HOLIDAY_DATES_ARRAY,
  serviceNumbers: TypeSchemas.SERVICE_NUMBERS_ARRAY
}, {
  idAttribute: enums => 'enums'
})

const electronicChannelForm = new schema.Object({
  basicInfo: channel,
  openingHours: channel,
  // enums
  enumCollection: enumDefinitionSchema
})

const searchResult = new schema.Union({
  services: service,
  channels: channel,
  generalDescriptions: generalDescription,
  organizations: organization,
  serviceCollections: serviceCollection
}, value => {
  switch (value.entityType.toLowerCase()) {
    case 'service': return 'services'
    case 'channel': return 'channels'
    case 'organization': return 'organizations'
    case 'servicecollection': return 'serviceCollections'
    case 'generaldescription': return 'generalDescriptions'
  }
})

const getSearch = searchSchema => new schema.Object({
  data: new schema.Array(searchSchema),
  enumCollection: enumDefinitionSchema
})

const translationOrderResult = new schema.Object({
  services: new schema.Array(service),
  channels: new schema.Array(channel),
  generalDescriptions: new schema.Array(generalDescription),
  translations: new schema.Array(translationSchema)
})

const addEnumsDefinition = schemaToInject =>
  schemaToInject.define({
    enumCollection: enumDefinitionSchema
  })

addEnumsDefinition(organization)
addEnumsDefinition(channel)
addEnumsDefinition(service)
addEnumsDefinition(serviceCollection)

export default {
  ENUM: enumDefinitionSchema,
  ENUM_COLLECTION: new schema.Object({ enumCollection: enumDefinitionSchema }),
  CHANNEL: channel,
  CHANNEL_ARRAY: new schema.Array(channel),
  SERVICE: service,
  SERVICE_ARRAY: new schema.Array(service),
  SERVICECOLLECTION: serviceCollection,
  SERVICECOLLECTION_ARRAY: new schema.Array(serviceCollection),
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
  TRANSLATION: translationSchema,
  TRANSLATION_ARRAY: new schema.Array(translationSchema),
  SEARCH: searchResult,
  GET_SEARCH: getSearch,
  TRANSLATION_ORDER_RESULT: translationOrderResult,
  LAST_MODIFIED_INFO: lastModifiedInfoSchema,
  LAST_MODIFIED_INFO_ARRAY: new schema.Array(lastModifiedInfoSchema),
  ENTITY_OPERATION: entityOperation,
  ENTITY_OPERATION_ARRAY: new schema.Array(entityOperation)
}
