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
import { ServiceSchemas } from '../../../../Services/Service/Schemas';
import { CommonChannelsSchemas } from '../../../../Channels/Common/Schemas';
import * as CommonActions from '../../../../Common/Actions';
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors';

const keyToState = 'serviceAndChannel';

export function saveRelations(language) {
	return (props) => { 
        const state = props.getState();
        return CommonActions.apiCall(
            [keyToState, 'all'],
            { endpoint: 'serviceAndChannel/SaveRelations', data:{ serviceAndChannelRelations : CommonServiceAndChannelSelectors.getSaveModel(state, {language}), language} },
		    [],
            ServiceSchemas.SERVICE_ARRAY, 
            keyToState)(props);	
    }
}

export function publishRelations(language) {
	return (props) => { 
        const state = props.getState();
        return CommonActions.apiCall(
            [keyToState, 'all'],
            { endpoint: 'serviceAndChannel/PublishRelations', 
            data: {
                services: CommonServiceAndChannelSelectors.getServiceIds(state),
                channels: CommonServiceAndChannelSelectors.getChannelIds(state),
            }},
		    [],
            [ServiceSchemas.SERVICE_ARRAY, CommonChannelsSchemas.CHANNEL_ARRAY], 
            keyToState)(props);	
    }
}

export const RELATIONS_SET_READONLY = 'RELATIONS_SET_READONLY';
export function setRelationsReadOnlyMode(readOnly) {
	return () => ({
		type: RELATIONS_SET_READONLY,
		relations:{
			readOnly: readOnly,
			keyToState: keyToState,
		}
	});
}