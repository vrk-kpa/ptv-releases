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
import shortId from 'shortid';
import { Map, List } from 'immutable';
import * as CommonSelectors from  '../../../Common/Selectors';
import { addressTypes } from '../../../Common/Helpers/types';
import { weekdaysEnum } from '../../../Common/Enums';

export const getChannelsEntities = createSelector(
    CommonSelectors.getEntities,
    search => search.get('channels') || new Map()
);

export const getChannels = createSelector(
    [getChannelsEntities, CommonSelectors.getLanguageParameter],
    (channels, language) => channels.map((channel) => channel.get(language))
);

export const getUrls = createSelector(
    CommonSelectors.getEntities,
    search => search.get('urls') || new Map()
);

export const getCommonReducer = createSelector(
    CommonSelectors.getChannelReducers,
    channels => channels.get('commonReducers')
)

export const getChannelType = CommonSelectors.getParameterFromProps('keyToState')


export const getChannelCommonReducer = createSelector(
    [getCommonReducer, getChannelType],
    (commonReducer, channelType) => commonReducer.get(channelType) || Map()
);

export const getChannelId = createSelector(
    // getChannelCommonReducer,
    CommonSelectors.getPageEntityId,
    id => id
);

// channels related

export const getChannel = createSelector(
    [getChannels, getChannelId],
    (channels, channelId) => channels.get(channelId) || Map()
);

export const getPublishingStatus = createSelector(
    getChannel,
    channel => channel.get('publishingStatus') || Map()
);

export const getDescriptionMap = createSelector(
    getChannel,
    channel => channel//.get('description') || Map()
);

export const getFormIdentifier = createSelector(
    getChannel,
    channel => channel.get('formIdentifier') || ''
);

export const getFormReceiver = createSelector(
    getChannel,
    channel => channel.get('formReceiver') || ''
);

export const getChannelName = createSelector(
    getDescriptionMap,
    description => description.get('name') || ''
);

export const getShortDescription = createSelector(
    getDescriptionMap,
    description => description.get('shortDescription') || ''
);

export const getOrganizationId = createSelector(
    getDescriptionMap,
    description => description.get('organizationId') || ''
);

export const getDescription = createSelector(
    getDescriptionMap,
    description => description.get('description') || ''
);

export const getIsOnLineAuthentication = createSelector(
    getChannel,
    channel => channel.get('isOnLineAuthentication') || false
);

export const getIsRestrictedRegion = createSelector(
    getChannel,
    channel => channel.get('isRestrictedRegion') || false
);

export const getNumberOfConnectedServices = createSelector(
    getChannel,
    channel => channel.get('connectedServices') || 0
);

export const getIsOnLineSign = createSelector(
    getChannel,
    channel => channel.get('isOnLineSign') || false
);

export const getNumberOfSigns = createSelector(
    getChannel,
    channel => channel.get('numberOfSigns') || null
);

export const getNumberOfSignsString = createSelector(
    getNumberOfSigns,
    numberOfSigns => numberOfSigns ? numberOfSigns.toString() : null,   
);

export const getEmail = createSelector(
    getChannel,
    channel => channel.get('email') || null
);

export const getSelectedEmailEntity = createSelector(
    [getEmail, CommonSelectors.getEmails],
    (id, emails) => emails.get(id) || null
);

export const getFax = createSelector(
    getChannel,
    channel => channel.get('fax') || null
);

export const getFaxEntity = createSelector (
    [CommonSelectors.getPhoneNumbers, getFax],
    (entities, id) => entities.get(id) || null
)

// urlAttachments

export const getUrlAttachments = createSelector(
    getChannel,
    channel => channel.get('urlAttachments') || List()
);

export const getChannelUrlAttachmentsEntities = createSelector(
    CommonSelectors.getEntities,
    channel => channel.get('urlAttachments') || List()
);

export const getChannelUrlAttachments = createSelector(
    [getChannelUrlAttachmentsEntities, CommonSelectors.getLanguageParameter],
    (attachments, language) => attachments.map((attachment) => attachment.get(language))
);

