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
import { ResponseError } from './Errors'
export const normalizeData = (object, schema, langCode) => {
  const normalized = normalize(object, schema)
  return normalized
}

export const normalizeResponse = ({ json, schemas }) => {
  const camelizedJson = json.doNotCamelize && json || camelizeKeys(json)
  let result = {}
  let resultModel = camelizedJson.data

  // normalize data by schema
  if (schemas && camelizedJson.data) {
    // normalize schema arrays
    if (Array.isArray(schemas)) {
      console.error('NORMALIZE: not supported operation - schemas array', camelizedJson.data, schemas)
    } else {
      // normalize schema for one model
      result = normalizeData(camelizedJson.data, schemas, json.language)
      resultModel = { result: result.result }
    }
  }

  result.result = resultModel
  // if (camelizedJson.messages) {
  //   result = { ...result, messages: camelizedJson.messages }
  // }

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
function callApi ({ endpoint, schemas, options }) {
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
      return {
        normalized: normalizeResponse({ json, schemas }),
        original: json
      }
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
  return callApi({ endpoint, options })
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

const handleError = (error, dispatch) => {
  const errorObject = getError(error)
  const errorMessage = error.message || 'Something bad happened'

  if (error.name !== 'AbortError') {
    dispatch(mergeInUIState({
      key: errorDialogKey,
      value: {
        isOpen: true,
        errorMessage,
        errorCode: errorObject && errorObject.code || null,
        errorObject
      }
    }))
  }

  return { errorObject, errorMessage }
}

const innerCallApi = async (route, body, method = 'POST', withAuth = true) => {
  let options = {
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'access-control-allow-origin': '*'
    }
  }

  if (withAuth) {
    const token = getAuthToken()
    if (token) {
      options.headers.Authorization = `Bearer ${getAuthToken()}`
    }
  }

  if (body) {
    options = {
      ...options,
      body: JSON.stringify(body),
      method
    }
  } else {
    options = {
      ...options,
      method: 'GET'
    }
  }
  const routeUrl = `${API_ROOT}${route}`
  return await fetch(routeUrl, options)
    .then(response => {
      if (!response.ok) {
        return Promise.reject(new ResponseError(response))
      }
      return response.json().then(json => ({ json, response }))
    }).then(({ json, response }) => {
      if (!response.ok) {
        return Promise.reject(json)
      }
      return json
    }).catch(error => {
      if (error.name === 'AbortError') {
        return Promise.reject(error)
      }
      console.error(error)
      return Promise.reject(error)
    })
}

export const asyncCallApi = async (url, values, dispatch, customException, method = 'POST', withAuth = true) => {
  return await innerCallApi(url, values, method, withAuth)
    .then(
      response => {
        return response
      },
      error => {
        handleError(error, dispatch)

        if (customException) {
          throw customException
        }
      }
    )
}

export const asyncDownloadFileApi = async (url, contentType, fileName) => {
  let options = {
    headers: {
      'Accept': contentType,
      'Content-Type': contentType,
      'access-control-allow-origin': '*'
    }
  }
  const token = getAuthToken()
  if (token) {
    options.headers.Authorization = `Bearer ${getAuthToken()}`
  }
  const routeUrl = `${API_ROOT}${url}`
  return await fetch(routeUrl, options)
    .then(response => {
      if (!response.ok) {
        return Promise.reject(new ResponseError(response))
      }
      return response.blob()
    })
    .then(blob => {
      if (window.navigator.msSaveOrOpenBlob) { // fix for IE 11
        window.navigator.msSaveOrOpenBlob(blob, fileName)
      } else {
        const url = window.URL.createObjectURL(new Blob([blob]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', fileName)
        document.body.appendChild(link)
        link.click()
        link.parentNode.removeChild(link)
      }
    })
    .catch(error => {
      if (error.name === 'AbortError') {
        return Promise.reject(error)
      }
      console.error(error)
      return Promise.reject(error)
    })
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
    successAction,
    successNextAction,
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

  const [ requestType, successType, failureType ] = types
  next(actionWith({
    type: requestType,
    request: payload,
    abort: abortController
      ? () => abortController && abortController.abort()
      : null
  }))
  return callApi({ endpoint, schemas, typeSchemas, options, clientId })
    .then(
      response => {
        next(actionWith({
          response: property('normalized')(response),
          type: successType
        }))
        return response
      },
      error => {
        const { errorObject, errorMessage } = handleError(error, store.dispatch)

        return next(actionWith({
          type: failureType,
          errorObject,
          error: errorMessage
        }))
      }
    ).then(
      ({ normalized, original }) => {
        if (successNextAction && typeof successNextAction === 'function') {
          const errors = property(`messages.errors.length`)(original)
          const customAction = errors === 0 && successNextAction({ response: normalized })
          console.warn('Function "successNextAction" is obsolete. Use successAction instead.')
          if (customAction) {
            if (typeof customAction === 'function') {
              store.dispatch(customAction)
            } else {
              next(customAction)
            }
          }
        }
        if (typeof successAction === 'function') {
          store.dispatch(
            successAction(
              normalized, {
                errors: property('messages.errors')(original) || [],
                warnings: property('messages.warnings')(original) || [],
                infos: property('messages.infos')(original) || []
              },
              original
            )
          )
        }
      })
}
