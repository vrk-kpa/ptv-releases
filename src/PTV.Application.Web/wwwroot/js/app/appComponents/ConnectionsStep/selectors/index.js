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
import { Map, List, Set } from 'immutable'
import { EntitySelectors } from 'selectors'
import { getApiCalls, getParameterFromProps, getFormStates, getEntitiesForIds } from 'selectors/base'
import { getEntity, getSelectedEntityId, getSelectedEntityConcreteType } from 'selectors/entities/entities'
import { getFormValues } from 'redux-form/immutable'
import { getKey, formAllTypes, entityConcreteTypesEnum, formEntityConcreteTypes } from 'enums'
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

export const getConnectionSubmiting = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn(['connectionsSubmiting', 'isFetching']) || false
)

export const getConnections = createSelector(
  getEntity,
  entity => entity.get('connections') || List()
)

const keyIn = keys => {
  const keySet = Set(keys)
  return (v, k) => keySet.has(k)
}

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
          .update('normalOpeningHours', openingHoursIds =>
            getEntitiesForIds(openingHoursEntities, openingHoursIds, List()))
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
          connection.filter(keyIn([
            'id',
            'astiDetails',
            'unificRootId',
            'name',
            'languagesAvailabilities',
            'organizationId',
            'channelType',
            'channelTypeId',
            'serviceTypeId',
            'modified',
            'modifiedBy'
          ]))
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
    connectionsByOrganizations: selectedConnections
      .filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
      .groupBy(connection => connection.get('organizationId'))
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

export const getChannelConnectionSearchIsMoreAvailable = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn(['connectionsChannelSearch', 'result', 'moreAvailable']) || false
)

export const getServiceConnectionSearchIsMoreAvailable = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn(['connectionsServiceSearch', 'result', 'moreAvailable']) || false
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

