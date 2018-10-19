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

