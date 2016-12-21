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

import { getEntities, getJS, getSearches, getObjectArray, getIdFromProps,getParameterFromProps, getSearchResultsModel, getPageModeStateForKey, getSearchResultsPageNumber, getLanguageToCode } from '../../../../Common/Selectors';

export const getServiceSearch = createSelector(
    getSearches,
    searches => searches.get('serviceAndChannelServiceSearch') || Map()
);

export const getServiceName = createSelector(
    getServiceSearch,
    search => search.get('serviceName') || ''
);

export const getOntologyWord = createSelector(
    getServiceSearch,
    search => search.get('ontologyWord') || ''
);

export const getOrganizationId = createSelector(
    getServiceSearch,
    search => search.get('organizationId') || ''
);

export const getServiceClassId = createSelector(
    getServiceSearch,
    search => search.get('serviceClassId') || ''
);

export const getServiceTypeId = createSelector(
    getServiceSearch,
    search => search.get('serviceTypeId') || ''
);

export const getSelectedPublishingStauses = createSelector(
    getServiceSearch,
    search => search.get('selectedPublishingStatuses') || new Map()
);

export const getOldServiceName = createSelector(
    getSearchResultsModel,
    search => search.get('serviceName') || ''
);

export const getOldLanguage = createSelector(
    getSearchResultsModel,
    search => search.get('language') || ''
);

export const getOldOntologyWord = createSelector(
    getSearchResultsModel,
    search => search.get('ontologyWord') || ''
);

export const getOldOrganizationId = createSelector(
    getSearchResultsModel,
    search => search.get('organizationId') || ''
);

export const getOldServiceClassId = createSelector(
    getSearchResultsModel,
    search => search.get('serviceClassId') || ''
);

export const getOldServiceTypeId = createSelector(
    getSearchResultsModel,
    search => search.get('serviceTypeId') || ''
);

export const getOldSelectedPublishingStauses = createSelector(
    getSearchResultsModel,
    search => search.get('selectedPublishingStatuses') || new Map()
);

export const getIsSelectedPublishingStatus = createSelector(
    [getSelectedPublishingStauses, getParameterFromProps('publishingStatusId')],
    (selectedPublishingStatuses, id) => selectedPublishingStatuses.includes(id)
);

export const getIsExpanded = createSelector(
    getPageModeStateForKey,
    search => search.get('isExpanded') || false
);

export const getIncludedRelations = createSelector(
    getServiceSearch,
    getServiceSearch => true
);
export const getModelToSearch = createSelector(
    [getServiceName,
    getLanguageToCode,
    getOntologyWord,
    getOrganizationId,
    getServiceClassId,
    getServiceTypeId,
    getSelectedPublishingStauses,
    getIncludedRelations],
    (serviceName, language, ontologyWord,
    organizationId, serviceClassId,
    serviceTypeId, selectedPublishingStatuses, includedRelations) => ({serviceName, language, ontologyWord, organizationId, serviceClassId, serviceTypeId, selectedPublishingStatuses, includedRelations})
);

export const getOldModelToSearch = createSelector(
    [getOldServiceName,
    getOldLanguage,
    getOldOntologyWord,
    getOldOrganizationId,
    getOldServiceClassId,
    getOldServiceTypeId,
    getOldSelectedPublishingStauses, getSearchResultsPageNumber],
    (serviceName, language, ontologyWord,
    organizationId, serviceClassId,
    serviceTypeId, selectedPublishingStatuses, pageNumber) => ({serviceName, language, ontologyWord, organizationId, serviceClassId, serviceTypeId, selectedPublishingStatuses, pageNumber})
);