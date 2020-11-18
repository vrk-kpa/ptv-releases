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
import ArrowDown from 'appComponents/ArrowDown'
import ArrowUp from 'appComponents/ArrowUp'
import Tooltip from 'appComponents/Tooltip'
import { Button } from 'sema-ui-components'
import cx from 'classnames'
import styles from './styles.scss'

const ToggleButton = ({
  className,
  children,
  disabled,
  onClick,
  showIcon,
  showTooltip,
  isCollapsed,
  tooltip,
  size
}) => {
  const toggleButtonClass = cx(
    styles.button,
    className
  )
  const isTooltipShown = tooltip && (showTooltip || !isCollapsed)
  return (
    <div className={toggleButtonClass}>
      <Button
        type='button'
        onClick={onClick}
        link
        disabled={disabled}
      >
        {children}
      </Button>
      <div className={styles.buttonIcon}>
        {showIcon && (
          <span>
            {isCollapsed
              ? <ArrowDown width={size} height={size} onClick={onClick} />
              : <ArrowUp width={size} height={size} onClick={onClick} />
            }
          </span>
        )}
        {isTooltipShown && <Tooltip tooltip={tooltip} className={styles.tooltipIcon} />}
      </div>
    </div>
  )
}

ToggleButton.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  disabled: PropTypes.bool,
  onClick: PropTypes.func,
  showIcon: PropTypes.bool,
  showTooltip: PropTypes.bool,
  isCollapsed: PropTypes.bool,
  tooltip: PropTypes.string,
  size: PropTypes.number
}

export default ToggleButton
