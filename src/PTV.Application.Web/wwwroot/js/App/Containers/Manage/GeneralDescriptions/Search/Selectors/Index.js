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
import * as CommonSelectors from '../../../../Common/Selectors';
import { getTargetGroups, getTopTargetGroups, getServiceClasses } from '../../../../Services/Common/Selectors'
import { Map, List } from 'immutable';

// Search selectors
export const getGeneralDescriptionSearch = createSelector(
    CommonSelectors.getSearches,
    searches => searches.get('generalDescription') || Map()
)

export const getGeneralDescriptionSearchResultsModel = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('descriptionSearchResults')
);

export const getGeneralDescriptionSearchModel = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('descriptionSearchForm')
);

export const getGeneralDescriptionSelectedId = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('generalDescriptionItemSelectedId') || ''
);

export const getGeneralDescriptionName = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('generalDescriptionName') || ''
);

export const getGeneralDescriptionServiceClassId = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('serviceClassId') || ''
);
export const getGeneralDescriptionSubTargetGroupId = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('subTargetGroupId') || ''
);
export const getGeneralDescriptionTargetGroupId = createSelector(
    getGeneralDescriptionSearch,
    search => search.get('targetGroupId') || ''
);

export const getOldGeneralDescriptionName = createSelector(
    CommonSelectors.getSearchResultsModel,
    search => search.get('generalDescriptionName') || ''
);

export const getOldGeneralDescriptionServiceClassId = createSelector(
    CommonSelectors.getSearchResultsModel,
    search => search.get('serviceClassId') || ''
);
export const getOldGeneralDescriptionSubTargetGroupId = createSelector(
    CommonSelectors.getSearchResultsModel,
    search => search.get('subTargetGroupId') || ''
);
export const getOldGeneralDescriptionTargetGroupId = createSelector(
    CommonSelectors.getSearchResultsModel,
    search => search.get('targetGroupId') || ''
);

export const getServiceClassesList = createSelector(
    CommonSelectors.getEnums,
    results => results.get('serviceClasses') || List()
)

export const geServiceClassesObjectArray = createSelector(
    [getServiceClasses, getServiceClassesList],
    (serviceClasses, ids) => CommonSelectors.getEntitiesForIdsJS(serviceClasses, ids)
);

export const getTopTargetGroupsObjectArray = createSelector(
    [getTargetGroups, getTopTargetGroups],
    (targetGroups, ids) => CommonSelectors.getEntitiesForIdsJS(targetGroups, ids)
);

export const getTopTargetGroupEntity = createSelector(
    [getTargetGroups, getGeneralDescriptionTargetGroupId],
    (targetGroups, id) => targetGroups.get(id) || Map()
);

export const getSubTargetGroupsList = createSelector(
    getTopTargetGroupEntity,
    topTargetGroups => topTargetGroups.get('children') || List()
);

export const getSubTargetGroupsObjectArray = createSelector(
    [getTargetGroups, getSubTargetGroupsList],
    (targetGroups, ids) => CommonSelectors.getEntitiesForIdsJS(targetGroups, ids)
);

export const getGeneralDescriptions = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('generalDescriptions') || Map()
);

export const getGeneralDescriptionSelectedItem = createSelector(
    [getGeneralDescriptions, getGeneralDescriptionSelectedId],
    (entities, id) => entities.get(id) || Map()
);

export const getSelectedGeneralDescriptionTargetGroups = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('targetGroups') || List()
);

export const getSelectedGeneralDescriptionTargetGroupNames = createSelector(
    [getSelectedGeneralDescriptionTargetGroups, getTargetGroups, getTopTargetGroups],
    (ids, entities, top) => CommonSelectors.joinTexts(CommonSelectors.getEntitiesForIds(entities, ids.filter(x => top.includes(x)), List()), 'name')//.map(x => x.get('name')).join(', ')
);

export const getSelectedGeneralDescriptionSubTargetGroups = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('targetGroups') || List()
);

export const getSelectedGeneralDescriptionSubTargetGroupNames = createSelector(
    [getSelectedGeneralDescriptionSubTargetGroups, getTargetGroups, getTopTargetGroups],
    (ids, entities, top) => CommonSelectors.joinTexts(CommonSelectors.getEntitiesForIds(entities, ids.filter(x => !top.includes(x)), List()), 'name')//.map(x => x.get('name')).join(', ')
);

export const getSelectedGeneralDescriptionServiceClasses = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('serviceClasses') || List()
);

export const getSelectedGeneralDescriptionServiceClassNames = createSelector(
    [getSelectedGeneralDescriptionServiceClasses, getServiceClasses],
    (ids, entities) => CommonSelectors.joinTexts(CommonSelectors.getEntitiesForIds(entities, ids, List()), 'name')//.map(x => x.get('name')).join(', ')
);

export const getGeneralDescription = createSelector(
    [getGeneralDescriptions, CommonSelectors.getIdFromProps],
    (descriptions, id) => descriptions.get(id) || Map() 
);

export const getGeneralDescriptionServiceClasses = createSelector(
    [getGeneralDescription, getServiceClasses],
    (description, serviceClasses) => CommonSelectors.getEntitiesForIds(serviceClasses, description.get('serviceClasses'), List())
);

export const getGeneralDescriptionServiceClassNames = createSelector(
    getGeneralDescriptionServiceClasses,
    serviceClasses => CommonSelectors.joinTexts(serviceClasses, 'name')
);

export const getModelToSearch = createSelector(
    [getGeneralDescriptionName,
        getGeneralDescriptionServiceClassId,
        getGeneralDescriptionTargetGroupId,
        getGeneralDescriptionSubTargetGroupId
    ],
    (generalDescriptionName, serviceClassId, targetGroupId, subTargetGroupId) => ({ generalDescriptionName, serviceClassId, targetGroupId, getGeneralDescriptionSubTargetGroupId })
);

export const getOldModelToSearch = createSelector(
    [getOldGeneralDescriptionName,
        getOldGeneralDescriptionServiceClassId,
        getOldGeneralDescriptionTargetGroupId,
        getOldGeneralDescriptionSubTargetGroupId,
        CommonSelectors.getSearchResultsPageNumber
    ],
    (generalDescriptionName, serviceClassId, targetGroupId, subTargetGroupId, pageNumber) => ({ generalDescriptionName, serviceClassId, targetGroupId, getGeneralDescriptionSubTargetGroupId, pageNumber })
);

// End Search selectors



