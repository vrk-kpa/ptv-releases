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
import { Map, List, OrderedSet } from 'immutable'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getApiCalls,
  getParameterFromProps,
  getFormStates,
  getEntitiesForIds,
  getUiSorting
} from 'selectors/base'
import {
  getEntity,
  getSelectedEntityId,
  getSelectedEntityType,
  getSelectedEntityConcreteType
} from 'selectors/entities/entities'
import { getFormValues } from 'redux-form/immutable'
import { getKey, formAllTypes, entityConcreteTypesEnum, formEntityConcreteTypes, entityTypesEnum } from 'enums'
import { keyIn, sortUIData } from 'util/helpers'
import { getGDConnectionsForIds } from 'appComponents/ConnectionsStep/ConnectionTags/selectors'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { createSimpleValidator as validate } from 'util/redux-form/util/validate'
import { additionalInformationValidators } from 'appComponents/ConnectionsStep/validators'
import { EditorState, convertFromRaw } from 'draft-js'

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
  'connectionType',
  'alternateName',
  'isAlternateNameUsedAsDisplayName'
]

// export const getChannelType = createSelector(
//   [EntitySelectors.channelTypes.getEntities, getParameterFromProps('input')],
//   (channelTypes, input) => {
//     const channelTypeId = input.value.get('channelTypeId')
//     return channelTypeId && channelTypes.get(channelTypeId) || Map()
//   }
// )

// export const getChannelCode = createSelector(
//   getChannelType,
//   channelType => channelType.get('code') || ''
// )

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

export const getServiceCollectionServiceIds = createSelector(
  getEntity,
  entity => entity.get('services') || List()
)

