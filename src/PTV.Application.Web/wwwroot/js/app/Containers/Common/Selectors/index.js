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
import { createSelector } from 'reselect'
import { Map, List, fromJS, Iterable, Set } from 'immutable'
import shortId from 'shortid'
import { keyToEntities, keyToStates } from './../Enums'

export const getCommon = state => state.get('common')
export const getServiceReducers = state => state.get('serviceReducers')
export const getOrganizationReducers = state => state.get('organizationReducers')
export const getGeneralDescriptionReducers = state => state.get('generalDescriptionReducers')
export const getChannelReducers = state => state.get('channelReducers')
export const getUi = state => state.get('ui')
export const joinTexts = (list, property) => Array.isArray(list) || Iterable.isIterable(list) ? list.map(x => x.get(property)).join(', ') : list

// export const getDescriptionSearch = state => state.descriptionSearch;
// export const getElectronicChannel = state => state.electronicChannel;
// export const getServiceLocationChannel = state => state.serviceLocationChannel;
// export const getPhoneChannel = state => state.phoneChannel;
// export const getWebPageChannel = state => state.webPageChannel;
// export const getPrintableFormChannel = state => state.printableFormChannel;
// export const getChannelsSearch = state => state.channelsSearch;
// export const getOrganization = state => state.organization;
export const getPageModeState = state => state.get('pageModeState')

export const getJS = (map, defaultResult) => map && map.toJS ? map.toJS() : (defaultResult || null)
export const getList = map => map && map.toList ? map.toList() : List()
export const getArray = map => map && map.toArray ? map.toArray() : []
export const getMap = map => map || Map()
export const getObjectArray = map => getJS(getList(map), [])

export const getParameters = (state, props) => props
export const getParameterFromProps = (key, defaultValue) => (state, props) => props ? props[key] || defaultValue : defaultValue || null
export const getIdFromProps = getParameterFromProps('id')
export const getLanguageParameter = getParameterFromProps('language', 'fi')
export const areStepDataValid = (stepModel, newId, languageCode) => stepModel.get('areDataValid') && (newId == stepModel.get('result')) && (languageCode === '' || languageCode == stepModel.get('language'))
export const getEntitiesForIds = (entities, ids, defaultResult) => ids && ids.size > 0 ? ids.map(id => entities.get(id)) : (defaultResult || null)
export const getEntitiesForIdsJS = (entities, ids) => getJS(getEntitiesForIds(entities, ids), [])
export const getEntityId = (pageModel, generateIfNotExists = true) => (pageModel ? pageModel.get('id') : null) || (generateIfNotExists ? shortId.generate() : null)
export const getPageLanguageCode = (pageModel) => pageModel.get('languageCode') || ''

// types
import { publishingStatuses } from '../Enums'

export const getCommonNotifications = createSelector(
    getCommon,
    common => common.get('notifications') || Map()
)

export const getUiData = createSelector(
    getUi,
    common => common.get('uiData') || Map()
)

export const getEntities = createSelector(
    getCommon,
    common => common.get('entities') || Map()
)

export const getEntitiesWithGivenType = createSelector(
    [getEntities, getParameterFromProps('entitiesType')],
    (entities, type) => entities.get(type) || Map()
)

export const getEntity = createSelector(
    [getEntitiesWithGivenType, getParameterFromProps('id')],
    (entities, id) => entities.get(id) || Map()
)

export const getIsSelectedSelector = listSelector => createSelector(
  [listSelector, getIdFromProps],
  (list, id) => Set(list).has(id)
)

export const getEntityName = createSelector(
    getEntity,
    entity => entity.get('name') || Map()
)

export const getEntityAvailableLanguages = createSelector(
    getEntity,
    entity => entity.get('languagesAvailabilities') || Map()
)

export const getLocalizedEntities = createSelector(
    [getEntities, getParameterFromProps('language')],
    (entities, language) => entities.get(language || 'fi') || Map()
)

export const getApiCalls = createSelector(
    getCommon,
    common => common.get('apiCalls') || Map()
)

export const getApiCall = key => createSelector(
    getApiCalls,
    apiCalls => apiCalls.get(key) || Map()
)

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
)

export const getSearchBoxesJS = createSelector(
    getSearchBoxesImmutable,
    searchBoxes => getJS(searchBoxes.get('searchBoxes'), [])
)

export const getSerchBoxJS = (key, state) => createSelector(
    getSearchBoxesImmutable,
    searchBoxes => {
      var map = getJS(searchBoxes, [])
      return map[key] || {}
    }
)(state)

export const getPublishingStatuses = createSelector(
    getEntities,
    entities => entities.get('publishingStatuses') || Map()
)

export const getPublishingStatus = createSelector(
    [getPublishingStatuses, getParameters],
    (statuses, id) => statuses.get(id)
)

export const getPublishingStatusType = createSelector(
    getPublishingStatus,
    status => status ? status.get('type') : null
)

export const getPublishingStatusMap = createSelector(
    [getPublishingStatuses, getParameters],
    (statuses, type) => statuses.filter(st => st.get('type') === type).first() || Map()
)

export const getPublishingStatusId = createSelector(
    getPublishingStatusMap,
    statuses => statuses.get('id') || ''
)

export const getPublishingStatusDraft = createSelector(
    [getPublishingStatuses],
    statuses => statuses.filter(st => st.get('type') === publishingStatuses.draft).first() || Map()
)

export const getPublishingStatusPublished = createSelector(
    [getPublishingStatuses],
    statuses => statuses.filter(st => st.get('type') === publishingStatuses.published).first() || Map()
)

export const getPublishingStatusModified = createSelector(
    [getPublishingStatuses],
    statuses => statuses.filter(st => st.get('type') === publishingStatuses.modified).first() || Map()
)

export const getChargeTypes = createSelector(
  getEntities,
  entities => entities.get('chargeTypes') || Map()
)

