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
import { getApiCalls, getParameterFromProps, getEntitiesForIds, getUi, getUiSortingData } from 'selectors/base'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getConnectionsEntity, getConnectionsMainEntity, getConnectionsActiveEntities } from 'selectors/selections'
import { getSelectedConnections } from 'appComponents/ConnectionsStep/selectors'
import { additionalInformationValidators } from 'appComponents/ConnectionsStep/validators'
import { Map, List, Set, fromJS, OrderedSet } from 'immutable'
import { getFormValues, getFormInitialValues } from 'redux-form/immutable'
import Entities, { getEntity } from 'selectors/entities/entities'
import { createSimpleValidator as validate } from 'util/redux-form/util/validate'
import { formTypesEnum } from 'enums'
import { getSelectedLanguage } from 'Intl/Selectors'

const keyIn = keys => {
  const keySet = Set(keys)
  return (v, k) => keySet.has(k)
}

const connectionFilteredKeys = [
  'id',
  'astiDetails',
  'unificRootId',
  'name',
  'languagesAvailabilities',
  'organizationId',
  'channelType',
  'channelTypeId',
  'serviceType',
  'modified',
  'modifiedBy',
  'connectionType',
  'alternateName',
  'isAlternateNameUsedAsDisplayName'
]

// Channel selectors //
const getChannelResult = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'channelSearch',
    'result'
  ]) || Map()
)
const getChannelResultIds = createSelector(
  getChannelResult,
  channelResult => channelResult.get('data') || List()
)
const getChannelIsFetching = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'channelSearch',
    'isFetching'
  ]) || false
)

const getPreviousChannelResultIds = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'channelSearch',
    'prevEntities'
  ]) || List()
)

export const getChannelResultCount = createSelector(
  getChannelResult,
  channelResult => channelResult.get('count') || 0
)

export const getChannelResultMoreAvailable = createSelector(
  getChannelResult,
  channelResult => channelResult.get('moreAvailable') || 0
)

export const getChannelResultPageNumber = createSelector(
  getChannelResult,
  channelResult => channelResult.get('pageNumber') || 0
)

export const getChannelSearchResultsIds = createSelector(
  [
    getChannelResultIds,
    getPreviousChannelResultIds
  ],
  (searchIds, previousIds) => {
    return OrderedSet(previousIds.concat(searchIds)).toList()
  }
)

const getChannelResults = createSelector(
  [getChannelSearchResultsIds, EntitySelectors.channels.getEntities],
  (channelResultIds, channels) =>
    channelResultIds.map(id => channels.get(id).filter(keyIn(connectionFilteredKeys))) || List()
)

const getServiceTypesTranslations = createSelector(
  [EntitySelectors.serviceTypes.getEntities, EntitySelectors.translatedItems.getEntities],
  (serviceTypes, trans) => serviceTypes.map(st => ({ id: st.get('id'), text :trans.get(st.get('id')).get('texts') }))
)
const getOrganizationTranslations = createSelector(
  [EntitySelectors.organizations.getEntities, EntitySelectors.translatedItems.getEntities],
  (organizations, trans) => organizations.map(st => ({ id: st.get('id'), text: trans.get(st.get('id')).get('texts') }))
)

const getChannelTypesTranslations = createSelector(
  [EntitySelectors.channelTypes.getEntities, EntitySelectors.translatedItems.getEntities],
  (channelTypes, trans) => channelTypes.map(st => ({ id: st.get('id'), text :trans.get(st.get('id')).get('texts') }))
)

// Service selectors //
const getServiceResult = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'serviceSearch',
    'result'
  ]) || Map()
)

const getPreviousServiceResultIds = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'serviceSearch',
    'prevEntities'
  ]) || List()
)

export const getServiceResultCount = createSelector(
  getServiceResult,
  serviceResult => serviceResult.get('count') || 0
)

export const getServiceResultMoreAvailable = createSelector(
  getServiceResult,
  serviceResult => serviceResult.get('moreAvailable') || 0
)

export const getServiceResultPageNumber = createSelector(
  getServiceResult,
  serviceResult => serviceResult.get('pageNumber') || 0
)

const getServiceResultIds = createSelector(
  getServiceResult,
  serviceResult => serviceResult.get('data') || List()
)

