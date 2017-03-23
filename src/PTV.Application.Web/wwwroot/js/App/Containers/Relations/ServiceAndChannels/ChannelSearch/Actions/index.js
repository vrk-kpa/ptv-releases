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
import { CommonSchemas } from '../../../../Common/Schemas';
import { OrganizationSchemas } from '../../../../Manage/Organizations/Organization/Schemas';
import { CommonChannelsSchemas } from '../../../../Channels/Common/Schemas';
import * as CommonActions from '../../../../Common/Actions';
import * as ChannelSearchSelectors from '../Selectors';
import { getSearchedEntities } from '../../../../Common/Selectors';

const keyToState = 'serviceAndChannelChannelSearch';
const keyToEntities = 'channels';

export const CHANNELS_SEARCH_INPUT_CHANGE = 'CHANNELS_SEARCH_INPUT_CHANGE';

export function onChannelSearchInputChange(input,value, isSet) {
	return () => CommonActions.onEntityInputChange('searches', keyToState, input, value, isSet)
}

export function onChannelsSearchListChange(input ,value, isAdd) {
	return () => ({
		type: CHANNELS_SEARCH_INPUT_CHANGE,
		payload: { property: ['searches', keyToState, input], item: value , isAdd }
	});
}



export function loadChannelSearch() {
	return CommonActions.apiCall([keyToState, 'search'], { endpoint: 'channel/GetChannelSearch', data: null },
		[CommonSchemas.PUBLISHING_STATUS_ARRAY, OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL, CommonSchemas.CHANNEL_TYPE_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY], CommonSchemas.SEARCH, keyToState, true );
}

export function loadChannelSearchResults(isShowMore=false) {
	return (props) => { 
		const state = props.getState();
		const model = isShowMore ? 
		{...ChannelSearchSelectors.getOldModelToSearch(state, {keyToState}), 
			prevEntities: getSearchedEntities(state, {keyToState, keyToEntities}),
			[keyToEntities]: List() } 
		: {...ChannelSearchSelectors.getModelToSearch(state, {keyToState}),
			 channels: List(), prevEntites: List() };
		return CommonActions.apiCall(
			[keyToState, 'searchResults'],
			{ endpoint: 'channel/ChannelSearchResult',
			data: model},
			[],
			[CommonChannelsSchemas.CHANNEL_ARRAY], undefined, undefined, true )(props)};
}

export const CHANNEL_SEARCH_SET_EXPANDED = 'CHANNEL_SEARCH_SET_EXPANDED';
export function setChannelSearchExpanded(isExpanded) {
	return () => ({
		type: CHANNEL_SEARCH_SET_EXPANDED,
		relations:{			
            isExpanded: isExpanded,
			keyToState: keyToState,
		}
	});
}