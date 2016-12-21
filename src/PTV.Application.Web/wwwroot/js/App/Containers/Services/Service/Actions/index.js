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
import { getServiceId } from '../../Common/Selectors';
import { getSaveStep1Model, getSaveStep2Model, getSaveStep3Model, getSaveStep4Model, getSelectedAttachedChannels } from '../Selectors';
import { onEntityInputChange, onEntityListChange, onEntityObjectChange, onEntityAdd, onLocalizedEntityObjectChange, onLocalizedEntityAdd, onLocalizedEntityInputChange, onLocalizedEntityListChange } from '../../../Common/Actions';
import {ServiceSchemas} from '../Schemas';
import merge from 'lodash/merge';

import { CommonSchemas } from '../../../Common/Schemas';
import { CommonChannelsSchemas } from '../../../Channels/Common/Schemas';
import { CommonServiceSchemas } from '../../Common/Schemas';
import { OrganizationSchemas } from '../../../Manage/Organizations/Organization/Schemas';
import * as CommonActions from '../../../Common/Actions';
import * as ServiceSelectors from '../Selectors';
import * as CommonSelector from '../../../Common/Selectors'


export const SERVICE_INPUT_CHANGE = 'SERVICE_INPUT_CHANGE';

export function onServiceInputChange(input,value, langCode, isSet) {
	return ({getState}) => onLocalizedEntityInputChange('services',getServiceId(getState()),input, value, langCode, isSet)
}

export function onProducerInputChange(id, input, value, langCode, isSet) {
	return () => onLocalizedEntityInputChange('serviceProducers', id, input, value, langCode, isSet)
}

export function onProducerObjectChange(id, object, langCode, isSet){
	return () => onLocalizedEntityObjectChange('serviceProducers', id, object, langCode, isSet)
}

export function onProducerAdd(entity, langCode) {
	return ({getState}) => {
		return onLocalizedEntityAdd({id: getServiceId(getState()), 'serviceProducers': entity}, ServiceSchemas.SERVICE, langCode);
	}
}

export function onKeyWordAdd(entity, input, langCode) {
	return ({getState}) => {
		return onLocalizedEntityAdd({ id: getServiceId(getState()), [input]: entity}, ServiceSchemas.SERVICE, langCode);
	}
}

export function onRemoveProducer(id, langCode) {
	return ({getState}) => {
		return onLocalizedEntityListChange('services', getServiceId(getState()), 'serviceProducers', id, langCode);
	}
}

export function onListChange(input ,value, langCode, isAdd) {
	return ({getState}) => {
		return onLocalizedEntityListChange('services', getServiceId(getState()), input, value, langCode, isAdd);
	};
}

export function onKeyWordRemove(input, value, isAdd, langCode) {
	return ({getState}) => {
		return onLocalizedEntityListChange('services', getServiceId(getState()), input, value, langCode, isAdd);
	}
}

const keyToState = 'service';

function callModifyAction(typeOfReducer, endpoint, data, typeSchemas) {
	return CommonActions.apiCall([keyToState, typeOfReducer], { endpoint: endpoint, data }, typeSchemas, ServiceSchemas.SERVICE, keyToState);
}
export function saveAllChanges({keyToState, language}) {
	return (props) => {
		 const state = props.getState();		 
		 return callModifyAction('all', 'service/SaveAllChanges', {
				step1Form: ServiceSelectors.getSaveStep1Model(state,{language}),
				step2Form: ServiceSelectors.getSaveStep2Model(state,{language}),
				step3Form: ServiceSelectors.getSaveStep3Model(state,{language}),
				step4Form: ServiceSelectors.getSelectedAttachedChannels(state),
				language
			},
			[])(props);
	};
}

export function publishService(id, keyToState, language) {
	return (props) => {
		 return callModifyAction('all', 'service/PublishService', { id, language }, [])(props);
	};
}

export function deleteService(id, keyToState, language) {
	return (props) => {
		return callModifyAction('all', 'service/DeleteService', { id, language }, [])(props);
	};
}

