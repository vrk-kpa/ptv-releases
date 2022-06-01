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
import { apiCall3 } from 'actions'
import { List } from 'immutable'
import { AdminTasksSchemas } from 'schemas/adminTasks'
import { EntitySchemas } from 'schemas'
import { getUiSortingData } from 'selectors/base'
import { adminTaskTypes, adminTaskTypesEnum } from 'enums'
import { mergeInUIState } from 'reducers/ui'
import { asyncDownloadFileApi } from 'Middleware/Api'

export const loadAdminTasksNumber = apiCall3({
  keys: ['adminTasks', 'load'],
  payload: { endpoint: 'admin/GetTasksNumbers' },
  schemas: AdminTasksSchemas.ADMIN_TASK_PAGE_ARRAY
})

const adminTasksLoadSettings = {
  [adminTaskTypesEnum.FAILEDTRANSLATIONORDERS]: {
    endpoint: 'admin/GetTasksEntities',
    schemas: AdminTasksSchemas.GET_ENTITIES(EntitySchemas.SEARCH)
  },
  [adminTaskTypesEnum.SCHEDULEDTASKS]: {
    endpoint: 'tasksJobs/joblist',
    schemas: AdminTasksSchemas.SCHEDULER_JOBS
  }
}

export const createLoadAdminTaskEntities = taskType => (state, dispatch) => {
  return apiCall3({
    keys: ['adminTasks', taskType, 'load'],
    payload: {
      endpoint: adminTasksLoadSettings[taskType].endpoint,
      data: taskType !== adminTaskTypesEnum.SCHEDULEDTASKS && { taskType }
    },
    schemas: adminTasksLoadSettings[taskType].schemas,
    saveRequestData: true
  })
}

const fetchTranslationsAgain = taskType => {
  return apiCall3({
    keys: ['adminTasks', taskType, 'load'],
    payload: {
      endpoint: 'admin/FetchFailedTranslationOrders',
      data: { taskType }
    },
    schemas: adminTasksLoadSettings[taskType].schemas,
    saveRequestData: true
  })
}

export const performOnSectionButtonAction = taskType => ({ dispatch }) => {
  switch (taskType) {
    case adminTaskTypesEnum.FAILEDTRANSLATIONORDERS:
      dispatch(fetchTranslationsAgain(taskType))
      break
    case adminTaskTypesEnum.SCHEDULEDTASKS:
      dispatch(createLoadAdminTaskEntities(taskType)(null, null))
      break
    default:
      () => console.warn('action not defined')
      break
  }
}

export const deleteTranslationOrder = (id, taskType) => {
  return apiCall3({
    keys: ['adminTasks', taskType, 'remove'],
    payload: {
      endpoint: 'admin/CancelTranslationOrder',
      data: { id, taskType }
    },
    successNextAction: createLoadAdminTaskEntities(taskType)
  })
}

export const forceJob = (id, taskType) => {
  return apiCall3({
    keys: ['adminTasks', taskType, 'force'],
    payload: {
      endpoint: 'tasksJobs/ForceJob/' + id
    },
    successNextAction: createLoadAdminTaskEntities(taskType)
  })
}

export const getJob = (id) => {
  return apiCall3({
    keys: ['adminTasks', adminTaskTypesEnum.SCHEDULEDTASKS, 'loadJob'],
    payload: {
      endpoint: 'tasksJobs/getJob/' + id
    },
    schemas:  AdminTasksSchemas.SCHEDULER_JOBS,
    saveRequestData: true
  })
}

export const downloadLogs = async () => {
  try {
    await asyncDownloadFileApi('tasksJobs/getLog', 'application/zip', 'allJobsLogs.zip')
  } catch (err) {
    console.error('Error log download failed!!!')
  }
}
