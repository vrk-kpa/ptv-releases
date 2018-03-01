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
import { fromJS, Map } from 'immutable'
import { getValidationMessages } from './selectors'
import { isRequired } from 'util/redux-form/validators'

export const validateForPublish = (values, { dispatch }) => {
  let validation = null
  dispatch(({ getState }) => {
    validation = getValidationMessages(getState())
  })
  console.log('validation values', values, values && values.toJS())
  console.log('validation server', validation, validation && validation.toJS())

  const val = validation.reduce(
    (prev, curr) => {
      prev[curr] = Map({ label: curr, message: 'required', path: curr })
      return prev
    },
    {}
  )
  console.log('errors', val, val && fromJS(val).toJS())
  return val
  // return {
  //   shortDescription : fromJS({ label: 'kokot', message: 'pica', path: 'shortDescription'})
  // }
}
