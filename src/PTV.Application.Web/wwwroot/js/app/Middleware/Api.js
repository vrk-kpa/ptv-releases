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
import { normalize } from 'normalizr'
import { camelizeKeys } from 'humps'
import 'isomorphic-fetch'
import { API_ROOT, API_WEB_ROOT, getAuthToken } from 'Configuration/AppHelpers'
import { property } from 'lodash'
import { mergeInUIState } from 'reducers/ui'
import { errorDialogKey } from 'enums'
import { CALL_API } from 'util/symbols'

const customAssign = (languageCode) => (output, key, value, input, schema) => {
  const language = languageCode || 'fi'

  if (schema && schema.getMeta('localizable')) {
    if (!output[language]) {
      output[language] = {}
    }
    output[language][key] = value
    if (key === 'publishingStatusId' || key === 'languagesAvailabilities') {
      output[key] = value
    }
  } else {
    output[key] = value
  }
}

export const normalizeData = (object, schema, langCode) => {
  const normalized = normalize(object, schema, { assignEntity: customAssign(langCode) })
  return normalized
}

class ResponseError extends Error {
  constructor (response) {
    super(response.statusText)
    this.code = response.status
  }
}

export const normalizeResponse = ({ json, schemas }) => {
  const camelizedJson = json.doNotCamelize && json || camelizeKeys(json)
  // if (json.translatedData) {
  //   camelizedJson.data = { ...camelizedJson.data, localizationMessages: json.translatedData }
  // }

  let result = {}
  let resultModel = camelizedJson.data

  // normalize data by schema
  if (schemas && camelizedJson.data) {
    // normalize schema arrays
    if (Array.isArray(schemas)) {
      console.error('NORMALIZE: not supported operation - schemas array', camelizedJson.data, schemas)
    } else {
      // normalize schema for one model
      // if (camelizedJson.data.id == null) {
      //   camelizedJson.data.id = clientId
      // }

      result = normalizeData(camelizedJson.data, schemas, json.language)
      resultModel = { result: result.result }
    }
  }

  result.result = resultModel
  if (camelizedJson.messages) {
    result = { ...result, messages: camelizedJson.messages }
  }

  return result
}

const getAbortController = canAbort => {
  if (canAbort) {
    if (!('AbortController' in window)) {
      console.warn('Abort is not supported')
    } else {
      return new AbortController()
    }
  }
}

// Fetches an API response and normalizes the result JSON according to schema.
// This makes every API response have the same shape, regardless of how nested it was.
function callApi (endpoint, schemas, typeSchemas, options, clientId, canAbort) {
  // checkif endpoint already contains base asi url or base web url
  const fullUrl = (endpoint.indexOf(API_ROOT) !== -1) || (endpoint.indexOf(API_WEB_ROOT) !== -1)
    ? endpoint
    : API_ROOT + endpoint

  return fetch(fullUrl, options)
    .then(response => {
      if (!response.ok) {
        return Promise.reject(new ResponseError(response))
      }
      return response.json().then(json => ({ json, response }))
    }).then(({ json, response }) => {
      if (!response.ok) {
        return Promise.reject(json)
      }

      // normalize static types
      if (typeSchemas) {
        if (Array.isArray(typeSchemas)) {
          console.error('NORMALIZE: not supported operation - types schemas array', json, typeSchemas)
        } else {
          console.error('NORMALIZE: not supported operation - types schema', json, typeSchemas)
        }
      }
      return normalizeResponse({ json, schemas })
    }).catch(error => {
      if (error.name === 'AbortError') {
        return Promise.reject(error)
      }
      console.error(error)
      return Promise.reject(error)
    })
}

function getOptions (data, action, abortController) {
  var token = getAuthToken() || null

  let options = {
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin' : '*'
    }
  }

  if (token) {
    options.headers.Authorization = 'Bearer ' + token
  }

  if (data) {
    options.method = 'post'
    let dataToServer = { ...data }
    if (action && action.clearRequest) {
      action.clearRequest.map(key => delete dataToServer[key])
    }
    options.body = JSON.stringify(dataToServer)
  }

  if (abortController) {
    options.signal = abortController.signal
  }
  return options
}

export function callApiDirect (endpoint, data) {
  const options = getOptions(data)
  return callApi(endpoint, null, null, options)
}

const getError = (error) => {
  if (error instanceof Error) {
    return {
      ...error,
      stack: error.stack,
      message: error.message,
      name: error.name,
      fileName: error.fileName,
      lineNumber: error.lineNumber,
      columnNumber: error.columnNumber
    }
  }
  return {}
}

// A Redux middleware that interprets actions with CALL_API info specified.
// Performs the call and promises when such actions are dispatched.
export default store => next => action => {
  const callAPI = action[CALL_API]
  if (typeof callAPI === 'undefined') {
    return next(action)
  }

  let { payload, payload: { data }, payload: { endpoint } } = callAPI
  const {
    schemas,
    types,
    typeSchemas,
    clientId,
    successNextAction,
    onErrorAction,
    onInfoAction,
    canAbort = true
  } = callAPI

  if (typeof endpoint === 'function') {
    endpoint = endpoint(store.getState())
  }

  if (typeof endpoint !== 'string') {
    throw new Error('Specify a string endpoint URL.')
  }

  if (!Array.isArray(types) || types.length !== 3) {
    throw new Error('Expected an array of three action types.')
  }
  if (!types.every(type => typeof type === 'string')) {
    throw new Error('Expected action types to be strings.')
  }

  const abortController = getAbortController(canAbort)
  let options = getOptions(data, action, abortController)

  function actionWith (data) {
    const finalAction = Object.assign({}, action, data)
    delete finalAction[CALL_API]
    return finalAction
  }

  const handleMessages = (messageType, callback, response) => {
    const messages = property(`response.messages.${messageType}`)(response)
    if (messages && messages.length > 0 && typeof callback === 'function') {
      store.dispatch(callback(messages))
    }
  }

  const [ requestType, successType, failureType ] = types
  next(actionWith({
    type: requestType,
    request: payload,
    abort: abortController
      ? () => abortController && abortController.abort()
      : null
  }))
  return callApi(endpoint, schemas, typeSchemas, options, clientId)
    .then(
      response => next(actionWith({
        response,
        type: successType
      })),
      error => {
        const errorObject = getError(error)
        const errorMessage = error.message || 'Something bad happened'

        if (error.name !== 'AbortError') {
          store.dispatch(mergeInUIState({
            key: errorDialogKey,
            value: {
              isOpen: true,
              errorMessage,
              errorCode: errorObject && errorObject.code || null,
              errorObject
            }
          }))
        }

        return next(actionWith({
          type: failureType,
          errorObject,
          error: errorMessage
        }))
      }
    ).then(
      response => {
        if (successNextAction && typeof successNextAction === 'function') {
          const errors = property(`response.messages.errors.length`)(response)
          const customAction = errors === 0 && successNextAction(response)
          if (customAction) {
            if (typeof customAction === 'function') {
              store.dispatch(customAction)
            } else {
              next(customAction)
            }
          }
        }
        handleMessages('errors', onErrorAction, response)
        handleMessages('infos', onInfoAction, response)
      })
}
