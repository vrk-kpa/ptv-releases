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
import { CALL_API, Schemas } from '../../../../Middleware/Api';
import { List, Map } from 'immutable';
import { ServiceSchemas } from '../../Service/Schemas';
import { CommonSchemas } from '../../../Common/Schemas';
import { CommonServiceSchemas } from '../../Common/Schemas';
import { OrganizationSchemas } from '../../../Manage/Organizations/Organization/Schemas';
import * as CommonActions from '../../../Common/Actions';
import { getServiceId } from '../../Common/Selectors';
import { getSearchedEntities } from '../../../Common/Selectors';

import * as ServiceSearchSelectors from '../Selectors';

const keyToState = 'service';
const keyToEntities = 'services'

export const SERVICE_SEARCH_INPUT_CHANGE = 'SERVICE_SEARCH_INPUT_CHANGE';

export function onServiceSearchInputChange(input,value, isSet) {
	return ({getState}) => CommonActions.onEntityInputChange('searches',keyToState,input, value, isSet)
}

export function onServiceSearchListChange(input ,value, isAdd) {
	return ({getState}) => ({
		type: SERVICE_SEARCH_INPUT_CHANGE,
		payload: { property: ['searches', keyToState, input], item: value , isAdd }
	});
}

export function loadServices(isShowMore=false) {
	return (props) => { 
		const state = props.getState();
		const model = isShowMore ? {...ServiceSearchSelectors.getOldModelToSearch(state, {keyToState}), prevEntities: getSearchedEntities(state, {keyToState, keyToEntities}), [keyToEntities]: List() } : {...ServiceSearchSelectors.getModelToSearch(props.getState(), { keyToState }), services: List(), prevEntities: List()}; 
		return CommonActions.apiCall([keyToState, 'searchResults'], { endpoint: 'service/SearchServices', data: model },
		[], [ServiceSchemas.SERVICE_ARRAY], undefined, undefined, true )(props)};
}

export function loadServiceSearch() {
	return CommonActions.apiCall([keyToState, 'search'], { endpoint: 'service/GetServiceSearch', data: null },
		[CommonSchemas.PUBLISHING_STATUS_ARRAY, OrganizationSchemas.ORGANIZATION_ARRAY, CommonSchemas.SERVICE_CLASS_ARRAY, CommonServiceSchemas.SERVICE_TYPE_ARRAY], CommonSchemas.SEARCH, keyToState, true );	
}
