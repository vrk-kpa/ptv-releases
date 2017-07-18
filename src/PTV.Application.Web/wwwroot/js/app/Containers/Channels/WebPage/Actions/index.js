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
import { channelTypes } from '../../../Common/Enums';
import { OrganizationSchemas } from '../../../Manage/Organizations/Organization/Schemas';
import { CommonSchemas } from '../../../Common/Schemas';
import { CommonChannelsSchemas } from '../../Common/Schemas';
import * as webPageSelector from '../Selectors';

// actions
import * as CommonActions from '../../../Common/Actions';
import * as CommonChannelsActions from '../../Common/Actions';

const keyToState = channelTypes.WEB_PAGE;

// step 1
const step1Call = (endpoint, data, successNextAction) => {
  return CommonChannelsActions.channelStepCall({
    stepKey: 'step1Form',
    payload: { endpoint, data },
    typeSchemas: [
      OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL,
      CommonSchemas.CHARGE_TYPE_ARRAY,
      CommonSchemas.SERVICE_CHANNEL_CONNECTION_TYPE_ARRAY,
      CommonSchemas.LANGUAGE_ARRAY,
      CommonSchemas.DIAL_CODE_ARRAY
    ],
    keyToState,
    successNextAction
  })
}

export function getStep1(entityId, language) {
	return step1Call('channel/GetAddWebPageChannelStep1', { entityId, language });
}

export function saveStep1Changes({ language, successNextAction }) {
  return (props) => {
    return step1Call(
      'channel/SaveWebPageStep1Changes',
      webPageSelector.getSaveStep1Model(props.getState(), { keyToState, language }),
      successNextAction
    )(props)
  }
}

// save all
export function saveAllChanges({ language }) {
	return (props) => {
		return CommonChannelsActions.channelCall('channel/SaveAllWebPageChannelChanges',
		{
			step1Form: webPageSelector.getSaveStep1Model(props.getState(), {keyToState, language}),
			language
		}
		, keyToState)(props);
	};
}