export const getServiceChannelConnectionTypes = createSelector(
  getEntities,
  entities => entities.get('serviceChannelConnectionTypes') || Map()
)

export const getServiceChannelConnectionTypesWithoutCommonFor = createSelector(
  getServiceChannelConnectionTypes,
  serviceChannelConnectionTypes => serviceChannelConnectionTypes.filter(
    sct => sct.get('code') != 'CommonFor'
  ) || List()
)

export const getServiceChannelConnectionTypesDefault = createSelector(
  getServiceChannelConnectionTypes,
  serviceChannelConnectionTypes => serviceChannelConnectionTypes.find(
    sct => sct.get('code') === 'CommonForAll'
  ) || Map()
)

export const getServiceChannelConnectionTypesDefaultId = createSelector(
  getServiceChannelConnectionTypesDefault,
  serviceChannelConnectionDefaultType => serviceChannelConnectionDefaultType.get('id') || ''
)

export const getServiceChannelConnectionTypesObjectArray = createSelector(
    getServiceChannelConnectionTypesWithoutCommonFor,
    serviceChannelConnectionTypes => getObjectArray(serviceChannelConnectionTypes)
)

export const getSearches = createSelector(
    getEntities,
    entities => entities.get('searches') || Map()
)

export const getChargeTypeMap = createSelector(
    [getChargeTypes, getParameters],
    (chargeTypes, code) => chargeTypes.filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
)

export const getChargeTypeId = createSelector(
    getChargeTypeMap,
    chargeTypes => chargeTypes.get('id') || ''
)

