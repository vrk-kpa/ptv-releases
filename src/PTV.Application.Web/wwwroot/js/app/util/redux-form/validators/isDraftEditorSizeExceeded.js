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

import { defineMessages } from 'util/react-intl'
import { isValid } from './isValid'

const messages = defineMessages({
  maxExceeded: {
    id: 'Components.Validators.MaxCharacterExceeded.Message',
    defaultMessage: 'You have exceeded your characters please shorten the text. ({count} char(s))'
  }
})

const isDraftEditorSizeExceeded = isValid(
  ({ value, language, fieldProps: { limit = 2500 } }) => {
    let val = value && value.getCurrentContent && value.getCurrentContent().getPlainText &&
    value.getCurrentContent().getPlainText('')

    const regex = /(?:\r\n|\r|\n){2,}/g
    val = (typeof val === 'string') && val.replace(regex, '\n') || ''

    return (val.length > limit)
      ? { count: val.length - limit }
      : undefined
  }, {
    validationMessage: messages.maxExceeded,
    messageOptions: (error, language) =>
      error.languages && error.languages.length
        ? error.result[language] || error.result[error.languages[0]]
        : error.result
  }
)

export default isDraftEditorSizeExceeded
