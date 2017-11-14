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
import { Map } from 'immutable'

export const DISABLE_FORM = 'DISABLE_FORM'
export const disableForm = createAction(DISABLE_FORM)

export const ENABLE_FORM = 'ENABLE_FORM'
export const enableForm = createAction(ENABLE_FORM)

export const TOGGLE_DISABLE_FORM = 'TOGGLE_DISABLE_FORM'
export const toggleDisableForm = createAction(TOGGLE_DISABLE_FORM)

export const SET_IS_ADDING_NEW_LANGUAGE = 'SET_IS_ADDING_NEW_LANGUAGE'
export const setIsAddingNewLanguage = createAction(SET_IS_ADDING_NEW_LANGUAGE)

export const SET_NEWLY_ADDED_LANGUAGE = 'SET_NEWLY_ADDED_LANGUAGE'
export const setNewlyAddedLanguage = createAction(SET_NEWLY_ADDED_LANGUAGE)

export const CLEAR_NEWLY_ADDED_LANGUAGE = 'CLEAR_NEWLY_ADDED_LANGUAGE'
export const clearNewlyAddedLanguage = createAction(CLEAR_NEWLY_ADDED_LANGUAGE)

export const SET_READ_ONLY = 'SET_READ_ONLY'
export const setReadOnly = createAction(SET_READ_ONLY)

export const SET_FORM_LOADING = 'SET_FORM_LOADING'
export const setFormLoading = createAction(SET_FORM_LOADING)

export const TOGGLE_READ_ONLY = 'TOGGLE_READ_ONLY'
export const toggleReadOnly = createAction(TOGGLE_READ_ONLY)

export const SET_COMPARE_MODE = 'SET_COMPARE_MODE'
export const setCompareMode = createAction(SET_COMPARE_MODE)

export const SET_SUBMIT_SUCCESS = 'SET_SUBMIT_SUCCESS'
export const setSubmitSuccess = createAction(SET_SUBMIT_SUCCESS)

export const RESET_SUBMIT_SUCCESS = 'RESET_SUBMIT_SUCCESS'
export const resetSubmitSuccess = createAction(RESET_SUBMIT_SUCCESS)

export const SET_CONNECTIONS_READ_ONLY = 'SET_CONNECTIONS_READ_ONLY'
export const setConnectionReadOnly = createAction(SET_CONNECTIONS_READ_ONLY)

export const RESET_CONNECTIONS_READ_ONLY = 'RESET_CONNECTIONS_READ_ONLY'
export const resetConnectionReadOnly = createAction(RESET_CONNECTIONS_READ_ONLY)

export const initialState = Map()
export default handleActions({
  // Disabled //
  [DISABLE_FORM]: (state, { payload }) =>
    state.setIn([payload, 'disabled'], true),
  [ENABLE_FORM]: (state, { payload }) =>
    state.setIn([payload, 'disabled'], false),
  [SET_SUBMIT_SUCCESS]: (state, { payload }) =>
    state.setIn([payload, 'submitSuccessed'], true),
  [RESET_SUBMIT_SUCCESS]: (state, { payload }) =>
    state.setIn([payload, 'submitSuccessed'], false),
  [TOGGLE_DISABLE_FORM]: (state, { payload }) =>
    state.setIn([payload, 'disabled'], !state.getIn([payload, 'disabled'])),
  // Adding new language //
  [SET_IS_ADDING_NEW_LANGUAGE]: (state, { payload }) =>
    state.setIn([payload.form, 'addingNewLanguage'], payload.value),
  [SET_NEWLY_ADDED_LANGUAGE]: (state, { payload }) =>
    state.setIn([payload.form, 'newlyAddedLanguage'], payload.value),
  // ReadOnly //
  [SET_READ_ONLY]: (state, { payload }) =>
    state.setIn([payload.form, 'readOnly'], payload.value),
  [SET_FORM_LOADING]: (state, { payload }) =>
    state.setIn([payload.form, 'isLoading'], payload.value),
  [TOGGLE_READ_ONLY]: (state, { payload }) =>
    state.setIn([payload, 'readOnly'], !state.getIn([payload, 'readOnly'])),
  [SET_COMPARE_MODE]: (state, { payload }) =>
    state.setIn([payload.form, 'compare'], payload.value),
  [CLEAR_NEWLY_ADDED_LANGUAGE]: (state, { payload }) =>
    state.setIn([payload, 'newlyAddedLanguage'], null),
  [SET_CONNECTIONS_READ_ONLY]: (state, { payload: { form, index, value } }) =>
    state.setIn([form, 'readOnlyIndex', index], value),
  [RESET_CONNECTIONS_READ_ONLY]: (state, { payload: { form } }) =>
    state.setIn([form, 'readOnlyIndex'], Map())
}, initialState)
