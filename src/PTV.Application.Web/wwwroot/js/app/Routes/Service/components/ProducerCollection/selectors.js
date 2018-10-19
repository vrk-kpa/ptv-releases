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
import enumSelector from 'selectors/enums'
import { getFormValue } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { Map, List, OrderedSet } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { getProvisionTypeSelfProducedId } from 'util/redux-form/fields/ProvisionType/selectors'

const getItems = (_, { items }) => items
const getIndex = (_, { index }) => index
const getIsInCompareMode = (_, { compare }) => !!compare

const getLanguageCode = createSelector(
  [
    getContentLanguageCode,
    getSelectedComparisionLanguageCode,
    getIsInCompareMode
  ],
  (language, comparisionLanguageCode, compare) => {
    return compare && comparisionLanguageCode || language || 'fi'
  }
)

const getProducer = createSelector(
  [getIndex, getItems],
  (index, items) => items && items.get(index) || Map()
)
const getProvisionTypeId = createSelector(
  getProducer,
  producer => {
    return producer.get('provisionType')
  }
)
const getIsSelfProduced = createSelector(
  [
    getProducer,
    getProvisionTypeSelfProducedId
  ],
  (producer, selfProducedId) => {
    return producer.get('provisionType') === selfProducedId
  }
)
const getOrganizationId = createSelector(
  getProducer,
  producer => {
    return producer.get('organization')
  }
)
const getAdditionalInformation = createSelector(
  [getProducer, getLanguageCode],
  (producer, languageCode) => {
    return producer.getIn(['additionalInformation', languageCode])
  }
)

const getProvisionMethodName = createSelector(
  [getProvisionTypeId, enumSelector.provisionTypes.getEntitiesMap],
  (provisionTypeId, entitiesMap) => {
    return entitiesMap.getIn([provisionTypeId, 'name']) || ''
  }
)
const getProviderName = createSelector(
  [
    getOrganizationId,
    EntitySelectors.organizations.getEntities,
    getAdditionalInformation
  ],
  (organizationId, organizations, additionalInformation) => {
    const displayName = organizations
      .getIn([organizationId, 'displayName']) || ''
    const alternateName = organizations
      .getIn([organizationId, 'alternateName']) || ''
    return displayName || alternateName || additionalInformation
  }
)

// Organization list
const getFormOrganization = createSelector(
  getFormValue('organization'),
  value => value || null
)
const getFormOrganizationOS = createSelector(
  getFormOrganization,
  organization => organization && OrderedSet([organization]) || OrderedSet()
)
const getFormResponsibleOrganizations = createSelector(
  getFormValue('responsibleOrganizations'),
  values => values || List()
)
const getFormResponsibleOrganizationsOS = createSelector(
  getFormResponsibleOrganizations,
  responsibleOrganizations => responsibleOrganizations.toOrderedSet() || OrderedSet()
)
const getFormOrganizationsOS = createSelector(
  [getFormResponsibleOrganizationsOS, getFormOrganizationOS],
  (responsibleOrganizations, organization) => organization.concat(responsibleOrganizations)
)

// Producer list
const getFormSelfProducers = createSelector(
  [getItems, getIndex],
  (producers, index) => producers.getIn([index, 'selfProducers'])
)
const getFormSelfProducersOS = createSelector(
  getFormSelfProducers,
  producers => producers && producers.toOrderedSet() || OrderedSet()
)

// Producer & organization intersection
const getFormSelfProducersOrganizationIntersect = createSelector(
  [getFormSelfProducersOS, getFormOrganizationsOS],
  (selfProducers, organizations) => organizations.intersect(selfProducers) || OrderedSet()
)

const getSelfProducerNamesList = createSelector(
  [
    getFormSelfProducersOrganizationIntersect,
    EntitySelectors.organizations.getEntities
  ],
  (ids, organizations) => ids.reduce((acc, curr) => {
    const displayName = organizations.getIn([curr, 'displayName']) || ''
    const alternateName = organizations.getIn([curr, 'alternateName']) || ''
    const selfProducerName = displayName || alternateName
    return acc.push(selfProducerName)
  }, List())
)

export const getProducerTitle = createSelector(
  [getProvisionMethodName, getProviderName, getIsSelfProduced, getSelfProducerNamesList],
  (provisionMethodName, providerName, isSelfProduced, selfProducerList) => {
    if (isSelfProduced) {
      return selfProducerList.size > 0 ? `${provisionMethodName}: ${selfProducerList.join(', ')}` : provisionMethodName
    } else if (provisionMethodName && !providerName) {
      return provisionMethodName
    } else if (provisionMethodName && providerName) {
      return `${provisionMethodName}: ${providerName}`
    }
  }
)
