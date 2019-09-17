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
import moment from 'moment'
import styles from './styles.scss'
import { defineMessages } from 'util/react-intl'

const messages = defineMessages({
  hoursConjunction: {
    id: 'Util.ReduxForm.DailyOpeningHour.HoursConjunction',
    defaultMessage: 'ja'
  }
})

const IntervalsPreview = ({
  openingDay,
  dayName,
  formatMessage
}) => openingDay
  ? <div className={styles.interval}>
    {openingDay.get('intervals')
      .filter(x => typeof x !== 'undefined')
      .map((interval, index, intervals) => {
        const shouldShow =
          interval.get('from') !== null &&
          interval.get('to') !== null
        const toFormatedValue = moment.utc(interval.get('to')).format('HH:mm')
        return shouldShow && (
          <div key={index} className={styles.intervalItem}>
            <div>{moment.utc(interval.get('from')).format('HH:mm')}</div>
            <div>-</div>
            <div>{
              toFormatedValue === '00:00'
                ? '24:00'
                : toFormatedValue
            }</div>
            {index < intervals.size - 1 &&
              <div className={styles.hoursConjunction}>{formatMessage(messages.hoursConjunction)}</div>
            }
          </div>
        )
      })}
  </div>
  : null

IntervalsPreview.propTypes = {
  openingDay: PropTypes.object,
  dayName: PropTypes.string,
  formatMessage: PropTypes.func
}

export default IntervalsPreview
