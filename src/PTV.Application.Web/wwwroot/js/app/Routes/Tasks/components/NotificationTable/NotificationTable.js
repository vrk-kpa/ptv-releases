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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { SimpleTable, Button, Spinner } from 'sema-ui-components'
import {
  getNotificationsResultsByType,
  getIsNotificationLoadingByType,
  getIsNotificationLoadingMoreByType,
  getNotificationsIsMoreAvailableByType,
  getNotificationsPageNumberByType,
  getNotificationsIdsByType,
  getNotificationsIsEmptyByType
} from 'Routes/Tasks/selectors/notifications'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import getColumnsDefinition from './getColumnsDefinition'
import { buttonMessages } from 'Routes/messages'
import styles from './styles.scss'
import { apiCall3, createGetEntityAction } from 'actions'
import { NotificationSchemas } from 'schemas/notifications'
import { mergeInUIState } from 'reducers/ui'
import { formAllTypes, getKey } from 'enums'
import EmptyTableMessage from 'appComponents/EmptyTableMessage'

class NotificationTable extends Component {
  handleLoadMoreOnClick = () => this.props.loadMore(this.props.notificationType)
  handlePreviewOnClick = (entityId, { subEntityType }) => {
    const previewEntityformName = subEntityType
      ? getKey(formAllTypes, subEntityType.toLowerCase())
      : ''
    this.props.mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: previewEntityformName,
        isOpen: true,
        entityId: null
      }
    })
    this.props.loadPreviewEntity(entityId, previewEntityformName)
  }
  render () {
    const {
      intl: { formatMessage },
      isLoadingMore,
      isMoreAvailable,
      isEmpty,
      rows
    } = this.props
    return (
      <SimpleTable
        columns={getColumnsDefinition({
          formatMessage,
          previewOnClick: this.handlePreviewOnClick
        })}
      >
        <SimpleTable.Header />
        {isEmpty
          ? <EmptyTableMessage colSpan={4} />
          : <SimpleTable.Body rowKey='id' rows={rows} />}
        {isMoreAvailable && (
          <SimpleTable.Extra bottom>
            <Button
              children={isLoadingMore
                ? <Spinner />
                : formatMessage(buttonMessages.showMore)}
              small
              secondary
              className={styles.showMoreButton}
              onClick={this.handleLoadMoreOnClick}
            />
          </SimpleTable.Extra>
        )}
      </SimpleTable>
    )
  }
}
NotificationTable.propTypes = {
  intl: intlShape,
  rows: PropTypes.array,
  isLoadingMore: PropTypes.bool,
  isMoreAvailable: PropTypes.bool,
  isEmpty: PropTypes.bool,
  notificationType: PropTypes.string,
  loadMore: PropTypes.func,
  mergeInUIState: PropTypes.func,
  loadPreviewEntity: PropTypes.func
}

export default compose(
  connect(
    (state, ownProps) => ({
      rows: getNotificationsResultsByType(state, ownProps),
      isLoading: getIsNotificationLoadingByType(state, ownProps),
      isLoadingMore: getIsNotificationLoadingMoreByType(state, ownProps),
      isMoreAvailable: getNotificationsIsMoreAvailableByType(state, ownProps),
      isEmpty: getNotificationsIsEmptyByType(state, ownProps)
    }), {
      loadMore: notificationType => ({ dispatch, getState }) => {
        const state = getState()
        const pageNumber = getNotificationsPageNumberByType(
          state,
          { notificationType }
        )
        const prevEntities = getNotificationsIdsByType(
          state,
          { notificationType }
        ).toJS()
        dispatch(
          apiCall3({
            keys: ['notifications', notificationType, 'load'],
            payload: {
              endpoint: `notifications/${notificationType}`,
              data: {
                pageNumber,
                prevEntities
              }
            },
            schemas: NotificationSchemas.NOTIFICATION_GROUP,
            saveRequestData: true,
            clearRequest: ['prevEntities']
          })
        )
      },
      mergeInUIState,
      loadPreviewEntity: createGetEntityAction
    }
  ),
  withBubbling,
  injectIntl,
)(NotificationTable)
