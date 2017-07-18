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
import { IntlSchemas } from '../../../Intl/Schemas'

const publishingStatusSchema = new Schema('publishingStatuses',
  { idAttribute: publishingStatus => publishingStatus.id })
const chargeTypeSchema = new Schema('chargeTypes', { idAttribute: chargeType => chargeType.id })
const languageSchema = new Schema('languages', { idAttribute: language => language.id })
const translationLanguageSchema = new Schema('translationLanguages', { idAttribute: language => language.id })
const municipalitySchema = new Schema('municipalities', { idAttribute: municipality => municipality.id })
const channelTypeSchema = new Schema('channelTypes', { idAttribute: channelType => channelType.id })
const phoneNumberTypeSchema = new Schema('phoneNumberTypes', { idAttribute: phoneType => phoneType.id })
const webPageTypeSchema = new Schema('webPageTypes', { idAttribute: webPageType => webPageType.id })
const printableFormUrlTypeSchema = new Schema('printableFormUrlTypes', { idAttribute: type => type.id })
const webPageSchema = new Schema('webPages', { idAttribute: webPage => webPage.id })
const voucherSchema = new Schema('serviceVouchers', { idAttribute: voucher => voucher.id })
const phoneNumberSchema = new Schema('phoneNumbers', { idAttribute: phoneNumber => phoneNumber.id })
const emailSchema = new Schema('emails', { idAttribute: email => email.id })
const coordinateSchema = new Schema('coordinates', { idAttribute: coordinate => coordinate.id })
const addressSchema = new Schema('addresses', { idAttribute: address => address.id, meta: { localizable: true } })
const postalCodeSchema = new Schema('postalCodes', { idAttribute: postalCode => postalCode.id })
const serviceHourTypeSchema = new Schema('serviceHourType', { idAttribute: serviceHourType => serviceHourType.id })
const keyWordSchema = new Schema('keyWords', { idAttribute: keyWord => keyWord.id, meta: { localizable: true } })
const businessSchema = new Schema('business', { idAttribute: business => business.id })
const searchSchema = new Schema('searches', { idAttribute: search => search.id })
const lawSchema = new Schema('laws', { idAttribute: law => law.id })
const dialCodeSchema = new Schema('dialCodes', { idAttribute: dialCode => dialCode.id })
const areaInformationTypeSchema = new Schema('areaInformationTypes',
 { idAttribute: areaInformationType => areaInformationType.id })
const areaTypeSchema = new Schema('areaTypes', { idAttribute: areaType => areaType.id })
const provinceSchema = new Schema('provincies', { idAttribute: province => province.id })
const hospitalRegionSchema = new Schema('hospitalRegions', { idAttribute: hospitalRegion => hospitalRegion.id })
const businessRegionSchema = new Schema('businessRegions', { idAttribute: businessRegion => businessRegion.id })

const serviceClassSchema = new Schema('serviceClasses',
  { idAttribute: serviceClass => serviceClass.id })
const filteredServiceClassSchema = new Schema('filteredServiceClasses',
  { idAttribute: serviceClass => serviceClass.id })
const industrialClassSchema = new Schema('industrialClasses',
  { idAttribute: industrialClass => industrialClass.id })
const filteredIndustrialClassSchema = new Schema('filteredIndustrialClasses',
  { idAttribute: industrialClass => industrialClass.id })
const lifeEventsSchema = new Schema('lifeEvents', { idAttribute: lifeEvent => lifeEvent.id })
const filteredLifeEventsSchema = new Schema('filteredLifeEvents', { idAttribute: lifeEvent => lifeEvent.id })
const targetGroupsSchema = new Schema('targetGroups', { idAttribute: targetGroup => targetGroup.id })
const ontologyTermsSchema = new Schema('ontologyTerms', { idAttribute: ontologyTerm => ontologyTerm.id })
const annotationOntologyTermsSchema = new Schema('annotationOntologyTerms',
  { idAttribute: ontologyTerm => ontologyTerm.id })
const digitalAuthorizationSchema = new Schema('digitalAuthorizations',
  { idAttribute: digitalAuthorization => digitalAuthorization.id })
const filteredDigitalAuthorizationSchema = new Schema('filteredDigitalAuthorizations',
  { idAttribute: digitalAuthorization => digitalAuthorization.id })

