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

import { createSelector } from 'reselect';
import { List, Map } from 'immutable';
import { getEntities, getResults, getJS, getObjectArray, getArray, getEnums, getEntitiesForIds, getOrganizationReducers, getCh,
         getParameters, getIdFromProps, getParameterFromProps, getPageModeStateForKey, getLanguageParameter, getTranslationLanguageCodes, getApiCalls } from '../../../../Common/Selectors';
import * as CommonChannelSelectors from '../../../../Channels/Common/Selectors';
import * as ServiceSelectors from '../../../../Services/Service/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';

export const getConnectedServices = createSelector(
    getEntities,
    entities => entities.get('connectedServices') || List()
);

export const getConnectedChannels = createSelector(
    getEntities,
    entities => entities.get('connectedChannels') || List()
);

export const getRelations = createSelector(
    getEntities,
    entities => entities.get('relations') || List()
);

export const getChannelRelations = createSelector(
    getEntities,
    entities => entities.get('channelRelations') || List()
);

export const getRelation = createSelector(
    getRelations,
    relations => relations.get('serviceAndChannelsId') || Map()
);

export const getRelationConnectedServicesUiIds = createSelector(
    getRelation,
    entities => entities.get('connectedServices') || List()
);

export const getIsAnyRelation = createSelector(
    getRelationConnectedServicesUiIds,
    entities => entities.size == 0
);

export const getRelationConnectedServices = createSelector(
    [getConnectedServices, getRelationConnectedServicesUiIds],
    (entities, uiIds) =>  getEntitiesForIds(entities, uiIds, List())
);

export const getRelationConnectedServicesIdsMap = createSelector(
    getRelationConnectedServices,
    entities =>  Map(entities.map(x => [x.get('service'), x.get('uiId')]))
);

export const getRelationConnectedServicesIds = createSelector(
    [getConnectedServices, getRelationConnectedServicesUiIds],
    (entities, uiIds) =>  uiIds && uiIds.size > 0 ? uiIds.map(uiId => entities.get(uiId).get('service')) : List()
);

export const getRelationConnectedServicesIdsSize = createSelector(
    getRelationConnectedServicesIds,
    connectedServicesIds => connectedServicesIds.size || 0
);

export const getRelationConnectedServicesJS = createSelector(
    getRelationConnectedServicesIds,
    connectedServices => getJS(connectedServices)
);

export const getRelationConnectedServiceEntities = createSelector(
    [getConnectedServices, getRelationConnectedServicesUiIds],
    (connectedServices, connectedServicesIds) => getEntitiesForIds(connectedServices, connectedServicesIds) || List()
);

//digital authorization
export const getDigitalAuthorizations = createSelector(
    getEntities,
    entities => entities.get('digitalAuthorizations') || Map()
);

