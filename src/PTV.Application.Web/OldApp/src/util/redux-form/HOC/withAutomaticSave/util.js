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
import {
  OrderedMap,
  OrderedSet,
  List,
  Map,
  Iterable
} from 'immutable'
import { startsWith } from 'lodash'
const { isOrderedMap } = OrderedMap
const { isOrderedSet } = OrderedSet
const { isList } = List
const { isIterable: isImmutable } = Iterable

export const markImmutableObjectKeysWithItsType = input => {
  if (!isImmutable(input)) {
    return input
  }
  return input.reduce((acc, curr, fieldName) => {
    if (isOrderedMap(curr)) {
      return acc.set(
        `orderedMap_${fieldName}`,
        markImmutableObjectKeysWithItsType(curr)
      )
    } else if (isOrderedSet(curr)) {
      return acc.set(
        `orderedSet_${fieldName}`,
        curr.map(markImmutableObjectKeysWithItsType)
      )
    } else if (isList(curr)) {
      return acc.set(
        fieldName,
        curr.map(markImmutableObjectKeysWithItsType)
      )
    } else if (isImmutable(curr)) {
      return acc.set(
        fieldName,
        markImmutableObjectKeysWithItsType(curr)
      )
    } else {
      return acc.set(
        fieldName,
        curr
      )
    }
  }, Map())
}
export const convertMarkedImmutableObjectsToItsType = input => {
  if (!isImmutable(input)) {
    return input
  }
  return input.reduce((acc, curr, fieldName) => {
    if (startsWith(fieldName, 'orderedMap_')) {
      return acc.set(
        fieldName.replace('orderedMap_', ''),
        OrderedMap(convertMarkedImmutableObjectsToItsType(curr))
      )
    } else if (startsWith(fieldName, 'orderedSet_')) {
      return acc.set(
        fieldName.replace('orderedSet_', ''),
        OrderedSet(curr.map(convertMarkedImmutableObjectsToItsType))
      )
    } else if (isList(curr)) {
      return acc.set(
        fieldName,
        curr.map(convertMarkedImmutableObjectsToItsType)
      )
    } else if (isImmutable(curr)) {
      return acc.set(
        fieldName,
        convertMarkedImmutableObjectsToItsType(curr)
      )
    } else {
      return acc.set(
        fieldName,
        curr
      )
    }
  }, Map())
}
