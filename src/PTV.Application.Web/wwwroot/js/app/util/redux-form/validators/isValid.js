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
import { Map, Iterable } from 'immutable'

const getValidationResult = (validation, language, languageErrors) => {
  return validation
    ? languageErrors.set(language, validation)
    : languageErrors
}

const getParentPath = name => {
  const regex = /\[(\d+)\]/gi
  var path = name && name.replace(regex, (match, p1) => `.${p1}`).split('.')
  // check if parent section is registred (if list item is missing, validaion for required field is skipped)
  // issue related to removing whole item from list, but validation is done before fields are unregistered
  if (path && path.length > 1) {
    return path.slice(0, path.length - 1)
  }
  return null
}

const localizedValueValidation = ({
  value,
  allValues,
  validationFunc,
  formProps,
  fieldProps,
  name
}) => {
  const la = allValues.get('languagesAvailabilities') || (
    fieldProps.getCustomValidationLanguages &&
    formProps.dispatch(store => fieldProps.getCustomValidationLanguages(store.getState(), fieldProps))
  )
  const laCodes = la.map((v, k) => v.get('code'))
  const errors = laCodes
    .reduce(
      (languageErrors, language) =>
        getValidationResult(
          validationFunc({ value: value.get(language), allValues, language, formProps, fieldProps, name }),
          language,
          languageErrors
        )
      , Map()
    )
  if (errors.size) {
    return {
      languages: errors.map((_, language) => language).toArray(),
      result: errors.toJS()
    }
  }
  return undefined
}

const localizedSimpleValueValidation = ({
  value,
  allValues,
  validationFunc,
  formProps,
  fieldProps,
  name
}) => {
  const errors = value.reduce(
    (langaugeErrors, value, language) =>
      getValidationResult(
        validationFunc({ value, allValues, language, formProps, fieldProps, name }),
        language,
        langaugeErrors
      )
    , Map()
  )
  if (errors.size > 0) {
    return {
      languages: errors.map((_, language) => language).toArray(),
      result: errors.toJS()
    }
  }
  return undefined
}

export const createError = ({
  label,
  validationMessage,
  languages,
  messageOptions,
  errorProps,
  fieldProps,
  result
}) => ({
  errors: [{
    ...errorProps,
    // id: typeof validationMessage === 'object' && validationMessage.id || validationMessage,
    message: typeof validationMessage === 'function' ? validationMessage(fieldProps) : validationMessage,
    label: label,
    languages,
    options: messageOptions,
    result
  }]
})

export const isValidWithForAllLanguages = (
  validationFunc,
  { validationMessage, messageOptions, errorProps, checkParentPath }
) =>
  ({ label, fieldProps } = {}) =>
    (value, allValues, formProps, name) => {
      const parentPath = checkParentPath ? getParentPath(name) : null
      if (fieldProps ? fieldProps.isLocalized : Iterable.isIterable(value)) {
        const error = localizedValueValidation({
          value,
          allValues,
          validationFunc,
          formProps,
          fieldProps,
          name,
          parentPath
        })
        return error
          ? createError({
            label: label || name,
            validationMessage,
            messageOptions,
            errorProps,
            fieldProps,
            ...error
          })
          : null
      }

      const result = validationFunc({ value, allValues, formProps, fieldProps, name, parentPath })
      return result
        ? createError({
          label: label || name,
          validationMessage,
          messageOptions,
          errorProps,
          fieldProps,
          result
        })
        : undefined
    }

export const isValid = (validationFunc, { validationMessage, messageOptions, errorProps, checkParentPath }) =>
  ({ label, fieldProps } = {}) =>
    (value, allValues, formProps, name) => {
      // if (Iterable.isIterable(value)) {
      const parentPath = checkParentPath ? getParentPath(name) : null
      if (value != null && (fieldProps ? fieldProps.isLocalized : Iterable.isIterable(value))) {
        const error = localizedSimpleValueValidation({
          value,
          allValues,
          validationFunc,
          formProps,
          fieldProps,
          name,
          parentPath
        })
        return error
          ? createError({
            label: label || name,
            validationMessage,
            messageOptions,
            errorProps,
            fieldProps,
            ...error
          })
          : null
      }

      const result = validationFunc({ value, allValues, formProps, fieldProps, name, parentPath })
      return result
        ? createError({
          label: label || name,
          validationMessage,
          messageOptions,
          errorProps,
          fieldProps,
          result
        })
        : undefined
    }
