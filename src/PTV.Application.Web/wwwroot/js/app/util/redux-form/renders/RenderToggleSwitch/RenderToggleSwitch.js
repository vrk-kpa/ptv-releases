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
import { fieldPropTypes } from 'redux-form/immutable'
import cx from 'classnames'
import styles from './styles.scss'
import { Label } from 'sema-ui-components'
import Tooltip from 'appComponents/Tooltip'

const ToggleSwitch = props => {
  const {
    round,
    label,
    labelClass,
    wrapperClass,
    switchClass,
    showTooltip,
    tooltip,
    tooltipClass,
    input
  } = props

  const sliderClass = cx(styles.slider, round ? styles.round : null, switchClass)

  return (
    <div className={cx(styles.wrapper, wrapperClass)}>
      <Label className={labelClass} labelText={label} />
      {showTooltip && <Tooltip className={cx(styles.tooltip, tooltipClass)} tooltip={tooltip} />}
      <label className={styles.switch}>
        <input type='checkbox' checked={input.value} onChange={input.onChange} />
        <span className={sliderClass} />
      </label>
    </div>
  )
}

ToggleSwitch.propTypes = {
  round: PropTypes.bool,
  showTooltip: PropTypes.bool,
  label: PropTypes.string,
  labelClass: PropTypes.string,
  wrapperClass: PropTypes.string,
  switchClass: PropTypes.string,
  tooltip: PropTypes.string,
  tooltipClass: PropTypes.string,
  input: fieldPropTypes.input
}

export default ToggleSwitch