export const getServiceCollectionChannelIds = createSelector(
  getEntity,
  entity => entity.get('channels') || List()
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

const getDescription = (description) => {
  try {
    return description &&
      description.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
  } catch (e) {
    return Map()
  }
}

export const getSelectedConnections = createSelector(
  [
    getConnections,
    EntitySelectors.connections.getEntities,
    EntitySelectors.emails.getEntities,
    EntitySelectors.phoneNumbers.getEntities,
    EntitySelectors.webPages.getEntities,
    EntitySelectors.addresses.getEntities,
    EntitySelectors.openingHours.getEntities,
    EntitySelectors.dialCodes.getEntities,
    EnumsSelectors.serviceNumbers.getEnums
  ],
  (
    ids,
    connections,
    emailEntities,
    phoneEntities,
    webPageEntities,
    addressEntities,
    openingHoursEntities,
    dialCodes,
    serviceNumbers
  ) => {
    const selectedConnections = ids
      .map(id => connections.get(id))
      .map(connection => {
        // Asti details //
        const astiDetails = connection.get('astiDetails')

        // Basic Information //
        const basicInformation = Map()
          // Description //
          .set('description', getDescription(connection.getIn(['basicInformation', 'description']) || null))
          // Additioanal Information //
          .set('additionalInformation', getDescription(connection.getIn(['basicInformation', 'additionalInformation']) || null))
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
              getEntitiesForIds(phoneEntities, faxIds, List())
                .map((phone) => {
                  const dial = dialCodes.get(phone.get('dialCode'))
                  const dialCode = dial && dial.get('code') || ''
                  const phoneNumber = phone && phone.get('phoneNumber') || null  
                  const servicePrefixies = phoneNumber && serviceNumbers && serviceNumbers.filter(serviceNumber => phoneNumber.startsWith(serviceNumber))
                  const isLocalNumberParsed = phone.get('isLocalNumber') && servicePrefixies && servicePrefixies.size > 0 
                  return phone.set('wholePhoneNumber', dialCode + phone.get('phoneNumber'))
                    .set('isLocalNumberParsed', isLocalNumberParsed)
                })))
          // Phone numbers //
          .update('phoneNumbers', phonesNumbers =>
            phonesNumbers.map(phoneIds =>
              getEntitiesForIds(phoneEntities, phoneIds, List())
                .map((phone) => {
                  const dial = dialCodes.get(phone.get('dialCode'))
                  const dialCode = dial && dial.get('code') || ''
                  const phoneNumber = phone && phone.get('phoneNumber') || null  
                  const servicePrefixies = phoneNumber && serviceNumbers && serviceNumbers.filter(serviceNumber => phoneNumber.startsWith(serviceNumber))
                  const isLocalNumberParsed = phone.get('isLocalNumber') && servicePrefixies && servicePrefixies.size > 0 
                  return phone.set('wholePhoneNumber', dialCode + phone.get('phoneNumber'))
                    .set('isLocalNumberParsed', isLocalNumberParsed)
                })))
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
          // Holiday hours //
          .update('holidayHours', openingHoursIds =>
            getHolidayHours(openingHoursIds, openingHoursEntities))

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

export const getSelectedServiceCollectionServices = createSelector(
  [
    getServiceCollectionServiceIds,
    EntitySelectors.connections.getEntities
  ],
  (ids, connections) => ids.map(id => connections.get(id)) || List()
)

export const getSelectedServiceCollectionChannels = createSelector(
  [
    getServiceCollectionChannelIds,
    EntitySelectors.connections.getEntities
  ],
  (ids, connections) => ids.map(id => connections.get(id)) || List()
)

const getName = (item, languageCode) => {
  const name = item.get('name').toJS()
  const altName = item.get('alternateName') && item.get('alternateName').toJS()
  const useAltName = item.get('isAlternateNameUsedAsDisplayName') && item.get('isAlternateNameUsedAsDisplayName').toJS()
  if (useAltName) {
    useAltName.forEach(lang => {
      if (altName && altName[lang]) {
        if (name[lang]) {
          name[lang] = altName[lang]
        }
      }
    })
  }

  const result = name && (
    name[languageCode] ||
    name[Object.keys(name)[0]]
  )
  return result
}

const getServiceTypeIds = createSelector(
  getSelectedConnections,
  connections => connections.map(connection => ({ id: connection.get('serviceType') }))
)

const getTranslatedServiceTypes = createTranslatedListSelector(
  getServiceTypeIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

const getChannelTypeIds = createSelector(
  getSelectedConnections,
  connections => connections.map(connection => ({ id: connection.get('channelTypeId') }))
)

const getTranslatedChannelTypes = createTranslatedListSelector(
  getChannelTypeIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)

const getConnectionsSorting = createSelector(
  getUiSorting,
  sorting => sorting && sorting.get('selectedConnections') || Map()
)

const getConnectionsSortingColumn = createSelector(
  getConnectionsSorting,
  sorting => sorting.get('column') || 'none'
)

const getConnectionsSortingDirection = createSelector(
  getConnectionsSorting,
  sorting => sorting.get('sortDirection') || 'none'
)

const sortingColumns = [
  'name',
  'serviceType',
  'channelType',
  'modified'
]

export const getConnectionsFormInitialValues = createSelector(
  [getSelectedEntityId,
    getSelectedConnections,
    getConnectionsSortingColumn,
    getConnectionsSortingDirection,
    getContentLanguageCode,
    getTranslatedServiceTypes,
    getTranslatedChannelTypes],
  (selectedEntityId,
    selectedConnections,
    column,
    direction,
    langCode,
    serviceTypes,
    channelTypes
  ) => {
    const data = selectedConnections.map((connection, index) => {
      return {
        name: getName(connection, langCode),
        serviceType:serviceTypes.get(index).name,
        channelType:channelTypes.get(index).name,
        modified:connection.get('modified').toString(),
        connection
      }
    })
    return Map({
      id: selectedEntityId,
      selectedConnections: sortingColumns.includes(column)
        ? List(sortUIData(data.toJS(), column, direction).map(item => item.connection)).filter(connection => !connection.getIn(['astiDetails', 'isASTIConnection']))
        : selectedConnections.filter(connection => !connection.getIn(['astiDetails', 'isASTIConnection']))
    })
  }
)

export const getServiceCollectionConnectionsInitialValues = createSelector(
  [getSelectedEntityId,
    getSelectedServiceCollectionServices,
    getSelectedServiceCollectionChannels,
    getConnectionsSortingColumn,
    getConnectionsSortingDirection,
    getContentLanguageCode],
  (selectedEntityId,
    selectedServices,
    selectedChannels,
    column,
    direction,
    langCode
  ) => {
    const serviceData = selectedServices.map(connection => {
      const type = connection.get('subEntityType')
      return {
        name: getName(connection, langCode),
        serviceType: type,
        modified: connection.get('modified').toString(),
        connection
      }
    })
    const channelData = selectedChannels.map(connection => {
      const type = connection.get('subEntityType')
      return {
        name: getName(connection, langCode),
        channelType: type,
        modified: connection.get('modified').toString(),
        connection
      }
    })

    return Map({
      id: selectedEntityId,
      selectedServices: sortingColumns.includes(column)
        ? List(sortUIData(serviceData.toJS(), column, direction)
          .map(item => item.connection))
        : selectedServices,
      selectedChannels: sortingColumns.includes(column)
        ? List(sortUIData(channelData.toJS(), column, direction)
          .map(item => item.connection))
        : selectedChannels
    })
  }
)

const getAstiConnectionsSorting = createSelector(
  getUiSorting,
  sorting => sorting && sorting.get('selectedAstiConnections') || Map()
)

const getAstiConnectionsSortingColumn = createSelector(
  getAstiConnectionsSorting,
  sorting => sorting.get('column') || 'none'
)

const getAstiConnectionsSortingDirection = createSelector(
  getAstiConnectionsSorting,
  sorting => sorting.get('sortDirection') || 'none'
)

export const getASTIConnectionsFormInitialValuesForChannels = createSelector(
  [getSelectedEntityId,
    getSelectedConnections,
    getAstiConnectionsSortingColumn,
    getAstiConnectionsSortingDirection,
    getContentLanguageCode,
    getTranslatedServiceTypes,
    getTranslatedChannelTypes],
  (selectedEntityId, selectedConnections, column, direction, langCode, serviceTypes, channelTypes) => {
    const data = selectedConnections.map((connection, index) => {
      return {
        name: getName(connection, langCode),
        serviceType:serviceTypes.get(index).name,
        channelType:channelTypes.get(index).name,
        modified:connection.get('modified').toString(),
        connection
      }
    })
    const sortedConnections = sortingColumns.includes(column)
      ? List(sortUIData(data.toJS(), column, direction).map(item => item.connection)).filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
      : selectedConnections.filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
    return Map({
      id: selectedEntityId,
      isAsti: true,
      connectionsByOrganizations: (sortedConnections
        .sort((a, b) => a.get('organizationId') > b.get('organizationId') ? -1 : 1)
        .groupBy(connection => connection.get('organizationId'))).toList()
    })
  }
)

export const getASTIConnectionsFormInitialValuesForServices = createSelector(
  [getSelectedEntityId, getSelectedConnections,
    getAstiConnectionsSortingColumn,
    getAstiConnectionsSortingDirection,
    getContentLanguageCode,
    getTranslatedServiceTypes,
    getTranslatedChannelTypes],
  (selectedEntityId, selectedConnections, column, direction, langCode, serviceTypes, channelTypes) => {
    const data = selectedConnections.map((connection, index) => {
      return {
        name: getName(connection, langCode),
        serviceType:serviceTypes.get(index).name,
        channelType:channelTypes.get(index).name,
        modified:connection.get('modified').toString(),
        connection
      }
    })
    const sortedConnections = sortingColumns.includes(column)
      ? List(sortUIData(data.toJS(), column, direction).map(item => item.connection)).filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
      : selectedConnections.filter(connection => connection.getIn(['astiDetails', 'isASTIConnection']))
    return Map({
      id: selectedEntityId,
      isAsti: true,
      connectionsFlat: sortedConnections
    })
  }
)

export const getIsAnyEntityConnected = createSelector(
  getFormValues('connections'),
  formValues => formValues && formValues.get('selectedConnections').size > 0 || false
)

export const getIsAnyChannelConnected = createSelector(
  getFormValues('connections'),
  formValues => {
    const channels = formValues && formValues.get('selectedChannels')
    return channels && channels.size > 0 || false
  }
)

export const getIsAnyServiceConnected = createSelector(
  getFormValues('connections'),
  formValues => {
    const services = formValues && formValues.get('selectedServices')
    return services && services.size > 0 || false
  }
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

export const getFormSelectedConnections = createSelector(
  getFormValues('connections'),
  formValues => formValues && formValues.get('selectedConnections') || List()
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
    return suggestedChannels.concat(mergedChannels)
      .map(channel => channel.filter(keyIn(conectionFilteredKeys))).toJS() || []
  }
)

export const getServiceCollectionSearchEntities = createSelector(
  [
    getConnectionSearchIds,
    EntitySelectors.services.getEntities,
    EntitySelectors.channels.getEntities
  ],
  (ids, services, channels) => {
    const results = ids.map(id => services.get(id) || channels.get(id)) || Map()
    return results.toJS()
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

export const getChannelEntity = createSelector(
  EntitySelectors.channels.getEntity,
  entity => entity
)

export const getServiceEntity = createSelector(
  EntitySelectors.services.getEntity,
  entity => entity
)

const getSavedConnectedEntities = createSelector(
  getConnections,
  EntitySelectors.connections.getEntities,
  (ids, connections) => getEntitiesForIds(connections, ids, List())
)

export const getIsSavedAnyNonAstiConnection = createSelector(
  getSavedConnectedEntities,
  connections => connections.filter(connection =>
    !connection.getIn(['astiDetails', 'isASTIConnection'])).size > 0 || false
)

export const getHasAnyConnectedChannelValue = createSelector(
  [getFormSelectedConnections,
    getSelectedASTIChannelConnections,
    getParameterFromProps('childIndex'),
    getParameterFromProps('isAsti')
  ],
  (selectedConnections, ASTIConnections, pIdx, isAsti) => {
    const child = isAsti && ASTIConnections.get(pIdx) || selectedConnections.get(pIdx) || Map()
    const checkedValues = validate(additionalInformationValidators)(child, { dispatch: () => { } })
    return checkedValues != null && checkedValues.size > 0
  }
)

export const getHasAnyConnectedServiceValue = createSelector(
  [getFormSelectedConnections,
    getSelectedASTIServiceConnectionsGrouped,
    getParameterFromProps('childIndex'),
    getParameterFromProps('isAsti')
  ],
  (selectedConnections, ASTIConnections, pIdx, isAsti) => {
    const child = isAsti && ASTIConnections.get(pIdx) || selectedConnections.get(pIdx) || Map()
    const checkedValues = validate(additionalInformationValidators)(child, { dispatch: () => { } })
    // console.log(pIdx, 'isSomeValue', checkedValues != null && checkedValues.size > 0, 'errors', checkedValues, child.toJS())
    return checkedValues != null && checkedValues.size > 0
  }
)

export const getServiceTypeId = createSelector(
  [EntitySelectors.serviceTypes.getEntities, getParameterFromProps('subEntityType')],
  (serviceTypes, code) => {
    if (!code) {
      return null
    }
    const lowerCode = code.toLowerCase()
    const type = serviceTypes.filter(t => t.get('code').toLowerCase() === lowerCode).first() || Map()
    return type.get('id')
  }
)

export const getChannelTypeId = createSelector(
  [EntitySelectors.channelTypes.getEntities, getParameterFromProps('subEntityType')],
  (channelTypes, code) => {
    if (!code) {
      return null
    }
    const lowerCode = code.toLowerCase()
    const type = channelTypes.filter(t => t.get('code').toLowerCase() === lowerCode).first() || Map()
    return type.get('id')
  }
)
