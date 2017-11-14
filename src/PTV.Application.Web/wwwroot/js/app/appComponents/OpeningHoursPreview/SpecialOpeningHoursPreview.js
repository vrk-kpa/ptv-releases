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
// import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { formValueSelector } from 'redux-form/immutable'
import { List } from 'immutable'
import { injectIntl, intlShape } from 'react-intl'
import { Label } from 'sema-ui-components'
import moment from 'moment'
import styles from './styles.scss'
import { messages } from 'util/redux-form/fields/DaySelect/messages'

const SpecialOpeningHoursPreview = ({
  specialOpeningHours,
  intl: { formatMessage }
}) => (
  specialOpeningHours && specialOpeningHours.size > 0 &&
  <div className={styles.previewBlock}>
    {specialOpeningHours
    .filter(x => typeof x !== 'undefined')
    .map((specialOpeningHour, index) => {
      const title = specialOpeningHour.get('title')
      const dayFrom =
        specialOpeningHour.has('dayFrom') &&
        specialOpeningHour.get('dayFrom') !== null &&
        formatMessage(messages[specialOpeningHour.get('dayFrom')])
      const dayTo =
        specialOpeningHour.has('dayTo') &&
        specialOpeningHour.get('dayTo') !== null &&
        formatMessage(messages[specialOpeningHour.get('dayTo')])
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
      return specialOpeningHour && (
        <div className={styles.previewBlockItem}>
          {title && <Label labelText={title} />}
          <div key={index} className={styles.record}>
            <div className={styles.from}>
              <span>{dateFromFormatted}</span>
              <span className={styles.capitalize}>{dayFrom}</span>
              <span>{timeFrom}</span>
            </div>
            <div>{(dateTo || dayTo || timeTo) && '-'}</div>
            <div className={styles.to}>
              <span>{dateToFormatted}</span>
              <span className={styles.capitalize}>{dayTo}</span>
              <span>{timeTo}</span>
            </div>
          </div>
        </div>
      )
    })}
  </div>
)
SpecialOpeningHoursPreview.propTypes = {
  specialOpeningHours: ImmutablePropTypes.list.isRequired,
  intl: intlShape
}

export default compose(
  connect(
    (state, { formName }) => {
      const getFormValues = formValueSelector(formName)
      return {
        specialOpeningHours: getFormValues(state, 'openingHours.specialOpeningHours') || List()
      }
    }
  ),
  injectIntl
)(SpecialOpeningHoursPreview)
