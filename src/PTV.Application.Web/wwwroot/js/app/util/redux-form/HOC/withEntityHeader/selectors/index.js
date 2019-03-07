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
import entitySelector, {
  getEntityAvailableLanguages,
  getSelectedEntityType,
  getPreviousInfoVersion,
  getSelectedEntityId,
  getCanBeTranslated,
  getSelectedEntityConcreteType
} from 'selectors/entities/entities'
import {
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusModifiedId,
  getPublishingStatusPublishedId
} from 'selectors/common'
import {
  getForm,
  getFormValue,
  getParameterFromProps
} from 'selectors/base'
import {
  getContentLanguageCode,
  getContentLanguageId
} from 'selectors/selections'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getIsAccessible } from 'appComponents/Security/selectors'
import { getIsReadOnly } from 'selectors/formStates'
import {
  formTypesEnum,
  entityConcreteTypesEnum
} from 'enums'
import { List, Map } from 'immutable'
import moment from 'moment'

export const getLanguages = createSelector(
  [getEntityAvailableLanguages, entitySelector.languages.getEntities],
  (languagesAvailability, languages) => {
    const result = languagesAvailability.map(language => {
      const languageId = language.get('languageId')
      return languages.get(languageId)
    })
    return result
  }
)

export const getFormNameFieldValue = formName => createSelector(
  [getForm, getContentLanguageCode],
  (formStates, contentLanguageCode) => {
    // TBD -> unite name and path for Name field
    if (formName === formTypesEnum.ELECTRONICCHANNELFORM) {
      return formStates.getIn([formName, 'values', 'basicInfo', 'name', contentLanguageCode]) || ''
    } else {
      return formStates.getIn([formName, 'values', 'name', contentLanguageCode]) || ''
    }
  }
)

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

export const getEntityTypeId = createSelector(
  EntitySelectors.getEntity,
  (entity) => entity.get('generalDescriptionType') || null
)

export const getIsEntityArchived = createSelector(
  [getEntityPublishingStatus, getArchivedStatusesId],
  (entityPs, archivedPS) => archivedPS.includes(entityPs)
)

export const getIsEntityWithConnections = createSelector(
  getSelectedEntityType,
  entityType => (
    entityType !== 'organizations' &&
    entityType !== 'serviceLocations'
  )
)

// ENTiTY ACTiON SELECTORS START
const getSelectedLanguage = createSelector(
  getFormValue('languagesAvailabilities'),
  getContentLanguageId,
  (formLanguages, contentLanguageId) =>
    formLanguages.filter(x => x.get('languageId') === contentLanguageId).first() ||
    formLanguages.first() ||
    Map()
)

export const getSelectedLanguageCode = createSelector(
  getSelectedLanguage,
  selectedLanguage => selectedLanguage.get('code')
)

const getIsArchivedEntity = createSelector(
  getEntityPublishingStatus,
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  (entityPS, deletedStatusId, oldPublishedStatusId) =>
    entityPS === deletedStatusId || entityPS === oldPublishedStatusId
)

const getStatusId = createSelector(
  getIsArchivedEntity,
  getPublishingStatusDeletedId,
  getPublishingStatusDraftId,
  getSelectedLanguage,
  (isArchivedEntity, deletedStatusId, draftStatusId, selectedLanguage) => (
    isArchivedEntity && deletedStatusId || (selectedLanguage.get('statusId') || draftStatusId)
  )
)

const getModifiedExists = createSelector(
  getEntityPublishingStatus,
  getPreviousInfoVersion,
  getPublishingStatusPublishedId,
  (entityPS, previousInfo, publishedStatusId) => (
    !!(publishedStatusId === entityPS && previousInfo.get('lastModifiedId'))
  )
)

