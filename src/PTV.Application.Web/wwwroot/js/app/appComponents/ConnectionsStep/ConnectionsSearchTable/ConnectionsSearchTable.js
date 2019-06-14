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
import { injectIntl, intlShape } from 'util/react-intl'
import { SimpleTable, Spinner, Button } from 'sema-ui-components'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import NameCell from 'appComponents/Cells/NameCell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import { PTVIcon } from 'Components'
import ConnectionsSearchForm from 'appComponents/ConnectionsStep/ConnectionsSearchForm'
import AddConnectionButton from 'appComponents/ConnectionsStep/AddConnectionButton'
import ConnectionTags from '../ConnectionTags'
import {
  getConnectionSearchIsMoreAvailable,
  getConnectionSearchIsFetching,
  getConnectionSearchReturnedPageNumber,
  getConnectionSearchEntities
} from '../selectors'
import { loadConnectableEntities } from '../actions'
import styles from '../styles.scss'
import cx from 'classnames'
import { entityConcreteTypesEnum, entityTypesEnum } from 'enums'
import { buttonMessages } from 'Routes/messages'

const channelsColumnsDefinition = (intl, previewOnClick) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: intl.formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: intl.formatMessage(CellHeaders.name)
      },
      cell: {
        formatters: [
          (name, { rowData: { id, channelType, isSuggested, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, channelType)
            }
            const tableCellInlineClass = cx(
              styles.tableCell,
              styles.noPadding,
              styles.inline
            )
            return <div className={tableCellInlineClass}>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <div>
                <NameCell {...rest} />
                <ConnectionTags
                  isSuggestedChannel={isSuggested}
                  entityId={id}
                  location='search'
                />
              </div>
            </div>
          }
        ]
      }
    },
    {
      property: 'channelTypeId',
      header: {
        label: intl.formatMessage(CellHeaders.channelType)
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
        label: intl.formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => <OrganizationCell organizationId={organizationId} />
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
              intl={intl}
            />
          )
        ]
      }
    }
  ]
}

const servicesColumnsDefinition = (intl, previewOnClick) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: intl.formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: intl.formatMessage(CellHeaders.name)
      },
      cell: {
        formatters: [
          (name, { rowData: { id, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, entityConcreteTypesEnum.SERVICE)
            }
            const tableCellInlineClass = cx(
              styles.tableCell,
              styles.noPadding,
              styles.inline
            )
            return <div className={tableCellInlineClass}>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <div>
                <NameCell {...rest} />
                <ConnectionTags entityId={id} location='search' />
              </div>
            </div>
          }
        ]
      }
    },
    {
      property: 'serviceType',
      header: {
        label: intl.formatMessage(CellHeaders.serviceType)
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
        label: intl.formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => <OrganizationCell organizationId={organizationId} />
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
  intl: { formatMessage },
  columns,
  rows,
  isFetching,
  pageNumber,
  isMoreAvailable,
  loadMore,
  ...rest
}) => {
  const handleLoadMore = () => {
    loadMore({ searchMode, loadMoreEntities: true })
  }
  const showSpinner = isFetching && !pageNumber
  const showMoreSpinner = isFetching && !!pageNumber
  const showData = (!isFetching || isFetching && !!pageNumber) && rows.length > 0
  return (
    <div>
      <SimpleTable
        columns={columns}
        scrollable
        columnWidths={['12%', '30%', '20%', '25%', '13%']}
      >
        <SimpleTable.Extra>
          <ConnectionsSearchForm searchMode={searchMode} />
        </SimpleTable.Extra>

        { showSpinner && <tbody><tr><td><div className={styles.spinner}><Spinner /></div></td></tr></tbody> }
        { showData && <SimpleTable.Header /> }
        { showData &&
          <SimpleTable.Body
            rowKey='id'
            rows={rows}
          />
        }
        {isMoreAvailable &&
          <SimpleTable.Extra bottom>
            <Button
              children={showMoreSpinner && <Spinner /> || formatMessage(buttonMessages.showMore)}
              onClick={handleLoadMore}
              disabled={showMoreSpinner}
              small secondary
              className={styles.showMoreButton}
            />
          </SimpleTable.Extra>
        }
      </SimpleTable>
    </div>
  )
}
ConnectionsSearchTable.propTypes = {
  searchMode: PropTypes.oneOf([
    'channels',
    'services',
    'generalDescriptions',
    'serviceCollections'
  ]),
  columns: PropTypes.array,
  rows: PropTypes.array,
  isFetching: PropTypes.bool,
  pageNumber: PropTypes.number,
  isMoreAvailable: PropTypes.bool,
  intl: intlShape,
  loadMore: PropTypes.func
}

const loadMore = props => ({ dispatch, getState }) => {
  dispatch(loadConnectableEntities(getState(), props))
}

export default compose(
  injectIntl,
  connect((state, { searchMode, previewOnClick, intl }) => {
    const columns = {
      'channels': servicesColumnsDefinition(intl, previewOnClick),
      'serviceCollections': servicesColumnsDefinition(intl, previewOnClick),
      'services': channelsColumnsDefinition(intl, previewOnClick),
      'generalDescriptions': channelsColumnsDefinition(intl, previewOnClick)
    }[searchMode]
    const entryKey = {
      [entityTypesEnum.SERVICES]: 'connectionsChannelSearch',
      [entityTypesEnum.GENERALDESCRIPTIONS]: 'connectionsChannelSearch',
      [entityTypesEnum.CHANNELS]: 'connectionsServiceSearch',
      [entityTypesEnum.SERVICECOLLECTIONS]: 'connectionsServiceSearch'
    }[searchMode]
    const entityType = {
      [entityTypesEnum.SERVICES]: entityTypesEnum.CHANNELS,
      [entityTypesEnum.GENERALDESCRIPTIONS]: entityTypesEnum.CHANNELS,
      [entityTypesEnum.CHANNELS]: entityTypesEnum.SERVICES,
      [entityTypesEnum.SERVICECOLLECTIONS]: entityTypesEnum.SERVICES
    }[searchMode]
    return {
      columns,
      rows: getConnectionSearchEntities(state, { entityType, entryKey }),
      isFetching: getConnectionSearchIsFetching(state, { entryKey }),
      pageNumber: getConnectionSearchReturnedPageNumber(state, { entryKey }),
      isMoreAvailable: getConnectionSearchIsMoreAvailable(state, { entryKey })
    }
  }, {
    loadMore
  })
)(ConnectionsSearchTable)
