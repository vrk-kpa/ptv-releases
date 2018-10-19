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
import { Label } from 'sema-ui-components'
import NoDataLabel from 'appComponents/NoDataLabel'
import styles from './styles.scss'
import cx from 'classnames'
import { fieldPropTypes } from 'redux-form/immutable'

const RenderTextFieldDisplay = ({
  input,
  label = '',
  getCustomValue,
  compare,
  preview,
  required,
  useBSClasses,
  fieldClass,
  labelClass,
  inputClass,
  ...rest
}) => {
  const fieldCSS = cx(
    useBSClasses && fieldClass
  )
  const labelCSS = cx(
    useBSClasses && labelClass
  )
  const inputCSS = cx(
    styles.textFieldDisplay,
    useBSClasses && inputClass
  )
  const value = getCustomValue
    ? getCustomValue(input.value, '', compare)
    : input.value || ''
  if (preview) return value ? <div className={styles.textFieldDisplay}>{value}</div> : null
  return <div className={fieldCSS}>
    <Label className={labelCSS} labelText={label} />
    <div className={inputCSS}>{value || <NoDataLabel required={required} />}</div>
  </div>
}
RenderTextFieldDisplay.propTypes = {
  input: fieldPropTypes.input,
  preview: PropTypes.bool,
  label: PropTypes.string,
  useBSClasses: PropTypes.bool,
  fieldClass: PropTypes.string,
  labelClass: PropTypes.string,
  inputClass: PropTypes.string,
  getCustomValue: PropTypes.func,
  compare: PropTypes.bool,
  required: PropTypes.bool
}

export default RenderTextFieldDisplay
