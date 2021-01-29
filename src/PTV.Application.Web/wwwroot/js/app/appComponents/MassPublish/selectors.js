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
import { List, Map } from 'immutable'
import { getParameterFromProps, getFormValue, getUiSortingData, getEntities, getIdFromProps } from 'selectors/base'
import { getEntity, getEntityAvailableLanguages } from 'selectors/entities/entities'
import { getPreviousLanguagesForApprovedEntities } from 'selectors/massTool'
import { sortUIData } from 'util/helpers'
import { messages } from 'Routes/Search/components/MassTool/messages'

const getReviewList = createSelector(
  getFormValue('review'),
  getParameterFromProps('approved'),
  getPreviousLanguagesForApprovedEntities,
  getParameterFromProps('showAll'),
  (review, approved, publishedLanguages, showAll) => review
    .map((x) => {
      const id = x.get('id')
      const language = x.get('language')
      const shouldPublish = publishedLanguages.hasIn([x.get('unificRootId'), x.get('languageId')])
      const autoApproved = shouldPublish && !x.get('useForReview')
      // console.log(`use for approved ${approved}: `, autoApproved, hasError, x.get('useForReview'))
      return (
        (showAll || !x.get('approved') === !approved) && x.get('useForReview')
        // !approved && autoApproved && hasError
      ) &&
      ({
        index: x.get('index'),
        step: x.get('step'),
        id,
        unificRootId: x.get('unificRootId'),
        meta: x.get('meta').toJS(),
        language,
        languageId: x.get('languageId'),
        approved: x.get('approved'),
        autoApproved,
        shouldPublish,
        rowKey: `${id}_${language}`
      }) || null
    })
    .filter(x => x) || List()
)

const getHasPublishErrors = createCachedSelector(
  getEntities,
  getIdFromProps,
  getParameterFromProps('type'),
  (entities, id, type) => (entities.getIn([type, id, 'languagesAvailabilities']) || List())
    .reduce(
      (languagesMap, lang) => languagesMap.set(lang.get('languageId'), !lang.get('canBePublished')),
      Map()
    )
)(
  getIdFromProps
)

export const getHasErrorForLanguage = createCachedSelector(
  getHasPublishErrors,
  getParameterFromProps('languageId'),
  (languages, languageId) => languages.get(languageId)
)(
  (_, { id, languageId }) => `${id}${languageId}`
)

export const getReviewListJS = createSelector(
  getReviewList,
  list => list.toArray()
)

export const getSortedReviewListJS = createSelector(
  getReviewListJS,
  getUiSortingData,
  getEntities,
  getParameterFromProps('formatMessage'),
  (reviewed, uiSortingData, entities, formatMessage) => {
    const data = reviewed.map(x => {
      const type = x.meta && x.meta.type
      const id = x.id
      const language = x.language
      const names = entities.getIn([type, id, 'name']) || Map()
      const acceptanceStatus = x.approved
        ? formatMessage(messages.itemAccepted)
        : formatMessage(messages.itemNotAccepted)
      return {
        ...x,
        ...{ name: names.get(language) },
        ...{ acceptanceStatus }
      }
    })
    const column = uiSortingData.get('column')
    const direction = uiSortingData.get('sortDirection')
    return column && sortUIData(data, column, direction) || data
  }
)

export const getReviewListCount = createSelector(
  getReviewListJS,
  list => list.length
)

export const getName = createCachedSelector(
  getEntity,
  getParameterFromProps('language'),
  (entity, language) => entity.getIn(['name', language])
)(
  (_, { id, type, language }) => `${id}${type}${language}`
)

const getLanguageStatuses = createCachedSelector(
  getEntityAvailableLanguages,
  languagesAvailabilities => languagesAvailabilities
    .reduce((langs, l) => langs.set(l.get('languageId'), l.get('statusId')), Map())
)(
  (_, { id, type }) => `${id}${type}`
)

export const getStatus = createCachedSelector(
  getLanguageStatuses,
  getParameterFromProps('languageId'),
  (statuses, languageId) => statuses.get(languageId)
)(
  (_, { id, type, languageId }) => `${id}${type}${languageId}`
)

export const getArchiveFilterDate = createSelector(
  getFormValue('publishAt'),
  dateFilter => dateFilter
)

export const getPublishFilterDate = createSelector(
  getFormValue('archiveAt'),
  dateFilter => dateFilter
)

export const getPublishingType = createSelector(
  getFormValue('timingType'),
  type => type
)
