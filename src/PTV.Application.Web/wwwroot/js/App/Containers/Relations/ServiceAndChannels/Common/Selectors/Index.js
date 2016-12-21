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
import { getEntities, getResults, getJS, getObjectArray, getEnums, getEntitiesForIds, getOrganizationReducers, getCh, getParameters, getIdFromProps, getParameterFromProps, getPageModeStateForKey } from '../../../../Common/Selectors';
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
    entities =>  Map(entities.map(x => [x.get('id'), x.get('uiId')]))
);

export const getRelationConnectedServicesIds = createSelector(
    [getConnectedServices, getRelationConnectedServicesUiIds],
    (entities, uiIds) =>  uiIds && uiIds.size > 0 ? uiIds.map(uiId => entities.get(uiId).get('id')) : List()
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

const createChannelRelationWithServiceChannelModel = (channelRelation, channels) => {      
    const channel = channels.get(channelRelation.get('connectedChannel'));
    
    return channelRelation.merge({ channelName: channel.get('name'), channelType: channel.get('type'), publishingStatus: channel.get('publishingStatus') });
}

export const getChannelRelationWithServiceChannel = createSelector(
    [getChannelRelations, getIdFromProps, CommonChannelSelectors.getChannels],
    (entities, id, channels) =>
    {
        return createChannelRelationWithServiceChannelModel(entities.get(id) || Map(), channels)  
    }  
);

//Channel relation
export const getChannelRelation = createSelector(
    [getChannelRelations, getIdFromProps],
    (entities, id) =>
    {
        return entities.get(id) || Map()
    }
);

export const getChannelRelationDescription = createSelector(
    getChannelRelation,
    search => search.get('description') || ''
)

export const getChargeType = createSelector(
    getChannelRelation,
    channelRelation => channelRelation.get('chargeType') || null
)

export const getChannelRelationChargeTypeAdditionalInformation = createSelector(
    getChannelRelation,
    search => search.get('chargeTypeAdditionalInformation') || ''
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

// only new channelRelations
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
export const getDraftConnectedServices = createSelector(
    [getConnectedServiceEntities, CommonSelectors.getPublishingStatusDraft],
    (connectedServices, statusDraft) => connectedServices.filter(service => service.get('publishingStatus') === statusDraft.get('id')) || List() 
);

export const getDraftConnectedChannels = createSelector(
    [getConnectedChannelEntities, CommonSelectors.getPublishingStatusDraft],
    (connectedChannels, statusDraft) => connectedChannels.filter(service => service.get('publishingStatus') === statusDraft.get('id')) || List()
);

export const AnyConnectedServiceOrChannelInDraft = createSelector(
    [getDraftConnectedServices, getDraftConnectedChannels],
    (draftServices, draftChannels) => draftServices.size > 0 || draftChannels.size > 0 || false
);
  
//Model for saving
const createRelationChannelRelationsEntitiesData = (channelRelation, channels) => {   
    const channel = channels.get(channelRelation.get('connectedChannel'));
    return channelRelation.merge({ connectedChannel: {id: channel.get('id')}});
}
 
export const createChannelRelations = (service, channelRelations, channels) => {
    const id = service.get('id');      
    return {id: id, channelRelations: service.get('channelRelations').map(chr => createRelationChannelRelationsEntitiesData(channelRelations.get(chr), channels))} 
}
 
export const getRelationConnectedServicesData = createSelector(
  [getRelationConnectedServiceEntities, getChannelRelations, CommonChannelSelectors.getChannels],
  (connectedServices, channelRelations, channels) => connectedServices.map(service => createChannelRelations(service, channelRelations, channels))
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
 
const createChannelRelationsWithServiceIdData = (service, channelRelations, channels) => {
    const id = service.get('id');      
    return {id: id, channelRelations: service.get('channelRelations').map(chr => createChannelRelationsReducedData (channelRelations.get(chr), channels))} 
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
 
 
