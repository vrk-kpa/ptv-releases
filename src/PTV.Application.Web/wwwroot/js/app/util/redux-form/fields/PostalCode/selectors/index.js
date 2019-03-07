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
import { EntitySelectors } from 'selectors'
import { getEntitiesForIds } from 'selectors/base'
import { createSelector } from 'reselect'
import createCachedSelector from 're-reselect'
import { List } from 'immutable'

const getPostalCodeJS = createSelector(
  EntitySelectors.postalCodes.getEntity,
  postalCode => postalCode.toJS()
)

export const getPostalCodeById = createSelector(
  EntitySelectors.postalCodes.getEntity,
  postalCode => (postalCode && postalCode.get('code')) || null
)

export const getPostalCodeDefaultOptionsJS = createSelector(
  getPostalCodeJS,
  postalCode => [postalCode]
)

export const getIsPostalCodeInStore = createSelector(
  EntitySelectors.postalCodes.getEntity,
  postalCode => postalCode.size !== 0
)

export const getPostalCodeOptionsJS = createSelector(
  EntitySelectors.postalCodes.getEntities,
  postalCodes => {
    const result = postalCodes
      .map(postalCode => ({
        value: postalCode.get('id'),
        label: postalCode.get('code')
      }))
    return result
  }
)

const getStreetPostalCodesIds = createCachedSelector(
  EntitySelectors.streets.getEntity,
  EntitySelectors.streetNumbers.getEntities,
  (street, streetNumbers) => {
    const streetNumberIds = street && street.get('streetNumbers')
    const postalCodeIds = streetNumberIds && streetNumberIds
      .map(id => streetNumbers.getIn([id, 'postalCode']))
      .toSet() || List()
    return postalCodeIds
  }
)(
  (_, { id }) => id || ''
)

const getStreetPostalCodes = createCachedSelector(
  getStreetPostalCodesIds,
  EntitySelectors.postalCodes.getEntities,
  (postalCodeIds, postalCodes) => getEntitiesForIds(postalCodes, postalCodeIds, List())
)(
  (_, { id }) => id || ''
)

export const getStreetPostalCodeOptionsJS = createCachedSelector(
  getStreetPostalCodes,
  postalCodes => !postalCodes
    ? []
    : postalCodes.map(postalCode => ({
      label: postalCode.get('code'),
      value: postalCode.get('id'),
      postOffice: postalCode.get('postOffice'),
      id: postalCode.get('id')
    })).toJS()
)(
  (_, { id }) => id || ''
)
