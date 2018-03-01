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
import { OrderedSet, List } from 'immutable'
import { getFormValue,
  getEntitiesForIds,
  getParameterFromProps } from 'selectors/base'
import { EntitySelectors } from 'selectors'

// form selectors
export const getFormOrganizations = createSelector(
    getFormValue('responsibleOrganizations'),
    values => values || List()
)

export const getFormOrganization = createSelector(
  getFormValue('organization'),
  value => value || null
)

export const getFormResponsibleOrganizationsOrderedSet = createSelector(
  getFormOrganizations,
  organizations => organizations.toOrderedSet() || OrderedSet()
)

export const getFormOrganizationsOrderedSet = createSelector(
    [getFormResponsibleOrganizationsOrderedSet, getFormOrganization],
    (organizations, organization) => organization && OrderedSet([organization]).concat(organizations) || organizations
)

export const getOrganizationsForForm = createSelector(
    [EntitySelectors.organizations.getEntities, getFormOrganizationsOrderedSet],
    (organizations, ids) => getEntitiesForIds(organizations, ids, List())
)

export const getOrganizationsForFormJS = createSelector(
    getOrganizationsForForm,
    organizations => organizations.map(organization => ({
      value: organization.get('id'),
      label: organization.get('displayName')
    })).toJS()
)

export const getFormProducers = createSelector(
    getFormValue('serviceProducers'),
    values => values || List()
)

export const getFormSelfProducers = createSelector(
    [getFormProducers, getParameterFromProps('index', 0)],
    (formProducers, index) => formProducers.getIn([index, 'selfProducers'])
)

export const getFormSelfProducersOrderedSet = createSelector(
    getFormSelfProducers,
    formProducers => formProducers && formProducers.toOrderedSet() || OrderedSet()
)

export const getFormSelfProducersOrganizationIntersect = createSelector(
    [getFormSelfProducersOrderedSet, getFormOrganizationsOrderedSet],
    (selfProducers, organizations) => organizations.intersect(selfProducers) || OrderedSet()
)
