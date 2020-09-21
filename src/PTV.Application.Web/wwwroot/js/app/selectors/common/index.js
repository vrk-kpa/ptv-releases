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
import EntitySelectors from 'selectors/entities'
import { getInTranslationLanguageCodes } from 'selectors/entities/entities'
import EnumsSelectors from 'selectors/enums'
import { getParameterFromProps, getObjectArray, getUi } from 'selectors/base'
import { getUserOrganization, getUserRoleName } from 'selectors/userInfo'
import { Map, List } from 'immutable'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { extraTypesEnum } from 'enums'

export const createEntityPropertySelector = (selector, propertyName, defaultValue = null) => createSelector(
  selector,
  entity => entity.get(propertyName) != null ? entity.get(propertyName) : defaultValue
)

export const getTranslationLanguage = createSelector(
  [EnumsSelectors.translationLanguages.getEntitiesMap, getParameterFromProps('id')],
  (translationLanguages, id) => translationLanguages.get(id) || Map()
)

export const getTranslationLanguageCode = createSelector(
  getTranslationLanguage,
  language => language.get('code') || 'fi'
)

export const getTranslationLanguageCodes = createSelector(
  EnumsSelectors.translationLanguages.getEntities,
  languages => languages.map(l => l.get('code'))
)

export const getPhoneNumberTypesObjectArray = createSelector(
  EnumsSelectors.phoneNumberTypes.getEntities,
  phoneNumberTypes => getObjectArray(phoneNumberTypes)
)

export const getUserAccessRightsGroupTypesObjectArray = createSelector(
  EnumsSelectors.userAccessRightsGroups.getEntities,
  userAccessRightsGroups => getObjectArray(userAccessRightsGroups)
)

export const getPublishingStatusDraftCode = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'draft')
    .first() || Map()).get('code') || ''
)

export const getPublishingStatusDraftId = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'draft')
    .first() || Map()).get('id') || ''
)

export const getPublishingStatusPublishedId = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'published')
    .first() || Map()).get('id') || ''
)

export const getPublishingStatusDeletedCode = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'deleted')
    .first() || Map()).get('code') || ''
)

export const getPublishingStatusDeletedId = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'deleted')
    .first() || Map()).get('id') || ''
)

export const getPublishingStatusOldPublishedId = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'oldpublished')
    .first() || Map()).get('id') || ''
)

export const getPublishingStatusModifiedId = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'modified')
    .first() || Map()).get('id') || ''
)

export const getOrganization = createSelector(
  [EntitySelectors.getEntity, getUserOrganization],
  (entity, userInfo) => entity.get('organization') || userInfo || null
)

export const getEntityLanguageAvailabilities = createCachedSelector([
  EntitySelectors.getEntityAvailableLanguages,
  getPublishingStatusModifiedId,
  getPublishingStatusPublishedId,
  EntitySelectors.languages.getEntities,
  getInTranslationLanguageCodes
], (
  languagesAvailabilities,
  modifiedStatusId,
  publishedStatusId,
  languages,
  translationLanguages
) => languagesAvailabilities
  .map(lA =>
    lA.set('newStatusId', (lA.get('statusId') === modifiedStatusId && lA.get('canBePublished')) && !translationLanguages.has(languages.getIn([lA.get('languageId'), 'code']))
      ? publishedStatusId
      : lA.get('statusId'))
      .set('code', languages.getIn([lA.get('languageId'), 'code']))
  ) || List()
)(
  (state, props) => EntitySelectors.getEntity(state, props).get('id') || ''
)

const getShowAll = createSelector([
  getUserRoleName,
  getParameterFromProps('showAll')
],
(role, showAll) => !!(role === 'Eeva' || showAll)
)

const getOrganizationCacheKey = createSelector(
  getShowAll,
  showAll => showAll && 'showAll' || 'userOnly'
)

// let uo, o, sa = null
export const getAvailableOrganizations = createSelector([
  EnumsSelectors.userOrganizations.getEntities,
  EntitySelectors.organizations.getEntities,
  getShowAll
],
(userOrganizations, organizations, showAll) =>
  // console.log(uo === userOrganizations, o === organizations, sa === showAll,
  // uo = userOrganizations, o = organizations, sa = showAll) ||

  // showAll &&
  organizations ||
  // userOrganizations ||
  // List()
  Map()
)
export const getAvailableOrganizationsJS = createCachedSelector(
  getAvailableOrganizations,
  userOrganizations => userOrganizations
)(
  getOrganizationCacheKey
)

export const getOrganizationsDisplayJS = createCachedSelector(
  getAvailableOrganizationsJS,
  organizations => organizations.map((org) => ({
    value: org.get('id'),
    label: org.get('displayName'),
    publishingStatus: org.get('publishingStatus')
  }))
)(
  getOrganizationCacheKey
)

export const getLocalizedOrganizationsJS = createTranslatedListSelector(
  getOrganizationsDisplayJS, {
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true,
    key: getOrganizationCacheKey
  })

export const getLanguageCode = createSelector(
  [getParameterFromProps('languageId'), EntitySelectors.languages.getEntities],
  (id, languages) => languages.getIn([id, 'code'])
)

export const getLanguageId = createSelector(
  [getParameterFromProps('code'), EntitySelectors.languages.getEntities],
  (code, languages) => (languages.find(language => language.get('code') === code) || Map()).get('id')
)

export const getOidParser = createSelector(
  EnumsSelectors.serverConstants.getEntities,
  serverConstants => (serverConstants.filter(x => x.get('code').toLowerCase() === 'oidparser')
    .first() || Map()).get('pattern') || ''
)

const getExtraTypeSote = createSelector(
  EntitySelectors.extraTypes.getEntities,
  extraTypes => extraTypes.find(type => type.get('code').toLowerCase() === extraTypesEnum.SOTE) || Map()
)

export const getExtraTypeSoteId = createSelector(
  getExtraTypeSote,
  extraTypeSote => extraTypeSote.get('id') || ''
)

export const getExpireOn = createSelector(
  EntitySelectors.getEntity,
  entity =>
    entity.get('expireOn') || 0
)

export const getMainNavigationIndex = createSelector(
  getUi,
  uiState => uiState.getIn(['mainNavigationIndex', 'activeIndex'])
)
