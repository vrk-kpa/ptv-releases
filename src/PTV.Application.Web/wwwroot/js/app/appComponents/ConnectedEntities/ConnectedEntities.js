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
import { getConnectedEntities } from './selectors'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'

// Components
import { Label, SimpleTable } from 'sema-ui-components'
import {
  channelColumnsDefinition,
  serviceColumnsDefinition,
  serviceCollectionColumnsDefinition
} from './ColumnDefinitions'

const messages = defineMessages({
  servicesConnectedTitle: {
    id: 'Containers.Services.AddService.Step4.Header.Title',
    defaultMessage: 'Liitetyt asiointikanavat'
  },
  channelsConnectedTitle: {
    id: 'Containers.Channel.Common.ChannelServiceStep.Title',
    defaultMessage: 'Liitetyt pavelut'
  },
  generalDescriptionsConnectedTitle: {
    id: 'AppComponents.ConnectionStep.GDProposedChannelConnections.Title',
    defaultMessage: 'Ehdotetut asiointikanavat'
  },
  serviceCollectionsConnectedTitle: {
    id : 'Routes.Service.Components.ServiceCollections.Title',
    defaultMessage: 'Liitokset palvelukokonaisuuksiin'
  }
})

const ConnectedEntities = ({
  type,
  rows,
  columns,
  intl: { formatMessage },
  omitLabel,
  ...rest
}) => {
  const titleMessageKey = `${type}ConnectedTitle`
  return (
    rows.length > 0 && <div className='form-row'>
      {!omitLabel && <Label labelText={formatMessage(messages[titleMessageKey])} />}
      <SimpleTable
        scrollable
        columnWidths={['20%', '30%', '20%', '30%']}
        maxHeight={300}
        columns={columns}>
        <SimpleTable.Header />
        <SimpleTable.Body
          rowKey='id'
          rows={rows}
        />
      </SimpleTable>
    </div>
  )
}

ConnectedEntities.propTypes = {
  type: PropTypes.string,
  rows: PropTypes.array,
  columns: PropTypes.array,
  intl: intlShape,
  omitLabel: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, { intl: { formatMessage }, rows, withNavigation, ...rest }) => {
    const columns = {
      'channels': serviceColumnsDefinition(formatMessage, rest.id, rest.type, withNavigation),
      'services': channelColumnsDefinition(formatMessage, rest.id, rest.type, withNavigation),
      'generalDescriptions': channelColumnsDefinition(formatMessage, rest.id, rest.type, withNavigation),
      'serviceCollections': serviceCollectionColumnsDefinition(formatMessage, rest.id, rest.type, withNavigation)
    }[rest.type]
    return {
      rows: rows || getConnectedEntities(state, rest),
      columns
    }
  })
)(ConnectedEntities)
