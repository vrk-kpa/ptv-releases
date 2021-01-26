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
import { Map, List, OrderedSet } from 'immutable'
import { createSelector } from 'reselect'
import createCachedSelector from 're-reselect'
import { EntitySelectors } from 'selectors'
import { getApiCalls,
  getParameterFromProps,
  getUi
} from 'selectors/base'
import { taskTypesEnum, taskTypes } from 'enums'
import { getEntitiesForIdsJS } from 'selectors/base'

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

export const getTasksCurrentEntitiesIds = createCachedSelector(
  getTasksType,
  taskType => taskType.get('entities') || List()
)((_, { taskType }) => taskType || '')

export const getTasksEntitiesIds = createCachedSelector(
  [getTaskTypePrevEntities, getTasksCurrentEntitiesIds],
  (prevIds, currentIds) => OrderedSet(prevIds.concat(currentIds)).toList()
)((_, { taskType }) => taskType || '')

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
  taskType => false
  // SFIPTV-1062 - remove postpone functionality
  // taskType.get('isPostponeButtonVisible') || false
)

export const getTasksServicesEntities = createSelector(
  EntitySelectors.getEntities,
  getTasksEntitiesIds,
  (entities, entityIds) =>
    entityIds.map(
      entityInfo => entities.getIn([ entityInfo.get('schema'), entityInfo.get('id') ])
    )
)

export const getTasksServicesEntitiesJS = createSelector(
  getTasksServicesEntities,
  getParameterFromProps('taskType'),
  (entities, taskType) => {
    if (taskType === taskTypesEnum.TIMEDPUBLISHFAILED) {
      return entities.reduce((acc, val) => {
        const las = val.get('languagesAvailabilities').map(
          la => ({
            languageId: la.get('languageId'),
            statusId: la.get('statusId'),
            lastFailedPublishDate: la.get('lastFailedPublishDate'),
            ...val.toJS()
          })
        ).toJS()

        return acc.concat(las)
      }, []).sort((a, b) => b.lastFailedPublishDate - a.lastFailedPublishDate)
    }
    return entities.toJS()
  }
)

export const getBrokenLinkEntitiesJS = createCachedSelector(
  EntitySelectors.brokenLinks.getEntities,
  getTasksEntitiesIds,
  (entities, ids) => getEntitiesForIdsJS(entities, ids)
)((_, { taskType }) => taskType || '')

export const getTasksSummaryCount = createSelector(
  [
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.NOTUPDATEDDRAFTSERVICES]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.NOTUPDATEDPUBLISHEDSERVICES]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.NOTUPDATEDDRAFTCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.NOTUPDATEDPUBLISHEDCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.SERVICESWITHOUTCHANNELS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.CHANNELSWITHOUTSERVICES]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.TRANSLATIONARRIVED]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.TRANSLATIONINPROGRESS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.TIMEDPUBLISHFAILED]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.UNSTABLELINKS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.BROKENLINKS]),
    getTaskTypeCountHelper(() => taskTypes[taskTypesEnum.CONTENTARCHIVED])
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

const getBrokenLinks = createCachedSelector(
  EntitySelectors.brokenLinks.getEntity,
  brokenLink => brokenLink
)((_, { id }) => id || '')

const getBrokenLinkUrl = createCachedSelector(
  getBrokenLinks,
  brokenLink => brokenLink.get('url') || null
)((_, { id }) => id || '')

const getBrokenLinkIsException = createCachedSelector(
  getBrokenLinks,
  brokenLink => brokenLink.get('isException') || false
)((_, { id }) => id || '')

const getBrokenLinkExceptionComment = createCachedSelector(
  getBrokenLinks,
  brokenLink => brokenLink.get('exceptionComment') || null
)((_, { id }) => id || '')

export const getBrokenLinkInitialValues = createCachedSelector(
  getParameterFromProps('id'),
  getBrokenLinkUrl,
  getBrokenLinkIsException,
  getBrokenLinkExceptionComment,
  (
    id,
    url,
    isException,
    exceptionComment
  ) => {
    return Map().mergeDeep({
      id,
      url,
      isException,
      exceptionComment
    })
  }
)((_, { id }) => id || '')

export const getActiveBrokenLinkId = createSelector(
  getUi,
  getParameterFromProps('taskType'),
  (ui, taskType) => ui.getIn([taskType, 'activeId']) || null
)

export const getIsBrokenLinkVisible = createSelector(
  getActiveBrokenLinkId,
  getParameterFromProps('id'),
  (activeId, id) => activeId === id
)

export const getBrokenLinkContentCount = createSelector(
  getBrokenLinks,
  brokenLink => brokenLink.get('count') || 0
)

export const getBrokenLinkContentMoreAvaliable = createSelector(
  getBrokenLinks,
  brokenLink => brokenLink.get('moreAvailable') || false
)

export const getContentLinkPageNumber = createSelector(
  getBrokenLinks,
  brokenLink => brokenLink.get('pageNumber') || 0
)

export const getTaskBrokenLinksCall = createSelector(
  getTaskTypeApiCallTasks,
  apiCall => apiCall.get('brokenLinks') || Map()
)

export const getBrokenLinkContentIsFetching = createSelector(
  getTaskBrokenLinksCall,
  apiCall => apiCall.getIn(['loadMore', 'isFetching']) || false
)
