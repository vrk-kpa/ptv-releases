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
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { getEntitiesForIdsJS, getEntitiesForIds, getJS, getApiCalls, getUiSortingData } from 'selectors/base'
import { getEntity } from 'selectors/entities/entities'
import { getContentLanguageCode } from 'selectors/selections'
import { getTranslationLanguageCodes } from 'selectors/common'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { sortUIData } from 'util/helpers'
import { entityTypesEnum } from 'enums'
import { List, Map } from 'immutable'

export const getConnectionTypeCommonForAllId = createSelector(
  EnumsSelectors.serviceChannelConnectionTypes.getEntities,
  types => (types.filter(x => x.get('code').toLowerCase() === 'commonforall')
    .first() || Map()).get('id') || ''
)

export const getConnectionTypeNotCommonId = createSelector(
  EnumsSelectors.serviceChannelConnectionTypes.getEntities,
  types => (types.filter(x => x.get('code').toLowerCase() === 'notcommon')
    .first() || Map()).get('id') || ''
)

export const getChannelConnectionType = createSelector(
  getEntity,
  entity => entity.get('connectionType')
)

export const isAstiConnectionExist = createSelector(
  getEntity,
  EntitySelectors.connections.getEntities,
  (entity, connections) => {
    const conIds = entity.get('connections')
    return !!connections.filter(con => con.getIn(['astiDetails', 'isASTIConnection']))
      .filter(con => conIds.contains(con.get('connectionId'))).size
  }
)

const getApiCall = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([entityTypesEnum.CHANNELS, 'connectedItems']) || Map()
)

export const getSearchForConnectedServicesIsFetching = createSelector(
  getApiCall,
  search => search.get('isFetching') || false
)

const getConnectedServiceIds = createSelector(
  getApiCall,
  search => search.getIn(['result', 'data']) || List()
)

const getConnectedServicesList = createSelector(
  [EntitySelectors.services.getEntities, getConnectedServiceIds],
  (services, serviceIds) => getEntitiesForIds(services, serviceIds) || []
)

export const getConnectedServices = createSelector(
  getConnectedServicesList,
  services => getJS(services, [])
)

const getServiceTypeIds = createSelector(
  getConnectedServices,
  services => services.map(service => ({ id: service.serviceType }))
)

const getTranslatedServiceTypes = createTranslatedListSelector(
  getServiceTypeIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

const getOrganizationIds = createSelector(
  getConnectedServicesList,
  services => services.map(service => service.get('organizationId'))
)

const getOrganizations = createSelector(
  getOrganizationIds,
  EntitySelectors.organizations.getEntities,
  (ids, organizations) => getEntitiesForIdsJS(organizations, ids)
)

const getTranslatedOrganizations = createTranslatedListSelector(
  getOrganizations, {
    nameAttribute: 'displayName',
    languageTranslationType: languageTranslationTypes.both
  }
)

const getFirstAvailableName = (names, langs, skipLang) => {
  const languageCodes = langs.filter(lang => lang !== skipLang)
  const firstFoundLanguage = languageCodes.find(lang => names[lang])
  return names[firstFoundLanguage]
}

export const getParsedConnectedServices = createSelector(
  [
    getConnectedServices,
    getTranslatedServiceTypes,
    getTranslatedOrganizations,
    getUiSortingData,
    getContentLanguageCode,
    getTranslationLanguageCodes
  ],
  (services, serviceTypes, organizations, uiSortingData, languageCode, languageCodes) => {
    const parsedServices = services.map((service, index) => {
      const serviceName = service.name[languageCode] ||
        getFirstAvailableName(service.name, languageCodes, languageCode) || ''
      return {
        id: service.id,
        unificRootId: service.unificRootId,
        name: serviceName,
        languagesAvailabilities: service.languagesAvailabilities,
        organization: organizations[index].displayName,
        serviceType: serviceTypes[index].name
      }
    })
    const column = uiSortingData.get('column')
    const direction = uiSortingData.get('sortDirection')
    return sortUIData(parsedServices, column, direction)
  }
)