export const getDigitalAuthorization = createSelector(
    [getDigitalAuthorizations, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getDigitalAuthorizationNameForId = createSelector(
    getDigitalAuthorization,
    digitalAuthorization => digitalAuthorization.get('name')
);

export const getFilteredDigitalAuthorizations = createSelector(
    getEntities,
    entities => entities.get('filteredDigitalAuthorizations') || Map()
);

export const getFilteredDigitalAuthorization = createSelector(
    [getFilteredDigitalAuthorizations, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
);

export const getDigitalAuthorizationChildren = createSelector(
    getDigitalAuthorization,
    entity => entity.get('children') || Map()
);

const getAllChildren = (id, digitalAuthorizations) => {
    const da = digitalAuthorizations.get(id) || Map()
    const daChildren = da.get('children') || List();
    return daChildren.size > 0 ? daChildren.concat(
        daChildren
            .map(x => getAllChildren(x, digitalAuthorizations))
            .reduce((all, curr) => all.concat(curr)))
            : daChildren;
}

export const getDigitalAuthorizationWithAllChildren = createSelector(
    [getDigitalAuthorizations, getIdFromProps],
    (entities, id) => getAllChildren(id, entities).push(id)
);

export const getFilteredDigitalAuthorizationChildren = createSelector(
    getFilteredDigitalAuthorization,
    entity => entity.get('children') || Map()
);

export const getDigitalAuthorizationWithFiltered = createSelector(
    [getDigitalAuthorization, getFilteredDigitalAuthorization],
    (entity, filtered) => entity.size > 0 ? entity : filtered
);

export const getDigitalAuthorizationsJS = createSelector(
    getDigitalAuthorizations,
    entities => getJS(entities, [])
);

export const getDigitalAuthorizationsObjectArray = createSelector(
    getDigitalAuthorizations,
    entities => getObjectArray(entities)
);

export const getTopDigitalAuthorizations = createSelector(
    getEnums,
    results => results.get('digitalAuthorizations') || List()
)
//digital authorization


//Related services
export const getService = createSelector(
    [ServiceSelectors.getServices, getParameterFromProps('id')],
    (services, id) => services.get(id) || Map()
);

export const getConnectedServiceEntities = createSelector(
    [ServiceSelectors.getServices, getRelationConnectedServicesIds],
    (entities, connectedServicesIds) => getEntitiesForIds(entities, connectedServicesIds) || List()
);

// Searched services
export const getSearchedServiceEntities = createSelector(
    [ServiceSelectors.getServices, CommonSelectors.getSearchedEntities],
    (entities, searchedServicesIds) => getEntitiesForIds(entities, searchedServicesIds) || List()
);


//Connected services
export const getConnectedServiceEntity = createSelector(
    [getConnectedServices, getIdFromProps],
    (entities, uiId) => entities.get(uiId) || Map()
);

export const getConnectedServiceEntityJS = createSelector(
    getConnectedServiceEntity,
    entity => getJS(entity)
);

export const getConnectedServiceChannelRelations = createSelector(
    getConnectedServiceEntity,
    service => service.get('channelRelations') || List()
);


//Channel relation
export const getChannelRelation = createSelector(
    [getChannelRelations, getIdFromProps],
    (entities, id) =>
    {
        return entities.get(id) || Map()
    }
);

export const getLocalizedChannelRelation = createSelector(
    [getChannelRelation, getLanguageParameter],
    (channelRelation, language) => channelRelation.get(language) || Map()
);

export const getChannelRelationCurrentTab = createSelector(
    getChannelRelation,
    channelRelation => channelRelation.get('tabId') || 0
)

export const getChargeType = createSelector(
    getChannelRelation,
    channelRelation => channelRelation.get('chargeType') || null
)

export const getChannelRelationDescription = createSelector(
    getLocalizedChannelRelation,
    channelRelation => channelRelation.get('description') || ''
)

export const getChannelRelationIsSomeDescription = createSelector(
    [getChannelRelation, getTranslationLanguageCodes],
    (channelRelation, langCodes) => {
        var data = langCodes.map(langCode =>
        {
            const localizedEntity = channelRelation.get(langCode);
            return localizedEntity ? localizedEntity.get('description') || '' : '';
        })
        return data.some(x => x != '' || false)
    }
)

export const getChannelRelationChargeTypeAdditionalInformation = createSelector(
    getLocalizedChannelRelation,
    channelRelation => channelRelation.get('chargeTypeAdditionalInformation') || ''
)

export const getChannelRelationIsSomeChargeTypeAdditionalInformation = createSelector(
    [getChannelRelation, getTranslationLanguageCodes],
    (channelRelation, langCodes) => {
        var data = langCodes.map(langCode =>
        {
            const localizedEntity = channelRelation.get(langCode);
            return localizedEntity ? localizedEntity.get('chargeTypeAdditionalInformation') || '' : '';
        })
        return data.some(x => x != '' || false)
    }
)

export const getSelectedDigitalAuthorizations = createSelector(
    getChannelRelation,
    channelRelation => channelRelation.get('digitalAuthorizations') || List()
)

export const getIsSelectedDigitalAuthorization = createSelector(
    [getSelectedDigitalAuthorizations, getParameterFromProps('digitalAuthorizationId')],
    (entities, id) => entities.includes(id)
)

//All channel relation for Relation
export const getRelationChannelRelationsOfConnectedServiceMap = createSelector(
    getRelationConnectedServiceEntities,
    connectedServices =>
    {
        return connectedServices.reduce((prev, curr) => prev.update('channelRelations', x => x.concat(curr.get('channelRelations')))) || Map();
    }
);

export const getRelationChannelRelationsOfConnectedServiceIds = createSelector(
    getRelationChannelRelationsOfConnectedServiceMap,
    entities => entities.get('channelRelations') || List()
);

export const getRelationChannelRelationsEntities = createSelector(
    [getChannelRelations, getRelationChannelRelationsOfConnectedServiceIds],
    (entities, selectedIds) => getEntitiesForIds(entities, selectedIds) || List()
);

export const getRelationConnectedChannelsMap = createSelector(
    getRelationChannelRelationsEntities,
    channelRelations =>
    {
        return channelRelations.reduce((prev, curr) => prev.update('connectedChannel', x => x.concat(curr.get('connectedChannel')))) || Map();
    }
);

//Channel Relation entities
const createChannelRelationWithServiceChannel = (channelRelation, channels) => {
    const channel = channels.get(channelRelation.get('connectedChannel')) || Map();
    return channelRelation.merge({ channelRootId: channel.get('rootId') });
}

export const getRelationConnectedChannelsWithServiceChannelEntities = createSelector(
    [getRelationChannelRelationsEntities, CommonChannelSelectors.getChannels],
    (entities, channels) => entities.map(channelRelation => createChannelRelationWithServiceChannel(channelRelation, channels))
);

// By param
const createChannelRelationWithServiceChannelModel = (channelRelation, channels) => {
    const channel = channels.get(channelRelation.get('connectedChannel'));
    return channelRelation.merge({ channelName: channel.get('name'), channelTypeId: channel.get('typeId'), publishingStatus: channel.get('publishingStatusId'), channelRootId: channel.get('rootId')});
}

export const getChannelRelationWithServiceChannel = createSelector(
    [getChannelRelations, getIdFromProps, CommonChannelSelectors.getChannels],
    (entities, id, channels) => createChannelRelationWithServiceChannelModel(entities.get(id) || Map(), channels)
);

// only new channelRelations
export const getRelationChannelRelationsIds = createSelector(
    getRelationChannelRelationsEntities,
    entities => entities.map(chr => chr.get('id')) || List()
);

export const getRelationNewChannelRelationsIds = createSelector(
    getRelationChannelRelationsEntities,
    channelRelations =>
    {
        return channelRelations.filter(x => x.get('isNew') === true).map(chr => chr.get('id')) || List();
    }
);

// only new connectedChannel
export const getRelationNewConnectedChannelsIds = createSelector(
    getRelationChannelRelationsEntities,
    channelRelations =>
    {
        return channelRelations.filter(x => x.get('isNew') === true).map(channel => channel.get('connectedChannel')) || List();
    }
);

export const getRelationNewConnectedChannelsIdsJS = createSelector(
    getRelationNewConnectedChannelsIds,
    channelId => getJS(channelId)
);

export const getRelationConnectedChannelsIds = createSelector(
    getRelationChannelRelationsEntities,
    channelRelations =>
    {
        return channelRelations.map(channel => channel.get('connectedChannel')) || List();
    }
);

export const getRelationConnectedIsAnyOnShowingAdditionalData = createSelector(
    getRelationChannelRelationsEntities,
    channelRelations => channelRelations.some((chr) => chr.get('showingAdditional') ? chr.get('showingAdditional') === true : false)
);

export const getConnectedChannelEntities = createSelector(
    [CommonChannelSelectors.getChannels, getRelationConnectedChannelsIds],
    (entities, connectedChannelsIds) => getEntitiesForIds(entities, connectedChannelsIds) || List()
);

// select all connected channels ids
export const getRelationConnectedChannelsIdsJS = createSelector(
    getRelationConnectedChannelsIds,
    channelId => getJS(channelId)
);

// map ids of channels
export const getRelationChannelRelationsIdsMap = createSelector(
    getRelationChannelRelationsEntities,
    entities =>  Map(entities.map(x => [x.get('connectedChannel'), x.get('id')]))
);

//draft states
export const getPublishabletConnectedServices = createSelector(
    [getConnectedServiceEntities, CommonSelectors.getPublishingStatusDraft,CommonSelectors.getPublishingStatusModified],
    (connectedServices, statusDraft, statusModified) => connectedServices.filter(service => service.get('publishingStatusId') === statusDraft.get('id') || service.get('publishingStatusId') === statusModified.get('id')) || List()
);

export const getPublishableConnectedChannels = createSelector(
    [getConnectedChannelEntities, CommonSelectors.getPublishingStatusDraft,CommonSelectors.getPublishingStatusModified],
    (connectedChannels, statusDraft, statusModified) => connectedChannels.filter(service => service.get('publishingStatusId') === statusDraft.get('id') || service.get('publishingStatusId') === statusModified.get('id')) || List()
);

export const AnyConnectedServiceOrChannelIsPublishable = createSelector(
    [getPublishabletConnectedServices, getPublishableConnectedChannels],
    (draftServices, draftChannels) => draftServices.size > 0 || draftChannels.size > 0 || false
);

//Model for saving
const createLocalizedDescriptions= (channelRelation, langCodes) => {
   const data = langCodes.map(langCode =>
    {
        const localizedEntity = channelRelation.get(langCode);
        return (localizedEntity) ?
          { language: langCode, description: localizedEntity.get('description') || '' } : null;
    })
    return data.filter(value => value != null);
}

const createLocalizedChargeTypeAdditionalInformations= (channelRelation, langCodes) => {
   const data = langCodes.map(langCode =>
    {
        const localizedEntity = channelRelation.get(langCode);
        return (localizedEntity) ?
          { language: langCode, description: localizedEntity.get('chargeTypeAdditionalInformation') || '' } : null;
    })
    return data.filter(value => value != null);
}

const createRelationChannelRelationsEntitiesData = (channelRelation, channels, langCodes) => {
    const channel = channels.get(channelRelation.get('connectedChannel'));
    return channelRelation.merge({ connectedChannel: {id: channel.get('id'), rootId: channel.get('rootId') },
                                   descriptions: createLocalizedDescriptions(channelRelation, langCodes),
                                   chargeTypeAdditionalInformations: createLocalizedChargeTypeAdditionalInformations(channelRelation, langCodes),
                                 });
}

export const createChannelRelations = (connectedService, channelRelations, channels, langCodes) => {
    const connectedServiceId = connectedService.get('service');
    return {id: connectedServiceId, connectedServiceId: connectedService.get('uiId'), channelRelations: connectedService.get('channelRelations').map(chr => createRelationChannelRelationsEntitiesData(channelRelations.get(chr), channels, langCodes))}
}

export const getRelationConnectedServicesData = createSelector(
  [getRelationConnectedServiceEntities, getChannelRelations, CommonChannelSelectors.getChannels, getTranslationLanguageCodes],
  (connectedServices, channelRelations, channels, langCodes) => connectedServices.map(service => createChannelRelations(service, channelRelations, channels, langCodes))
);

export const getSaveModel = createSelector(
  getRelationConnectedServicesData,
  data => data || List()
);

export const getServiceIds = createSelector(
 getRelationConnectedServicesIds,
 serviceIds => serviceIds
);


//prepare model for channel search
const createChannelRelationsReducedData = (channelRelation, channels) => {
    const channel = channels.get(channelRelation.get('connectedChannel'));
    return { connectedChannel: {id: channel.get('id')}, isNew: channelRelation.get('isNew')};
}

const createChannelRelationsWithServiceIdData = (connectedService, channelRelations, channels) => {
    const id = connectedService.get('service');
    return {id: id, channelRelations: connectedService.get('channelRelations').map(chr => createChannelRelationsReducedData (channelRelations.get(chr), channels))}
}

export const getRelationConnectedServicesWithChannelsData = createSelector(
  [getRelationConnectedServiceEntities, getChannelRelations, CommonChannelSelectors.getChannels],
  (connectedServices, channelRelations, channels) => connectedServices.map(service => createChannelRelationsWithServiceIdData(service, channelRelations, channels))
);

export const getServiceChannelRelationsModel = createSelector(
  getRelationConnectedServicesWithChannelsData,
  data => data || List()
);
//prepare model for channel search

export const getChannelIds = createSelector(
 getRelationConnectedChannelsIds,
 channelIds => channelIds
 );

 export const getDetailId = createSelector(
    getPageModeStateForKey,
    search => search.get('detailId') || null
 );

export const getDetailService = createSelector(
    [ServiceSelectors.getServices, getDetailId],
    (services, serviceId) => services.get(serviceId) || new Map()
)

export const getDetailChannel = createSelector(
    [CommonChannelSelectors.getChannels, getDetailId],
    (channels, channelId) => channels.get(channelId) || new Map()
)

export const getConfirmation = createSelector(
    getPageModeStateForKey,
    confirmation =>
    {
        return  Map({[confirmation.get('confirmationType')]: confirmation.get('confirmationValue')})
    }
 );

 export const getChannel = createSelector(
    [CommonChannelSelectors.getChannels, getParameterFromProps('id')],
    (channels, id) => channels.get(id) || Map()
);

export const getCurrentTabIndex = createSelector(
    getPageModeStateForKey,
    tabs => tabs.get('tabId') || 0
);

// connection table on service step 4 and all channel service steps
export const getConnections = createSelector(
    getEntities,
    entities => entities.get('connections') || List()
);

export const getConnection = createSelector(
    [getConnections,getIdFromProps],
    (connections, id) => connections.get(id) || Map()
);

export const getConnectionServiceId = createSelector(
    getConnection,
    connection => connection.get('serviceId') || null
);

export const getConnectionService = createSelector(
    [ServiceSelectors.getServices, getConnectionServiceId],
    (services, id) => services.get(id) || Map()
);

export const getConnectionServiceName = createSelector(
    getConnectionService,
    service => service.get('name') || ''
);

export const getConnectionServiceType = createSelector(
    getConnectionService,
    service => service.get('serviceType') || ''
);

export const getConnectionServiceTypeId = createSelector(
    getConnectionService,
    service => service.get('serviceTypeId') || ''
);

export const getConnectionChannelId = createSelector(
    getConnection,
    connection => connection.get('channelId') || null
);

export const getConnectionChannel = createSelector(
    [CommonChannelSelectors.getChannels, getConnectionChannelId],
    (channels, id) => channels.get(id) || Map()
);

export const getConnectionChannelName = createSelector(
    getConnectionChannel,
     channel => channel.get('name') || ''
);

export const getConnectionChannelTypeId = createSelector(
    getConnectionChannel,
    channel => channel.get('typeId') || null
);

export const getConnectionChannelTypeCode = createSelector(
    getConnectionChannel,
    channel => channel.get('subEntityType') || ''
);

export const getConnectionModified = createSelector(
    getConnection,
    connection => connection.get('modified') || ''
);

export const getConnectionModifiedBy = createSelector(
    getConnection,
    connection => connection.get('modifiedBy') || ''
);

//ApiCall
export const getApiCallServiceAndChannel = createSelector(
    getApiCalls,
    apiCalls => apiCalls.get('serviceAndChannel') || Map()
);

export const getRelationDetail = createSelector(
    getApiCallServiceAndChannel,
    relation => relation.get('relationDetail') || Map()
);

export const getChannelRelationDetail = createSelector(
    [getRelationDetail, getParameterFromProps('id')],
    (relation, id) => relation.get(id) || Map()
);

export const getRelationDetailIsFetching = createSelector(
    getChannelRelationDetail,
    relation => relation.get('isFetching') || false
)

export const getRelationDetailAreDataValid = createSelector(
    getChannelRelationDetail,
    relation => relation.get('areDataValid') || false
)

export const getRelationDetailIsLoaded = createSelector(
    getChannelRelation,
    relation => relation.get('isLoaded') || false
)





