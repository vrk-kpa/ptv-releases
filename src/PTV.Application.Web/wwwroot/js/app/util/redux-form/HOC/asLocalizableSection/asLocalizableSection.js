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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { FormSection } from 'redux-form/immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { withFormStates } from 'util/redux-form/HOC'
import cx from 'classnames'
import styles from './styles.scss'

const asLocalizableSection = name => WrappedComponent => {
  const InnerComponent = ({
    isLocalized,
    language,
    comparisionLanguageCode,
    isCompareMode,
    isReadOnly,
    ...rest
  }) => {
    const compareWrapClass = cx(
      styles.compareWrap,
      {
        [styles.isCompareMode]: isCompareMode,
        [styles.isReadOnly]: isReadOnly
      }
    )
    const sideOneClass = cx(
      styles.compareSide,
      styles.sideOne
    )
    const sideTwoClass = cx(
      styles.compareSide,
      styles.sideTwo
    )
    return isLocalized && (
      <FormSection name={name}>
        { isCompareMode
        ? <div className={compareWrapClass}>
          <div className={sideOneClass}>
            <WrappedComponent key={language} name={language} {...rest} isCompareMode={false} splitView />
          </div>
          <div className={sideTwoClass}>
            <WrappedComponent key={comparisionLanguageCode} name={comparisionLanguageCode} {...rest} isCompareMode={false} splitView compare />
          </div>
        </div>
        : <WrappedComponent key={language} name={language} {...rest} />
        }
      </FormSection>
    ) || <WrappedComponent {...rest} />
  }
  InnerComponent.propTypes = {
    language: PropTypes.string.isRequired,
    isLocalized: PropTypes.bool
  }
  InnerComponent.defaultProps = {
    language: 'fi',
    isLocalized: true
  }
  return compose(
    withFormStates,
    connect(
      (state, ownProps) => ({
        language: getContentLanguageCode(state, ownProps) || 'fi',
        comparisionLanguageCode: getSelectedComparisionLanguageCode(state) || 'fi'
      })
    )
  )(InnerComponent)
}

export default asLocalizableSection
