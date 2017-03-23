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
import { createSelector } from 'reselect';
import { Map, List, Iterable } from 'immutable';
import shortId from 'shortid';
import { keyToEntities, keyToStates } from './../Enums';

export const getCommon = state => state.get('common');
export const getServiceReducers = state => state.get('serviceReducers');
export const getOrganizationReducers = state => state.get('organizationReducers');
export const getGeneralDescriptionReducers = state => state.get('generalDescriptionReducers');
export const getChannelReducers = state => state.get('channelReducers');
export const joinTexts = (list, property) => Array.isArray(list) || Iterable.isIterable(list) ? list.map(x => x.get(property)).join(', ') : list;

// export const getDescriptionSearch = state => state.descriptionSearch;
// export const getElectronicChannel = state => state.electronicChannel;
// export const getServiceLocationChannel = state => state.serviceLocationChannel;
// export const getPhoneChannel = state => state.phoneChannel;
// export const getWebPageChannel = state => state.webPageChannel;
// export const getPrintableFormChannel = state => state.printableFormChannel;
// export const getChannelsSearch = state => state.channelsSearch;
// export const getOrganization = state => state.organization;
export const getPageModeState = state => state.get('pageModeState');

export const getJS = (map, defaultResult) => map && map.toJS ? map.toJS() : (defaultResult || null);
export const getList = map => map && map.toList ? map.toList() : List();
export const getArray = map => map && map.toArray ? map.toArray() : [];
export const getMap = map => map || Map();
export const getObjectArray = map => getJS(getList(map), []);

export const getParameters = (state, props) => props;
export const getParameterFromProps = (key, defaultValue) => (state, props) => props ? props[key] || defaultValue : defaultValue || null;
export const getIdFromProps = getParameterFromProps('id');
export const getLanguageParameter = getParameterFromProps('language', 'fi');
export const areStepDataValid = (stepModel, newId, languageCode) => stepModel.get('areDataValid') && (newId == stepModel.get('result')) && (languageCode === '' || languageCode == stepModel.get('language'));
export const getEntitiesForIds = (entities, ids, defaultResult) => ids && ids.size > 0 ? ids.map(id => entities.get(id)) : (defaultResult || null);
export const getEntitiesForIdsJS = (entities, ids) => getJS(getEntitiesForIds(entities, ids), []);
export const getEntityId = (pageModel, generateIfNotExists = true) => (pageModel ? pageModel.get('id') : null) || (generateIfNotExists ? shortId.generate() : null);
export const getPageLanguageCode = (pageModel) => pageModel.get('languageCode') || ''

//types
import { publishingStatuses } from '../Enums';

export const getCommonNotifications = createSelector(
    getCommon,
    common => common.get('notifications') || Map()
);

export const getEntities = createSelector(
    getCommon,
    common => common.get('entities') || Map()
);

export const getEntitiesWithGivenType = createSelector(
    [getEntities, getParameterFromProps('entitiesType')],
    (entities, type)  => entities.get(type) || Map()
)

export const getEntity = createSelector(
    [getEntitiesWithGivenType, getParameterFromProps('id')],
    (entities, id)  => entities.get(id) || Map()
)

export const getEntityName = createSelector(
    getEntity,
    entity  => entity.get('name') || Map()
)

export const getEntityAvailableLanguages = createSelector(
    getEntity,
    entity  => entity.get('languagesAvailabilities') || Map()
)

export const getLocalizedEntities = createSelector(
    [getEntities, getParameterFromProps('language')],
    (entities, language) => entities.get(language || 'fi') || Map()
);

export const getApiCalls = createSelector(
    getCommon,
    common => common.get('apiCalls') || Map()
);

export const getResults = createSelector(
    getCommon,
    common => common.get('result') || Map()
)

export const getEnums = createSelector(
    getCommon,
    common => common.get('enums') || Map()
)

export const getSearchBoxesImmutable = createSelector(
    getEntities,
    entities => entities.get('searchBoxes') || Map()
);

export const getSearchBoxesJS = createSelector(
    getSearchBoxesImmutable,
    searchBoxes => getJS(entities.get('searchBoxes'), [])
);

export const getSerchBoxJS = (key, state) => createSelector(
    getSearchBoxesImmutable,
    searchBoxes => {
        var map = getJS(searchBoxes, []);
        return map[key] || {} }
)(state);

