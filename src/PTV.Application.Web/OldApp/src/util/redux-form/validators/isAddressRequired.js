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
import messages from './messages'
import { isValid } from './isValid'

const isNullOrEmpty = ({ value, allValues, parentPath }) => {
  // check if parent section is registred (if list item is missing, validaion for required field is skipped)
  // issue related to removing whole item from list, but validation is done before fields are unregistered
  if (parentPath && !allValues.hasIn(parentPath)) {
    return null
  }
  return (value == null || (value.every(text => text.trim().length === 0)))
}

const isAddressRequired = customErrorMessage => isValid(isNullOrEmpty, {
  validationMessage: customErrorMessage || messages.isRequired,
  checkParentPath: true
})

export default isAddressRequired
