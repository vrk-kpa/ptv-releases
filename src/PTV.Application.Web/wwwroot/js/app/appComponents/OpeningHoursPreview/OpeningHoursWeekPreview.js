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
import IntervalsPreview from './IntervalsPreview'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { compose } from 'redux'
import { Label } from 'sema-ui-components'
import moment from 'moment'
import styles from './styles.scss'

const messages = defineMessages({
  monday: {
    id: 'Containers.Channels.OpeningHours.Mo',
    defaultMessage: 'Ma'
  },
  tuesday: {
    id: 'Containers.Channels.OpeningHours.Tu',
    defaultMessage: 'Ti'
  },
  wednesday: {
    id: 'Containers.Channels.OpeningHours.We',
    defaultMessage: 'Ke'
  },
  thursday: {
    id: 'Containers.Channels.OpeningHours.Th',
    defaultMessage: 'To'
  },
  friday: {
    id: 'Containers.Channels.OpeningHours.Fr',
    defaultMessage: 'Pe'
  },
  saturday: {
    id: 'Containers.Channels.OpeningHours.Sa',
    defaultMessage: 'La'
  },
  sunday: {
    id: 'Containers.Channels.OpeningHours.Su',
    defaultMessage: 'Su'
  },
  hoursConjunction: {
    id: 'Util.ReduxForm.DailyOpeningHour.HoursConjunction',
    defaultMessage: 'ja'
  },
  isOpenNonStop: {
    id: 'Containers.Channels.Common.OpeningHours.Nonstop.Title',
    defaultMessage: 'Aina avoinna'
  }
})

const OpeningHoursWeekPreview = ({
  openingHours,
  intl: { formatMessage }
}) => {
  const dayOfWeek = [
    'monday',
    'tuesday',
    'wednesday',
    'thursday',
    'friday',
    'saturday',
    'sunday'
  ]
  const isOpenNonStop = openingHours && openingHours.get('isOpenNonStop')
  const isPeriod = openingHours && openingHours.get('isPeriod')
  const periodFrom = openingHours && openingHours.get('dateFrom')
  const periodTo = openingHours && openingHours.get('dateTo')
  const title = openingHours && openingHours.get('title')
  return (
    <div>
      {title && <Label labelText={title} />}
      <div className={styles.previewBlockItem}>
        {isPeriod &&
          <div>
              {moment(periodFrom).isValid() &&
                moment(periodFrom).format('DD.MM.YYYY')}
              -
              {moment(periodTo).isValid() &&
                moment(periodTo).format('DD.MM.YYYY')}
          </div>}
        {isOpenNonStop
        ? <div>{formatMessage(messages.isOpenNonStop)}</div>
        : openingHours && dayOfWeek.map((dayName, index) => {
          const openingDay = openingHours.getIn(['dailyOpeningHours', dayName])
          return openingDay && openingDay.get('active') && openingDay.has('intervals') && (
            <div className={styles.day}>
              <div className={styles.dayName}>{formatMessage(messages[dayName])}</div>
              <IntervalsPreview
                key={index}
                dayName={dayName}
                openingDay={openingDay}
              />
            </div>
          )
        })}
      </div>
    </div>
  )
}
OpeningHoursWeekPreview.propTypes = {
  openingHours: PropTypes.array,
  intl: intlShape
}

export default compose(
  injectIntl
)(OpeningHoursWeekPreview)
