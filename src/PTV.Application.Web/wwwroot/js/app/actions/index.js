/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import { CALL_API } from 'Middleware/Api'
import { getShowErrorsAction, getShowInfosAction } from 'reducers/notifications'

export const API_CALL_REQUEST = 'API_CALL_REQUEST'
export const API_CALL_SUCCESS = 'API_CALL_SUCCESS'
export const API_CALL_FAILURE = 'API_CALL_FAILURE'

export function apiCall3 ({
  keys,
  payload,
  typeSchemas,
  saveRequestData,
  schemas,
  clearRequest,
  successNextAction,
  onErrorAction,
  onInfoAction,
  formName
}) {
  return ({ getState, dispatch }) => {
    const errorAction = onErrorAction || formName && getShowErrorsAction(formName, true) || null
    const infoAction = onInfoAction || formName && getShowInfosAction(formName, true) || null
    return {
      keys,
      saveRequestData,
      clearRequest,
      [CALL_API]: {
        types: [API_CALL_REQUEST,
          API_CALL_SUCCESS,
          API_CALL_FAILURE],
        payload,
        typeSchemas,
        schemas,
        successNextAction,
        onErrorAction: errorAction,
        onInfoAction: infoAction
      }
    }
  }
}