export const getServiceSearchResultsIds = createSelector(
  [
    getServiceResultIds,
    getPreviousServiceResultIds
  ],
  (searchIds, previousIds) => {
    return OrderedSet(previousIds.concat(searchIds)).toList()
  }
)

export const getServiceIsFetching = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'serviceSearch',
    'isFetching'
  ]) || false
)
const getServiceResults = createSelector(
  [getServiceSearchResultsIds, EntitySelectors.services.getEntities],
  (serviceResultIds, services) =>
    serviceResultIds.map(id => services.get(id)) || List()
)

export const getResults = createSelector(
  [getConnectionsEntity, getServiceResults, getChannelResults],
  (searchFor, serviceResults, channelResults) => {
    return {
      services: serviceResults,
      channels: channelResults
    }[searchFor] || List()
  }
)

const getResultTotalCount = createSelector(
  [getConnectionsEntity, getServiceResultCount, getChannelResultCount],
  (searchFor, serviceTotalcount, channelTotalcount) => {
    return {
      services: serviceTotalcount,
      channels: channelTotalcount
    }[searchFor] || 0
  }
)

export const getResultsIds = createSelector(
  getResults,
  results => results.map(x => x.get('unificRootId')) || List()
)
export const getHasResultCountOverPage = createSelector(getResultTotalCount, count => count > 100)
export const getHasResults = createSelector(getResults, results => results.size > 0)
export const getResultCount = createSelector(getResults, results => results.size || 0)
export const getResultsJS = createSelector(getResults, results => results.toJS())
export const getIsSearchingResults = createSelector(
  [getConnectionsEntity, getServiceIsFetching, getChannelIsFetching],
  (searchFor, serviceIsFetching, channelIsFetching) => {
    return {
      services: serviceIsFetching,
      channels: channelIsFetching
    }[searchFor] || false
  }
)

// main entity connections
export const getMainEntity = createSelector(
  [getConnectionsMainEntity,
    EntitySelectors.services.getEntities,
    EntitySelectors.channels.getEntities,
    getParameterFromProps('id')],
  (main, services, channels, id) => main === 'services'
    ? services.get(id) || Map()
    : channels.get(id) || Map()
)

const getConnections = createSelector(
  getMainEntity,
  entity => entity.get('connections') || List()
)

const getHolidayHours = (hours, entities) => {
  const holidays = getEntitiesForIds(entities, hours, List())
  let result = Map()
  holidays.forEach(holiday => {
    result = result.set(holiday.get('code'), Map({
      active : true,
      intervals: holiday.get('intervals'),
      type: holiday.get('isClosed') ? 'close' : 'open'
    }))
  })
  return result
}

