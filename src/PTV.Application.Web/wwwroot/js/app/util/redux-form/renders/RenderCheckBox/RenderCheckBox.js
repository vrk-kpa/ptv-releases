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
import { Checkbox } from 'sema-ui-components'
import RenderError from '../RenderError'
import { fieldPropTypes } from 'redux-form/immutable'

const RenderCheckBox = ({
  input,
  getCustomValue,
  customOnChange,
  compare,
  isCheckedDefault = false,
  disabled,
  centered,
  label,
  className,
  draft,
  alpha,
  beta,
  archived,
  small,
  lakka,
  kanerva,
  havumetsa,
  jarvi,
  taivas,
  success,
  warning,
  ...rest
}) => {
  const value = getCustomValue
    ? getCustomValue(input.value, isCheckedDefault, compare)
    : (input.value != null && input.value !== '' ? input.value : isCheckedDefault)
  return (
    <RenderError input={input} {...rest} >
      <Checkbox
        checked={!!value}
        onChange={customOnChange && customOnChange(input, compare) || input.onChange}
        disabled={disabled}
        centered={centered}
        label={label}
        className={className}
        draft={draft}
        alpha={alpha}
        beta={beta}
        archived={archived}
        small={small}
        lakka={lakka}
        kanerva={kanerva}
        havumetsa={havumetsa}
        jarvi={jarvi}
        taivas={taivas}
        success={success}
        warning={warning}
      />
    </RenderError>
  )
}

RenderCheckBox.propTypes = {
  input: fieldPropTypes.input,
  isCheckedDefault: PropTypes.bool,
  getCustomValue: PropTypes.func,
  centered: PropTypes.bool,
  customOnChange: PropTypes.func,
  compare: PropTypes.bool,
  disabled: PropTypes.bool,
  label: PropTypes.string,
  className: PropTypes.string,
  draft: PropTypes.bool,
  alpha: PropTypes.bool,
  beta: PropTypes.bool,
  archived: PropTypes.bool,
  small: PropTypes.bool,
  lakka: PropTypes.bool,
  kanerva: PropTypes.bool,
  havumetsa: PropTypes.bool,
  jarvi: PropTypes.bool,
  taivas: PropTypes.bool,
  success: PropTypes.bool,
  warning: PropTypes.bool
}

export default RenderCheckBox
