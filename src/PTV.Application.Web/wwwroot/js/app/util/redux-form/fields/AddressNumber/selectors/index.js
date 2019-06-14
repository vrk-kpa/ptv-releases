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
import { getFormValueWithPath } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { combinePathAndField } from 'util/redux-form/util'

export const getFormStreetId = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'street')),
  id => id
)

export const getFormPostalCodeId = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'postalCode')),
  id => id
)

const startNumberRegex = /(^\d+)(.*$)/i

export const parseStreetNumber = input => parseInt(input && input.replace(startNumberRegex, '$1'))

const getStreetNumbersByStreet = createSelector(
  [EntitySelectors.streets.getEntities, EntitySelectors.streetNumbers.getEntities, getFormStreetId],
  (streets, streetNumbers, streetId) => {
    const street = streets.get(streetId)
    const streetNumberIds = street && street.get('streetNumbers')
    const ranges = streetNumberIds && streetNumbers.filter((value, key) => streetNumberIds.some(id => id === key))
    return ranges
  }
)

export const isAddressNumberInRange = (input, rangeStart, rangeEnd, isEven) => {
  if (input % 2 !== (isEven ? 0 : 1)) {
    return false
  }

  if (rangeEnd === 0) {
    return input === rangeStart
  }

  return (input >= rangeStart) && (input <= rangeEnd)
}

const getStreetNumberRange = createSelector(
  [getStreetNumbersByStreet, getFormValueWithPath((props) => combinePathAndField(props.path, 'streetNumber'))],
  (streetNumbers, input) => {
    const streetNumber = parseStreetNumber(input)
    const ranges = streetNumbers && streetNumbers.filter(
      (value, key) => {
        const rangeStart = value.get('startNumber')
        const rangeEnd = value.get('endNumber')
        const isEven = value.get('isEven')

        return isAddressNumberInRange(streetNumber, rangeStart, rangeEnd, isEven)
      })

    const rangeList = ranges && ranges.toList()
    const range = rangeList && rangeList.get(0)
    return range
  }
)

export const getStreetNumberRangeId = createSelector(
  getStreetNumberRange,
  range => (range && range.get('id')) || null
)

export const getPostalCodeByStreetNumber = createSelector(
  getStreetNumberRange,
  range => range && range.get('postalCode')
)
