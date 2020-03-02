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
import createCachedSelector from 're-reselect'
import { Map, Set } from 'immutable'
import { getFormValue, getIdFromProps } from 'selectors/base'
import EntitySelectors from 'selectors/entities/entities'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

const getApprovedIds = property => createSelector(
  getFormValue('review'),
  review => review
    .filter(x => x.get('approved'))
    .map(x => x.get(property || 'unificRootId'))
    .toSet()
)

export const getPreviousPublishedLanguagesForApprovedEntities = createSelector(
  EntitySelectors.previousInfos.getEntities,
  getApprovedIds('unificRootId'),
  (previousInfos, approvedIds) => previousInfos.map(
    (x, key) => approvedIds.has(key) &&
      Set(x.get('lastPublishedLanguages')) ||
      null
  )
)

const getUnapprovedEntitiesLanguages = createSelector(
  getFormValue('review'),
  getPreviousPublishedLanguagesForApprovedEntities,
  (review, publishedLanguages) =>
    review.reduce(
      (entities, langVersion) => {
        const unificRootId = langVersion.get('unificRootId')
        const published = publishedLanguages.get(unificRootId) || Set()
        return langVersion.get('approved')
          ? entities.update(
            unificRootId,
            x => (x || published).delete(langVersion.get('languageId'))
          )
          : entities
      },
      Map()
    )
)

export const getUnapprovedEntityLanguages = createCachedSelector(
  getUnapprovedEntitiesLanguages,
  getIdFromProps,
  (entities, id) => entities.get(id) || Set()
)(
  getIdFromProps
)

export const getIsPublishedUnapproved = createCachedSelector(
  getUnapprovedEntityLanguages,
  unapprovedLanguages => unapprovedLanguages.size
)(
  getIdFromProps
)

export const getIsAnyPublishedUnaprroved = createSelector(
  getUnapprovedEntitiesLanguages,
  entities => entities.some(x => x.size)
)

const getUnaprovedLanguagesForTranslation = createSelector(
  getUnapprovedEntityLanguages,
  getIdFromProps,
  (languages, id) => languages.map(id => ({ id })).toArray()
)

export const getMissingLanguagesForApproveTranslated = createTranslatedListSelector(
  getUnaprovedLanguagesForTranslation, {
    key: (_, { unificRootId }) => unificRootId,
    languageTranslationType: languageTranslationTypes.locale
  })