export const getChargeType = createSelector(
    [getChargeTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
)

export const getChargeTypesNameForId = createSelector(
    getChargeType,
    chargeType => chargeType.get('name')
)

export const getMunicipalities = createSelector(
    getEntities,
    entities => entities.get('municipalities') || List()
)

export const getMunicipalitiesList = createSelector(
    getEnums,
    entities => entities.get('municipalities') || List()
)

export const getCoordinates = createSelector(
    getEntities,
    entities => entities.get('coordinates') || List()
)

export const getMunicipalitiesObjectArray = createSelector(
   [getMunicipalities, getMunicipalitiesList],
    (municipalities, ids) => getObjectArray(ids.map(id => municipalities.get(id)).sort(x => x.get('name')))
)

export const getMunicipality = createSelector(
    [getMunicipalities, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

// areas provincies
export const getProvincies = createSelector(
    getEntities,
    entities => entities.get('provincies') || List()
)

export const getProvinciesList = createSelector(
    getEnums,
    entities => entities.get('provincies') || List()
)

export const getProvinciesObjectArray = createSelector(
   [getProvincies, getProvinciesList],
    (provincies, ids) => getObjectArray(ids.map(id => provincies.get(id)).sort(x => x.get('name')))
)

export const getProvince = createSelector(
    [getProvincies, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

// areas hospitalRegions
export const getHospitalRegions = createSelector(
    getEntities,
    entities => entities.get('hospitalRegions') || List()
)

export const getHospitalRegionsList = createSelector(
    getEnums,
    entities => entities.get('hospitalRegions') || List()
)

export const getHospitalRegionsObjectArray = createSelector(
   [getHospitalRegions, getHospitalRegionsList],
    (hospitalRegions, ids) => getObjectArray(ids.map(id => hospitalRegions.get(id)).sort(x => x.get('name')))
)

export const getHospitalRegion = createSelector(
    [getHospitalRegions, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

// areas businessRegions
export const getBusinessRegions = createSelector(
    getEntities,
    entities => entities.get('businessRegions') || List()
)

export const getBusinessRegionsList = createSelector(
    getEnums,
    entities => entities.get('businessRegions') || List()
)

export const getBusinessRegionsObjectArray = createSelector(
   [getBusinessRegions, getBusinessRegionsList],
    (businessRegions, ids) => getObjectArray(ids.map(id => businessRegions.get(id)).sort(x => x.get('name')))
)

export const getBusinessRegion = createSelector(
    [getBusinessRegions, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

export const getChargeTypesObjectArray = createSelector(
    getChargeTypes,
    chargeTypes => getObjectArray(chargeTypes)
)

export const getChargeTypesFilteredObjectArray = createSelector(
    [getChargeTypes, getParameterFromProps('filterCode')],
    (chargeTypes, code) => getObjectArray(chargeTypes.filter(chargeType => chargeType.get('code') != code))
)

export const getLanguages = createSelector(
    getEntities,
    entities => entities.get('languages') || List()
)

export const getPublishingEntityRows = createSelector(
    [getEntityAvailableLanguages, getLanguages, getEntityName],
    (entityAvailableLanguages, languages, entityName) => {
      return entityAvailableLanguages.map(availLang => {
        const languageId = availLang.get('languageId') || ''
        const language = languages.get(languageId) || ''
        const languageCode = language.get('code') || ''
        return { id: languageId,
          name: entityName.get(languageCode),
          language: languageCode.toUpperCase(),
          visibility: availLang.get('statusId') }
      })
    }
)

export const getAvailableLanguagesPair = createSelector(
    getEntityAvailableLanguages,
    (entityAvailableLanguages) => {
      let data = {}
      entityAvailableLanguages.map(availLang => {
        const languageId = availLang.get('languageId') || ''
        const statusId = availLang.get('statusId') || ''
        data[languageId] = statusId
      })
      return data
    }
)

export const getEntityLanguageVersions = createSelector(
  getEntityAvailableLanguages,
  entityAvailableLanguages => {
    let availableLanguages = entityAvailableLanguages.map(availLang => {
      const languageId = availLang.get('languageId') || ''
      const statusId = availLang.get('statusId') || ''
      return {
        languageId,
        statusId
      }
    })
    return availableLanguages
  }
)

export const getEntityAvailableLanguagesCodes = createSelector(
    [getEntityAvailableLanguages, getLanguages],
    (entityAvailableLanguages, languages) => {
      return entityAvailableLanguages.map(availLang => {
        const languageId = availLang.get('languageId') || ''
        const language = languages.get(languageId) || Map()
        const languageCode = language.get('code') || ''
        return { code: languageCode.toUpperCase() }
      })
    }
)

export const getLanguageList = createSelector(
    getEnums,
    entities => entities.get('languages') || List()
)

export const getLanguagesObjectArray = createSelector(
    [getLanguages, getLanguageList],
    (languages, ids) => getObjectArray(ids.map(id => languages.get(id)))
)

export const getTranslationLanguages = createSelector(
    getEntities,
    entities => entities.get('translationLanguages') || Map()
)

export const getTranslationLanguageCodes = createSelector(
    getTranslationLanguages, entities => getList(entities.map(language => language.get('code')))
)

export const getTranslationLanguagesResult = getApiCall('translationLanguages')

export const getTranslationLanguageList = createSelector(
    getTranslationLanguagesResult,
    result => result.get('result') || List()
)

export const getIsFetching = (apiCallSelector) => createSelector(
  apiCallSelector,
  result => result.get('isFetching') || false
)

export const getTranslationLanguageIsFetching = getIsFetching(getTranslationLanguagesResult)

export const getTranslationLanguageAreDataValid = createSelector(
    getTranslationLanguagesResult,
    result => result.get('areDataValid') || false
)

export const getTranslationLanguageMap = createSelector(
    [getTranslationLanguages, getParameters],
    (translationLanguages, code) => translationLanguages.filter(tL => tL.get('code').toLowerCase() === code).first() || Map()
)

export const getFiLanguage = createSelector(
    getTranslationLanguages,
    translationLanguages => translationLanguages.filter(tL => tL.get('code').toLowerCase() === 'fi').first() || Map()
)

export const getDefaultLanguage = createSelector(
    [getTranslationLanguages, getParameterFromProps('language')],
    (translationLanguages, code) => translationLanguages.filter(tL => tL.get('code').toLowerCase() === code).first() || Map()
)

export const getDefaultLanguageId = createSelector(
    [getDefaultLanguage, getFiLanguage],
    (defaultLanguage, getFiLanguage) => defaultLanguage.get('id') || getFiLanguage.get('id') || ''
)

export const getTranslationLanguageId = createSelector(
    getTranslationLanguageMap,
    translationLanguages => translationLanguages.get('id') || ''
)

export const getTranslationLanguageName = createSelector(
    getTranslationLanguageMap,
    translationLanguages => translationLanguages.get('name') || ''
)

export const getTranslationLanguage = createSelector(
    [getTranslationLanguages, getParameterFromProps('id')],
    (translationLanguages, id) => translationLanguages.get(id) || Map()
)

export const getTranslationLanguageCode = createSelector(
    getTranslationLanguage,
    language => language.get('code') || 'fi'
)

export const getTranslationLanguagesObjectArray = createSelector(
    [getTranslationLanguages, getTranslationLanguageList],
    (languages, ids) => getObjectArray(ids.map(id => languages.get(id)))
)

export const getLanguagesJS = createSelector(
    getLanguages,
    languages => getJS(languages, [])
)

export const getPhoneNumberTypes = createSelector(
    getEntities,
    entities => entities.get('phoneNumberTypes') || List()
)

export const getPhoneNumberTypeMap = createSelector(
    [getPhoneNumberTypes, getParameters],
    (phoneNumberTypes, code) => phoneNumberTypes.filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
)

export const getPhoneNumberTypeId = createSelector(
    getPhoneNumberTypeMap,
    phoneNumberTypes => phoneNumberTypes.get('id') || ''
)

export const getWebPageTypes = createSelector(
    getEntities,
    entities => entities.get('webPageTypes') || List()
)

export const getWebPageTypesList = createSelector(
    getEnums,
    entities => entities.get('webPageTypes') || List()
)

export const getWebPageTypesObjectArray = createSelector(
    [getWebPageTypes, getWebPageTypesList],
    (webPageTypes, ids) => getEntitiesForIdsJS(webPageTypes, ids)
)

export const getPrintableFormUrlTypes = createSelector(
    getEntities,
    entities => entities.get('printableFormUrlTypes') || List()
)

export const getPrintableFormUrlTypesList = createSelector(
    getEnums,
    entities => entities.get('printableFormUrlTypes') || List()
)

export const getPrintableFormUrlTypesObjectArray = createSelector(
    [getPrintableFormUrlTypes, getPrintableFormUrlTypesList],
    (webPageTypes, ids) => getEntitiesForIdsJS(webPageTypes, ids)
)

export const getPhoneNumberTypesJS = createSelector(
    getPhoneNumberTypes,
    phoneNumberTypes => getJS(phoneNumberTypes, [])
)

export const getPhoneNumberTypesObjectArray = createSelector(
    getPhoneNumberTypes,
    phoneNumberTypes => getObjectArray(phoneNumberTypes)
)

export const getChannelTypes = createSelector(
    getEntities,
    entities => entities.get('channelTypes') || List()
)

export const getChannelType = createSelector(
    [getChannelTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
)

export const geChannelTypeNameForId = createSelector(
    getChannelType,
    channelType => channelType.get('name')
)

export const getChannelTypesList = createSelector(
    getEnums,
    entities => entities.get('channelTypes') || List()
)

export const getChannelTypesObjectArray = createSelector(
    [getChannelTypes, getChannelTypesList],
    (channelTypes, ids) => getObjectArray(ids.map(id => channelTypes.get(id)))
)

export const getChannelTypesJS = createSelector(
    getChannelTypes,
    channelTypes => getJS(channelTypes, [])
)

export const getPublishingStatusesImmutableList = createSelector(
    getPublishingStatuses,
    publishingStatuses => getList(publishingStatuses)
)

export const getPublishingStatusesJS = createSelector(
    getPublishingStatuses,
    publishingStatuses => getJS(publishingStatuses, [])
)

export const getNotifications = createSelector(
    getCommon,
    common => common.get('notifications') || Map()
)

export const getNotificationsByKey = createSelector(
    [getNotifications, getParameterFromProps('keyToState'), getParameterFromProps('notificationKey')],
    (notification, keyToState, notificationKey) =>
        notification.getIn([keyToState, notificationKey || 'all']) || Map()
)

export const getErrors = createSelector(
    getNotificationsByKey,
    notifications => notifications.get('errors') || List()
)

export const getErrorMessages = createSelector(
    getErrors,
    errors => getArray(errors.map(x => ({ message: x.get('error'), info: x.get('stack') })))
)

export const getInfos = createSelector(
    getNotificationsByKey,
    notifications => getArray(notifications.get('infos'))
)

export const getPageModeStateForKey = createSelector(
    [getPageModeState, getParameterFromProps('keyToState')],
    (state, key) => state.get(key) || Map()
)

export const getPageModeStateData = createSelector(
    getPageModeState,
    state => state.get('data') || Map()
)

export const getPageEntityId = createSelector(
    getPageModeStateForKey,
    state => getEntityId(state)
)

export const getForceReload = createSelector(
    getPageModeStateForKey,
    state => state.get('forceReload') || false
)

export const getPageEntity = createSelector(
    [getEntitiesWithGivenType, getPageEntityId],
    (entities, id) => entities.get(id) || Map()
)

export const getPublsihingStatusIdOfEntity = createSelector(
    getPageEntity,
    entity => entity.get('publishingStatusId') || ''
)

export const getPageEntityLanguageCode = createSelector(
    getPageModeStateForKey,
    state => getPageLanguageCode(state)
)

export const getPageEntityIdNotGenerated = createSelector(
    getPageModeStateForKey,
    state => getEntityId(state, false)
)

export const getIsDirty = createSelector(
    getPageModeStateForKey,
    search => search.get('isDirty') || false
)

export const getLanguageFrom = createSelector(
    getPageModeStateForKey,
    search => search.get('languageFrom') || null
)

export const getLanguageTo = createSelector(
    [getPageModeStateForKey, getDefaultLanguageId],
    (search, defaultLanguage) => search.get('languageTo') || defaultLanguage
)

export const getToLanguage = createSelector(
    [getTranslationLanguages, getLanguageTo],
    (translationLanguages, id) => translationLanguages.get(id) || Map()
)

export const getLanguageToCode = createSelector(
    getToLanguage,
    language => language.get('code') || 'fi'
)

export const getFromLanguage = createSelector(
    [getTranslationLanguages, getLanguageFrom],
    (translationLanguages, id) => translationLanguages.get(id) || Map()
)

export const getLanguageFromCode = createSelector(
    getFromLanguage,
    language => language.get('code') || 'fi'
)

export const getLanguageToName = createSelector(
    getToLanguage,
    language => language.get('name') || ''
)

export const getApiEntityId = (state, entityId) => entityId || ''

// related url attachments

export const getUrlAttachmentsApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('urlAttachments') || Map()
)

export const getUrlAttachmentsApiCall = createSelector(
    [getUrlAttachmentsApiCalls, getParameterFromProps('id')],
    (urlAttachmentsApiCalls, id) => urlAttachmentsApiCalls.get(id) || Map()
)

export const getUrlAttachmentIsFetching = createSelector(
    getUrlAttachmentsApiCall,
    urlAttachmentApiCall => urlAttachmentApiCall.get('isFetching') || false
)

export const getUrlAttachmentAreDataValid = createSelector(
    getUrlAttachmentsApiCall,
    urlAttachmentApiCall => urlAttachmentApiCall.get('areDataValid') || false
)

// related emails
export const getEmails = createSelector(
    getEntities,
    entities => entities.get('emails') || Map()
)

export const getEmail = createSelector(
    [getEmails, getApiEntityId],
    (emails, emailId) => emails.get(emailId) || Map()
)

export const getEmailEmail = createSelector(
    getEmail,
    email => email.get('email') || ''
)

export const getEmailAdditionalInformation = createSelector(
    getEmail,
    email => email.get('additionalInformation') || ''
)

// related phoneNumbers
export const getPhoneNumbers = createSelector(
    getEntities,
    entities => entities.get('phoneNumbers') || Map()
)

export const getPhoneNumber = createSelector(
    [getPhoneNumbers, getParameterFromProps('phoneId')],
    (phoneNumbers, phoneNumberId) => phoneNumbers.get(phoneNumberId) || Map()
)

export const getPhoneNumberChargeTypeId = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('chargeTypeId') || null
)

export const getPhoneNumberChargeDescription = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('chargeDescription') || ''
)

export const getPhoneNumberPhoneNumber = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('number') || ''
)

export const getPhoneNumberAdditionalInformation = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('additionalInformation') || ''
)

export const getPhoneNumberPrefixNumber = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('prefixNumber') || ''
)
export const getPhoneNumberType = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('typeId') || null
)
export const getIsFinnishServiceNumber = createSelector(
    getPhoneNumber,
    phoneNumber => phoneNumber.get('isFinnishServiceNumber') || false
)

// related dialCodes
export const getDialCodes = createSelector(
    getEntities,
    entities => entities.get('dialCodes') || Map()
)

export const getSelectedPrefixNumber = createSelector(
    [getDialCodes, getPhoneNumberPrefixNumber],
    (dialCodes, prefixNumberId) => dialCodes.get(prefixNumberId)
)

export const getSelectedPrefixNumberJS = createSelector(
    [getSelectedPrefixNumber],
    (prefixNumber) => getJS(prefixNumber)
)

// related webpages
export const getWebPages = createSelector(
    getEntities,
    entities => entities.get('webPages') || Map()
)

export const getWebPagesApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('webPages') || Map()
)

export const getWebPage = createSelector(
    [getWebPages, getIdFromProps],
    (webPages, webPageId) => webPages.get(webPageId) || Map()
)

export const getWebPageApiCall = createSelector(
    [getWebPagesApiCalls, getIdFromProps],
    (webPagesApiCalls, webPageId) => webPagesApiCalls.get(webPageId) || Map()
)

export const getWebPageIsFetching = createSelector(
    getWebPageApiCall,
    webPageApiCall => webPageApiCall.get('isFetching') || false
)

export const getWebPageAreDataValid = createSelector(
    getWebPageApiCall,
    webPageApiCall => webPageApiCall.get('areDataValid') || false
)

export const getWebPageTypeId = createSelector(
    getWebPage,
    webPage => webPage.get('typeId') || ''
)

export const getWebPageUrlAddress = createSelector(
    getWebPage,
    webPage => webPage.get('urlAddress') || ''
)

export const getWebPageName = createSelector(
    getWebPage,
    webPage => webPage.get('name') || ''
)

export const getWebPageOrderNumber = createSelector(
    getWebPage,
    webPage => webPage.get('orderNumber') || ''
)

export const getWebPageUrlExists = createSelector(
    getWebPage,
    webPage => typeof webPage.get('urlExists') === 'boolean' ? webPage.get('urlExists') : null
)

export const getWebPageAdditionalInformation = createSelector(
    getWebPage,
    webPage => webPage.get('additionalInformation') || ''
)


// related vouchers
export const getServiceVouchers = createSelector(
    getEntities,
    entities => entities.get('serviceVouchers') || Map()
)

export const getServiceVouchersApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('serviceVouchers') || Map()
)

export const getServiceVoucher = createSelector(
    [getServiceVouchers, getIdFromProps],
    (serviceVouchers, serviceVoucherId) => serviceVouchers.get(serviceVoucherId) || Map()
)

export const getServiceVoucherApiCall = createSelector(
    [getServiceVouchersApiCalls, getIdFromProps],
    (serviceVouchersApiCalls, serviceVoucherId) => serviceVouchersApiCalls.get(serviceVoucherId) || Map()
)

export const getServiceVoucherIsFetching = createSelector(
    getServiceVoucherApiCall,
    serviceVoucherApiCall => serviceVoucherApiCall.get('isFetching') || false
)

export const getServiceVoucherAreDataValid = createSelector(
    getServiceVoucherApiCall,
    serviceVoucherApiCall => serviceVoucherApiCall.get('areDataValid') || false
)

export const getServiceVoucherTypeId = createSelector(
    getServiceVoucher,
    serviceVoucher => serviceVoucher.get('typeId') || ''
)

export const getServiceVoucherUrlAddress = createSelector(
    getServiceVoucher,
    serviceVoucher => serviceVoucher.get('urlAddress') || ''
)

export const getServiceVoucherName = createSelector(
    getServiceVoucher,
    serviceVoucher => serviceVoucher.get('name') || ''
)

export const getServiceVoucherOrderNumber = createSelector(
    getServiceVoucher,
    serviceVoucher => serviceVoucher.get('orderNumber') || ''
)

export const getServiceVoucherUrlExists = createSelector(
    getServiceVoucher,
    serviceVoucher => typeof serviceVoucher.get('urlExists') === 'boolean' ? serviceVoucher.get('urlExists') : null
)

export const getServiceVoucherAdditionalInformation = createSelector(
    getServiceVoucher,
    serviceVoucher => serviceVoucher.get('additionalInformation') || ''
)

// related laws
export const getLaws = createSelector(
    getEntities,
    entities => entities.get('laws') || Map()
)

export const getLawApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('laws') || Map()
)

