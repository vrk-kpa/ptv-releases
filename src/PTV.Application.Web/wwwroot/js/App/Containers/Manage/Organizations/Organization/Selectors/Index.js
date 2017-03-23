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
import { List, Map } from 'immutable';
import { getEntities, getResults, getJS, getObjectArray, getEnums, getOrganizationReducers, getParameters, getLanguageParameter } from '../../../../Common/Selectors';
import * as CommonSelectors from '../../Common/Selectors';

export const getSaveStep1Model = createSelector(
 [  
    CommonSelectors.getOrganizationId,
    CommonSelectors.getOrganizationParentId,
    CommonSelectors.getOrganizationType,
    CommonSelectors.getMunicipalityId,
    CommonSelectors.getName,
    CommonSelectors.getAlternateName,
    CommonSelectors.isAlternateNameUsedAsDisplayNameSelected,
    CommonSelectors.getBusinessEntity,
    CommonSelectors.getOrganizationStringId,
    CommonSelectors.getDescription,
    CommonSelectors.getUnificRootId,

    CommonSelectors.getOrganizationEmailEntities,
    CommonSelectors.getOrganizationPhoneNumberEntities,
    CommonSelectors.getOrganizationWebPageEntities,
    CommonSelectors.getOrganizationVisitingAddressEntities,
    CommonSelectors.getOrganizationPostalAddressEntities,
    getLanguageParameter   
 ],
 (  
    id,
    parentId,
    organizationTypeId,
    municipality,
    organizationName,
    organizationAlternateName,
    isAlternateNameUsedAsDisplayName,
    business,
    organizationId,
    description,
    unificRootId,
    emails,
    phoneNumbers,
    webPages,
    visitingAddresses,
    postalAddresses,
    language
    ) => (
    {
        id,
        parentId,
        organizationTypeId,
        municipality,
        organizationName,
        organizationAlternateName,
        isAlternateNameUsedAsDisplayName,
        business,
        organizationId,
        description,
        unificRootId,
        emails,
        phoneNumbers,
        webPages,
        visitingAddresses,
        postalAddresses,
        language
    })
) 