export const getPublishingStatuses = createSelector(
    getEntities,
    entities => entities.get('publishingStatuses') || Map()
);

export const getPublishingStatus = createSelector(
    [getPublishingStatuses, getParameters],
    (statuses, id) => statuses.get(id)
);

export const getPublishingStatusType = createSelector(
    getPublishingStatus,
    status => status ? status.get('type') : null
);

export const getPublishingStatusMap = createSelector(
    [getPublishingStatuses, getParameters],
    (statuses, type) => statuses.filter(st => st.get('type')=== type).first() || Map()
);

export const getPublishingStatusId = createSelector(
    getPublishingStatusMap,
    statuses => statuses.get('id') || ''
);

export const getPublishingStatusDraft = createSelector(
    [getPublishingStatuses],
    statuses => statuses.filter(st => st.get('type')=== publishingStatuses.draft).first() || Map()
);

export const getPublishingStatusPublished = createSelector(
    [getPublishingStatuses],
    statuses => statuses.filter(st => st.get('type')=== publishingStatuses.published).first() || Map()
);

export const getPublishingStatusModified = createSelector(
    [getPublishingStatuses],
    statuses => statuses.filter(st => st.get('type')=== publishingStatuses.modified).first() || Map()
);

export const getChargeTypes = createSelector(
    getEntities,
    entities => entities.get('chargeTypes') || List()
);

export const getSearches = createSelector(
    getEntities,
    entities => entities.get('searches') || Map()
);

export const getChargeTypeMap = createSelector(
    [getChargeTypes, getParameters],
    (chargeTypes, code) => chargeTypes.filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
);

export const getChargeTypeId = createSelector(
    getChargeTypeMap,
    chargeTypes => chargeTypes.get('id') || ''
);

