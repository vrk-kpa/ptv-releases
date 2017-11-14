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
import cx from 'classnames'
import styles from './styles.scss'

const RenderDateRangePicker = ({ dateFrom, dateTo, fromLabel, toLabel, mode = 'date', fromChildren, toChildren, conjunction, layout, ...rest }) => {

  const handleDateFromChange = newDate => {
    handleDateChange(newDate, dateFrom)
  }

  const handleDateToChange = newDate => {
    handleDateChange(newDate, dateTo)
  }

  const handleDateChange = (newDate, field) => {
    if (moment.isMoment(newDate)) {
      field.input.onChange(newDate.valueOf())
    } else {
      field.input.onChange(newDate)
    }
  }

  const handleDateFromBlur = date => {
    handleDateBlur(date, dateFrom)
  }

  const handleDateToBlur = date => {
    handleDateBlur(date, dateTo)
  }

  const handleDateBlur = (date, field) => {
    if (!moment.isMoment(date)) {
      field.input.onChange(null)
    }
  }

  const validateDate = (type, filterValue) => {
    if (!type || !filterValue) return () => { return true }
    if (type === 'from' && !isNaN(Number(filterValue)) && Math.abs(Number(filterValue)) > 0) {
      return validBefore
    } else if (type === 'to' && !isNaN(Number(filterValue)) && Math.abs(Number(filterValue)) > 0) {
      return validAfter
    }
  }

  const validAfter = (current) => {
    const treshold = moment(dateFrom.input.value)
    return current.isAfter(treshold)
  }

  const validBefore = (current) => {
    const treshold = moment(dateTo.input.value)
    return current.isBefore(treshold)
  }

  const validateDateFrom = validateDate('from', dateTo.input.value)
  const validateDateTo = validateDate('to', dateFrom.input.value)

  const dateRangeClass = cx(
    styles.dateRange,
    styles[layout]
  )
  const fromFieldClass = cx(styles.field, styles.from)
  const toFieldClass = cx(styles.field, styles.to)
  return (
    <div className={dateRangeClass}>
      <div className={fromFieldClass}>
        {fromLabel &&
          <div className={styles.label}>
            <Label labelText={fromLabel} />
          </div>
        }
        <div id='dateFrom' className={styles.input}>
          <DateTime
            onChange={handleDateFromChange}
            onBlur={handleDateFromBlur}
            value={dateFrom.input.value}
            timeFormat={mode === 'time' || mode === 'dateTime' ? 'HH:mm' : false}
            dateFormat={mode === 'date' || mode === 'dateTime' ? 'DD.MM.YYYY' : false}
            strictParsing
            isValidDate={validateDateFrom}
            {...rest}
          />
        </div>
        {fromChildren}
      </div>
      {conjunction && <div className={styles.conjunction}>-</div>}
      <div className={toFieldClass}>
        {toLabel &&
          <div className={styles.label}>
            <Label labelText={toLabel} />
          </div>
        }
        <div id='dateTo' className={styles.input}>
          <DateTime
            onChange={handleDateToChange}
            onBlur={handleDateToBlur}
            value={dateTo.input.value}
            timeFormat={mode === 'time' || mode === 'dateTime' ? 'HH:mm' : false}
            dateFormat={mode === 'date' || mode === 'dateTime' ? 'DD.MM.YYYY' : false}
            strictParsing
            isValidDate={validateDateTo}
            {...rest}
          />
        </div>
        {toChildren}
      </div>
    </div>
  )
}
RenderDateRangePicker.propTypes = {
  dateFrom: PropTypes.object,
  dateTo: PropTypes.object,
  fromLabel: PropTypes.string,
  toLabel: PropTypes.string,
  mode: PropTypes.oneOf(['time', 'date', 'dateTime']),
  label: PropTypes.string,
  fromChildren: PropTypes.node,
  toChildren: PropTypes.node,
  conjunction: PropTypes.bool,
  layout: PropTypes.oneOf(['block', 'column', 'inline'])
}
RenderDateRangePicker.defaultProps = {
  conjunction: false,
  layout: 'inline'
}

export default compose()(RenderDateRangePicker)