const connectionSchema = new Schema('connections', { idAttribute: connection => connection.id })
const serviceChannelConnectionTypeSchema = new Schema('serviceChannelConnectionTypes',
  { idAttribute: serviceChannelConnectionType => serviceChannelConnectionType.id })
const serviceTypeSchema = new Schema('serviceTypes', { idAttribute: serviceType => serviceType.id });
const securitySchema = new Schema('securityInfo', { idAttribute: security => security.id })
const enumDefinitionSchema = new Schema('enums', { idAttribute: enums => 'enums' })
const fundingTypeSchema = new Schema('fundingTypes', { idAttribute: fundingType => fundingType.id })

export const defineChildrenSchema = (schema) => {
  schema.define({
    children: arrayOf(schema),
    translation: IntlSchemas.TRANSLATED_ITEM
  })
}

defineChildrenSchema(industrialClassSchema)
defineChildrenSchema(filteredIndustrialClassSchema)

defineChildrenSchema(serviceClassSchema)
defineChildrenSchema(filteredServiceClassSchema)

defineChildrenSchema(lifeEventsSchema)
defineChildrenSchema(filteredLifeEventsSchema)

defineChildrenSchema(targetGroupsSchema)
defineChildrenSchema(ontologyTermsSchema)
defineChildrenSchema(annotationOntologyTermsSchema)

defineChildrenSchema(digitalAuthorizationSchema)
defineChildrenSchema(filteredDigitalAuthorizationSchema)

addressSchema.define({
  postalCode: postalCodeSchema,
  coordinates: arrayOf(coordinateSchema)
})

phoneNumberSchema.define({
  prefixNumber: dialCodeSchema
})

postalCodeSchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

phoneNumberSchema.define({
  prefixNumber: dialCodeSchema
})

postalCodeSchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

municipalitySchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

provinceSchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

hospitalRegionSchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

businessRegionSchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

dialCodeSchema.define({
  translation: IntlSchemas.TRANSLATED_ITEM
})

searchSchema.define({
  ontologyWord: ontologyTermsSchema
})

enumDefinitionSchema.define({
  topServiceClasses: arrayOf(serviceClassSchema),
  topTargetGroups: arrayOf(targetGroupsSchema),
  topLifeEvents: arrayOf(lifeEventsSchema),
  serviceClasses: arrayOf(serviceClassSchema),
  targetGroups: arrayOf(targetGroupsSchema),
  lifeEvents: arrayOf(lifeEventsSchema),
  industrialClasses: arrayOf(industrialClassSchema),
  keyWords: arrayOf(keyWordSchema),
  laws: arrayOf(lawSchema),
  serviceTypes: arrayOf(serviceTypeSchema),
  chargeTypes: arrayOf(chargeTypeSchema),
  languages: arrayOf(languageSchema),
  fundingTypes: arrayOf(fundingTypeSchema)
})

