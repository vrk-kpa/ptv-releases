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
import { onLocalizedEntityInputChange, onLocalizedEntityListChange, onLocalizedEntityObjectChange, onLocalizedEntityAdd, apiCall, onEntityInputChange, onEntityObjectChange } from '../../../Common/Actions';
import * as CommonSelectors from '../Selectors';
import shortId from 'shortid';
import { CommonChannelsSchemas } from '../Schemas';
import { ServiceSchemas }  from '../../../Services/Service/Schemas'
import { CommonSchemas } from '../../../Common/Schemas';
import * as CommonActions from '../../../Common/Actions';

export function onChannelEntityAdd(property, entity, id, language, schema=CommonChannelsSchemas.CHANNEL) {
	return () => onLocalizedEntityAdd({id: id, [property]: entity}, schema, language);
}

export function onChannelInputChange(input, id, value, isSet, language, entity='channels') {
	return () => onLocalizedEntityInputChange(entity, id, input, value, language, isSet)
}

export function onChannelObjectChange(id, object, isSet, language, entity='channels') {
	return () => onLocalizedEntityObjectChange([entity], id, object, language, isSet)
}

export function onChannelListChange(input, id, value, language, isAdd, entity='channels') {
	return () => onLocalizedEntityListChange(entity, id, input, value, language, isAdd)
}

// Opening Hours
// export function onOpeningHoursEntityAdd(property, entity, id, schema=CommonChannelsSchemas.OPENING_HOUR) {
// 	return () => onEntityAdd({id: id, [property]: entity}, schema);
// }

export function onOpeningHoursInputChangeNoLang(id, input, value, isSet) {
	return () => onEntityInputChange('openingHours', id, input, value, isSet)
}

// export function onOpeningHoursObjectChange(id, object, isSet){
// 	return () => onEntityObjectChange('openingHours', id, object, isSet)
// }

// export function onOpeningHoursListChange(input, id, value, isAdd) {
// 	return () => onEntityListChange('openingHours', id, input, value, isAdd)
// }

export function onDailyOpeningHoursInputChangeNoLang(id, input, value, isSet) {
	return () => onEntityInputChange('dailyOpeningHours', id, input, value, isSet)
}

export function onDailyOpeningHoursObjectChangeNoLang(id, object, isSet){
	return () => onEntityObjectChange('dailyOpeningHours', id, object, isSet)
}

export function onOpeningHoursInputChange(id, input, value, isSet, language) {
	return () => onLocalizedEntityInputChange('openingHours', id, input, value, language, isSet)
}

export function onDailyOpeningHoursInputChange(id, input, value, isSet, language) {
	return () => onLocalizedEntityInputChange('dailyOpeningHours', id, input, value, language, isSet)
}

export function onDailyOpeningHoursObjectChange(id, object, isSet, language){
	return () => onLocalizedEntityObjectChange('dailyOpeningHours', id, object, language, isSet)
}

export const CHANNELS_SET_CHANNEL_ID = 'CHANNELS_SET_CHANNEL_ID';

export function setChannelId(channelType, channelId) {
	return () => ({
        channelType,
		type: CHANNELS_SET_CHANNEL_ID,
		pageSetup: {
			id: channelId || shortId.generate(),
			keyToState: channelType
		}
	});
}

export const CHANNELS_GET_STEP = 'CHANNELS_GET_STEP';

export const CHANNELS_API_CALL_REQUEST = 'CHANNELS_API_CALL_REQUEST';
export const CHANNELS_API_CALL_SUCCESS = 'CHANNELS_API_CALL_SUCCESS';
export const CHANNELS_API_CALL_FAILURE = 'CHANNELS_API_CALL_FAILURE';

export function channelsApiCall(keys, payload, typeSchemas, schemas) {
	return CommonActions.apiCall(keys, payload, typeSchemas, schemas);
}

export const channelCall = (endpoint, data, keyToState, successNextAction) => {
	return CommonActions.apiCall2({
    keys: [keyToState, 'all'],
		payload: { endpoint, data },
		schemas: CommonChannelsSchemas.CHANNEL,
    keyToState,
    successNextAction
  })
}

export const channelStepCall = ({ stepKey, payload, typeSchemas, keyToState, successNextAction }) => {
  return CommonActions.apiCall2({
    keys: [keyToState, stepKey],
    payload,
    typeSchemas,
    schemas: CommonChannelsSchemas.CHANNEL,
    keyToState,
    saveRequestData: true,
    successNextAction
  })
}

export function getChannelNames (id, keyToState) {
  return CommonActions.apiCall2({
    keys: [keyToState, 'all'],
    payload: { endpoint: 'channel/GetChannelNames', data: { id } },
    schemas: CommonChannelsSchemas.CHANNEL_V2,
    typeSchemas: [CommonSchemas.LANGUAGE_ARRAY]
  })
}

export function publishChannel(id, formValues, keyToState) {
	return channelCall('channel/PublishChannel',
   { id,
   languagesAvailabilities: formValues.map((value, key) => ({languageId: key, statusId: value}) ).toList() },
   keyToState);
}

export function withdrawChannel(id, keyToState, language) {
	return channelCall('channel/WithdrawChannel',
   { id, language },
   keyToState);
}

export function restoreChannel(id, keyToState, language) {
	return channelCall('channel/RestoreChannel',
   { id, language },
   keyToState);
}

export function deleteChannel(id, keyToState, language) {
	return channelCall('channel/DeleteChannel', { id, language }, keyToState);
}

export function getChannelServiceStep(Id, language, keyToState) {
	return CommonActions.apiCall([keyToState, 'channelServiceStep'],
		 { endpoint: 'channel/GetChannelServiceStep', data: { entityId: Id, channelType: keyToState, language } },
		 [],CommonChannelsSchemas.CHANNEL, keyToState);
}

export function lockChannel(id, keyToState, successNextAction) {
	return (props) => {
		return CommonActions.apiCall2({
      keys:[keyToState, 'lock'],
			payload: { endpoint: 'channel/LockChannel', data: { id } },
			saveRequestData: true,
      successNextAction
    })(props)
	};
}

export function unLockChannel(id, keyToState) {
	return (props) => {
		return CommonActions.apiCall([keyToState, 'lock'],
									{ endpoint: 'channel/UnLockChannel', data: { id } },
									null, null, undefined, undefined, true )(props)
	};
}

export function isChannelLocked(id, keyToState) {
	return (props) => {
		return CommonActions.apiCall([keyToState, 'lock'],
									{ endpoint: 'channel/IsChannelLocked', data: { id } },
									null, null, undefined, undefined, true )(props)
	};
}

export function isChannelEditable (id, keyToState) {
  return (props) => {
    return CommonActions.apiCall([keyToState, 'editable'],
									{ endpoint: 'channel/IsChannelEditable', data: { id } },
									null, null, undefined, undefined, true)(props)
  }
}
