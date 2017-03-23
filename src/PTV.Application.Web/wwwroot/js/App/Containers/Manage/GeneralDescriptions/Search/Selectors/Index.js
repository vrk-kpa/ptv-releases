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
import { getTargetGroups, getTopTargetGroups, getServiceClasses, getOntologyTerms } from '../../../../Services/Common/Selectors'
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

export const getSelectedGeneralDescriptionName = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('name') || Map()
);

export const getSelectedGeneralDescriptionNameLocale = createSelector(
    getSelectedGeneralDescriptionName,
    name => name.get('fi') || ''
);

export const getSelectedGeneralDescriptionAlternateName = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('alternateName') || Map()
);

export const getSelectedGeneralDescriptionAlternateNameLocale = createSelector(
    getSelectedGeneralDescriptionAlternateName,
    alternateName => alternateName.get('fi') || ''
);

export const getSelectedGeneralDescriptionShortDescription = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('shortDescription') || Map()
);

export const getSelectedGeneralDescriptionShortDescriptionLocale = createSelector(
    getSelectedGeneralDescriptionShortDescription,
    shortDescription => shortDescription.get('fi') || ''
);

export const getSelectedGeneralDescriptionDescription = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('description') || Map()
);

export const getSelectedGeneralDescriptionDescriptionLocale = createSelector(
    getSelectedGeneralDescriptionDescription,
    description => description.get('fi') || ''
);

export const getSelectedGeneralDescriptionTargetGroupEntities = createSelector(
    [getSelectedGeneralDescriptionTargetGroups, getTargetGroups, getTopTargetGroups],
    (ids, entities, top) => CommonSelectors.getEntitiesForIds(entities, ids.filter(x => top.includes(x)), List())
);

export const getSelectedGeneralDescriptionSubTargetGroups = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('targetGroups') || List()
);

export const getSelectedGeneralDescriptionSubTargetGroupEntities = createSelector(
    [getSelectedGeneralDescriptionSubTargetGroups, getTargetGroups, getTopTargetGroups],
    (ids, entities, top) => CommonSelectors.getEntitiesForIds(entities, ids.filter(x => !top.includes(x)), List())
);

export const getSelectedGeneralDescriptionServiceClasses = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('serviceClasses') || List()
);
export const getSelectedGeneralDescriptionServiceName = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('name') || Map()
);

export const getSelectedGeneralDescriptionServiceNameLocale = createSelector(
    [getSelectedGeneralDescriptionServiceName, CommonSelectors.getLanguageParameter],
    (generalDescription, language) => generalDescription.get(language) || ''
)


export const getSelectedGeneralDescriptionServiceClassEntities = createSelector(
     [getSelectedGeneralDescriptionServiceClasses, getServiceClasses],
     (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids, List())
);

export const getSelectedGeneralDescriptionOntologyTerms = createSelector(
    getGeneralDescriptionSelectedItem,
    generalDescription => generalDescription.get('ontologyTerms') || List()
);

export const getSelectedGeneralDescriptionOntologyTermEntities = createSelector(
     [getSelectedGeneralDescriptionOntologyTerms, getOntologyTerms],
     (ids, entities) => CommonSelectors.getEntitiesForIds(entities, ids, List())
);

export const getGeneralDescription = createSelector(
    [getGeneralDescriptions, CommonSelectors.getIdFromProps],
    (descriptions, id) => descriptions.get(id) || Map()
);

export const getGeneralDescriptionServiceClasses = createSelector(
    [getGeneralDescription, getServiceClasses],
    (description, serviceClasses) => CommonSelectors.getEntitiesForIds(serviceClasses, description.get('serviceClasses'), List())
);

export const getModelToSearch = createSelector(
    [getGeneralDescriptionName,
        getGeneralDescriptionServiceClassId,
        getGeneralDescriptionTargetGroupId,
        getGeneralDescriptionSubTargetGroupId
    ],
    (name, serviceClassId, targetGroupId, subTargetGroupId) => ({ name, serviceClassId, targetGroupId, subTargetGroupId })
);

export const getOldModelToSearch = createSelector(
    [getOldGeneralDescriptionName,
        getOldGeneralDescriptionServiceClassId,
        getOldGeneralDescriptionTargetGroupId,
        getOldGeneralDescriptionSubTargetGroupId,
        CommonSelectors.getSearchResultsPageNumber
    ],
    (name, serviceClassId, targetGroupId, subTargetGroupId, pageNumber) => ({ name, serviceClassId, targetGroupId, subTargetGroupId, pageNumber })
);

export const getServiceInfo = createSelector(
    CommonSelectors.getPageModeStateData,
    pageModeStateData => pageModeStateData.get('serviceInfo') || Map()
);

export const getServiceInfoServiceId = createSelector(
    getServiceInfo,
    serviceInfo => serviceInfo.get('serviceId') || null
);

export const getServiceInfoReturnPath = createSelector(
    getServiceInfo,
    serviceInfo => serviceInfo.get('returnPath') || null
);

// End Search selectors



