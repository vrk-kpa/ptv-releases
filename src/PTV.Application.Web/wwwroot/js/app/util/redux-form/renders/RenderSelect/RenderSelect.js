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
import { Select } from 'sema-ui-components'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import Tooltip from 'appComponents/Tooltip'
import { isUndefined } from 'lodash'
import injectDefaultTexts from './injectDefaultTexts'
import { fieldPropTypes } from 'redux-form/immutable'

export const RenderSelect = ({
  input,
  label,
  tooltip,
  options,
  error,
  errorMessage,
  inlineLabel,
  resetValue,
  onChangeObject,
  defaultValue,
  onBlurResetsInput,
  ...rest
}) => {
  const LabelChildren = () => tooltip && <Tooltip tooltip={tooltip} /> || null
  const onBlur = () => input.onBlur(input.value)

  return (
    <div>
      <Select
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && <LabelChildren />}
        {...input}
        value={input.value || typeof input.value === 'number' ? input.value : defaultValue}
        onChange={(option) => {
          onChangeObject && onChangeObject(option)
          if (option === null && !isUndefined(resetValue)) {
            input.onChange(resetValue)
          } else {
            input.onChange(option ? option.value : null)
          }
        }}
        options={options}
        size='full'
        clearable
        onBlur={onBlur}
        {...rest}
        onBlurResetsInput={onBlurResetsInput} // onBlurResetsInput doesn't work react-select #751
        error={error}
        errorMessage={errorMessage}
      />
    </div>
  )
}

RenderSelect.propTypes = {
  input: fieldPropTypes.input,
  error: PropTypes.bool,
  errorMessage: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  options: PropTypes.array.isRequired,
  inlineLabel: PropTypes.bool,
  isDirty: PropTypes.bool,
  resetValue: PropTypes.any,
  onChangeObject: PropTypes.func,
  defaultValue: PropTypes.any,
  onBlurResetsInput: PropTypes.bool
}

RenderSelect.defaultProps = {
  labelPosition: 'top',
  defaultValue: null
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor,
  injectDefaultTexts
)(RenderSelect)
