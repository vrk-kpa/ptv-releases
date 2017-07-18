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
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Link, browserHistory } from 'react-router'
import {
  getEditableLastPublished,
  getEditableEntityId,
  getPublsihingStatusIdOfEntity,
  getPublishingStatusType,
  getEditableLastModified,
  getIsFetchingOfAnyStep } from './Selectors'

// components
import { PTVIcon, PTVLabel } from '../../Components'
import { Alert } from 'sema-ui-components'

// actions
import { onRecordSelect } from '../../Routes/FrontPageV2/actions'
import { resetAllMessage } from './Actions'

// types
import { publishingStatuses } from './Enums'

const messages = defineMessages({
  LinkModifiedTitle: {
    id: 'Containers.Common.VersionLinkContainer.Modified.Title',
    defaultMessage: 'Tästä sisällöstä on olemassa muokattu versio.'
  },
  LinkModifiedLink: {
    id: 'Containers.Common.VersionLinkContainer.Modified.Link',
    defaultMessage: 'Siirry muokattuun versioon.'
  },
  LinkPublishedTitle: {
    id: 'Containers.Common.VersionLinkContainer.Published.Title',
    defaultMessage: 'Tästä sisällöstä on olemassa julkaistu versio.'
  },
  LinkPublishedLink: {
    id: 'Containers.Common.VersionLinkContainer.Published.Link',
    defaultMessage: 'Siirry julkaistuun versioon.'
  }
})

const VersionLinkContainer = ({
  intl,
  keyToState,
  linkToModified,
  lastModified,
  linkToPublished,
  lastPublished,
  location,
  onRecordSelect,
  resetAllMessage }) => {
  const goToVersion = (id, keyToState, url) => {
    onRecordSelect(id, keyToState)
    resetAllMessage()
    browserHistory.push(url)
  }
  return (
    <div>
      {linkToModified &&
      <Alert info className='valign with-padding'>
        <PTVIcon name='icon-info' />
        <PTVLabel>{intl.formatMessage(messages.LinkModifiedTitle)}</PTVLabel>
        <Link onClick={() => goToVersion(lastModified, keyToState, location.pathname)}>
          {intl.formatMessage(messages.LinkModifiedLink)}
        </Link>
      </Alert>}
      { linkToPublished &&
      <Alert info className='valign with-padding'>
        <PTVIcon name='icon-info' />
        <PTVLabel>{intl.formatMessage(messages.LinkPublishedTitle)}</PTVLabel>
        <Link onClick={() => goToVersion(lastPublished, keyToState, location.pathname)}>
          {intl.formatMessage(messages.LinkPublishedLink)}
        </Link>
      </Alert>}
    </div>
  )
}

VersionLinkContainer.propTypes = {
  intl: intlShape.isRequired,
  onRecordSelect: PropTypes.func,
  resetAllMessage: PropTypes.func,
  linkToPublished: PropTypes.bool.isRequired,
  lastPublished: PropTypes.any,
  linkToModified: PropTypes.bool.isRequired,
  lastModified: PropTypes.any,
  location: PropTypes.any,
  keyToState: PropTypes.string
}

function mapStateToProps (state, ownProps) {
  const entityId = getEditableEntityId(state, ownProps)
  const publishingStatusOfEntity = getPublsihingStatusIdOfEntity(state, { id:entityId, ...ownProps })
  const publishingstatusType = getPublishingStatusType(state, publishingStatusOfEntity)
  const lastPublished = getEditableLastPublished(state, ownProps)
  const lastModified = getEditableLastModified(state, ownProps)
  const isFetching = getIsFetchingOfAnyStep(state, ownProps)
  return {
    linkToPublished : publishingstatusType === publishingStatuses.modified && lastPublished !== null && !isFetching,
    lastPublished,
    linkToModified : publishingstatusType === publishingStatuses.published && lastModified !== null && !isFetching,
    lastModified
  }
}

export default compose(
  injectIntl,
  connect(mapStateToProps, { onRecordSelect, resetAllMessage })
)(VersionLinkContainer)
