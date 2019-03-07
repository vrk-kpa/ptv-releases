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
import { Field, formValueSelector } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  DatePicker,
  TimeSelect,
  OpeningHoursValidityType
} from 'util/redux-form/fields'
import {
  Label
} from 'sema-ui-components'
import {
  RenderCheckBox
} from 'util/redux-form/renders'
import NotifyLabel from 'appComponents/NotifyLabel'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import { isPastDate } from 'util/helpers'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  closedDaySingle: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.ClosedDaySingle',
    defaultMessage: 'Suljettu koko päivän'
  },
  closedDayRange: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.ClosedDayRange',
    defaultMessage: 'Suljettu koko ajanjakson'
  },
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

const ExceptionalOpeningHourSelection = ({
  isClosedForPeriod,
  isPeriod,
  compare,
  dateFrom,
  dateTo,
  intl: { formatMessage }
}) => (
  <div>
    <div className='form-group'>
      <OpeningHoursValidityType
        dayRange
      />
    </div>
    <div className='form-group'>
      <div className='row'>
        <div className='col'>
          <div className={styles.dateRow}>
            <div>
              <NotifyLabel
                shouldShowNotificationIcon={isPastDate(dateFrom)}
                notificationText={formatMessage(messages.pastDate)}
                labelText={formatMessage(messages.start)}
                popupProps={{ maxWidth: 'mW160' }}
              />
            </div>
            <div className={styles.datePicker}>
              <DatePicker
                name='dateFrom'
                label={null}
                section='exceptionalOpeningHours'
                type='from'
                isCompareMode={false}
                compare={compare}
              />
            </div>
            {!isClosedForPeriod &&
              <div>
                <TimeSelect
                  name='timeFrom'
                  isCompareMode={false}
                  compare={compare}
                />
              </div>
            }
          </div>
        </div>
        <div className='col'>
          <div className={styles.dateRow}>
            {(isPeriod || !isClosedForPeriod) &&
              <div>
                <NotifyLabel
                  shouldShowNotificationIcon={isPastDate(dateTo)}
                  notificationText={formatMessage(messages.pastDate)}
                  labelText={formatMessage(messages.end)}
                  popupProps={{ maxWidth: 'mW160' }}
                />
              </div>
            }
            {isPeriod &&
              <div className={styles.datePicker}>
                <DatePicker
                  name='dateTo'
                  label={null}
                  section='exceptionalOpeningHours'
                  type='to'
                  isCompareMode={false}
                  compare={compare}
                />
              </div>
            }
            {!isClosedForPeriod &&
              <div>
                <TimeSelect name='timeTo'
                  isCompareMode={false}
                  compare={compare}
                />
              </div>
            }
          </div>
        </div>
      </div>
    </div>
    <div className='form-group'>
      <div className='row'>
        <div className='col-md-8'>
          <Field
            name='closedForPeriod'
            component={RenderCheckBox}
            label={isPeriod ? formatMessage(messages.closedDayRange) : formatMessage(messages.closedDaySingle)}
            type='checkbox'
            isCompareMode={false}
            compare={compare}
          />
        </div>
      </div>
    </div>
  </div>
)

ExceptionalOpeningHourSelection.propTypes = {
  isPeriod: PropTypes.bool.isRequired,
  isClosedForPeriod: PropTypes.bool.isRequired,
  compare: PropTypes.bool,
  intl: intlShape,
  dateFrom: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  dateTo: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ])
}

export default compose(
  injectIntl,
  injectFormName,
  withPath,
  connect(
    (state, { formName, path, field }) => {
      const openingHoursValueSelector = formValueSelector(formName)
      const isPeriod = openingHoursValueSelector(state, `${path}.isPeriod`) || false
      const isClosedForPeriod = openingHoursValueSelector(state, `${path}.closedForPeriod`) || false
      return {
        isPeriod,
        isClosedForPeriod,
        dateFrom: openingHoursValueSelector(state, `${path}.dateFrom`),
        dateTo: openingHoursValueSelector(state, `${path}.dateTo`)
      }
    }
  )
)(ExceptionalOpeningHourSelection)