export const CommonSchemas = {
  LANGUAGE: languageSchema,
  LANGUAGE_ARRAY: arrayOf(languageSchema),
  TRANSLATED_LANGUAGE: translationLanguageSchema,
  TRANSLATED_LANGUAGE_ARRAY: arrayOf(translationLanguageSchema),
  PUBLISHING_STATUS: publishingStatusSchema,
  PUBLISHING_STATUS_ARRAY: arrayOf(publishingStatusSchema),
  CHARGE_TYPE:chargeTypeSchema,
  CHARGE_TYPE_ARRAY: arrayOf(chargeTypeSchema),
  AREA_INFORMATION_TYPE: areaInformationTypeSchema,
  AREA_INFORMATION_TYPE_ARRAY: arrayOf(areaInformationTypeSchema),
  AREA_TYPE: areaTypeSchema,
  AREA_TYPE_ARRAY: arrayOf(areaTypeSchema),
  SERVICE_CHANNEL_CONNECTION_TYPE: serviceChannelConnectionTypeSchema,
  SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY: arrayOf(serviceChannelConnectionTypeSchema),
  PHONE_NUMBER_TYPE: phoneNumberTypeSchema,
  PHONE_NUMBER_TYPE_ARRAY: arrayOf(phoneNumberTypeSchema),
  MUNICIPALITY: municipalitySchema,
  MUNICIPALITY_ARRAY: arrayOf(municipalitySchema),
  PROVINCE: provinceSchema,
  PROVINCE_ARRAY: arrayOf(provinceSchema),
  HOSPITAL_REGION: hospitalRegionSchema,
  HOSPITAL_REGION_ARRAY: arrayOf(hospitalRegionSchema),
  BUSINESS_REGION: businessRegionSchema,
  BUSINESS_REGION_ARRAY: arrayOf(businessRegionSchema),
  CHANNEL_TYPE: channelTypeSchema,
  CHANNEL_TYPE_ARRAY: arrayOf(channelTypeSchema),
  WEB_PAGE_TYPE: webPageTypeSchema,
  WEB_PAGE_TYPE_ARRAY: arrayOf(webPageTypeSchema),
  PRINTABLE_FORM_TYPE: printableFormUrlTypeSchema,
  PRINTABLE_FORM_TYPE_ARRAY: arrayOf(printableFormUrlTypeSchema),
  WEB_PAGE: webPageSchema,
  WEB_PAGE_ARRAY: arrayOf(webPageSchema),
  SERVICE_VOUCHER: voucherSchema,
  SERVICE_VOUCHER_ARRAY: arrayOf(voucherSchema),
  PHONE_NUMBER: phoneNumberSchema,
  PHONE_NUMBER_ARRAY: arrayOf(phoneNumberSchema),
  EMAIL: emailSchema,
  EMAIL_ARRAY: arrayOf(emailSchema),
  COORDINATE: coordinateSchema,
  COORDINATE_ARRAY: arrayOf(coordinateSchema),
  ADDRESS: addressSchema,
  ADDRESS_ARRAY: arrayOf(addressSchema),
  SERVICE_HOUR_TYPE: serviceHourTypeSchema,
  SERVICE_HOUR_TYPE_ARRAY: arrayOf(serviceHourTypeSchema),
  KEYWORD: keyWordSchema,
  KEYWORD_ARRAY: arrayOf(keyWordSchema),
  BUSINESS: businessSchema,
  BUSINESS_ARRAY: arrayOf(businessSchema),
  SEARCH: searchSchema,
  SERVICE_CLASS: serviceClassSchema,
  SERVICE_CLASS_ARRAY: arrayOf(serviceClassSchema),
  CONNECTION: connectionSchema,
  CONNECTION_ARRAY: arrayOf(connectionSchema),
  LAW: lawSchema,
  LAW_ARRAY: arrayOf(lawSchema),
  DIAL_CODE: dialCodeSchema,
  DIAL_CODE_ARRAY: arrayOf(dialCodeSchema),
    // finto
  FILTERED_SERVICE_CLASS_ARRAY: arrayOf(filteredServiceClassSchema),
  INDUSTRIAL_CLASS: industrialClassSchema,
  INDUSTRIAL_CLASS_ARRAY: arrayOf(industrialClassSchema),
  FILTERED_INDUSTRIAL_CLASS_ARRAY: arrayOf(filteredIndustrialClassSchema),
  LIFE_EVENT: lifeEventsSchema,
  LIFE_EVENT_ARRAY: arrayOf(lifeEventsSchema),
  FILTERED_LIFE_EVENTS_ARRAY: arrayOf(filteredLifeEventsSchema),
  TARGET_GROUP: targetGroupsSchema,
  TARGET_GROUP_ARRAY: arrayOf(targetGroupsSchema),
  ONTOLOGY_TERM: ontologyTermsSchema,
  ONTOLOGY_TERM_ARRAY: arrayOf(ontologyTermsSchema),
  ANNOTATION_ONTOLOGY_TERM: annotationOntologyTermsSchema,
  ANNOTATION_ONTOLOGY_TERM_ARRAY: arrayOf(annotationOntologyTermsSchema),
  DIGITAL_AUTHORIZATION: digitalAuthorizationSchema,
  DIGITAL_AUTHORIZATION_ARRAY: arrayOf(digitalAuthorizationSchema),
  FILTERED_DIGITAL_AUTHORIZATION_ARRAY: arrayOf(filteredDigitalAuthorizationSchema),

  ENUMS: enumDefinitionSchema,

  SECURITY: securitySchema,
  
  FUNDING_TYPE: fundingTypeSchema,
  FUNDING_TYPE_ARRAY: arrayOf(fundingTypeSchema)
}
