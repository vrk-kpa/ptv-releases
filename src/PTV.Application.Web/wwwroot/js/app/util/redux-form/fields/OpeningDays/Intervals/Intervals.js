/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { PropTypes } from 'prop-types'
import moment from 'moment'
import { FieldArray, fieldArrayPropTypes } from 'redux-form/immutable'
import TimeSelect from 'util/redux-form/fields/TimeSelect'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import _ from 'lodash'

const timeOptions = _.range(1, 96).map(index => {
  const minute = 60 * 1000
  const time = moment.utc(index * minute * 15)
  return {
    label: time.format('HH:mm'),
    value: time.valueOf()
  }
})
const beginningTimeOptions = [
  { label: '00:00', value: 0 },
  ...timeOptions
]
const endTimeOptions = [
  ...timeOptions,
  { label: '24:00', value: 0 }
]

const messages = defineMessages({
  hoursConjunction: {
    id: 'Util.ReduxForm.DailyOpeningHour.HoursConjunction',
    defaultMessage: 'ja'
  }
})

const getIndexForValue = value => beginningTimeOptions.findIndex(x => x.value === value)

const RenderIntervals = ({
  fields,
  formatMessage,
  name,
  isReadOnly,
  intervals,
  isCompareMode,
  maxIntervals,
  ...props
}) => {
  const lastInterval = intervals.length > 1
    ? _.last(_.dropRight(intervals)) // last but one
    : _.first(intervals)
  const maximumToValue = _.get(lastInterval, 'to.input.value')
  const maximumToIndex = _.findIndex(beginningTimeOptions, ({ value }) => value === maximumToValue)
  const intervalOptions = _.drop(beginningTimeOptions, maximumToIndex + 1)
  const allFiled = intervals.every(interval => interval.from.input.value && interval.to.input.value)
  const canAddNewInterval = allFiled && !isReadOnly && fields.length > 0 && fields.length < maxIntervals
  const canRemoveInterval = !isReadOnly && fields.length > 1
  return (
    <div className={styles.intervalWrap}>
      <div className={styles.intervalItems}>
        {fields.map((member, index) => {
          const shouldFilter = fields.length !== 1 && fields.length - 1 === index
          const fromValue = _.get(intervals[index], 'from.input.value')
          const toValue = _.get(intervals[index], 'to.input.value')
          const fromOptions = shouldFilter
            ? toValue
              ? intervalOptions.filter(x => x.value < toValue)
              : intervalOptions
            : toValue
              ? beginningTimeOptions.filter(x => x.value < toValue)
              : undefined
          const toOptions = shouldFilter
            ? fromValue
              ? [...intervalOptions.filter(x => x.value > fromValue), { label: '24:00', value: 0 }]
              : intervalOptions
            : fromValue
              ? [...beginningTimeOptions.filter(x => x.value > fromValue), { label: '24:00', value: 0 }]
              : endTimeOptions
          const fromResetValueDefault = 28800000
          const toValueIndex = getIndexForValue(toValue)
          const fromResetValue = toValue
            ? Math.min(beginningTimeOptions[toValueIndex - 1].value, fromResetValueDefault)
            : fromResetValueDefault
          const toResetValueDefault = 57600000
          const fromValueIndex = getIndexForValue(fromValue)
          const toResetValue = fromValue
            ? Math.max(beginningTimeOptions[fromValueIndex + 1].value, toResetValueDefault)
            : toResetValueDefault
          return (
            <div className={styles.interval}>
              <div className={styles.from}>
                <TimeSelect
                  name={`${member}.from`}
                  options={fromOptions}
                  resetValue={fromResetValue}
                  clearable={index === 0}
                  isCompareMode={isCompareMode}
                />
              </div>
              <div>-</div>
              <div className={styles.to}>
                <TimeSelect
                  name={`${member}.to`}
                  options={toOptions}
                  resetValue={toResetValue}
                  clearable={index === 0}
                  isCompareMode={isCompareMode}
                />
              </div>
              {fields.length > 1 &&
                index < fields.length - 1 && (
                  <div>{formatMessage(messages.hoursConjunction)}</div>
                )}
            </div>
          )
        })}
      </div>
      <div className={styles.intervalActions}>
        {canRemoveInterval && (
          <PTVIcon
            name='icon-close'
            onClick={() => fields.remove(fields.length - 1)}
          />
        )}
        {canAddNewInterval && (
          <PTVIcon
            name='icon-plus'
            onClick={() => {
              fields.push()
            }}
          />
        )}
      </div>
    </div>
  )
}
RenderIntervals.propTypes = {
  fields: fieldArrayPropTypes.fields,
  intervals: PropTypes.array,
  name: PropTypes.string,
  formatMessage: PropTypes.func,
  onChange: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  maxIntervals: PropTypes.number
}
RenderIntervals.defaultProps = {
  maxIntervals: 4
}

const Intervals = ({
  intl: { formatMessage },
  disabled,
  intervals,
  ...props
}) => (
  <FieldArray
    component={RenderIntervals}
    formatMessage={formatMessage}
    intervals={intervals}
    {...props}
  />
)
Intervals.propTypes = {
  onChange: PropTypes.func,
  intervals: PropTypes.array,
  intl: intlShape,
  disabled: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates
)(Intervals)
