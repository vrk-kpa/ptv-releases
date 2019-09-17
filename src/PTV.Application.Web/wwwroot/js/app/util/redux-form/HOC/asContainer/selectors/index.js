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
import { createSelector } from 'reselect'
import { getUiByKey, getFormValueWithPath, getFormSyncErrorWithPath } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { Iterable, fromJS } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'

export const getIsCollapsed = createSelector(
  getUiByKey,
  component => component.get('isCollapsed') !== false
)

const getDataPath = (dataPaths) => ({ path }) => {
  return path && `${path}.${dataPaths}` || dataPaths
}

const getPath = ({ path }) => {
  return path
}

const createGetDataForDataPaths = (dataPaths) => createSelector(
  getFormValueWithPath(getPath),
  values => {
    return values
      .filter((v, k) => {
        return (Iterable.isIterable(v) ? v.size : v) && dataPaths.includes(k)
      })
  }
)

const getDataSelector = dataPaths => {
  if (dataPaths) {
    return Array.isArray(dataPaths)
      ? createGetDataForDataPaths(dataPaths)
      : getFormValueWithPath(getDataPath(dataPaths))
  }
  return getFormValueWithPath(getPath, false)
}

export const createGetHasData = (dataPaths) => {
  const selector = getDataSelector(dataPaths)
  return createSelector(
    selector,
    data => {
      return !!(Iterable.isIterable(data) ? data.size > 0 : data)
    }
  )
}

const createGetErrorForDataPaths = (dataPaths) => createSelector(
  getFormSyncErrorWithPath(getPath),
  errors => {
    // getFormSyncErrors returns object
    const errorsMap = fromJS(errors)
    return errorsMap
      .filter((v, k) => {
        return (Iterable.isIterable(v) ? v.size : v) && dataPaths.includes(k)
      })
  }
)

const getErrorSelector = dataPaths => {
  if (dataPaths) {
    return Array.isArray(dataPaths)
      ? createGetErrorForDataPaths(dataPaths)
      : getFormSyncErrorWithPath(getDataPath(dataPaths))
  }
  return getFormSyncErrorWithPath(getPath, false)
}

export const createGetHasError = (dataPaths) => {
  const selector = getErrorSelector(dataPaths)
  return createSelector(
    selector,
    errors => {
      return !!(Iterable.isIterable(errors) ? errors.size > 0 : errors)
    }
  )
}

export const createGetData = dataPaths => {
  const selector = getDataSelector(dataPaths)
  return createSelector(
    selector,
    data => data
  )
}

export const getDialCodeById = createSelector(
  EntitySelectors.dialCodes.getEntity,
  dialCode => dialCode.get('code') || ''
)

export const getMunicipalityCodeById = createSelector(
  EntitySelectors.municipalities.getEntity,
  municipality => municipality.get('code') || ''
)

const getIsInCompareMode = (_, { compare }) => !!compare

export const getLanguageCode = createSelector(
  [
    getContentLanguageCode,
    getSelectedComparisionLanguageCode,
    getIsInCompareMode
  ],
  (language, comparisionLanguageCode, compare) => {
    return compare && comparisionLanguageCode || language || 'fi'
  }
)