export const getChargeType = createSelector(
    [getChargeTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
);

export const getChargeTypesNameForId = createSelector(
    getChargeType,
    chargeType => chargeType.get('name')
);

export const getMunicipalities = createSelector(
    getEntities,
    entities => entities.get('municipalities') || List()
);

export const getMunicipalitiesList = createSelector(
    getEnums,
    entities => entities.get('municipalities') || List()
);

export const getCoordinates = createSelector(
    getEntities,
    entities => entities.get('coordinates') || Map()
);

export const getMunicipalitiesObjectArray = createSelector(
   [getMunicipalities, getMunicipalitiesList],
    (municipalities, ids) => getObjectArray(ids.map(id => municipalities.get(id)).sort(x => x.get('name')))
);
export const getChargeTypesObjectArray = createSelector(
    getChargeTypes,
    chargeTypes => getObjectArray(chargeTypes)
);

export const getChargeTypesFilteredObjectArray = createSelector(
    [getChargeTypes, getParameterFromProps('filterCode')],
    (chargeTypes, code) => getObjectArray(chargeTypes.filter(chargeType => chargeType.get('code') != code))
);

export const getLanguages = createSelector(
    getEntities,
    entities => entities.get('languages') || List()
);

export const getPublishingEntityRows = createSelector(
    [getEntityAvailableLanguages, getLanguages, getEntityName],
    (entityAvailableLanguages, languages, entityName)  =>{
      return entityAvailableLanguages.map(availLang => {
      const languageId = availLang.get('languageId') || ''
      const language = languages.get(languageId) || ''
      const languageCode = language.get('code') || ''
      return {id: languageId, name: entityName.get(languageCode),
      language: languageCode.toUpperCase(),
    visibility: availLang.get('statusId') }})}
)

export const getAvailableLanguagesPair = createSelector(
    getEntityAvailableLanguages,
    (entityAvailableLanguages) =>{
      let data = {};
      entityAvailableLanguages.map(availLang => {
      const languageId = availLang.get('languageId') || ''
      const statusId = availLang.get('statusId') || ''
      data[languageId] = statusId})
      return data }
)

export const getLanguageList = createSelector(
    getEnums,
    entities => entities.get('languages') || List()
);

export const getLanguagesObjectArray = createSelector(
    [getLanguages, getLanguageList],
    (languages, ids) => getObjectArray(ids.map(id => languages.get(id)))
);

export const getTranslationLanguages = createSelector(
    getEntities,
    entities => entities.get('translationLanguages') || Map()
);

export const getTranslationLanguageCodes = createSelector(
    getTranslationLanguages, entities => getList(entities.map(language => language.get('code')))
);

export const getTranslationLanguagesResult = createSelector(
    getApiCalls,
    apiCalls => apiCalls.get('translationLanguages') || Map()
);

export const getTranslationLanguageList = createSelector(
    getTranslationLanguagesResult,
    result => result.get('result') || List()
);

export const getTranslationLanguageIsFetching = createSelector(
    getTranslationLanguagesResult,
    result => result.get('isFetching') || false
);

export const getTranslationLanguageAreDataValid= createSelector(
    getTranslationLanguagesResult,
    result => result.get('areDataValid') || false
);

export const getTranslationLanguageMap = createSelector(
    [getTranslationLanguages, getParameters],
    (translationLanguages, code) => translationLanguages.filter(tL => tL.get('code').toLowerCase() === code).first() || Map()
);

export const getFiLanguage = createSelector(
    getTranslationLanguages,
    translationLanguages => translationLanguages.filter(tL => tL.get('code').toLowerCase() === 'fi').first() || Map()
);

export const getDefaultLanguage = createSelector(
    [getTranslationLanguages, getParameterFromProps('language')],
    (translationLanguages, code) => translationLanguages.filter(tL => tL.get('code').toLowerCase() === code).first() || Map()
);

export const getDefaultLanguageId = createSelector(
    [getDefaultLanguage, getFiLanguage],
    (defaultLanguage, getFiLanguage) => defaultLanguage.get('id') || getFiLanguage.get('id') || ''
);

export const getTranslationLanguageId = createSelector(
    getTranslationLanguageMap,
    translationLanguages => translationLanguages.get('id') || ''
);

export const getTranslationLanguageName = createSelector(
    getTranslationLanguageMap,
    translationLanguages => translationLanguages.get('name') || ''
);

export const getTranslationLanguage = createSelector(
    [getTranslationLanguages,getParameterFromProps('id')],
    (translationLanguages,id) => translationLanguages.get(id) || Map()
);

export const getTranslationLanguageCode = createSelector(
    getTranslationLanguage,
    language => language.get('code') || 'fi'
);

export const getTranslationLanguagesObjectArray = createSelector(
    [getTranslationLanguages, getTranslationLanguageList],
    (languages, ids) => getObjectArray(ids.map(id => languages.get(id)))
);

export const getLanguagesJS = createSelector(
    getLanguages,
    languages => getJS(languages, [])
);

export const getPhoneNumberTypes = createSelector(
    getEntities,
    entities => entities.get('phoneNumberTypes') || List()
);

export const getPhoneNumberTypeMap = createSelector(
    [getPhoneNumberTypes, getParameters],
    (phoneNumberTypes, code) => phoneNumberTypes.filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
);

export const getPhoneNumberTypeId = createSelector(
    getPhoneNumberTypeMap,
    phoneNumberTypes => phoneNumberTypes.get('id') || ''
);

export const getWebPageTypes = createSelector(
    getEntities,
    entities => entities.get('webPageTypes') || List()
);

export const getWebPageTypesList = createSelector(
    getEnums,
    entities => entities.get('webPageTypes') || List()
);

export const getWebPageTypesObjectArray = createSelector(
    [getWebPageTypes, getWebPageTypesList],
    (webPageTypes, ids) => getEntitiesForIdsJS(webPageTypes, ids)
);

export const getPrintableFormUrlTypes = createSelector(
    getEntities,
    entities => entities.get('printableFormUrlTypes') || List()
);

export const getPrintableFormUrlTypesList = createSelector(
    getEnums,
    entities => entities.get('printableFormUrlTypes') || List()
);

export const getPrintableFormUrlTypesObjectArray = createSelector(
    [getPrintableFormUrlTypes, getPrintableFormUrlTypesList],
    (webPageTypes, ids) => getEntitiesForIdsJS(webPageTypes, ids)
);

export const getPhoneNumberTypesJS = createSelector(
    getPhoneNumberTypes,
    phoneNumberTypes => getJS(phoneNumberTypes, [])
);

export const getPhoneNumberTypesObjectArray = createSelector(
    getPhoneNumberTypes,
    phoneNumberTypes => getObjectArray(phoneNumberTypes)
);

export const getChannelTypes = createSelector(
    getEntities,
    entities => entities.get('channelTypes') || List()
);

export const getChannelType = createSelector(
    [getChannelTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
);

export const geChannelTypeNameForId = createSelector(
    getChannelType,
    channelType => channelType.get('name')
);

export const getChannelTypesList = createSelector(
    getEnums,
    entities => entities.get('channelTypes') || List()
);

export const getChannelTypesObjectArray = createSelector(
    [getChannelTypes, getChannelTypesList],
    (channelTypes, ids) => getObjectArray(ids.map(id => channelTypes.get(id)))
);

export const getChannelTypesJS = createSelector(
    getChannelTypes,
    channelTypes => getJS(channelTypes, [])
);

export const getPublishingStatusesImmutableList = createSelector(
    getPublishingStatuses,
    publishingStatuses => getList(publishingStatuses)
);

export const getPublishingStatusesJS = createSelector(
    getPublishingStatuses,
    publishingStatuses => getJS(publishingStatuses, [])
);

export const getNotifications = createSelector(
    getCommon,
    common => common.get('notifications') || Map()
);

export const getNotificationsByKey = createSelector(
    [getNotifications, getParameterFromProps('keyToState'), getParameterFromProps('notificationKey')],
    (notification, keyToState, notificationKey) =>
        notification.getIn([keyToState, notificationKey || 'all']) || Map()
);

export const getErrors = createSelector(
    getNotificationsByKey,
    notifications => notifications.get('errors') || List()
);

export const getErrorMessages = createSelector(
    getErrors,
    errors => getArray(errors.map(x => ({message: x.get('error'), info: x.get('stack') })))
);

export const getInfos = createSelector(
    getNotificationsByKey,
    notifications => getArray(notifications.get('infos'))
);

export const getPageModeStateForKey = createSelector(
    [getPageModeState, getParameterFromProps('keyToState')],
    (state, key) => state.get(key) || Map()
);

export const getPageModeStateData = createSelector(
    getPageModeState,
    state => state.get('data') || Map()
);

export const getPageEntityId = createSelector(
    getPageModeStateForKey,
    state => getEntityId(state)
);

export const getPageEntityLanguageCode = createSelector(
    getPageModeStateForKey,
    state => getPageLanguageCode(state)
);

export const getPageEntityIdNotGenerated = createSelector(
    getPageModeStateForKey,
    state => getEntityId(state, false)
);

export const getIsDirty = createSelector(
    getPageModeStateForKey,
    search => search.get('isDirty') || false
);

export const getLanguageFrom = createSelector(
    getPageModeStateForKey,
    search => search.get('languageFrom') || null
);

export const getLanguageTo = createSelector(
    [getPageModeStateForKey, getDefaultLanguageId],
    (search, defaultLanguage) => search.get('languageTo') || defaultLanguage
);

export const getToLanguage = createSelector(
    [getTranslationLanguages, getLanguageTo],
    (translationLanguages, id) => translationLanguages.get(id) || Map()
);

export const getLanguageToCode = createSelector(
    getToLanguage,
    language => language.get('code') || 'fi'
);

export const getFromLanguage = createSelector(
    [getTranslationLanguages, getLanguageFrom],
    (translationLanguages, id) => translationLanguages.get(id) || Map()
);

export const getLanguageFromCode = createSelector(
    getFromLanguage,
    language =>language.get('code') || 'fi'
);

export const getLanguageToName = createSelector(
    getToLanguage,
    language => language.get('name') || ''
);

export const getApiEntityId = (state, entityId) => entityId || ''

//related url attachments

export const getUrlAttachmentsApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('urlAttachments') || Map()
);

export const getUrlAttachmentsApiCall = createSelector(
    [getUrlAttachmentsApiCalls, getParameterFromProps('id')],
    (urlAttachmentsApiCalls, id) => urlAttachmentsApiCalls.get(id) || Map()
);

export const getUrlAttachmentIsFetching = createSelector(
    getUrlAttachmentsApiCall,
    urlAttachmentApiCall => urlAttachmentApiCall.get('isFetching') || false
);

export const getUrlAttachmentAreDataValid = createSelector(
    getUrlAttachmentsApiCall,
    urlAttachmentApiCall => urlAttachmentApiCall.get('areDataValid') || false
);

// related emails
export const getEmails = createSelector(
    getEntities,
    entities => entities.get('emails') || Map()
);

export const getEmail = createSelector(
    [getEmails, getApiEntityId],
    (emails, emailId) => emails.get(emailId) || Map()
);

export const getEmailEmail = createSelector(
    getEmail,
    email => email.get('email') || ''
);

export const getEmailAdditionalInformation = createSelector(
    getEmail,
    email => email.get('additionalInformation') || ''
);

// related phoneNumbers
export const getPhoneNumbers = createSelector(
    getEntities,
    entities => entities.get('phoneNumbers') || Map()
);

export const getPhoneNumber = createSelector(
    [getPhoneNumbers, getParameterFromProps('phoneId')],
    (phoneNumbers, phoneNumberId) => phoneNumbers.get(phoneNumberId) || Map()
);

export const getPhoneNumberChargeTypeId = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('chargeTypeId') || null
);

