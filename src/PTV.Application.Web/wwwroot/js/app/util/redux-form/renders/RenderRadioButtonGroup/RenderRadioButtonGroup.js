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
import { RadioGroup, RadioButton, Label } from 'sema-ui-components'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import { compose } from 'redux'
import { withFormFieldAnchor, withFormStates } from 'util/redux-form/HOC'
import cx from 'classnames'
import styles from './styles.scss'

const RenderRadioButtonGroup = ({
  input,
  options = [],
  label,
  tooltip,
  inline,
  className,
  isCompareMode,
  defaultValue,
  required,
  disabled,
  small,
  ...rest
}) => {
  const radioGroupClass = cx(
    styles.radioGroup,
    className,
    {
      [styles.compareMode]: isCompareMode
    }
  )
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
      <RadioGroup
        value={input.value == null || input.value === '' ? defaultValue : input.value}
        onChange={input.onChange}
        inline={inline}
        className={radioGroupClass}
      // {...rest}
      >
        {options.map(({ label, value, disabledItem }, index) =>
          <RadioButton
            key={index}
            label={label}
            value={value}
            disabled={disabledItem || disabled}
            small={small}
          // {...rest}
          />)}
      </RadioGroup>
    </div>
  )
}
RenderRadioButtonGroup.propTypes = {
  input: PropTypes.object.isRequired,
  defaultValue: PropTypes.any,
  options: PropTypes.array.isRequired,
  className: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  isCompareMode: PropTypes.bool,
  required: PropTypes.bool,
  disabled: PropTypes.bool,
  inline: PropTypes.bool,
  small: PropTypes.bool
}

export default compose(
  withFormFieldAnchor,
  withFormStates
)(RenderRadioButtonGroup)
