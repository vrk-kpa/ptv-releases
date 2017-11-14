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
import { getApiCalls } from 'selectors/base'
import { injectIntl, defineMessages } from 'react-intl'
import { createSelector } from 'reselect'
import { EntitySelectors } from 'selectors'
import { SimpleTable, Spinner } from 'sema-ui-components'
import { NameCell, LanguageBarCell, ServiceTypeCell } from 'appComponents'
import ConnectionsSearchForm from 'appComponents/ConnectionsStep/ConnectionsSearchForm'
import AddConnectionButton from 'appComponents/ConnectionsStep/AddConnectionButton'
import ConnectionTags from '../ConnectionTags'
import {
  OrganizationCell,
  ChannelTypeCell
} from 'Routes/FrontPage/routes/Search/components'
import { List, Map } from 'immutable'
import {
  getChannelConnectionSearchIsFetching,
  getServiceConnectionSearchIsFetching
} from './selectors'
import styles from './styles.scss'
import { entityConcreteTypesEnum } from 'enums'

const messages = defineMessages({
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  languages: {
    id: 'AppComponents.RadioButtonCell.Header.Languages',
    defaultMessage: 'Kieli ja tila'
  },
  edited: {
    id: 'FrontPage.Shared.Search.Header.Edited',
    defaultMessage: 'Muokattu'
  },
  modified: {
    id: 'FrontPage.Shared.Search.Header.Modifier',
    defaultMessage: 'Muokkaaja'
  },
  channelType: {
    id: 'appComponents.CellHeaders.ChannelType',
    defaultMessage: 'Kanavatyyppi'
  },
  serviceType: {
    id: 'appComponents.CellHeaders.ServiceType',
    defaultMessage: 'Palvelutyyppi'
  },
  channelSearchBoxOrganizationTitle: {
    id: 'Containers.ServiceAndChannels.ChannelSearch.SearchBox.Organization.Title',
    defaultMessage: 'Organisaatio'
  }
})

const getConnectionServiceSearch = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('connectionsServiceSearch') || Map()
)
const getConnectionChannelSearch = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('connectionsChannelSearch') || Map()
)
const getConnectionServiceSearchIds = createSelector(
  getConnectionServiceSearch,
  connectionsSearch => {
    return (
      connectionsSearch.hasIn(['result', 'data']) &&
      connectionsSearch.getIn(['result', 'data'])
    ) || List()
  }
)
const getConnectionChannelSearchIds = createSelector(
  getConnectionChannelSearch,
  connectionsSearch => {
    return (
      connectionsSearch.hasIn(['result', 'data']) &&
      connectionsSearch.getIn(['result', 'data'])
    ) || List()
  }
)

export const getConnectionChannelSearchCount = createSelector(
  getConnectionChannelSearchIds,
  ids => ids && ids.count() || 0
)

export const getConnectionChannelSearchTotal = createSelector(
  getConnectionChannelSearch,
  connectionsSearch => {
    return (
      connectionsSearch.hasIn(['result', 'count']) &&
      connectionsSearch.getIn(['result', 'count'])
      ) || 0
  }
)

export const getIsConnectionChannelPageSearching = createSelector(
  [getConnectionChannelSearch],
  (connectionPageSearch) => connectionPageSearch.get('isFetching') || false
)

export const getConnectionServiceSearchCount = createSelector(
  getConnectionServiceSearchIds,
  ids => ids && ids.count() || 0
)

export const getConnectionServiceSearchTotal = createSelector(
  getConnectionServiceSearch,
  connectionsSearch => {
    return (
      connectionsSearch.hasIn(['result', 'count']) &&
      connectionsSearch.getIn(['result', 'count'])
      ) || 0
  }
)

export const getIsConnectionServicePageSearching = createSelector(
  [getConnectionServiceSearch],
  (connectionPageSearch) => connectionPageSearch.get('isFetching') || false
)

const getConnectionsServiceSearchResult = createSelector(
  [getConnectionServiceSearchIds, EntitySelectors.services.getEntities],
  (ids, serviceEntities) => {
    const result = ids.map(id => serviceEntities.get(id)).toJS()
    return result
  }
)
const getConnectionsChannelSearchResult = createSelector(
  [getConnectionChannelSearchIds, EntitySelectors.channels.getEntities],
  (ids, channelEntities) => {
    return ids.map(id => channelEntities.get(id)).toJS()
  }
)

