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
import { apiCall3, API_CALL_CLEAN } from 'actions'
import { TasksSchemas } from 'schemas/tasks'
import { EntitySchemas } from 'schemas'
import { NotificationSchemas } from 'schemas/notifications'
import { taskTypes, formTypesEnum } from 'enums'
import { getUiSortingData } from 'selectors/base'
import { getTasksEntitiesIds,
  getTasksPageNumber,
  getContentLinkPageNumber
} from '../selectors'
import {
  getNotificationsPageNumberByType,
  getNotificationsIdsByType
} from 'Routes/Tasks/selectors/notifications'
import { List } from 'immutable'
import { mergeInUIState } from 'reducers/ui'
import { getSelectedLanguage } from 'Intl/Selectors'

export const loadTasksNumber = () => {
  return apiCall3({
    keys: ['tasks', 'load'],
    payload: { endpoint:'tasks/GetTasksNumbers' },
    schemas: TasksSchemas.TASK_PAGE_ARRAY
  })
}
export const loadNotificationsNumbers = apiCall3({
  keys: ['notifications', 'notificationNumbers'],
  payload: { endpoint: 'notifications/NotificationsNumbers' },
  schemas: NotificationSchemas.NOTIFICATION_GROUP_ARRAY
})

export const postponeTasksEntities = (taskType, schema) => {
  return null
}

export const createLoadTaskEntities = (taskType, schemas) => (state, dispatch, { form, loadMoreEntities }) => {
  const sortingData = getUiSortingData(state, { contentType: form })
  const language = getSelectedLanguage(state)
  const pageNumber = loadMoreEntities && getTasksPageNumber(state, { taskType: taskTypes[taskType] }) || 0
  const prevEntities = loadMoreEntities &&
    getTasksEntitiesIds(state, { taskType: taskTypes[taskType] }).toJS() || List()

  if (!loadMoreEntities) {
    dispatch({
      type: API_CALL_CLEAN,
      keys: [
        'tasks',
        taskTypes[taskType],
        'loadMore',
        'prevEntities'
      ]
    })
  }
  return apiCall3({
    keys: ['tasks', taskTypes[taskType], loadMoreEntities ? 'loadMore' : 'load'],
    payload: { endpoint:'tasks/GetTasksEntities',
      data: { taskType: taskTypes[taskType],
        sortData: (sortingData && sortingData.size > 0) ? [sortingData] : [],
        pageNumber,
        prevEntities,
        language }
    },
    schemas: schemas,
    saveRequestData: true,
    clearRequest: ['prevEntities']
  })
}

export const createLoadNotificationEntities = (notificationType) => (state, dispatch, { form, loadMoreEntities }) => {
  const pageNumber = loadMoreEntities && getNotificationsPageNumberByType(
    state,
    { notificationType }
  ) || 0
  const prevEntities = loadMoreEntities && getNotificationsIdsByType(
    state,
    { notificationType }
  ).toJS()

  if (!loadMoreEntities) {
    dispatch({
      type: API_CALL_CLEAN,
      keys: [
        'notifications',
        notificationType,
        'loadMore',
        'prevEntities'
      ]
    })
  }
  const sortingData = getUiSortingData(state, { contentType: form }) || List()
  return apiCall3({
    keys: ['notifications', notificationType, loadMoreEntities ? 'loadMore' : 'load'],
    payload: {
      endpoint: `notifications/${notificationType}`,
      data: {
        sortData: (sortingData && sortingData.size > 0) ? [sortingData] : [],
        pageNumber,
        prevEntities
      }
    },
    saveRequestData: true,
    schemas: NotificationSchemas.NOTIFICATION_GROUP,
    clearRequest: ['prevEntities']
  })
}

export const changeTaskTab = index => ({ dispatch }) => {
  dispatch(mergeInUIState({
    key: 'taskTabIndex',
    value: {
      'activeTabIndex': index
    }
  }))
}

export const toggleBrokenLinksAccordion = (index, indices, type) => ({ dispatch }) => {
  dispatch(mergeInUIState({
    key: 'taskType',
    value: {
      'activeTypeBrokenLinks': index > -1
        ? [...indices.slice(0, index), ...indices.slice(index + 1)]
        : [...indices, type]
    }
  }))
}

export const toggleBrokenLinkDetail = (activeId, id, taskType) => ({ dispatch }) => {
  dispatch(mergeInUIState({
    key: taskType,
    value: {
      activeId: activeId === id
        ? null
        : id
    }
  }))
}

export const loadMoreLinkContent = (id, taskType) => ({ dispatch, getState }) => {
  const state = getState()
  const pageNumber = getContentLinkPageNumber(state, { id })
  dispatch(apiCall3({
    keys: ['tasks', 'brokenLinks', 'loadMore'],
    payload: {
      endpoint: 'tasks/loadLinkContent',
      data: {
        id,
        pageNumber,
        taskType
      }
    },
    saveRequestData: true,
    schemas: EntitySchemas.BROKEN_LINK
  }))
}

