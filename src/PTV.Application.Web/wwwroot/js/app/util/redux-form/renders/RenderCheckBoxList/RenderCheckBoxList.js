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
import { Checkbox, Label } from 'sema-ui-components'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import { compose } from 'redux'
import { withFormFieldAnchor, withFormStates } from 'util/redux-form/HOC'
import { Set } from 'immutable'

const RenderCheckBoxList = ({
  input,
  options = [],
  label,
  tooltip,
  isCompareMode,
  required,
  localizedOnChange,
  disabled,
  ...rest
}) => {
  const value = (input && input.value) || Set()
  const onChange = localizedOnChange && localizedOnChange(input) || input.onChange
  return (
    <div>
      {label &&
        <Label
          labelText={label}
          labelPosition='top'
          required={required}
        >
          {tooltip &&
            <Popup
              trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
              content={tooltip}
            />
          }
        </Label>
      }

      {options.map(({ label, id, disabledItem, isSelected }) =>
        <Checkbox
          checked={value.has(id) || isSelected}
          label={label}
          onChange={(checked) => {
            const newValue = (value || Set())
            onChange(checked ? newValue.add(id) : newValue.delete(id))
          }}
          disabled={disabledItem || disabled}
        />)}
    </div>
  )
}

RenderCheckBoxList.propTypes = {
  input: PropTypes.object.isRequired,
  options: PropTypes.array.isRequired,
  className: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  isCompareMode: PropTypes.bool,
  required: PropTypes.bool,
  disabled: PropTypes.bool,
  localizedOnChange: PropTypes.func
}

export default compose(
  withFormFieldAnchor,
  withFormStates
)(RenderCheckBoxList)
