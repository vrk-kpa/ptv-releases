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
import { fromJS, Set } from 'immutable'
import { handleActions, createAction } from 'redux-actions'
import { isUndefined } from 'lodash'
import { APPLICATION_INIT } from 'actions/init'

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

export const SET_SEARCH_DOMAIN = 'SET_SEARCH_DOMAIN'
export const setSearchDomain = createAction(SET_SEARCH_DOMAIN)

export const SET_CONNECTIONS_MAIN_ENTIY = 'SET_CONNECTIONS_MAIN_ENTIY'
export const setConnectionsMainEntity = createAction(SET_CONNECTIONS_MAIN_ENTIY)

export const SET_CONNECTIONS_ENTITY = 'SET_CONNECTIONS_ENTITY'
export const setConnectionsEntity = createAction(SET_CONNECTIONS_ENTITY)

export const ADD_CONNECTIONS_ACTIVE_ENTITY = 'ADD_CONNECTIONS_ACTIVE_ENTITY'
export const addConnectionsActiveEntity = createAction(ADD_CONNECTIONS_ACTIVE_ENTITY)

export const SET_CONNECTIONS_ACTIVE_ENTITY = 'SET_CONNECTIONS_ACTIVE_ENTITY'
export const setConnectionsActiveEntity = createAction(SET_CONNECTIONS_ACTIVE_ENTITY)

export const REMOVE_CONNECTIONS_ACTIVE_ENTITY = 'REMOVE_CONNECTIONS_ACTIVE_ENTITY'
export const removeConnectionsActiveEntity = createAction(REMOVE_CONNECTIONS_ACTIVE_ENTITY)

export const CLEAR_CONNECTIONS_ACTIVE_ENTITY = 'CLEAR_CONNECTIONS_ACTIVE_ENTITY'
export const clearConnectionsActiveEntity = createAction(CLEAR_CONNECTIONS_ACTIVE_ENTITY)

export const SET_SHOULD_ADD_CHILD_TO_ALL_ENTITIES = 'SET_SHOULD_ADD_CHILD_TO_ALL_ENTITIES'
export const setShouldAddChildToAllEntities = createAction(SET_SHOULD_ADD_CHILD_TO_ALL_ENTITIES)

export const SET_CONNECTIONS_VIEW = 'SET_CONNECTIONS_VIEW'
export const setConnectionsView = createAction(SET_CONNECTIONS_VIEW)

export const SET_CONNECTION_OPEN_INDEX = 'SET_CONNECTION_OPEN_INDEX'
export const setConnectionOpenIndex = createAction(SET_CONNECTION_OPEN_INDEX)

