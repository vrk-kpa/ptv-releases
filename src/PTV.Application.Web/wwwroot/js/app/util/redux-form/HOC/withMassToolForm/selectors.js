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
import {
  getUi,
  getParameterFromProps,
  getFormValue,
  getFormValueWithPath
} from 'selectors/base'
import EntitySelectors, { getEntity, getPreviousInfoVersion, getPreviousInfo } from 'selectors/entities/entities'
import { Map, List } from 'immutable'
import { getReviewCurrentStep } from 'selectors/selections'

const getUiApprovalPhase = createSelector(
  getUi,
  ui => ui.get('pageBody') || Map()
)

export const getIsBottomReached = createSelector(
  getUiApprovalPhase,
  approvalPhase => approvalPhase.get('bottomReached') || false
)

const getEntityLanguages = createSelector(
  getEntity,
  EntitySelectors.languages.getEntities,
  (entity, languageEntities) => (entity.get('languagesAvailabilities') || List())
    .reduce(
      (languages, lang) => languages.set(languageEntities.getIn([lang.get('languageId'), 'code']), lang),
      Map()
    )
)

export const getIsEditable = createSelector(
  getPreviousInfoVersion,
  versionInfo => versionInfo.get('isEditable') || false
)

const getLatestVersion = rootVersionInfo =>
  rootVersionInfo.get('lastModifiedId') || rootVersionInfo.get('lastPublishedId') || null

export const getCurrentItem = createSelector(
  getEntity,
  getParameterFromProps('language'),
  getEntityLanguages,
  getPreviousInfo,
  (entity, language, languages, rootVersionInfo) => {
    const latestKnownVersionId = getLatestVersion(rootVersionInfo)
    return Map({
      name: entity.getIn(['name', language]),
      expireOn: entity.get('expireOn'),
      hasError: !languages.getIn([language, 'canBePublished']),
      statusId: languages.getIn([language, 'statusId']),
      isEditable: latestKnownVersionId
        ? entity.get('id') === latestKnownVersionId
        : rootVersionInfo.getIn(['versions', entity.get('id'), 'isEditable']),
      editableVersion: latestKnownVersionId
    })
  }
)

export const getLanguageStatus = createSelector(
  getEntityLanguages,
  getParameterFromProps('language'),
  (languages, language) => languages.getIn([language, 'statusId'])
)

export const getTotalCount = createSelector(
  getFormValue('review'),
  review => review && review.size || 0
)

export const getShowReviewBar = createSelector(
  getReviewCurrentStep,
  step => Number.isInteger(step) && step >= 0
)

const getReviewApprovalPath = props => `${props.path}.approved`

export const getIsItemApproved = createSelector(
  getFormValueWithPath(getReviewApprovalPath),
  value => value || false
)
