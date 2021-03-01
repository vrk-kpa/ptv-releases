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
import PropTypes from 'prop-types'
import { formValueSelector } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
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
import styles from './styles.scss'
import cx from 'classnames'

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

const SpecialOpeningHourSelection = ({
  intl: { formatMessage },
  isPeriod,
  dateFrom,
  dateTo,
  timeFrom,
  timeTo,
  dayFrom,
  dayTo,
  compare,
  isCompareMode,
  splitView
}) => {
  const shouldFilter = !isPeriod && dayFrom && dayTo && dayTo === dayFrom
  const labelClass = cx(
    'col-md-6 mb-2',
    splitView && 'col-lg-10 col-xl-8',
    !isPeriod && splitView && 'col-lg-24',
    styles.labelWrap
  )
  const dateClass = cx(
    'col-24 col-sm-6 col-md-5 col-lg-6 col-xl-4 mb-2',
    splitView && 'col-lg-14 col-xl-16'
  )
  const dayClass = cx(
    'col-10 col-sm-6 col-xl-4 mb-2',
    splitView && 'col-lg-10 col-xl-6',
    isPeriod && splitView && 'offset-xl-8'
  )
  const timeClass = cx(
    'col-10 col-sm-6 col-xl-4 mb-2',
    splitView && 'col-lg-10 col-xl-6'
  )
  return (
    <div>
      <div className='form-group'>
        <OpeningHoursValidityType>
          <div className={styles.specialHoursSelection}>
            <div className='row align-itmes-center'>
              <div className={labelClass} >
                <NotifyLabel
                  shouldShowNotificationIcon={isPastDate(dateFrom)}
                  notificationText={formatMessage(messages.pastDate)}
                  labelText={formatMessage(messages.start)}
                  popupProps={{ maxWidth: 'mW160' }}
                />
              </div>
              {isPeriod &&
              <div className={dateClass}>
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
              <div className={dayClass}>
                <DaySelect name='dayFrom' label={null}
                  isCompareMode={false}
                  compare={compare}
                  disabled={isPeriod && !!dateFrom} />
              </div>
              <div className={timeClass}>
                <TimeSelect name='timeFrom'
                  isCompareMode={false}
                  compare={compare}
                  filterValue={shouldFilter && timeTo}
                  filterBefore
                />
              </div>
            </div>
            <div className='row'>
              <div className={labelClass}>
                <NotifyLabel
                  shouldShowNotificationIcon={isPastDate(dateTo)}
                  notificationText={formatMessage(messages.pastDate)}
                  labelText={formatMessage(messages.end)}
                  popupProps={{ maxWidth: 'mW160' }}
                  className={styles.labelWithIcon}
                />
              </div>
              {isPeriod &&
              <div className={dateClass}>
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
              <div className={dayClass}>
                <DaySelect name='dayTo' label={null} isCompareMode={false}
                  compare={compare}
                  disabled={isPeriod && !!dateTo} />
              </div>
              <div className={timeClass}>
                <TimeSelect name='timeTo'
                  isCompareMode={false}
                  compare={compare}
                  filterValue={shouldFilter && timeFrom}
                />
              </div>
            </div>
          </div>
        </OpeningHoursValidityType>
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
  timeFrom: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  timeTo: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  dayFrom: PropTypes.any,
  dayTo: PropTypes.any,
  isCompareMode: PropTypes.bool,
  splitView: PropTypes.bool
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
        timeFrom: dailyOpeningHoursValueSelector(state, `${path}.timeFrom`),
        timeTo: dailyOpeningHoursValueSelector(state, `${path}.timeTo`),
        dayFrom: dailyOpeningHoursValueSelector(state, `${path}.dayFrom`),
        dayTo: dailyOpeningHoursValueSelector(state, `${path}.dayTo`),
        dateFrom: dailyOpeningHoursValueSelector(state, `${path}.dateFrom`),
        dateTo: dailyOpeningHoursValueSelector(state, `${path}.dateTo`)
      }
    }
  )
)(SpecialOpeningHourSelection)
