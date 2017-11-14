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
import ConnectionsSearchTable from './ConnectionsSearchTable'
import ConnectionsForm from './ConnectionsForm'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'Routes/actions'
import { getKey, formAllTypes } from 'enums'
import { Label } from 'sema-ui-components'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { isConnectionsReadOnly } from 'appComponents/ConnectionsStep/ConnectionsForm/selectors'

const messages = defineMessages({
  channelsConnectedTitle: {
    id: 'Containers.Services.AddService.Step4.Header.Title',
    defaultMessage: 'Liitetyt asiointikanavat'
  },
  channelsAvailableTitle: {
    id: 'Containers.Services.AddService.Step4.ConnectChannels.RelationLink.Add.Title',
    defaultMessage: 'Lisää asiointikanavia'
  },
  servicesConnectedTitle: {
    id: 'Containers.Channel.Common.ChannelServiceStep.Title',
    defaultMessage: 'Liitetyt pavelut'
  },
  servicesAvailableTitle: {
    id: 'Containers.Channel.Common.ChannelServiceStep.Add.Button',
    defaultMessage: 'Lisää palveluja'
  },
  gdConnectedTitle: {
    id: 'AppComponents.ConnectionStep.GDProposedChannelConnections.Title',
    defaultMessage: 'Ehdotetut asiointikanavat'
  },
  gdAvailableTitle: {
    id: 'Containers.Services.AddService.Step4.ConnectChannels.RelationLink.Add.Title',
    defaultMessage: 'Lisää asiointikanavia'
  }
})

const ConnectionsStep = ({
  searchMode,
  mergeInUIState,
  loadPreviewEntity,
  notificationForm,
  isConnectionsReadOnly,
  intl: { formatMessage }
}) => {
  const handlePreviewOnClick = (id, entityType) => {
    const formName = entityType && getKey(formAllTypes, entityType.toLowerCase())
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true
      }
    })
    loadPreviewEntity(id, formName)
  }
  const type = {
    channels: 'services',
    services: 'channels',
    generalDescriptions: 'gd'
  }[searchMode]
  return (
    <div>
      {!isConnectionsReadOnly &&
      <div className='form-row'>
        <Label
          labelPosition='top'
          labelText={formatMessage(messages[`${type}AvailableTitle`])}
        />
        <ConnectionsSearchTable
          searchMode={searchMode}
          previewOnClick={handlePreviewOnClick}
        />
      </div>}
      <div className='form-row'>
        <ConnectionsForm
          searchMode={searchMode}
          notificationForm={notificationForm}
          label={messages[`${type}ConnectedTitle`]}
        />
      </div>
    </div>
  )
}
ConnectionsStep.propTypes = {
  searchMode: PropTypes.oneOf(['channels', 'services', 'generalDescriptions']),
  loadPreviewEntity: PropTypes.func,
  mergeInUIState: PropTypes.func,
  intl: intlShape,
  isConnectionsReadOnly: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state) => ({
    isConnectionsReadOnly: isConnectionsReadOnly(state)
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  })
)(ConnectionsStep)
