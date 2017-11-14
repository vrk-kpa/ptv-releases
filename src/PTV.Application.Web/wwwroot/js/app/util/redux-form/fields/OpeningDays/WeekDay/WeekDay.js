import React from 'react'
import PropTypes from 'prop-types'
import Intervals from '../Intervals'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { Checkbox } from 'util/redux-form/fields'
import styles from '../styles.scss'

const messages = defineMessages({
  monday: {
    id: 'Containers.Channels.OpeningHours.Monday',
    defaultMessage: 'Maanantai'
  },
  tuesday: {
    id: 'Containers.Channels.OpeningHours.Tuesday',
    defaultMessage: 'Tiistai'
  },
  wednesday: {
    id: 'Containers.Channels.OpeningHours.Wednesday',
    defaultMessage: 'Keskiviikko'
  },
  thursday: {
    id: 'Containers.Channels.OpeningHours.Thursday',
    defaultMessage: 'Torstai'
  },
  friday: {
    id: 'Containers.Channels.OpeningHours.Friday',
    defaultMessage: 'Perjantai'
  },
  saturday: {
    id: 'Containers.Channels.OpeningHours.Saturday',
    defaultMessage: 'Lauantai'
  },
  sunday: {
    id: 'Containers.Channels.OpeningHours.Sunday',
    defaultMessage: 'Sunnuntai'
  },
  hoursConjunction: {
    id: 'Util.ReduxForm.DailyOpeningHour.HoursConjunction',
    defaultMessage: 'ja'
  }
})

const WeekDay = ({
  dayName,
  isActive,
  intervals,
  onActivate,
  intl: { formatMessage }
}) => {
  return (
    <div className={styles.weekday}>
      <div className='row'>
        <div className='col-lg-8'>
          <Checkbox
            name={`dailyOpeningHours.${dayName}.active`}
            onChange={onActivate}
            label={formatMessage(messages[dayName])}
          />
        </div>
        <div className='col-lg-16'>
          {isActive &&
            <Intervals
              name={`dailyOpeningHours.${dayName}.intervals`}
              intervals={intervals}
            />
          }
        </div>
      </div>
    </div>
  )
}
WeekDay.propTypes = {
  dayName: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl
)(WeekDay)
