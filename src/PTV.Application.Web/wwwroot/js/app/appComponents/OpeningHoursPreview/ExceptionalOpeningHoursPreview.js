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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Label } from 'sema-ui-components'
import { isPastDate } from 'util/helpers'
import NotifyLabel from 'appComponents/NotifyLabel'
import moment from 'moment'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import styles from './styles.scss'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { getSelectedLanguage } from 'Intl/Selectors'
import withPath from 'util/redux-form/HOC/withPath'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import { getExceptionalOpeningHours } from './selectors'

const messages = defineMessages({
  closeMessage: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.ClosedMessage',
    defaultMessage: 'Suljettu'
  },
  openMessage: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.OpenMessage',
    defaultMessage: 'Avoinna'
  },
  pastDate: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PastDate.Text',
    defaultMessage: 'Menneet palveluajat eivät näy loppukäyttäjille.'
  }
})

const ExceptionalOpeningHoursPreview = ({
  exceptionalOpeningHours,
  language,
  intl: { formatMessage }
}) => {
  return (
    exceptionalOpeningHours && exceptionalOpeningHours.size > 0 &&
    <div className={styles.previewBlock}>
      {exceptionalOpeningHours
        .filter(x => typeof x !== 'undefined')
        .map((exceptionalOpeningHour, index) => {
          const isClosedForPeriod =
            exceptionalOpeningHour.get('closedForPeriod')
          const isPeriod =
            exceptionalOpeningHour.get('isPeriod')
          const title = exceptionalOpeningHour.getIn(['title', language])
          const openCloseMessage = isClosedForPeriod
            ? formatMessage(messages.closeMessage)
            : formatMessage(messages.openMessage)
          const timeFrom =
            exceptionalOpeningHour.has('timeFrom') &&
            exceptionalOpeningHour.get('timeFrom') !== null &&
            <span>{moment.utc(exceptionalOpeningHour.get('timeFrom')).format('HH:mm')}</span>
          const timeTo =
            exceptionalOpeningHour.has('timeTo') &&
            exceptionalOpeningHour.get('timeTo') !== null &&
            <span>{moment.utc(exceptionalOpeningHour.get('timeTo')).format('HH:mm')}</span>
          const dateFrom =
            exceptionalOpeningHour.has('dateFrom') &&
            exceptionalOpeningHour.get('dateFrom') !== null &&
            exceptionalOpeningHour.get('dateFrom')
          const dateFromFormatted = moment(dateFrom).isValid() && moment(dateFrom).format('DD.MM.YYYY')
          const dateTo =
            exceptionalOpeningHour.has('dateTo') &&
            exceptionalOpeningHour.get('dateTo') !== null &&
            exceptionalOpeningHour.get('dateTo')
          const dateToFormatted = moment(dateTo).isValid() && moment(dateTo).format('DD.MM.YYYY')
          const shouldShowNotificationIcon = moment(dateTo).isValid() && isPastDate(dateTo) ||
            moment(dateFrom).isValid() && isPastDate(dateFrom) || false
          return (
            <div key={index} className={styles.previewBlockItem}>
              <div className={styles.record}>
                <div className={styles.emphasize}>
                  <span>{dateFromFormatted}</span>
                </div>
                {dateTo && isPeriod && <div className={styles.emphasize}>{'-'}</div>}
                {isPeriod && dateToFormatted && (
                  <div className={styles.emphasize}>
                    <span>{dateToFormatted}</span>
                  </div>
                )}
                {shouldShowNotificationIcon
                  ? <NotifyLabel
                    shouldShowNotificationIcon
                    notificationText={formatMessage(messages.pastDate)}
                    labelText={title}
                    popupProps={{ maxWidth: 'mW160' }}
                    iconClass={styles.notifyIcon}
                  />
                  : title && <div className={styles.to}><Label labelText={title} /></div>
                }
              </div>
              <div className={styles.record}>
                <div>{openCloseMessage}</div>
                <div className={styles.from}>
                  {!isClosedForPeriod && <span>{timeFrom}</span>}
                </div>
                <div>{timeTo && !isClosedForPeriod && '-'}</div>
                <div className={styles.to}>
                  {!isClosedForPeriod && <span>{timeTo}</span>}
                </div>
              </div>
            </div>
          )
        })}
    </div>
  )
}
ExceptionalOpeningHoursPreview.propTypes = {
  exceptionalOpeningHours: ImmutablePropTypes.list.isRequired,
  language: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  withPath,
  withLanguageKey,
  connect(
    (state, ownProps) => {
      // const getFormValues = formValueSelector(ownProps.formName)
      const pathParts = [ownProps.path, ownProps.field, 'openingHours.exceptionalOpeningHours']
      const fromIndex = pathParts.lastIndexOf(undefined)
      const composedPath = fromIndex !== -1 && pathParts.slice(fromIndex + 1, pathParts.length).join('.') ||
        pathParts.join('.')
      const holidayPath = composedPath.replace('exceptionalOpeningHours', 'holidayHours')
      return {
        exceptionalOpeningHours: getExceptionalOpeningHours(state, { formName: ownProps.formName, composedPath, holidayPath }),
        // getFormValues(state, composedPath) || List(),
        language: !ownProps.compare
          ? getContentLanguageCode(state, ownProps) || getSelectedLanguage(state)
          : getSelectedComparisionLanguageCode(state, ownProps) || getSelectedLanguage(state)
      }
    }
  )
)(ExceptionalOpeningHoursPreview)
