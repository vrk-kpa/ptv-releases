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
import { RenderDatePicker, RenderDatePickerDisplay } from 'util/redux-form/renders'
import { Field, formValueSelector } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { compose } from 'redux'
import moment from 'moment'
import { asDisableable, asComparable, injectFormName } from 'util/redux-form/HOC'
import styles from './styles.scss'
import { injectIntl } from 'react-intl'
import withPath from 'util/redux-form/HOC/withPath'

const DatePicker = ({
  validate,
  type,
  filterDate,
  ...rest
}) => {
  const validateDate = () => {
    if (!type || !filterDate) return () => { return true }
    if (type === 'from' && !isNaN(Number(filterDate)) && Math.abs(Number(filterDate)) > 0) {
      return validBefore
    } else if (type === 'to' && !isNaN(Number(filterDate)) && Math.abs(Number(filterDate)) > 0) {
      return validAfter
    }
  }

  const validAfter = (current) => {
    const treshold = moment(filterDate)
    return current.isAfter(treshold)
  }

  const validBefore = (current) => {
    const treshold = moment(filterDate)
    return current.isBefore(treshold)
  }

  const validDate = validateDate()
  return (
    <div className={styles.datePicker}>
      <Field
        name='date'
        component={RenderDatePicker}
        mode='date'
        isValidDate={validDate}
        {...rest}
      />
    </div>
  )
}
DatePicker.propTypes = {
  validate: PropTypes.func,
  type: PropTypes.string,
  filterDate: PropTypes.number
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderDatePickerDisplay }),
  asDisableable,
  injectFormName,
  withPath,
  connect(
    (state, { formName, type, path }) => {
      const getFormValues = formValueSelector(formName)
      const dateType = type === 'from' ? 'dateTo' : 'dateFrom'
      return {
        filterDate: getFormValues(state, `${path}.${dateType}`)
      }
    }
  )
)(DatePicker)
