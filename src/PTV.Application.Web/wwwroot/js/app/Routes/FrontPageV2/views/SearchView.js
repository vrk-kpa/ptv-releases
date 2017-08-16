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
import { Button, SimpleTable } from 'sema-ui-components'
import FrontPageSearchForm from '../components/FrontPageSearchForm'
import {
  getDomainSearchRows,
  getSearchDomain,
  getIsFrontPageSearching,
  getDomainSearchPageNumber,
  getDomainSearchPageSize,
  getDomainShowMoreAvailable,
  getDomainSearchTotal
} from '../selectors'
import { connect } from 'react-redux'
import { getSearchDomainColumns } from '../util/columnsDefinitions/columnsDefinitions'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'react-intl'
import { PTVPreloader } from 'Components'
import Divider from 'Components/Divider/Divider'
import {
  fetchMoreServices,
  fetchMoreChannels,
  fetchMoreGeneralDescriptions,
  fetchMoreOrganizations
} from 'Routes/FrontPageV2/actions'
import { isDirty, submit } from 'redux-form/immutable'
import ui from 'Utils/redux-ui'
import { sortDirectionTypes } from '../../../Containers/Common/Enums'
import { Map } from 'immutable'

const messages = defineMessages({
  loadMore: {
    id: 'Components.Buttons.ShowMoreButton',
    defaultMessage: 'Näytä lisää'
  }
})

const loadMore = () => store => {
  const searchDomain = getSearchDomain(store.getState())
  switch (searchDomain) {
    case 'services':
      fetchMoreServices(store)
      break
    case 'eChannel':
    case 'webPage':
    case 'printableForm':
    case 'phone':
    case 'serviceLocation':
    case 'channels':
      fetchMoreChannels(store)
      break
    case 'generalDescriptions':
      fetchMoreGeneralDescriptions(store)
      break
    case 'organizations':
      fetchMoreOrganizations(store)
      break
  }
}

const SearchView = ({
  rows = [],
  searchDomain,
  intl: { formatMessage },
  isSearching,
  loadMore,
  isShowMoreAvailable,
  page,
  pageSize,
  total,
  dirty,
  ui,
  submit,
  updateUI
}) => {
  const isEmpty = rows.length === 0
  const isLoadingMore = isSearching && !isEmpty
  const isLoadedWithResults = !isSearching && !isEmpty
  const isInitialSearch = isSearching && isEmpty

  const onSort = (column) => {
    const sortDirection = ui.sorting.getIn([searchDomain, 'column']) === column.property ? ui.sorting.getIn([searchDomain, 'sortDirection']) || sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const sorting = ui.sorting.mergeIn([searchDomain], { column: column.property, sortDirection: sortDirection === sortDirectionTypes.DESC && sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    updateUI('sorting', sorting)
    submit('frontPageSearch')
  }

  return (
    <SimpleTable columns={getSearchDomainColumns(formatMessage, searchDomain, null, onSort)}>
      <SimpleTable.Extra className=''>
        <FrontPageSearchForm
        />
        {isLoadedWithResults && <Divider componentClass='no-margin' />}
      </SimpleTable.Extra>
      {isLoadedWithResults && <SimpleTable.Header />}
      {isInitialSearch
        ? <PTVPreloader />
        : <SimpleTable.Body rows={rows} rowKey='id' />}
      {isShowMoreAvailable && !isInitialSearch &&
        <SimpleTable.Extra bottom>
          {isLoadingMore
            ? <PTVPreloader />
            : <div className='centered'>
              {!dirty &&
              <Button
                children={`${formatMessage(messages.loadMore)}`}
                onClick={loadMore}
                small secondary
                />}
            </div>}
        </SimpleTable.Extra>}
    </SimpleTable>
  )
}
SearchView.propTypes = {
  rows: PropTypes.array,
  searchDomain: PropTypes.string.isRequired,
  intl: PropTypes.any.isRequired,
  isSearching: PropTypes.bool.isRequired,
  isShowMoreAvailable: PropTypes.bool.isRequired,
  loadMore: PropTypes.func.isRequired,
  page: PropTypes.number.isRequired,
  pageSize: PropTypes.number.isRequired,
  total: PropTypes.number.isRequired
}

export default compose(
  connect(
    state => ({
      rows: getDomainSearchRows(state),
      searchDomain: getSearchDomain(state),
      isSearching: getIsFrontPageSearching(state),
      page: getDomainSearchPageNumber(state),
      pageSize: getDomainSearchPageSize(state),
      isShowMoreAvailable: getDomainShowMoreAvailable(state),
      total: getDomainSearchTotal(state),
      dirty: isDirty('frontPageSearch')(state)
    }),
    {
      loadMore,
      submit
    }
  ),
  ui({
    key: 'uiData',
    state: {
      sorting: Map()
    }
  }),
  injectIntl
)(SearchView)
