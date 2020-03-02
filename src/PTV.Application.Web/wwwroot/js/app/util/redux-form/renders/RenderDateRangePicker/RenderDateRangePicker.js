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
import moment from 'moment'
import { compose } from 'redux'
import cx from 'classnames'
import styles from './styles.scss'
import RenderDatePicker from '../RenderDatePicker'

const RenderDateRangePicker = ({
  dateFrom,
  dateTo,
  fromLabel,
  toLabel,
  mode = 'date',
  fromChildren,
  toChildren,
  conjunction,
  layout,
  splitView,
  ...rest
}) => {
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
    styles[layout],
    {
      [styles.splitView]: splitView
    }
  )
  const fromFieldClass = cx(styles.field, styles.from)
  const toFieldClass = cx(styles.field, styles.to)
  return (
    <div className={dateRangeClass}>
      <RenderDatePicker
        fieldClass={fromFieldClass}
        label={fromLabel}
        labelClass={styles.label}
        inputClass={styles.input}
        input={dateFrom.input}
        mode={mode}
        isValidDate={validateDateFrom}
        compareDate={dateTo.input.value}
        type='from'
        {...rest}
      />
      {conjunction && <div className={styles.conjunction}>-</div>}
      <RenderDatePicker
        fieldClass={toFieldClass}
        label={toLabel}
        labelClass={styles.label}
        inputClass={styles.input}
        input={dateTo.input}
        mode={mode}
        isValidDate={validateDateTo}
        compareDate={dateFrom.input.value}
        type='to'
        {...rest}
      />
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
  layout: PropTypes.oneOf(['block', 'column', 'inline']),
  splitView: PropTypes.bool
}
RenderDateRangePicker.defaultProps = {
  conjunction: false,
  layout: 'inline'
}

export default compose()(RenderDateRangePicker)
