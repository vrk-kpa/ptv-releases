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
import merge from 'lodash/merge'
import 'isomorphic-fetch'
import { getAppUrl } from '../Configuration/AppHelpers'

function buildApiRootUrl () {
  var url = ''

  if (typeof (getApiUrl) === 'function') {
    url = getApiUrl()
  }

  return url !== '' ? url : (getAppUrl() + '/api/')
}

const API_ROOT = buildApiRootUrl()
export const API_WEB_ROOT = getAppUrl() + '/api/'

console.log('URL: ' + API_ROOT)
console.log('URL: ' + API_WEB_ROOT)

const customAssign = (languageCode) => (output, key, value, input, schema) => {
  const language = languageCode || 'fi'

  if (schema && schema.getMeta('localizable')) {
    if (!output[language]) {
      output[language] = {}
    }
    output[language][key] = value
  } else {
    output[key] = value
  }
}

const reduceNormalizedDataArray = (prev, curr) => {
  return {
    entities: merge(prev.entities, curr.entities),
    results: merge(prev.results, curr.results),
    enums: merge(prev.enums, curr.enums)
  }
}

export const normalizeData = (object, schema, langCode) => {
  const normalized = normalize(object, schema, { assignEntity: customAssign(langCode) })
  return normalized
}

const normalizeApiData = (object, schema, resultPropertyName, language) => {
  const key = getSchemaKey(schema)
  const property = object[key]
  if (property) {
    const normalized = normalizeData(property, schema, language)
    object[key] = normalized.result
    normalized.results = { [key]: normalized.result }
    if (resultPropertyName) {
      normalized[resultPropertyName] = normalized.results
    }
    return normalized
  }

  return object
}

const normalizeSchemaArray = (schemas, object, resultPropertyName, language) => {
  const normalized = schemas.map((s) => normalizeApiData(object, s, resultPropertyName, language))
  return normalized
}

const getSchemaKey = (schema) => {
  return schema._itemSchema && schema._itemSchema.getKey ? schema._itemSchema.getKey() : schema.getKey()
}

// Fetches an API response and normalizes the result JSON according to schema.
// This makes every API response have the same shape, regardless of how nested it was.
function callApi (endpoint, schemas, typeSchemas, options, clientId) {
  // checkif endpoint already contains base asi url or base web url
  const fullUrl = (endpoint.indexOf(API_ROOT) !== -1) || (endpoint.indexOf(API_WEB_ROOT) !== -1)
    ? endpoint
    : API_ROOT + endpoint

  return fetch(fullUrl, options)
    .then(response =>
      response.json().then(json => ({ json, response }))
    ).then(({ json, response }) => {
      if (!response.ok) {
        return Promise.reject(json)
      }

      const camelizedJson = camelizeKeys(json)
      if (json.translatedData) {
        camelizedJson.data = { ...camelizedJson.data, localizationMessages: json.translatedData }
      }

      let result = {}
      let normalizedEnums = []
      // normalize static types
      if (camelizedJson.data && camelizedJson.data.enumCollection && typeSchemas) {
        if (Array.isArray(typeSchemas)) {
          normalizedEnums = normalizeSchemaArray(typeSchemas, camelizedJson.data.enumCollection, 'enums', json.language)
        } else {
          const normalized = normalizeData(camelizedJson.data.enumCollection, typeSchemas, json.language)
          const enums = normalized.entities.enums
          normalizedEnums = [{ entities: normalized.entities, ...enums }]
        }
        delete camelizedJson.data['enumCollection']
      }

      let resultModel = camelizedJson.data
      // normalize data by schema
      if (schemas && camelizedJson.data) {
        // normalize schema arrays
        if (Array.isArray(schemas)) {
          let normalized = normalizeSchemaArray(schemas, camelizedJson.data, null, json.language)
          result = [...normalized, ...normalizedEnums].reduce(reduceNormalizedDataArray)
          resultModel.language = json.language
        } else { // normalize schema for one model
          if (camelizedJson.data.id == null) {
            camelizedJson.data.id = clientId
          }

          let normalized = normalizeData(camelizedJson.data, schemas, json.language)

          result = [normalized, ...normalizedEnums].reduce(reduceNormalizedDataArray)

          !result.results
            ? result.results = { [getSchemaKey(schemas)]: normalized.result }
            : result.results[getSchemaKey(schemas)] = normalized.result
          // data are already merged to entities by schema
          resultModel = { result: normalized.result }
        }
      } else if (normalizedEnums.length > 0) {
        // reduce normalized enum collections
        result = [...normalizedEnums].reduce(reduceNormalizedDataArray)
      }

      result.model = resultModel
      if (camelizedJson.messages) {
        result = { ...result, messages: camelizedJson.messages }
      }

      return result
    })
}

function getOptions (data) {
  var token = getAccessToken() || null

     //   localStorage.getItem('access_token') || null;

  if (!token) {
    console.log('getOptions, bearer token is not set!')
  }

  let options = {
    headers: {
      'Authorization': 'Bearer ' + token,
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin' : '*'
    }
  }

  if (data) {
    options.method = 'post'
    options.body = JSON.stringify(data)
  }
  return options
}

export function callApiDirect (endpoint, data) {
  const options = getOptions(data)
  return callApi(endpoint, null, null, options)
}

const getError = (error) => {
  if (error instanceof TypeError) {
    return {
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

// Action key that carries API call info interpreted by this Redux middleware.
export const CALL_API = Symbol('Call API')

// A Redux middleware that interprets actions with CALL_API info specified.
// Performs the call and promises when such actions are dispatched.
export default store => next => action => {
  const callAPI = action[CALL_API]
  if (typeof callAPI === 'undefined') {
    return next(action)
  }

  let { payload, payload: { data }, payload: { endpoint } } = callAPI
  const { schemas, types, typeSchemas, clientId, successNextAction } = callAPI

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

  let options = getOptions(data)

  function actionWith (data) {
    const finalAction = Object.assign({}, action, data)
    delete finalAction[CALL_API]
    return finalAction
  }

  const [ requestType, successType, failureType ] = types
  next(actionWith({ type: requestType, request: payload }))

  return callApi(endpoint, schemas, typeSchemas, options, clientId)
  .then(
    response => next(actionWith({
      response,
      type: successType
    })),
    error => next(actionWith({
      type: failureType,
      errorObject: getError(error),
      error: error.message || 'Something bad happened'
    }))
  ).then(
    response => {
      if (successNextAction && typeof successNextAction === 'function') {
        const customAction = successNextAction(response)
        if (customAction) {
          next(customAction)
        }
      }
    })
}
