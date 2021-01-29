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
import { createSelector } from 'reselect'
import { Map, fromJS, List } from 'immutable'
import { camelizeKeys } from 'humps'
import { getApplication, getEntitiesForIds } from 'selectors/base'
import EntitySelectors from 'selectors/entities'

export const getUserInfo = createSelector(
  getApplication,
  app => app.get('userInfo') || Map()
)
export const getUserInfoIsFetching = createSelector(
  getUserInfo,
  app => app.get('isFetching') || false
)
export const getUserInfoAreDataValid = createSelector(
  getUserInfo,
  app => app.get('areDataValid') || false
)
export const getApplicationVersion = () => getAppVersion()

export const getMapDNSes = () => fromJS(camelizeKeys(g_mapDNSnames)) || Map()

export const getUserRoleName = createSelector(
  getUserInfo,
  user => user.get('role')
)
export const getUserName = createSelector(
  getUserInfo,
  user => user.get('name') || ''
)

export const getUserSurname = createSelector(
  getUserInfo,
  user => user.get('surname') || ''
)

export const getUserEmail = createSelector(
  getUserInfo,
  user => user.get('email') || ''
)

export const getHasAccess = createSelector(
  getUserInfo,
  user => !!user.get('hasAccess')
)

export const getUserOrganization = createSelector(
  getUserInfo,
  user => user.get('userOrganization') || null
)

const getUserOrganizationRolesData = createSelector(
  getApplication,
  app => app.getIn(['userOrganizationRoles', 'result']) || Map()
)

export const getIsRegionUserOrganization = createSelector(
  getUserOrganizationRolesData,
  data => data.get('isRegionUserOrganization') || false
)

export const getUserOrganizationIds = createSelector(
  getUserOrganizationRolesData,
  app => app.get('organizationRoles') || List()
)

export const getUserOrganizationRoles = createSelector(
  getUserOrganizationIds,
  EntitySelectors.organizationRoles.getEntities,
  (ids, entities) => ids.reduce(
    (result, id) => result.set(id, entities.getIn([id, 'role']).toLowerCase()),
    Map()
  )
  // .map(or => or.get('role').toLowerCase())
)
