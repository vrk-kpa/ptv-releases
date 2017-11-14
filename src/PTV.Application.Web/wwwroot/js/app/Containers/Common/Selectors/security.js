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
import { Map } from 'immutable'
import * as CommonSelectors from './index'
import { securityOrganizationCheckTypes } from '../Enums'
import { getUserInfo } from 'selectors/userInfo'

// const permisions = getUserPermisions();

// export const getUserInfoJs = createSelector(
//     [getUserName, getUserRoleName],
//     (name, role) => {
//         return { name, role, permisions };
//     }
// );

const getPermisions = createSelector(
    getUserInfo,
    user => user.get('permisions') || Map()
)

const getPermision = createSelector(
    [getPermisions, CommonSelectors.getParameterFromProps('domain')],
    (permisions, domain) => {
        // console.log(domain, permisions.toJS())
      return permisions.get(domain) || Map()
    }
)

// const getUserOrganizations = createSelector(
//     getUserInfo,
//     user => user.get('allowedOrganizations') || null
// );

const getSecurityInfos = createSelector(
  CommonSelectors.getEntities,
  entities => entities.get('securityInfo') || Map()
)

const getSecurityInfo = createSelector(
  [getSecurityInfos, CommonSelectors.getPageEntityId],
  (entities, id) => entities.get(id) || Map()
)

const getIsOwnOrganization = createSelector(
  getSecurityInfo,
  securityInfo => securityInfo.get('isOwnOrganization') || false
)

const getRuleName = (isOwnOrganization, checkOrganization) => {
  switch (checkOrganization) {
      case securityOrganizationCheckTypes.byOrganization:
        {
          return isOwnOrganization ? 'rulesOwn' : 'rulesAll'
        }
      case securityOrganizationCheckTypes.ownOrganization:
        {
          return 'rulesOwn'
        }
      case securityOrganizationCheckTypes.otherOrganization:
        {
          return 'rulesAll'
        }
      default:
        {
          return 'rulesOwn'
        }
    }
}

const getRule = createSelector(
  [getPermision, getIsOwnOrganization, CommonSelectors.getParameterFromProps('checkOrganization'), CommonSelectors.getParameterFromProps('domain')],
  (permision, isOwnOrganization, checkOrganization, domain) => {
    return permision.get(getRuleName(isOwnOrganization, checkOrganization))
  }
)

export const getIsAccessible = createSelector(
  [getRule, CommonSelectors.getParameterFromProps('permisionType')],
  (rule, type) => rule & type
)

export const getIsAnyAccessible = createSelector(
  [
    getPermisions,
    getIsOwnOrganization,
    CommonSelectors.getParameterFromProps('checkOrganization'),
    CommonSelectors.getParameterFromProps('permisionType')
  ],
  (permisions, isOwnOrganization, checkOrganization, type) => {
    const ruleName = getRuleName(isOwnOrganization, checkOrganization)
    return permisions.filter(x => x.get(ruleName) & type).size > 0
  }
)
