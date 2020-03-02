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
import { createSelector } from 'reselect'
import { getParameterFromProps, getFormValueWithPath } from 'selectors/base'
import { getFormInitialValues } from 'redux-form/immutable'
import { Map, OrderedSet } from 'immutable'

const getPath = ({ path }) => path

const getCurrentFormValue = createSelector(
  getFormValueWithPath(getPath),
  value => value || Map()
)

const getInitialFormValues = createSelector(
  [state => state, getParameterFromProps('formName')],
  (state, formName) => getFormInitialValues(formName)(state)
)

const getStoredFormValue = createSelector(
  [getInitialFormValues, getParameterFromProps('path')],
  (values, path) => values && values.get(path) || Map()
)

const sortItems = items => {
  if (OrderedSet.isOrderedSet(items)) return items.sort()
  return items.map(item => {
    if (OrderedSet.isOrderedSet(item)) return item.sort()
    return item
  })
}

export const getIsEqual = createSelector(
  [getCurrentFormValue, getStoredFormValue],
  (current, stored) => {
    const sortedCurrent = sortItems(current)
    const sortedStored = sortItems(stored)
    return sortedCurrent.equals(sortedStored)
  }
)