const channelsColumnsDefinition = (formatMessage, previewOnClick) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(messages.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.name)
      },
      cell: {
        formatters: [
          (name, { rowData: { id, channelType, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, channelType)
            }
            return <div>
              <NameCell viewIcon viewOnClick={handleOnClick} id={id} {...rest} />
              <ConnectionTags entityId={id} location='search' />
            </div>
          }
        ]
      }
    },
    {
      property: 'channelTypeId',
      header: {
        label: formatMessage(messages.channelType)
      },
      cell: {
        formatters: [
          channelTypeId => <ChannelTypeCell channelTypeId={channelTypeId} />
        ]
      }
    },
    {
      property: 'organizationId',
      header: {
        label: formatMessage(messages.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => <OrganizationCell OrganizationIds={[organizationId]} />
        ]
      }
    },
    {
      cell: {
        formatters: [
          (_, { rowIndex, rowData }) => (
            <AddConnectionButton
              rowIndex={rowIndex}
              rowData={rowData}
            />
          )
        ]
      }
    }
  ]
}

const servicesColumnsDefinition = (formatMessage, previewOnClick) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(messages.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.name)
      },
      cell: {
        formatters: [
          (name, { rowData: { id, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, entityConcreteTypesEnum.SERVICE)
            }
            return <NameCell viewIcon viewOnClick={handleOnClick} id={id} {...rest} />
          }
          // (name, { rowData }) => <NameCell viewIcon viewOnClick={previewOnClick}{...rowData} />
        ]
      }
    },
    {
      property: 'serviceTypeId',
      header: {
        label: formatMessage(messages.serviceType)
      },
      cell: {
        formatters: [
          serviceTypeId => <ServiceTypeCell serviceTypeId={serviceTypeId} />
        ]
      }
    },
    {
      property: 'organizationId',
      header: {
        label: formatMessage(messages.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => <OrganizationCell OrganizationIds={[organizationId]} />
        ]
      }
    },
    {
      cell: {
        formatters: [
          (_, { rowIndex, rowData }) => (
            <AddConnectionButton
              rowIndex={rowIndex}
              rowData={rowData}
            />
          )
        ]
      }
    }
  ]
}

const ConnectionsSearchTable = ({
  searchMode,
  formatMessage,
  columns,
  rows,
  isFetching
}) => {
  return (
    <SimpleTable
      columns={columns}
      scrollable
      columnWidths={['12%', '30%', '20%', '25%', '13%']}
    >
      <SimpleTable.Extra>
        <ConnectionsSearchForm searchMode={searchMode} />
      </SimpleTable.Extra>

      { isFetching && <tbody><tr><td><div className={styles.spinner}><Spinner /></div></td></tr></tbody> }
      { !isFetching && rows.length > 0 && <SimpleTable.Header /> }
      { !isFetching && rows.length > 0 &&
        <SimpleTable.Body
          rowKey='id'
          rows={rows}
        />
      }
    </SimpleTable>
  )
}
ConnectionsSearchTable.propTypes = {
  searchMode: PropTypes.oneOf([
    'channels',
    'services',
    'generalDescriptions'
  ]),
  formatMessage: PropTypes.func,
  columns: PropTypes.array,
  rows: PropTypes.array,
  isFetching: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, { searchMode, previewOnClick, intl: { formatMessage } }) => {
    const columns = {
      'channels': servicesColumnsDefinition(formatMessage, previewOnClick),
      'services': channelsColumnsDefinition(formatMessage, previewOnClick),
      'generalDescriptions': channelsColumnsDefinition(formatMessage, previewOnClick)
    }[searchMode]
    const rows = {
      'channels': getConnectionsServiceSearchResult(state),
      'services': getConnectionsChannelSearchResult(state),
      'generalDescriptions': getConnectionsChannelSearchResult(state)
    }[searchMode]
    const isFetching = {
      'channels': getServiceConnectionSearchIsFetching(state),
      'services': getChannelConnectionSearchIsFetching(state),
      'generalDescriptions': getChannelConnectionSearchIsFetching(state)
    }[searchMode]
    return { columns, rows, isFetching }
  })
)(ConnectionsSearchTable)