export const getSelectedChannelUrlAttachmentsEntity = createSelector(
    [getChannelUrlAttachments, getUrlAttachments],
    (entities, selectedUrlAttachments) => CommonSelectors.getEntitiesForIds(entities, selectedUrlAttachments) || List()
);

export const getChannelUrlAttachment = createSelector(
    [getChannelUrlAttachments, CommonSelectors.getIdFromProps],
    (channelUrlAttachments, channelUrlAttachmentId) => channelUrlAttachments.get(channelUrlAttachmentId) || Map()
);

export const getChannelUrlAttachmentName = createSelector(
    getChannelUrlAttachment,
    channelUrlAttachment => channelUrlAttachment.get('name') || ''
);

export const getChannelUrlAttachmentDescription = createSelector(
    getChannelUrlAttachment,
    channelUrlAttachment => channelUrlAttachment.get('description') || ''
);

export const getChannelUrlAttachmentUrlAddress = createSelector(
    getChannelUrlAttachment,
    channelUrlAttachment => channelUrlAttachment.get('urlAddress') || ''
);

export const getChannelUrlAttachmentUrlExists = createSelector(
    getChannelUrlAttachment,
    channelUrlAttachment => channelUrlAttachment.get('urlExists') || ''
);

export const getUrlMap = createSelector(
    getChannel,
    channel => channel.get('webPage') || Map()
);

export const getUrlAddress = createSelector(
    getUrlMap,
    url => url.get('urlAddress') || ''
);

export const getUrlName = createSelector(
    getUrlMap,
    url => url.get('name') || ''
);

export const getUrlDescription = createSelector(
    getUrlMap,
    url => url.get('description') || ''
);

export const getUrlId = createSelector(
    getUrlMap,
    url => url.get('id') || shortId.generate()
);

export const getUrlResult = createSelector(
    CommonSelectors.getApiCallForType,
    url => url.get('webPage') || Map()
);

export const getUrlIsFetching = createSelector(
    getUrlResult,
    url => url.get('isFetching') || false
);

export const getUrlAreDataValid = createSelector(
    getUrlResult,
    url => url.get('areDataValid') || false
);

export const getUrlExists = createSelector(
    getUrlResult,
    url => typeof url.get('urlExists') === 'boolean' ? url.get('urlExists') : null
);

export const getSelectedWebPages = createSelector(
    getChannel,
    channel => channel.get('webPages') || List()
);

export const getSelectedWebPagesEntities = createSelector(
    [CommonSelectors.getWebPages, getSelectedWebPages],
    (entities, selectedWebPages) => CommonSelectors.getEntitiesForIds(entities, selectedWebPages) || List()
);

export const getPhoneNumber = createSelector(
    getChannel,
    channel => channel.get('phoneNumber') || null
);

export const getPhoneNumberEntity = createSelector (
    [CommonSelectors.getPhoneNumbers, getPhoneNumber],
    (entities, id) => entities.get(id) || null
)

// addresses related
export const getDeliveryAdresses = createSelector(
    getChannel,
    channel => channel.get(addressTypes.DELIVERY) || null
);

export const getSelectedDeliveryAddressesEntities = createSelector(
    [CommonSelectors.getAddresses, getDeliveryAdresses],
    (entities, deliveryAddresses) => CommonSelectors.getEntitiesForIds(entities, deliveryAddresses) || List()
);

export const getDeliveryAddress = createSelector(
    getChannel,
    channel => channel.get('deliveryAddress') || null
);

export const getSelectedDeliveryAddressEntity = createSelector(
    [getDeliveryAddress, CommonSelectors.getAddresses],
    (id, addresses) => addresses.get(id) || null
);

export const getPostalAdresses = createSelector(
    getChannel,
    channel => channel.get(addressTypes.POSTAL) || List()
);

export const getSelectedPostalAdressesEntities = createSelector(
    [CommonSelectors.getAddresses, getPostalAdresses],
    (entities, postalAddresses) => CommonSelectors.getEntitiesForIds(entities, postalAddresses) || List()
);

