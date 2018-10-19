import { createSelector } from 'reselect'
import EntitySelectors from 'selectors/entities'
import { getFormValues } from 'redux-form/immutable'
import { List, Map } from 'immutable'
import { getPublishingStatusDraftId, getEntityLanguageAvailabilities } from 'selectors/common'
import { getParameterFromProps } from 'selectors/base'

export const getLanguagesToCopy = createSelector(
  getFormValues('copyEntityForm'),
  formValues => {
    return formValues && formValues.get('languagesVersions') || List()
  }
)

const getEntityById = createSelector(
  [EntitySelectors.getEntitiesWithGivenType, getParameterFromProps('copyId')],
  (entities, id) => entities.get(id) || Map()
)

const getLanguagesCodesToCopy = createSelector(
  [getLanguagesToCopy, EntitySelectors.languages.getEntities],
  (ids, languages) => ids.map(id => languages.getIn([id, 'code'])) || List()
)

export const getSelectedOrCopyEntity = createSelector(
  [getEntityById, EntitySelectors.getEntity],
  (copyEntity, entity) => copyEntity.size > 0 ? copyEntity : entity
)

export const createFilteredField = selector => createSelector(
  [selector, getLanguagesCodesToCopy, getParameterFromProps('copyId')],
  (field, copyLanguageCodes, copyId) => copyId ? field.filter((item, key) => copyLanguageCodes.includes(key)) : field
)

const getLanguageAvailabilitiesForEntity = createSelector(
  getEntityById,
  entity => entity.get('languagesAvailabilities') || List()
)

const getFilteredLanguageAvailabilities = createSelector(
  [
    getLanguageAvailabilitiesForEntity,
    getLanguagesToCopy,
    getPublishingStatusDraftId,
    EntitySelectors.languages.getEntities
  ], (
    languageAvailabilities,
    copyLanguageIds,
    draftStatusId,
    languages
  ) => {
    const filteredLAs = copyLanguageIds.size > 0
      ? languageAvailabilities.filter(lA => {
        return copyLanguageIds.includes(lA.get('languageId'))
      })
      : languageAvailabilities
    let copiedLAs = filteredLAs.map(lA => {
      const languageId = lA.get('languageId')
      const language = languages.get(languageId)
      const code = language && language.get('code')
      return (
        lA.clear().merge({ 'languageId': languageId, 'code': code })
      )
    })
    return copiedLAs
  }
)

export const getLanguageAvailabilities = createSelector(
  [getFilteredLanguageAvailabilities, getEntityLanguageAvailabilities, getParameterFromProps('copyId')],
  (filteredLAs, entityLAs, copyId) => copyId ? filteredLAs : entityLAs
)
