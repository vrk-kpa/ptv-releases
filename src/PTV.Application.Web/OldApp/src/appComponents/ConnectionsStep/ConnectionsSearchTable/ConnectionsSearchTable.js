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
import { injectIntl, intlShape } from 'util/react-intl'
import ConnectionsSearchForm from 'appComponents/ConnectionsStep/ConnectionsSearchForm'
import {
  getConnectionSearchIsMoreAvailable,
  getConnectionSearchIsFetching,
  getConnectionSearchReturnedPageNumber,
  getConnectionSearchEntities,
  getServiceCollectionSearchEntities
} from '../selectors'
import { loadConnectableEntities } from '../actions'
import styles from '../styles.scss'
import { entityTypesEnum } from 'enums'
import { DataTable } from 'util/redux-form/fields'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import LoadMoreButton from 'appComponents/LoadMoreButton'
import { EntitySchemas } from 'schemas'
import {
  servicesColumnsDefinition,
  serviceOrChannelColumnsDefinition,
  channelsColumnsDefinition
} from './columns'

const ConnectionsSearchTable = ({
  searchMode,
  intl: { formatMessage },
  columns,
  rows,
  isFetching,
  pageNumber,
  isMoreAvailable,
  loadMore,
  formName,
  ...rest
}) => {
  const handleLoadMore = () => {
    loadMore({ searchMode, loadMoreEntities: true, formName })
  }
  const handleOnSort = (sort) => {
    loadMore({ searchMode, loadMoreEntities: false, sort, formName })
  }
  const showSpinner = isFetching && !pageNumber
  const showMoreSpinner = isFetching && !!pageNumber
  const showData = (!isFetching || isFetching && !!pageNumber) && rows.length > 0
  return (
    <div className={styles.formHead}>
      <ConnectionsSearchForm searchMode={searchMode} />
      {showData && (
        <DataTable
          name={searchMode}
          rows={rows}
          columnsDefinition={columns}
          scrollable
          columnWidths={['12%', '30%', '20%', '25%', '13%']}
          sortOnClick={handleOnSort}
          isLoading={showSpinner}
          borderless
          tight
        />
      )}
      <LoadMoreButton
        visible={isMoreAvailable}
        isLoadingMore={showMoreSpinner}
        onClick={handleLoadMore}
        componentClass={styles.topBorder}
      />
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
  loadMore: PropTypes.func,
  formName: PropTypes.string.isRequired
}

const loadMore = props => ({ dispatch, getState }) => {
  dispatch(loadConnectableEntities(getState(), props))
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { searchMode, previewOnClick, intl, formName }) => {
    const columns = {
      'channels': servicesColumnsDefinition(intl, previewOnClick, formName),
      'serviceCollections': serviceOrChannelColumnsDefinition(intl, previewOnClick, formName),
      'services': channelsColumnsDefinition(intl, previewOnClick, formName),
      'generalDescriptions': channelsColumnsDefinition(intl, previewOnClick, formName)
    }[searchMode]
    const entryKey = {
      [entityTypesEnum.SERVICES]: 'connectionsChannelSearch',
      [entityTypesEnum.GENERALDESCRIPTIONS]: 'connectionsChannelSearch',
      [entityTypesEnum.CHANNELS]: 'connectionsServiceSearch',
      [entityTypesEnum.SERVICECOLLECTIONS]: 'connectionsContentSearch'
    }[searchMode]
    const entityType = {
      [entityTypesEnum.SERVICES]: entityTypesEnum.CHANNELS,
      [entityTypesEnum.GENERALDESCRIPTIONS]: entityTypesEnum.CHANNELS,
      [entityTypesEnum.CHANNELS]: entityTypesEnum.SERVICES,
      [entityTypesEnum.SERVICECOLLECTIONS]: EntitySchemas.GET_SEARCH(EntitySchemas.SEARCH)
    }[searchMode]
    return {
      columns,
      rows: searchMode === entityTypesEnum.SERVICECOLLECTIONS
        ? getServiceCollectionSearchEntities(state, { entityType, entryKey })
        : getConnectionSearchEntities(state, { entityType, entryKey }),
      isFetching: getConnectionSearchIsFetching(state, { entryKey }),
      pageNumber: getConnectionSearchReturnedPageNumber(state, { entryKey }),
      isMoreAvailable: getConnectionSearchIsMoreAvailable(state, { entryKey })
    }
  }, {
    loadMore
  })
)(ConnectionsSearchTable)
