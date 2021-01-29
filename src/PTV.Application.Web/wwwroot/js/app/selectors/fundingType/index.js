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
import { getParameterFromProps } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { getUserOrganization } from 'selectors/userInfo'
import { Map } from 'immutable'

const getFundingTypePublic = createSelector(
  EntitySelectors.fundingTypes.getEntities,
  fundingTypes => fundingTypes.find(ft => ft.get('code').toLowerCase() === 'publiclyfunded') || Map()
)

const getFundingTypePublicId = createSelector(
  getFundingTypePublic,
  fundingTypePublic => fundingTypePublic.get('id') || ''
)

const getFundingTypePrivate = createSelector(
  EntitySelectors.fundingTypes.getEntities,
  fundingTypes => fundingTypes.find(ft => ft.get('code').toLowerCase() === 'marketfunded') || Map()
)

const getFundingTypePrivateId = createSelector(
  getFundingTypePrivate,
  fundingTypePrivate => fundingTypePrivate.get('id') || ''
)

const getOrganizationTypeCode = createSelector(
  getParameterFromProps('organization'),
  getUserOrganization,
  EntitySelectors.organizations.getEntities,
  EntitySelectors.organizationTypes.getEntities,
  (organization, userOrganization, organizations, organizationTypes) => {
    const organizationId = organization || userOrganization
    if (!organizationId) return null

    const organizationTypeId = organizations.getIn([organizationId, 'typeId'])
    const organizationTypeCode = organizationTypes.getIn([organizationTypeId, 'code']) || ''
    return organizationTypeCode.toLowerCase()
  }
)

export const getCanBeChangedFundingType = createSelector(
  getOrganizationTypeCode,
  organizationTypeCode => {
    switch (organizationTypeCode) {
      case '':
      case 'company':
      case 'municipality':  
      case 'organization':  
      case 'state':
      case 'regionalorganization':
        return true
      default:
        return false
    }
  }
)

export const getFundingTypeForOrganization = createSelector(
  getOrganizationTypeCode,
  getFundingTypePublicId,
  getFundingTypePrivateId,
  (organizationTypeCode, publicId, privateId) => {
    switch (organizationTypeCode) {
      case '':
        return null
      case 'company':
      case 'organization':
        return privateId
      default:
        return publicId
    }
  }
)
