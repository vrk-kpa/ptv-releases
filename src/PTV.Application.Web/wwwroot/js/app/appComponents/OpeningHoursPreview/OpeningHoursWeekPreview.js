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
import ImmutablePropTypes from 'react-immutable-proptypes'
import IntervalsPreview from './IntervalsPreview'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import { Label } from 'sema-ui-components'
import moment from 'moment'
import styles from './styles.scss'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'

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

const PreviewDay = compose(
  localizeProps({
    languageTranslationType: languageTranslationTypes.data
  })
)(({ dayName, name, formatMessage }) =>
  <div className={styles.dayName}>
    {name || formatMessage(messages[dayName])}
  </div>
)

const NonStop = compose(
  localizeProps({
    getId: () => messages.isOpenNonStop.id,
    languageTranslationType: languageTranslationTypes.data
  })
)(({ formatMessage, name }) =>
  <div>{name || formatMessage(messages.isOpenNonStop)}</div>
)

const OpeningHoursWeekPreview = ({
  openingHours,
  compare,
  language,
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
  const title = openingHours && openingHours.getIn(['title', language])
  return (
    <div>
      {title && <Label labelText={title} />}
      <div className={styles.previewBlockItem}>
        {isPeriod &&
          <div>
              {moment(periodFrom).isValid() &&
                moment(periodFrom).format('DD.MM.YYYY')}
            <span className={styles.dash}>-</span>
              {moment(periodTo).isValid() &&
                moment(periodTo).format('DD.MM.YYYY')}
          </div>}
        {isOpenNonStop
        ? <NonStop formatMessage={formatMessage} compare={compare} />
        : openingHours && dayOfWeek.map((dayName, index) => {
          const openingDay = openingHours.getIn(['dailyOpeningHours', dayName])
          return openingDay && openingDay.get('active') && openingDay.has('intervals') && (
            <div key={index} className={styles.day}>
              <PreviewDay
                id={messages[dayName].id}
                dayName={dayName}
                formatMessage={formatMessage}
                compare={compare}
              />
              <IntervalsPreview
                key={index}
                dayName={dayName}
                openingDay={openingDay}
                formatMessage={formatMessage}
              />
            </div>
          )
        })}
      </div>
    </div>
  )
}
OpeningHoursWeekPreview.propTypes = {
  openingHours: ImmutablePropTypes.map,
  compare: PropTypes.bool,
  language: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  connect(
    (state, props) => ({
      language: !props.compare && getContentLanguageCode(state, props) ||
          getSelectedComparisionLanguageCode(state, props)
    })
  )
)(OpeningHoursWeekPreview)
