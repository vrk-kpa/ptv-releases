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

export const GET_CHANNEL_SEARCH_REQUEST = 'GET_CHANNEL_SEARCH_REQUEST';
export const GET_CHANNEL_SEARCH_SUCCESS = 'GET_CHANNEL_SEARCH_SUCCESS';
export const GET_CHANNEL_SEARCH_FAILURE = 'GET_CHANNEL_SEARCH_FAILURE';

export const CHANNEL_SELECT_ORGANIZATION_CHANGED = 'CHANNEL_SELECT_ORGANIZATION_CHANGED';
export const CHANNEL_NAME_CHANGED = 'CHANNEL_NAME_CHANGED';
export const CHANNEL_CHANNEL_TYPES_CHANGED = 'CHANNEL_CHANNEL_TYPES_CHANGED';

export const GET_CHANNEL_SEARCH_RESULT_REQUEST = 'GET_CHANNEL_SEARCH_RESULT_REQUEST';
export const GET_CHANNEL_SEARCH_RESULT_SUCCESS = 'GET_CHANNEL_SEARCH_RESULT_SUCCESS';
export const GET_CHANNEL_SEARCH_RESULT_FAILURE = 'GET_CHANNEL_SEARCH_RESULT_FAILURE';

export const CHANNEL_SEARCH_RESULT_ROW_SELECT = 'CHANNEL_SEARCH_RESULT_ROW_SELECT';
export const CHANNEL_SEARCH_RESULT_ALL_ROWS_SELECT = 'CHANNEL_SEARCH_RESULT_ALL_ROWS_SELECT';

export function onNameChange(name) {
	return () => ({
		type: CHANNEL_NAME_CHANGED,
		payload: {
			name: name
		}
	});
}

export function onSelectOrganizationChange(organizationId) {
	return () => ({
		type: CHANNEL_SELECT_ORGANIZATION_CHANGED,
		payload: {
			organizationId: organizationId
		}
	});
}

export function onChannelTypesListChange(channelTypes) {
	return () => ({
		type: CHANNEL_CHANNEL_TYPES_CHANGED,
		payload: {
			channelTypes: channelTypes
		}
	});
}

export function onResultTableRowSelect(row, isSelected)  
{
	return () => ({
		type: CHANNEL_SEARCH_RESULT_ROW_SELECT,
		payload: {
			row: row,
			isSelected: isSelected
		}
	});
}

export function onResultTableAllRowsSelect(rows, isSelected)  
{
	return () => ({
		type: CHANNEL_SEARCH_RESULT_ALL_ROWS_SELECT,
		payload: {
			rows: rows,
			isSelected: isSelected
		}
	});
}

function getChannelSearch(channelSearchForm) {
	return {
		channelSearchForm,
		[CALL_API]: {
			types: [GET_CHANNEL_SEARCH_REQUEST,
					GET_CHANNEL_SEARCH_SUCCESS,
					GET_CHANNEL_SEARCH_FAILURE],
			payload: { endpoint: 'channel/GetChannelSearch' }
		}
	}
}

export function loadChannelSearch() {
		return getChannelSearch('channelSearchForm');
}


function channelSearchResult(channelSearchResults, data) {
		return {
		channelSearchResults,
		[CALL_API]: {
			types: [GET_CHANNEL_SEARCH_RESULT_REQUEST,
					GET_CHANNEL_SEARCH_RESULT_SUCCESS,
					GET_CHANNEL_SEARCH_RESULT_FAILURE],
			payload: { 
						endpoint: 'channel/ChannelSearchResult',
						data : data 
					 }
		}
	}
}

export function loadChannelSearchResults(data) {		
		return channelSearchResult('channelSearchResults', data)
}
