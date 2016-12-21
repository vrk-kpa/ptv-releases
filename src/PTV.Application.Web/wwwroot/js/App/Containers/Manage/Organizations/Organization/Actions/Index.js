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
import { CALL_API } from '../../../../../Middleware/Api';

//Actions
import * as CommonActions from '../../../../../Containers/Common/Actions';
import * as CommonOrganizationActions from '../../Common/Actions';

//Schemas
import { OrganizationSchemas } from '../../Organization/Schemas';
import { CommonSchemas } from '../../../../Common/Schemas';
import { CommonOrganizationSchemas } from '../../Common/Schemas';

//Selectors
import * as OrganizationSelectors from '../Selectors';

//Step1
const step1Call = (endpoint, data) => {
	return CommonActions.apiCall(['organization', 'step1Form'],
		 { endpoint, data },
		 [OrganizationSchemas.ORGANIZATION_ARRAY, CommonOrganizationSchemas.ORGANIZATION_TYPE_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY, CommonSchemas.WEB_PAGE_TYPE_ARRAY],
		 OrganizationSchemas.ORGANIZATION, 'organization');
}

export function getStep1(organizationId, language) {
	return step1Call('organization/GetOrganizationStep1', { organizationId, language });
}

export function saveStep1Changes({id, keyToState, language}) {
	return ({getState}) => {
		 return step1Call('organization/SaveStep1Changes', { step1Form: OrganizationSelectors.getSaveStep1Model(getState(), {keyToState, language}), id })({getState});
	};
}

export function saveAllChanges({keyToState, language}) {
	return ({getState}) => {
		return CommonOrganizationActions.organizationCall('organization/SaveAllChanges', { step1Form: OrganizationSelectors.getSaveStep1Model(getState(), {keyToState, language}) })({getState});
	};
}

export function publishOrganization(id, keyToState) {
	return CommonOrganizationActions.organizationCall('organization/PublishOrganization', { id }, keyToState);
}

export function deleteOrganization(id, keyToState) {
	return CommonOrganizationActions.organizationCall('organization/DeleteOrganization', { id }, keyToState);		
}