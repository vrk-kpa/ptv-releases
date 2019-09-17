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
import Tooltip from 'appComponents/Tooltip'
import { compose } from 'redux'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import { Set } from 'immutable'
import { fieldPropTypes } from 'redux-form/immutable'
import styles from './styles.scss'

const RenderCheckBoxList = ({
  input,
  options = [],
  label,
  tooltip,
  isCompareMode,
  required,
  customOnChange,
  disabled,
  additionalText,
  additionalTooltip,
  ...rest
}) => {
  const value = (input && input.value) || Set()
  const onChange = customOnChange && customOnChange(input) || input.onChange
  return (
    <div>
      {label &&
        <Label
          labelText={label}
          labelPosition='top'
          required={required}
        >
          {tooltip && <Tooltip tooltip={tooltip} />}
        </Label>
      }

      {options.map(({ label, id, disabledItem, isSelected, isAdditionalText }) =>
        <div className={styles.targetLanguage}>
          <Checkbox
            checked={value.has(id) || isSelected}
            label={label}
            onChange={(checked) => {
              const newValue = (value || Set())
              onChange(checked ? newValue.add(id) : newValue.delete(id))
            }}
            disabled={disabledItem || disabled}
          />
          {isAdditionalText &&
            <Label
              labelText={additionalText}
              labelPosition='top'
              infoLabel
              className={styles.reorderLabel}
            >
              {additionalTooltip && <Tooltip tooltip={additionalTooltip} />}
            </Label>
          }
        </div>
      )}
    </div>
  )
}

RenderCheckBoxList.propTypes = {
  input: fieldPropTypes.input,
  options: PropTypes.array.isRequired,
  className: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  additionalText: PropTypes.string,
  additionalTooltip: PropTypes.string,
  isCompareMode: PropTypes.bool,
  required: PropTypes.bool,
  disabled: PropTypes.bool,
  customOnChange: PropTypes.func
}

export default compose(
  withFormFieldAnchor,
  withFormStates
)(RenderCheckBoxList)