export const getVisitingAdresses = createSelector(
    getChannel,
    channel => channel.get(addressTypes.VISITING) || List()
);

export const getSelectedVisitingAdressesEntities = createSelector(
    [CommonSelectors.getAddresses, getVisitingAdresses],
    (entities, visitingAddresses) => CommonSelectors.getEntitiesForIds(entities, visitingAddresses) || List()
);

// languages
export const getSelectedLanguages = createSelector(
    getChannel,
    channel => channel.get('languages') || null
);

export const getSelectedLanguagesItemsJS = createSelector(
    [CommonSelectors.getLanguages, getSelectedLanguages],
    (languages, selectedLanguages) => CommonSelectors.getEntitiesForIdsJS(languages, selectedLanguages)
);

// municipalities

export const getSelectedMunicipalities = createSelector(
    getChannel,
    channel => channel.get('municipalities') || null
);

export const getSelectedMunicipalitiesItemsJS = createSelector(
    [CommonSelectors.getMunicipalities, getSelectedMunicipalities],
    (municipalities, selectedMunicipalities) => CommonSelectors.getEntitiesForIdsJS(municipalities, selectedMunicipalities)
);

// opening hours

export const getActiveOpeningHours = createSelector(
    getChannel,
    channel => {
        const activeHours = channel.get('activeHours');
        return activeHours === 0 ? 0 : activeHours || -1;
    }
);

export const getOpeningHours = createSelector(
    CommonSelectors.getEntities,
    openingHours => openingHours.get('openingHours') || Map()
);

export const getOpeningHoursExceptional = createSelector(
    getChannel,
    channel => channel.get('openingHoursExceptional') || List()
);

export const getOpeningHoursNormal = createSelector(
    getChannel,
    channel => channel.get('openingHoursNormal') || List()
);

export const getOpeningHoursSpecial = createSelector(
    getChannel,
    channel => channel.get('openingHoursSpecial') || List()
);

export const getOpeningHour = createSelector(
    [getOpeningHours, CommonSelectors.getParameterFromProps('id'), CommonSelectors.getParameterFromProps('openingHoursId')],
    (openingHours, id, openingHourId) => openingHours.get(openingHourId || id) || Map()
);

export const getOpeningHourType = createSelector(
    [getOpeningHour, CommonSelectors.getParameterFromProps('openingHoursType')], 
    (openingHour, type) => type || ""
);

export const getOpeningHourAlterTitle = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('alterTitle') || ''
);

export const getOpeningHourValidityType = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('isDateRange') || false
);

export const getOpeningHourValidFrom = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('validFrom') || ''
);

export const getOpeningHourValidTo = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('validTo') || ''
);

export const getOpeningHourNonstop = createSelector(
    getOpeningHour, 
    openingHour => openingHour.get('nonstop') || false
);

// Daily Opening Hours

export const getWeekdaysList = _ => List(Object.keys(weekdaysEnum).map(x => weekdaysEnum[x]));

export const getDailyId = (openingHourId, dayId) => (
    `${openingHourId}_${dayId}`
);

export const getDailyOpeningHours = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('dailyOpeningHours') || Map()
);

export const getDailyOpeningHourId = CommonSelectors.getParameterFromProps('id');

export const getDailyOpeningHour = createSelector(
    [getDailyOpeningHours, getDailyOpeningHourId],
    (dailyOpeningHours, id) => dailyOpeningHours.get(id) || Map()
);

const findFisrtSelectedDay = (days) => days.find(day => day.get('day') || false);

const findDefaultDailyOpeningHour = (dailyOpeningHours, openingHoursId) => {
    return findFisrtSelectedDay( 
                getWeekdaysList()
                .map(day => dailyOpeningHours.get(getDailyId(openingHoursId, day)) || Map())
        ) || Map();
} 

