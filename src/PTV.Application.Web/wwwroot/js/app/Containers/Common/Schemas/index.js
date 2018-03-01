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
import { IntlSchemas } from '../../../Intl/Schemas'

const publishingStatusSchema = new schema.Entity('publishingStatuses')
const chargeTypeSchema = new schema.Entity('chargeTypes')
const languageSchema = new schema.Entity('languages')
const translationLanguageSchema = new schema.Entity('translationLanguages')
const municipalitySchema = new schema.Entity('municipalities')
const channelTypeSchema = new schema.Entity('channelTypes')
const phoneNumberTypeSchema = new schema.Entity('phoneNumberTypes')
const webPageTypeSchema = new schema.Entity('webPageTypes')
const printableFormUrlTypeSchema = new schema.Entity('printableFormUrlTypes')
const webPageSchema = new schema.Entity('webPages')
const voucherSchema = new schema.Entity('serviceVouchers')
const phoneNumberSchema = new schema.Entity('phoneNumbers')
const emailSchema = new schema.Entity('emails')
const coordinateSchema = new schema.Entity('coordinates')
const addressSchema = new schema.Entity('addresses')
const postalCodeSchema = new schema.Entity('postalCodes')
const countrySchema = new schema.Entity('countries', { idAttribute: country => country.id })
const serviceHourTypeSchema = new schema.Entity('serviceHourType')
const keyWordSchema = new schema.Entity('keyWords')
const businessSchema = new schema.Entity('business')
const searchSchema = new schema.Entity('searches')
const lawSchema = new schema.Entity('laws')
const dialCodeSchema = new schema.Entity('dialCodes')
const areaInformationTypeSchema = new schema.Entity('areaInformationTypes')
const areaTypeSchema = new schema.Entity('areaTypes')
const provinceSchema = new schema.Entity('provincies')
const hospitalRegionSchema = new schema.Entity('hospitalRegions')
const businessRegionSchema = new schema.Entity('businessRegions')

const serviceClassSchema = new schema.Entity('serviceClasses')
const filteredServiceClassSchema = new schema.Entity('filteredServiceClasses')
const industrialClassSchema = new schema.Entity('industrialClasses')
const filteredIndustrialClassSchema = new schema.Entity('filteredIndustrialClasses')
const lifeEventsSchema = new schema.Entity('lifeEvents')
const filteredLifeEventsSchema = new schema.Entity('filteredLifeEvents')
const targetGroupsSchema = new schema.Entity('targetGroups')
const ontologyTermsSchema = new schema.Entity('ontologyTerms')
const annotationOntologyTermsSchema = new schema.Entity('annotationOntologyTerms')
const digitalAuthorizationSchema = new schema.Entity('digitalAuthorizations')
const filteredDigitalAuthorizationSchema = new schema.Entity('filteredDigitalAuthorizations')

const connectionSchema = new schema.Entity('connections')
const serviceChannelConnectionTypeSchema = new schema.Entity('serviceChannelConnectionTypes')
const serviceTypeSchema = new schema.Entity('serviceTypes')
const securitySchema = new schema.Entity('securityInfo')
const fundingTypeSchema = new schema.Entity('fundingTypes')

