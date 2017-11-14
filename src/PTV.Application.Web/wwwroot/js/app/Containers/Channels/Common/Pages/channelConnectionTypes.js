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
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'react-intl'

// components
import { PTVAddItem, PTVLabel } from 'Components'

// localized components
import { LocalizedRadioGroup } from '../../../Common/localizedData'

// actions
import * as channelActions from '../../Common/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as CommonChannelSelectors from '../Selectors'
import * as CommonSelectors from '../../../Common/Selectors'

export const messages = defineMessages({
  connectionTypeTitle: {
    id: 'Containers.Channels.ConnectionType.Header.Title',
    defaultMessage: 'Asiointikanavan yhteiskäyttöisyystieto'
  },
  warning: {
    id: 'Containers.Channels.ConnectionType.Warning.Message',
    defaultMessage: 'Olet muuttamassa yhteiskäyttöisen kanavan vain oman organisaatiosi käyttöön.'
  },
  warningTooltip: {
    id: 'Containers.Channels.ConnectionType.Warning.Tooltip',
    defaultMessage: 'Olet muuttamassa yhteiskäyttöisen kanavan vain oman organisaatiosi käyttöön.'
  }
})

const channelConnectionTypes = ({
  readOnly,
  language,
  translationMode,
  channelId,
  connectionTypeId,
  connectionTypes,
  actions,
  showNotification,
  intl }) => {
  const onInputChange = value => {
    actions.onChannelInputChange('connectionTypeId', channelId, value, false, language)
  }

  const renderItem = () => {
    return (
      <div>
        { showNotification &&
          <PTVLabel labelClass='has-error' tooltip={intl.formatMessage(messages.warningTooltip)} >
            {intl.formatMessage(messages.warning)}
          </PTVLabel>
        }
        <LocalizedRadioGroup
          language={language}
          name='channelConnectionTypes'
          value={connectionTypeId}
          items={connectionTypes}
          onChange={onInputChange}
          verticalLayout
          readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
        />
      </div>
    )
  }

  return (
    <PTVAddItem
      readOnly={readOnly && translationMode === 'none'}
      renderItemContent={renderItem}
      messages={{ 'label': intl.formatMessage(messages.connectionTypeTitle) }}
      collapsible={translationMode === 'none'}
      multiple={translationMode === 'none' || translationMode === 'edit'}
    />
  )
}

const mapStateToProps = (state, ownProps) => {
  return {
    channelId: CommonChannelSelectors.getChannelId(state, ownProps),
    connectionTypeId: CommonChannelSelectors.getConnectionTypeId(state, ownProps),
    connectionTypes: CommonSelectors.getServiceChannelConnectionTypesObjectArray(state),
    showNotification: !ownProps.readOnly && CommonChannelSelectors.getIsConnectionTypeCommonWarningVisible(state, ownProps)
  }
}

const actions = [
  channelActions
]

export default compose(
  injectIntl,
  connect(mapStateToProps, mapDispatchToProps(actions))
)(channelConnectionTypes)