export const getLaw = createSelector(
    [getLaws, getIdFromProps],
    (laws, lawId) => laws.get(lawId) || Map()
)

export const getLawApiCall = createSelector(
    [getLawApiCalls, getIdFromProps],
    (lawsApiCalls, lawId) => lawsApiCalls.get(lawId) || Map()
)

export const getLawIsFetching = createSelector(
    getLawApiCall,
    lawsApiCalls => lawsApiCalls.get('isFetching') || false
)

export const getLawAreDataValid = createSelector(
    getLawApiCall,
    lawsApiCalls => lawsApiCalls.get('areDataValid') || false
)

const getLawLocalizableValue = (propertyName) => createSelector(
  [getLaw, getLanguageParameter],
  (law, language) => {
    return law.getIn([propertyName, language])
  }
)

const getWithDefaultValue = (valueSelector, defaultValue) => createSelector(
  valueSelector,
  value => value || defaultValue
)

export const getLawUrlAddress = getWithDefaultValue(getLawLocalizableValue('urlAddress'), '')

export const getLawName = getWithDefaultValue(getLawLocalizableValue('name'), '')

const getLawUrlExistsValue = getLawLocalizableValue('urlExists')

export const getLawUrlExists = createSelector(
    getLawUrlExistsValue,
    urlExists => typeof urlExists === 'boolean' ? urlExists : null
)

