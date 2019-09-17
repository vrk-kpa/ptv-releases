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
import { List, fromJS, Iterable } from 'immutable'
import CommonMessages from 'util/redux-form/messages'

const flat = (x, key = [], doNotPropagateKey = false) => {
  // console.log('flat', x, x && x.toJS && x.toJS(), x && x.toObject && x.toObject())
  if (!x) {
    return null
  }
  if (x.has('message')) {
    // console.log(CommonMessages, key, key.map(k => CommonMessages[k]))
    return List([x.set('path', key).set('pathLabel', key.map(k => CommonMessages[k] || k))])
  }
  const isErrorInField = x.has('errors')
  // console.log(x.toJS(), isErrorInField, doNotPropagateKey, key)
  if (typeof x.map === 'function') {
    const r = x
      .map((a, k) => flat(
        a,
        isErrorInField || doNotPropagateKey ? key : [...key, k],
        doNotPropagateKey || isErrorInField
      ))
    return r.reduce((all, c) => c ? all.concat(c) : all, List())
  }
  return null
}

const getErrors = obj => {
  // console.log('getErrors', obj, obj && obj.toObject && obj.toObject())
  const x = flat(!Iterable.isIterable(obj) && fromJS(obj) || obj)
  // console.log(x.toJS())
  return x
}

export const createGetFlatFormValidationErrors = validationSelector => createSelector(
  validationSelector,
  errors => getErrors(errors) || null
)
