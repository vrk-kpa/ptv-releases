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
import { Map, List, OrderedSet } from 'immutable'
import { EntitySelectors } from 'selectors'
import {
  getApiCalls,
  getParameterFromProps,
  getFormStates,
  getEntitiesForIds
} from 'selectors/base'
import {
  getEntity,
  getSelectedEntityId,
  getSelectedEntityType,
  getSelectedEntityConcreteType
} from 'selectors/entities/entities'
import { getFormValues } from 'redux-form/immutable'
import { getKey, formAllTypes, entityConcreteTypesEnum, formEntityConcreteTypes, entityTypesEnum } from 'enums'
import { keyIn } from 'util/helpers'
import { getGDConnectionsForIds } from 'appComponents/ConnectionsStep/ConnectionTags/selectors'

const conectionFilteredKeys = [
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
  'connectionType'
]

export const getChannelType = createSelector(
  [EntitySelectors.channelTypes.getEntities, getParameterFromProps('input')],
  (channelTypes, input) => {
    const channelTypeId = input.value.get('channelTypeId')
    return channelTypeId && channelTypes.get(channelTypeId) || Map()
  }
)

export const getChannelCode = createSelector(
  getChannelType,
  channelType => channelType.get('code') || ''
)

export const getChannelConnectionType = createSelector(
  [EntitySelectors.serviceChannelConnectionTypes.getEntities, getParameterFromProps('input')],
  (connectionTypes, input) => {
    const connectionTypeId = input.value.get('connectionType')
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

export const getConnectionSubmiting = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn(['connectionsSubmiting', 'isFetching']) || false
)

export const getConnections = createSelector(
  getEntity,
  entity => entity.get('connections') || List()
)

export const getSelectedConnections = createSelector(
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
            const digitalAuthorizations = connection.getIn(['digitalAuthorization', 'digitalAuthorizations']) || Map()
            return digitalAuthorizations.toOrderedSet()
          })
        // Contact details tab //
        const contactDetails = connection.get('contactDetails') && connection
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
        const openingHours = connection.get('openingHours') && connection
          .get('openingHours')
          // Normal opening hours //
          .update('normalOpeningHours', openingHoursIds =>
            getEntitiesForIds(openingHoursEntities, openingHoursIds, List())
              .map(oh => oh.update('dailyOpeningHours', days => days.filter(x => x))))
          // Exceptional opening hours //
          .update('exceptionalOpeningHours', openingHoursIds =>
            getEntitiesForIds(openingHoursEntities, openingHoursIds, List()))
          // Special opening hours //
          .update('specialOpeningHours', openingHoursIds =>
            getEntitiesForIds(openingHoursEntities, openingHoursIds, List()))
        return Map({
          basicInformation,
          digitalAuthorization,
          astiDetails,
          contactDetails,
          openingHours
        }).mergeDeep(
          connection.filter(keyIn(conectionFilteredKeys))
        )
      })
    return selectedConnections || List()
  }
)

export const getConnectionsFormInitialValues = createSelector(
  [getSelectedEntityId, getSelectedConnections],
  (selectedEntityId, selectedConnections) => Map({
    id: selectedEntityId,
    selectedConnections: selectedConnections
      .filter(connection => !connection.getIn(['astiDetails', 'isASTIConnection']))
  })
)

export const getASTIConnectionsFormInitialValuesForChannels = createSelector(
  [getSelectedEntityId, getSelectedConnections],
  (selectedEntityId, selectedConnections) => Map({
    id: selectedEntityId,
    isAsti: true,
    connectionsByOrganizations: (selectedConnections
      .filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
      .groupBy(connection => connection.get('organizationId'))).toList()
  })
)

export const getASTIConnectionsFormInitialValuesForServices = createSelector(
  [getSelectedEntityId, getSelectedConnections],
  (selectedEntityId, selectedConnections) => Map({
    id: selectedEntityId,
    isAsti: true,
    connectionsFlat: selectedConnections
      .filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
  })
)

