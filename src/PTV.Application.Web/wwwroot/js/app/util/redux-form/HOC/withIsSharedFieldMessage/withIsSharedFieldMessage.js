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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import { getIsEqual } from './selectors'
import styles from './styles.scss'
import cx from 'classnames'

const withIsSharedFieldMessage = ({
  warningText,
  withinGroup,
  customPath
}) => WrappedComponent => {
  const InnerComponent = ({
    shouldShowWarning,
    warningFromProps,
    warningComponent,
    className,
    ...rest
  }) => {
    const warningWrapClass = cx(
      styles.warningWrap,
      {
        [styles.withinGroup]: withinGroup
      },
      className
    )
    const formatText = text => text && typeof text === 'object' && text.id && rest.intl.formatMessage(text) || text
    const warning = warningFromProps || warningText
    return (
      <div className={warningWrapClass}>
        {shouldShowWarning && warning &&
          <div className={styles.warning}>
            {warningComponent || formatText(warning)}
          </div>
        }
        <WrappedComponent {...rest} />
      </div>
    )
  }
  InnerComponent.propTypes = {
    shouldShowWarning: PropTypes.bool,
    warningText: PropTypes.string,
    warningComponent: PropTypes.node,
    className: PropTypes.string,
    warningFromProps: PropTypes.object
  }
  return compose(
    injectFormName,
    withPath,
    connect((state, { formName, path, warningText, disableWarning }) => {
      const isEqual = getIsEqual(state, { formName, path: path || customPath })
      return {
        shouldShowWarning: !isEqual && !disableWarning,
        warningFromProps: warningText
      }
    })
  )(InnerComponent)
}

export default withIsSharedFieldMessage

