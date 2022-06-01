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
import { Map, List, OrderedSet } from 'immutable'
import { getFormValue,
  getObjectArray,
  getParameterFromProps } from 'selectors/base'
import { EnumsSelectors } from 'selectors'

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

export const getProvisionTypeSelfProducedId = createSelector(
  getProvisionTypeSelfProduced,
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

export const getProvisionTypesObjectArrayFiltered = createSelector(
  [getProvisionTypes, getProvisionTypeSelfProducedId],
  (provisionTypes, selfProducedId) =>
    getObjectArray(provisionTypes.filter(pt => pt.get('id') !== selfProducedId))
)

export const getIsAlreadySelfProduced = createSelector(
  [getFormProducers, getProvisionTypeSelfProducedId],
  (serviceProducers, selfProducedId) =>
    serviceProducers.some(p => p && p.get('provisionType') === selfProducedId) || false
)

export const getProvisionTypesObjectArray = createSelector(
  [getProvisionTypes,
    getIsAlreadySelfProducedForIndex,
    getProvisionTypesObjectArrayFiltered,
    getIsAlreadySelfProduced],
  (provisionTypes, isSelfProducedOnIndex, filtered, isAlreadySelfProducedInList) =>
   !isAlreadySelfProducedInList ? getObjectArray(provisionTypes)
   : isAlreadySelfProducedInList && isSelfProducedOnIndex ? getObjectArray(provisionTypes) : filtered
)

export const getFormOrganizations = createSelector(
    [getFormValue('responsibleOrganizations'), getFormValue('organization')],
    (rO, o) => o && rO && rO.add(o) || o && OrderedSet([o]) || rO || OrderedSet()
)
