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
import { getApiCalls, getParameterFromProps, getEntitiesForIds } from 'selectors/base'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getConnectionsEntity, getConnectionsMainEntity, getConnectionsActiveEntities } from 'selectors/selections'
import { getSelectedConnections } from 'appComponents/ConnectionsStep/selectors'
import { Map, List, Set, fromJS } from 'immutable'
import { getFormValues } from 'redux-form/immutable'
import Entities, { getEntity } from 'selectors/entities/entities'

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
const getChannelResults = createSelector(
  [getChannelResultIds, EntitySelectors.channels.getEntities],
  (channelResultIds, channels) =>
    channelResultIds.map(id => channels.get(id).filter(keyIn(connectionFilteredKeys))) || List()
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
const getServiceResultIds = createSelector(
  getServiceResult,
  serviceResult => serviceResult.get('data') || List()
)
const getServiceIsFetching = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn([
    'connections',
    'serviceSearch',
    'isFetching'
  ]) || false
)
const getServiceResults = createSelector(
  [getServiceResultIds, EntitySelectors.services.getEntities],
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
export const getResultsIds = createSelector(
  getResults,
  results => results.map(x => x.get('unificRootId')) || List()
)
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
  (main, services, channels, id) => main === 'services' ? services.get(id) : channels.get(id))
const getConnections = createSelector(
  getMainEntity,
  entity => entity.get('connections') || List()
)

export const getMainEntityConnections = createSelector(
  [
    getConnections,
    EntitySelectors.connections.getEntities,
    EntitySelectors.emails.getEntities,
    EntitySelectors.phoneNumbers.getEntities,
    EntitySelectors.webPages.getEntities,
    EntitySelectors.addresses.getEntities,
    EntitySelectors.openingHours.getEntities
  ],
  (
    ids,
    connections,
    emailEntities,
    phoneEntities,
    webPageEntities,
    addressEntities,
    openingHoursEntities
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
              getEntitiesForIds(phoneEntities, faxIds, List())))
          // Phone numbers //
          .update('phoneNumbers', phonesNumbers =>
            phonesNumbers.map(phoneIds =>
              getEntitiesForIds(phoneEntities, phoneIds, List())))
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
  formValues => formValues && formValues.get('connections') || Map()
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
  (usePreset, location) => usePreset && ({ type: location.state.entityType, id: location.state.entityId }) || null
)

export const getConnectionsWorkbenchInitialValues = createSelector(
  getEntity,
  getSelectedConnections,
  getConnectionsMainEntity,
  (entity, connectedEntities, mainEntityType) => {
    const connections = entity.size > 0 && [{
      astiChilds: connectedEntities.filter(connection => connection.getIn(['astiDetails', 'isASTIConnection'])),
      childs: connectedEntities.filter(connection => !connection.getIn(['astiDetails', 'isASTIConnection'])),
      mainEntity: entity
    }] || []
    return fromJS({ connections, mainEntityType })
  }
)