export const getMainEntityConnections = createSelector(
  [
    getConnections,
    EntitySelectors.connections.getEntities,
    EntitySelectors.emails.getEntities,
    EntitySelectors.phoneNumbers.getEntities,
    EntitySelectors.webPages.getEntities,
    EntitySelectors.addresses.getEntities,
    EntitySelectors.openingHours.getEntities,
    EntitySelectors.dialCodes.getEntities
  ],
  (
    ids,
    connections,
    emailEntities,
    phoneEntities,
    webPageEntities,
    addressEntities,
    openingHoursEntities,
    dialCodes
  ) => {
    const selectedConnections = ids
      .map(id => connections.get(id))
      .map(connection => {
        // Asti details //
        const astiDetails = connection.get('astiDetails')
        // Basic Information //
        const basicInformation = Map()
          // Description //
          .set('description', connection.getIn(['basicInformation', 'description']))
          // Additioanal Information //
          .set('additionalInformation', connection.getIn(['basicInformation', 'additionalInformation']) || null)
          // Charge Type //
          .set('chargeType', connection.getIn(['basicInformation', 'chargeType']) || null)
        // Digital Authorization tab //
        const digitalAuthorization = Map()
          // Digital Authorizations //
          .update('digitalAuthorizations', () => {
            const digitalAuthorizations = connection.getIn(['digitalAuthorization', 'digitalAuthorizations'])
            return digitalAuthorizations.toOrderedSet()
          })
        // Contact details tab //
        const contactDetails = connection
          .get('contactDetails')
          // Emails //
          .update('emails', emails =>
            emails.map(emailIds =>
              getEntitiesForIds(emailEntities, emailIds, List())))
          // Fax numbers //
          .update('faxNumbers', faxNumbers =>
            faxNumbers.map(faxIds =>
              getEntitiesForIds(phoneEntities, faxIds, List())
                .map((phone) => {
                  const dial = dialCodes.get(phone.get('dialCode'))
                  const dialCode = dial && dial.get('code') || ''
                  return phone.set('wholePhoneNumber', dialCode + phone.get('phoneNumber'))
                    .set('isLocalNumberParsed', phone.get('isLocalNumber'))
                })))
          // Phone numbers //
          .update('phoneNumbers', phonesNumbers =>
            phonesNumbers.map(phoneIds =>
              getEntitiesForIds(phoneEntities, phoneIds, List())
                .map((phone) => {
                  const dial = dialCodes.get(phone.get('dialCode'))
                  const dialCode = dial && dial.get('code') || ''
                  return phone.set('wholePhoneNumber', dialCode + phone.get('phoneNumber'))
                    .set('isLocalNumberParsed', phone.get('isLocalNumber'))
                })))
          // WebPages //
          .update('webPages', webPages =>
            webPages.map(webpagesIds =>
              getEntitiesForIds(webPageEntities, webpagesIds, List())))
          // Postal addresses //
          .update('postalAddresses', postalAddressIds =>
            getEntitiesForIds(addressEntities, postalAddressIds, List()))
        // Opening hours //
        const openingHours = connection.get('openingHours')
          // Normal opening hours //
          .update('normalOpeningHours', openingHoursIds => {
            return getEntitiesForIds(openingHoursEntities, openingHoursIds, List())
              .update(openingHours => {
                return openingHours.map(openingHour => {
                  return openingHour
                    .update('dailyOpeningHours', dailyOpeningHours => {
                      return dailyOpeningHours.filter(dailyOpeningHour => {
                        return dailyOpeningHour !== null
                      })
                    })
                })
              })
          })
          // Holiday hours //
          .update('holidayHours', openingHoursIds =>
            getHolidayHours(openingHoursIds, openingHoursEntities))

          // Exceptional opening hours //
          .update('exceptionalOpeningHours', openingHoursIds => {
            return getEntitiesForIds(openingHoursEntities, openingHoursIds, List())
          })
          // Special opening hours //
          .update('specialOpeningHours', openingHoursIds => {
            return getEntitiesForIds(openingHoursEntities, openingHoursIds, List())
          })
        return Map({
          basicInformation,
          digitalAuthorization,
          astiDetails,
          contactDetails,
          openingHours
        }).mergeDeep(
          connection.filter(keyIn(connectionFilteredKeys))
        )
      })
    return selectedConnections || List()
  }
)

export const getMainEntitiesCount = createSelector(
  getFormValues('connectionsWorkbench'),
  formValues => formValues && formValues.get('connections').size || 0
)
export const getIsWorkbenchEmpty = createSelector(
  getMainEntitiesCount,
  count => count === 0
)

export const getMainEntities = createSelector(
  getFormValues('connectionsWorkbench'),
  formValues => formValues && formValues.get('connections') || List()
)

export const getMainEntitiesList = createSelector(
  getMainEntities,
  connections => connections.map(connection => connection.get('mainEntity')) || List()
)

export const getMainTranslatedEntities = createSelector(
  [getMainEntities, getServiceTypesTranslations, getChannelTypesTranslations],
  (connections, serviceTypes, channelTypes) => {
    const types = serviceTypes.toList().concat(channelTypes.toList())
    return connections.map(x => x.setIn(['mainEntity', 'contentType'],
      types.filter(st =>
        st.id === x.getIn(['mainEntity', 'serviceType']) ||
        st.id === x.getIn(['mainEntity', 'channelTypeId'])
      ).first().text))
  }
)

