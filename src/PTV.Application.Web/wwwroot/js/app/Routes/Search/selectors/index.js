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
  getApiCalls,
  getObjectArray,
  getSelections,
  getEntitiesForIds,
  getFormValue,
  getUi
} from 'selectors/base'
import {
  getEntities
} from 'selectors/entities/entities'
import { getUserOrganization as getUserOrganizationFromHeader } from 'selectors/userInfo'
import { Map, List, OrderedSet } from 'immutable'
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { contentTypes, allContentTypesEnum, searchTypesEnum } from 'enums'
import { getDefaultDialCode } from 'util/redux-form/fields/DialCode/selectors'

export const channelTypes = {
  ELECTRONIC: 'eChannel',
  WEB_PAGE: 'webPage',
  PHONE: 'phone',
  PRINTABLE_FORM: 'printableForm',
  SERVICE_LOCATION: 'serviceLocation'
}

export const keyToEntities = {
  'service': 'services',
  'organization': 'organizations',
  'generalDescription': 'generalDescriptions',
  [channelTypes.ELECTRONIC]: 'channels',
  [channelTypes.WEB_PAGE]: 'channels',
  [channelTypes.PHONE]: 'channels',
  [channelTypes.PRINTABLE_FORM]: 'channels',
  [channelTypes.SERVICE_LOCATION]: 'channels',
  'serviceCollection': 'serviceCollections'
}

export const getSearchDomain = createSelector(
  getSelections,
  selections => 'entities' || selections.get('searchDomain') || 'services'
)

// .filter should be temporary till legacy statuses are removed //
export const getFrontPageSearch = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('frontPageSearch') || Map()
)

export const getFrontPageSearchForm = createSelector(
  getFrontPageSearch,
  frontPageSearch => frontPageSearch.get('form') || Map()
)

export const getFrontPageSearchFormResult = createSelector(
  getFrontPageSearchForm,
  frontPageSearch => frontPageSearch.get('result') || Map()
)

export const getFrontPageSearchFormIsFetching = createSelector(
  getFrontPageSearchForm,
  frontPageSearch => frontPageSearch.get('isFetching') || false
)
export const getSelectedPublishingStauses = createSelector(
  getFrontPageSearchFormResult,
  frontPageSearchForm => frontPageSearchForm.get('selectedPublishingStatuses') || List()
)

export const getUserOrganization = createSelector(
  [getFrontPageSearchFormResult, getSearchDomain],
  (frontPageSearchForm, searchDomain) => searchDomain !== 'organizations' &&
  frontPageSearchForm.get('organizationId') || ''
)

export const getPublishingStatuses = createSelector(
  EnumsSelectors.publishingStatuses.getEntities,
  publishingStatuses => publishingStatuses
    .filter(publishingStatus =>
      publishingStatus.get('code') !== 'OldPublished' &&
      publishingStatus.get('code') !== 'Modified')
)

export const getSelectedPublishingStatusesIds = createSelector(
  [getPublishingStatuses, getSelectedPublishingStauses],
  (publishingStatuses, selectedPublishingStatuses) => publishingStatuses
    .reduce((acc, curr) => acc.set(
      curr.get('id'),
      selectedPublishingStatuses.includes(curr.get('id'))
    ), Map())
)

export const isSelectedServiceContentType = createSelector(
  getFormValue('contentTypes'),
  values => values &&
    values.size &&
    values.filter(x =>
      !['generalDescription', 'service', 'serviceService', 'serviceProfessional', 'servicePermit'].includes(x)
    ).size === 0 || false
)

export const isSelectedGdContentType = createSelector(
  getFormValue('contentTypes'),
  values => values && values.size === 1 && values.contains('generalDescription') || false
)

export const getTranslatableLanguagesCodeArray = createSelector(
  EnumsSelectors.translationLanguages.getEntities,
  translatableLanguages => {
    const result = getObjectArray(translatableLanguages).map(({ code }) => {
      return code
    })
    return result
  }
)
export const getTranslatableLanguages = createSelector(
  EnumsSelectors.translationLanguages.getEntities,
  translatableLanguages => translatableLanguages.map(x => x.get('code')).toOrderedSet()
)

export const getIsFrontPageSearching = createSelector(
  [getFrontPageSearch, getSearchDomain],
  (frontPageSearch, searchDomain) => frontPageSearch.getIn([searchDomain, 'isFetching']) || false
)

const getDomainSearch = createSelector(
  [getFrontPageSearch, getSearchDomain],
  (frontPageSearch, searchDomain) => frontPageSearch.get(searchDomain) || Map()
)

const getDomainSearchResult = createSelector(
  [getFrontPageSearch, getSearchDomain],
  (frontPageSearch, searchDomain) => frontPageSearch.getIn([searchDomain, 'result']) || Map()
)

export const getDomainPreviousSearchIds = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('prevEntities') || List()
)

export const getSearchAbort = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('abort')
)

export const getDomainCurrentSearchIds = createSelector(
  [getDomainSearchResult, getSearchDomain],
  (domainSearch, searchDomain) => {
    // Results from server are always returned under different key.
    // Its possible that searchDomain will not always match key under which results are returned from server
    // const key = keyToEntities[searchDomain] || searchDomain
    return domainSearch.get('data') || List()
  }
)

