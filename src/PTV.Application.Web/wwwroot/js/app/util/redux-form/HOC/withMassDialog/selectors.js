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
import { formTypesEnum, massToolTypes, timingTypes } from 'enums'
import { getFormValues } from 'redux-form/immutable'
import { List } from 'immutable'
import { getApiCalls } from 'selectors/base'
import EntitySelectors from 'selectors/entities/entities'
import { getIsAnyPublishedUnaprroved } from 'selectors/massTool'

const getReviewedRecords = createSelector(
  getFormValues(formTypesEnum.MASSTOOLFORM),
  formValues => {
    return formValues && formValues.get('review') || List()
  }
)

export const getIsAnyApproved = createSelector(
  getReviewedRecords,
  records => records.some(record => record.get('approved'))
)

export const getLastModifiedVersionInfoIsFetching = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    formTypesEnum.MASSTOOLFORM,
    'lastModifiedInfo',
    'isFetching'
  ]) || false
)

const getSelectedItems = createSelector(
  getFormValues(formTypesEnum.MASSTOOLFORM),
  formValues => {
    return (formValues && formValues.get('selected') || List()).filter(x => x)
  }
)

export const getIsAnySelectedApproved = createSelector(
  getSelectedItems,
  items => items.some(item => item.get('approved'))
)

export const getAnySelectApprovedWithoutLastModified = createSelector(
  getSelectedItems,
  EntitySelectors.previousInfos.getEntities,
  (selected, previousInfoEntities) => {
    return selected.some(item =>
      item.get('approved') &&
      !previousInfoEntities.getIn([item.get('unificRootId'), 'lastModifiedId'])
    ) || false
  }
)

export const getIsConfirmButtonDisabled = (state, props) => {
  const { massToolType, timingType, publishAt, archiveAt } = props
  switch (massToolType) {
    case massToolTypes.PUBLISH:
      return (timingType === timingTypes.TIMED && !publishAt) ||
        !getIsAnyApproved(state, props) ||
        getIsAnyPublishedUnaprroved(state, { formName: formTypesEnum.MASSTOOLFORM })
    case massToolTypes.ARCHIVE:
      return (timingType === timingTypes.TIMED && !archiveAt) || !getIsAnySelectedApproved(state)
    case massToolTypes.COPY:
      return !getIsAnySelectedApproved(state)
    case massToolTypes.RESTORE:
      return !getAnySelectApprovedWithoutLastModified(state)
  }
  return true
}
