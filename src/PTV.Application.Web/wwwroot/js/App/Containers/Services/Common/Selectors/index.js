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
import { getEntities, getEntitiesForIds, getResults, getEnums, getJS, getServiceReducers, getObjectArray, getPageModeState, getEntityId, getEntityRootId, getSearchInfo, getParameters, getIdFromProps, getParameterFromProps, getLanguageParameter } from '../../../Common/Selectors';
import { List, Map } from 'immutable';

// serviceClasses
export const getServiceClassesEntities = createSelector(
    getEntities,
    entities => entities.get('serviceClasses') || Map()
);

export const getServiceClassesForIds = createSelector(
    [getServiceClassesEntities, getParameterFromProps('ids')],
    (serviceClasses, ids) => getEntitiesForIds(serviceClasses, List(ids), List())
);

export const getServiceClassesOrdered = createSelector(
    getServiceClassesEntities,
    serviceClasses => serviceClasses.sortBy(sc => sc.get('name'))
);

export const getServiceClasses = createSelector(
    getServiceClassesOrdered,
    serviceClasses => serviceClasses
);

export const getServiceClass = createSelector(
    [getServiceClasses, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const geServiceClassNameForId = createSelector(
    getServiceClass,
    serviceClass => serviceClass.get('name')
);
export const getFilteredServiceClasses = createSelector(
    getEntities,
    entities => entities.get('filteredServiceClasses') || Map()
);

export const getFilteredServiceClass = createSelector(
    [getFilteredServiceClasses, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getServiceClassChildren = createSelector(
    getServiceClass,
    entity => entity.get('children') || Map()
);

export const getFilteredServiceClassChildren = createSelector(
    getFilteredServiceClass,
    entity => entity.get('children') || Map()
);

export const getServiceClassWithFiltered = createSelector(
    [getServiceClass, getFilteredServiceClass],
    (entity, filtered) => entity.size > 0 ? entity : filtered
);

export const getServiceClassesJS = createSelector(
    getServiceClasses,
    serviceClasses => getJS(serviceClasses, [])
);

export const getServiceClassesObjectArray = createSelector(
    getServiceClasses,
    serviceClasses => getObjectArray(serviceClasses)
);

export const getTopServiceClasses = createSelector(
    getEnums,
    results => results.get('topServiceClasses') || results.get('serviceClasses') || List()
)

// end serviceClasses

// industrial classes
export const getIndustrialClasses = createSelector(
    getEntities,
    entities => entities.get('industrialClasses') || List()
);

export const getIndustrialClass = createSelector(
    [getIndustrialClasses, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getFilteredIndustrialClasses = createSelector(
    getEntities,
    entities => entities.get('filteredIndustrialClasses') || List()
);

export const getFilteredIndustrialClass = createSelector(
    [getFilteredIndustrialClasses, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getTopIndustrialClasses = createSelector(
    getEnums,
    results => results.get('industrialClasses') || List()
)

export const getIndustrialClassWithFiltered = createSelector(
    [getIndustrialClass, getFilteredIndustrialClass],
    (entity, filtered) => entity.size > 0 ? entity : filtered
);

// end industrialClasses

// lifeEvents
export const getLifeEvents = createSelector(
    getEntities,
    entities => entities.get('lifeEvents') || List()
);

export const getLifeEvent = createSelector(
    [getLifeEvents, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getFilteredLifeEvents = createSelector(
    getEntities,
    entities => entities.get('filteredLifeEvents') || List()
);

export const getFilteredLifeEvent = createSelector(
    [getFilteredLifeEvents, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getTopLifeEvents = createSelector(
    getEnums,
    results => results.get('topLifeEvents') || results.get('lifeEvents') || List()
)

export const getLifeEventWithFiltered = createSelector(
    [getLifeEvent, getFilteredLifeEvent],
    (entity, filtered) => entity.size > 0 ? entity : filtered
);
// end lifeEvents

// targetGroups
export const getTargetGroups = createSelector(
    getEntities,
    entities => entities.get('targetGroups') || List()
);


export const getTopTargetGroups = createSelector(
    getEnums,
    results => results.get('topTargetGroups') || results.get('targetGroups') || List()
)
// end targetGroups

// ontologyTerms

export const getOntologyTerms = createSelector(
    getEntities,
    entities => entities.get('ontologyTerms') || List()
);
export const getAnnotationOntologyTerms = createSelector(
    getEntities,
    entities => entities.get('annotationOntologyTerms') || List()
);

export const getOntologyTerm = createSelector(
    [getOntologyTerms, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getAnnotationOntologyTerm = createSelector(
    [getAnnotationOntologyTerms, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getTopOntologyTerms = createSelector(
    getEnums,
    results => List()
)

export const getOntologyTermsForIds = createSelector(
    [getOntologyTerms, getParameterFromProps('ids')],
    (ontologyTerms, ids) => getEntitiesForIds(ontologyTerms, List(ids), List())
);
export const getOntologyNameForId = createSelector(
    getOntologyTerm,
    ontologyTerms => {
      return ontologyTerms.get('name')
    }
);
// end ontologyTerms

// keyWords
export const getKeyWordEntities = createSelector(
    getEntities,
    entities => entities.get('keyWords') || List()
);

export const getKeyWords = createSelector(
   [getKeyWordEntities, getLanguageParameter],
   (keywords, language) => keywords.map((keyword) => keyword.get(language)).filter(k => k) || List()
);

export const getKeyWordsJS = createSelector(
    getKeyWords,
    keyWords => getJS(keyWords, [])
);

export const getKeyWordsObjectArray = createSelector(
    getKeyWords,
    keyWords => getObjectArray(keyWords)
);
// end keyWords

export const getCommonReducer = createSelector(
    getServiceReducers,
    services => services.get('commonReducer')
)

export const serviceKeyToState = "service";

export const getServiceId = createSelector(
    getPageModeState,
    pageState => getEntityId(pageState.get(serviceKeyToState))
)

export const getServiceRootId = createSelector(
    getPageModeState,
    pageState => getEntityRootId(pageState.get(serviceKeyToState))
)

export const getServiceTypes = createSelector(
    getEntities,
    entities => entities.get('serviceTypes') || Map()
);

export const getServiceType = createSelector(
    [getServiceTypes, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
);

export const geServiceTypeNameForId = createSelector(
    getServiceType,
    serviceType => serviceType.get('name')
);

export const getProvisionTypes = createSelector(
    getEntities,
    entities => entities.get('provisionTypes') || List()
);

export const getProvisionTypesObjectArray = createSelector(
    getProvisionTypes,
    provisionTypes => getObjectArray(provisionTypes)
);

export const getCoverageTypes = createSelector(
    getEntities,
    entities => entities.get('coverageTypes') || List()
);

export const getCoverageTypesObjectArray = createSelector(
    getCoverageTypes,
    coverageTypes => getObjectArray(coverageTypes)
);
export const getCoverageTypesJS = createSelector(
    getCoverageTypes,
    coverageTypes => getJS(coverageTypes, [])
);

export const getServiceTypesObjectArray = createSelector(
    getServiceTypes,
    serviceTypes => getObjectArray(serviceTypes)
);

export const getServiceTypesJS = createSelector(
    getServiceTypes,
    serviceTypes => getJS(serviceTypes, [])
);
