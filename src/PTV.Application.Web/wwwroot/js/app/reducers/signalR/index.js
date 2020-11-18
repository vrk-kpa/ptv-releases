/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { handleActions, createAction } from 'redux-actions'
import { Map } from 'immutable'

export const CREATE_HUB_CONNECTION = 'CREATE_HUB_CONNECTION'

export const SET_HUB_REQUEST_SENT = 'SET_HUB_REQUEST_SENT'
export const setHubRequestSent = createAction(SET_HUB_REQUEST_SENT)

export const CLEAR_HUB_REQUST_SENT = 'CLEAR_HUB_REQUST_SENT'
export const clearHubRequestSent = createAction(CLEAR_HUB_REQUST_SENT)

export const setHubConnection = (
  hubName,
  hub
) => (
  {
    type: CREATE_HUB_CONNECTION,
    payload: { hubName, hub }
  }
)

// sarzijan:remove
export const TEST_CALL_MASS_ACTION = 'TEST_CALL_MASS_ACTION'

// sarzijan:remove
export const testCallMassFunction = (
) => (
  {
    type: TEST_CALL_MASS_ACTION
  }
)

export const initialState = Map()

export default handleActions({
  [CREATE_HUB_CONNECTION]: (state, { payload: { hubName, hub } }) => state.setIn(['connections', hubName], hub),
  [SET_HUB_REQUEST_SENT]: (state, { payload: { hubName } }) => state.setIn(['calls', hubName], true),
  [CLEAR_HUB_REQUST_SENT]: (state, { payload: { hubName } }) => state.setIn(['calls', hubName], false)
}, initialState)
