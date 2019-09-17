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
import withState from 'util/withState'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { timingTypes, massToolTypes } from 'enums'
import {
  getReviewListCount,
  getArchiveFilterDate,
  getPublishFilterDate,
  getPublishingType
} from './selectors'
import { getExpireOn } from 'selectors/common'
import MassPublishTable from './MassPublishTable'
import TooltipLabel from 'appComponents/TooltipLabel'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Tabs, Tab } from 'sema-ui-components'
import { PublishingType, DatePicker } from 'util/redux-form/fields'
import styles from './styles.scss'

const messages = defineMessages({
  approvedTabLabel: {
    id: 'AppComponents.MassPublishForm.ApprovedTab.Title',
    defaultMessage: 'Hyväksytyt ({approvedCount})'
  },
  notApprovedTabLabel: {
    id: 'AppComponents.MassPublishForm.NotApprovedTab.Title',
    defaultMessage: 'Hyväksymättömät  ({notApprovedCount})'
  },
  approvedEmptyTableMessage: {
    id: 'AppComponents.MassPublishForm.ApprovedEmptyTableMessage.Title',
    defaultMessage: 'There are no approved records'
  },
  notApprovedEmptyTableMessage: {
    id: 'AppComponents.MassPublishForm.NotApprovedEmptyTableMessage.Title',
    defaultMessage: 'There are no unapproved records'
  },
  releaseDates: {
    id: 'AppComponents.MassPublishForm.ReleaseDates.Title',
    defaultMessage: 'Julkaisupäivämäärät'
  },
  releaseDatesTooltip: {
    id: 'AppComponents.MassPublishForm.ReleaseDates.Tooltip',
    defaultMessage: 'Julkaisupäivämäärät tooltip'
  },
  releaseDay: {
    id: 'AppComponents.MassPublishForm.ReleaseDay.Title',
    defaultMessage: 'Julkaisupäivä'
  },
  archiveDay: {
    id: 'AppComponents.MassPublishForm.ArchiveDay.Title',
    defaultMessage: 'Arkistoitumispäivä',
    description: 'AppComponents.MassArchive.ArchiveDay.Title'
  },
  invalidDate: {
    id: 'AppComponents.MassPublishForm.InvalidDate.Tooltip',
    defaultMessage: 'You have to select publish date'
  }
})

const MassPublish = ({
  activeIndex,
  approvedCount,
  notApprovedCount,
  updateUI,
  closeDialog,
  publishingType,
  publishFilterDate,
  expireOn,
  archiveFilterDate,
  timingType,
  publishAt,
  intl: { formatMessage }
}) => {
  const handleOnChange = activeIndex => updateUI('activeIndex', activeIndex)

  // If one value is set and the other is not, let's use the set value
  if (!publishFilterDate || publishFilterDate === 0) {
    publishFilterDate = expireOn
  }

  return (
    <div className='row'>
      <div className='col-lg-12'>
        <Tabs
          index={activeIndex}
          onChange={handleOnChange}
          className={styles.approvalTabs}
        >
          <Tab label={formatMessage(messages.approvedTabLabel, { approvedCount: approvedCount || 0 })}>
            <MassPublishTable
              approved
              emptyMessage={formatMessage(messages.approvedEmptyTableMessage)}
            />
          </Tab>
          <Tab label={formatMessage(messages.notApprovedTabLabel, { notApprovedCount: notApprovedCount || 0 })}>
            <MassPublishTable
              closeDialog={closeDialog}
              emptyMessage={formatMessage(messages.notApprovedEmptyTableMessage)}
            />
          </Tab>
        </Tabs>
      </div>
      <div className='col-lg-11 offset-lg-1'>
        <div className={styles.publishingType}>
          <PublishingType action={massToolTypes.PUBLISH} />
        </div>
        <TooltipLabel
          labelProps={{ labelText: formatMessage(messages.releaseDates) }}
          tooltipProps={{ tooltip: formatMessage(messages.releaseDatesTooltip) }}
        />
        <div className='row'>
          <div className='col-lg-9'>
            <DatePicker
              type='from'
              name='publishAt'
              futureDays
              inputProps={{
                disabled: publishingType === timingTypes.NOW
              }}
              className={styles.datePicker}
              // select the lower maximal publish date
              filterDate={Math.min(publishFilterDate, expireOn)}
              label={formatMessage(messages.releaseDay)}
              labelRequired
            />
          </div>
          <div className='col-lg-9'>
            <DatePicker
              type='to'
              name='archiveAt'
              futureDays
              filterDate={archiveFilterDate}
              label={formatMessage(messages.archiveDay)}
              className={styles.datePicker}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

MassPublish.propTypes = {
  activeIndex: PropTypes.number,
  approvedCount: PropTypes.number,
  notApprovedCount: PropTypes.number,
  updateUI: PropTypes.func,
  closeDialog: PropTypes.func.isRequired,
  publishFilterDate: PropTypes.number,
  expireOn: PropTypes.number,
  archiveFilterDate: PropTypes.number,
  publishingType: PropTypes.string,
  timingType: PropTypes.string,
  publishAt: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    initialState: {
      activeIndex: 0
    }
  }),
  formValues({ timingType: 'timingType', publishAt: 'publishAt' }),
  connect((state, ownProps) => {
    return {
      approvedCount: getReviewListCount(state, { ...ownProps, approved: true }),
      notApprovedCount: getReviewListCount(state, ownProps),
      publishingType: getPublishingType(state, ownProps),
      archiveFilterDate: getArchiveFilterDate(state, ownProps),
      publishFilterDate: getPublishFilterDate(state, ownProps),
      expireOn: getExpireOn(state),
      timingType: ownProps.timingType,
      publishAt: ownProps.publishAt
    }
  })
)(MassPublish)