export const getDefaultDailyOpeningHour = createSelector(
    [getDailyOpeningHours, CommonSelectors.getParameterFromProps('openingHoursId')],
    (dailyOpeningHours, openingHoursId) => findDefaultDailyOpeningHour(dailyOpeningHours, openingHoursId)        
);

export const getDailyOpeningHourDay = createSelector(
    [getDailyOpeningHour, getOpeningHourNonstop],
    (dailyOpeningHour, nonstop) => dailyOpeningHour.get('day') && !nonstop// false
);

const getWithDefault = (checked, item, defaultItem, property, defaultResult) => {
    if (!checked){
        return Map ();
    }
    const isValueSet = item.get(property) || item.get(property) === 0;
    const value = (isValueSet ? item.get(property) : defaultItem.get(property));

    return Map({ 
        isValueSet, 
        value: value != null ? value : defaultResult
    });
    //checked ? (item.get(property) ? item.get(property) : defaultItem.get(property) || defaultResult) : defaultResult;
}

const getWithDefaultValue = (valueOrDefault) => {
    return valueOrDefault.get('value');
}

const getWithDefaultIsValueSet = (valueOrDefault) => {
    return valueOrDefault.get('isValueSet') || false;
}

export const getDailyOpeningHourExtra = createSelector(
    [getDailyOpeningHour, getDailyOpeningHourDay, getDefaultDailyOpeningHour],
    (dailyOpeningHour, checked, defaultDay) => {
        return getWithDefaultValue(getWithDefault(checked, dailyOpeningHour, defaultDay, 'extra', null));
    }
);

export const getOpeningHoursType = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('openingHoursType') || List()
);

export const getOpeningHoursTypeObjectArray = createSelector(
    getOpeningHoursType,
    openingHoursType => CommonSelectors.getObjectArray(openingHoursType)
);

export const getOpeningHoursValidityTypes = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('openingHoursValidityTypes') || List()
);

export const getOpeningHoursValidityTypesObjectArray = createSelector(
    getOpeningHoursValidityTypes,
    openingHoursValidityTypes => CommonSelectors.getObjectArray(openingHoursValidityTypes)
);

const defaultStartTime = (24 + 8) * 3600 * 1000;
const defaultEndTime = (24 + 16) * 3600 * 1000;

export const getDailyOpeningHourTimeFromInfo = createSelector(
    [getDailyOpeningHour, getDailyOpeningHourDay, getDefaultDailyOpeningHour],
    (dailyOpeningHour, checked, defaultDay) => getWithDefault(checked, dailyOpeningHour, defaultDay, 'timeFrom', defaultStartTime )
);

export const getDailyOpeningHourTimeFromExtraInfo = createSelector(
    [getDailyOpeningHour, getDailyOpeningHourDay, getDefaultDailyOpeningHour],
    (dailyOpeningHour, checked, defaultDay) => getWithDefault(checked, dailyOpeningHour, defaultDay, 'timeFromExtra', null)
);

export const getDailyOpeningHourTimeToInfo = createSelector(
    [getDailyOpeningHour, getDailyOpeningHourDay, getDefaultDailyOpeningHour],
    (dailyOpeningHour, checked, defaultDay) => getWithDefault(checked, dailyOpeningHour, defaultDay, 'timeTo', defaultEndTime)
);

export const getDailyOpeningHourTimeToExtraInfo = createSelector(
    [getDailyOpeningHour, getDailyOpeningHourDay, getDefaultDailyOpeningHour],
    (dailyOpeningHour, checked, defaultDay) => getWithDefault(checked, dailyOpeningHour, defaultDay, 'timeToExtra', null)
);

export const getDailyOpeningHourTimeFrom = createSelector(
    getDailyOpeningHourTimeFromInfo,
    time => getWithDefaultValue(time)
);

export const getDailyOpeningHourTimeFromExtra = createSelector(
    getDailyOpeningHourTimeFromExtraInfo,
    time => getWithDefaultValue(time)
);

export const getDailyOpeningHourTimeTo = createSelector(
    getDailyOpeningHourTimeToInfo,
    time => getWithDefaultValue(time)
);

