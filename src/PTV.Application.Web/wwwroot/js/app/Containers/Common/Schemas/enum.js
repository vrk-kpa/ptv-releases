import { schema } from 'normalizr'
import { CommonSchemas } from './index'
import { OrganizationSchemas } from '../../Manage/Organizations/Organization/Schemas'
import { CommonOrganizationSchemas } from '../../Manage/Organizations/Common/Schemas'
import { CommonServiceSchemas } from '../../Services/Common/Schemas'

export const EnumDefinitionSchema = new schema.Entity('enums', {
  areaInformationTypes: CommonSchemas.AREA_INFORMATION_TYPE_ARRAY,
  areaTypes: CommonSchemas.AREA_TYPE_ARRAY,
  availableOrganizations: OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL,
  businessRegions: CommonSchemas.BUSINESS_REGION_ARRAY,
  channelTypes: CommonSchemas.CHANNEL_TYPE_ARRAY,
  chargeTypes: CommonSchemas.CHARGE_TYPE_ARRAY,
  coverageTypes: CommonServiceSchemas.COVERAGE_TYPE_ARRAY,
  dialCodes: CommonSchemas.DIAL_CODE_ARRAY,
  digitalAuthorizations: CommonSchemas.DIGITAL_AUTHORIZATION_ARRAY,
  fundingTypes: CommonSchemas.FUNDING_TYPE_ARRAY,
  hospitalRegions: CommonSchemas.HOSPITAL_REGION_ARRAY,
  industrialClasses: CommonSchemas.INDUSTRIAL_CLASS_ARRAY,
  keyWords: CommonServiceSchemas.KEY_WORD_ARRAY,
  languages: CommonSchemas.LANGUAGE_ARRAY,
  laws: CommonSchemas.LAW_ARRAY,
  lifeEvents: CommonSchemas.LIFE_EVENT_ARRAY,
  municipalities: CommonSchemas.MUNICIPALITY_ARRAY,
  organizations: OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL,
  organizationTypes: CommonOrganizationSchemas.ORGANIZATION_TYPE_ARRAY,
  phoneNumberTypes: CommonSchemas.PHONE_NUMBER_TYPE_ARRAY,
  printableFormUrlTypes: CommonSchemas.PRINTABLE_FORM_TYPE_ARRAY,
  provincies: CommonSchemas.PROVINCE_ARRAY,
  provisionTypes: CommonSchemas.PROVISION_TYPE_ARRAY,
  publishingStatuses: CommonSchemas.PUBLISHING_STATUS_ARRAY,
  serviceChannelConnectionTypes: CommonSchemas.SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY,
  serviceClasses: CommonSchemas.SERVICE_CLASS_ARRAY,
  serviceTypes: CommonServiceSchemas.SERVICE_TYPE_ARRAY,
  targetGroups: CommonSchemas.TARGET_GROUP_ARRAY,
  topLifeEvents: CommonSchemas.LIFE_EVENT_ARRAY,
  topServiceClasses: CommonSchemas.SERVICE_CLASS_ARRAY,
  topTargetGroups: CommonSchemas.TARGET_GROUP_ARRAY,
  webPageTypes: CommonSchemas.WEB_PAGE_TYPE_ARRAY
},
{ idAttribute: enums => 'enums' })
