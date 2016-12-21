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
import { Map, List } from 'immutable';

import {getSearches, getParameterFromProps, getIdFromProps, getSearchResultsModel, getSearchResultsPageNumber, getLanguageToCode } from '../../../Common/Selectors';

export const getChannelSearch = createSelector(
    [getSearches, getParameterFromProps('keyToState')],
    (searches, keyToState) => searches.get(keyToState) || Map()
);

export const getChannelName = createSelector(
    getChannelSearch,
    search => search.get('channelName') || ''
);

export const getOrganizationId = createSelector(
    getChannelSearch,
    search => search.get('organizationId') || ''
);

export const getChannelFormIdentifier = createSelector(
    getChannelSearch,
    search => search.get('channelFormIdentifier') || ''
);

export const getSelectedPublishingStauses = createSelector(
    getChannelSearch,
    search => search.get('selectedPublishingStatuses') || new List()
);

export const getSelectedPhoneNumberTypes = createSelector(
    getChannelSearch,
    search => search.get('selectedPhoneNumberTypes') || new List()
);

export const getOldChannelName = createSelector(
    getSearchResultsModel,
    search => search.get('channelName') || ''
);

export const getOldLanguage = createSelector(
    getSearchResultsModel,
    search => search.get('language') || ''
);

export const getOldOrganizationId = createSelector(
    getSearchResultsModel,
    search => search.get('organizationId') || ''
);

export const getOldChannelFormIdentifier = createSelector(
    getSearchResultsModel,
    search => search.get('channelFormIdentifier') || ''
);

export const getOldSelectedPublishingStauses = createSelector(
    getSearchResultsModel,
    search => search.get('selectedPublishingStatuses') || new List()
);

export const getOldSelectedPhoneNumberTypes = createSelector(
    getSearchResultsModel,
    search => search.get('selectedPhoneNumberTypes') || new List()
);

export const getIsSelectedPublishingStatus = createSelector(
    [getSelectedPublishingStauses, getIdFromProps],
    (selectedPublishingStatuses, id) => selectedPublishingStatuses.includes(id)
);

export const getIsSelectedPhoneNumberType = createSelector(
    [getSelectedPhoneNumberTypes, getIdFromProps],
    (selectedPhoneNumberTypes, id) => selectedPhoneNumberTypes.includes(id)
);


export const getModelToSearch = createSelector(
    [getChannelName,
    getLanguageToCode,
    getOrganizationId,
    getChannelFormIdentifier,
    getSelectedPublishingStauses,
    getSelectedPhoneNumberTypes, getParameterFromProps('keyToState')],
    (channelName, language, organizationId, channelFormIdentifier, selectedPublishingStatuses, selectedPhoneNumberTypes, channelType) =>
     ({ channelName, language, organizationId, channelFormIdentifier, selectedPublishingStatuses, selectedPhoneNumberTypes, channelType})
);

export const getOldModelToSearch = createSelector(
    [getOldChannelName,
    getOldLanguage,
    getOldOrganizationId,
    getOldChannelFormIdentifier,
    getOldSelectedPublishingStauses,
    getOldSelectedPhoneNumberTypes, getParameterFromProps('keyToState'), getSearchResultsPageNumber],
    (channelName, language, organizationId, channelFormIdentifier, selectedPublishingStatuses, selectedPhoneNumberTypes, channelType, pageNumber) =>
     ({ channelName, language, organizationId, channelFormIdentifier, selectedPublishingStatuses, selectedPhoneNumberTypes, channelType, pageNumber})
);
