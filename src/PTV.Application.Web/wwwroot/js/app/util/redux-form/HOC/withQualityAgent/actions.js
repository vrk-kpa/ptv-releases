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
import { apiCall3 } from 'actions'
import { QualityAgentSchemas } from 'schemas/qualityAgent'
import { change, formValueSelector, getFormValues } from 'redux-form/immutable'
import generateUuid from 'uuid/v4'
import { debounce } from 'debounce'

const getProperty = (property, props, qaProviderOptions) => {
  if (typeof property === 'function') {
    return property(props, qaProviderOptions)
  }
  return property
}

const getValueForCheck = (property, getValue, props, qaProviderOptions, options) => {
  const propertyName = getProperty(property, props, qaProviderOptions)
  if (propertyName) {
    return {
      [propertyName]: [{
        ...options,
        language: props.language,
        value: getValue()
      }]
    }
  }
  return {}
}

export const setAlternativeId = (store, formName) => {
  const state = store.getState()
  const values = getFormValues(formName)(state)
  if (values && !values.get('unificRootId') && !values.get('alternativeId')) {
    store.dispatch(change(formName, 'alternativeId', generateUuid()))
  }
}

const qualityCheckCall = ({
  value,
  getValue,
  language,
  property,
  valueSelector,
  options,
  props,
  providerOptions,
  customAction
}, store) => {
  const dataSelector = typeof providerOptions.dataSelector === 'function'
    ? providerOptions.dataSelector
    : () => ({})
  setAlternativeId(store, props.formName)
  typeof customAction === 'function' && store.dispatch(customAction(props))
  const customValues = dataSelector(store.getState(), { formName: props.formName })
  const checkedValue = getValueForCheck(property, () => getValue(value, language), props, providerOptions, options)
  // console.log('input: ', getValue(value, language), checkedValue)
  const data = {
    Input: {
      ...customValues,
      ...checkedValue,
      // [getProperty(property, providerOptions)]: [{
      //   ...options,
      //   language,
      //   value
      // }],
      status: 'Draft'
    },
    Language: props.language || props.contentLanguage,
    Profile: providerOptions.profile || 'VRKp'
  }
  // console.log('data', data)
  store.dispatch(apiCall3({
    keys: ['services', 'quality'],
    payload: { endpoint: 'qualityAgent/Check', data },
    schemas: QualityAgentSchemas.getQualityAgentSchema(customValues.id || customValues.alternativeId)
    // schemas: QualityAgentSchemas.QUALITY_AGENT
  }))
}

const check = debounce(qualityCheckCall, 2000)

export const qualityCheck = data => store => {
  check(data, store)
}