export const getPhoneNumberChargeDescription = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('chargeDescription') || ''
);

export const getPhoneNumberPhoneNumber = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('number') || ''
);

export const getPhoneNumberAdditionalInformation = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('additionalInformation') || ''
);

export const getPhoneNumberPrefixNumber = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('prefixNumber') || ''
);
export const getPhoneNumberType = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('typeId') || null
);
export const getIsFinnishServiceNumber = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('isFinnishServiceNumber') || false
);

//related dialCodes
export const getDialCodes = createSelector(
    getEntities,
    entities => entities.get('dialCodes') || Map()
);

export const getSelectedPrefixNumber = createSelector(
    [getDialCodes, getPhoneNumberPrefixNumber],
    (dialCodes, prefixNumberId) => dialCodes.get(prefixNumberId)
);

export const getSelectedPrefixNumberJS = createSelector(
    [getSelectedPrefixNumber],
    (prefixNumber) => getJS(prefixNumber)
);

// related webpages
export const getWebPages = createSelector(
    getEntities,
    entities => entities.get('webPages') || Map()
);

export const getWebPagesApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('webPages') || Map()
);

export const getWebPage = createSelector(
    [getWebPages, getIdFromProps],
    (webPages, webPageId) => webPages.get(webPageId) || Map()
);

