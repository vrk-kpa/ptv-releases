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
import {
  getEntityInfo,
  formMainEntityType,
  formAllTypes,
  permisionTypes,
  securityOrganizationCheckTypes,
  entityTypesEnum
} from 'enums'
import { getIsAccessible } from 'appComponents/Security/selectors'
import { getIsRegionUserOrganization } from 'selectors/userInfo'
import {
  getEntity
} from 'selectors/entities/entities'

const getOrganizationCheckType = (entityType) => {
  return entityType === entityTypesEnum.GENERALDESCRIPTIONS
    ? securityOrganizationCheckTypes.otherOrganization
    : securityOrganizationCheckTypes.byOrganization
}

export const getIsEntityAccessible = (state, props) => {
  const { formName } = props
  const entity = getEntity(state)
  const entityType = formMainEntityType[formName] || ''
  const subEntityType = formAllTypes[formName] || ''
  const info = getEntityInfo(entityType, subEntityType)
  const isRegion = getIsRegionUserOrganization(state)
  const organizationId = entity.get('organizationId')
  const unificRootId = entity.get('unificRootId')
  return getIsAccessible(state, {
    domain: `${info.type}MassTool`,
    permisionType: permisionTypes.publish,
    organization: info.type === entityTypesEnum.ORGANIZATIONS ? unificRootId : organizationId,
    formName,
    checkOrganization: getOrganizationCheckType(info.type, permisionTypes.publish, isRegion)
  })
}