export const SET_REVIEW_CURRENT_STEP = 'SET_REVIEW_CURRENT_STEP'
export const setReviewCurrentStep = createAction(SET_REVIEW_CURRENT_STEP)

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
  },
  connectionsMainEntity: null,
  connectionsEntity: null,
  connectionsActiveEntities: Set(),
  shouldAddChildToAllEntities: true,
  connectionOpenIndex: null,
  visitingAddressOpenIndex: 0,
  visitingAddressFocusIndex: 0,
  reviewCurrentStep: null
})
export default handleActions({
  // Selected entity //
  [SET_SELECTED_ENTITY]: (state, { payload }) =>
    state.setIn(['entity', 'id'], payload.id)
      .setIn(['entity', 'type'], payload.type)
      .setIn(['entity', 'concreteType'], payload.concreteType),
  [SET_SELECTED_ENTITY_ID]: (state, { payload }) =>
    state.setIn(['entity', 'id'], payload),
  [SET_SELECTED_ENTITY_TYPE]: (state, { payload }) =>
    state.setIn(['entity', 'type'], payload),
  // Selected content language //
  [SET_CONTENT_LANGUAGE]: (state, { payload }) => payload.id && payload.code
    ? payload.languageKey &&
      state
        .setIn(['contentLanguage', 'keys', payload.languageKey, 'id'], payload.id)
        .setIn(['contentLanguage', 'keys', payload.languageKey, 'code'], payload.code) ||
      state
        .setIn(['contentLanguage', 'id'], payload.id)
        .setIn(['contentLanguage', 'code'], payload.code)
    : state,
  [SET_CONTENT_LANGUAGE_ID]: (state, { payload }) =>
    payload.languageKey
      ? state.setIn(['contentLanguage', 'keys', payload.languageKey, 'id'], payload)
      : state.setIn(['contentLanguage', 'id'], payload),
  [SET_CONTENT_LANGUAGE_CODE]: (state, { payload }) =>
    payload.languageKey
      ? state.setIn(['contentLanguage', 'keys', payload.languageKey, 'code'], payload.code)
      : state.setIn(['contentLanguage', 'code'], payload),
  [CLEAR_CONTENT_LANGUAGE]: (state, { payload }) =>
    payload && payload.languageKey
      ? state
        .deleteIn(['contentLanguage', 'keys', payload.languageKey])
      : state
        .setIn(['contentLanguage', 'id'], null)
        .setIn(['contentLanguage', 'code'], null),
  // Selected comparision language //
  [SET_COMPARISION_LANGUAGE]: (state, { payload }) =>
    !isUndefined(payload.id) && !isUndefined(payload.code)
      ? state
        .setIn(['comparisionLanguage', 'id'], payload.id)
        .setIn(['comparisionLanguage', 'code'], payload.code)
      : state,
  [SET_COMPARISION_LANGUAGE_ID]: (state, { payload }) =>
    state.setIn(['comparisionLanguage', 'id'], payload),
  [SET_COMPARISION_LANGUAGE_CODE]: (state, { payload }) =>
    state.setIn(['comparisionLanguage', 'code'], payload),
  [CLEAR_COMPARISION_LANGUAGE]: state => state
    .setIn(['comparisionLanguage', 'id'], null)
    .setIn(['comparisionLanguage', 'code'], null),
  [SET_SEARCH_DOMAIN]: (state, { payload }) =>
    state.set('searchDomain', payload),
  // Connections //
  [SET_CONNECTIONS_MAIN_ENTIY]: (state, { payload }) =>
    payload === 'channels' || payload === 'services' || payload === null
      ? state.set('connectionsMainEntity', payload)
      : state,
  [SET_CONNECTIONS_ENTITY]: (state, { payload }) =>
    payload === 'channels' || payload === 'services' || payload === null
      ? state.set('connectionsEntity', payload)
      : state,
  [ADD_CONNECTIONS_ACTIVE_ENTITY]: (state, { payload }) =>
    state.update(
      'connectionsActiveEntities',
      activeEntities => activeEntities.add(payload)
    ),
  [SET_CONNECTIONS_ACTIVE_ENTITY]: (state, { payload }) =>
    state.set(
      'connectionsActiveEntities',
      payload
    ),
  [REMOVE_CONNECTIONS_ACTIVE_ENTITY]: (state, { payload }) =>
    state.update(
      'connectionsActiveEntities',
      activeEntities => activeEntities.delete(payload)
    ),
  [CLEAR_CONNECTIONS_ACTIVE_ENTITY]: state => state.set(
    'connectionsActiveEntities',
    Set()
  ),
  [SET_SHOULD_ADD_CHILD_TO_ALL_ENTITIES]: (state, { payload }) => state.set(
    'shouldAddChildToAllEntities',
    !!payload
  ),
  [SET_CONNECTIONS_VIEW]: (state, { payload }) =>
    state.set('connectionsPreview', payload
    ),
  [SET_CONNECTION_OPEN_INDEX]: (state, { payload }) =>
    state.set('connectionOpenIndex', payload
    ),
  [SET_REVIEW_CURRENT_STEP]: (state, { payload = null }) =>
    state.set('reviewCurrentStep', payload
    ),
  [APPLICATION_INIT]: state => initialState
}, initialState)