export const GetSortedServiceResult = createSelector(
  [getMainEntities,
    getServiceTypesTranslations,
    getChannelTypesTranslations,
    getOrganizationTranslations,
    getUiSortingData,
    getSelectedLanguage],
  (serviceResults, serviceTypes, channelTypes, organizations, uiSortingData, lang) => {
    const types = serviceTypes.toList().concat(channelTypes.toList())
    let result = serviceResults.map(x => x.setIn(['mainEntity', 'subEntityType'],
      types.filter(st =>
        st.id === x.getIn(['mainEntity', 'serviceType']) ||
      st.id === x.getIn(['mainEntity', 'channelTypeId'])
      ).first().text))
    result = result.map(x => x.setIn(['mainEntity', 'organization'],
      organizations.get(x.getIn(['mainEntity', 'organizationId'])).text))
    const column = uiSortingData.get('column')
    const direction = uiSortingData.get('sortDirection')
    if (column && direction) {
      const sortedResult = result.sort((a, b) => {
        const first = a.getIn(['mainEntity', column]).get(lang) || a.getIn(['mainEntity', column]).first()
        const second = b.getIn(['mainEntity', column]).get(lang) || b.getIn(['mainEntity', column]).first()
        return direction === 'asc'
          ? first.toLowerCase().localeCompare(second.toLowerCase(), lang)
          : second.toLowerCase().localeCompare(first.toLowerCase(), lang)
      })
      return sortedResult
    }
    return result
  }
)

export const GetServiceResultJS = createSelector(
  [getServiceResults, getParameterFromProps('index')],
  (serviceResults, index) => serviceResults &&
  serviceResults.size &&
  serviceResults.toArray()[index] &&
  serviceResults.toArray()[index].toJS()
)

export const getMainEntitiesIds = createSelector(
  getMainEntities,
  connections => connections.map(connection => connection.getIn(['mainEntity', 'id'])) || Map()
)

export const getMainEntityForm = createSelector(
  [getMainEntities, getParameterFromProps('id')],
  (parents, parentId) => parents.find(x => x.getIn(['mainEntity', 'id']) === parentId) || Map()
)
export const getMainEntityFormConnections = createSelector(
  getMainEntityForm,
  parent => parent.get('childs') || Map()
)

export const getMainEntityFormAstiConnections = createSelector(
  getMainEntityForm,
  parent => parent.get('astiChilds') || Map()
)

const getPublishingStatusDraft = createSelector(
  EntitySelectors.publishingStatuses.getEntities,
  publishingStatuses => publishingStatuses.find(st => st.get('code').toLowerCase() === 'draft') || Map()
)

const getPublishingStatusDraftId = createSelector(
  getPublishingStatusDraft,
  status => status.get('id') || ''
)

const getPublishingStatusPublished = createSelector(
  EntitySelectors.publishingStatuses.getEntities,
  publishingStatuses => publishingStatuses.find(st => st.get('code').toLowerCase() === 'published') || Map()
)

const getPublishingStatusPublishedId = createSelector(
  getPublishingStatusPublished,
  status => status.get('id') || ''
)

const getIsServiceLanguageFilterOn = createSelector(
  [
    getFormValues('searchServicesConnections'),
    EnumsSelectors.translationLanguages.getEntities
  ],
  (formValues, translationLanguages) => {
    const languages = formValues && formValues.get('languages') || List()
    return languages.size > 0 && languages.size < translationLanguages.size
  }
)

const getServicePublishingStatuses = createSelector(
  getFormValues('searchServicesConnections'),
  formValues => formValues && formValues.get('selectedPublishingStatuses') || Map()
)

const getIsServicePublishingStatusDraftOn = createSelector(
  [
    getServicePublishingStatuses,
    getPublishingStatusDraftId
  ],
  (statuses, id) => statuses.get(id) || false
)

const getIsServicePublishingStatusPublishedOn = createSelector(
  [
    getServicePublishingStatuses,
    getPublishingStatusPublishedId
  ],
  (statuses, id) => statuses.get(id) || false
)

const getIsServicePublishingStatusFilterOn = createSelector(
  [
    getIsServicePublishingStatusDraftOn,
    getIsServicePublishingStatusPublishedOn
  ],
  (isDraft, isPublished) => !(isDraft && isPublished) || false
)

export const getIsAnyServiceFilterOn = createSelector(
  [
    getFormValues('searchServicesConnections'),
    getIsServiceLanguageFilterOn,
    getIsServicePublishingStatusFilterOn
  ],
  (formValues, isLanguageFiltered, isStatusFiltered) => isLanguageFiltered || isStatusFiltered || formValues && (
    !!formValues.get('organizationId') ||
    !!formValues.get('serviceTypeId') ||
    !!formValues.get('serviceClasses') ||
    !!formValues.get('ontologyTerms') ||
    !!formValues.get('areaInformationTypes') ||
    !!formValues.get('targetGroups') ||
    !!formValues.get('lifeEvents') ||
    !!formValues.get('industrialClasses')
  )
)

