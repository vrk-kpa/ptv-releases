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
import { combineReducers } from 'redux-immutable'
import { createSelector } from 'reselect'
import * as ActionTypes from 'actions'
import { Map, List } from 'immutable'
import getResults, { merger, mergerWithDeepCheck } from 'Middleware'

const getTranslationLanguiagesFromEntities = createSelector(
  entities => entities.getIn(['enums', 'enums', 'translationLanguages']) || List(),
  entities => entities.get('languages') || Map(),
  (languageIds, languages) => languageIds.map(id => languages.getIn([id, 'code'])).toSet()

)

export const CLEAR_IN_ENTITIES = 'clearInEntities'

function entities(state = Map(), action) {
  // called by apiCall - response (success)
  // console.log(action.type, action)
  if (action.response && action.response.entities) {
    const filteredKeys = getTranslationLanguiagesFromEntities(state)
    state = state.mergeWith(
      mergerWithDeepCheck(
        current => current.size === 0 ||
          current.size > filteredKeys.size ||
          current.keySeq().toSet().subtract(filteredKeys).size
      ),
      action.response.entities
    )
  }

  // called by apiCall - request
  if (action.request && action.request.entities) {
    state = state.mergeWith(merger, action.request.entities)
  }


  if (action.type && action.type === CLEAR_IN_ENTITIES && action.payload && action.payload.entityPath) {
    state = state.deleteIn(action.payload.entityPath)
  }
  return state
}

function result (state = Map(), action) {
  if (action.response && action.response.results) {
    let newState = state.mergeDeep(action.response.results)
    return newState
  }
  return state
}

function enums (state = Map(), action) {
  if (action.response && action.response.enums) {
    state = state.merge(action.response.enums)
  }

  if (action.response && action.response.entities && action.response.entities.enums) {
    state = state.merge(action.response.entities.enums.enums)
    // return newState
  }

  return state
}

const apiCallsImpl = (state, action) => {
  switch (action.type) {
    case ActionTypes.API_CALL_CLEAN:
      if (action.keys) {
        state = state.deleteIn(Array.isArray(action.keys) ? action.keys : [action.keys])
      }
      break
  }
  return state
}

const apiCalls = getResults({
  mapActionToKey: action => action.keys,
  types: [
    ActionTypes.API_CALL_REQUEST,
    ActionTypes.API_CALL_SUCCESS,
    ActionTypes.API_CALL_FAILURE
  ],
  processAction: apiCallsImpl
})

const commonReducer = combineReducers({
  apiCalls,
  entities,
  result,
  enums
})

export default commonReducer
