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
import EntitySelectors from 'selectors/entities'
import { getFormValues } from 'redux-form/immutable'
import { List, Map } from 'immutable'
import { getEntityLanguageAvailabilities } from 'selectors/common'
import { getParameterFromProps } from 'selectors/base'

export const getLanguagesToCopy = createSelector(
  getFormValues('copyEntityForm'),
  formValues => {
    return formValues && formValues.get('languagesVersions') || List()
  }
)

const getEntityById = createSelector(
  [EntitySelectors.getEntitiesWithGivenType, getParameterFromProps('templateId')],
  (entities, id) => entities.get(id) || Map()
)

const getFilteredLanguageAvailabilities = createSelector(
  [
    (state, props) => props && props.templateId &&
      EntitySelectors.getEntityAvailableLanguages(state, { id: props.templateId, type: props.type }) || List(),
    getLanguagesToCopy,
    EntitySelectors.languages.getEntities
  ], (
    languageAvailabilities,
    copyLanguageIds,
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

const getLanguagesCodesToCopy = createSelector(
  getFilteredLanguageAvailabilities,
  languages => languages.map(l => l.get('code'))
)

export const getSelectedOrCopyEntity = createSelector(
  [getEntityById, EntitySelectors.getEntity],
  (copyEntity, entity) => copyEntity.size > 0 ? copyEntity : entity
)

export const createFilteredField = selector => createSelector(
  [selector, getLanguagesCodesToCopy, getParameterFromProps('templateId')],
  (field, copyLanguageCodes, templateId) => templateId ? field.filter((item, key) => copyLanguageCodes.includes(key)) : field
)

export const getLanguageAvailabilities = createSelector(
  [getFilteredLanguageAvailabilities, getEntityLanguageAvailabilities, getParameterFromProps('templateId')],
  (filteredLAs, entityLAs, templateId) => templateId ? filteredLAs : entityLAs
)
