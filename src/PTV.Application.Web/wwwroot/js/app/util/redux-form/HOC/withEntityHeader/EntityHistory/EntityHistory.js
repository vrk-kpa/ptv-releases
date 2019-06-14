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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { apiCall3 } from 'actions'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { FormattedMessage, defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { EntitySchemas } from 'schemas'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { Spinner, SimpleTable, Button, Label } from 'sema-ui-components'
import {
  getRows,
  getIsLoading,
  getIsAlreadyLoaded,
  getVersioningId,
  getOperationIds,
  getPageNumber,
  getIsMoreAvailable
} from './selectors'
import columns from './columns'
import styles from './styles.scss'
import { buttonMessages } from 'Routes/messages'
import { List } from 'immutable'

const messages = defineMessages({
  languages: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.NoResults',
    defaultMessage: 'No results'
  },
  archiveNote: {
    id : 'EntityHistory.ArchiveNote.Title',
    defaultMessage: 'Palvelutietovarannossa ei näytetä yli 15 kk vanhempaa tietoa tai yli 10 versiota arkistoiduista sisällöistä.', // eslint-disable-line
    description: { en: 'Services and channel information older than 15 months or more than 10 archived versions are not shown in the Finnish Service Catalogue.' } // eslint-disable-line
  }
})

class EntityHistory extends Component {
  componentDidMount () {
    this.props.loadConnectionHistory()
  }
  componentWillReceiveProps ({ isAlreadyLoaded }) {
    if (isAlreadyLoaded !== this.props.isAlreadyLoaded) {
      this.props.loadConnectionHistory()
    }
  }
  render () {
    const {
      isLoading,
      rows,
      intl: { formatMessage },
      isShowMoreLoading,
      isMoreAvailable
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
      <SimpleTable
        columns={columns}
        scrollable
        columnWidths={['20%', '30%', '30%', '20%']}
        className={styles.entityHistory}
      >
        <SimpleTable.Header />
        <SimpleTable.Body rows={rows} rowKey='id' />
        <SimpleTable.Extra bottom>
          {isMoreAvailable
            ? <Button
              children={isShowMoreLoading && <Spinner /> || formatMessage(buttonMessages.showMore)}
              onClick={() => this.props.loadConnectionHistory(true)}
              disabled={isShowMoreLoading}
              small secondary
              className={styles.showMoreButton}
            />
            : <Label labelText={formatMessage(messages.archiveNote)} infoLabel className={styles.archiveNote} />
          }
        </SimpleTable.Extra>
      </SimpleTable>
    )
  }
}
EntityHistory.propTypes = {
  isLoading: PropTypes.bool,
  rows: PropTypes.array,
  loadConnectionHistory: PropTypes.func.isRequired,
  intl: intlShape,
  isMoreAvailable: PropTypes.bool,
  isShowMoreLoading: PropTypes.bool,
  isAlreadyLoaded: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const page = getPageNumber(state)
    return {
      rows: getRows(state),
      isLoading: !page && getIsLoading(state),
      isShowMoreLoading: !!page && getIsLoading(state),
      isMoreAvailable: getIsMoreAvailable(state),
      pageNumber: getPageNumber(state),
      isAlreadyLoaded: getIsAlreadyLoaded(state)
    }
  }, {
    loadConnectionHistory: (loadMore) => ({ dispatch, getState }) => {
      const state = getState()
      const isAlreadyLoaded = getIsAlreadyLoaded(state)
      if (loadMore || !isAlreadyLoaded) {
        const versioningId = getVersioningId(state)
        const entityType = getSelectedEntityType(state)
        const pageNumber = loadMore && getPageNumber(state) || 0
        const prevEntities = loadMore && getOperationIds(state).toJS() || List()
        let endpoint
        if (entityType === 'services') {
          endpoint = `service/GetEntityHistory`
        } else if (entityType === 'channels') {
          endpoint = `channel/GetEntityHistory`
        } else if (entityType === 'organizations') {
          endpoint = `organization/GetEntityHistory`
        } else if (entityType === 'serviceCollections') {
          endpoint = `serviceCollection/GetEntityHistory`
        } else if (entityType === 'generalDescriptions') {
          endpoint = `generalDescription/GetEntityHistory`
        } else {
          throw new Error(
            `EntityHistory: No endpoint for entityType ${entityType}`
          )
        }
        dispatch(
          apiCall3({
            keys: ['entityHistory', entityType, versioningId, 'connection'],
            payload: { endpoint,
              data: {
                id: versioningId,
                pageNumber,
                prevEntities
              }
            },
            schemas: EntitySchemas.GET_SEARCH(EntitySchemas.ENTITY_OPERATION),
            saveRequestData: true,
            clearRequest: ['prevEntities']
          })
        )
      }
    }
  })
)(EntityHistory)
