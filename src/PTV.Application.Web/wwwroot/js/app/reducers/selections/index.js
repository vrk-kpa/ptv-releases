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
import { fromJS } from 'immutable'
import { handleActions, createAction } from 'redux-actions'
import { isUndefined } from 'lodash'

export const SET_SELECTED_ENTITY = 'SET_SELECTED_ENTITY'
export const setSelectedEntity = createAction(SET_SELECTED_ENTITY)

export const SET_SELECTED_ENTITY_ID = 'SET_SELECTED_ENTITY_ID'
export const setSelectedEntityId = createAction(SET_SELECTED_ENTITY_ID)

export const SET_SELECTED_ENTITY_TYPE = 'SET_SELECTED_ENTITY_TYPE'
export const setSelectedEntityType = createAction(SET_SELECTED_ENTITY_TYPE)

export const SET_CONTENT_LANGUAGE = 'SET_CONTENT_LANGUAGE'
export const setContentLanguage = createAction(SET_CONTENT_LANGUAGE)

export const SET_CONTENT_LANGUAGE_ID = 'SET_CONTENT_LANGUAGE_ID'
export const setContentLanguageId = createAction(SET_CONTENT_LANGUAGE_ID)

export const SET_CONTENT_LANGUAGE_CODE = 'SET_CONTENT_LANGUAGE_CODE'
export const setContentLanguageCode = createAction(SET_CONTENT_LANGUAGE_CODE)

export const CLEAR_CONTENT_LANGUAGE = 'CLEAR_CONTENT_LANGUAGE'
export const clearContentLanguage = createAction(CLEAR_CONTENT_LANGUAGE)

export const SET_COMPARISION_LANGUAGE = 'SET_COMPARISION_LANGUAGE'
export const setComparisionLanguage = createAction(SET_COMPARISION_LANGUAGE)

export const SET_COMPARISION_LANGUAGE_ID = 'SET_COMPARISION_LANGUAGE_ID'
export const setComparisionLanguageId = createAction(SET_COMPARISION_LANGUAGE_ID)

export const SET_COMPARISION_LANGUAGE_CODE = 'SET_COMPARISION_LANGUAGE_CODE'
export const setComparisionLanguageCode = createAction(SET_COMPARISION_LANGUAGE_CODE)

export const CLEAR_COMPARISION_LANGUAGE = 'CLEAR_COMPARISION_LANGUAGE'
export const clearComparisionLanguage = createAction(CLEAR_COMPARISION_LANGUAGE)

export const SET_PREVIEWDIALOG_LANGUAGE = 'SET_PREVIEWDIALOG_LANGUAGE'
export const setPreviewDialogLanguage = createAction(SET_PREVIEWDIALOG_LANGUAGE)

export const SET_PREVIEWDIALOG_LANGUAGE_ID = 'SET_PREVIEWDIALOG_LANGUAGE_ID'
export const setPreviewDialogLanguageId = createAction(SET_PREVIEWDIALOG_LANGUAGE_ID)

export const SET_PREVIEWDIALOG_LANGUAGE_CODE = 'SET_PREVIEWDIALOG_LANGUAGE_CODE'
export const setPreviewDialogLanguageCode = createAction(SET_PREVIEWDIALOG_LANGUAGE_CODE)

export const CLEAR_PREVIEWDIALOG_LANGUAGE = 'CLEAR_PREVIEWDIALOG_LANGUAGE'
export const clearPreviewDialogLanguage = createAction(CLEAR_PREVIEWDIALOG_LANGUAGE)

export const SET_CONNECTION_LANGUAGE = 'SET_CONNECTION_LANGUAGE'
export const setConnectionLangauge = createAction(SET_CONNECTION_LANGUAGE)

export const SET_CONNECTION_LANGUAGE_ID = 'SET_CONNECTION_LANGUAGE_ID'
export const setConnectionLangaugeId = createAction(SET_CONNECTION_LANGUAGE_ID)

export const SET_CONNECTION_LANGUAGE_CODE = 'SET_CONNECTION_LANGUAGE_CODE'
export const setConnectionLangaugeCode = createAction(SET_CONNECTION_LANGUAGE_CODE)

export const CLEAR_CONNECTION_LANGUAGE = 'CLEAR_CONNECTION_LANGUAGE'
export const clearConnectionLangauge = createAction(CLEAR_CONNECTION_LANGUAGE)

