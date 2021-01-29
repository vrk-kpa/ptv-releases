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
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { getFormValue } from 'selectors/base'
import { List, Map, OrderedSet } from 'immutable'
import createCachedSelector from 're-reselect'

export const getOrganizationEntities = createSelector(
  EntitySelectors.organizations.getEntities,
  organizations => organizations
)
export const getTopOrganizations = createSelector(
  EnumsSelectors.organizations.getEnums,
  organizations => organizations
)

export const getSelectedOrganizationOnForm = createCachedSelector(
  getFormValue('organization'),
  organization => organization
)(
  (state, props) => props.formName || ''
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

export const getFormServiceProducers = createCachedSelector(
  getFormValue('serviceProducers'),
  producers => producers || List()
)(
  (state, props) => props.formName || ''
)

export const getProvisionTypeSelfProducedId = createSelector(
  getProvisionTypeSelfProduced,
  selfProduced => selfProduced.get('id') || ''
)

export const getFormServiceProducersSelfProduced = createSelector(
  [getFormServiceProducers, getProvisionTypeSelfProducedId],
  (producers, selfProducerTypeId) =>
    producers.filter(x => x && x.get('provisionType') === selfProducerTypeId).first() ||
      Map()
)
export const getFormServiceProducersSelfProducedIndex = createSelector(
  [getFormServiceProducers, getProvisionTypeSelfProducedId],
  (producers, selfProducerTypeId) =>
    producers.findKey(x => x && x.get('provisionType') === selfProducerTypeId)
)

export const getFormServiceProducersSelfProducers = createCachedSelector(
  getFormServiceProducersSelfProduced,
  selfProducer => OrderedSet(selfProducer.get('selfProducers'))
)(
  (state, props) => props.formName || ''
)
