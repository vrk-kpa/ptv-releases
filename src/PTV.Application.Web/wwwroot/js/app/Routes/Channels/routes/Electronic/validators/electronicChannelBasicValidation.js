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
import { Map } from 'immutable'
import { phoneNumbersValidation } from 'util/redux-form/validators'

const electronicChannelBasicValidation = values => {
  let electronicChannelBasicErrors = Map()
  // if (values.getIn(['summary'])) {
  //   electronicChannelBasicErrors = electronicChannelBasicErrors.mergeDeep(
  //     values.getIn(['summary']).reduce((acc, summary, languageKey) => {
  //       return !summary
  //         ? acc.setIn(['summary', languageKey], {
  //           label: 'Summary',
  //           message: 'Summary is required',
  //           path: ['summary', languageKey].join('.'),
  //           language: languageKey
  //         })
  //         : acc
  //     }, Map())
  //   )
  // } else {
  //   electronicChannelBasicErrors = electronicChannelBasicErrors.setIn(['summary', 'fi'], {
  //     label: 'Summary',
  //     message: 'Summary is required',
  //     path: ['summary', 'fi'].join('.')
  //   })
  // }
  // if (values.getIn(['phoneNumbers'])) {
  //   electronicChannelBasicErrors = electronicChannelBasicErrors.mergeDeep(
  //     values.getIn(['phoneNumbers']).reduce((acc, phoneNumbers, languageKey) => {
  //       return acc.mergeDeep(
  //         phoneNumbers.reduce((acc, phoneNumber, index) => {
  //           return acc.mergeDeep(
  //             phoneNumbersValidation(['phoneNumbers', languageKey, index])(values)
  //           )
  //         }, Map())
  //       )
  //     }, Map())
  //   )
  // }
  return electronicChannelBasicErrors
}

export default electronicChannelBasicValidation
