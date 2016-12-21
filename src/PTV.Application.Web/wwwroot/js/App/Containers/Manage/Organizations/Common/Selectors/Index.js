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
import { getEntities, getResults, getJS, getObjectArray, getEnums, getParameterFromProps, getOrganizationReducers, ggetIdFromProps, getParameters, getPageEntityId, getIdFromProps, getPageModeState, getEntityId, getEntitiesForIds } from '../../../../Common/Selectors';
import { getAddresses } from '../../../../Common/Selectors'
import { Map, List } from 'immutable';
// types
import { addressTypes } from '../../../../Common/Helpers/types';

//Shared
export const getOrganizations = createSelector(
    getEntities,
    entities => entities.get('organizations') || Map()
);

export const getOrganizationForId = createSelector(
    [getOrganizations, getIdFromProps],
    (entity, id) => entity.get(id) || Map()
);

export const getOrganizationNameForId = createSelector(
    getOrganizationForId,
    organistion => organistion.get('name')
);

export const getOrganizationsJS = createSelector(
    getOrganizations,
    organizations => getJS(organizations, [])
);

export const getTopOrganizations = createSelector(
    getEnums,
    results => results.get('organizations') || List()
)

export const getOrganizationsObjectArray = createSelector(
    [getOrganizations, getTopOrganizations, getParameterFromProps('withoutOrganizationId')],
    (organizations, ids, wId) => getObjectArray(ids.filter((id) => id != wId).map(id => organizations.get(id)))
);

export const getFilteredOrganizations = createSelector(
    getEntities,
    entities => entities.get('filteredOrganizations') || List()
);

export const getFilteredOrganization = createSelector(
    [getFilteredOrganizations, getIdFromProps],
    (entity, id) => entity.get(id)
);

export const getOrganizationWithFiltered = createSelector(
    [getOrganizationForId, getFilteredOrganization],
    (entity, filtered) => entity.size > 0 ? entity : filtered
);

export const getTopFilteredOrganizations = createSelector(
    getResults,
    results => results.get('filteredOrganizations') || null
)
//Shared

//Org related
export const serviceKeyToState = "organization"; 
export const getOrganizationId = createSelector(
    getPageModeState,
    pageState => getEntityId(pageState.get(serviceKeyToState))
);

export const getOrganization = createSelector(
    [getOrganizations, getOrganizationId],
    (organizations, organizationId) => organizations.get(organizationId) || Map()
);

export const getPublishingStatus = createSelector(
    getOrganization,
    organization => organization.get('publishingStatus') || Map()
);

export const getOrganizationParentId = createSelector(
    getOrganization,
    organization => organization.get('parentId') || null
);

export const getSubOrganizationSelected = createSelector(
    getOrganization,
    organization =>
    {
        return organization.get('isSubOrganizationSelected')
    }
); 

export const isSubOrganizationSelected = createSelector(
    [getSubOrganizationSelected, getOrganizationParentId],
    (isSubOrganization, parentId) => (isSubOrganization == undefined && parentId !== null)  
                                        ? true 
                                        : (isSubOrganization !== undefined ? isSubOrganization : false)  
                                         
);

export const getOrganizationType = createSelector(
    getOrganization,
    entities => entities.get('organizationTypeId') || null
);

export const getOrganizationTypeParam = createSelector(
    getOrganization,
    entities => entities.get('organizationType') || null
);

export const getMunicipalityCode = createSelector(
    getOrganization,
    entities => entities.get('municipalityCode') || ''
);

export const getMunicipalities = createSelector(
    getEntities,
    entities => entities.get('municipalities') || Map()
);

export const getMunicipalityId = createSelector(
    getOrganization,
    entities => entities.get('municipality') || null
);

export const getMunicipalityEntity = createSelector (
    [getMunicipalities, getMunicipalityId],
    (entities, id) => entities.get(id) || null
)
   
export const getMunicipalityJS = createSelector(
    getMunicipalityEntity,
    entity => entity ? getJS(entity) : null
);

//Org description container
export const getName = createSelector(
    getOrganization,
    entities => entities.get('organizationName') || ''
);

export const getAlternateName = createSelector(
    getOrganization,
    entities => entities.get('organizationAlternateName') || ''
);

export const getDescription = createSelector(
    getOrganization,
    entities => entities.get('description') || ''
);

