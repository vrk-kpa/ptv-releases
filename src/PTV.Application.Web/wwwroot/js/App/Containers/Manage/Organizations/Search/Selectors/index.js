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
import { getSearches, getEntities, getResults, getJS, getObjectArray, getEnums, getOrganizationReducers, getParameters, getIdFromProps, getSearchResultsModel, getSearchResultsPageNumber } from '../../../../Common/Selectors';
import { Map, List } from 'immutable';

export const getOrganizationSearch = createSelector(
    getSearches,
    searches => searches.get('organization') || Map()
);

export const getOrganizationName = createSelector(
    getOrganizationSearch,
    search => search.get('organizationName') || ''
);

export const getOrganizationId = createSelector(
    getOrganizationSearch,
    search => search.get('organizationId') || ''
);

export const getSelectedPublishingStauses = createSelector(
    getOrganizationSearch,
    search => search.get('selectedPublishingStatuses') || new Map()
);

export const getOldOrganizationName = createSelector(
    getSearchResultsModel,
    search => search.get('organizationName') || ''
);

export const getOldOrganizationId = createSelector(
    getSearchResultsModel,
    search => search.get('organizationId') || ''
);

export const getOldSelectedPublishingStauses = createSelector(
    getSearchResultsModel,
    search => search.get('selectedPublishingStatuses') || new Map()
);

export const getIsSelectedPublishingStatus = createSelector(
    [getSelectedPublishingStauses, getIdFromProps],
    (selectedPublishingStatuses, id) => selectedPublishingStatuses.includes(id)
);

export const getModelToSearch = createSelector(
    [getOrganizationName,
    getOrganizationId,
    getSelectedPublishingStauses],
    (organizationName, organizationId, selectedPublishingStatuses) => ({organizationName, organizationId, selectedPublishingStatuses})
);

export const getOldModelToSearch = createSelector(
    [getOldOrganizationName,
    getOldOrganizationId,
    getOldSelectedPublishingStauses, getSearchResultsPageNumber],
    (organizationName, organizationId, selectedPublishingStatuses, pageNumber) => ({organizationName, organizationId, selectedPublishingStatuses, pageNumber})
);

// End Search selectors
