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
const step1Call = (endpoint, data, successNextAction) => {
  console.log(endpoint, data, successNextAction)
	return CommonActions.apiCall2({
    keys: ['organization', 'step1Form'],
		payload: { endpoint, data },
		typeSchemas: [OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL, CommonOrganizationSchemas.ORGANIZATION_TYPE_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY, CommonSchemas.WEB_PAGE_TYPE_ARRAY, CommonSchemas.MUNICIPALITY_ARRAY],
		schemas: OrganizationSchemas.ORGANIZATION,
    keyToState: 'organization',
    useGivenId: false,
    saveRequestData: true,
    successNextAction
  });
}

export function getStep1(organizationId, language) {
	return step1Call('organization/GetOrganizationStep1', { organizationId, language });
}

export function saveStep1Changes({id, keyToState, language, successNextAction}) {
	return ({getState}) => {
		 return step1Call('organization/SaveStep1Changes', { step1Form: OrganizationSelectors.getSaveStep1Model(getState(), {keyToState, language}), id }, successNextAction)({getState});
	};
}

export function saveAllChanges({keyToState, language}) {
	return ({getState}) => {
		return CommonOrganizationActions.organizationCall('organization/SaveAllChanges', { step1Form: OrganizationSelectors.getSaveStep1Model(getState(), {keyToState, language}), language:language })({getState});
	};
}

export function getOrganizationNames (id, keyToState) {
  return CommonActions.apiCall2({
    keys: [keyToState, 'all'],
    payload: { endpoint: 'organization/GetOrganizationNames', data: { id } },
    schemas: OrganizationSchemas.ORGANIZATION_V2,
    typeSchemas: [CommonSchemas.LANGUAGE_ARRAY]
  })
}

export function publishOrganization(id, formValues, keyToState) {
	return CommonOrganizationActions.organizationCall('organization/PublishOrganization',
   { id,
    languagesAvailabilities: formValues.map((value, key) => ({languageId: key, statusId: value}) ).toList() },
    keyToState);
}

export function deleteOrganization(id, keyToState, language) {
	return CommonOrganizationActions.organizationCall('organization/DeleteOrganization', { id, language }, keyToState);
}

export function lockOrganization(id, keyToState, successNextAction) {
	return (props) => {
		return CommonActions.apiCall2({
      keys: [keyToState, 'lock'],
			payload: { endpoint: 'organization/LockOrganization', data: { id } },
      saveRequestData: true,
      successNextAction
    })(props)
	};
}

export function unLockOrganization(id, keyToState) {
	return (props) => {
		return CommonActions.apiCall([keyToState, 'lock'],
									{ endpoint: 'organization/UnLockOrganization', data: { id } },
									null, null, undefined, undefined, true )(props)
	};
}

export function isOrganizationLocked(id, keyToState) {
	return (props) => {
		return CommonActions.apiCall([keyToState, 'lock'],
									{ endpoint: 'organization/IsOrganizationLocked', data: { id } },
									null, null, undefined, undefined, true )(props)
	};
}
