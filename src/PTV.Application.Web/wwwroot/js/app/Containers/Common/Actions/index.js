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
import { Schema, arrayOf, normalize } from 'normalizr'
// import * as CommonSelectors from '../Selectors';
import { List } from 'immutable'
// import { CommonSchemas } from '../Schemas';
import { CALL_API } from 'util/symbols'

export const CLEAN_ALL_STEPS = 'CLEAN_ALL_STEPS'
export const ENTITY_INPUT_CHANGE = 'ENTITY_INPUT_CHANGE'

export function onEntityAdd (object, schema) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: normalize(object, schema)
  }
}

export function onLocalizedEntityAdd (object, schema, langCode) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: normalizeData(object, schema, langCode)
  }
}

export function fakeApiCall (object, schema) {
  const response = normalize(object, schema)
  response['errors'] = []
  response['infos'] = []
  response['warnings'] = []
  return {
    type: ENTITY_INPUT_CHANGE,
    response:response
  }
}

export function onEntityClearAll (entityType) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: {
      clearAllEntity: true,
      property: entityType
    }
  }
}

export function onEntityClear (entityType, ids) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: {
      clearEntities: ids ? List.isList(ids) ? ids : List([ids]) : null,
      property: entityType
    }
  }
}

export function onEntityInputChange (entityType, id, input, value, isSet) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: !isSet ? { entities: { [entityType]: { [id]: { [input]: value } } }, pageModeState:{ [id]: { isDirty: true } } }
      : { property: [entityType, id], objectToMerge: { [input]: value } }
  }
}

export function onLocalizedEntityInputChange (entityType, id, input, value, langCode, isSet) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: !isSet ? { entities: { [entityType]: { [id]: { [langCode]: { [input]: value } } } } }
      : { property: [entityType, id, langCode], objectToMerge: { [input]: value } }
  }
}

export function onTranslatedEntityInputChange (entityType, id, input, value, langCode, isSet) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: !isSet ? { entities: { [entityType]: { [id]: { [input]: { [langCode]: value } } } } }
      : { property: [entityType, id, input], objectToMerge: { [langCode]: value } }
  }
}

export function onEntityObjectChange (entityType, id, object, isSet) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: !isSet ? { entities: { [entityType]: { [id]:  object } } }
      : { property: [entityType, id], objectToMerge:  object }
  }
}

export function onLocalizedEntityObjectChange (entityType, id, object, langCode, isSet) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload: !isSet ? { entities:{ [entityType]:  { [id]: { [langCode]:  object } } } }
      : { property: [entityType, id, langCode], objectToMerge:  object }
  }
}

export function onEntityListChange (entityType, id, input, value, isAdd) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload:  { property: Array.isArray(input) ? [entityType, id].concat(input) : [entityType, id, input], item: value, isAdd }
  }
}

export function onLocalizedEntityListChange (entityType, id, input, value, langCode, isAdd) {
  return {
    type: ENTITY_INPUT_CHANGE,
    payload:  { property: Array.isArray(input) ? [entityType, id, langCode].concat(input) : [entityType, id, langCode, input], item: value, isAdd }
  }
}

export function cleanAllSteps (value) {
  return () => ({
    type: CLEAN_ALL_STEPS,
    payload: value
  })
}

export const CLEAR_RESULT = 'CLEAR_RESULT'

export function clearResult (id) {
  return () => ({
    type: CLEAR_RESULT,
    payload: id
  })
}

export const RESET_MESSAGE = 'RESET_MESSAGE'

export function resetMessages (keys) {
  return () => ({
    type: RESET_MESSAGE,
    payload: keys
  })
}

export const RESET_ALL_MESSAGE = 'RESET_ALL_MESSAGE'

// Resets the currently visible All message.
export function resetAllMessage () {
  return {
    type: RESET_ALL_MESSAGE
  }
}

export const API_CALL_REQUEST = 'API_CALL_REQUEST'
export const API_CALL_SUCCESS = 'API_CALL_SUCCESS'
export const API_CALL_FAILURE = 'API_CALL_FAILURE'
export const API_CALL_CLEAN = 'API_CALL_CLEAN'

export function apiCall (keys, payload, typeSchemas, schemas, keyToState, useGivenId, saveRequestData = false) {
  return ({ getState }) => {
    return {
      keys,
      keyToState,
      saveRequestData,
      [CALL_API]: {
        types: [API_CALL_REQUEST,
          API_CALL_SUCCESS,
          API_CALL_FAILURE],
        payload,
        // typeSchemas,
        schemas
      }
    }
  }
}

export function apiCall2 ({ keys, payload, typeSchemas, schemas, keyToState, useGivenId, saveRequestData = false, successNextAction }) {
  return ({ getState }) => {
    return {
      keys,
      keyToState,
      saveRequestData,
      [CALL_API]: {
        types: [API_CALL_REQUEST,
          API_CALL_SUCCESS,
          API_CALL_FAILURE],
        payload,
        typeSchemas: typeSchemas,
        schemas,
        successNextAction
      }
    }
  }
}

// export const loadTranslatableLanguages = () => {
// 	return apiCall(['translationLanguages'], { endpoint: 'common/GetTranslationLanguages', data: {} }, [], CommonSchemas.TRANSLATED_LANGUAGE_ARRAY);
// }

// export const loadPublishingStatuses = () => {
// 	return apiCall(['publishingStatuses'], { endpoint: 'common/GetPublishingStatuses', data: {} }, [], CommonSchemas.PUBLISHING_STATUS_ARRAY);
// }

export const SET_LANGUAGE_TO = 'SET_LANGUAGE_TO'

export function setLanguageTo (key, id) {
  return () => ({
    type: SET_LANGUAGE_TO,
    payload: {
      keyToState : key,
      languageId :id
    }
  })
}

export const FORCE_RELOAD = 'FORCE_RELOAD'
export function forceReload (key, forceReload) {
  return () => ({
    type: FORCE_RELOAD,
    payload: {
      keyToState : key,
      forceReload
    }
  })
}

export const SET_LANGUAGE_FROM = 'SET_LANGUAGE_FROM'

export function setLanguageFrom (key, id) {
  return () => ({
    type: SET_LANGUAGE_FROM,
    payload: {
      keyToState : key,
      languageId :id
    }
  })
}

export function clearApiCall (keys, response) {
  return {
    keys,
    type: API_CALL_SUCCESS,
    response: response
  }
}

export function deleteApiCall (keys) {
  return {
    keys,
    type: API_CALL_CLEAN
  }
}
