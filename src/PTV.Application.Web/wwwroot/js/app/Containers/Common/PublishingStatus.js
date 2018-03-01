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
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import PTVLabel from '../../Components/PTVLabel'
import { getPublishingStatuses } from './Selectors'

const messages = defineMessages({
  serviceStatusTitle: {
    id: 'Containers.Common.PublishingStatus.ServiceStatus.Title',
    defaultMessage: 'Palvelun tila: '
  },
  channelStatusTitle: {
    id: 'Containers.Common.PublishingStatus.ChannelStatus.Title',
    defaultMessage: 'Tila: '
  },
  statusDraft: {
    id: 'Containers.Common.PublishingStatus.Status.Draft',
    defaultMessage: 'Luonnos'
  },
  statusPublished: {
    id: 'Containers.Common.PublishingStatus.Status.Published',
    defaultMessage: 'Julkaistu'
  },
  statusModified: {
    id: 'Containers.Common.PublishingStatus.Status.Modified',
    defaultMessage: 'Muokattu'
  },
  statusDeleted: {
    id: 'Containers.Common.PublishingStatus.Status.Deleted',
    defaultMessage: 'Deleted'
  }
})

const getStatus = (
  formatMessage,
  publishingStatus
) => {
  let status = ''
  if (publishingStatus) {
    switch (publishingStatus.get('type')) {
      case 0: return formatMessage(messages.statusDraft)
      case 1: return formatMessage(messages.statusPublished)
      case 2: return formatMessage(messages.statusDeleted)
      case 3: return formatMessage(messages.statusModified)
      case 4: return formatMessage(messages.statusDeleted)
      default : ''
    }
  }

  return status
}

const PublishingStatus = ({
  intl,
  status,
  pageType
}) => {
  const { formatMessage } = intl
  const statusTitle = pageType === 'service'
    ? formatMessage(messages.serviceStatusTitle)
    : formatMessage(messages.channelStatusTitle)

  if (!status) {
    return null
  }

  return (
    <div className='row'>
      <div className='col-xs-12 col-md-6'>
        <PTVLabel><strong>{statusTitle}</strong> {getStatus(formatMessage, status)}</PTVLabel>
      </div>
    </div>
  )
}

PublishingStatus.propTypes = {
  intl: intlShape.isRequired,
  publishingStatus: PropTypes.any
}

function mapStateToProps (state, ownProps) {
  return {
    status: getPublishingStatuses(state).get(ownProps.publishingStatus)
  }
}

export default connect(mapStateToProps)(injectIntl(PublishingStatus))