export const getWebPageApiCall = createSelector(
    [getWebPagesApiCalls, getIdFromProps],
    (webPagesApiCalls, webPageId) => webPagesApiCalls.get(webPageId) || Map()
);

export const getWebPageIsFetching = createSelector(
    getWebPageApiCall,
    webPageApiCall => webPageApiCall.get('isFetching') || false
);

export const getWebPageAreDataValid = createSelector(
    getWebPageApiCall,
    webPageApiCall => webPageApiCall.get('areDataValid') || false
);

export const getWebPageTypeId = createSelector(
    getWebPage,
    webPage => webPage.get('typeId') || ''
);

export const getWebPageUrlAddress = createSelector(
    getWebPage,
    webPage => webPage.get('urlAddress') || ''
);

export const getWebPageName = createSelector(
    getWebPage,
    webPage => webPage.get('name') || ''
);

export const getWebPageOrderNumber = createSelector(
    getWebPage,
    webPage => webPage.get('orderNumber') || ''
);

export const getWebPageUrlExists = createSelector(
    getWebPage,
    webPage => typeof webPage.get('urlExists') === 'boolean' ? webPage.get('urlExists') : null
);

// related laws
export const getLawsEntities = createSelector(
    getEntities,
    entities => entities.get('laws') || Map()
);

export const getLaws = createSelector(
    [getLawsEntities, getLanguageParameter],
    (laws, language) => laws.map((law) => law.get(language))
);

export const getLawApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('laws') || Map()
);

export const getLaw = createSelector(
    [getLaws, getIdFromProps],
    (laws, lawId) => laws.get(lawId) || Map()
);

export const getLawApiCall = createSelector(
    [getLawApiCalls, getIdFromProps],
    (lawsApiCalls, lawId) => lawsApiCalls.get(lawId) || Map()
);

export const getLawIsFetching = createSelector(
    getLawApiCall,
    lawsApiCalls => lawsApiCalls.get('isFetching') || false
);

export const getLawAreDataValid = createSelector(
    getLawApiCall,
    lawsApiCalls => lawsApiCalls.get('areDataValid') || false
);

