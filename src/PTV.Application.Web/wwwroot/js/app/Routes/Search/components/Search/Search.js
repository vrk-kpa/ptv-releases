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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { Button, SimpleTable, Spinner } from 'sema-ui-components'
import FrontPageSearchForm from '../FrontPageSearchForm'
import { getEntitiesColumnsDefinition } from './columnsDefinitions/columnsDefinitions'
import withWindowDimensionsContext from 'util/redux-form/HOC/withWindowDimensions/withWindowDimensionsContext'
import Spacer from 'appComponents/Spacer'
import { compose } from 'redux'
import withState from 'util/withState'
import { Map } from 'immutable'
import { connect } from 'react-redux'
import { submit } from 'redux-form/immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'
import { buttonMessages } from 'Routes/messages'
import { formTypesEnum, BREAKPOINT_LG, BREAKPOINT_MD } from 'enums'
import {
  MassToolSelectionForm,
  MassToolFormWrapper,
  MassToolBatchCheckbox,
  MassToolSelectionContent
} from '../MassTool'
import { getIsMassToolActive } from '../MassTool/selectors'
import cx from 'classnames'
import styles from './columnsDefinitions/styles.scss'

export const sortDirectionTypes = {
  ASC: 'asc',
  DESC: 'desc'
}

class Search extends Component {
  onSort = (column) => {
    const {
      searchDomain,
      sorting,
      updateUI
    } = this.props
    const sortDirection = sorting.getIn([searchDomain, 'column']) === column.property
      ? sorting.getIn([searchDomain, 'sortDirection']) ||
        sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const newSorting = sorting.mergeIn([searchDomain],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypes.DESC &&
        sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    updateUI('sorting', newSorting)
    this.props.submit(formTypesEnum.FRONTPAGESEARCH)
  }

  handlePreviewOnClick = (id, formName) => {
    this.props.mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true,
        entityId: null
      }
    })
    this.props.loadPreviewEntity(id, formName)
  }

  render () {
    const {
      rows = [],
      intl: { formatMessage },
      isSearching,
      loadMore,
      isShowMoreAvailable,
      isMassToolActive,
      dimensions
    } = this.props
    const isEmpty = rows.length === 0
    const isLoadingMore = isSearching && !isEmpty
    const isLoadedWithResults = !isSearching && !isEmpty
    const isInitialSearch = isSearching && isEmpty
    const filterColumns = dimensions.windowWidth < BREAKPOINT_LG && dimensions.windowWidth >= BREAKPOINT_MD
    const cardBreakpoint = dimensions.windowWidth < BREAKPOINT_MD
    const searchTableBodyClass = cx(
      styles.searchTableBody,
      {
        [styles.massToolActive]: isMassToolActive
      }
    )
    const searchTableClass = cx(
      styles.searchTable,
      {
        [styles.isMassToolActive]: isMassToolActive,
        [styles.cards]: cardBreakpoint
      }
    )
    const columns = getEntitiesColumnsDefinition(
      formatMessage,
      this.onSort,
      this.handlePreviewOnClick,
      isMassToolActive,
      filterColumns,
      cardBreakpoint
    )

    return (
      <SimpleTable
        columns={columns}
        className={searchTableClass}
        tight
        defaultCards={false}
        asCards={cardBreakpoint}
      >
        <SimpleTable.Extra className=''>
          <FrontPageSearchForm />
          <MassToolSelectionForm content={MassToolSelectionContent} />
          {isLoadedWithResults && !isMassToolActive && !cardBreakpoint && <div className={styles.searchResultsSpacer}>
            <Spacer marginSize='m0' />
          </div>}
        </SimpleTable.Extra>
        {isLoadedWithResults && <SimpleTable.Header />}
        {isInitialSearch
          ? <tbody className={styles.loadingCell}><tr><td><Spinner /></td></tr></tbody>
          : <MassToolFormWrapper>
            {isLoadedWithResults && <MassToolBatchCheckbox colSpan={columns.length} />}
            <SimpleTable.Body rows={rows} rowKey='id' className={searchTableBodyClass} />
          </MassToolFormWrapper>
        }

        {isShowMoreAvailable && !isInitialSearch &&
        <SimpleTable.Extra bottom>
          {isLoadingMore
            ? <Spinner />
            : <Button
              children={`${formatMessage(buttonMessages.showMore)}`}
              onClick={loadMore}
              small secondary
              className={styles.showMoreButton}
            />
          }
        </SimpleTable.Extra>}
      </SimpleTable>
    )
  }
}

Search.propTypes = {
  rows: PropTypes.array,
  searchDomain: PropTypes.string.isRequired,
  intl: PropTypes.any.isRequired,
  isSearching: PropTypes.bool.isRequired,
  isShowMoreAvailable: PropTypes.bool.isRequired,
  loadMore: PropTypes.func.isRequired,
  updateUI: PropTypes.func.isRequired,
  submit: PropTypes.func.isRequired,
  sorting: ImmutablePropTypes.map.isRequired,
  loadPreviewEntity: PropTypes.func,
  mergeInUIState: PropTypes.func,
  isMassToolActive: PropTypes.bool,
  dimensions: PropTypes.object
}

export default compose(
  connect(state => ({
    isMassToolActive: getIsMassToolActive(state)
  }), {
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
  withWindowDimensionsContext
)(Search)
