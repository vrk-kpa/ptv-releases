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
import { apiCall3, deleteApiCall } from 'actions'
import { EntitySchemas } from 'schemas'
import { getUserOrganization, getHasAccess } from 'selectors/userInfo'
import { COMMON_NOTIFICATION_KEY } from 'reducers/notifications'
import { getIsLocalizationMessagesLoaded } from 'Intl/Selectors'
import { getMessages } from 'Intl/Actions'
import { getAuthToken, deleteCookie } from 'Configuration/AppHelpers'
import { loadNotificationsNumbers, loadTasksNumber } from 'Routes/Tasks/actions'
import { loadAdminTasksNumber } from 'Routes/Admin/actions'
import { reset } from 'redux-form/immutable'
import {
  formTypesEnum,
  ptvCookieTokenName,
  pahaCookieTokenName
} from 'enums'
import { deleteUIState } from 'reducers/ui'

export const APPLICATION_INIT = 'APPLICATION_INIT'

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
      formName: COMMON_NOTIFICATION_KEY
    })
  )
  dispatch(loadNotificationsNumbers)
  dispatch(loadTasksNumber())
  dispatch(loadAdminTasksNumber)
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

export const deleteSearchResult = () => ({ dispatch }) => {
  dispatch(deleteApiCall(['frontPageSearch', 'entities']))
}

export const applicationInit = (logout = false) => ({ dispatch }) => {
  if (logout) {
    deleteCookie(ptvCookieTokenName)
    deleteCookie(pahaCookieTokenName)
    dispatch(deleteUserInfo())
    dispatch(deleteUIState())
  }
  dispatch(reset(formTypesEnum.MASSTOOLFORM))
  dispatch(reset(formTypesEnum.MASSTOOLSELECTIONFORM))
  return {
    type: APPLICATION_INIT
  }
}
