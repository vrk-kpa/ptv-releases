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
import { Select } from 'sema-ui-components'
import { compose } from 'redux'
import { withErrorMessage, withFormFieldAnchor } from 'util/redux-form/HOC'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import { isUndefined } from 'lodash'

export const RenderSelect = ({
  input,
  label,
  tooltip,
  options,
  error,
  errorMessage,
  inlineLabel,
  resetValue,
  ...rest
}) => {
  const LabelChildren = () => (
    tooltip &&
    <Popup
      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
      content={tooltip}
    /> || null
  )

  return (
    <div>
      <Select
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && <LabelChildren />}
        {...input}
        value={input.value || typeof input.value === 'number' ? input.value : null}
        onChange={(option) => {
          if (option === null && !isUndefined(resetValue)) {
            input.onChange(resetValue)
          } else {
            input.onChange(option ? option.value : null)
          }
        }}
        options={options}
        size='full'
        clearable
        {...rest}
        onBlur={() => {}} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput
        error={error}
        errorMessage={errorMessage}
      />
    </div>
  )
}

RenderSelect.propTypes = {
  input: PropTypes.object.isRequired,
  error: PropTypes.bool,
  errorMessage: PropTypes.string,
  label: PropTypes.string.isRequired,
  tooltip: PropTypes.string,
  options: PropTypes.array.isRequired,
  inlineLabel: PropTypes.bool,
  isDirty: PropTypes.bool,
  resetValue: PropTypes.any
}

RenderSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor
)(RenderSelect)