const getIsChannelLanguageFilterOn = createSelector(
  [
    getFormValues('searchChannelsConnections'),
    EnumsSelectors.translationLanguages.getEntities
  ],
  (formValues, translationLanguages) => {
    const languages = formValues && formValues.get('languages') || List()
    return languages.size > 0 && languages.size < translationLanguages.size
  }
)

const getChannelPublishingStatuses = createSelector(
  getFormValues('searchChannelsConnections'),
  formValues => formValues && formValues.get('selectedPublishingStatuses') || Map()
)

const getIsChannelPublishingStatusDraftOn = createSelector(
  [
    getChannelPublishingStatuses,
    getPublishingStatusDraftId
  ],
  (statuses, id) => statuses.get(id) || false
)

const getIsChannelPublishingStatusPublishedOn = createSelector(
  [
    getChannelPublishingStatuses,
    getPublishingStatusPublishedId
  ],
  (statuses, id) => statuses.get(id) || false
)

const getIsChannelPublishingStatusFilterOn = createSelector(
  [
    getIsChannelPublishingStatusDraftOn,
    getIsChannelPublishingStatusPublishedOn
  ],
  (isDraft, isPublished) => !(isDraft && isPublished) || false
)

export const getIsAnyChannelFilterOn = createSelector(
  [
    getFormValues('searchChannelsConnections'),
    getIsChannelLanguageFilterOn,
    getIsChannelPublishingStatusFilterOn
  ],
  (formValues, isLanguageFiltered, isStatusFiltered) =>
    isLanguageFiltered || isStatusFiltered || formValues && (
      !!formValues.get('organizationId') ||
    !!formValues.get('channelType') ||
    !!formValues.get('areaInformationTypes'))
)

export const getIsConnectionEntityActive = createSelector(
  [
    getConnectionsActiveEntities,
    getParameterFromProps('connectionIndex')
  ],
  (activeEntities, index) => activeEntities.some(entity => entity === index) || false
)

const getChannelTypeSelector = code => createSelector(
  EntitySelectors.channelTypes.getEntities,
  channelTypes => {
    return channelTypes
      .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
  }
)

const getIdSelector = entitySelector => createSelector(
  entitySelector,
  entity => entity.get('id') || null
)
const getEChannelId = getIdSelector(getChannelTypeSelector('echannel'))
const getWebPageId = getIdSelector(getChannelTypeSelector('webpage'))
const getPrintableFormId = getIdSelector(getChannelTypeSelector('printableform'))
const getPhoneId = getIdSelector(getChannelTypeSelector('phone'))
const getServiceLocationId = getIdSelector(getChannelTypeSelector('servicelocation'))

export const getTypePriority = createSelector(
  [
    getEChannelId,
    getPrintableFormId,
    getServiceLocationId,
    getPhoneId,
    getWebPageId
  ],
  (id1, id2, id3, id4, id5) => Map({ [id1]: 0, [id2]: 1, [id3]: 2, [id4]:3, [id5]:4 }))

export const getInsertIndex = createSelector(
  [
    getParameterFromProps('typeId'),
    getTypePriority,
    getParameterFromProps('connections')
  ],
  (typeId, priority, connections) => {
    if (typeId) {
      let index = connections.findLastIndex(x => x.get('channelTypeId') === typeId)
      if (index === -1) {
        return connections.findIndex(x => priority.get(x.get('channelTypeId')) > priority.get(typeId))
      }
      return index + 1
    }
    return -1
  }
)

