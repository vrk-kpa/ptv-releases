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
import * as CommonActions from '../../../../Common/Actions';
import { ServiceAndChannelsSchemas } from '../../ServiceAndChannel/Schemas';
import { List } from 'immutable';
import * as CommoSelectors from './../Selectors';

export function onConnectedEntityAdd(entity, id, schema= ServiceAndChannelsSchemas.RELATION) {
	return () => CommonActions.onLocalizedEntityAdd(entity, schema, 'fi');
}

export function onServiceAndChannelsListChange(input, id, value, isAdd, entity='relations') {
	return () => CommonActions.onEntityListChange(entity, id, input, value, isAdd)
}
        
export function onServiceAndChannelsListRemove() {
	return () => CommonActions.onEntityObjectChange('relations', 'serviceAndChannelsId', {connectedServices: List()}, true)
}

//Add channelrelation with channel to service 
export function onConnectedChannelEntityAdd(entity, id, uiId, schema=ServiceAndChannelsSchemas.CONNECTED_SERVICE) {
	return ({getUid}) => CommonActions.onEntityAdd({uiId: uiId, 'channelRelations': [{ id: getUid(), connectedChannel: entity, service: id, isNew: true }]}, schema);
}

//Remove channelrelation of service
export function onConnectedChannelListChange(id, value, isAdd, entity='connectedServices') {
	return () => CommonActions.onEntityListChange(entity, id, 'channelRelations', value, isAdd)
}

export function onConnectedServiceInputChange(input, id, value, isSet, entity='connectedServices') {
	return () => CommonActions.onEntityInputChange(entity, id, input, value, isSet)
} 

//ChannelRelations
export function onChannelRelationInputChange(input, id, value, isSet, entity='channelRelations') {
	return () => CommonActions.onEntityInputChange(entity, id, input, value, isSet)
}

export function onChannelRelationLocalizedInputChange(input, id, value, langCode, isSet, entity='channelRelations') {
	return () => CommonActions.onLocalizedEntityInputChange(entity, id, input, value, langCode, isSet)
}

export function onChannelRelationListChange(input, id, value, isAdd, entity='channelRelations') {
	return () => CommonActions.onEntityListChange(entity, id, input, value, isAdd)
}

export function onChannelRelationDigitalAuthorizationWithChildrenChange(input, id, value, isAdd) {
 return ({getState}) => {
	const newSelected = CommoSelectors.getDigitalAuthorizationWithAllChildren(getState(), { id: value }); 
	const selectedIds = CommoSelectors.getSelectedDigitalAuthorizations(getState(), {id: id});

	if (isAdd)
	  	return CommonActions.onEntityAdd({id, 'digitalAuthorizations': newSelected.filter(x => !selectedIds.includes(x)).toJS()}, ServiceAndChannelsSchemas.CHANNEL_RELATION)
	  else 
	  	return CommonActions.onEntityInputChange('channelRelations', id, 'digitalAuthorizations', selectedIds.filter(x => !newSelected.includes(x)).toJS(), true)
 };
}

export function onChannelRelationsEntityClearAll(entity='channelRelations'){
	return ({getState}) =>
	{
	 	return CommonActions.onEntityClearAll(entity)
	}; 
}

export function onChannelRelationsEntityClear(connectedServiceId, entity='channelRelations'){
	return ({getState}) =>
	{
	 	return CommonActions.onEntityClear(entity, CommoSelectors.getConnectedServiceChannelRelations(getState(), {id :connectedServiceId}))
	}; 
}	

export const RELATIONS_SET_DETAIL = 'RELATIONS_SET_DETAIL';
export function setRelationsDetail(id, keyToState, entityKeyToState) {
	return () => ({
		type: RELATIONS_SET_DETAIL,
		relations:{
			detailId: id,
			keyToState: keyToState,
			entityKeyToState: entityKeyToState
		}
	});
}

export const RELATIONS_SET_CONFIRMATION = 'RELATIONS_SET_CONFIRMATION';
export function setRelationsConfirmation(type, value) {
	return () => ({
		type: RELATIONS_SET_CONFIRMATION,
		relations:{
			confirmationType: type,
			confirmationValue: value,			
			keyToState: 'serviceAndChannelConfirmation',
		}
	});
}


export function clearRelationsConfirmation() {
	return setRelationsConfirmation(null, null);
}

//Search list removing
export function onSearchListRemove(keyToState, response) {
    const payload = CommonActions.onEntityObjectChange('searches', keyToState, { id: null, name: null }).payload;   		
	let result = CommonActions.clearApiCall([keyToState,'searchResults'], response );
	result['payload'] = {...payload, ...result.payload};
	return result;    
}

export function onServiceSearchListRemove() {   
    return onSearchListRemove('serviceAndChannelServiceSearch', { model: { services: List() }} );   
}

export function onChannelSearchListRemove() {        
    return onSearchListRemove('serviceAndChannelChannelSearch', { model: { channels: List() }} );
}