export const defineChildrenSchema = (itemSchema) => {
  itemSchema.define({
    children: new schema.Array(itemSchema),
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
  country: countrySchema,
  coordinates: new schema.Array(coordinateSchema)
})

phoneNumberSchema.define({
  prefixNumber: dialCodeSchema
})

countrySchema.define({
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

export const CommonSchemas = {
  LANGUAGE: languageSchema,
  LANGUAGE_ARRAY: new schema.Array(languageSchema),
  TRANSLATED_LANGUAGE: translationLanguageSchema,
  TRANSLATED_LANGUAGE_ARRAY: new schema.Array(translationLanguageSchema),
  PUBLISHING_STATUS: publishingStatusSchema,
  PUBLISHING_STATUS_ARRAY: new schema.Array(publishingStatusSchema),
  CHARGE_TYPE:chargeTypeSchema,
  CHARGE_TYPE_ARRAY: new schema.Array(chargeTypeSchema),
  AREA_INFORMATION_TYPE: areaInformationTypeSchema,
  AREA_INFORMATION_TYPE_ARRAY: new schema.Array(areaInformationTypeSchema),
  AREA_TYPE: areaTypeSchema,
  AREA_TYPE_ARRAY: new schema.Array(areaTypeSchema),
  SERVICE_CHANNEL_CONNECTION_TYPE: serviceChannelConnectionTypeSchema,
  SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY: new schema.Array(serviceChannelConnectionTypeSchema),
  PHONE_NUMBER_TYPE: phoneNumberTypeSchema,
  PHONE_NUMBER_TYPE_ARRAY: new schema.Array(phoneNumberTypeSchema),
  MUNICIPALITY: municipalitySchema,
  MUNICIPALITY_ARRAY: new schema.Array(municipalitySchema),
  PROVINCE: provinceSchema,
  PROVINCE_ARRAY: new schema.Array(provinceSchema),
  HOSPITAL_REGION: hospitalRegionSchema,
  HOSPITAL_REGION_ARRAY: new schema.Array(hospitalRegionSchema),
  BUSINESS_REGION: businessRegionSchema,
  BUSINESS_REGION_ARRAY: new schema.Array(businessRegionSchema),
  CHANNEL_TYPE: channelTypeSchema,
  CHANNEL_TYPE_ARRAY: new schema.Array(channelTypeSchema),
  WEB_PAGE_TYPE: webPageTypeSchema,
  WEB_PAGE_TYPE_ARRAY: new schema.Array(webPageTypeSchema),
  PRINTABLE_FORM_TYPE: printableFormUrlTypeSchema,
  PRINTABLE_FORM_TYPE_ARRAY: new schema.Array(printableFormUrlTypeSchema),
  WEB_PAGE: webPageSchema,
  WEB_PAGE_ARRAY: new schema.Array(webPageSchema),
  SERVICE_VOUCHER: voucherSchema,
  SERVICE_VOUCHER_ARRAY: new schema.Array(voucherSchema),
  PHONE_NUMBER: phoneNumberSchema,
  PHONE_NUMBER_ARRAY: new schema.Array(phoneNumberSchema),
  EMAIL: emailSchema,
  EMAIL_ARRAY: new schema.Array(emailSchema),
  COORDINATE: coordinateSchema,
  COORDINATE_ARRAY: new schema.Array(coordinateSchema),
  ADDRESS: addressSchema,
  ADDRESS_ARRAY: new schema.Array(addressSchema),
  SERVICE_HOUR_TYPE: serviceHourTypeSchema,
  SERVICE_HOUR_TYPE_ARRAY: new schema.Array(serviceHourTypeSchema),
  KEYWORD: keyWordSchema,
  KEYWORD_ARRAY: new schema.Array(keyWordSchema),
  BUSINESS: businessSchema,
  BUSINESS_ARRAY: new schema.Array(businessSchema),
  SEARCH: searchSchema,
  SERVICE_CLASS: serviceClassSchema,
  SERVICE_CLASS_ARRAY: new schema.Array(serviceClassSchema),
  CONNECTION: connectionSchema,
  CONNECTION_ARRAY: new schema.Array(connectionSchema),
  LAW: lawSchema,
  LAW_ARRAY: new schema.Array(lawSchema),
  DIAL_CODE: dialCodeSchema,
  DIAL_CODE_ARRAY: new schema.Array(dialCodeSchema),
    // finto
  FILTERED_SERVICE_CLASS_ARRAY: new schema.Array(filteredServiceClassSchema),
  INDUSTRIAL_CLASS: industrialClassSchema,
  INDUSTRIAL_CLASS_ARRAY: new schema.Array(industrialClassSchema),
  FILTERED_INDUSTRIAL_CLASS_ARRAY: new schema.Array(filteredIndustrialClassSchema),
  LIFE_EVENT: lifeEventsSchema,
  LIFE_EVENT_ARRAY: new schema.Array(lifeEventsSchema),
  FILTERED_LIFE_EVENTS_ARRAY: new schema.Array(filteredLifeEventsSchema),
  TARGET_GROUP: targetGroupsSchema,
  TARGET_GROUP_ARRAY: new schema.Array(targetGroupsSchema),
  ONTOLOGY_TERM: ontologyTermsSchema,
  ONTOLOGY_TERM_ARRAY: new schema.Array(ontologyTermsSchema),
  ANNOTATION_ONTOLOGY_TERM: annotationOntologyTermsSchema,
  ANNOTATION_ONTOLOGY_TERM_ARRAY: new schema.Array(annotationOntologyTermsSchema),
  DIGITAL_AUTHORIZATION: digitalAuthorizationSchema,
  DIGITAL_AUTHORIZATION_ARRAY: new schema.Array(digitalAuthorizationSchema),
  FILTERED_DIGITAL_AUTHORIZATION_ARRAY: new schema.Array(filteredDigitalAuthorizationSchema),

  SECURITY: securitySchema,

  FUNDING_TYPE: fundingTypeSchema,
  FUNDING_TYPE_ARRAY: new schema.Array(fundingTypeSchema)
}
