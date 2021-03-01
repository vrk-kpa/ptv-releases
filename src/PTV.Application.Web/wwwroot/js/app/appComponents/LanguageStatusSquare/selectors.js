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
import entitySelector from 'selectors/entities'
import {
  getPublishingStatusDraftCode,
  getPublishingStatusDeletedCode,
  getPublishingStatusDeletedId
} from 'selectors/common'
import { getIsEntityArchived } from 'util/redux-form/HOC/withEntityButtons/selectors'

export const getPublishingStatuses = createSelector(
  [
    entitySelector.languages.getEntities,
    entitySelector.publishingStatuses.getEntities,
    (_, { languageId }) => languageId || null,
    (_, { statusId }) => statusId || null,
    getPublishingStatusDraftCode,
    (_, { isArchived }) => isArchived,
    getIsEntityArchived,
    getPublishingStatusDeletedCode,
    getPublishingStatusDeletedId
  ], (
    languages,
    statuses,
    languageId,
    statusId,
    pSdraftCode,
    isArchived,
    isEntityArchived,
    pSdeletedCode,
    deletedStatusId
  ) => ({
    code: languages.getIn([languageId, 'code']) || '?',
    status: isArchived
      ? pSdeletedCode
      : isEntityArchived && typeof isArchived === 'undefined' && pSdeletedCode ||
         statuses.getIn([statusId, 'code']) || pSdraftCode,
    statusId: isArchived
      ? deletedStatusId
      : isEntityArchived && typeof isArchived === 'undefined' && deletedStatusId || statusId
  })
)
