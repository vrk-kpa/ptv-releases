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
import { Button } from 'sema-ui-components'
import ArrowDown from 'appComponents/ArrowDown'
import ArrowUp from 'appComponents/ArrowUp'
import cx from 'classnames'
import styles from './styles.scss'

const ToggleButton = ({
  showToggle,
  buttonProps = { small: true, secondary: true },
  onClick,
  disabled,
  isCollapsed,
  toggleIconCollapsed,
  toggleIconExpanded,
  children,
  className
}) => {
  const toggleButtonClass = cx(
    styles.toggleButton,
    {
      [styles.disabled]: disabled,
      [styles.primary]: buttonProps.secondary === false
    },
    className
  )
  return showToggle && (
    <Button
      onClick={onClick}
      disabled={disabled}
      {...buttonProps}
      className={toggleButtonClass}
    >
      {children}
      {isCollapsed
        ? toggleIconCollapsed || (
          <span className={styles.toggleIcon}><ArrowDown width={18} height={18} onClick={onClick} /></span>
        )
        : toggleIconExpanded || (
          <span className={styles.toggleIcon}><ArrowUp width={18} height={18} onClick={onClick} /></span>
        )
      }
    </Button>
  ) || null
}

ToggleButton.propTypes = {
  buttonProps: PropTypes.object,
  showToggle: PropTypes.bool,
  disabled: PropTypes.bool,
  onClick: PropTypes.func.isRequired,
  children: PropTypes.node,
  isCollapsed: PropTypes.bool,
  toggleIconCollapsed: PropTypes.node,
  toggleIconExpanded: PropTypes.node,
  className: PropTypes.string
}

export default ToggleButton
