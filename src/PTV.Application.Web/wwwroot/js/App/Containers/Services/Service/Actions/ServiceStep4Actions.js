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
import { CommonChannelsSchemas } from '../../../Channels/Common/Schemas';
import { CommonSchemas } from '../../../Common/Schemas';
import { ServiceSchemas } from '../../Service/Schemas';
import { OrganizationSchemas } from '../../../Manage/Organizations/Organization/Schemas'
import * as ServiceSelectors from '../Selectors';


export const SERVICE_ADD_STEP4_CHANNELS_RESULT_DATA_REQUEST = 'SERVICE_ADD_STEP4_CHANNELS_RESULT_DATA_REQUEST';
export const SERVICE_ADD_STEP4_CHANNELS_RESULT_DATA_SUCCESS = 'SERVICE_ADD_STEP4_CHANNELS_RESULT_DATA_SUCCESS';
export const SERVICE_ADD_STEP4_CHANNELS_RESULT_DATA_FAILURE = 'SERVICE_ADD_STEP4_CHANNELS_RESULT_DATA_FAILURE';

export const SERVICE_ADD_STEP4_CHANNELS_RESULT_SELECT_ROW = 'SERVICE_ADD_STEP4_CHANNELS_RESULT_SELECT_ROW';
export const SERVICE_ADD_STEP4_CHANNELS_RESULT_SELECT_ALL_ROWS = 'SERVICE_ADD_STEP4_CHANNELS_RESULT_SELECT_ALL_ROWS';

export const SERVICE_ADD_STEP4_CHANNELS_SEARCH_REQUEST = 'SERVICE_ADD_STEP4_CHANNELS_SEARCH_REQUEST';
export const SERVICE_ADD_STEP4_CHANNELS_SEARCH_SUCCESS = 'SERVICE_ADD_STEP4_CHANNELS_SEARCH_SUCCESS';
export const SERVICE_ADD_STEP4_CHANNELS_SEARCH_FAILURE = 'SERVICE_ADD_STEP4_CHANNELS_SEARCH_FAILURE';

export const SERVICE_ADD_STEP4_CHANNEL_ORGANIZATION_CHANGED = 'SERVICE_ADD_STEP4_CHANNEL_ORGANIZATION_CHANGED';
export const SERVICE_ADD_STEP4_CHANNEL_NAME_CHANGED = 'SERVICE_ADD_STEP4_CHANNEL_NAME_CHANGED';
export const SERVICE_ADD_STEP4_CHANNEL_TYPES_CHANGED = 'SERVICE_ADD_STEP4_CHANNEL_TYPES_CHANGED';

export const SERVICE_STEP4_CHANNELS_DATA_REQUEST= 'SERVICE_STEP4_CHANNELS_DATA_REQUEST';
export const SERVICE_STEP4_CHANNELS_DATA_SUCCESS= 'SERVICE_STEP4_CHANNELS_DATA_SUCCESS';
export const SERVICE_STEP4_CHANNELS_DATA_FAILURE= 'SERVICE_STEP4_CHANNELS_DATA_FAILURE';


export function onChannelNameChange(channelName) {
	return () => ({
		type: SERVICE_ADD_STEP4_CHANNEL_NAME_CHANGED,
		payload: {
			channelName: channelName
		}
	});
}

export function onOrganizationChange(organizationId) {
	return () => ({
		type: SERVICE_ADD_STEP4_CHANNEL_ORGANIZATION_CHANGED,
		payload: {
			organizationId: organizationId
		}
	});
}

export function onChannelTypesChange(channelTypes) {
	return () => ({
		type: SERVICE_ADD_STEP4_CHANNEL_TYPES_CHANGED,
		payload: {
			channelTypes: channelTypes
		}
	});
}

export function onResultTableRowSelect(row, isSelected) {
   return () => ({
   		type: SERVICE_ADD_STEP4_CHANNELS_RESULT_SELECT_ROW,
   		payload: {
   			row: row,
   			isSelected: isSelected
   		}
   });
}

export function onResultTableAllRowsSelect(rows, isSelected)
{
	return () => ({
		type: SERVICE_ADD_STEP4_CHANNELS_RESULT_SELECT_ALL_ROWS,
		payload: {
			rows: rows,
			isSelected: isSelected
		}
	});
}

