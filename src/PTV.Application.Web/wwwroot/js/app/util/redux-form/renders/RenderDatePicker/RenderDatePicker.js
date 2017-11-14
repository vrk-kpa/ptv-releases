/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
import DateTime from 'react-datetime'
import { Label } from 'sema-ui-components'
import moment from 'moment'
import { compose } from 'redux'
import { withFormFieldAnchor } from 'util/redux-form/HOC'

const RenderDatePicker = ({
  input,
  mode = 'dateTime',
  label,
  fieldClass,
  inputClass,
  labelClass,
  ...rest
}) => {
  const onChange = (newDate) => {
    if (moment.isMoment(newDate)) {
      // if (this.props.mode === DateTimeModes.timeOnly) {
      //   newDate = newDate.valueOf() - moment().startOf('day').valueOf()
      //   input.onChange(newDate)
      // }
      input.onChange(newDate.valueOf())
    } else {
      input.onChange(newDate)
    }
  }

  const onBlur = date => {
    if (!moment.isMoment(date)) {
      input.onChange(null)
    }
  }

  return (
    <div className={fieldClass}>
      {label &&
        <div className={labelClass}>
          <Label labelText={label} />
        </div>
      }
      <div className={inputClass}>
        <DateTime
          onChange={onChange}
          onBlur={onBlur}
          value={input.value}
          timeFormat={mode === 'time' || mode === 'dateTime' ? 'HH:mm' : false}
          dateFormat={mode === 'date' || mode === 'datTime' ? 'DD.MM.YYYY' : false}
          strictParsing
          {...rest}
        />
      </div>
    </div>
  )
}
RenderDatePicker.propTypes = {
  input: PropTypes.object.isRequired,
  mode: PropTypes.oneOf(['time', 'date', 'dateTime']),
  label: PropTypes.string,
  fieldClass: PropTypes.string,
  inputClass: PropTypes.string,
  labelClass: PropTypes.string
}

export default compose(
  withFormFieldAnchor
)(RenderDatePicker)
