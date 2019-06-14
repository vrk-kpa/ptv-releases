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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import ListBox from 'appComponents/ListBox'
import TooltipLabel from 'appComponents/TooltipLabel'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import { fieldPropTypes } from 'redux-form/immutable'

export const RenderListBox = ({
  input,
  options,
  onChangeObject,
  defaultValue,
  label,
  tooltip,
  ...rest
}) => {
  const labelId = label ? `${rest.reduxKey}_label` : undefined
  return (
    <Fragment>
      {(label || tooltip) && (
        <TooltipLabel
          labelProps={{ labelText: label, id: labelId }}
          tooltipProps={{ tooltip: tooltip }}
        />
      )}
      <ListBox
        {...input}
        value={input.value || typeof input.value === 'number' ? input.value : defaultValue}
        onChange={value => {
          onChangeObject && onChangeObject(value)
          input.onChange(value || null)
        }}
        options={options}
        aria-labelledby={labelId}
        {...rest}
      />
    </Fragment>
  )
}

RenderListBox.propTypes = {
  input: fieldPropTypes.input,
  options: PropTypes.array.isRequired,
  onChangeObject: PropTypes.func,
  defaultValue: PropTypes.any,
  label: PropTypes.string,
  tooltip: PropTypes.string
}

RenderListBox.defaultProps = {
  defaultValue: null
}

export default compose(
  withFormFieldAnchor
)(RenderListBox)