export const getPriorityResult = createSelector(
  [getTypePriority, getResults, getConnectionsEntity],
  (priority, result, searchFor) => {
    if (searchFor === 'channels') {
      let priorityResult = List()
      priority.map((v, k) => {
        priorityResult = priorityResult.concat(result.filter(r => r.get('channelTypeId') === k).toList())
      })
      return priorityResult
    }
    return result
  }
)
export const getAllWorkbenchProposedChannelIds = createSelector(
  getMainEntities,
  mainEntities => mainEntities.reduce((acc, curr) => {
    const suggestedChannelIds = Set(
      curr.getIn(['mainEntity', 'suggestedChannelIds']) || [])
    return suggestedChannelIds.concat(acc)
  }, Set())
)
export const getAllWorkbenchChildsUnificRootIds = createSelector(
  getMainEntities,
  mainEntities => {
    return mainEntities.reduce((acc, curr) => {
      const channelIds = Set(
        curr.get('childs').map(child => child.get('unificRootId')) || [])
      return channelIds
        .concat(acc)
    }, Set())
  }
)
export const getNotConnectedProposedChannelIds = createSelector(
  [getAllWorkbenchProposedChannelIds, getAllWorkbenchChildsUnificRootIds],
  (workbenchChannelIds, workbenchSuggestedChannelIds) => {
    return workbenchChannelIds
      .subtract(workbenchSuggestedChannelIds)
  }
)
export const getShouldSearchProposedChannels = createSelector(
  [
    getIsWorkbenchEmpty,
    getNotConnectedProposedChannelIds,
    getConnectionsMainEntity
  ],
  (
    isWorkBenchEmpty,
    notConnectedProposedChannelIds,
    mainEntity
  ) => {
    return !isWorkBenchEmpty &&
      notConnectedProposedChannelIds.size !== 0 &&
      mainEntity === 'services'
  }
)

const getUnificRootId = (_, { unificRootId }) => unificRootId
export const getIsProposedChannelSearchResults = createSelector(
  [getAllWorkbenchProposedChannelIds, getUnificRootId],
  (channelIds, unificRootId) => channelIds.contains(unificRootId)
)
const getConnectionIndex = (_, { connectionIndex }) => connectionIndex
const getConnectionProposedChannelIds = createSelector(
  [getMainEntities, getConnectionIndex],
  (connections, connectionIndex) => {
    const result = typeof connectionIndex !== 'undefined'
      ? connections.getIn([connectionIndex, 'mainEntity', 'suggestedChannelIds']) || List()
      : List()
    return result
  }
)
export const getIsProposedChannelConnectionByConnectionIndex = createSelector(
  [getConnectionProposedChannelIds, getUnificRootId],
  (channelIds, unificRootId) => channelIds.contains(unificRootId)
)

export const getLanguageCode = createSelector(
  [getParameterFromProps('id'), Entities.languages.getEntities],
  (id, languages) => languages.getIn([id, 'code'])
)

export const getChannelConnectionType = createSelector(
  [EntitySelectors.serviceChannelConnectionTypes.getEntities, getParameterFromProps('connectionType')],
  (connectionTypes, connectionTypeId) => {
    return connectionTypeId && connectionTypes.get(connectionTypeId) || Map()
  }
)

export const getChannelConnectionCode = createSelector(
  getChannelConnectionType,
  channelConnectionType => channelConnectionType.get('code') || ''
)

export const isChannelConnectionCommon = createSelector(
  getChannelConnectionCode,
  code => code.toLowerCase() === 'commonforall'
)

const getUsePresetConnections = createSelector(
  getParameterFromProps('location'),
  location => location && location.state && location.state.entityId && location.state.entityType || false
)

export const getPresetConnectionsEntityInfo = createSelector(
  getUsePresetConnections,
  getParameterFromProps('location'),
  (usePreset, location) => usePreset && ({ type: location.state.entityType, id: location.state.entityId, concreteType: location.state.concreteType }) || null
)

const getChannelTypeId = createSelector(
  EntitySelectors.channelTypes.getEntities,
  getParameterFromProps('concreteType'),
  (channelTypes, code) => {
    const cType = code && channelTypes
      .filter(chT => chT.get('code').toLowerCase() === code.toLowerCase()).first()
    return cType && cType.get('id') || null
  }
)

export const getConnectionsWorkbenchInitialValues = createSelector(
  getEntity,
  getSelectedConnections,
  getChannelTypeId,
  getParameterFromProps('concreteType'),
  (entity, connectedEntities, typeId, channelType) => {
    entity = typeId ? entity.set('channelTypeId', typeId).set('channelType', channelType) : entity
    const connections = entity.size > 0 && [{
      astiChilds: connectedEntities.filter(connection => connection.getIn(['astiDetails', 'isASTIConnection'])),
      childs: connectedEntities.filter(connection => !connection.getIn(['astiDetails', 'isASTIConnection'])),
      mainEntity: entity
    }] || []
    return fromJS({ connections })
  }
)

