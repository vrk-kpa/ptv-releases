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
import { apiCall3 } from 'actions'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { EntitySchemas } from 'schemas'
import { getEntityUnificRoot, getSelectedEntityType } from 'selectors/entities/entities'
import { Spinner, SimpleTable, Button } from 'sema-ui-components'
import {
  getRows,
  getIsLoading,
  getIsAlreadyLoaded,
  getOperationIds,
  getPageNumber,
  getIsMoreAvailable
} from './selectors'
import { FormattedMessage, defineMessages, injectIntl, intlShape } from 'util/react-intl'
import columns from './columns'
import styles from './styles.scss'
import { buttonMessages } from 'Routes/messages'
import { List } from 'immutable'

const messages = defineMessages({
  languages: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.ConnectionHistory.NoResults',
    defaultMessage: 'No results'
  }
})

class ConnectionHistory extends Component {
  componentDidMount () {
    this.props.loadConnectionHistory()
  }
  render () {
    const {
      isLoading,
      isMoreAvailable,
      intl: { formatMessage },
      isShowMoreLoading,
      rows
    } = this.props
    if (isLoading) {
      return (
        <div className={styles.spinner}>
          <Spinner />
        </div>
      )
    }
    if (!isLoading && rows.length === 0) {
      return (
        <div className={styles.empty}>
          <FormattedMessage {...messages.languages} />
        </div>
      )
    }
    return (
      <SimpleTable columns={columns}
        scrollable
        columnWidths={['20%', '30%', '30%', '10%', '10%']}>
        <SimpleTable.Header />
        <SimpleTable.Body rows={rows} rowKey='id' />
        {isMoreAvailable &&
          <SimpleTable.Extra bottom>
            <Button
              children={isShowMoreLoading && <Spinner /> || formatMessage(buttonMessages.showMore)}
              onClick={() => this.props.loadConnectionHistory(true)}
              disabled={isShowMoreLoading}
              small secondary
              className={styles.showMoreButton}
            />
          </SimpleTable.Extra>
        }
      </SimpleTable>
    )
  }
}
ConnectionHistory.propTypes = {
  isLoading: PropTypes.bool,
  rows: PropTypes.array,
  loadConnectionHistory: PropTypes.func.isRequired,
  intl: intlShape,
  isMoreAvailable: PropTypes.bool,
  isShowMoreLoading: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(state => {
    const page = getPageNumber(state)
    return {
      rows: getRows(state),
      isLoading: !page && getIsLoading(state),
      isShowMoreLoading: !!page && getIsLoading(state),
      isMoreAvailable: getIsMoreAvailable(state),
      pageNumber: getPageNumber(state)
    }
  }, {
    loadConnectionHistory: (loadMore) => ({ dispatch, getState }) => {
      const state = getState()
      const isAlreadyLoaded = getIsAlreadyLoaded(state)
      if (loadMore || !isAlreadyLoaded) {
        const unificRootId = getEntityUnificRoot(state)
        const entityType = getSelectedEntityType(state)
        const pageNumber = loadMore && getPageNumber(state) || 0
        const prevEntities = loadMore && getOperationIds(state).toJS() || List()
        dispatch(
          apiCall3({
            keys: ['connectionHistory', entityType, unificRootId, 'connection'],
            payload: {
              endpoint: {
                services: `service/GetConnectionHistory`,
                channels: `channel/GetConnectionHistory`,
                serviceCollections: `serviceCollection/GetConnectionHistory`,
                generalDescriptions: `generalDescription/GetConnectionHistory`
              }[entityType],
              data: {
                id: unificRootId,
                pageNumber,
                prevEntities
              }
            },
            schemas: EntitySchemas.GET_SEARCH(EntitySchemas.CONNECTION_OPERATION),
            saveRequestData: true,
            clearRequest: ['prevEntities']
          })
        )
      }
    }
  })
)(ConnectionHistory)
