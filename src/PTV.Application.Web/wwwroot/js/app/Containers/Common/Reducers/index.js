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
import { combineReducers } from 'redux-immutable'
import * as ActionTypes from '../Actions'
import nodes from './Nodes'
import { Map, List } from 'immutable'
import zip from 'lodash/zipWith'
import getResults, { merger, mergerWithReplace } from '../../../Middleware'

function entities (state = Map(), action) {
  // called by apiCall - response (success)
  if (action.response && action.response.entities) {
    state = state.mergeWith(mergerWithReplace, action.response.entities)
  }

  if (action.payload && action.payload.clearEntities && action.payload.clearEntities.size > 0 && action.payload.property) {
    if (action.payload.clearEntities && action.payload.clearEntities.size > 0) {
      action.payload.clearEntities.forEach(id => state = state.deleteIn(Array.isArray(action.payload.property) ? action.payload.property.concat([id]) : [action.payload.property, id]))
    }
  }

  if (action.payload && action.payload.clearAllEntity && action.payload.clearAllEntity === true) {
    state = state.delete(action.payload.property)
  }

  // onEntityInputChange action - isSet = true
  // onEntityObjectChange action - isSet = true
  if (action.payload && action.payload.property && action.payload.objectToMerge) {
    state = state.mergeIn(action.payload.property, action.payload.objectToMerge)
  }

  // onEntityListChange action
  if (action.payload && action.payload.property && action.payload.item) {
    if (!state.getIn(action.payload.property)) {
      state = state.setIn(action.payload.property, List())
    }

    state = action.payload.isAdd ? state.updateIn(action.payload.property, (list) => list.push(action.payload.item))
      : state.updateIn(action.payload.property, (list) => list.filter((item) => (typeof action.payload.item === 'string' && item != action.payload.item) ||
        (typeof action.payload.item === 'object' && item.id != action.payload.item.id)))
  }

  // onEntityInputChange action - isSet = false
  // onEntityObjectChange action - isSet = false
  // onEntityAdd
  if (action.payload && action.payload.entities) {
    state = state.mergeWith(merger, action.payload.entities)
  }

  // called by apiCall - request
  if (action.request && action.request.entities) {
    state = state.mergeWith(merger, action.request.entities)
  }

  return state
}

function result (state = Map(), action) {
  if (action.response && action.response.results) {
    let newState = state.mergeDeep(action.response.results)
    return newState
  }
  if (action.type === ActionTypes.CLEAR_RESULT) {
    return state.delete(action.payload)
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

// Updates error message to notify about the failed fetches.
function notifications (state = new Map(), action) {
  const { type, error, response, request } = action

  switch (type) {
    case ActionTypes.RESET_ALL_MESSAGE:
      return Map()
    case ActionTypes.RESET_MESSAGE:
      return state.updateIn(action.payload, Map(), m => m.clear())
  }
  if (error) {
    console.error(action.error)
    return state.update('errors', e => (e || List()).push(Map({ error: action.error, stack: action.errorObject ? action.errorObject.stack : '' })))
  }
  const allMessages = (response && response.messages) || (request && request.messages)
  if (allMessages) {
    let messages = {}
    if (allMessages.errors && allMessages.errors.length > 0) {
      let errors = allMessages.errors
      if (allMessages.stackTrace.length > 0) {
        errors = zip(errors, allMessages.stackTrace, (e, s) => Map({ error: e, stack: s }))
      }
      messages['errors'] = errors
    }
    if (allMessages.infos && allMessages.infos.length > 0) {
      messages['infos'] = allMessages.infos
    }
    if (allMessages.warnings && allMessages.warnings.length > 0) {
      messages['warnings'] = allMessages.warnings
    }
    const keys = action.keys || ['global']
    if (Object.keys(messages).length > 0) {
      state = state.updateIn(keys, x => (x || Map()).mergeWith(merger, messages))
    }
  }

  if (request && action.keys && action.clearMessages) {
    state = state.deleteIn(action.keys)
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
  enums,
  nodes
})

export default commonReducer