// LATEST MODiFiED
export const getlinkToModified = createSelector(
  getModifiedExists,
  getPreviousInfoVersion,
  (modifiedExists, previousInfo) => (
    modifiedExists && (
      Map({
        idToEntity: previousInfo.get('lastModifiedId'),
        dateOfEntity: moment(previousInfo.get('modifiedOfLastModified')).format('DD.MM.YYYY')
      }) || null
    ) || null
  )
)
// LAST PUBLiSHED
export const getlinkToPublished = createSelector(
  getEntityPublishingStatus,
  getPreviousInfoVersion,
  getPublishingStatusModifiedId,
  (entityPS, previousInfo, modifiedStatusId) => (
    modifiedStatusId === entityPS && previousInfo.get('lastPublishedId') && (
      Map({
        idToEntity: previousInfo.get('lastPublishedId'),
        dateOfEntity: moment(previousInfo.get('modifiedOfLastPublished')).format('DD.MM.YYYY')
      }) || null
    ) || null
  )
)

const getEntityExists = createSelector(
  getSelectedEntityId,
  entityId => !!entityId
)
const getIsLatestEntity = createSelector(
  getModifiedExists,
  getSelectedEntityId,
  (modifiedExists, entityId) => !!entityId && !modifiedExists
)

export const getHasUserAccessToCreateServiceVisibility = createSelector(
  [
    EnumsSelectors.generalDescriptionTypes.getEntitiesMap,
    getEntityTypeId
  ],
  (gdTypes, entityTypeId) => {
    const gdType = gdTypes.get(entityTypeId) || Map()
    return gdType && gdType.get('allowed') || false
  }
)

// CREATE SERVICE ACTION VISIBILITY
export const getCreateServiceVisibilitySelector = createSelector(
  getEntityPublishingStatus,
  getPublishingStatusPublishedId,
  getIsReadOnly,
  getEntityExists,
  getSelectedEntityConcreteType,
  getHasUserAccessToCreateServiceVisibility,
  (statusId, publishedId, isReadOnly, entityExists, entityConcreteType, hasUserAccess) => (
    hasUserAccess && isReadOnly && entityExists && entityConcreteType === entityConcreteTypesEnum.GENERALDESCRIPTION && statusId === publishedId
  )
)

// ARCHIVE ACTiONS ViSiBiLiTY
export const getArchiveEntityVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getIsArchivedEntity,
  getEntityExists,
  (isAccessible, isReadOnly, isArchivedEntity, entityExists) => (
    isAccessible && isReadOnly && entityExists && !isArchivedEntity
  )
)
export const getArchiveLanguageVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getStatusId,
  getPublishingStatusDeletedId,
  getIsLatestEntity,
  (isAccessible, isReadOnly, statusId, deletedStatusId, isLatestEntity) => (
    isAccessible && isReadOnly && isLatestEntity && statusId !== deletedStatusId
  )
)

// RESTORE ACTiONS ViSiBiLiTY
export const getRestoreEntityVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getIsArchivedEntity,
  getEntityExists,
  (isAccessible, isReadOnly, isArchivedEntity, entityExists) => (
    isAccessible && isReadOnly && entityExists && isArchivedEntity
  )
)
export const getRestoreLanguageVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getStatusId,
  getPublishingStatusDeletedId,
  getIsLatestEntity,
  getIsArchivedEntity,
  (isAccessible, isReadOnly, statusId, deletedStatusId, isLatestEntity, isArchivedEntity) => (
    isAccessible && isReadOnly && isLatestEntity && !isArchivedEntity && statusId === deletedStatusId
  )
)

