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
import { TextField } from 'sema-ui-components'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import Tooltip from 'appComponents/Tooltip'
import styles from './styles.scss'
import { fieldPropTypes } from 'redux-form/immutable'

const RenderTextField = ({
  input,
  tooltip,
  getCustomValue,
  customOnChange,
  customOnBlur,
  label,
  compare,
  counter,
  disabled,
  defaultValue,
  error,
  errorMessage,
  labelPosition,
  maxLength,
  multiline,
  placeholder,
  required,
  rows,
  size,
  type,
  fieldClass,
  labelClass,
  inputClass,
  warning,
  autocomplete,
  prefix,
  className,
  compactLabel,
  valueLink,
  resizeable,
  focused,
  counterClass,
  labelTop,
  elementClass,
  children,
  ...rest
}) => {
  const labelChildren = () => tooltip && <Tooltip tooltip={tooltip} />

  const value = getCustomValue
    ? getCustomValue(input.value, defaultValue || '', compare)
    : input.value
  return (
    <div className={rest.counter && styles.withCounter}>
      <TextField
        label={label}
        labelChildren={labelChildren()}
        {...input}
        onChange={customOnChange && customOnChange(input, compare) || input.onChange}
        onBlur={customOnBlur && customOnBlur(input, compare) || input.onBlur}
        value={value}
        counter={counter}
        disabled={disabled}
        error={error}
        errorMessage={errorMessage}
        warning={warning}
        labelPosition={labelPosition}
        maxLength={maxLength}
        multiline={multiline}
        placeholder={placeholder}
        required={required}
        rows={rows}
        size={size}
        type={type}
        fieldClass={fieldClass}
        labelClass={labelClass}
        inputClass={inputClass}
        autoComplete={autocomplete}
        className={className}
        prefix={prefix}
        compactLabel={compactLabel}
        valueLink={valueLink}
        resizeable={resizeable}
        focused={focused}
        counterClass={counterClass}
        labelTop={labelTop}
        elementClass={elementClass}
        children={children}
      />
    </div>
  )
}
RenderTextField.propTypes = {
  input: fieldPropTypes.input,
  label: PropTypes.string,
  getCustomValue: PropTypes.func,
  customOnChange: PropTypes.func,
  customOnBlur: PropTypes.func,
  defaultValue: PropTypes.any,
  size: PropTypes.string,
  tooltip: PropTypes.string,
  compare: PropTypes.bool,
  counter: PropTypes.bool,
  disabled: PropTypes.bool,
  error: PropTypes.bool,
  errorMessage: PropTypes.node,
  labelPosition: PropTypes.string,
  maxLength: PropTypes.number,
  multiline: PropTypes.bool,
  placeholder: PropTypes.string,
  required: PropTypes.bool,
  rows: PropTypes.number,
  type: PropTypes.string,
  fieldClass: PropTypes.string,
  labelClass: PropTypes.string,
  inputClass: PropTypes.string,
  autocomplete: PropTypes.string.isRequired,
  prefix: PropTypes.oneOfType([PropTypes.string, PropTypes.node]),
  warning: PropTypes.node,
  className: PropTypes.string,
  compactLabel: PropTypes.bool,
  valueLink: PropTypes.object,
  resizeable: PropTypes.bool,
  focused: PropTypes.bool,
  counterClass: PropTypes.string,
  labelTop: PropTypes.any,
  elementClass: PropTypes.string,
  children: PropTypes.any
}

RenderTextField.defaultProps = {
  size: 'full',
  labelPosition: 'top',
  autocomplete: ''
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor
)(RenderTextField)
