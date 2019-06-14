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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getAccessibilityRegisterSentenceGroups,
  getARContentLanguageCode,
  getARSelectedComparisionLanguageCode
} from './selectors'
import { Label } from 'sema-ui-components'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'

const Info = ({
  accessibilityInfo,
  languageCode
}) => (
  <div>
    {accessibilityInfo.map((info, index) => (
      <div key={index}>
        <Label position='top'>
          {info.getIn(['sentenceGroups', languageCode])}
        </Label>
        {info.get('sentences').map((sentence, index) => (
          <div
            key={index}
            className={styles.item}
          >
            {sentence.getIn(['sentences', languageCode])}
          </div>
        ))}
      </div>
    ))}
  </div>
)
Info.propTypes = {
  accessibilityInfo: PropTypes.object,
  languageCode: PropTypes.string
}

export default compose(
  withLanguageKey,
  injectFormName,
  connect((state, { formName, compare, languageKey, index }) => {
    const language = getARContentLanguageCode(state, { formName, languageKey })
    const comparisionLanguageCode = getARSelectedComparisionLanguageCode(state)
    return {
      accessibilityInfo: getAccessibilityRegisterSentenceGroups(state, { index }),
      languageCode: compare && comparisionLanguageCode || language
    }
  })
)(Info)
