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
import { isValid } from './isValid'
import { getTotalCount } from './selectors'
import { OrderedSet } from 'immutable'
import { entityTypesEnum } from 'enums'
import messages from './messages'

const hasMaxItemCountWithGD = (limit, field, message, reverseValidation) => isValid(
  ({ value, allValues, language, formProps }) => {
    const items = allValues.get(field) || OrderedSet()
    let totalCount
    formProps.dispatch(({ getState }) => {
      const state = getState()
      totalCount = getTotalCount(state, {
        id: allValues.get('generalDescriptionId'),
        type: entityTypesEnum.GENERALDESCRIPTIONS,
        field,
        defaultValue: OrderedSet(),
        items
      })
    })
    const validateCondition = (totalCount >= limit)
    return reverseValidation ? !validateCondition : validateCondition
  }, {
    validationMessage: message || messages.itemMaxCountReached,
    messageOptions: {
      limit
    },
    errorProps: {
      limit
    }
  }
)

export default hasMaxItemCountWithGD
