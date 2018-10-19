/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
import { getParameterFromProps, getFormValue } from 'selectors/base'
import { getEntity } from 'selectors/entities/entities'

const getReviewList = createSelector(
  getFormValue('review'),
  getParameterFromProps('approved'),
  (review, approved) => review
    .map((x, index) => !x.get('approved') === !approved && {
      index,
      id: x.get('id'),
      meta: x.get('meta').toJS(),
      language: x.get('language'),
      languageId: x.get('languageId')
    } || null)
    .filter(x => x) || List()
)

export const getReviewListJS = createSelector(
  getReviewList,
  list => list.toArray()
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
  getEntity,
  entity => entity
    .get('languagesAvailabilities')
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
