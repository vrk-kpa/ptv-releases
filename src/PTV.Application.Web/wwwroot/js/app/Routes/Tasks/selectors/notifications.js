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
import { Map, List, OrderedSet } from 'immutable'
import { createSelector } from 'reselect'
import { EntitySelectors } from 'selectors'
import { getApiCalls, getEntitiesForIds, getParameterFromProps } from 'selectors/base'
import { notificationTypesEnum } from 'enums'
import { isUndefined } from 'lodash'

const createNotificationSelector = notificationTypeSelector => {
  return createSelector(
    [EntitySelectors.notificationsGroup.getEntities, notificationTypeSelector],
    (entities, notificationType) => {
      return entities.get(notificationType) || Map()
    }
  )
}

const getNotificationTypeFromProps = getParameterFromProps('notificationType')
const getNotificationsByType = createNotificationSelector(getNotificationTypeFromProps)

export const getNotificationsApiCall = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('notifications') || Map()
)
export const getNotificationsNumbersApiCall = createSelector(
  getNotificationsApiCall,
  notifications => notifications.get('notificationNumbers') || Map()
)

export const getNotificationsNumbersResult = createSelector(
  getNotificationsNumbersApiCall,
  notificationNumbers => notificationNumbers.get('result') || List()
)

const getNotificationSearchIdsByType = createSelector(
  getNotificationsByType,
  notifications => notifications.get('notifications') || List()
)
const getNotificationPrevSearchIdsByType = createSelector(
  [getApiCalls, getNotificationTypeFromProps],
  (apiCalls, notificationType) => apiCalls.getIn([
    'notifications',
    notificationType,
    'loadMore',
    'prevEntities'
  ]) || List()
)
export const getNotificationsIdsByType = createSelector(
  [getNotificationSearchIdsByType, getNotificationPrevSearchIdsByType],
  (currentIds, previousIds) => OrderedSet(previousIds.concat(currentIds)).toList()
)
export const getNotificationsResultsByType = createSelector(
  [EntitySelectors.notifications.getEntities, getNotificationsIdsByType],
  (notifications, ids) => getEntitiesForIds(notifications, ids, List()).toJS()
)
export const getNotificationsPageNumberByType = createSelector(
  getNotificationsByType,
  notifications => {
    const pageNumber = notifications.get('pageNumber')
    return isUndefined(pageNumber)
      ? 0
      : pageNumber
  }
)
export const getNotificationsIsMoreAvailableByType = createSelector(
  getNotificationsByType,
  notifications => !!notifications.get('moreAvailable')
)

export const getNotificationTypeApiCall = createSelector(
  [getNotificationsApiCall, getParameterFromProps('notificationType')],
  (apiCall, notificationType) => apiCall.get(notificationType) || Map()
)

export const getNotificationsApiCallLoadMore = createSelector(
  getNotificationTypeApiCall,
  notifications => notifications.get('loadMore') || Map()
)

export const getIsNotificationLoadingByType = createSelector(
  EntitySelectors.notifications.getEntityIsFetchingKeyPath(
    getNotificationTypeFromProps
  ),
  (isFetching) => isFetching
)

export const getIsNotificationLoadingMoreByType = createSelector(
  getNotificationsApiCallLoadMore,
  notificationTypeApiCallLoadMore => notificationTypeApiCallLoadMore.get('isFetching') || false
)
export const getNotificationsIsEmptyByType = createSelector(
  [getNotificationsResultsByType, getIsNotificationLoadingByType],
  (notifications, isLoading) => {
    return (!notifications || notifications.length === 0) && !isLoading
  }
)

const createNotificationCountSelector = notificationTypeSelector => createSelector(
  [createNotificationSelector(notificationTypeSelector)],
  notification => notification.get('count') || 0
)

export const getNotificationCount = createNotificationCountSelector(getParameterFromProps('notificationType'))

export const getServiceChannelInCommonUseCount =
  createNotificationCountSelector(() => notificationTypesEnum.SERVICECHANNELINCOMMONUSE)
export const getContentUpdatedCount =
  createNotificationCountSelector(() => notificationTypesEnum.CONTENTUPDATED)
export const getContentArchivedCount =
  createNotificationCountSelector(() => notificationTypesEnum.CONTENTARCHIVED)
export const getGeneralDescriptionCreatedCount =
  createNotificationCountSelector(() => notificationTypesEnum.GENERALDESCRIPTIONCREATED)
export const getGeneralDescriptionUpdatedCount =
  createNotificationCountSelector(() => notificationTypesEnum.GENERALDESCRIPTIONUPDATED)
export const getConnectionChangesCount =
  createNotificationCountSelector(() => notificationTypesEnum.CONNECTIONCHANGES)

export const getNotificationsCount = createSelector(
  [
    getServiceChannelInCommonUseCount,
    getContentUpdatedCount,
    getContentArchivedCount,
    getGeneralDescriptionCreatedCount,
    getGeneralDescriptionUpdatedCount,
    getConnectionChangesCount
  ],
  (...counts) => counts.reduce((acc, curr) => acc + curr, 0)
)

export const getAreNotificationsEmpty = createSelector(
  getNotificationsCount,
  count => count === 0
)
