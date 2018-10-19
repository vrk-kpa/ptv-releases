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
import withFormStates from 'util/redux-form/HOC/withFormStates'
import Popup from 'appComponents/Popup'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import cx from 'classnames'

export const Tooltip = ({
  indent,
  inverse,
  tooltip,
  position,
  children,
  className,
  withinText,
  isReadOnly,
  hideOnScroll,
  showInReadOnly
}) => {
  const tooltipClass = cx(
    styles.tooltip,
    styles[indent],
    {
      [styles.inverse]: inverse,
      [styles.withinText]: withinText
    },
    className
  )
  if (isReadOnly && !showInReadOnly) return null
  return (
    <span className={tooltipClass}>
      <Popup
        trigger={
          <PTVIcon
            name='icon-tip2'
            width={30}
            height={30}
          />
        }
        content={tooltip}
        position={position}
        hideOnScroll={hideOnScroll}
      >
        {children}
      </Popup>
    </span>
  )
}

Tooltip.propTypes = {
  indent: PropTypes.string,
  inverse: PropTypes.bool,
  className: PropTypes.string,
  tooltip: PropTypes.string,
  position: PropTypes.string,
  children: PropTypes.any,
  withinText: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  hideOnScroll: PropTypes.bool,
  showInReadOnly: PropTypes.bool
}

Tooltip.defaultProps = {
  indent: 'i10'
}

export default compose(
  withFormStates
)(Tooltip)
