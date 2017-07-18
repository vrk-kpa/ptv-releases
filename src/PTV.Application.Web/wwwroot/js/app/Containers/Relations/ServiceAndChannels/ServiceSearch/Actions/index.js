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
import { CALL_API, Schemas } from '../../../../../Middleware/Api';
import { List } from 'immutable';
import { ServiceSchemas } from '../../../../Services/Service/Schemas';
import { CommonSchemas } from '../../../../Common/Schemas';
import { CommonServiceSchemas } from '../../../../Services/Common/Schemas';
import { OrganizationSchemas } from '../../../../Manage/Organizations/Organization/Schemas';
import * as CommonActions from '../../../../Common/Actions';
import { getServiceId } from '../../../../Services/Common/Selectors';
import * as ServiceSearchSelectors from '../Selectors';
import { getSearchedEntities, getTranslationLanguageId } from '../../../../Common/Selectors';

const keyToState = 'serviceAndChannelServiceSearch';
const keyToEntities= 'services';

export const SERVICE_SEARCH_INPUT_CHANGE = 'SERVICE_SEARCH_INPUT_CHANGE';

export function onServiceSearchInputChange(input,value, isSet) {

	return () => CommonActions.onEntityInputChange('searches', keyToState, input, value, isSet)
}

export function onServiceSearchListChange(input ,value, isAdd) {
	return () => ({
		type: SERVICE_SEARCH_INPUT_CHANGE,
		payload: { property: ['searches', keyToState, input], item: value , isAdd }
	});
}

export function onServiceSearchEntityAdd (propetry, entity, schema = CommonSchemas.SEARCH) {
  return () => CommonActions.onEntityAdd({
    id: keyToState,
    [propetry]: entity
  }, schema)
}

export function loadServices(isShowMore=false, language) {
	return (props) => {
		const state = props.getState();
		const model = isShowMore ? {...ServiceSearchSelectors.getOldModelToSearch(state, {keyToState, language}),
									 prevEntities: getSearchedEntities(state, {keyToState, keyToEntities}),
									 [keyToEntities]: List() }
							     : {...ServiceSearchSelectors.getModelToSearch(props.getState(), {keyToState, language}), services: List(), prevEntities: List()};
		return CommonActions.apiCall(
			[keyToState, 'searchResults'],
			{ endpoint: 'service/SearchServices', data: model },
			[],
			[ServiceSchemas.SERVICE_ARRAY], undefined, undefined, true )(props)};
}

export function loadService(id, language) {
	return (props) => {
		const state = props.getState();
		const languageId = getTranslationLanguageId(state, language);
		//payload: {}
		return CommonActions.apiCall(
			[keyToState, 'searchResults'],
			{ pageModeState:
				{serviceAndChannel: {languageTo: languageId}},
				 endpoint: 'service/SearchService', data: {id, languages: [language], prevEntities: List() } },
			[],
			[ServiceSchemas.SERVICE_ARRAY], undefined, undefined, true )(props)};
}

export function loadChannelServices(id, language) {
	return (props) => {
		const state = props.getState();
		const languageId = getTranslationLanguageId(state, language);
		return CommonActions.apiCall(
			[keyToState, 'searchResults'],
			{ pageModeState: {serviceAndChannel: {languageTo: languageId}},
			endpoint: 'service/SearchChannelServices', data: {id, languages: [language], prevEntities: List() } },
			[],
			[ServiceSchemas.SERVICE_ARRAY], undefined, undefined, true )(props)};
}

export function loadServiceSearch() {
	return CommonActions.apiCall([keyToState, 'search'], { endpoint: 'service/GetServiceSearch', data: null },
		[CommonSchemas.PUBLISHING_STATUS_ARRAY,
		 OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL,
		 CommonSchemas.SERVICE_CLASS_ARRAY,
		 CommonServiceSchemas.SERVICE_TYPE_ARRAY,
		 CommonSchemas.DIGITAL_AUTHORIZATION_ARRAY,
		 CommonSchemas.CHARGE_TYPE_ARRAY,
     CommonSchemas.CHANNEL_TYPE_ARRAY
		],
	    CommonSchemas.SEARCH, keyToState, true );
}

export const SERVICE_SEARCH_SET_EXPANDED = 'SERVICE_SEARCH_SET_EXPANDED';
export function setServiceSearchExpanded(isExpanded) {
	return () => ({
		type: SERVICE_SEARCH_SET_EXPANDED,
		relations:{
            isExpanded: isExpanded,
			keyToState: keyToState,
		}
	});
}
