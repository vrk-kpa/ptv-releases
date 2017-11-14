import {
  editorStateTransformer,
  Mappers
} from 'util/redux-form/submitFilters'

export const printableFormTransformer = values => {
  const result = values
    .update('description', Mappers.languageMap(editorStateTransformer))
    .update('formFiles', Mappers.languageMap(Mappers.getFilteredList))
    .update('attachments', Mappers.languageMap(Mappers.getFilteredList))
    .update('emails', Mappers.languageMap(Mappers.getFilteredList))
    .update('phoneNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('deliveryAddress', da => da && da.size && da || null)

  return result
}

export const addressTransformer = values => {
  // if (values.hasIn(['deliveryAddress', 'address'])) {
  //   // Flattening deliveryAddress structure //
  //   let flatDeliveryAddress = values
  //     .getIn(['deliveryAddress'])
  //     .flatten(true)
  //   values = values.set('deliveryAddress', flatDeliveryAddress)
  //   // Renaming some properties //
  //   const newAddressPropertyNames = {
  //     municipality: 'municipalityId'
  //   }
  //   values = values
  //     .update('deliveryAddress', address => address
  //         .mapKeys(key => newAddressPropertyNames[key] || key))
  // }
  return values
}