export const getHasAnyConnectedChildValue = createSelector(
  [getMainEntities,
    getParameterFromProps('connectionIndex'),
    getParameterFromProps('childIndex'),
    getParameterFromProps('isAsti')
  ],
  (connections, pIdx, cIdx, isAsti) => {
    const connection = connections && connections.get(pIdx) || Map()
    const childs = isAsti && connection.get('astiChilds') || connection.get('childs') || List()
    const child = childs.get(cIdx) || Map()
    const checkedValues = validate(additionalInformationValidators)(child, { dispatch: () => { } })
    // eslint-disable-next-line max-len
    // console.log(pIdx, 'isSomeValue', checkedValues != null && checkedValues.size > 0, 'errors', checkedValues, child.toJS())
    return checkedValues != null && checkedValues.size > 0
  }
)

export const getIsConnectionsOrganizingActive = createSelector(
  getUi,
  uiState => Number.isInteger(uiState.getIn(['activeConnectionUiData', 'activeIndex']))
)

export const getIsOrganizingActiveForConnection = createSelector(
  getUi,
  getParameterFromProps('connectionIndex'),
  (uiState, connectionIndex) => Number.isInteger(connectionIndex) &&
    connectionIndex === uiState.getIn(['activeConnectionUiData', 'activeIndex'])
)

export const getConnectionsInitialOrder = createSelector(
  getUi,
  uiState => uiState.getIn(['activeConnectionUiData', 'order'])
)

export const getConnectionsUISorting = createSelector(
  getUi,
  uiState => uiState.getIn(['uiData', 'sorting']) || Map()
)

export const getConnectionsUIPageNumber = createSelector(
  getUi,
  uiState => uiState.getIn(['uiData', 'currentPage']) || 0
)

export const getOrganizationConnectionsActiveTab = createSelector(
  getUi,
  uiState => uiState.getIn(['connectionsTabIndex', 'activeConnectionsTabIndex'])
)

export const getConnectionsUISortingByIndex = createSelector(
  getUi,
  getOrganizationConnectionsActiveTab,
  getParameterFromProps('connectionIndex'),
  getParameterFromProps('isAsti'),
  (uiState, connectionsActiveTab, connectionIndex, isAsti) => {
    const key = isAsti && `connectionASTI${connectionIndex}` || `connection${connectionIndex}`
    return uiState.getIn(['uiData', 'sorting', key]) || Map()
  }
)

const getCurrentConnectionsForIndex = createSelector(
  getFormValues(formTypesEnum.CONNECTIONSWORKBENCH),
  getParameterFromProps('connectionIndex'),
  (formValues, connectionIndex) =>
    formValues.getIn(['connections', connectionIndex, 'childs']) || List()
)

export const getInitialConnectionsForIndex = createSelector(
  getFormInitialValues(formTypesEnum.CONNECTIONSWORKBENCH),
  getParameterFromProps('connectionIndex'),
  (formValues, connectionIndex) =>
    formValues.getIn(['connections', connectionIndex, 'childs']) || List()
)

export const getHasOrderChanged = createSelector(
  getCurrentConnectionsForIndex,
  getInitialConnectionsForIndex,
  (current, initial) => current
    .map(item => item.get('id'))
    .some((id, index) => id !== initial.getIn([index, 'id']))
)

const getConnectionServiceSearch = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'serviceSearch'
  ]) || Map()
)

export const getConnectionServiceSearchAbort = createSelector(
  getConnectionServiceSearch,
  search => search.get('abort')
)

const getConnectionServiceSearchRequestProps = createSelector(
  getConnectionServiceSearch,
  search => search.get('requestProps') || Map()
)

const getConnectionServiceSearchRequestPropsIsWorkbench = createSelector(
  getConnectionServiceSearchRequestProps,
  search => search.get('isWorkbench') || false
)

export const getIsWorkbenchLoading = createSelector(
  [getConnectionServiceSearch, getConnectionServiceSearchRequestPropsIsWorkbench],
  (search, isWorkbench) => isWorkbench && search.get('isFetching') || false
)
