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
import { PropTypes } from 'prop-types'
import moment from 'moment'
import { FieldArray } from 'redux-form/immutable'
import { TimeSelect } from 'util/redux-form/fields'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { withFormStates } from 'util/redux-form/HOC'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import _ from 'lodash'

const timeOptions = _.range(0, 96).map(index => {
  const minute = 60 * 1000
  const time = moment.utc(index * minute * 15)
  return {
    label: time.format('HH:mm'),
    value: time.valueOf()
  }
})

const messages = defineMessages({
  hoursConjunction: {
    id: 'Util.ReduxForm.DailyOpeningHour.HoursConjunction',
    defaultMessage: 'ja'
  }
})

const RenderIntervals = ({
  fields,
  formatMessage,
  name,
  isReadOnly,
  intervals,
  isCompareMode,
  ...props
}) => {
  const lastInterval = intervals.length > 1
    ? _.last(_.dropRight(intervals)) // last but one
    : _.first(intervals)
  const maximumToValue = _.get(lastInterval, 'to.input.value')
  const maximumToIndex = _.findIndex(timeOptions, ({ value }) => value === maximumToValue)
  const intervalOptions = _.drop(timeOptions, maximumToIndex + 1)
  const allFiled = intervals.every(interval => interval.from.input.value && interval.to.input.value)
  const canAddNewInterval = allFiled && !isReadOnly && fields.length > 0 && fields.length < 4
  const canRemoveInterval = !isReadOnly && fields.length > 1
  return (
    <div className={styles.intervalWrap}>
      <div className={styles.intervalItems}>
        {fields.map((member, index) => {
          const shouldFilter = fields.length !== 1 && fields.length - 1 === index
          const fromValue = _.get(intervals[index], 'from.input.value')
          return (
            <div className={styles.interval}>
              <div className={styles.from}>
                <TimeSelect
                  name={`${member}.from`}
                  options={shouldFilter
                    ? intervalOptions
                    : undefined}
                  resetValue={28800000}
                  clearable={index === 0}
                  isCompareMode={isCompareMode}
                />
              </div>
              <div>-</div>
              <div className={styles.to}>
                <TimeSelect
                  name={`${member}.to`}
                  options={shouldFilter
                    ? fromValue
                        ? intervalOptions.filter(x => x.value > fromValue)
                        : intervalOptions
                    : undefined}
                  resetValue={57600000}
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
  fields: PropTypes.array,
  intervals: PropTypes.array,
  name: PropTypes.string,
  formatMessage: PropTypes.func,
  onChange: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool
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
