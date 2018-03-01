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
import { Map, List, OrderedSet } from 'immutable'
import { getFormValue,
  getEntitiesForIds,
  getParameterFromProps } from 'selectors/base'
import { EnumsSelectors, EntitySelectors } from 'selectors'

// form selectors
export const getFormProducers = createSelector(
    getFormValue('serviceProducers'),
    values => values || List()
)

export const getProvisionTypes = createSelector(
  EnumsSelectors.provisionTypes.getEntities,
  entities => entities || Map()
)

export const getProvisionTypeSelfProduced = createSelector(
  getProvisionTypes,
  provisionTypes =>
    provisionTypes.filter(pT => pT.get('code').toLowerCase() === 'selfproduced').first() || Map()
)

export const getProvisionTypeOther = createSelector(
  getProvisionTypes,
  provisionTypes =>
    provisionTypes.filter(pT => pT.get('code').toLowerCase() === 'other').first() || Map()
)

export const getProvisionTypeSelfProducedId = createSelector(
  getProvisionTypeSelfProduced,
  selfProduced => selfProduced.get('id') || ''
)

export const getProvisionTypeOtherId = createSelector(
  getProvisionTypeOther,
  selfProduced => selfProduced.get('id') || ''
)

export const getIsAlreadySelfProducedForIndex = createSelector(
  [getFormProducers,
    getParameterFromProps('index', 0),
    getProvisionTypeSelfProducedId],
  (serviceProducers, index, selfProducedId) =>
    serviceProducers.get(index) &&
    serviceProducers.get(index).get('provisionType') === selfProducedId || false
)

export const getIsOtherWithOrganizationForIndex = createSelector(
  [getFormProducers,
    getParameterFromProps('index', 0),
    getProvisionTypeOtherId],
  (serviceProducers, index, otherId) =>
    serviceProducers.get(index) && !!serviceProducers.get(index).get('organization') || false
)

export const getIsAnySelectedForIndex = createSelector(
  [getFormProducers,
    getParameterFromProps('index', 0),
    getProvisionTypeSelfProducedId],
  (serviceProducers, index, selfProducedId) =>
    serviceProducers.get(index) &&
    serviceProducers.get(index).get('provisionType') != null || false
)

export const getFormOrganizations = createSelector(
    getFormValue('responsibleOrganizations'),
    values => values || OrderedSet()
)

export const getFormOrganization = createSelector(
  getFormValue('organization'),
  value => value || null
)

export const getOrganizationsMergedForForm = createSelector(
    [getFormOrganizations, getFormOrganization],
    (organizations, org) => org && organizations.add(org) || organizations
)

export const getOrganizationsForForm = createSelector(
  [EntitySelectors.organizations.getEntities, getOrganizationsMergedForForm],
  (organizations, ids) => getEntitiesForIds(organizations, ids, List())
)

export const getIsMoreThanOneOrganizationForForm = createSelector(
    getOrganizationsForForm,
    organizations => organizations.size > 1 || false
)
