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
import { getFormValueWithPath } from 'selectors/base'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { EnumsSelectors } from 'selectors'

const getFormPublishingStatusChecks = createSelector(
  getFormValueWithPath(props => 'selectedPublishingStatuses'),
  publishingStatuses => publishingStatuses
)

const getFormPublishingStatusIds = createSelector(
  getFormPublishingStatusChecks,
  statuses => {
    return statuses.map((selected, id) => id)
  }
)

const getFormPublishingStatuses = createSelector(
  [getFormPublishingStatusIds, EnumsSelectors.publishingStatuses.getEntities],
  (ids, statuses) => {
    return statuses.filter((value, key) => ids.some(id => id === value.get('id')))
  }
)

const getTranslatedPublishingStatuses = createTranslatedListSelector(
  getFormPublishingStatuses, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getAreAllPublishingStatusesSelected = createSelector(
  getFormPublishingStatusChecks,
  statuses => statuses.every(isSelected => isSelected)
)

export const getAreNoPublishingStatusesSelected = createSelector(
  getFormPublishingStatusChecks,
  statuses => statuses.every(isSelected => !isSelected)
)

export const getSelectedPublishingStatuses = createSelector(
  [getTranslatedPublishingStatuses, getFormPublishingStatusChecks],
  (publishingStatuses, checks) => {
    const result = []
    publishingStatuses.forEach(status => {
      const id = status.get('id')
      const check = checks.get(id)
      if (check) {
        result.push({
          value: id,
          label: status.get('name')
        })
      }
    })

    return result
  }
)
