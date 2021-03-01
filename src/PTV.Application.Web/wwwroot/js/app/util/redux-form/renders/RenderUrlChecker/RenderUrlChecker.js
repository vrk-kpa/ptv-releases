/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import { TextField, Spinner, Label } from 'sema-ui-components'
import cx from 'classnames'
import styles from './styles.scss'
import Tooltip from 'appComponents/Tooltip'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { fieldPropTypes } from 'redux-form/immutable'
import 'isomorphic-fetch'

const RenderUrlChecker = ({
  isChecking,
  isBroken,
  updateUI,
  input,
  tooltip,
  placeholder,
  label,
  inlineLabel,
  getCustomValue,
  customOnChange,
  isCompareMode,
  splitView,
  compare,
  required,
  error,
  disabled,
  errorMessage,
  warning,
  customOnBlur,
  ...rest
}) => {
  const getValue = () => getCustomValue ? getCustomValue(input.value, '', compare) : input.value
  const urlCheckerClass = cx(
    styles.urlChecker,
    'col-lg-24',
    {
      [styles.isChecking]: isChecking
    }
  )
  return (
    <div>
      {label && <Label labelText={label} labelPosition='top' required={required}>
        {tooltip && <Tooltip tooltip={tooltip} />}
      </Label>}
      <div className='row'>
        <div className={urlCheckerClass}>
          <TextField
            {...input}
            value={getValue()}
            onChange={customOnChange && customOnChange(input, compare) || input.onChange}
            onBlur={customOnBlur && customOnBlur(getValue()) || input.onBlur}
            children={isChecking && <Spinner className={styles.spinner} />}
            placeholder={placeholder}
            label={inlineLabel && label}
            counter
            size='full'
            maxLength={500}
            error={error}
            disabled={disabled}
            errorMessage={errorMessage}
            warning={warning}
          />
        </div>
      </div>
    </div>
  )
}

RenderUrlChecker.propTypes = {
  input: fieldPropTypes.input,
  tooltip: PropTypes.string,
  placeholder: PropTypes.string,
  label: PropTypes.string,
  inlineLabel: PropTypes.string,
  updateUI: PropTypes.func,
  getCustomValue: PropTypes.func,
  customOnChange: PropTypes.func,
  customOnBlur: PropTypes.func,
  isCompareMode: PropTypes.bool,
  required: PropTypes.bool,
  error: PropTypes.bool,
  compare: PropTypes.bool,
  isBroken: PropTypes.bool,
  splitView: PropTypes.bool,
  isChecking: PropTypes.bool,
  disabled: PropTypes.bool,
  errorMessage: PropTypes.string,
  warning: PropTypes.node
}

export default compose(
  // withState({
  //   initialState: {
  //     isChecking: false,
  //     isBroken: null
  //   }
  // }),
  withErrorMessage,
  withFormFieldAnchor,
  withFormStates
)(RenderUrlChecker)