export const getLawUrlAddress = createSelector(
    getLaw,
    law => law.get('urlAddress') || ''
);

export const getLawName = createSelector(
    getLaw,
    law => law.get('name') || ''
);

export const getLawUrlExists = createSelector(
    getLaw,
    law => typeof law.get('urlExists') === 'boolean' ? law.get('urlExists') : null
);


// related addresses
export const getAddressEntities = createSelector(
    getEntities,
    entities => entities.get('addresses') || Map()
);

export const getAddresses = createSelector(
    [getAddressEntities, getLanguageParameter],
    (addresses, language) => addresses.map((address) => address.get(language))
);

export const getFromAddresses = createSelector(
    [getAddressEntities, getLanguageFromCode],
    (addresses, language) => addresses.map((address) => address.get(language))
);

export const getAddressesForModel = createSelector(
    [getAddressEntities, getLanguageParameter, getCoordinates],
    (addresses, language, coordinates) => addresses.map((address) => {
        const concreteAddress = address.get(language);
        return concreteAddress ? concreteAddress.set('coordinates', getEntitiesForIds(coordinates, concreteAddress.get('coordinates'))): Map()})
);

export const getAddress = createSelector(
    [getAddresses, getIdFromProps],
    (addresses, addressId) => addresses.get(addressId) || Map()
);

export const getFromAddress = createSelector(
    [getFromAddresses, getIdFromProps],
    (addresses, addressId) => addresses.get(addressId) || Map()
);

export const getAddressStreetType = createSelector(
    getAddress,
    address => address.get('streetType') || 'Street'
);

export const getAddressStreet = createSelector(
    [getAddress, getFromAddress],
    (address, defaultAddress) => address.get('street') || defaultAddress.get('street') || ''
);

export const getAddressPOBox = createSelector(
    getAddress,
    address => address.get('poBox') || ''
);

export const getAddressStreetNumber = createSelector(
    getAddress,
    address => address.get('streetNumber') || ''
);

export const getAddressStreetLongtitude = createSelector(
    getAddress,
    address => address.get('longtitude') || null
);

export const getAddressCoordinates = createSelector(
    getAddress,
    address => address.get('coordinates') || List()
);

export const getCoordinatesForAddressId = createSelector(
    [getAddressCoordinates, getCoordinates],
    (coordinatesIds, coordinates) => getEntitiesForIds(coordinates, coordinatesIds, Map())
);

export const getAddressStreetLatitude = createSelector(
    getAddress,
    address => address.get('latitude') || null
);

export const getAddressStreetCoordinatesStateSelector = createSelector(
    getAddress,
    address => address.get('coordinateState') || ''
);

export const getAddressStreetCoordinatesState = createSelector(
    getAddressStreetCoordinatesStateSelector,
    coordinateState => coordinateState.toLowerCase()
);

export const getAddressPostalCode = createSelector(
    getAddress,
    address => address.get('postalCode') || ''
);

export const getAddressAditionalInformation = createSelector(
    [getAddress, getFromAddress],
     (address, defaultAddress) => address.get('additionalInformation') || defaultAddress.get('additionalInformation') || ''
);

export const getAddressesApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('addresses') || Map()
);

export const getAddressesApiCall = createSelector(
    [getAddressesApiCalls, getParameterFromProps('id')],
    (addressesApiCalls, id) => {
      return addressesApiCalls.get(id) || Map()
    }
);

export const getAddressIsFetching = createSelector(
    getAddressesApiCall,
    addressApiCall => addressApiCall.get('isFetching') || false
);

export const getAttemptCount = createSelector(
    getAddressesApiCall,
    addressApiCall => addressApiCall.get('attemptCount') || 0
);

// related postalCodes
export const getPostalCodes = createSelector(
    getEntities,
    entities => entities.get('postalCodes') || Map()
);

export const getSelectedPostalCode = createSelector(
    [getPostalCodes, getAddressPostalCode],
    (postalCodes, postalCodeId) => postalCodes.get(postalCodeId)
);

export const getPostalCode = createSelector(
    getSelectedPostalCode,
    postalCode => getMap(postalCode)
);

export const getPostalPostOffice = createSelector(
    getPostalCode,
    postalCode => postalCode.get('postOffice') || ''
);