export const getDailyOpeningHourTimeToExtra = createSelector(
    getDailyOpeningHourTimeToExtraInfo,
    time => getWithDefaultValue(time)
);

export const getIsDefaultDailyOpeningHourTimeFrom = createSelector(
    getDailyOpeningHourTimeFromInfo,
    time => !getWithDefaultIsValueSet(time)
);

export const getIsDefaultDailyOpeningHourTimeFromExtra = createSelector(
    getDailyOpeningHourTimeFromExtraInfo,
    time => !getWithDefaultIsValueSet(time)
);

export const getIsDefaultDailyOpeningHourTimeTo = createSelector(
    getDailyOpeningHourTimeToInfo,
    time => !getWithDefaultIsValueSet(time)
);

export const getIsDefaultDailyOpeningHourTimeToExtra = createSelector(
    getDailyOpeningHourTimeToExtraInfo,
    time => !getWithDefaultIsValueSet(time)
);

const getSelectedDays = (dailyHoursEntities, openingHourId) => (
     getWeekdaysList()
                .map(w => (dailyHoursEntities.get(getDailyId(openingHourId, w)) || Map()).set('dayFrom', w))
                .filter(x => x.get('day'))
);

const getFilteredDailyHours = (dailyHoursEntities, openingHourId) => { 
        // CommonSelectors.getEntitiesForIds(dailyHoursEntities, getWeekdaysList().map(w => getDailyId(openingHourId, w)), List())
        const selectedDays = getSelectedDays(dailyHoursEntities, openingHourId);
        const defaultDay = selectedDays.first();
        return selectedDays.map(x => x.merge({ 
            timeFrom: getWithDefaultValue(getWithDefault(true, x, defaultDay, 'timeFrom', defaultStartTime)), 
            timeTo: getWithDefaultValue(getWithDefault(true, x, defaultDay, 'timeTo', defaultEndTime)), 
            timeFromExtra: getWithDefaultValue(getWithDefault(true, x, defaultDay, 'timeFromExtra', null)), 
            timeToExtra: getWithDefaultValue(getWithDefault(true, x, defaultDay, 'timeToExtra', null)),
            extra: getWithDefaultValue(getWithDefault(true, x, defaultDay, 'extra', null)) 
        }));
};

const isOpeningHourCommonNotEmpty = ({alterTitle, isDateRange, dayFrom, dayTo, validFrom, validTo}) => ( 
        (alterTitle != '' && alterTitle != null)
        || (isDateRange && (validFrom || validTo))
);

const isOpeningHourNormalNotEmpty = ({alterTitle, isDateRange, dayFrom, dayTo, validFrom, validTo, nonstop, selectedDays}) => ( 
        isOpeningHourCommonNotEmpty({alterTitle, isDateRange, dayFrom, dayTo, validFrom, validTo}) || nonstop || selectedDays.size > 0 
);

const getFilteredOpeningHours = (hoursEntities,  dailyHoursEntities, ids) => {
    const temp = CommonSelectors.getEntitiesForIds(hoursEntities, ids, List())
        .filter(x => {
            let oh = x.toJS();
            oh.selectedDays = getSelectedDays(dailyHoursEntities, x.get('id'));
            console.log('filter inside', oh);
            return isOpeningHourNormalNotEmpty(oh);
        })
        .map(x => x.set('dailyOpeningHours', x.get('nonstop') ? List() : getFilteredDailyHours(dailyHoursEntities, x.get('id'))))
        
        console.log('filtered', temp, temp.toJS(), temp.size)
        return temp;
};

export const getIsOpeningHourNonstopRequired = createSelector(
    [
        getOpeningHourAlterTitle, 
        getOpeningHourValidityType,
        getOpeningHourValidFrom,
        getOpeningHourValidTo,
        getOpeningHourNonstop,
        getDailyOpeningHours,
        CommonSelectors.getIdFromProps
    ],
    (alterTitle, isDateRange, validFrom, validTo, nonstop, dailyOpeningHours, id) => {
        const selectedDays = getSelectedDays(dailyOpeningHours, id);
        return selectedDays.size == 0 && isOpeningHourNormalNotEmpty({alterTitle, isDateRange, validFrom, validTo, nonstop, selectedDays});
    }
);

