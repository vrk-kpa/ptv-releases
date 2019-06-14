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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  getIsCompareMode,
  getIsReadOnly,
  getIsFormDisabled
} from 'selectors/formStates'
import { isUndefined } from 'lodash'
import { getContentLanguageCode } from 'selectors/selections'
import { getIsInTranslation } from 'selectors/entities/entities'

const withFormStates = WrappedComponent => compose(
  injectFormName,
  connect((state, { formName, isReadOnly, isCompareMode, isDisabled }) => {
    const languageCode = getContentLanguageCode(state, { formName }) || 'fi'
    const inTranslation = getIsInTranslation(state, { languageCode })
    return {
      isReadOnly: inTranslation
        ? true
        : isUndefined(isReadOnly)
          ? getIsReadOnly(state, { formName })
          : isReadOnly,
      isCompareMode: isUndefined(isCompareMode)
        ? getIsCompareMode(formName)(state)
        : isCompareMode,
      isDisabled: isUndefined(isDisabled)
        ? getIsFormDisabled(formName)(state)
        : isDisabled,
      inTranslation
    }
  })
)(WrappedComponent)

export const formStatesPropTypes = {
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  isDisabled: PropTypes.bool.isRequired
}

export default withFormStates
