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
import { getApiCalls,
  getEntitiesForIdsJS,
  getParameterFromProps
} from 'selectors/base'
import { taskTypesEnum, taskTypes } from 'enums'

const getTaskTypeHelper = (taskTypeSelector) => createSelector(
  [EntitySelectors.tasks.getEntities, taskTypeSelector],
  (entities, taskType) => entities.get(taskType) || Map()
)

const getTaskTypeCountHelper = (taskTypeSelector) => createSelector(
  getTaskTypeHelper(taskTypeSelector),
  taskType => taskType.get('count') || 0
)

export const getTasksType = getTaskTypeHelper(getParameterFromProps('taskType'))

export const getTasksTypeCount = getTaskTypeCountHelper(getParameterFromProps('taskType'))

export const getTaskTypeApiCallTasks = createSelector(
  getApiCalls,
  taskTypeApiCall => taskTypeApiCall.get('tasks') || Map()
)

export const getTaskTypeApiCall = createSelector(
  [getTaskTypeApiCallTasks, getParameterFromProps('taskType')],
  (apiCall, taskType) => apiCall.get(taskType) || Map()
)

export const getTaskTypeApiCallLoadMore = createSelector(
  getTaskTypeApiCall,
  taskTypeApiCall => taskTypeApiCall.get('loadMore') || Map()
)

export const getTaskTypePrevEntities = createSelector(
  getTaskTypeApiCallLoadMore,
  taskTypeApiCallLoad => taskTypeApiCallLoad.get('prevEntities') || List()
)

export const getTaskTypeLoadMoreIsFetching = createSelector(
  getTaskTypeApiCallLoadMore,
  taskTypeApiCallLoad => taskTypeApiCallLoad.get('isFetching') || false
)

export const getTasksCurrentEntitiesIds = createSelector(
  getTasksType,
  taskType => taskType.get('entities') || List()
)

export const getTasksEntitiesIds = createSelector(
  [getTaskTypePrevEntities, getTasksCurrentEntitiesIds],
  (prevIds, currentIds) => OrderedSet(prevIds.concat(currentIds)).toList()
)

export const getTasksMaxPageCount = createSelector(
  getTasksType,
  taskType => taskType.get('maxPageCount') || 0
)

export const getTasksPageNumber = createSelector(
  getTasksType,
  taskType => taskType.get('pageNumber') || 0
)

export const getTasksIsMoreAvailable = createSelector(
  getTasksType,
  taskType => taskType.get('moreAvailable') || false
)

export const getIsPostponeButtonVisible = createSelector(
  getTasksType,
  taskType => taskType.get('isPostponeButtonVisible') || false
)

export const getTasksServicesEntities = createSelector(
  [EntitySelectors.getEntities, getParameterFromProps('taskType'), getTasksEntitiesIds],
  (entities, taskType, entityIds) => {
    let entity = 'services'

    switch (taskType) {
      case taskTypesEnum.OUTDATEDDRAFTSERVICES:
      case taskTypesEnum.OUTDATEDPUBLISHEDSERVICES:
      case taskTypesEnum.CHANNELSWITHOUTSERVICES:
        entity = 'services'
        break
      case taskTypesEnum.OUTDATEDDRAFTCHANNELS:
      case taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS:
      case taskTypesEnum.SERVICESWITHOUTCHANNELS:
        entity = 'channels'
        break
    }

    return getEntitiesForIdsJS(entities.get(entity), entityIds, [])
  }
)

export const getTasksSummaryCount = createSelector(
  [
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.OUTDATEDDRAFTSERVICES]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDSERVICES]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.OUTDATEDDRAFTCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.SERVICESWITHOUTCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.CHANNELSWITHOUTSERVICES])
  ],
  (...counts) => counts.reduce((acc, curr) => acc + curr, 0)
)

export const getAreTasksEmpty = createSelector(
  getTasksSummaryCount,
  tasksSummary => tasksSummary === 0
)

export const getNumbersIsFetching = createSelector(
  EntitySelectors.tasks.getEntityIsFetching,
  isFetching => isFetching
)

export const getTasksIsFetching = createSelector(
  EntitySelectors.tasks.getEntityIsFetchingKeyPath(getParameterFromProps('taskType')),
  isFetching => isFetching
)

const getPostponeKey = createSelector(
  getParameterFromProps('taskType'),
  taskType => [taskType, 'postpone', 'load']
)

export const getPostponeIsFetching = createSelector(
  EntitySelectors.tasks.getEntityIsFetchingKeyPath(getPostponeKey),
  isFetching => isFetching
)
