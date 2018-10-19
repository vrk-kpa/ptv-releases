import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { getEnumTypesAreValid } from 'selectors/base'
import { getUserOrganization, getHasAccess, getUserInfo } from 'selectors/userInfo'
import { COMMON_NOTIFICATION_KEY } from 'reducers/notifications'
import { getIsLocalizationMessagesLoaded } from 'Intl/Selectors'
import { getMessages } from 'Intl/Actions'
import { getAuthToken } from 'Configuration/AppHelpers'
import { deleteApiCall } from 'Containers/Common/Actions'
import { TasksSchemas } from 'schemas/tasks'
import { loadNotificationsNumbers } from 'Routes/Tasks/actions'

const loadTasksNumbers = (dispatch, getState) => () => {
  dispatch(
    apiCall3({
      keys: ['tasks', 'load'],
      payload: { endpoint:'tasks/GetTasksNumbers' },
      schemas: TasksSchemas.TASK_PAGE_ARRAY
    })
  )
}

export const loadUserOrganizationRoles = () => ({ dispatch, getState }) => {
  if (!getHasAccess(getState())) {
    return
  }

  dispatch(
    apiCall3({
      keys: ['application', 'userOrganizationRoles'],
      payload: {
        endpoint: 'common/GetUserOrganizationsAndRoles',
        data: {}
      },
      schemas: EntitySchemas.USER_ORGANIZATION_ROLES,
      formName: COMMON_NOTIFICATION_KEY
    })
  )
}

const loadDataForLoggedUser = (dispatch, getState) => (result) => {
  const state = getState()
  if (!getHasAccess(state)) {
    return
  }
  // !getEnumTypesAreValid(state) &&
  dispatch(
    apiCall3({
      keys: ['application', 'enumTypes'],
      payload: {
        endpoint: 'common/GetEnumTypes',
        data: {
          userOrganization: getUserOrganization(state)
        }
      },
      schemas: EntitySchemas.ENUM,
      successNextAction: loadTasksNumbers(dispatch, getState),
      formName: COMMON_NOTIFICATION_KEY
    })
  )
  dispatch(loadNotificationsNumbers)
  // console.log(result)
  // !EnumsSelectors.organizationRoles.getIsEnumLoaded(state) &&
  dispatch(loadUserOrganizationRoles())
}

export const loadEnums = () => ({ dispatch, getState }) => {
  // console.log('init')
  if (getAuthToken()) {
    dispatch(
      apiCall3({
        keys: ['application', 'userInfo'],
        payload: {
          endpoint: 'common/GetUserInfo',
          data: {}
        },
        formName: COMMON_NOTIFICATION_KEY,
        successNextAction: loadDataForLoggedUser(dispatch, getState)
      })
    )
  }

  !getIsLocalizationMessagesLoaded(getState()) && dispatch(getMessages())
}

export const deleteUserInfo = () => ({ dispatch, getState }) => {
  dispatch(deleteApiCall(['application', 'userInfo']))
  dispatch(deleteApiCall(['frontPageSearch']))
}
