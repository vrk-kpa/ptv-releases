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
import { formValueSelector } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Label } from 'sema-ui-components'
import NotifyLabel from 'appComponents/NotifyLabel'
import {
  DaySelect,
  TimeSelect,
  DatePicker,
  OpeningHoursValidityType
} from 'util/redux-form/fields'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { isPastDate } from 'util/helpers'
import { range } from 'lodash'
import styles from './styles.scss'

import moment from 'moment'

const messages = defineMessages({
  start: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.StartDate.Title',
    defaultMessage: 'Alkaa'
  },
  end: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.EndDate.Title',
    defaultMessage: 'Päättyy'
  },
  pastDate: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PastDate.Text',
    defaultMessage: 'Menneet palveluajat eivät näy loppukäyttäjille.'
  }
})

const timeOptions = range(0, 96).map(index => {
  const minute = 60 * 1000
  const time = moment.utc(index * minute * 15)
  return {
    label: time.format('HH:mm'),
    value: time.valueOf()
  }
})

const SpecialOpeningHourSelection = ({
  intl: { formatMessage },
  isPeriod,
  dateFrom,
  dateTo,
  timeFrom,
  dayFrom,
  dayTo,
  compare,
  isCompareMode
}) => {
  const shouldFilter = !isPeriod && dayFrom && dayTo && dayTo === dayFrom
  const dateTimeClass = isCompareMode ? 'col-24' : 'col'
  return (
    <div>
      <div className='form-group'>
        <OpeningHoursValidityType />
      </div>
      <div className='form-group'>
        <div className='row'>
          <div className={dateTimeClass}>
            <NotifyLabel
              shouldShowNotificationIcon={isPastDate(dateFrom)}
              notificationText={formatMessage(messages.pastDate)}
              labelText={formatMessage(messages.start)}
              popupProps={{ maxWidth: 'mW160' }}
            />
            <div className='row'>
              {isPeriod &&
                <div className='col-md-8 mb-2'>
                  <div className={styles.datePicker}>
                    <DatePicker
                      name='dateFrom'
                      label={null}
                      section='specialOpeningHours'
                      type='from'
                      isCompareMode={false}
                      compare={compare}
                      setDay
                      skipValidation
                    />
                  </div>
                </div>
              }
              <div className='col-md-8 mb-2'>
                <DaySelect name='dayFrom' label={null}
                  isCompareMode={false}
                  compare={compare}
                  disabled={isPeriod && !!dateFrom} />
              </div>
              <div className='col-md-8 mb-2'>
                <TimeSelect name='timeFrom'
                  isCompareMode={false}
                  compare={compare} />
              </div>
            </div>
          </div>
          <div className={dateTimeClass}>
            <NotifyLabel
              shouldShowNotificationIcon={isPastDate(dateTo)}
              notificationText={formatMessage(messages.pastDate)}
              labelText={formatMessage(messages.end)}
              popupProps={{ maxWidth: 'mW160' }}
            />
            <div className='row'>
              {isPeriod &&
                <div className='col-md-8 mb-2'>
                  <div className={styles.datePicker}>
                    <DatePicker
                      name='dateTo'
                      label={null}
                      section='specialOpeningHours'
                      type='to'
                      isCompareMode={false}
                      compare={compare}
                      setDay
                      skipValidation
                    />
                  </div>
                </div>
              }
              <div className='col-md-8 mb-2'>
                <DaySelect name='dayTo' label={null} isCompareMode={false}
                  compare={compare}
                  disabled={isPeriod && !!dateTo} />
              </div>
              <div className='col-md-8 mb-2'>
                <TimeSelect name='timeTo'
                  isCompareMode={false}
                  compare={compare}
                  options={shouldFilter
                    ? timeFrom
                      ? [...timeOptions.filter(x => x.value > timeFrom)]
                      : timeOptions
                    : timeOptions}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

SpecialOpeningHourSelection.defaultProps = {
  name: 'specialOpeningHour'
}
SpecialOpeningHourSelection.propTypes = {
  intl: intlShape,
  isPeriod: PropTypes.bool.isRequired,
  compare: PropTypes.bool,
  dateFrom: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  dateTo: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  timeFrom: PropTypes.any,
  dayFrom: PropTypes.any,
  dayTo: PropTypes.any,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withPath,
  withFormStates,
  connect(
    (state, { formName, path, field }) => {
      const dailyOpeningHoursValueSelector = formValueSelector(formName)
      const isPeriod = dailyOpeningHoursValueSelector(state, `${path}.isPeriod`) || false
      return {
        isPeriod,
        dateFrom: dailyOpeningHoursValueSelector(state, `${path}.dateFrom`),
        timeFrom: dailyOpeningHoursValueSelector(state, `${path}.timeFrom`),
        dayFrom: dailyOpeningHoursValueSelector(state, `${path}.dayFrom`),
        dayTo: dailyOpeningHoursValueSelector(state, `${path}.dayTo`),
        dateTo: dailyOpeningHoursValueSelector(state, `${path}.dateTo`)
      }
    }
  )
)(SpecialOpeningHourSelection)
