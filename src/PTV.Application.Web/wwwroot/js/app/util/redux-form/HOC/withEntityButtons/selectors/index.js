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
import { EntitySelectors } from 'selectors'
import { List, Map } from 'immutable'
import {
  getPublishingStatusPublishedId
} from 'selectors/common'
import { getFormValue, getApiCalls } from 'selectors/base'

export const getArchivedStatusesId = createSelector(
  EntitySelectors.publishingStatuses.getEntities,
  (publishingStatuses) => (publishingStatuses.filter(ps => {
    const status = ps.get('code').toLowerCase()
    return status === 'deleted' || status === 'oldpublished'
  }) ||
    List()).map(x => x.get('id'))
)

export const getEntityPublishingStatus = createSelector(
  EntitySelectors.getEntity,
  (entity) => entity.get('publishingStatus')
)

export const getIsEntityArchived = createSelector(
  [getEntityPublishingStatus, getArchivedStatusesId],
  (entityPs, archivedPS) => archivedPS.includes(entityPs)
)
export const isEntityOnlyEdit = createSelector(
  [EntitySelectors.getEntityAvailableLanguages, getPublishingStatusPublishedId],
  (languages, publishedId) => !languages.some(x => x.get('statusId') !== publishedId)
)

export const getLanguagesAvailabilitiesCount = createSelector(
  getFormValue('languagesAvailabilities'),
  languages => languages.size || 0
)

export const getIsCheckingUrl = createSelector(
  getApiCalls,
  apiCalls => (apiCalls.get('urlChecker') || List()).some(url => url.get('isFetching'))
)

const getUrlCheckCall = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('urlChecker') || Map()
)

export const getUrlCheckingAbort = createSelector(
  getUrlCheckCall,
  check => {
    const urls = check.filter(url => url.get('abort'))
    if (urls.size) {
      return urls.first().get('abort')
    }
    return null
  }
)