// related addresses
export const getAddressEntities = createSelector(
    getEntities,
    entities => entities.get('addresses') || Map()
)

export const getAddresses = createSelector(
    [getAddressEntities, getLanguageParameter],
    (addresses, language) => addresses.map((address) => address.get(language))
)

export const getFromAddresses = createSelector(
    [getAddressEntities, getLanguageFromCode],
    (addresses, language) => addresses.map((address) => address.get(language))
)

export const getAddressesForModel = createSelector(
    [getAddressEntities, getLanguageParameter, getCoordinates],
    (addresses, language, coordinates) => addresses.map((address) => {
      const concreteAddress = address.get(language)
      return concreteAddress ? concreteAddress.set('coordinates', getEntitiesForIds(coordinates, concreteAddress.get('coordinates'))) : Map()
    })
)

export const getAddress = createSelector(
    [getAddresses, getIdFromProps],
    (addresses, addressId) => addresses.get(addressId) || Map()
)

export const getFromAddress = createSelector(
    [getFromAddresses, getIdFromProps],
    (addresses, addressId) => addresses.get(addressId) || Map()
)

export const getAddressStreetType = createSelector(
    getAddress,
    address => address.get('streetType') || 'Street'
)

export const getAddressStreet = createSelector(
    getAddress,
    (address) => address.get('street') || ''
)

