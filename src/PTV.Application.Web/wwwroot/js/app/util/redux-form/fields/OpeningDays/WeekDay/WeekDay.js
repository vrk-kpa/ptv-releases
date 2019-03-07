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
import Intervals from '../Intervals'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Checkbox } from 'sema-ui-components'
import styles from '../styles.scss'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'

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
  label,
  intervals,
  onActivate,
  intl: { formatMessage },
  isCompareMode,
  compare,
  disabled,
  checked,
  ...rest
}) => {
  return (
    <div className={styles.weekday}>
      <div className='row'>
        <div className='col-lg-8'>
          <Checkbox
            name={`dailyOpeningHours.${dayName}.active`}
            onChange={onActivate}
            label={label || formatMessage(messages[dayName])}
            disabled={disabled}
            checked={checked}
          />
        </div>
        <div className='col-lg-16'>
          {checked && !disabled &&
            <Intervals
              name={`dailyOpeningHours.${dayName}.intervals`}
              intervals={intervals}
              isCompareMode={isCompareMode}
              compare={compare}
            />
          }
        </div>
      </div>
    </div>
  )
}
WeekDay.propTypes = {
  dayName: PropTypes.string,
  intl: intlShape,
  compare: PropTypes.bool,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  localizeProps({
    languageTranslationType: languageTranslationTypes.data,
    getId: item => item.dayName && messages[item.dayName].id,
    idAttribute: 'dayName',
    nameAttribute: 'label'
  })
)(WeekDay)