/// Step 1
export function loadAddServiceStep1(id, language) {
	return CommonActions.apiCall([keyToState, 'step1Form'], { endpoint: 'service/GetAddServiceStep1', data: { id, language} },

		[CommonServiceSchemas.COVERAGE_TYPE_ARRAY, CommonSchemas.MUNICIPALITY_ARRAY, CommonSchemas.LANGUAGE_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY, CommonServiceSchemas.SERVICE_TYPE_ARRAY], ServiceSchemas.SERVICE, 'service' );
}

export function saveStep1Changes({id, language}) {
	return (props) => {
		return callModifyAction('step1Form', 'service/SaveStep1Changes', { step1Form: ServiceSelectors.getSaveStep1Model(props.getState(), { language} ), id },
			[CommonSchemas.LANGUAGE_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY, CommonServiceSchemas.SERVICE_TYPE_ARRAY])(props);
	}
}

/// Step 2

export function loadAddServiceStep2(id, language) {
	return CommonActions.apiCall([keyToState, 'step2Form'], { endpoint: 'service/GetAddServiceStep2', data: { id, language } },
		[CommonSchemas.SERVICE_CLASS_ARRAY, CommonSchemas.INDUSTRIAL_CLASS_ARRAY, CommonSchemas.LIFE_EVENT_ARRAY, CommonServiceSchemas.KEY_WORD_ARRAY, CommonSchemas.TARGET_GROUP_ARRAY],
		ServiceSchemas.SERVICE, keyToState );
}

export function saveStep2Changes({id, language}) {
	return (props) => {
		return callModifyAction('step2Form', 'service/SaveStep2Changes', { step2Form: ServiceSelectors.getSaveStep2Model(props.getState(), { language } ), id },
			[CommonSchemas.SERVICE_CLASS_ARRAY, CommonSchemas.INDUSTRIAL_CLASS_ARRAY, CommonSchemas.LIFE_EVENT_ARRAY, CommonServiceSchemas.KEY_WORD_ARRAY, CommonSchemas.TARGET_GROUP_ARRAY])(props);
	}
}

/// Step 3

export function loadAddServiceStep3(id, language) {
	return CommonActions.apiCall([keyToState, 'step3Form'], { endpoint: 'service/GetAddServiceStep3', data: { id , language } },
		[CommonServiceSchemas.COVERAGE_TYPE_ARRAY, CommonServiceSchemas.PROVISION_TYPE_ARRAY, CommonSchemas.MUNICIPALITY_ARRAY, OrganizationSchemas.ORGANIZATION_ARRAY],
		ServiceSchemas.SERVICE, keyToState );
}

export function saveStep3Changes({id, language}) {
	return (props) => {
		return callModifyAction('step3Form', 'service/SaveStep3Changes', { step3Form: ServiceSelectors.getSaveStep3Model(props.getState(), {language }), id },
			[CommonServiceSchemas.PROVISION_TYPE_ARRAY, OrganizationSchemas.ORGANIZATION_ARRAY])(props);
	}
}

/// Step4

export function loadStep4ChannelData(id, language) {
	return CommonActions.apiCall([keyToState, 'step4Form'], { endpoint: 'service/GetServiceStep4Channeldata', data: { id, language } },
		[CommonSchemas.CHANNEL_TYPE_ARRAY ,OrganizationSchemas.ORGANIZATION_ARRAY],
		ServiceSchemas.SERVICE, keyToState );
}

export function loadChannelSearchData(value, language) {

		return (props) => {
			const payload = CommonActions.onEntityInputChange('services', getServiceId(props.getState()), 'searchChannelName', value).payload;
			return CommonActions.apiCall([keyToState, 'innerSearch'], {...payload, ...{ endpoint: 'channel/ConnectingChannelSearchResultData', data: {...ServiceSelectors.getModelToSearch(props.getState(), { language }), ...{channelName: value}} }} ,
			[],
			[CommonChannelsSchemas.CHANNEL_ARRAY] )(props)};
}

export function saveStep4Changes({id, language}) {
	return (props) => {
		return callModifyAction('step4Form', 'service/SaveStep4Changes', { step4Form: ServiceSelectors.getSelectedAttachedChannels(props.getState(), { language }), id }, [])(props);
	}
}