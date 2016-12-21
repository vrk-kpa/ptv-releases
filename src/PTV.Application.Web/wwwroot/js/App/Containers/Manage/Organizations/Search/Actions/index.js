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
import { List } from 'immutable';
import { CALL_API, Schemas } from '../../../../../Middleware/Api';
import { CommonSchemas } from '../../../../Common/Schemas';
import { OrganizationSchemas } from '../../Organization/Schemas';
import * as CommonOrganizationSelectors from '../../Common/Selectors';
import * as OrganizationSearchSelectors from '../Selectors';
import * as CommonActions from '../../../../Common/Actions';
import { getSearchedEntities } from '../../../../Common/Selectors';

const keyToState = 'organization';
const keyToEntities = 'organizations';

export const ORGANIZATION_SEARCH_INPUT_CHANGE = 'ORGANIZATION_SEARCH_INPUT_CHANGE';

export function onOrganizationSearchInputChange(input,value, isSet) {
	return ({getState}) => CommonActions.onEntityInputChange('searches', keyToState ,input, value, isSet)
}

export function onOrganizationSearchListChange(input ,value, isAdd) {
	return ({getState}) => ({
		type: ORGANIZATION_SEARCH_INPUT_CHANGE,
		payload: { property: ['searches', keyToState , input], item: value , isAdd }
	});
}

export function loadOrganization(isShowMore=false) {
	return (props) => { 
		const state = props.getState();
		const model = isShowMore ? {...OrganizationSearchSelectors.getOldModelToSearch(state, {keyToState}), prevEntities: getSearchedEntities(state, {keyToState, keyToEntities}), [keyToEntities]: List() } :
		 	{...OrganizationSearchSelectors.getModelToSearch(state, {keyToState}), prevEntities: List(), organizations: List() };
		return CommonActions.apiCall([keyToState, 'searchResults'], { endpoint: 'organization/SearchOrganizations', data: model },
		[], [OrganizationSchemas.ORGANIZATION_ARRAY], undefined, undefined, true )(props)};
}

export function loadOrganizationSearch() {
	return CommonActions.apiCall([keyToState, 'search'], { endpoint: 'organization/GetOrganizationSearch', data: null },
		[CommonSchemas.PUBLISHING_STATUS_ARRAY, OrganizationSchemas.ORGANIZATION_ARRAY], CommonSchemas.SEARCH, keyToState, true );	
}
