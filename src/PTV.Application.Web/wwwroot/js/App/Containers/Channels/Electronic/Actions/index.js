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

import { OrganizationSchemas } from '../../../Manage/Organizations/Organization/Schemas';
import { CommonSchemas } from '../../../Common/Schemas';
import { CommonChannelsSchemas } from '../../Common/Schemas';
import { channelTypes } from '../../Common/Helpers/types';

import * as CommonActions from '../../../Common/Actions';
import * as CommonChannelsActions from '../../Common/Actions';
import * as electronicChannelSelector from '../Selectors'
import * as commonChannelSelector from '../../Common/Selectors'

const keyToState = channelTypes.ELECTRONIC;

// step 1
const step1Call = (endpoint, data) => {
	return CommonActions.apiCall([keyToState, 'step1Form'],
		 { endpoint, data },
		 [OrganizationSchemas.ORGANIZATION_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY, CommonSchemas.PUBLISHING_STATUS_ARRAY],
		 CommonChannelsSchemas.CHANNEL, keyToState);
}

export function getStep1(entityId, language) {
	return step1Call('channel/GetAddElectronicChannelStep1', { entityId, language });
}

export function saveStep1Changes({ language }) {
	return (props) => {
		 return step1Call('channel/SaveElectronicChannelStep1Changes', electronicChannelSelector.getSaveStep1Model(props.getState(), {keyToState, language}))(props);
	};
}


// step 2
const step2Call = (endpoint, data) => {
	return CommonActions.apiCall([keyToState, 'step2Form'],
		 { endpoint, data },
		 [],
		 CommonChannelsSchemas.CHANNEL, keyToState);
}

export function getStep2(entityId, language) {
	return CommonActions.apiCall([keyToState, 'step2Form'],
		 { endpoint: 'channel/GetAddElectronicChannelStep2', data: { entityId, language } },
		 [],
		 CommonChannelsSchemas.CHANNEL, keyToState);
}

export function saveStep2Changes({ language }) {
	return (props) => {
		 return step2Call('channel/SaveElectronicChannelStep2Changes', commonChannelSelector.getSaveStepOpeningHoursModel(props.getState(), {keyToState, language}))(props);
	};
}

// save all
export function saveAllChanges({ language }) {
	return (props) => {
		return CommonChannelsActions.channelCall('channel/SaveAllElectronicChannelChanges', {
				step1Form: electronicChannelSelector.getSaveStep1Model(props.getState(), {keyToState, language}),
				step2Form: commonChannelSelector.getSaveStepOpeningHoursModel(props.getState(), {keyToState, language}),
				language
			}, keyToState)(props);
	};
}


