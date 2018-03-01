import {
  editorStateTransformer,
  Mappers
} from 'util/redux-form/submitFilters'

export const phoneChannelTransformer = values => {
  return values
    .update('description', Mappers.languageMap(editorStateTransformer))
    .update('emails', Mappers.languageMap(Mappers.getFilteredList))
    .update('phoneNumbers', Mappers.languageMap(Mappers.getFilteredList))
}
