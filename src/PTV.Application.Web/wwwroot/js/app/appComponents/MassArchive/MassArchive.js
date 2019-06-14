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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { formValues } from 'redux-form/immutable'
import { ArchivingType, DatePicker } from 'util/redux-form/fields'
import { timingTypes, massToolTypes } from 'enums'
import { getArchivingType } from './selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'

const messages = defineMessages({
  archiveDay: {
    id: 'AppComponents.MassArchive.ArchiveDay.Title',
    defaultMessage: 'Arkistoitumispäivä'
  },
  archiveDates: {
    id: 'AppComponents.MassArchive.ArchiveDates.Title',
    defaultMessage: 'Arkistointitapa'
  },
  invalidDate: {
    id: 'AppComponents.MassArchive.InvalidDate.Tooltip',
    defaultMessage: 'You have to select archive date'
  }
})

const MassArchive = ({
  intl: { formatMessage },
  archivingType,
  timingType,
  archiveAt
}) => {
  return (
    <div className='row'>
      <div className='col-24 mb-2'>
        <ArchivingType action={massToolTypes.ARCHIVE} />
      </div>
      <div className='col-10 col-lg-24 mb-4'>
        <DatePicker
          type='to'
          name='archiveAt'
          futureDays
          inputProps={{
            disabled: archivingType === timingTypes.NOW
          }}
          className={styles.archiveDatepicker}
          label={formatMessage(messages.archiveDay)}
          labelRequired
        />
      </div>
    </div>
  )
}

MassArchive.propTypes = {
  intl: intlShape.isRequired,
  archivingType: PropTypes.string,
  timingType: PropTypes.string,
  archiveAt: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ])
}

export default compose(
  injectIntl,
  injectFormName,
  formValues({ timingType: 'timingType', archiveAt: 'archiveAt' }),
  connect((state, ownProps) => {
    return {
      archivingType: getArchivingType(state, ownProps),
      timingType: ownProps.timingType,
      archiveAt: ownProps.archiveAt
    }
  })
)(MassArchive)
