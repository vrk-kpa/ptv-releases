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
import { List, Map, OrderedSet } from 'immutable'
import {
  getFormValue
} from 'selectors/base'
import { EnumsSelectors } from 'selectors'

export const getProvisionTypes = createSelector(
  EnumsSelectors.provisionTypes.getEntities,
  entities => entities || Map()
)

export const getProvisionTypeSelfProduced = createSelector(
  getProvisionTypes,
  provisionTypes =>
    provisionTypes.filter(pT => pT.get('code').toLowerCase() === 'selfproduced').first() || Map()
)

export const getFormServiceProducers = createSelector(
  getFormValue('serviceProducers'),
  producers => producers || List()
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

export const getFormServiceProducersSelfProducers = createSelector(
  getFormServiceProducersSelfProduced,
  selfProducer => OrderedSet(selfProducer.get('selfProducers'))
)

export const getFormOrganizations = createSelector(
  getFormValue('responsibleOrganizations'),
  organizations => organizations || OrderedSet()
)