export const getFilteredOpeningHoursNormal = createSelector(
    [getOpeningHoursNormal, getOpeningHours, getDailyOpeningHours],
    (openingHours, entities, dailyHoursEntities) => getFilteredOpeningHours(entities, dailyHoursEntities, openingHours)
);

export const getIsAnyOpeningHoursNormal = createSelector(
    getFilteredOpeningHoursNormal,
    openingHours => openingHours && openingHours.size > 0
);

// Special opening hours
export const getOpeningHourDayFrom = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('dayFrom')
);

export const getOpeningHourDayTo = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('dayTo')
);

export const getOpeningHourTimeFrom = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('timeFrom')
);

export const getOpeningHourTimeTo = createSelector(
    getOpeningHour,
    openingHour => openingHour.get('timeTo')
);

const isOpeningHourSpecialNotEmpty = ({alterTitle, isDateRange, dayFrom, dayTo, timeFrom, timeTo, validFrom, validTo}) => ( 
        isOpeningHourCommonNotEmpty({alterTitle, isDateRange, dayFrom, dayTo, validFrom, validTo}) || timeFrom || timeTo
);

export const getIsOpeningHourSpecialNotEmpty = createSelector(
    [
        getOpeningHourAlterTitle, 
        getOpeningHourValidityType,
        getOpeningHourDayFrom,
        getOpeningHourDayTo,
        getOpeningHourTimeFrom,
        getOpeningHourTimeTo,
        getOpeningHourValidFrom,
        getOpeningHourValidTo
    ],
    (alterTitle, isDateRange, dayFrom, dayTo, timeFrom, timeTo, validFrom, validTo) => 
        isOpeningHourSpecialNotEmpty({alterTitle, isDateRange, dayFrom, dayTo, timeFrom, timeTo, validFrom, validTo})  
);

export const getFilteredOpeningHoursSpecial = createSelector(
    [getOpeningHoursSpecial, getOpeningHours],
    (openingHours, entities) => CommonSelectors.getEntitiesForIds(entities, openingHours, List())
        .filter(x => isOpeningHourSpecialNotEmpty(x.toJS()))
);

export const getIsAnyOpeningHoursSpecial = createSelector(
    getFilteredOpeningHoursSpecial,
    openingHours => openingHours && openingHours.size > 0
);

// Exceptional opening hours
export const getOpeningHourClosed = createSelector(
    getOpeningHour, 
    openingHour => openingHour.get('closed') || false
);

//quick fix for rel 1.3 
// export const getIsAnyOpeningHoursExceptionalForSize = createSelector(
//     [getOpeningHoursExceptional, getOpeningHours],
//     (openingHours, entities) => CommonSelectors.getEntitiesForIds(entities, openingHours, List()) || List()
// );

export const getIsAnyOpeningHoursExceptional = createSelector(
    getOpeningHoursExceptional,
    openingHours => openingHours && openingHours.size > 0
);

export const getSaveStepOpeningHoursModel = createSelector(
    [
        getChannelId,
        getFilteredOpeningHoursNormal,
        getOpeningHoursExceptional,
        getFilteredOpeningHoursSpecial,
        getOpeningHours,
        CommonSelectors.getLanguageParameter
    ],
    (
        id,
        normal,
        exceptional,
        special,
        openingHoursEntities,
        language
    ) => ({
        id,
        openingHoursNormal: normal,
        openingHoursExceptional: CommonSelectors.getEntitiesForIds(openingHoursEntities, exceptional, List()),
        openingHoursSpecial: special,
        language
    })
);

// connected channel services
export const getConnectedServices = createSelector(
    getChannel,
    channel => channel.get('services') || List()
);
