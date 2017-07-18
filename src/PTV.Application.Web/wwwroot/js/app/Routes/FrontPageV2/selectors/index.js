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
  getPublishingStatusesImmutableList,
  getTranslationLanguagesObjectArray,
  getEnums,
  getApiCalls,
  getPhoneNumberTypes,
  getChargeTypes,
  getPageModeState,
  getEntities,
  getUiData,
  getTranslationLanguages,
  getArray,
  getParameterFromProps
} from 'Containers/Common/Selectors'
import {
  getTranslationItems,
  getIntlLocale
} from 'Intl/Selectors'
import {
  getTargetGroups,
  getTopTargetGroups,
  getServiceTypes,
  getServiceClasses
} from 'Containers/Services/Common/Selectors'
import { Map, List } from 'immutable'
import { keyToEntities } from '../../../Containers/Common/Enums'
import { formValueSelector } from 'redux-form/immutable'

const frontPageFormValueSelector = formValueSelector('frontPageSearch')

export const frontPageFormLanguages = state =>
  frontPageFormValueSelector(state, 'languages') || List(['fi'])

// export const getFrontPageFormLanguageWithHighestPriority = createSelector(
//   frontPageFormLanguages,
//   languages => languages.size !== 0 && languages.sortBy(language => {
//     return ({
//       fi: 0,
//       sv: 1,
//       en: 2
//     })[language]
//   }).first() || 'fi'
// )

export const getAreFrontPageDataLoaded = createSelector(
  [getPublishingStatusesImmutableList, getServiceClasses, getServiceTypes, getTargetGroups, getPhoneNumberTypes, getChargeTypes],
  (publishingStatuses, serviceClasses, serviceTypes, targetGroups, phoneNumberTypes, chargeTypes) =>
    publishingStatuses.size > 0
    && serviceClasses.size > 0
    && serviceTypes.size > 0
    && targetGroups.size > 0
    && phoneNumberTypes.size > 0
    && chargeTypes.size > 0
)

export const getServiceEntities = createSelector(
  getEntities,
  entities => entities.get('services') || Map()
)
export const getOntologyTermEntities = createSelector(
  getEntities,
  entities => entities.get('ontologyTerms') || Map()
)
export const getSearchDomain = createSelector(
  getPageModeState,
  pageModeState => {
    return pageModeState.get('searchDomain') || ''
  }
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
export const getFrontPageSearchFormIsFetching = createSelector(
  getFrontPageSearchForm,
  frontPageSearchForm => frontPageSearchForm.get('isFetching') || false
)
export const getSelectedPublishingStauses = createSelector(
  getFrontPageSearchForm,
  frontPageSearchForm => frontPageSearchForm.get('selectedPublishingStatuses') || List()
)

export const getUserOrganization = createSelector(
  [getFrontPageSearchForm, getSearchDomain],
  (frontPageSearchForm, searchDomain) => searchDomain !== 'organizations' &&
  frontPageSearchForm.get('organizationId') || ''
)

export const getPublishingStatuses = createSelector(
  getPublishingStatusesImmutableList,
  publishingStatuses => publishingStatuses
    .filter(publishingStatus => publishingStatus.get('code') !== 'OldPublished' &&
      publishingStatus.get('code') !== 'Modified')
)
export const getSelectedPublishingStatusesIds = createSelector(
  [getPublishingStatuses, getSelectedPublishingStauses],
  (publishingStatuses, selectedPublishingStatuses) => publishingStatuses
    .filter(publishingStatus => selectedPublishingStatuses.includes(publishingStatus.get('id')))
    .map(publishingStatus => publishingStatus.get('id'))
)
export const getOrganizationEntities = createSelector(
    getEntities,
    entities => entities.get('organizations') || Map()
)
export const getTopOrganizations = createSelector(
    getEnums,
    results => results.get('organizations') || List()
)
export const getOrganizationsJS = createSelector(
    [getOrganizationEntities, getTopOrganizations],
    (organizations, ids) => {
      const result = ids.map(organizationId => {
        const organization = organizations.get(organizationId)
        return {
          value: organizationId,
          label: organization.get('name')
        }
      })
      return result.toJS()
    }
)

export const getServiceClassesOrdered = createSelector(
  getServiceClasses,
  serviceClasses => serviceClasses.sortBy(sc => sc.get('name'))
)
export const getServiceClassesJS = createSelector(
  getServiceClasses,
  serviceClasses =>
    getArray(serviceClasses
      .map(serviceClass => {
        return {
          value: serviceClass.get('id'),
          label: serviceClass.get('name')
        }
      })
    )
)

export const getServiceTypesJS = createSelector(
  getServiceTypes,
  serviceTypes => getArray(serviceTypes.map(serviceType => ({
    value: serviceType.get('id'),
    label: serviceType.get('name')
  })))
)

export const getTranslatableLanguages = createSelector(
  getTranslationLanguagesObjectArray,
  translatableLanguages => translatableLanguages.map(({ id, code, name }) => ({
    id,
    value: code,
    label: name
  }))
)

export const getTranslatableLanguagesCodeArray = createSelector(
  getTranslationLanguagesObjectArray,
  translatableLanguages => {
   const result = translatableLanguages.map(({ code }) => {
    return code
  })
  return result
  }
)

export const getUiSorting = createSelector(
    getUiData,
    entities => entities.get('sorting') || Map()
)

export const getUiSortingData = createSelector(
    [getUiSorting, getParameterFromProps('contentType')],
    (sortingData, contentType) => sortingData.get(contentType) || Map()
)

export const getUiSortingDataDirection = createSelector(
    [getUiSortingData, getParameterFromProps('column')],
    (sortingData, column) => sortingData.get('column') === column ? sortingData.get('sortDirection') : 'none'
)

export const getTopTargetGroupsObjectArray = createSelector(
  [getTargetGroups, getTopTargetGroups],
  (targetGroups, ids) => {
    const result = ids.map(id => targetGroups.get(id)).map(targetGroup => {
      return {
        value: targetGroup.get('id'),
        label: targetGroup.get('name')
      }
    })
    return result.toJS()
  }
)
export const getIsFrontPageSearching = createSelector(
  [getFrontPageSearch, getSearchDomain],
  (frontPageSearch, searchDomain) => frontPageSearch.getIn([searchDomain, 'isFetching']) || false
)
export const getDomainSearch = createSelector(
  [getFrontPageSearch, getSearchDomain],
  (frontPageSearch, searchDomain) => {
    return frontPageSearch.get(searchDomain) || Map()
  }
)
export const getDomainPreviousSearchIds = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('prevEntities') || List()
)
export const getDomainCurrentSearchIds = createSelector(
  [getDomainSearch, getSearchDomain],
  (domainSearch, searchDomain) => {
    // Results from server are always returned under different key.
    // Its possible that searchDomain will not always match key under which results are returned from server
    const key = keyToEntities[searchDomain] || searchDomain
    return domainSearch.get(key) || List()
  }
)
export const getDomainSearchIds = createSelector(
  [getDomainPreviousSearchIds, getDomainCurrentSearchIds],
  (previousIds, currentIds) => {
    return previousIds.concat(currentIds) || List()
  }
)
export const getDomainSearchPageNumber = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('pageNumber') || 0
)
export const getDomainSearchPageSize = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('maxPageCount') || 0
)
export const getDomainSearchTotal = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('count') || 0
)
export const getDomainSearchCount = createSelector(
  getDomainSearchIds,
  domainSearchIds => (domainSearchIds && domainSearchIds.count()) || 0
)
export const getDomainSearchIsSubmitting = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('isFetching') || false
)

