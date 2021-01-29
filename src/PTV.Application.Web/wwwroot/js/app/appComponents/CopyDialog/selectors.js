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
import { getFormValues } from 'redux-form/immutable'
import { List, Map } from 'immutable'
import { getParameterFromProps } from 'selectors/base'
import { languageOrder, entityTypesEnum } from 'enums'
import Entities from 'selectors/entities/entities'
import { EntitySelectors } from 'selectors'

export const getLanguagesToCopy = createSelector(
  getFormValues('copyEntityForm'),
  formValues => formValues && formValues.get('languagesVersions') || List()
)

export const getIsCopyEnabled = createSelector(
  getLanguagesToCopy,
  languages => languages.size > 0
)

export const getLanguageCode = createSelector(
  [getParameterFromProps('languageId'), Entities.languages.getEntities],
  (id, languages) => languages.getIn([id, 'code'])
)

const getCopyLanguages = createSelector(
  [getLanguagesToCopy, Entities.languages.getEntities],
  (copyLanguages, languages) => copyLanguages
    .map(languageId => Map({
      id: languageId,
      code: languages.getIn([languageId, 'code'])
    }))
)

const getCopyLanguagesSorted = createSelector(
  getCopyLanguages,
  languages => languages
    .sort((a, b) => {
      if (languageOrder[a.get('code')] > languageOrder[b.get('code')]) return 1
      else if (languageOrder[a.get('code')] < languageOrder[b.get('code')]) return -1
      return 0
    })
)

export const getFirstCopiedLanguage = createSelector(
  getCopyLanguagesSorted,
  languages => languages.first() || Map()
)

export const getOrganizationId = (entityId, entityType) => createSelector(
  EntitySelectors[entityType].getEntities,
  entities => {
    if (entityType === entityTypesEnum.ORGANIZATIONS) {
      return entityId
    }

    const entity = entities && entities.get(entityId)
    return entity && entity.get('organization')
  }
)
