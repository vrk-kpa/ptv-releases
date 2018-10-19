import {
  editorStateTransformer,
  Mappers
} from 'util/redux-form/submitFilters'

import {
  Set
} from 'immutable'

const keyIn = keys => {
  const keySet = Set(keys)
  return (v, k) => keySet.has(k)
}

const servicePropertyNames = {
  isAlternateNameUsed: 'isAlternateNameUsedAsDisplayName'
}

export const serviceLocationChannelTransformer = values => {
  values = values.update(x => x.mapKeys(key => servicePropertyNames[key] || key))
  return values
    .update('description', Mappers.languageMap(editorStateTransformer))
    .update('emails', Mappers.languageMap(Mappers.getFilteredList))
    .update('phoneNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('faxNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('webPages', Mappers.languageMap(Mappers.getFilteredList))
    .update('isAlternateNameUsedAsDisplayName', x => x && x.filter(y => y).keySeq().toList())
}
export const accessibilityRegisterTransformer = values => {
  return values
    .update('visitingAddresses', visitingAddresses => {
      return visitingAddresses
        .map(visitingAddress => visitingAddress.update('accessibilityRegister', accessibilityRegister => {
          if (!accessibilityRegister) return null
          return accessibilityRegister.filter(keyIn([
            'id',
            'entranceId',
            'isMainEntrance'
          ]))
        }))
    })
}
