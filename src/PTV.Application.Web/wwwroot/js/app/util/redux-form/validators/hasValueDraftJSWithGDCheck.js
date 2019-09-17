/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import { isValid } from './isValid'
import messages from './messages'
import { getGDLocalizedValue } from './selectors'
import { entityTypesEnum } from 'enums'
import { EditorState, convertFromRaw } from 'draft-js'

const hasValueDraftJSWithGDCheck = field => isValid(
  ({ value, allValues, language, formProps }) => {
    const hasText = value &&
      typeof value.getCurrentContent === 'function' &&
      value.getCurrentContent().hasText()
    if (!hasText) {
      let gdValue
      formProps.dispatch(({ getState }) => {
        const state = getState()
        gdValue = getGDLocalizedValue(state, {
          id: allValues.get('generalDescriptionId'),
          type: entityTypesEnum.GENERALDESCRIPTIONS,
          language,
          field
        })
      })
      const editorState = gdValue && EditorState.createWithContent(convertFromRaw(JSON.parse(gdValue))) ||
        EditorState.createEmpty()
      const isLocalizedValueInGD = editorState.getCurrentContent().hasText()
      return !isLocalizedValueInGD
    }
  }, {
    validationMessage: messages.draftJSValueWithGDCheck
})

export default hasValueDraftJSWithGDCheck