export const getDefaultMunicipality = createSelector(
    getPostalCode,
    postalCode => postalCode ? postalCode.get('municipalityId') : ''
);

export const getSelectedMunicipality = createSelector(
    [getAddress, getDefaultMunicipality],
    (address, municipalityId) => address.get('municipalityId') || municipalityId || ''
);

// related business
export const getBusinessArray = createSelector(
    getEntities,
    entities => entities.get('business') || Map()
);

export const getBusiness = createSelector(
    [getBusinessArray, getApiEntityId],
    (business, businessId) => business.get(businessId) || Map()
);

export const getBusinessCode = createSelector(
    getBusiness,
    business => business.get('code') || ''
);

// related to all steps
export const getApiCallForType = createSelector(
    [getApiCalls, getParameterFromProps('keyToState')],
    (apiCalls, type) => (Array.isArray(type) ? apiCalls.getIn(type) : apiCalls.get(type)) || new Map()
);

export const getSearchModel = createSelector(
    getApiCallForType,
    search => search.get('search') || new Map()
);

export const getSearchResultsModel = createSelector(
    getApiCallForType,
    search => search.get('searchResults') || new Map()
);

export const getStep1Model = createSelector(
    getApiCallForType,
    search => search.get('step1Form') || new Map()
);

export const getStep2Model = createSelector(
    getApiCallForType,
    search => search.get('step2Form') || new Map()
);

export const getStep3Model = createSelector(
    getApiCallForType,
    search => search.get('step3Form') || new Map()
);

export const getStep4Model = createSelector(
    getApiCallForType,
    search => search.get('step4Form') || new Map()
);

export const getChannelServiceStepModel = createSelector(
    getApiCallForType,
    search => search.get('channelServiceStep') || new Map()
);

export const getStepCommonModel = createSelector(
    getApiCallForType,
    search => search.get('all') || new Map()
);

const getId = (state, props) => props.id || ''

export const getStepCommonModelForId = createSelector(
    [getStepCommonModel, getId],
    (model, id) => model.get(id) || Map()
)

export const getIsFetchingForId = createSelector(
    getStepCommonModelForId,
    model => model.get('isFetching') || false
)

export const getLockModel = createSelector(
    getApiCallForType,
    search => search.get('lock') || new Map()
);

export const getAnnotationModel = createSelector(
    getApiCallForType,
    search => search.get('annotation') || new Map()
);

export const getStepInnerSearchModel = createSelector(
    getApiCallForType,
    search => search.get('innerSearch') || new Map()
);

export const getSearchIsFetching = createSelector(
    getSearchModel,
    searchModel => searchModel.get('isFetching') || false
)

export const getSearchAreDataValid = createSelector(
    getSearchModel,
    searchModel => searchModel.get('areDataValid') || false
)

export const getSearchIds = createSelector(
    getSearchModel,
    search => search.get('result') || null
);

export const getSearchResultsIsFetching = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('isFetching') || false
)

export const getSearchResultsAreDataValid = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('areDataValid') || false
)

export const getSearchResultsMaxPageCount = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('maxPageCount') || 0
)

export const getSearchResultsCount = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('count') || 0
)

export const getSearchResultsPageNumber = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('pageNumber') || 0
)

export const getSearchResultsIsMoreThanMax = createSelector(
    [getSearchResultsMaxPageCount, getSearchResultsCount, getSearchResultsPageNumber],
    (maxPageCount, count, pageNumber) => count > (maxPageCount * pageNumber) || false
)

export const getSearchResultsIsMoreAvailable = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('moreAvailable') || false
)

export const getSearchResultsLanguage = createSelector(
    getSearchResultsModel,
    searchModel => searchModel.get('language') || 'fi'
)

export const getSearchedNewEntities = createSelector(
    [getSearchResultsModel,getParameterFromProps('keyToEntities')],
    (searchModel, keyToEntities) => searchModel.get(keyToEntities) || List()
)

export const getSearchedPrevEntities = createSelector(
    getSearchResultsModel,
    (searchModel) => searchModel.get('prevEntities') || List()
)

export const getSearchedEntities = createSelector(
    [getSearchedNewEntities,getSearchedPrevEntities],
    (newEntities, prevEntities) => prevEntities.concat(newEntities) || List()
)

export const getStep1isFetching = createSelector(
    getStep1Model,
    step1Model => step1Model.get('isFetching') || false
)

