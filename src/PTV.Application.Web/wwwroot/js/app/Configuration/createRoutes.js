/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import App from './App'
import FrontPageRoute from 'Routes/FrontPage'
import ChannelsRoute from 'Routes/Channels'
import OrganizationRoute from 'Routes/Organization'
import ServiceRoute from 'Routes/Service'
import GeneralDescriptionRoute from 'Routes/GeneralDescription'
import Connections from 'Routes/Connections'
import CurrentIssuesRoute from 'Routes/CurrentIssues'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { getEnumTypesAreValid } from 'selectors/base'
import { getUserOrganization } from 'selectors/userInfo'
// import { EnumsSelectors } from 'selectors'
import { COMMON_NOTIFICATION_KEY } from 'reducers/notifications'
import { setReadOnly } from 'reducers/formStates'
import { formTypesEnum } from 'enums'
import { getMessages } from 'Intl/Actions'
import { getIsLocalizationMessagesLoaded } from 'Intl/Selectors'

const loadEnums = ({ dispatch, getState }) => {
  const state = getState()
  !getEnumTypesAreValid(state) &&
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
  // !EnumsSelectors.organizationRoles.getIsEnumLoaded(state) &&
  dispatch(
      apiCall3({
        keys: ['application', 'userInfo'],
        payload: {
          endpoint: 'common/GetUserOrganizationsAndRoles',
          data: {}
        },
        schemas: EntitySchemas.ENUM,
        formName: COMMON_NOTIFICATION_KEY
      })
    )

  !getIsLocalizationMessagesLoaded(state) && dispatch(getMessages())
}

const setPreviewReadOnly = ({ dispatch }) => {
  dispatch(
    setReadOnly({
      form: formTypesEnum.PREVIEW,
      value: true
    })
  )
}

const init = (store, nextState) => {
  loadEnums(store)
  setPreviewReadOnly(store)
}

const createRoutes = store => ({
  path: '/',
  indexRoute: {
    onEnter: (_, replace) => replace('/frontpage/search')
  },
  childRoutes: [
    FrontPageRoute(store),
    ChannelsRoute(store),
    OrganizationRoute(store),
    ServiceRoute(store),
    GeneralDescriptionRoute(store),
    CurrentIssuesRoute(store),
    Connections(store)
  ],
  getComponent (nextState, cb) {
    init(store, nextState)
    cb(null, App)
  }
})

export default createRoutes
