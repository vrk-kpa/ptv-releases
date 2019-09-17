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
import { getMainEntitiesCount } from 'Routes/Connections/selectors'
import {
  getConnectionsEntity,
  getConnectionsMainEntity
} from 'selectors/selections'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import Tooltip from 'appComponents/Tooltip'
import { Label } from 'sema-ui-components'
import SelectionSwitcher from 'Routes/Connections/components/SelectionSwitcher'
import styles from './styles.scss'

const messages = defineMessages({
  headerTitle: {
    id: 'Routes.Connections.Components.ConnectionsHeader.HeaderTitle.Title',
    defaultMessage: 'Työpöytä ({count})'
  },
  headerTooltip: {
    id: 'Routes.Connections.Components.ConnectionsHeader.HeaderTitle.Tooltip',
    defaultMessage: 'Connections header title tooltip'
  }
})

const ConnectionsHeader = ({
  intl: { formatMessage },
  selectedMainEntitiesCount,
  selectedEntity,
  mainEntity
}) => {
  return (
    <div className={styles.connectionsTitle}>
      <div>
        <Label
          labelText={formatMessage(messages.headerTitle, { count: selectedMainEntitiesCount })}
          labelPosition='top'>
          <Tooltip tooltip={formatMessage(messages.headerTooltip)} />
        </Label>
      </div>
      {selectedEntity &&
        <SelectionSwitcher disabled={!mainEntity} size='w160' />
      }
    </div>
  )
}
ConnectionsHeader.propTypes = {
  intl: intlShape,
  selectedMainEntitiesCount: PropTypes.number,
  selectedEntity: PropTypes.oneOf(['channels', 'services']),
  mainEntity: PropTypes.oneOf(['channels', 'services'])
}

export default compose(
  injectIntl,
  connect((state) => ({
    selectedMainEntitiesCount: getMainEntitiesCount(state),
    selectedEntity: getConnectionsEntity(state),
    mainEntity: getConnectionsMainEntity(state)
  }))
)(ConnectionsHeader)
