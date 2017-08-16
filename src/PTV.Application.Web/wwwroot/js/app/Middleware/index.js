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
import {Record, Iterable, Map, List} from 'immutable';
import merge from 'lodash/merge'
import union from 'lodash/union'

var isList = List.isList;

export function merger(a, b) {
  if (isList(a) && isList(b)) {
    return a.concat(b)
  }
  if (a && a.mergeWith) {
    return a.mergeWith(merger, b)
  }
  return b
}


export function mergerWithReplace(a, b) {
  if (isList(a)) {
    return b
  }
  if (a && a.mergeWith) {
    return a.mergeWith(mergerWithReplace, b)
  }
  return b
}

export default function getResults({ types, mapActionToKey, processAction }) {
  
  if (!Array.isArray(types) || types.length !== 3) {
    throw new Error('Expected types to be an array of three elements.')
  }
  if (!types.every(t => typeof t === 'string')) {
    throw new Error('Expected types to be strings.')
  }

  const [ requestType, successType, failureType ] = types

  function updateResults2(state = Map(), action) {
    switch (action.type) {
      case requestType:
        const objectToMerge = action.saveRequestData ? { isFetching: true, areDataValid: false, ...action.request.data } : { isFetching: true, areDataValid: false }; 
        return state.merge(objectToMerge);
      case successType:
        /*if (action.response.result)
        {
          return state.merge({ isFetching: false, areDataValid: true });
        }*/
        return state.mergeWith(mergerWithReplace, { ...action.response.model, isFetching: false, areDataValid: true });
      case failureType:
        return state.merge({
          isFetching: false,
          areDataValid: false,
        })
      default:
        return state
    }
  }

  return function getResultsByKey2(state = Map(), action) {
    switch (action.type) {
      case requestType:
      case successType:
      case failureType:
        if (typeof mapActionToKey === 'function') {
          const key = mapActionToKey(action)
          if (typeof key !== 'string' && !Array.isArray(key)) {
            throw new Error('Expected key to be a string or an array.')
          }

          state = state.setIn(Array.isArray(key) ? key : [key], updateResults2(state.getIn(key), action));
        } else {
          state = updateResults2(state, action);
        }  
	    break;
      }

      if (typeof processAction === 'function')
      {
          return processAction(state, action);
      }
      return state
  }
}