export const getStep2isFetching = createSelector(
    getStep2Model,
    step2Model => step2Model.get('isFetching') || false
)

export const getStep3isFetching = createSelector(
    getStep3Model,
    step3Model => step3Model.get('isFetching') || false
)

export const getStep4isFetching = createSelector(
    getStep4Model,
    step4Model => step4Model.get('isFetching') || false
)

export const getChannelServiceStepIsFetching = createSelector(
    getChannelServiceStepModel,
    channelServiceStepModel => channelServiceStepModel.get('isFetching') || false
)

export const getStepCommonIsFetching = createSelector(
    getStepCommonModel,
    step4Model => step4Model.get('isFetching') || false
)

export const getStepCommonAreDataValid = createSelector(
    getStepCommonModel,
    commonModel => commonModel.get('areDataValid') || false
)

export const getLockIsFetching = createSelector(
    getLockModel,
    lockModel => lockModel.get('isFetching') || false
)

export const getLockAreDataValid = createSelector(
    getLockModel,
    lockModel => lockModel.get('areDataValid') || false
)

export const getEntityLocked = createSelector(
    getLockModel,
    lockModel => lockModel.get('entityLockedForMe') || false
)

export const getAnnotationIsFetching = createSelector(
    getAnnotationModel,
    annotationModel => annotationModel.get('isFetching') || false
)

export const getIsFetchingOfAnyStep = createSelector(
    [getLockIsFetching, getStepCommonIsFetching, getStep1isFetching, getStep2isFetching, getStep3isFetching, getStep4isFetching],
    (lock, common, step1, step2, step3, step4) => lock || common || step1 || step2 || step3 || step4
)

export const getStep1AreDataValid = createSelector(
    [getStep1Model, getPageEntityId, getPageEntityLanguageCode],
    (stepModel, id, languageCode) => areStepDataValid(stepModel, id, languageCode) || false
)

export const getStep2AreDataValid = createSelector(
    [getStep2Model, getPageEntityId, getPageEntityLanguageCode],
    (stepModel, id, languageCode) => areStepDataValid(stepModel, id, languageCode) || false
)

export const getStep3AreDataValid = createSelector(
    [getStep3Model, getPageEntityId, getPageEntityLanguageCode],
    (stepModel, id, languageCode) => areStepDataValid(stepModel, id, languageCode) || false
)

export const getStep4AreDataValid = createSelector(
    [getStep4Model, getPageEntityId, getPageEntityLanguageCode],
    (stepModel, id, languageCode) => areStepDataValid(stepModel, id, languageCode) || false
)

export const getChannelServiceStepAreDataValid = createSelector(
    [getChannelServiceStepModel, getPageEntityId],
    (stepModel, id) => areStepDataValid(stepModel, id) || false
)

export const getStepInnerSearchIsFetching = createSelector(
    getStepInnerSearchModel,
    innerSearchModel => innerSearchModel.get('isFetching') || false
)

export const getStepInnerSearchAreDataValid = createSelector(
    getStepInnerSearchModel,
    innerSearchModel => innerSearchModel.get('areDataValid') || false
)

// tree
export const getNodes = createSelector(
    getCommon,
    common => common.get('nodes') || Map()
);

export const getNodeInfo = createSelector(
    [getNodes, getParameterFromProps('id'), getParameterFromProps('contextId')],
    (nodes, id, contextId) => (contextId ? nodes.getIn([contextId, id]) : nodes.get(id)) || Map()
);

export const getNodeIsCollapsed = createSelector(
    getNodeInfo,
    node => node.get('isCollapsed')
);

export const getNodeApiCall = createSelector(
    [getApiCalls, getParameterFromProps('id')],
    (apiCalls, id) => apiCalls.getIn(['nodes', id]) || Map()
);
export const getNodeIsFetching = createSelector(
    getNodeApiCall,
    node => node.get('isFetching') || false
);

export const getNodeSearchItem = createSelector(
    getSearchModel,
    search => search.get('id')? Map({id: search.get('id'), name: search.get('searchValue')}) : null
);

export const getNodeSearchItemJS = createSelector(
    getNodeSearchItem,
    search => getJS(search)
);

export const getSearchDomain = createSelector(
  getPageModeState,
  pageModeState => pageModeState.get('searchDomain')
)