export const SET_SEARCH_DOMAIN = 'SET_SEARCH_DOMAIN'
export const setSearchDomain = createAction(SET_SEARCH_DOMAIN)

export const initialState = fromJS({
  entity: {
    id: null,
    type: null,
    concreteType: null
  },
  contentLanguage: {
    id: null,
    code: null
  },
  comparisionLanguage: {
    id: null,
    code: null
  },
  previewDialogLanguage: {
    id: null,
    code: null
  },
  connectionLanguage: {
    id: null,
    code: null
  }
})
export default handleActions({
  // Selected entity //
  [SET_SELECTED_ENTITY]: (state, { payload }) =>
      state.setIn(['entity', 'id'], payload.id)
      .setIn(['entity', 'type'], payload.type)
      .setIn(['entity', 'concreteType'], payload.concreteType),
  [SET_SELECTED_ENTITY_ID]: (state, { payload }) => state.setIn(['entity', 'id'], payload),
  [SET_SELECTED_ENTITY_TYPE]: (state, { payload }) => state.setIn(['entity', 'type'], payload),
  // Selected content language //
  [SET_CONTENT_LANGUAGE]: (state, { payload }) => payload.id && payload.code
    ? state
        .setIn(['contentLanguage', 'id'], payload.id)
        .setIn(['contentLanguage', 'code'], payload.code)
    : state,
  [SET_CONTENT_LANGUAGE_ID]: (state, { payload }) => state.setIn(['contentLanguage', 'id'], payload),
  [SET_CONTENT_LANGUAGE_CODE]: (state, { payload }) => state.setIn(['contentLanguage', 'code'], payload),
  [CLEAR_CONTENT_LANGUAGE]: state => state
    .setIn(['contentLanguage', 'id'], null)
    .setIn(['contentLanguage', 'code'], null),
  // Selected comparision language //
  [SET_COMPARISION_LANGUAGE]: (state, { payload }) => !isUndefined(payload.id) && !isUndefined(payload.code)
    ? state
        .setIn(['comparisionLanguage', 'id'], payload.id)
        .setIn(['comparisionLanguage', 'code'], payload.code)
    : state,
  [SET_COMPARISION_LANGUAGE_ID]: (state, { payload }) => state.setIn(['comparisionLanguage', 'id'], payload),
  [SET_COMPARISION_LANGUAGE_CODE]: (state, { payload }) => state.setIn(['comparisionLanguage', 'code'], payload),
  [CLEAR_COMPARISION_LANGUAGE]: state => state
    .setIn(['comparisionLanguage', 'id'], null)
    .setIn(['comparisionLanguage', 'code'], null),
  // Selected preview dialog language //
  [SET_PREVIEWDIALOG_LANGUAGE]: (state, { payload }) => {
    return payload.id && payload.code
      ? state
          .setIn(['previewDialogLanguage', 'id'], payload.id)
          .setIn(['previewDialogLanguage', 'code'], payload.code)
      : state
  },
  [SET_PREVIEWDIALOG_LANGUAGE_ID]: (state, { payload }) =>
    state.setIn(['previewDialogLanguage', 'id'], payload),
  [SET_PREVIEWDIALOG_LANGUAGE_CODE]: (state, { payload }) =>
    state.setIn(['previewDialogLanguage', 'code'], payload),
  [CLEAR_PREVIEWDIALOG_LANGUAGE]: state => state
    .setIn(['previewDialogLanguage', 'id'], null)
    .setIn(['previewDialogLanguage', 'code'], null),
  [SET_CONNECTION_LANGUAGE]: (state, { payload }) => {
    return payload.id && payload.code
      ? state
          .setIn(['connectionLanguage', 'id'], payload.id)
          .setIn(['connectionLanguage', 'code'], payload.code)
      : state
  },
  [SET_CONNECTION_LANGUAGE_ID]: (state, { payload }) =>
    state.setIn(['connectionLanguage', 'id'], payload),
  [SET_CONNECTION_LANGUAGE_CODE]: (state, { payload }) =>
    state.setIn(['connectionLanguage', 'code'], payload),
  [CLEAR_CONNECTION_LANGUAGE]: state => state
    .setIn(['connectionLanguage', 'id'], null)
    .setIn(['connectionLanguage', 'code'], null),
  [SET_SEARCH_DOMAIN]: (state, { payload }) => state.set('searchDomain', payload)
}, initialState)