export const getAddressPOBox = createSelector(
    getAddress,
    (address) => address.get('poBox') || ''
)

export const getAddressStreetNumber = createSelector(
    getAddress,
    address => address.get('streetNumber') || ''
)

export const getForeignAddress = createSelector(
    getAddress,
    address => address.get('foreignAddress') || ''
)

export const getIsAddressValid = createSelector(
    getAddress,
    address => address.get('addressIsValid') !== false
)

export const getAddressCoordinates = createSelector(
    getAddress,
    address => address.get('coordinates') || List()
)

export const getCoordinatesForAddressId = createSelector(
    [getAddressCoordinates, getCoordinates],
    (coordinatesIds, coordinates) => getEntitiesForIds(coordinates, coordinatesIds, List())
)

export const getMainCoordinatesForAddressId = createSelector(
    getCoordinatesForAddressId,
    coordinates => coordinates.filter(c => c.get('isMain')) || Map()
)

export const getMainCoordinateForAddressId = createSelector(
    getMainCoordinatesForAddressId,
    mainCoordinates => mainCoordinates.first() || Map()
)

export const getMainCoordinateIdForAddressId = createSelector(
    getMainCoordinateForAddressId,
    mainCoordinate => mainCoordinate.get('id') || ''
)

export const getAddressStreetLongitude = createSelector(
    getMainCoordinateForAddressId,
    mainCoordinate => mainCoordinate.get('longtitude') || null
)

export const getAddressStreetLatitude = createSelector(
    getMainCoordinateForAddressId,
    mainCoordinate => mainCoordinate.get('latitude') || null
)

export const getAddressStreetCoordinatesStateSelector = createSelector(
    getMainCoordinateForAddressId,
    mainCoordinate => mainCoordinate.get('coordinateState') || ''
)

export const getAddressStreetCoordinatesState = createSelector(
    getAddressStreetCoordinatesStateSelector,
    coordinateState => coordinateState.toLowerCase()
)

export const getAddressPostalCode = createSelector(
    getAddress,
    address => address.get('postalCode') || ''
)

export const getAddressAditionalInformation = createSelector(
    getAddress,
     (address) => address.get('additionalInformation') || ''
)

export const getAddressOtherAditionalInformation = createSelector(
    getAddress,
    address => address.get('otherAdditionalInformation') || ''
)

export const getAddressCountryId = createSelector(
    getAddress,
     (address) => address.get('country') || ''
)

export const getAddressesApiCalls = createSelector(
    getApiCalls,
    appiCalls => appiCalls.get('addresses') || Map()
)

export const getAddressesApiCall = createSelector(
    [getAddressesApiCalls, getParameterFromProps('id')],
    (addressesApiCalls, id) => {
      return addressesApiCalls.get(id) || Map()
    }
)

export const getAddressIsFetching = createSelector(
    getAddressesApiCall,
    addressApiCall => addressApiCall.get('isFetching') || false
)

export const getAttemptCount = createSelector(
    getAddressesApiCall,
    addressApiCall => addressApiCall.get('attemptCount') || 0
)

// related countries
export const getCountries = createSelector(
    getEntities,
    entities => entities.get('countries') || Map()
)

export const getSelectedCountry = createSelector(
    [getCountries, getAddressCountryId],
    (countries, countryId) => countries.get(countryId)
)

export const getSelectedCountryJS = createSelector(
    [getSelectedCountry],
    (country) => getJS(country)
)

// related postalCodes
export const getPostalCodes = createSelector(
    getEntities,
    entities => entities.get('postalCodes') || Map()
)

export const getSelectedPostalCode = createSelector(
    [getPostalCodes, getAddressPostalCode],
    (postalCodes, postalCodeId) => postalCodes.get(postalCodeId) || Map()
)

export const getPostalCode = createSelector(
    getSelectedPostalCode,
    postalCode => getMap(postalCode)
)

export const getPostalPostOffice = createSelector(
    getPostalCode,
    postalCode => postalCode.get('postOffice') || ''
)

export const getDefaultMunicipality = createSelector(
    getPostalCode,
    postalCode => postalCode ? postalCode.get('municipalityId') : ''
)

export const getSelectedMunicipalityId = createSelector(
    [getAddress, getDefaultMunicipality],
    (address, municipalityId) => address.get('municipalityId') || municipalityId || ''
)

export const getSelectedMunicipality = createSelector(
    [getMunicipalities, getSelectedMunicipalityId],
    (municipalities, selectedMunicipalityId) => municipalities.get(selectedMunicipalityId) || Map()
)

export const getAddressInfo = createSelector(
  [getAddressStreet,
    getAddressStreetNumber,
    getSelectedPostalCode,
    getSelectedMunicipality],
    (streetName, streetNumber, postalCode, municipality) => ({
      streetName,
      streetNumber,
      postalCode,
      municipality })
)

// related address
export const getIsAddressFilled = createSelector(
    [getAddressStreet, getAddressStreetNumber, getSelectedPostalCode],
    (street, streetNumber, postalCode) => street && streetNumber && postalCode.size > 0
)

export const getIsAddressFilledMinimum = createSelector(
    [getAddressStreet, getSelectedPostalCode],
    (street, postalCode) => street && postalCode.size > 0
)