export const getDomainShowMoreAvailable = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('moreAvailable')
)

export const getDomainSearchSkip = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('skip') || 0
)
export const getDomainSearchEntities = createSelector(
  [getSearchDomain, getEntities],
  (searchDomain, entities) => {
    const key = keyToEntities[searchDomain] || searchDomain
    return entities.get(key)
  }
)

const getDomainSearchLanguage = createSelector(
  getDomainSearch,
  search => search.get('language') || 'fi'
)

export const getDomainSearchResults = createSelector(
  [getDomainSearchIds, getDomainSearchEntities, getDomainSearchLanguage, getSearchDomain],
  (ids, entities, locale, searchDomain) => {
    // TODO: Remove case for generalDescriptions when they are translated //
    const result = searchDomain === 'generalDescriptions'
      ? ids.map(id => entities.get(id))
      : ids.map(id => entities.getIn([id, locale]))
    return result
  }
)
export const getDomainSearchRows = createSelector(
  getDomainSearchResults,
  results => results.toJS()
)
export const getServiceClassesByIds = serviceClassesIds => createSelector(
  getServiceClasses,
  serviceClasses => {
    return serviceClasses
      .filter((serviceClass, serviceClassId) => serviceClassesIds.indexOf(serviceClassId) !== -1)
      .sortBy(serviceClass => serviceClass.get('name'))
      .toList()
  }
)

export const getOntologyTermsByIds = ontologyTermsIds => createSelector(
  getOntologyTermEntities,
  ontologyTerms => {
    return ontologyTerms
      .filter((ontologyTerm, ontologyTermsId) => ontologyTermsIds.indexOf(ontologyTermsId) !== -1)
      .toList()
  }
)
export const getOntologyTermNamesByIds = ontologyTermsIds => createSelector(
  [getOntologyTermsByIds(ontologyTermsIds), getTranslationItems, getIntlLocale],
  (ontologyTerms, translations, locale) => {
    return ontologyTerms.map(ontologyTerm => {
      return translations.getIn([ontologyTerm.get('translation'), 'texts', locale]) || ontologyTerm.get('name')
    })
  }
)
export const getOrganizationsByIds = organizationsIds => createSelector(
  getOrganizationEntities,
  organizations => {
    return organizations
      .filter((organization, organizationId) => organizationsIds.indexOf(organizationId) !== -1)
      .sortBy(organization => organization.get('name'))
      .toList()
  }
)
export const getOrganizationsNamesByIds = organizationsIds => createSelector(
  [getOrganizationsByIds(organizationsIds), getTranslationItems, getIntlLocale],
  (organizations, translations, locale) => {
    return organizations.map(organization => {
      return translations.getIn([organization.get('translation'), 'texts', locale]) || organization.get('name')
    })
  }
)

const getFrontPageRows = state => state.get('frontPageRows') || Map()

const getFrontPageRowId = (state, ownProps) => ownProps.id || ''

const getFrontPageRow = createSelector(
  [getFrontPageRows, getFrontPageRowId],
  (frontPageRows, rowId) => frontPageRows.get(rowId) || Map()
)

export const getFrontPageRowFirstAvailableLanguageCode = (state, ownProps) => {
  const languageId = Array.isArray(ownProps.languagesAvailabilities) &&
    ownProps.languagesAvailabilities.length > 0 &&
    ownProps.languagesAvailabilities[0].languageId
  const language = getTranslationLanguages(state).get(languageId) || Map()
  return language.get('code') || 'fi'
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

export const getServiceClassesNamesByIds = serviceClassesIds => createSelector(
  [getServiceClassesByIds(serviceClassesIds), getTranslationItems, getFrontPageRowLanguage],
  (serviceClasses, translations, locale) => {
    return serviceClasses.map(serviceClass => {
      return translations.getIn([serviceClass.get('translation'), 'texts', locale]) || serviceClass.get('name')
    })
  }
)