export const getOrganizationStringId = createSelector(
    getOrganization,
    entities => entities.get('organizationId') || ''
);

export const isAlternateNameUsedAsDisplayNameSelected = createSelector(
    getOrganization,
    entities => entities.get('isAlternateNameUsedAsDisplayName') || false
);

export const getBusinessId = createSelector(
    getOrganization,
    entities => entities.get('business') || null
);

export const getShowContacts = createSelector(
    getOrganization,
    organization => organization.get('showContacts') || Map()
);

export const ShowPostalAddress = createSelector(
    getOrganization,
    organization => organization.get('ShowPostalAddress') || Map()
);

export const ShowVisitingAddress = createSelector(
    getOrganization,
    organization => organization.get('ShowVisitingAddress') || Map()
);

export const getBusiness = createSelector(
    getEntities,
    entities => entities.get('business') || Map()
);

export const getBusinessEntity = createSelector (
    [getBusiness, getBusinessId],
    (entities, id) => entities.get(id) || null
)

//Org type
export const getOrganizationTypes = createSelector(
    getEntities,
    entities => entities.get('organizationTypes') || List()
);

export const getOrganizationTypesIds = createSelector(
    getEnums,
    entities => entities.get('organizationTypes') || List()
);

export const getOrganizationTypesObjectArray = createSelector(
    getOrganizationTypes,
    organizationTypes => getObjectArray(organizationTypes)
);

export const getOrganizationTypesJS = createSelector(
    getOrganizationTypes,
    organizationTypes => getJS(organizationTypes, [])
);

export const getOrganizationTypeMap = createSelector(
    [getOrganizationTypes, getParameters],
    (organizationTypes, code) => organizationTypes.filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
);

export const getOrganizationTypeForCode = createSelector(
    getOrganizationTypeMap,   
    organizationType => organizationType.get('id') || ''
);

export const getIsOrganizationTypeSelected = createSelector(
    [getOrganizationType, getParameters ],  
    (selectedOrganizationType, id) => selectedOrganizationType === id
);

//Org emails
export const getEmails = createSelector(
    getEntities,
    entities => entities.get('emails') || Map()
);

export const getOrganizationEmails = createSelector(
    getOrganization,
    organization => organization.get('emails') || Map()
);

export const getOrganizationEmailEntities = createSelector(
    [getEmails, getOrganizationEmails],
    (emails, organizationEmails) => getEntitiesForIds(emails, organizationEmails) || List()
);

//Org phone numbers
export const getPhoneNumbers = createSelector(
    getEntities,
    entities => entities.get('phoneNumbers') || Map()
);

export const getOrganizationPhoneNumbers = createSelector(
    getOrganization,
    organization => organization.get('phoneNumbers') || Map()
);

export const getOrganizationPhoneNumberEntities = createSelector(
    [getPhoneNumbers, getOrganizationPhoneNumbers],
    (phoneNumbers, organizationPhoneNumbers) => getEntitiesForIds(phoneNumbers, organizationPhoneNumbers) || List()
);

//Org webPages
export const getWebPages = createSelector(
    getEntities,
    entities => entities.get('webPages') || Map()
);

export const getOrganizationWebPages = createSelector(
    getOrganization,
    entities => entities.get('webPages') || Map()
);

export const getOrganizationWebPageEntities = createSelector(
    [getWebPages, getOrganizationWebPages],
    (webPages, organizationWebPages) => getEntitiesForIds(webPages, organizationWebPages) || List()
);

export const getSortedOrganizationWebPages = createSelector(
    getOrganizationWebPageEntities,
    webPages => webPages.sortBy(webPage => webPage.get('orderNumber')).map(webPage => webPage.get('id')) || List()
);

//Org addresses
export const getPostalAddresses = createSelector(
    getOrganization,
    organization => organization.get(addressTypes.POSTAL) || List()
);

export const getVisitingAddresses = createSelector(
    getOrganization,
    organization => organization.get(addressTypes.VISITING) || List()
);

export const getOrganizationPostalAddressEntities = createSelector(
    [getAddresses, getPostalAddresses],
    (addresses, postalAddresses) => getEntitiesForIds(addresses, postalAddresses) || List()
);

export const getOrganizationVisitingAddressEntities = createSelector(
    [getAddresses, getVisitingAddresses],
    (addresses, visitingAdresses) => getEntitiesForIds(addresses, visitingAdresses) || List()
);