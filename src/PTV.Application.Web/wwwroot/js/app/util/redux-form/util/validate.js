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
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'
import { isUndefined, set, mergeWith, isArray, isObject, forEach } from 'lodash'
import { validationMessageTypes } from '../validators/types'
import { Map, Iterable, OrderedMap } from 'immutable'
import { merger } from 'Middleware'

const errorMerger = (objValue, srcValue, key) => {
  // console.log('merger', objValue, srcValue, key)
  if (key === 'errors') {
    // return merger(objValue, srcValue, key)
    if (isArray(objValue)) {
      if (isArray(srcValue)) {
        return objValue.concat(srcValue)
      }
      objValue.push(srcValue)
      return objValue
    }
    return merger(objValue, srcValue, key)
  }

  if (objValue && objValue.mergeWith) {
    return objValue.mergeWith(errorMerger, srcValue)
  }
  if (isObject(objValue)) {
    return mergeWith(objValue, Iterable.isIterable(srcValue) ? srcValue.toJS() : srcValue, errorMerger)
  }
  return srcValue
}

const createValidator = (validators) => {
  return function validator (formValues, props) {
    const serverErrors = validateForPublish(formValues, props)
    const syncErrors = composeValidators(validators)(formValues, props)
    const errors = syncErrors.mergeWith(errorMerger, serverErrors)
    return errors.toJS()
  }
}
const composeValidators = (validators) => {
  return function composedValidator (formValues, props) {
    return validators.reduce((errors, validator) => {
      const error = getErrors(validator, formValues, props)
      if (error) {
        return errors.mergeWith(errorMerger, error)
      }
      return errors
    }, OrderedMap())
  }
}
const getErrors = (validator, formValues, props) => {
  const isArrayValidation = ~validator.path.indexOf('[]')
  return isArrayValidation
    ? arrayValidation(validator, formValues, props)
    : simpleValidation(validator, formValues, props)
}
const simpleValidation = (validator, formValues, props) => {
  const valueAtPath = formValues.getIn(stringPathToArrayPath(validator.path))
  let error = validator.validate(valueAtPath, formValues, props)
  error = setValidationType(error, validator)
  if (error) {
    return Map().set(validator.path, error)
  }
}

const arrayValidation = (validator, formValues, props) => {
  const dotAtStartOrEnd = /(^\.)|(\.$)/g
  const emptySquareBrackets = /\[\]/g
  /*
    Get array of paths where array is found
    `openingHours[].normalOpeningHours[].dailyOpeningHours.monday[].intervals`
    to:
    [
      openingHours,
      normalOpeningHours,
      dailyOpeningHours.monday,
      intervals
    ]
  */
  const arrayPaths = validator.path
    .split('[]')
    .map(split => split.replace(dotAtStartOrEnd, ''))
    .filter(x => x)
  // [] to [*] which latter is replaced with correct index //
  var fieldPath = validator.path.replace(emptySquareBrackets, '[*]')
  return getErrorsForArrayPath(
    arrayPaths,
    formValues,
    props,
    validator,
    fieldPath,
    formValues
  )
}

const getErrorsForArrayPath = (
  arrayPaths,
  previousValueAtPath,
  props,
  validator,
  fieldPath,
  formValues
) => {
  // Item is null when first added to collection //
  if (isUndefined(previousValueAtPath)) return Map()
  const path = arrayPaths[0]
  const remainingArrayPaths = arrayPaths.slice(1)
  const currentValueAtPath = previousValueAtPath.getIn(stringPathToArrayPath(path))
  const isEndOfPath = remainingArrayPaths.length === 0
  if (!isEndOfPath) {
    const result = currentValueAtPath
      .reduce((errors, itemValue, reduceIndex) => {
        /*
          Updates value between brackets to index
          openingDays[*].monday -> openingDays[0].monday
        */
        fieldPath = replaceAt(
          fieldPath,
          reduceIndex,
          fieldPath.indexOf(path) + path.length + 1
        )
        // Recurse //
        let error = getErrorsForArrayPath(
          remainingArrayPaths,
          currentValueAtPath.get(reduceIndex),
          props,
          validator,
          fieldPath,
          formValues
        )
        error = setValidationType(error, validator)
        // console.log(error)
        errors = error
          ? errors.set(fieldPath, error)
          : errors.delete(fieldPath)
        return errors
      }, Map())
    return result
  } else {
    let error = validator.validate(currentValueAtPath, formValues, props, fieldPath)
    // console.log(error)
    error = setValidationType(error, validator)
    if (arrayPaths.length === 1) {
      return error && error.size
        ? error
        : undefined
    }
    return error
  }
}
// Util fns //
const replaceAt = (str, replacement, index) => {
  return str.substr(0, index) + replacement + str.substr(index + 1)
}

const stringPathToArrayPath = (path) => {
  return ~path.indexOf('.')
    ? path.split('.')
    : [path]
}

const setValidationType = (error, validator) => {
  if (error) {
    if (Iterable.isIterable(error)) {
      return error.update(
        'errors',
        e => e && e.map(x => x && x.set('type', validator.type || validationMessageTypes.hidden))
      )
    }
    forEach(error.errors, x => {
      if (x) {
        x.type = validator.type || validationMessageTypes.hidden
      }
    })
  }
  return error
}

export default createValidator
