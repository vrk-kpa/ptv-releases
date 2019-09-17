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
import { createSelector } from 'reselect'
import { EntitySelectors } from 'selectors'
import { getParameterFromProps, getUi, getUiSorting } from 'selectors/base'
import { List, Map } from 'immutable'
import { adminTaskTypesEnum } from 'enums'
import { sortUIData } from 'util/helpers'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

export const getNumbersIsFetching = createSelector(
  EntitySelectors.adminTasks.getEntityIsFetching,
  isFetching => isFetching
)

const createAdminTaskSelector = adminTaskTypeSelector => {
  return createSelector(
    [EntitySelectors.adminTasks.getEntities, adminTaskTypeSelector],
    (entities, adminTaskType) => {
      return entities.get(adminTaskType) || Map()
    }
  )
}

const getAdminTaskTypeFromProps = getParameterFromProps('taskType')
const getAdminTaskByType = createAdminTaskSelector(getAdminTaskTypeFromProps)

export const getIsAdminTaskLoadingByType = createSelector(
  EntitySelectors.adminTasks.getEntityIsFetchingKeyPath(
    getAdminTaskTypeFromProps
  ),
  (isFetching) => isFetching
)

const getAdminTaskSearchIdsByType = createSelector(
  getAdminTaskByType,
  tasks => tasks.get('entities') || List()
)

const getAdminTasksEntities = createSelector(
  EntitySelectors.getEntities,
  getAdminTaskSearchIdsByType,
  (entities, entityIds) =>
    entityIds.map(
      entityInfo => entities.getIn([ entityInfo.get('schema'), entityInfo.get('id') ])
    )
)

export const getAdminTasksEntitiesJS = createSelector(
  getAdminTasksEntities,
  entities => {
    return entities.toJS()
  }
)

export const getAdminTasksEntityByCode = createSelector(
  getAdminTaskByType,
  getParameterFromProps('code'),
  (entities, code) => {
    return entities.get(code) || Map()
  }
)

export const getActiveDetailEntityIds = createSelector(
  getUi,
  getAdminTaskTypeFromProps,
  (ui, taskType) => ui.getIn([taskType, 'activeDetailEntityIds']) || List()
)

const getTaskSchedulerSorting = createSelector(
  getUiSorting,
  sorting => sorting && sorting.get('adminTaskscheduledTasks') || Map()
)

const getTaskSchedulerSortingColumn = createSelector(
  getTaskSchedulerSorting,
  sorting => sorting.get('column') || 'none'
)

const getTaskSchedulerSortingDirection = createSelector(
  getTaskSchedulerSorting,
  sorting => sorting.get('sortDirection') || 'none'
)

const sortingColumns = [
  'name',
  'nextFireTimeUtc',
  'lastJobStatus'
]

const getTranslatedTaskSchedulerResult = createTranslatedListSelector(
  createAdminTaskSelector(getAdminTaskTypeFromProps), {
    nameAttribute: 'name',
    getId: item => item.getIn(['name', 'id']),
    getDefaultTranslationText: (id, name) => name.get('defaultMessage'),
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getVisibleAdminTaskSchedulerResultByTypeJS = createSelector(
  getTranslatedTaskSchedulerResult,
  getTaskSchedulerSortingColumn,
  getTaskSchedulerSortingDirection,
  (result, column, direction) => {
    return sortingColumns.includes(column)
      ? sortUIData(result.map(x => x.toJS()).toArray(), column, direction)
      : result.toList()
  }
)

export const getAdminTaskIsEmptyByType = createSelector(
  [getAdminTaskSearchIdsByType, getIsAdminTaskLoadingByType],
  (tasks, isLoading) => {
    return (!tasks || tasks.size === 0) && !isLoading
  }
)

const createAdminTaskCountSelector = taskTypeSelector => createSelector(
  [createAdminTaskSelector(taskTypeSelector)],
  task => (task.has('count') ? task.get('count') : task.size) || 0
)

export const getAdminTaskCount = createAdminTaskCountSelector(getAdminTaskTypeFromProps)

export const getFailedTranslationOrdersCount =
  createAdminTaskCountSelector(() => adminTaskTypesEnum.FAILEDTRANSLATIONORDERS)
export const getBrokenLinksOrganizationsCount =
  createAdminTaskCountSelector(() => adminTaskTypesEnum.BROKENLINKSORGANIZATIONS)
export const getBrokenLinksGeneralDescriptionsCount =
  createAdminTaskCountSelector(() => adminTaskTypesEnum.BROKENLINKSGENERALDESCRIPTIONS)
export const getMissingLicenseOrganizationsCount =
  createAdminTaskCountSelector(() => adminTaskTypesEnum.MISSINGLICENSEORGANIZATIONS)
export const getScheduledTasksCount =
  createAdminTaskCountSelector(() => adminTaskTypesEnum.SCHEDULEDTASKS)

export const getAdminTasksCount = createSelector(
  [
    getFailedTranslationOrdersCount,
    getBrokenLinksOrganizationsCount,
    getBrokenLinksGeneralDescriptionsCount,
    getMissingLicenseOrganizationsCount
    // getScheduledTasksCount
  ],
  (...counts) => counts.reduce((acc, curr) => acc + curr, 0)
)

const getUiDeleteTranslationOrderDialog = createSelector(
  getUi,
  ui => ui.get('deleteTranslationOrderDialog') || Map()
)

export const getTranslationOrderIdToDelete = createSelector(
  getUiDeleteTranslationOrderDialog,
  deleteTODialog => deleteTODialog.get('id') || ''
)

const getUiForceJobDialog = createSelector(
  getUi,
  ui => ui.get('forceJobDialog') || Map()
)

export const getForceJobId = createSelector(
  getUiForceJobDialog,
  forceDialog => forceDialog.get('id') || ''
)

export const getForceJobName = createSelector(
  getUiForceJobDialog,
  forceDialog => forceDialog.get('jobName') || ''
)

const getUiLogJobDialog = createSelector(
  getUi,
  ui => ui.get('logJobDialog') || Map()
)

export const getLogJobId = createSelector(
  getUiLogJobDialog,
  logDialog => logDialog.get('id') || ''
)

export const getLogJobName = createSelector(
  getUiLogJobDialog,
  logDialog => logDialog.get('jobName') || ''
)

export const getLogJobArchive = createSelector(
  getUiLogJobDialog,
  logDialog => logDialog.get('archive') || false
)

const getLoadJobKey = createSelector(
  getAdminTaskTypeFromProps,
  taskType => [taskType, 'loadJob']
)

export const getLoadJobIsFetching = createSelector(
  EntitySelectors.adminTasks.getEntityIsFetchingKeyPath(getLoadJobKey),
  isFetching => isFetching
)

const getUiFailedTranslationOrderLogDialog = createSelector(
  getUi,
  ui => ui.get('failedTranslationOrderLogDialog') || Map()
)

export const getUiFailedTranslationOrderLogData = createSelector(
  getUiFailedTranslationOrderLogDialog,
  logDialog => logDialog.get('data') || List()
)
