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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getPublishOn } from '../selectors'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { injectIntl, intlShape } from 'util/react-intl'
import { entityTypesEnum } from 'enums'
import NotificationDialog from './NotificationDialog'
import moment from 'moment'
import messages from '../messages'

const TimedPublishInfoDialog = compose(
  injectIntl,
  connect(state => ({
    entityType: getSelectedEntityType(state),
    publishOn: getPublishOn(state)
  }))
)(({ publishOn, intl: { formatMessage }, onCrossClick, entityType }) => {
  const timedPublishDescription = {
    [entityTypesEnum.SERVICES]: messages.serviceTimedPublishDescription,
    [entityTypesEnum.CHANNELS]: messages.channelTimedPublishDescription,
    [entityTypesEnum.GENERALDESCRIPTIONS]: messages.generalDescriptionTimedPublishDescription,
    [entityTypesEnum.ORGANIZATIONS]: messages.organizationTimedPublishDescription,
    [entityTypesEnum.SERVICECOLLECTIONS]: messages.serviceCollectionTimedPublishDescription
  }[entityType]

  return (
    <NotificationDialog
      type='info'
      asAlert
      title={formatMessage(messages.timedPublishTitle)}
      description={formatMessage(timedPublishDescription, { publishOn: moment(publishOn).format('DD.MM.YYYY') })}
    />
  )
})

TimedPublishInfoDialog.propTypes = {
  publishOn: PropTypes.number,
  onCrossClick: PropTypes.func,
  intl: intlShape,
  entityType: PropTypes.string
}

export default TimedPublishInfoDialog