// WITHDRAW ACTiONS ViSiBiLiTY
export const getWithdrawEntityVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getEntityPublishingStatus,
  getPublishingStatusPublishedId,
  getlinkToModified,
  getEntityExists,
  (isAccessible, isReadOnly, entityPS, publishedStatusId, linkToModified, entityExists) => (
    isAccessible && isReadOnly && entityExists && !linkToModified && entityPS === publishedStatusId
  )
)
export const getWithdrawLanguageVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getStatusId,
  getPublishingStatusDraftId,
  getPublishingStatusModifiedId,
  getIsLatestEntity,
  getIsArchivedEntity,
  getRestoreLanguageVisibilitySelector,
  (
    isAccessible,
    isReadOnly,
    statusId,
    draftStatusId,
    modifiedStatusId,
    isLatestEntity,
    isArchivedEntity,
    canBeRestoredLanguage
  ) => (
    isAccessible && isReadOnly && !canBeRestoredLanguage && isLatestEntity && !isArchivedEntity &&
    statusId !== draftStatusId && statusId !== modifiedStatusId
  )
)

// LiNK TO MODiFiED ACTiON ViSiBiLiTY
export const getLinkToModifiedVisibilitySelector = createSelector(
  getlinkToModified,
  (linkToModified) => (
    !!linkToModified
  )
)
// LiNK TO PUBLiSHED ACTiON ViSiBiLiTY
export const getLinkToPublishedVisibilitySelector = createSelector(
  getlinkToPublished,
  (linkToPublished) => (
    !!linkToPublished
  )
)

// TRANSLATiON ACTiON ViSiBiLiTY
export const getTranslateVisibilitySelector = createSelector(
  getIsAccessible,
  getIsReadOnly,
  getCanBeTranslated,
  getIsArchivedEntity,
  getlinkToModified,
  (isAccessible, isReadOnly, canBeTranslated, isArchivedEntity, linkToModified) => (
    isAccessible && isReadOnly && !isArchivedEntity && !linkToModified && canBeTranslated
  )
)

// COPY ACTiON ViSiBiLiTY
export const getCopyVisibilitySelector = createSelector(
  getSelectedEntityConcreteType,
  getSelectedEntityId,
  (entityConcreteType, entityId) => (
    entityConcreteType !== entityConcreteTypesEnum.ORGANIZATION &&
    entityConcreteType !== entityConcreteTypesEnum.GENERALDESCRIPTION &&
    !!entityId
  )
)

export const getLinkToModifiedOptionsSelector = createSelector(
  getlinkToModified,
  linkToModified => (
    linkToModified && { date: linkToModified.get('dateOfEntity') } || null
  )
)
export const getLinkToPublishedOptionsSelector = createSelector(
  getlinkToPublished,
  linkToPublished => (
    linkToPublished && { date: linkToPublished.get('dateOfEntity') } || null
  )
)

export const getIsEntityActionsComboVisible = createSelector(
  getShowReviewBar,
  getParameterFromProps('options'),
  getCreateServiceVisibilitySelector,
  getArchiveEntityVisibilitySelector,
  getArchiveLanguageVisibilitySelector,
  getWithdrawEntityVisibilitySelector,
  getWithdrawLanguageVisibilitySelector,
  getRestoreEntityVisibilitySelector,
  getRestoreLanguageVisibilitySelector,
  getTranslateVisibilitySelector,
  getCopyVisibilitySelector,
  (
    isInReview,
    options,
    createservice,
    archiveEntity,
    archiveLanguage,
    withdrawEntity,
    withdrawLanguage,
    restoreEntity,
    restoreLanguage,
    translate,
    copy
  ) => {
    const optionVisibilities = [
      createservice,
      archiveEntity,
      archiveLanguage,
      withdrawEntity,
      withdrawLanguage,
      restoreEntity,
      restoreLanguage,
      translate,
      copy
    ]
    return !isInReview && options.some((option, index) => optionVisibilities[index])
  }
)
// ENTiTY ACTiON SELECTORS END

export const getIsSoteOid = createSelector(
  [
    EntitySelectors.organizationTypes.getEntities,
    getFormValue('organizationType')
  ],
  (orgTypes, orgTypeId) => {
    const orgType = orgTypes.get(orgTypeId) || Map()
    const orgTypeCode = orgType.get('code')
    return orgTypeCode === 'SotePublic' || orgTypeCode === 'SotePrivate'
  }
)
