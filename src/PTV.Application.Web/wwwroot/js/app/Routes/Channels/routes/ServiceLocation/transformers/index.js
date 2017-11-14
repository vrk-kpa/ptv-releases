import {
  editorStateTransformer,
  Mappers
} from 'util/redux-form/submitFilters'

export const serviceLocationChannelTransformer = values => {
  return values
    .update('description', Mappers.languageMap(editorStateTransformer))
    .update('emails', Mappers.languageMap(Mappers.getFilteredList))
    .update('phoneNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('faxNumbers', Mappers.languageMap(Mappers.getFilteredList))
    .update('webPages', Mappers.languageMap(Mappers.getFilteredList))
}
