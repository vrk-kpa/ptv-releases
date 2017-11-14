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
import {getSearches, getParameterFromProps, getIdFromProps, getChannelTypes, getEntitiesForIdsJS, getPageModeStateForKey, getSearchResultsModel, getSearchResultsPageNumber } from '../../../../Common/Selectors';
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors';


export const getChannelSearch = createSelector(
    getSearches,
    searches => searches.get('serviceAndChannelChannelSearch') || Map()
);

export const getChannelName = createSelector(
    getChannelSearch,
    search => search.get('channelName') || ''
);

export const getOrganizationId = createSelector(
    getChannelSearch,
    search => search.get('organizationId') || ''
);

export const getChannelTypeId = createSelector(
    getChannelSearch,
    search => search.get('channelTypeId') || ''
);

export const getSelectedPublishingStauses = createSelector(
    getChannelSearch,
    search => search.get('selectedPublishingStatuses') || new List()
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

export const getOldSelectedPublishingStauses = createSelector(
    getSearchResultsModel,
    search => search.get('selectedPublishingStatuses') || new List()
);

export const getOldChannelTypeId = createSelector(
    getSearchResultsModel,
    search => search.get('channelTypeId') || ''
);

export const getOldServiceChannelRelations = createSelector(
    getSearchResultsModel,
    search => search.get('serviceChannelRelations') || ''
);

export const getIsExpanded = createSelector(
    getPageModeStateForKey,
    search => search.get('isExpanded') || false
);

export const getIncludedRelations = createSelector(
    getChannelSearch,
    search => true
);

export const getModelToSearch = createSelector(
    [getChannelName,
    CommonServiceAndChannelSelectors.getLanguageToCodeForServiceAndChannel,
    getOrganizationId,
    getSelectedPublishingStauses,
    getChannelTypeId,
    CommonServiceAndChannelSelectors.getServiceChannelRelationsModel,
    getIncludedRelations],
    (
     name,
     language,
     organizationId,
     selectedPublishingStatuses,
     channelTypeId,
     serviceChannelRelations,
     includedRelations
     ) =>
        ({
            name,
            language,
            languages: [language],
            organizationId,
            selectedPublishingStatuses,
            channelTypeId,
            serviceChannelRelations,
            includedRelations
        })
);

export const getOldModelToSearch = createSelector(
    [getOldChannelName,
    getOldLanguage,
    getOldOrganizationId,
    getOldSelectedPublishingStauses,
    getOldChannelTypeId, getOldServiceChannelRelations, getSearchResultsPageNumber, getIncludedRelations],
    (name, language, organizationId, selectedPublishingStatuses, channelTypeId, serviceChannelRelations, pageNumber, includedRelations) =>
     ({ name, language, languages: [language], organizationId, selectedPublishingStatuses, channelTypeId, serviceChannelRelations, pageNumber, includedRelations})
);