export const isConnectionRowReadOnly = index => createSelector(
  getFormStates,
  formStates => {
    const value = formStates.getIn(['connections', 'readOnlyIndex', index])
    return typeof value === 'undefined'
      ? true // If we have no value start as readOnly
      : value
  }
)
export const getIsAnyEntityConnected = createSelector(
  getFormValues('connections'),
  formValues => formValues && formValues.get('selectedConnections').size > 0 || false
)
export const isConnectionsReadOnly = createSelector(
  getFormStates,
  formStates => {
    const value = formStates.getIn(['connections', 'readOnly'])
    return typeof value === 'undefined'
      ? true // If we have no value start as readOnly
      : value
  }
)

export const getAstiTypeEntity = createSelector(
  [EntitySelectors.astiTypes.getEntities, getParameterFromProps('astiTypeId')],
  (astiTypes, id) => {
    return astiTypes && astiTypes.get(id) || Map()
  }
)

export const getConnectionCountByOrganization = organizationId => createSelector(
  getFormValues('ASTIConnections'),
  formValues => formValues.getIn(['connectionsByOrganizations', organizationId]).size
)

// ASTI channel connections at SERVICE
const getSelectedASTIChannelConnections = createSelector(
  getFormValues('ASTIConnections'),
  formValues => (formValues && formValues.get('connectionsFlat')) || List()
)
const getSelectedASTIChannelConnectionsIds = createSelector(
  getSelectedASTIChannelConnections,
  selectedConnections => (
    selectedConnections &&
    selectedConnections.map(connection => connection.get('id'))
  ) || List()
)
export const getIsChannelAttachedAsASTI = id => createSelector(
  getSelectedASTIChannelConnectionsIds,
  selectedIds => (
    selectedIds &&
    selectedIds.some(selectedId => selectedId === id)
  ) || false
)

// ASTI service connections at CHANNEL
const getSelectedASTIServiceConnectionsGrouped = createSelector(
  getFormValues('ASTIConnections'),
  formValues => (formValues && formValues.get('connectionsByOrganizations')) || List()
)
const getSelectedASTIServiceConnections = createSelector(
  getSelectedASTIServiceConnectionsGrouped,
  groupedConnections =>
    groupedConnections.reduce((acc, curr) =>
      acc.concat(curr), List()
    ) || List()
)
const getSelectedASTIServiceConnectionsIds = createSelector(
  getSelectedASTIServiceConnections,
  selectedConnections => (
    selectedConnections &&
    selectedConnections.map(connection => connection.get('id'))
  ) || List()
)
export const getIsServiceAttachedAsASTI = id => createSelector(
  getSelectedASTIServiceConnectionsIds,
  selectedIds => (
    selectedIds &&
    selectedIds.some(selectedId => selectedId === id)
  ) || false
)

export const getFormNameByEntityConcreteType = createSelector(
  [
    getParameterFromProps('entityConcreteType'),
    getSelectedEntityConcreteType
  ],
  (
    entityTypeProps,
    entityType
  ) => {
    return entityTypeProps === entityConcreteTypesEnum.SERVICE
      ? entityType && getKey(formEntityConcreteTypes, entityType.toLowerCase()) || ''
      : entityTypeProps && getKey(formAllTypes, entityTypeProps.toLowerCase()) || ''
  }
)

export const getIsAnyASTIChannelConnected = createSelector(
  getFormValues('ASTIConnections'),
  formValues => formValues && formValues.get('connectionsFlat') && formValues.get('connectionsFlat').size > 0 || false
)

export const getIsAnyASTIServiceConnected = createSelector(
  getSelectedASTIServiceConnectionsIds,
  ids => ids.size > 0 || false
)

export const getApiCallByKey = createSelector(
  [getApiCalls, getParameterFromProps('entryKey')],
  (apiCalls, entryKey) => apiCalls.get(entryKey) || Map()
)

export const getConnectionSearchIsFetching = createSelector(
  getApiCallByKey,
  search => search.get('isFetching') || false
)

export const getConnectionSearchResult = createSelector(
  getApiCallByKey,
  apiCalls => apiCalls.get('result') || Map()
)

export const getConnectionSearchReturnedPageNumber = createSelector(
  getApiCallByKey,
  apiCall => apiCall.get('pageNumber') || 0
)

export const getConnectionSearchIsMoreAvailable = createSelector(
  getConnectionSearchResult,
  result => result.get('moreAvailable') || false
)

