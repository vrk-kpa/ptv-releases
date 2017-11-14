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
import { getParameterFromProps } from 'selectors/base'
import { getFormOrganization } from 'selectors/form'
import { securityOrganizationCheckTypes, entityTypesEnum } from 'enums'
import { getUserInfo, getUserOrganizationRoles } from 'selectors/userInfo'
import { getEntityUnificRoot } from 'selectors/entities/entities'

const getRolesPermisions = createSelector(
    getUserInfo,
    user => user.get('permisions') || Map()
)

export const getRole = createSelector(
    getUserInfo,
    user => (user.get('role') || '').toLowerCase()
)

const getUserOrganizationRole = createSelector(
  [getUserOrganizationRoles, getFormOrganization, getRole, getEntityUnificRoot, getParameterFromProps('domain')],
  (organizationRoles, organizationId, role, id, domain) =>
    organizationRoles.get(organizationId) ||
    domain === entityTypesEnum.ORGANIZATIONS && organizationRoles.get(id) ||
    role
)

const getPermisionsForRole = createSelector(
  [getRolesPermisions, getUserOrganizationRole],
  (user, role) => {
    // console.log(user && user.toJS(), role)
    return user.get(role) || Map()
  }
)

const getPermision = createSelector(
    [getPermisionsForRole, getParameterFromProps('domain')],
    (permisions, domain) => {
      // console.log(permisions && permisions.toJS())
      return permisions.get(domain) || Map()
    }
)

// const getSecurityInfo = createSelector(
//   [EntitySelectors.securityInfo.getEntities, EntitySelectors.getSelectedEntityId],
//   (entities, id) => entities.get(id) || Map()
// )

const getIsOwnOrganization = createSelector(
  [getUserOrganizationRoles, getFormOrganization, getEntityUnificRoot, getParameterFromProps('domain')],
  (userOrganizations, organizationId, id, domain) =>
    (organizationId && userOrganizations.get(organizationId)) ||
    (domain === entityTypesEnum.ORGANIZATIONS && id && userOrganizations.get(id))
)

const getRuleName = (isOwnOrganization, checkOrganization) => {
  switch (checkOrganization) {
    case securityOrganizationCheckTypes.byOrganization: {
      return isOwnOrganization ? 'rulesOwn' : 'rulesAll'
    }
    case securityOrganizationCheckTypes.ownOrganization: {
      return 'rulesOwn'
    }
    case securityOrganizationCheckTypes.otherOrganization: {
      return 'rulesAll'
    }
    default: {
      return 'rulesOwn'
    }
  }
}

const getRule = createSelector(
  [
    getPermision,
    getIsOwnOrganization,
    getParameterFromProps('checkOrganization'),
    getParameterFromProps('domain')
  ],
  (permision, isOwnOrganization, checkOrganization, domain) => {
    const rule = permision.get(getRuleName(isOwnOrganization, checkOrganization))
    // console.log(rule, permision, isOwnOrganization, checkOrganization, domain)
    return rule
  }
)

export const getIsAccessible = createSelector(
  [getRule, getParameterFromProps('permisionType')],
  (rule, type) => {
    // console.log(rule, type, rule & type)
    return (rule & type) === type
  }
)

export const getIsAnyAccessible = createSelector(
  [
    getPermisionsForRole,
    getIsOwnOrganization,
    getParameterFromProps('checkOrganization'),
    getParameterFromProps('permisionType')
  ],
  (permisions, isOwnOrganization, checkOrganization, type) => {
    const ruleName = getRuleName(isOwnOrganization, checkOrganization)
    return permisions.filter(x => x.get(ruleName) & type).size > 0
  }
)
