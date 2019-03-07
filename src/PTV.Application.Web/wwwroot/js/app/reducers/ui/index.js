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
import { handleActions, createAction } from 'redux-actions'
import { Map, fromJS } from 'immutable'
import { API_CALL_FAILURE } from 'Containers/Common/Actions'
import { isArray } from 'lodash'

const initialState = Map()
export const uiErrorDialogCustomReducer = handleActions({
  [API_CALL_FAILURE]: (state, payload) => state.mergeDeep({
    isOpen: true,
    errorMessage: payload.error,
    errorCode: payload.errorObject && payload.errorObject.code || null,
    errorObject: payload.errorObject
  })
}, initialState)

export const SET_IN_UI_STATE = 'SET_IN_UI_STATE'
export const setInUIState = createAction(SET_IN_UI_STATE)

export const MERGE_IN_UI_STATE = 'MERGE_IN_UI_STATE'
export const mergeInUIState = createAction(MERGE_IN_UI_STATE)

export const DELETE_IN_UI_STATE = 'DELETE_IN_UI_STATE'
export const deleteInUIState = createAction(DELETE_IN_UI_STATE)

export const DELETE_UI_STATE = 'DELETE_UI_STATE'
export const deleteUIState = createAction(DELETE_UI_STATE)

export default handleActions({
  [SET_IN_UI_STATE]: (state, { payload: { path = [], key, value } }) => {
    value = fromJS(value)
    path = isArray(path)
      ? [key, ...path]
      : [key, path]
    return state.setIn(path, value)
  },
  [MERGE_IN_UI_STATE]: (state, { payload: { key, value } }) =>
    state.mergeIn([key], fromJS(value)),
  [DELETE_IN_UI_STATE]: (state, { payload: { key, value } }) =>
    state.deleteIn([key]),
  [DELETE_UI_STATE]: state => state.clear()
}, Map())
