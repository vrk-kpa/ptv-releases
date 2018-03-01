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
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getParameterFromProps, getObjectArray } from 'selectors/base'
import { getUserOrganization, getUserRoleName } from 'selectors/userInfo'
import { getTranslationItems } from 'Intl/Selectors'
import { Map, List } from 'immutable'
import { createTranslatedListSelector } from 'appComponents/Localize/selectorCreators'
import { getContentLanguageCode } from 'selectors/selections'

export const getTranslationLanguage = createSelector(
    [EnumsSelectors.translationLanguages.getEntitiesMap, getParameterFromProps('id')],
    (translationLanguages, id) => translationLanguages.get(id) || Map()
)

export const getTranslationLanguageCode = createSelector(
    getTranslationLanguage,
    language => language.get('code') || 'fi'
)

export const getPhoneNumberTypesObjectArray = createSelector(
    EnumsSelectors.phoneNumberTypes.getEntities,
    phoneNumberTypes => getObjectArray(phoneNumberTypes)
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

export const getTranslationItem = createSelector(
    [getTranslationItems, getParameterFromProps('id')],
    (texts, id) => texts.get(id) || Map()
)

export const getTranslationExists = createSelector(
    [getTranslationItem, getParameterFromProps('language')],
    (item, language) => item.getIn(['texts', language]) != null
)

export const getOrganization = createSelector(
  [EntitySelectors.getEntity, getUserOrganization],
  (entity, userInfo) => entity.get('organization') || userInfo || null
)

export const getEntityLanguageAvailabilities = createSelector([
  EntitySelectors.getEntity,
  getPublishingStatusModifiedId,
  getPublishingStatusPublishedId,
  EntitySelectors.languages.getEntities
], (
  entity,
  modifiedStatusId,
  publishedStatusId,
  languages
) => (entity.get('languagesAvailabilities') || List())
  .map(lA =>
    lA.set('newStatusId', (lA.get('statusId') === modifiedStatusId && lA.get('canBePublished'))
      ? publishedStatusId
      : lA.get('statusId'))
      .set('code', languages.getIn([lA.get('languageId'), 'code']))
  ) || List()
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
  EnumsSelectors.organizations.getEntities,
  getShowAll
],
  (userOrganizations, organizations, showAll) =>
  // console.log(uo === userOrganizations, o === organizations, sa === showAll,
  // uo = userOrganizations, o = organizations, sa = showAll) ||
  showAll &&
  organizations ||
  userOrganizations ||
  List()
)
export const getAvailableOrganizationsJS = createCachedSelector(
  getAvailableOrganizations,
  userOrganizations => userOrganizations.toJS()
)(
  getOrganizationCacheKey
)

export const getOrganizationsDisplayJS = createCachedSelector(
  getAvailableOrganizationsJS,
  organizations => organizations.map(({ id, displayName }) => ({
    value: id,
    label: displayName
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
