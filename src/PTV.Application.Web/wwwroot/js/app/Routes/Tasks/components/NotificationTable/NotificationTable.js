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
import {
  getNotificationsResultsByType,
  getIsNotificationLoadingByType,
  getIsNotificationLoadingMoreByType,
  getNotificationsIsMoreAvailableByType,
  getNotificationsIsEmptyByType,
  getNotificationCount
} from 'Routes/Tasks/selectors/notifications'
import { getTNColumnsDefinition } from 'Routes/Tasks/components/columnsDefinition'
import EntityForm from 'Routes/Tasks/components/EntityForm'
import { createLoadNotificationEntities } from 'Routes/Tasks/actions'
class NotificationTable extends Component {
  loadNotificationEntities = createLoadNotificationEntities(this.props.notificationType)
  render () {
    const {
      isLoadingMore,
      isMoreAvailable,
      notificationType,
      count,
      activeType,
      isLoading,
      rows,
      columnWidths
    } = this.props
    return (
      <EntityForm form={`notificationEntitiesForm${notificationType}`}
        getColumnsDefinition={getTNColumnsDefinition(notificationType)}
        count={count}
        isLoading={isLoading}
        isActive={activeType === notificationType}
        searchResults={rows}
        isMoreAvailable={isMoreAvailable}
        isLoadMoreFetching={isLoadingMore}
        loadEntities={this.loadNotificationEntities}
        columnWidths={columnWidths}
        scrollable
      />

    )
  }
}
NotificationTable.propTypes = {
  rows: PropTypes.array,
  isLoadingMore: PropTypes.bool.isRequired,
  isMoreAvailable: PropTypes.bool.isRequired,
  isLoading: PropTypes.bool.isRequired,
  notificationType: PropTypes.string.isRequired,
  activeType: PropTypes.string,
  count: PropTypes.number.isRequired,
  columnWidths: PropTypes.array
}

export default compose(
  connect(
    (state, ownProps) => ({
      rows: getNotificationsResultsByType(state, ownProps),
      isLoading: getIsNotificationLoadingByType(state, ownProps),
      isLoadingMore: getIsNotificationLoadingMoreByType(state, ownProps),
      isMoreAvailable: getNotificationsIsMoreAvailableByType(state, ownProps),
      isEmpty: getNotificationsIsEmptyByType(state, ownProps),
      count: getNotificationCount(state,
        { notificationType: ownProps.notificationType })
    })),
)(NotificationTable)