export const getAddressInfoForCoordinates = createSelector(
  [getMainCoordinateIdForAddressId, getAddressStreet,
    getAddressStreetNumber,
    getSelectedMunicipality],
    (mainCoordinateId, streetName, streetNumber, municipality) => ({
      mainCoordinateId,
      streetName,
      streetNumber,
      municipalityCode: municipality.get('code') })
)

// related business
export const getBusinessArray = createSelector(
    getEntities,
    entities => entities.get('business') || Map()
)

export const getBusiness = createSelector(
    [getBusinessArray, getApiEntityId],
    (business, businessId) => business.get(businessId) || Map()
)

export const getBusinessCode = createSelector(
    getBusiness,
    business => business.get('code') || ''
)

// related to all steps
export const getApiCallForType = createSelector(
    [getApiCalls, getParameterFromProps('keyToState')],
    (apiCalls, type) => (Array.isArray(type) ? apiCalls.getIn(type) : apiCalls.get(type)) || new Map()
)

export const getSearchModel = createSelector(
    getApiCallForType,
    search => search.get('search') || new Map()
)

export const getSearchResultsModel = createSelector(
    getApiCallForType,
    search => search.get('searchResults') || new Map()
)

export const getStep1Model = createSelector(
    getApiCallForType,
    search => search.get('step1Form') || new Map()
)

export const getStep2Model = createSelector(
    getApiCallForType,
    search => search.get('step2Form') || new Map()
)

export const getStep3Model = createSelector(
    getApiCallForType,
    search => search.get('step3Form') || new Map()
)

export const getStep4Model = createSelector(
    getApiCallForType,
    search => search.get('step4Form') || new Map()
)

export const getChannelServiceStepModel = createSelector(
    getApiCallForType,
    search => search.get('channelServiceStep') || new Map()
)

export const getStepCommonModel = createSelector(
    getApiCallForType,
    search => search.get('all') || new Map()
)

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
)

export const getEditableModel = createSelector(
    getApiCallForType,
    search => search.get('editable') || new Map()
)

export const getDeleteModel = createSelector(
    getApiCallForType,
    search => search.get('delete') || new Map()
)

export const getAnnotationModel = createSelector(
    getApiCallForType,
    search => search.get('annotation') || new Map()
)

export const getAreaInformationModel = createSelector(
    getApiCallForType,
    search => search.get('areaInformation') || new Map()
)

export const getStepInnerSearchModel = createSelector(
    getApiCallForType,
    search => search.get('innerSearch') || new Map()
)

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
)

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
    [getSearchResultsModel, getParameterFromProps('keyToEntities')],
    (searchModel, keyToEntities) => searchModel.get(keyToEntities) || List()
)

export const getSearchedPrevEntities = createSelector(
    getSearchResultsModel,
    (searchModel) => searchModel.get('prevEntities') || List()
)

