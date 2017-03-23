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
import React, { PropTypes } from 'react'
import { getPublishingStatuses } from 'Containers/Common/Selectors'
import { injectIntl, defineMessages } from 'react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { camelCase } from 'lodash'
import './styles.scss'

const messages = defineMessages({
  draft: {
    id: 'Components.EntityVersion.PublishingStatus.Draft.Title',
    defaultMessage: 'Luonnos'
  },
  published: {
    id: 'Components.EntityVersion.PublishingStatus.Published.Title',
    defaultMessage: 'Julkaistu'
  },
  oldPublished: {    
    id: 'Components.EntityVersion.PublishingStatus.Deleted.Title',
    defaultMessage: 'Arkistoitu'
  },
  deleted: {
    id: 'Components.EntityVersion.PublishingStatus.Deleted.Title',
    defaultMessage: 'Arkistoitu'
  },
  modified: {
    id: 'Components.EntityVersion.PublishingStatus.Modified.Title',
    defaultMessage: 'Muokkauksessa'
  },
  noStatus: {
    id: 'Components.EntityVersion.PublishingStatus.NoStatus.Title',
    defaultMessage: 'NoStatus'
  },
  versionTitle: {
    id: 'FrontPage.Name.Version.Cell.Title',
    defaultMessage: '{publishingStatus} : Versio {version}'
  }
})

const EntityVersion = ({
  version,
  publishingStatus,
  intl: { formatMessage }
}) => {
  publishingStatus = publishingStatus ? camelCase(publishingStatus) : 'noStatus'
  const versionTitle = version ? version.value : 'not'
  return (
    <div className='entity-version'>
      {formatMessage(
        messages.versionTitle,
        {
          publishingStatus: formatMessage(messages[publishingStatus] || messages['noStatus']),
          version: versionTitle
        })
      }
    </div>
  )
}

EntityVersion.propTypes = {
  publishingStatus: PropTypes.string.isRequired,
  version: PropTypes.object,
  intl: PropTypes.object
}

export default compose(
  connect(
    (state, { publishingStatusId }) => ({
      publishingStatus: getPublishingStatuses(state).getIn([publishingStatusId, 'code'])
    })
  ),
  injectIntl
)(EntityVersion)
