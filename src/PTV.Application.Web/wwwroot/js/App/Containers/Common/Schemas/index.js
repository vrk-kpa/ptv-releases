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

const publishingStatusSchema = new Schema('publishingStatuses', { idAttribute: publishingStatus => publishingStatus.id });
const chargeTypeSchema = new Schema('chargeTypes', { idAttribute: chargeType => chargeType.id });
const languageSchema = new Schema('languages', { idAttribute: language => language.id });
const translationLanguageSchema = new Schema('translationLanguages', { idAttribute: language => language.id });
const municipalitySchema = new Schema('municipalities', { idAttribute: municipality => municipality.id });
const channelTypeSchema = new Schema('channelTypes', { idAttribute: channelType => channelType.id });
const phoneNumberTypeSchema = new Schema('phoneNumberTypes', { idAttribute: phoneType => phoneType.id });
const webPageTypeSchema = new Schema('webPageTypes', { idAttribute: webPageType => webPageType.id });
const printableFormUrlTypeSchema = new Schema('printableFormUrlTypes', { idAttribute: type => type.id });
const webPageSchema = new Schema('webPages', { idAttribute: webPage => webPage.id });
const phoneNumberSchema = new Schema('phoneNumbers', { idAttribute: phoneNumber => phoneNumber.id });
const emailSchema = new Schema('emails', { idAttribute: email => email.id });
const addressSchema = new Schema('addresses', { idAttribute: address => address.id, meta: { localizable: true } });
const postalCodeSchema = new Schema('postalCodes', { idAttribute: postalCode => postalCode.id });
const serviceHourTypeSchema = new Schema('serviceHourType', { idAttribute: serviceHourType => serviceHourType.id });
const keyWordSchema = new Schema('keyWords', { idAttribute: keyWord => keyWord.id, meta: { localizable: true } });
const businessSchema = new Schema('business', { idAttribute: business => business.id }); 
const searchSchema = new Schema('searches', { idAttribute: search => search.id });

const serviceClassSchema = new Schema('serviceClasses', { idAttribute: serviceClass => serviceClass.id });
const filteredServiceClassSchema = new Schema('filteredServiceClasses', { idAttribute: serviceClass => serviceClass.id });
const industrialClassSchema = new Schema('industrialClasses', { idAttribute: industrialClass => industrialClass.id });
const filteredIndustrialClassSchema = new Schema('filteredIndustrialClasses', { idAttribute: industrialClass => industrialClass.id });
const lifeEventsSchema = new Schema('lifeEvents', { idAttribute: lifeEvent => lifeEvent.id });
const filteredLifeEventsSchema = new Schema('filteredLifeEvents', { idAttribute: lifeEvent => lifeEvent.id });
const targetGroupsSchema = new Schema('targetGroups', { idAttribute: targetGroup => targetGroup.id });
const ontologyTermsSchema = new Schema('ontologyTerms', { idAttribute: ontologyTerm => ontologyTerm.id });

export const defineChildrenSchema = (schema) => {
  schema.define({
    children: arrayOf(schema)  
  })
};

defineChildrenSchema(industrialClassSchema);
defineChildrenSchema(filteredIndustrialClassSchema);

defineChildrenSchema(serviceClassSchema);
defineChildrenSchema(filteredServiceClassSchema);

defineChildrenSchema(lifeEventsSchema);
defineChildrenSchema(filteredLifeEventsSchema);

defineChildrenSchema(targetGroupsSchema);
defineChildrenSchema(ontologyTermsSchema);


addressSchema.define({
    postalCode: postalCodeSchema
});

export const CommonSchemas = {
  LANGUAGE: languageSchema,
  LANGUAGE_ARRAY: arrayOf(languageSchema),
  TRANSLATED_LANGUAGE: translationLanguageSchema,
  TRANSLATED_LANGUAGE_ARRAY: arrayOf(translationLanguageSchema),
  PUBLISHING_STATUS: publishingStatusSchema,
  PUBLISHING_STATUS_ARRAY: arrayOf(publishingStatusSchema),
  CHARGE_TYPE:chargeTypeSchema,
  CHARGE_TYPE_ARRAY: arrayOf(chargeTypeSchema),
  PHONE_NUMBER_TYPE: phoneNumberTypeSchema,
  PHONE_NUMBER_TYPE_ARRAY: arrayOf(phoneNumberTypeSchema),
  MUNICIPALITY: municipalitySchema,
  MUNICIPALITY_ARRAY: arrayOf(municipalitySchema),
  CHANNEL_TYPE: channelTypeSchema,
  CHANNEL_TYPE_ARRAY: arrayOf(channelTypeSchema),
  WEB_PAGE_TYPE: webPageTypeSchema,
  WEB_PAGE_TYPE_ARRAY: arrayOf(webPageTypeSchema),
  PRINTABLE_FORM_TYPE: printableFormUrlTypeSchema,
  PRINTABLE_FORM_TYPE_ARRAY: arrayOf(printableFormUrlTypeSchema),
  WEB_PAGE: webPageSchema,
  WEB_PAGE_ARRAY: arrayOf(webPageSchema),
  PHONE_NUMBER: phoneNumberSchema,
  PHONE_NUMBER_ARRAY: arrayOf(phoneNumberSchema),
  EMAIL: emailSchema,
  EMAIL_ARRAY: arrayOf(emailSchema),
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
  ONTOLOGY_TERM_ARRAY: arrayOf(ontologyTermsSchema)
}