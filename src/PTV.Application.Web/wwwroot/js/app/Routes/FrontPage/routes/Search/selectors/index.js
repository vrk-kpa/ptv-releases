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
  getArray,
  getApiCalls,
  getObjectArray,
  getSelections,
  getParameterFromProps,
  getUiData,
  getEntitiesForIds
} from 'selectors/base'
import {
  getEntities
} from 'selectors/entities/entities'
import {
  getTranslationItems,
  getIntlLocale
} from 'Intl/Selectors'
import { Map, List, fromJS } from 'immutable'
import { keyToEntities } from 'Containers/Common/Enums'
import { formValueSelector } from 'redux-form/immutable'
import { EnumsSelectors, EntitySelectors } from 'selectors'

const frontPageFormValueSelector = formValueSelector('frontPageSearch')

export const frontPageFormLanguages = state =>
  frontPageFormValueSelector(state, 'languages') || List(['fi'])

export const getServiceEntities = createSelector(
  EntitySelectors.services.getEntities,
  services => services
)
export const getOntologyTermEntities = createSelector(
  EntitySelectors.ontologyTerms.getEntities,
  ontologyTerms => ontologyTerms
)
export const getSearchDomain = createSelector(
  getSelections,
  selections => selections.get('searchDomain') || 'services'
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

export const getAreFrontPageDataLoaded = createSelector(
  [getFrontPageSearchForm, getFrontPageSearchFormResult],
  (frontPage, frontPageResults) => frontPage.get('areDataValid') && frontPageResults.size > 0
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
export const getOrganizationEntities = createSelector(
    EntitySelectors.organizations.getEntities,
    organizations => organizations
)
export const getTopOrganizations = createSelector(
    EnumsSelectors.organizations.getEnums,
    organizations => organizations
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
  EnumsSelectors.serviceClasses.getEntities,
  serviceClasses => serviceClasses.sortBy(sc => sc.get('name'))
)
export const getServiceClassesJS = createSelector(
  EnumsSelectors.serviceClasses.getEntities,
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
  EnumsSelectors.serviceTypes.getEntities,
  serviceTypes => getArray(serviceTypes.map(serviceType => ({
    value: serviceType.get('id'),
    label: serviceType.get('name')
  })))
)

export const getTranslatableLanguages = createSelector(
  EnumsSelectors.translationLanguages.getEntities,
  translatableLanguages => getObjectArray(translatableLanguages).map(({ id, code, name }) => ({
    id,
    value: code,
    label: name
  }))
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

export const getUiSorting = createSelector(
  getUiData,
  ui => ui.get('sorting') || Map()
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
  [EnumsSelectors.targetGroups.getEntitiesMap, EnumsSelectors.topTargetGroups.getEnums],
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
  (frontPageSearch, searchDomain) => frontPageSearch.get(searchDomain) || Map()
)

export const getDomainSearchResult = createSelector(
  [getFrontPageSearch, getSearchDomain],
  (frontPageSearch, searchDomain) => frontPageSearch.getIn([searchDomain, 'result']) || Map()
)
export const getDomainPreviousSearchIds = createSelector(
  getDomainSearch,
  domainSearch => domainSearch.get('prevEntities') || List()
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
    return previousIds.concat(currentIds) || List()
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
export const getDomainSearchEntities = createSelector(
  [getSearchDomain, getEntities],
  (searchDomain, entities) => {
    const key = keyToEntities[searchDomain] || searchDomain
    return entities.get(key)
  }
)

const getDomainSearchLanguage = createSelector(
  getDomainSearchResult,
  search => search.get('language') || 'fi'
)

export const getDomainSearchResults = createSelector(
  [getDomainSearchIds, getDomainSearchEntities],
  (ids, entities) => ids.map(id => entities.get(id))
)
export const getDomainSearchRows = createSelector(
  getDomainSearchResults,
  results => results.toJS()
)
export const getServiceClassesByIds = serviceClassesIds => createSelector(
  EntitySelectors.serviceClasses.getEntities,
  serviceClasses => getEntitiesForIds(serviceClasses,
    fromJS(serviceClassesIds),
    List()).sortBy(serviceClass => serviceClass.get('name'))
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
  EntitySelectors.organizations.getEntities,
  orgEntities => getEntitiesForIds(orgEntities, List(organizationsIds), List())
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

export const getServiceClassesNamesByIds = serviceClassesIds => createSelector(
  [getServiceClassesByIds(serviceClassesIds), getTranslationItems, getFrontPageRowLanguage],
  (serviceClasses, translations, locale) => {
    return serviceClasses.map(serviceClass => {
      return translations.getIn([serviceClass.get('translation'), 'texts', locale]) || serviceClass.get('name')
    })
  }
)
