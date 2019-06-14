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
import NotifyLabel from 'appComponents/NotifyLabel'
import { isPastDate } from 'util/helpers'
import moment from 'moment'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import styles from './styles.scss'
import { getIntlLocale } from 'Intl/Selectors'
import { connect } from 'react-redux'
import { fieldPropTypes, change } from 'redux-form/immutable'
import { daysOfTheWeek } from 'enums'
import Tooltip from 'appComponents/Tooltip'

const RenderDatePicker = ({
  input,
  mode = 'dateTime',
  label,
  labelRequired,
  fieldClass,
  inputClass,
  labelClass,
  type,
  locale,
  compareDate,
  pastDateLabel,
  setDay,
  formName,
  path,
  change,
  defaultValue,
  ignoreDefaultValue,
  includeTooltip,
  tooltipProps = {},
  ...rest
}) => {
  const onChange = (newDate) => {
    if (moment.isMoment(newDate)) {
      input.onChange(newDate.valueOf())
    }
    if (newDate && setDay) {
      // isoWeekday: Mo(1) - Su(7), daysOfTheWeek: Mo(0) - Su(6)
      const day = moment(newDate).isoWeekday() - 1
      const field = type === 'from' && 'dayFrom' || 'dayTo'
      change(formName, `${path}.${field}`, daysOfTheWeek[day])
    }
    // not store default value
    if (ignoreDefaultValue && defaultValue && defaultValue.isSame(newDate, 'day')) {
      input.onChange(null)
    }
  }

  const onBlur = date => {
    let isNewDateValid = false
    if (type === 'from') {
      if (!compareDate || compareDate && date.valueOf() < compareDate) {
        isNewDateValid = true
      }
    } else {
      if (!compareDate || compareDate && date.valueOf() > compareDate) {
        isNewDateValid = true
      }
    }
    if (!isNewDateValid || !moment.isMoment(date)) {
      input.onChange(null)
    }
  }
  const toDate = type === 'to' && (
    input.value && moment(input.value).isValid() && moment(input.value).utc() ||
    compareDate && moment(compareDate).isValid() && moment(compareDate).utc() || moment.utc()
  )
  const viewDate = toDate || moment.utc()
  return (
    <div className={fieldClass}>
      {label &&
        <div className={labelClass}>
          <NotifyLabel
            shouldShowNotificationIcon={isPastDate(input.value)}
            notificationText={pastDateLabel}
            labelText={label}
            required={labelRequired}
            className={styles.labelWithIcon}
            popupProps={{ maxWidth: 'mW160' }}
          />
        </div>
      }
      <div className={inputClass}>
        <DateTime
          onChange={onChange}
          onBlur={onBlur}
          value={input.value || defaultValue}
          timeFormat={mode === 'time' || mode === 'dateTime' ? 'HH:mm' : false}
          dateFormat={mode === 'date' || mode === 'datTime' ? 'DD.MM.YYYY' : false}
          utc
          locale={locale}
          strictParsing
          viewDate={viewDate}
          {...rest}
        />
        {includeTooltip && <Tooltip {...tooltipProps} />}
      </div>
    </div>
  )
}
RenderDatePicker.propTypes = {
  input: fieldPropTypes.input,
  mode: PropTypes.oneOf(['time', 'date', 'dateTime']),
  label: PropTypes.string,
  labelRequired: PropTypes.bool,
  pastDateLabel: PropTypes.string,
  fieldClass: PropTypes.string,
  inputClass: PropTypes.string,
  labelClass: PropTypes.string,
  type: PropTypes.string,
  locale: PropTypes.string,
  compareDate: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  setDay: PropTypes.bool,
  formName: PropTypes.string,
  path: PropTypes.string,
  change: PropTypes.func,
  defaultValue: PropTypes.any,
  ignoreDefaultValue: PropTypes.any,
  includeTooltip: PropTypes.bool,
  tooltipProps: PropTypes.object
}

export default compose(
  withFormFieldAnchor,
  injectFormName,
  withPath,
  connect((state, { formName, path }) => ({
    locale: getIntlLocale(state),
    formName,
    path
  }), {
    change
  })
)(RenderDatePicker)
