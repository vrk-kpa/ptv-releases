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
import { Button, SimpleTable, Spinner } from 'sema-ui-components'
import FrontPageSearchForm from '../FrontPageSearchForm'
import { getSearchDomainColumns } from '../../util/columnsDefinitions/columnsDefinitions'
import { defineMessages } from 'react-intl'
import { Spacer } from 'appComponents'
import { compose } from 'redux'
import withState from 'util/withState'
import { Map } from 'immutable'
import { connect } from 'react-redux'
import { submit } from 'redux-form/immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'

const messages = defineMessages({
  loadMore: {
    id: 'Components.Buttons.ShowMoreButton',
    defaultMessage: 'Näytä lisää'
  }
})

export const sortDirectionTypes = {
  ASC: 'asc',
  DESC: 'desc'
}

const Search = ({
  rows = [],
  searchDomain,
  intl: { formatMessage },
  isSearching,
  loadMore,
  isShowMoreAvailable,
  page,
  pageSize,
  submit,
  sorting,
  updateUI,
  total,
  dirty,
  loadPreviewEntity,
  mergeInUIState
}) => {
  const isEmpty = rows.length === 0
  const isLoadingMore = isSearching && !isEmpty
  const isLoadedWithResults = !isSearching && !isEmpty
  const isInitialSearch = isSearching && isEmpty

  const onSort = (column) => {
    const sortDirection = sorting.getIn([searchDomain, 'column']) === column.property ?
      sorting.getIn([searchDomain, 'sortDirection']) ||
        sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const newSorting = sorting.mergeIn([searchDomain],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypes.DESC &&
        sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    updateUI('sorting', newSorting)
    submit('frontPageSearch')
  }

  const handlePreviewOnClick = (id, formName) => {
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true
      }
    })
    loadPreviewEntity(id, formName)
  }

  return (
    <SimpleTable
      columns={getSearchDomainColumns(formatMessage, searchDomain, null, onSort, handlePreviewOnClick)}
      className='search-table'>
      <SimpleTable.Extra className=''>
        <FrontPageSearchForm />
        {isLoadedWithResults && <Spacer marginSize='m0' />}
      </SimpleTable.Extra>
      {isLoadedWithResults && <SimpleTable.Header />}
      {isInitialSearch
        ? <tbody><tr><td style={{ textAlign: 'center' }}><Spinner /></td></tr></tbody>
        : <SimpleTable.Body rows={rows} rowKey='id' />}
      {isShowMoreAvailable && !isInitialSearch &&
        <SimpleTable.Extra bottom>
          {isLoadingMore
            ? <Spinner />
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
Search.propTypes = {
  rows: PropTypes.array,
  searchDomain: PropTypes.string.isRequired,
  intl: PropTypes.any.isRequired,
  isSearching: PropTypes.bool.isRequired,
  isShowMoreAvailable: PropTypes.bool.isRequired,
  loadMore: PropTypes.func.isRequired,
  dirty: PropTypes.bool.isRequired,
  updateUI: PropTypes.func.isRequired,
  submit: PropTypes.func.isRequired,
  sorting: ImmutablePropTypes.map.isRequired,
  page: PropTypes.number.isRequired,
  pageSize: PropTypes.number.isRequired,
  total: PropTypes.number.isRequired,
  loadPreviewEntity: PropTypes.func,
  mergeInUIState: PropTypes.func
}

export default compose(
  connect(null, {
    submit,
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  }),
  withState({
    redux: true,
    key: 'uiData',
    keepImmutable: true,
    initialState: {
      sorting: Map()
    }
  }),
)(Search)
