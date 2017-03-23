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
import { CALL_API, Schemas } from '../../../../Middleware/Api'
import { channelTypes } from '../../../Common/Enums'
import * as phoneSelector from '../Selectors'
import { CommonChannelsSchemas } from '../../Common/Schemas'
import { OrganizationSchemas } from '../../../Manage/Organizations/Organization/Schemas'
import { CommonSchemas } from '../../../Common/Schemas'

// actions
import * as CommonActions from '../../../Common/Actions'
import * as CommonChannelsActions from '../../Common/Actions'
import * as commonChannelSelector from '../../Common/Selectors'

const keyToState = channelTypes.PHONE

// step 1
const step1Call = (endpoint, data, successNextAction) => {
  return CommonChannelsActions.channelStepCall({
    stepKey: 'step1Form',
    payload: { endpoint, data },
    typeSchemas: [
      OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL,
      CommonSchemas.LANGUAGE_ARRAY, CommonSchemas.CHARGE_TYPE_ARRAY,
      CommonSchemas.PHONE_NUMBER_TYPE_ARRAY
    ],
    keyToState,
    successNextAction
  })
}

export function getStep1 (entityId, language) {
  return step1Call('channel/GetAddPhoneChannelStep1', { entityId, language })
}

export function saveStep1Changes ({ language, successNextAction }) {
  return (props) => {
    return step1Call(
      'channel/SavePhoneChannelStep1Changes',
      phoneSelector.getSaveStep1Model(props.getState(), { keyToState, language }),
      successNextAction
    )(props)
  }
}

// step 2
const step2Call = (endpoint, data, successNextAction) => {
  return CommonChannelsActions.channelStepCall({
    stepKey: 'step2Form',
    payload: { endpoint, data },
    keyToState,
    successNextAction
  })
}

export function getStep2 (entityId, language) {
  return step2Call('channel/GetAddPhoneChannelStep2', { entityId, language })
}

export function saveStep2Changes ({ language, successNextAction }) {
  return (props) => {
    return step2Call(
      'channel/SavePhoneChannelStep2Changes',
      commonChannelSelector.getSaveStepOpeningHoursModel(props.getState(), { keyToState, language }),
      successNextAction
    )(props)
}
}

// save all
export function saveAllChanges ({ language }) {
  return (props) => {
return CommonChannelsActions.channelCall(
				'channel/PhoneSaveAllChanges',
  {
    step1Form: phoneSelector.getSaveStep1Model(props.getState(), { keyToState, language }),
    step2Form: commonChannelSelector.getSaveStepOpeningHoursModel(props.getState(), { keyToState, language }),
    language
  },
				keyToState)(props)
}
}