export const getConnectionSearchMaxPageCount = createSelector(
  getConnectionSearchResult,
  result => result.get('maxPageCount') || 0
)

export const getConnectionSearchTotal = createSelector(
  getConnectionSearchResult,
  result => result.get('count') || 0
)

export const getConnectionSearchNextPageNumber = createSelector(
  getConnectionSearchResult,
  result => {
    let currentPageNumber = result.get('pageNumber')
    return currentPageNumber ? currentPageNumber + 1 : 1
  }
)

export const getConnectionSearchCurrentIds = createSelector(
  getApiCallByKey,
  apiCall => {
    return (
      apiCall.hasIn(['result', 'data']) &&
      apiCall.getIn(['result', 'data'])
    ) || List()
  }
)
export const getConnectionSearchPreviousIds = createSelector(
  getApiCallByKey,
  apiCall => {
    return (
      apiCall.has('prevEntities') &&
      apiCall.get('prevEntities')
    ) || List()
  }
)

const getShouldShowSuggestedChannels = createSelector(
  getFormValues('searchConnectionsForm'),
  connectionsForm => connectionsForm && connectionsForm.get('shouldShowSuggestedChannels')
)
const getSuggestedChannels = createSelector(
  [
    getShouldShowSuggestedChannels,
    getEntity,
    getSelectedEntityType,
    EntitySelectors.generalDescriptions.getEntities,
    EntitySelectors.connections.getEntities,
    getGDConnectionsForIds
  ],
  (
    shouldShowSuggestedChannels,
    entity,
    entityType,
    generalDescriptions,
    connectionEntities,
    suggestedChannelIds
  ) => {
    if (!shouldShowSuggestedChannels || entityType !== 'services') {
      return List()
    }
    const generalDescriptionOutputId = entity
      .get('generalDescriptionOutput')
    const connectionIds = generalDescriptions
      .getIn([generalDescriptionOutputId, 'connections']) || List()
    const connections = connectionIds
      .map(connectionId => connectionEntities.get(connectionId))
      .map(connection => connection.set('isSuggested', true))
    return connections || List()
  }
)

export const getConnectionSearchIds = createSelector(
  [
    getConnectionSearchCurrentIds,
    getConnectionSearchPreviousIds
  ],
  (currentIds, previousIds) => {
    return OrderedSet(previousIds.concat(currentIds)).toList()
  }
)

export const getConnectionSearchEntities = createSelector(
  [
    getConnectionSearchIds,
    EntitySelectors.getEntities,
    getParameterFromProps('entityType'),
    getSuggestedChannels
  ],
  (ids, entities, entityType, suggestedChannels) => {
    if (suggestedChannels.size === 0) {
      const concreteEntities = entities.get(entityType)
      return ids.map(id => concreteEntities.get(id).filter(keyIn(conectionFilteredKeys))).toJS() || []
    }
    const mergedChannels = ids.toOrderedSet()
      .subtract(suggestedChannels.map(channel => channel.get('id')).toOrderedSet())
      .map(id => entities.get(entityType).get(id))
    return suggestedChannels.concat(mergedChannels).map(channel => channel.filter(keyIn(conectionFilteredKeys))).toJS() || []
  }
)

export const getConnectionSearchCount = createSelector(
  getConnectionSearchIds,
  ids => ids.size || 0
)

export const getServiceSuggestedChannels = createSelector(
  [
    getEntity,
    EntitySelectors.connections.getEntities,
    EntitySelectors.generalDescriptions.getEntities
  ],
  (entity, connectionEntities, generalDescriptionEntities) => {
    const generalDescriptionId = entity.get('generalDescriptionId')
    if (!generalDescriptionId) {
      return List()
    }
    const generalDescription = generalDescriptionEntities.get(generalDescriptionId)
    const connectionIds = generalDescription.get('connections')
    const connections = getEntitiesForIds(connectionEntities, connectionIds, List())
      .map(connection => connection.filter(keyIn(conectionFilteredKeys)))
    return connections
  }
)

export const getIsShowSuggestedChannelsVisible = createSelector(
  getSelectedEntityType,
  selectedEntityType => selectedEntityType === entityTypesEnum.SERVICES
)
