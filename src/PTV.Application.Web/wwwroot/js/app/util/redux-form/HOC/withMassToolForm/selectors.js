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
import {
  getUi,
  getParameterFromProps,
  getFormValue,
  getFormValueWithPath
} from 'selectors/base'
import EntitySelectors, { getEntity, getEntityAvailableLanguages,
  getPreviousInfoVersion, getPreviousInfo, getFormName } from 'selectors/entities/entities'
import { Map, List, OrderedSet } from 'immutable'
import { getReviewCurrentStep } from 'selectors/selections'
import { getFrontPageInitialValues } from 'Routes/Search/selectors'
import {
  getPublishingStatusPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusDeletedId
} from 'selectors/common'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { getIsReadOnly } from 'selectors/formStates'

const getUiApprovalPhase = createSelector(
  getUi,
  ui => ui.get('entityReview') || Map()
)

export const getIsEntityReviewed = createSelector(
  getUiApprovalPhase,
  approvalPhase => approvalPhase.get('reviewed') || false
)

export const getEntityLanguages = createSelector(
  getEntityAvailableLanguages,
  EntitySelectors.languages.getEntities,
  (languagesAvailabilities, languageEntities) => languagesAvailabilities
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
      hasTranslationOrder: entity.get('hasTranslationOrder'),
      hasError: !languages.getIn([language, 'canBePublished']),
      statusId: languages.getIn([language, 'statusId']),
      isEditable: latestKnownVersionId
        ? entity.get('id') === latestKnownVersionId
        : rootVersionInfo.getIn(['versions', entity.get('id'), 'isEditable']),
      editableVersion: latestKnownVersionId,
      unificRootId: entity.get('unificRootId')
    })
  }
)

export const getLanguageStatus = createSelector(
  getEntityLanguages,
  getParameterFromProps('language'),
  (languages, language) => languages.getIn([language, 'statusId'])
)

export const getStepIndexMaping = createSelector(
  getFormValue('review', false, formTypesEnum.MASSTOOLFORM),
  review => review &&
    review
      .filter(x => x.get('useForReview'))
      .map(x => x.get('index')) ||
    List()
)

export const getMappedStepIndex = createSelector(
  getStepIndexMaping,
  getReviewCurrentStep,
  (mapping, step) => mapping.get(step) || 0
)

export const getTotalCount = createSelector(
  // getFormValue('review'),
  getStepIndexMaping,
  review => review && review.size || 0
)

export const getApprovedCount = createSelector(
  getFormValue('review'),
  list => list && list.count(x => x.get('approved')) || 0
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

export const getInitialValuesForLatestPublishedSearch = createSelector(
  getFrontPageInitialValues,
  getPublishingStatusPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusDeletedId,
  getFormValue('organizationIds', false, formTypesEnum.FRONTPAGESEARCH),
  (values, publishStatus, draftStatus, deletedStatus, orgIds) => {
    return values.set('selectedPublishingStatuses', Map({ [publishStatus]: true, [draftStatus]: false, [deletedStatus]: false })).set('organizationIds', OrderedSet(orgIds))
  }
)

export const getInitialValuesForCopiedSearch = createSelector(
  getFrontPageInitialValues,
  getPublishingStatusPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusDeletedId,
  getFormValue('organization', false, formTypesEnum.MASSTOOLFORM),
  (values, publishStatus, draftStatus, deletedStatus, orgId) => {
    return values.set('selectedPublishingStatuses', Map({ [publishStatus]: false, [draftStatus]: true, [deletedStatus]: false })).set('organizationIds', OrderedSet([orgId]))
  }
)

export const getInitialValuesForRestoredSearch = createSelector(
  getFrontPageInitialValues,
  getPublishingStatusPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusDeletedId,
  getFormValue('organizationIds', false, formTypesEnum.FRONTPAGESEARCH),
  (values, publishStatus, draftStatus, deletedStatus, orgIds) => {
    return values.set('selectedPublishingStatuses', Map({ [publishStatus]: false, [draftStatus]: true, [deletedStatus]: false })).set('organizationIds', OrderedSet(orgIds))
  }
)

export const getCanBeItemRestore = createSelector(
  getParameterFromProps('id'),
  EntitySelectors.previousInfos.getEntities,
  (id, entities) => {
    const lastModiedExist = entities.getIn([id, 'lastModifiedId']) || false
    return !lastModiedExist
  }
)

export const getCurrentItemPath = createSelector(
  getFormValues(formTypesEnum.MASSTOOLFORM),
  getMappedStepIndex,
  (form, mappedStepIndex) => {
    const currentItem = form.getIn(['review', mappedStepIndex])
    if (!currentItem) return null
    const path = currentItem.getIn(['meta', 'path']) + '/' + currentItem.get('id')
    return path
  }
)

export const getShowReminderDialog = createSelector(
  getShowReviewBar, (state) => getIsReadOnly(state, { formName: getFormName(state) }),
  (isInReview, isReadOnly) => (isInReview && !isReadOnly) || false
)
