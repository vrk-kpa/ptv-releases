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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { Button, Spinner } from 'sema-ui-components'
import {
  getAddressSearchResults,
  getAddressSearchIsFetching,
  getAddressSearchIsMoreFetching,
  getAddressSearchIsMoreAvailable,
  getChannelAddressSearchResults
} from './selectors/index'
import {
  loadMoreAddresses,
  loadAddresses
} from './actions'
import getColumnsDefinition from './getColumnsDefinition'
import { buttonMessages } from 'Routes/messages'
import styles from './styles.scss'
import cx from 'classnames'
import { createGetEntityAction } from 'actions'
import { mergeInUIState } from 'reducers/ui'
import { formTypesEnum } from 'enums'
import {
  DataTable
} from 'util/redux-form/fields'
import injectFormName from 'util/redux-form/HOC/injectFormName'

class SearchResult extends Component {
  handleLoadMoreOnClick = (values, formName) => this.props.loadMore(values, formName)
  handlePreviewOnClick = (entityId) => {
    this.props.mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formTypesEnum.SERVICELOCATIONFORM,
        isOpen: true,
        entityId: null
      }
    })
    this.props.loadPreviewEntity(entityId, formTypesEnum.SERVICELOCATIONFORM, 'loadPreview')
  }
  render () {
    const {
      intl: { formatMessage },
      isLoadingMore,
      isLoading,
      isMoreAvailable,
      preview,
      rows,
      top,
      values,
      formName
    } = this.props
    const searchResultClass = cx(
      styles.searchResult,
      {
        [styles.top]: top
      }
    )
    return (
      <div>
        <DataTable
          name={'ServiceChannelAddressSearchResult'}
          rows={rows}
          columnsDefinition={getColumnsDefinition({
            formatMessage,
            previewOnClick: this.handlePreviewOnClick,
            formName
          })}
          scrollable
          columnWidths={['15%', '25%', '25%', '35%']}
          sortOnClick={() => this.props.load(values, formName, true)}
          className={searchResultClass}
          isLoading={isLoading}
        />
        {!preview && isMoreAvailable && (<div className={styles.showMore}>
          <Button
            children={isLoadingMore
              ? <Spinner />
              : formatMessage(buttonMessages.showMore)}
            small
            secondary
            className={styles.showMoreButton}
            onClick={() => this.handleLoadMoreOnClick(values, formName)}
          />
        </div>)}
      </div>
    )
  }
}
SearchResult.propTypes = {
  intl: intlShape,
  rows: PropTypes.array,
  isLoadingMore: PropTypes.bool,
  isLoading: PropTypes.bool,
  isMoreAvailable: PropTypes.bool,
  loadMore: PropTypes.func,
  load: PropTypes.func,
  mergeInUIState: PropTypes.func,
  loadPreviewEntity: PropTypes.func,
  preview: PropTypes.bool,
  top: PropTypes.bool,
  values: PropTypes.any
}

const loadMore = (values, formName) => ({ dispatch, getState }) => {
  dispatch(loadMoreAddresses(getState(), values, formName))
}
const load = (values, formName, sort) => ({ dispatch, getState }) => {
  dispatch(loadAddresses(getState(), dispatch, values, formName, sort))
}

export default compose(
  injectFormName,
  connect(
    (state, ownProps) => ({
      rows: getAddressSearchResults(state, ownProps),
      isLoading: getAddressSearchIsFetching(state, ownProps),
      isLoadingMore: getAddressSearchIsMoreFetching(state, ownProps),
      isMoreAvailable: getAddressSearchIsMoreAvailable(state, ownProps)
    }), {
      loadMore,
      load,
      mergeInUIState,
      loadPreviewEntity: createGetEntityAction
    }),
  injectIntl
)(SearchResult)
