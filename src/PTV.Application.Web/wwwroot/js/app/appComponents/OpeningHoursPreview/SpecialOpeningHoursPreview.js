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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { formValueSelector } from 'redux-form/immutable'
import { List } from 'immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import { isPastDate } from 'util/helpers'
import NotifyLabel from 'appComponents/NotifyLabel'
import moment from 'moment'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import { messages } from 'util/redux-form/fields/DaySelect/messages'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import withPath from 'util/redux-form/HOC/withPath'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'

const DayPreview = compose(
  injectIntl,
  localizeProps({
    languageTranslationType: languageTranslationTypes.data,
    getId: item => item.dayName && messages[item.dayName].id,
    idAttribute: 'dayName',
    nameAttribute: 'dayLabel'
  })
)(({ style, dateFormatted, dayName, dayLabel, time, intl: { formatMessage }, showDate }) => (
  <div className={style}>
    {showDate && <span>{dateFormatted}</span>}
    <span className={styles.capitalize}>{dayLabel || dayName && formatMessage(messages[dayName])}</span>
    <span>{time}</span>
  </div>
))

const SpecialOpeningHoursPreview = ({
  specialOpeningHours,
  compare,
  language,
  intl: { formatMessage }
}) => (
  specialOpeningHours && specialOpeningHours.size > 0 &&
  <div className={styles.previewBlock}>
    {specialOpeningHours
      .filter(x => typeof x !== 'undefined')
      .map((specialOpeningHour, index) => {
        const title = specialOpeningHour.getIn(['title', language])
        const dayFrom = specialOpeningHour.get('dayFrom') || null
        const dayTo = specialOpeningHour.get('dayTo') || null
        const isPeriod = specialOpeningHour.get('isPeriod')
        const timeFrom =
          specialOpeningHour.has('timeFrom') &&
          specialOpeningHour.get('timeFrom') !== null &&
          <span>{moment.utc(specialOpeningHour.get('timeFrom')).format('HH:mm')}</span>
        const timeTo =
          specialOpeningHour.has('timeTo') &&
          specialOpeningHour.get('timeTo') !== null &&
          <span>{moment.utc(specialOpeningHour.get('timeTo')).format('HH:mm')}</span>
        const dateFrom =
          specialOpeningHour.has('dateFrom') &&
          specialOpeningHour.get('dateFrom') !== null &&
          specialOpeningHour.get('dateFrom')
        const dateFromFormatted = moment(dateFrom).isValid() && moment(dateFrom).format('DD.MM.YYYY')
        const dateTo =
          specialOpeningHour.has('dateTo') &&
          specialOpeningHour.get('dateTo') !== null &&
          specialOpeningHour.get('dateTo')
        const dateToFormatted = moment(dateTo).isValid() && moment(dateTo).format('DD.MM.YYYY')
        const shouldShowNotificationIcon = moment(dateTo).isValid() && isPastDate(dateTo) ||
          moment(dateFrom).isValid() && isPastDate(dateFrom) || false
        return specialOpeningHour && (
          <div key={index} className={styles.previewBlockItem}>
            {shouldShowNotificationIcon
              ? <NotifyLabel
                shouldShowNotificationIcon
                notificationText={formatMessage(messages.pastDate)}
                labelText={title}
                popupProps={{ maxWidth: 'mW160' }}
                iconClass={styles.notifyIcon}
              />
              : title && <Label labelText={title} />
            }
            <div className={styles.record}>
              <DayPreview
                style={styles.from}
                dateFormatted={dateFromFormatted}
                dayName={dayFrom}
                time={timeFrom}
                compare={compare}
                showDate={isPeriod}
              />
              <div>{((dateTo && isPeriod) || dayTo || timeTo) && '-'}</div>
              <DayPreview
                style={styles.to}
                dateFormatted={dateToFormatted}
                dayName={dayTo}
                time={timeTo}
                compare={compare}
                showDate={isPeriod}
              />
            </div>
          </div>
        )
      })}
  </div>
)
SpecialOpeningHoursPreview.propTypes = {
  specialOpeningHours: ImmutablePropTypes.list.isRequired,
  compare: PropTypes.bool,
  language: PropTypes.string,
  intl: intlShape
}

export default compose(
  withPath,
  withLanguageKey,
  connect(
    (state, ownProps) => {
      const getFormValues = formValueSelector(ownProps.formName)
      const pathParts = [ownProps.path, ownProps.field, 'openingHours.specialOpeningHours']
      const fromIndex = pathParts.lastIndexOf(undefined)
      const composedPath = fromIndex !== -1 && pathParts.slice(fromIndex + 1, pathParts.length).join('.') || pathParts.join('.')
      return {
        specialOpeningHours: getFormValues(state, composedPath) || List(),
        language: !ownProps.compare && getContentLanguageCode(state, ownProps) ||
          getSelectedComparisionLanguageCode(state, ownProps)
      }
    }
  ),
  injectIntl
)(SpecialOpeningHoursPreview)
