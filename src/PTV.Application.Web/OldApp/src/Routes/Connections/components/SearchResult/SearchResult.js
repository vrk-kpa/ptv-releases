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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getResultsJS,
  getIsSearchingResults,
  getChannelResultMoreAvailable,
  getServiceResultMoreAvailable,
  getServiceResultCount,
  getChannelResultCount
} from 'Routes/Connections/selectors'
import { getConnectionsEntity } from 'selectors/selections'
import { SimpleTable, Spinner, Button } from 'sema-ui-components'
import { serviceColumnDefinition, channelColumnDefinition } from './columnDefinition'
import { entityTypesEnum } from 'enums'
import { injectIntl, intlShape } from 'util/react-intl'
import messages from '../../messages'
import styles from './styles.scss'
import { buttonMessages } from 'Routes/messages'
import { searchEntities } from 'Routes/Connections/actions'

class SearchResult extends PureComponent {
  columnsDefinition (formatMessage, size) {
    return {
      [entityTypesEnum.CHANNELS]: channelColumnDefinition(formatMessage, size),
      [entityTypesEnum.SERVICES]: serviceColumnDefinition(formatMessage, size)
    }[this.props.entity]
  }
  render () {
    const {
      results,
      isSearching,
      entity,
      moreAvailable,
      searchEntities,
      totalCount,
      intl: { formatMessage }
    } = this.props
    const length = results && results.length || 0
    const countMessage = length === totalCount ? totalCount : `${length}/${totalCount}`
    const resultTable = (
      <div className={styles.searchResults}>
        <SimpleTable
          columns={this.columnsDefinition(formatMessage, countMessage)}
          borderless
        >
          <SimpleTable.Header />
          <SimpleTable.Body rows={results} rowKey={'id'} />
          {moreAvailable &&
          <SimpleTable.Extra bottom>
            {isSearching
              ? <Spinner />
              : <Button
                children={`${formatMessage(buttonMessages.showMore)}`}
                onClick={() => searchEntities(entity)}
                small secondary
                className={styles.showMoreButton}
              />
            }
          </SimpleTable.Extra>}
        </SimpleTable>
      </div>
    )
    const loading = (
      <div className={styles.info}>
        <span><Spinner /></span>
      </div>
    )
    const noResults = (
      <div className={styles.info}>
        <span>{formatMessage(messages.noResultsPlaceholder)}</span>
      </div>
    )
    const selectEntity = (
      <div className={styles.standaloneInfo}>
        <span>{formatMessage(messages.entityNotSelectedPlaceholder)}</span>
      </div>
    )
    if (isSearching && results.length === 0) return loading
    if (entity === null) return selectEntity
    if (!isSearching && results.length === 0) return noResults
    return resultTable
  }
}
SearchResult.propTypes = {
  results: PropTypes.array,
  isSearching: PropTypes.bool,
  entity: PropTypes.oneOf(['channels', 'services']),
  intl: intlShape,
  moreAvailable: PropTypes.bool,
  searchEntities: PropTypes.func,
  totalCount: PropTypes.number
}

export default compose(
  injectIntl,
  connect(state => {
    const entity = getConnectionsEntity(state)
    return {
      results: getResultsJS(state),
      isSearching: getIsSearchingResults(state),
      entity,
      moreAvailable: {
        [entityTypesEnum.CHANNELS]: getChannelResultMoreAvailable(state),
        [entityTypesEnum.SERVICES]: getServiceResultMoreAvailable(state)
      }[entity] || false,
      totalCount: {
        [entityTypesEnum.CHANNELS]: getChannelResultCount(state),
        [entityTypesEnum.SERVICES]: getServiceResultCount(state)
      }[entity] || 0
    }
  }, {
    searchEntities
  })
)(SearchResult)