export const getDomainSearchIds = createSelector(
  [getDomainPreviousSearchIds, getDomainCurrentSearchIds],
  (previousIds, currentIds) => {
    // Dirty fix for filtering non unique values //
    // (could not find root cause) //
    return OrderedSet(previousIds.concat(currentIds)).toList()
  }
)
export const getDomainSearchPageNumber = createSelector(
  getDomainSearchResult,
  domainSearch => domainSearch.get('pageNumber') || 0
)
export const getDomainSearchPageSize = createSelector(
  getDomainSearchResult,
  domainSearch => domainSearch.get('maxPageCount') || 0
)
export const getDomainSearchTotal = createSelector(
  getDomainSearchResult,
  domainSearch => domainSearch.get('count') || 0
)
export const getDomainSearchCount = createSelector(
  getDomainSearchIds,
  domainSearchIds => (domainSearchIds && domainSearchIds.count()) || 0
)

export const getDomainShowMoreAvailable = createSelector(
  getDomainSearchResult,
  domainSearch => domainSearch.get('moreAvailable') || false
)

export const getDomainSearchSkip = createSelector(
  getDomainSearchResult,
  domainSearch => domainSearch.get('skip') || 0
)
// export const getDomainSearchEntities = createSelector(
//   [getSearchDomain, getEntities],
//   (searchDomain, entities) => {
//     const key = keyToEntities['service'] || searchDomain
//     return entities.get(key) || Map()
//   }
// )

export const getDomainSearchResults = createSelector(
  [getDomainSearchIds, getEntities],
  (ids, entities) => ids.map(id => entities.get(id.get('schema')).get(id.get('id')))
)
export const getDomainSearchRows = createSelector(
  getDomainSearchResults,
  results => results.toJS()
)

export const getOrganizationsByIds = organizationsIds => createSelector(
  EntitySelectors.organizations.getEntities,
  orgEntities => getEntitiesForIds(orgEntities, List(organizationsIds), List())
)

const getFrontPageRows = state => state.get('frontPageRows') || Map()

const getFrontPageRowId = (state, ownProps) => ownProps.id || ''

const getFrontPageRow = createSelector(
  [getFrontPageRows, getFrontPageRowId],
  (frontPageRows, rowId) => frontPageRows.get(rowId) || Map()
)

const getFrontPageRowFirstAvailableLanguageCode = (state, ownProps) => {
  const languageId = Array.isArray(ownProps.languagesAvailabilities) &&
    ownProps.languagesAvailabilities.length > 0 &&
    ownProps.languagesAvailabilities[0].languageId
  const languages = EnumsSelectors.translationLanguages.getEntitiesMap(state) || Map()
  return languages.getIn([languageId, 'code']) || 'fi'
}

export const getFrontPageRowFirstAvailableLanguageId = (state, ownProps) => {
  const languageId = Array.isArray(ownProps.languagesAvailabilities) &&
    ownProps.languagesAvailabilities.length > 0 &&
    ownProps.languagesAvailabilities[0].languageId
  return languageId || ''
}

export const getFrontPageRowLanguage = createSelector(
  [getFrontPageRow, getFrontPageRowFirstAvailableLanguageCode],
  (frontPageRows, defaultLanguage) => frontPageRows.get('languageCode') || defaultLanguage
)

export const getFrontPageRowLanguageId = createSelector(
  [getFrontPageRow, getFrontPageRowFirstAvailableLanguageId],
  (frontPageRows, defaultLanguageId) => frontPageRows.get('languageId') || defaultLanguageId
)

// form values

// const isTargetGroupsAllSelected = createSelector(
//   getUi,
//   ui => ui.getIn(['targetGroupSearch', 'selectAll']) || false
// )
export const getFormTargetGroups = createSelector(
  getFormValue('targetGroups'),
  values => values || List()
)

const isTargetGroupsAllSelected = createSelector(
  getFormTargetGroups,
  targetGroups => targetGroups ? targetGroups.size === 0 : true
)

const getTargetGrougKR1 = createSelector(
  EnumsSelectors.targetGroups.getEntities,
  targetGroups => targetGroups.find(tg => tg.get('code') === 'KR1') || Map()
)

const getTargetGrougKR1Id = createSelector(
  getTargetGrougKR1,
  targetGroup => targetGroup.get('id') || ''
)

export const getIsKR1Selected = createSelector(
  [isTargetGroupsAllSelected, getFormTargetGroups, getTargetGrougKR1Id],
  (all, targetGroups, kr1Id) => all || targetGroups.includes(kr1Id) || false
)

const getTargetGrougKR2 = createSelector(
  EnumsSelectors.targetGroups.getEntities,
  targetGroups => targetGroups.find(tg => tg.get('code') === 'KR2') || Map()
)

const getTargetGrougKR2Id = createSelector(
  getTargetGrougKR2,
  targetGroup => targetGroup.get('id') || ''
)

export const getIsKR2Selected = createSelector(
  [isTargetGroupsAllSelected, getFormTargetGroups, getTargetGrougKR2Id],
  (all, targetGroups, kr2Id) => all || targetGroups.includes(kr2Id) || false
)

export const getFrontPageInitialValues = createSelector(
  getSelectedPublishingStatusesIds,
  getUserOrganizationFromHeader,
  getTranslatableLanguages,
  getDefaultDialCode,
  (selectedPublishingStatuses, userOrganization, languages, defaultDialCode) => {
    const defaultContentTypes = contentTypes.filter(type => type !== allContentTypesEnum.GENERALDESCRIPTION)
    return {
      selectedPublishingStatuses,
      organizationIds: OrderedSet([userOrganization]),
      languages,
      contentTypes: OrderedSet(defaultContentTypes),
      dialCode: defaultDialCode,
      searchType: searchTypesEnum.NAME
    }
  }
)