export const getSearchedEntities = createSelector(
    [getSearchedNewEntities, getSearchedPrevEntities],
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

export const getEditableIsFetching = createSelector(
    getEditableModel,
    editableModel => editableModel.get('isFetching') || false
)

export const getEditableAreDataValid = createSelector(
    getEditableModel,
    editableModel => editableModel.get('areDataValid') || false
)

export const getEditableEntityId = createSelector(
    getEditableModel,
    editableModel => editableModel.get('id') || Map()
)

export const getEditableIsEditable = createSelector(
    getEditableModel,
    editableModel => editableModel.get('isEditable') !== false
)

export const getEditableLastPublished = createSelector(
    getEditableModel,
    editableModel => editableModel.get('lastPublishedId') || null
)

export const getEditableLastModified = createSelector(
    getEditableModel,
    editableModel => editableModel.get('lastModifiedId') || null
)

export const getDeleteIsFetching = createSelector(
    getDeleteModel,
    deleteModel => deleteModel.get('isFetching') || false
)

export const getDeleteAreDataValid = createSelector(
    getDeleteModel,
    deleteModel => deleteModel.get('areDataValid') || false
)

export const getDeleteAnyConnected = createSelector(
    getDeleteModel,
    deleteModel => deleteModel.get('anyConnected') || false
)

export const getDeleteSubOrganizationsConnected = createSelector(
    getDeleteModel,
    deleteModel => deleteModel.get('subOrganizationsConnected') || false
)




export const getAnnotationIsFetching = createSelector(
    getAnnotationModel,
    annotationModel => annotationModel.get('isFetching') || false
)

export const getAreaInformationIsFetching = createSelector(
    getAreaInformationModel,
    areaInformationModel => areaInformationModel.get('isFetching') || false
)

export const getIsFetchingOfAnyStep = createSelector(
    [getDeleteIsFetching, getEditableIsFetching, getLockIsFetching, getStepCommonIsFetching, getStep1isFetching, getStep2isFetching, getStep3isFetching, getStep4isFetching],
    (deleting, editable, lock, common, step1, step2, step3, step4) => deleting || editable || lock || common || step1 || step2 || step3 || step4
)

export const getStep1AreDataValid = createSelector(
    [getStep1Model, getPageEntityId, getPageEntityLanguageCode, getParameterFromProps('simpleView')],
    (stepModel, id, languageCode, simpleView) => areStepDataValid(stepModel, id, simpleView ? '' : languageCode) || false
)

export const getStep2AreDataValid = createSelector(
    [getStep2Model, getPageEntityId, getPageEntityLanguageCode, getParameterFromProps('simpleView')],
    (stepModel, id, languageCode, simpleView) => areStepDataValid(stepModel, id, simpleView ? '' : languageCode) || false
)

export const getStep3AreDataValid = createSelector(
    [getStep3Model, getPageEntityId, getPageEntityLanguageCode, getParameterFromProps('simpleView')],
    (stepModel, id, languageCode, simpleView) => areStepDataValid(stepModel, id, simpleView ? '' : languageCode) || false
)

export const getStep4AreDataValid = createSelector(
    [getStep4Model, getPageEntityId, getPageEntityLanguageCode, getParameterFromProps('simpleView')],
    (stepModel, id, languageCode, simpleView) => areStepDataValid(stepModel, id, simpleView ? '' : languageCode) || false
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
)

export const getNodeInfo = createSelector(
    [getNodes, getParameterFromProps('id'), getParameterFromProps('contextId')],
    (nodes, id, contextId) => (contextId ? nodes.getIn([contextId, id]) : nodes.get(id)) || Map()
)

export const getNodeIsCollapsed = createSelector(
    getNodeInfo,
    node => node.get('isCollapsed')
)

export const getNodeApiCall = createSelector(
    [getApiCalls, getParameterFromProps('id')],
    (apiCalls, id) => apiCalls.getIn(['nodes', id]) || Map()
)
export const getNodeIsFetching = createSelector(
    getNodeApiCall,
    node => node.get('isFetching') || false
)

export const getNodeSearchItem = createSelector(
    getSearchModel,
    search => search.get('id') ? Map({ id: search.get('id'), name: search.get('searchValue') }) : null
)

export const getNodeSearchItemJS = createSelector(
    getNodeSearchItem,
    search => getJS(search)
)

export const getSearchDomain = createSelector(
  getPageModeState,
  pageModeState => pageModeState.get('searchDomain')
)

export const getAreaInformationTypes = createSelector(
    getEntities,
    entities => entities.get('areaInformationTypes') || Map()
)

export const getAreaInformationType = createSelector(
    [getAreaInformationTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
)

export const getAreaInformationTypeNameForId = createSelector(
    getAreaInformationType,
    areaInformationType => areaInformationType.get('name')
)

export const getAreaInformationTypesObjectArray = createSelector(
    getAreaInformationTypes,
    areaInformationType => getObjectArray(areaInformationType)
)

export const getAreaInformationTypesJS = createSelector(
    getAreaInformationTypes,
    areaInformationType => getJS(areaInformationType, [])
)

export const getAreaInformationTypeMap = createSelector(
    [getAreaInformationTypes, getParameters],
    (areaInformationTypes, code) => {
      const codeDefault = typeof code === 'string' ? code : 'wholecountry'
      return areaInformationTypes
        .filter(chT => chT.get('code').toLowerCase() === codeDefault).first() || Map()
    }
)
export const getDefaultAreaInformationTypeSelector = codeDefault => createSelector(
    getAreaInformationTypes,
    areaInformationTypes => {
      return areaInformationTypes
        .filter(chT => chT.get('code').toLowerCase() === codeDefault).first() || Map()
    }
)
export const getAreaTypeSelector = code => createSelector(
    getAreaTypes,
    areaTypes => {
      return areaTypes
        .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
    }
)

export const getIdSelector = entitySelector => createSelector(
    entitySelector,
    entity => entity.get('id') || null
)
export const getAreaInformationTypeId = createSelector(
    getAreaInformationTypeMap,
    areaInformationTypes => areaInformationTypes.get('id') || null
)
export const getAreaTypes = createSelector(
    getEntities,
    entities => entities.get('areaTypes') || Map()
)

export const getAreaType = createSelector(
    [getAreaTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
)

export const getAreaTypeCodeForId = createSelector(
    getAreaType,
    areaType => (areaType.get('code') && areaType.get('code').toLowerCase()) || null
)

export const getAreaTypesObjectArray = createSelector(
    getAreaTypes,
    areaType => getObjectArray(areaType)
)

export const getAreaTypesJS = createSelector(
    getAreaTypes,
    areaType => getJS(areaType, [])
)

export const getAreaTypeMap = createSelector(
    [getAreaTypes, getParameters],
    (areaTypes, code) => areaTypes
        .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
)
export const getAreaTypeId = createSelector(
    getAreaTypeMap,
    areaTypes => areaTypes.get('id') || ''
)

export const getArea = createSelector(
    [getProvincies, getBusinessRegions, getHospitalRegions, getIdFromProps],
    (provincies, businessRegions, hospitalRegions, id) =>
        provincies.get(id) || businessRegions.get(id) || hospitalRegions.get(id) || Map()
)

export const getAreaReferenceItems = createSelector(
    getArea,
    (area) => area.get('referenceItems') || Map()
)

export const getAreaMunicipalities = createSelector(
    [getMunicipalities, getAreaReferenceItems],
    (municipalities, ids) => getEntitiesForIdsJS(municipalities, ids)
)

// DefaultValues
export const getDefaultDialCodeEnumIds = createSelector(
    getEnums,
    entities => entities.get('dialCodes') || List()
)

export const getDefaultDialCodeEnumId = createSelector(
    getDefaultDialCodeEnumIds,
    ids => ids && ids.size > 0 ? ids.first() : null
)

export const getDefaultDialCode = createSelector(
    getEntities,
    entities => entities.get('dialCodes') || List()
)

export const getDefaultDialCodeEntities = createSelector(
    [getDefaultDialCode, getDefaultDialCodeEnumIds],
    (entities, ids) => getEntitiesForIds(entities, ids)
)

export const getDefaultDialCodeEntity = createSelector(
    getDefaultDialCodeEntities,
    entities => entities && entities.size > 0 ? entities.first() : Map()
)

export const getDefaultDialCodeEntityJS = createSelector(
    getDefaultDialCodeEntity,
    entity => getJS(entity)
)

export const getIsDefaultDialCodeSelected = createSelector(
    [getDefaultDialCodeEnumId, getPhoneNumberPrefixNumber],
    (defaultId, selectedId) => defaultId === selectedId
)

export const getFundingTypes = createSelector(
    getEntities,
    entities => entities.get('fundingTypes') || Map()
)

export const getFundingTypesObjectArray = createSelector(
    getFundingTypes,
    fundingType => getObjectArray(fundingType)
)

export const getFundingTypesJS = createSelector(
    getFundingTypes,
    fundingType => getJS(fundingType, [])
)

