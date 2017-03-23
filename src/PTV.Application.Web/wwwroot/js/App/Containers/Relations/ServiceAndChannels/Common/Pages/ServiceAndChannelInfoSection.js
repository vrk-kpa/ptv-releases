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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

//Actions
//import * as commonActions from '../Actions';
import * as serviceSearchActions from '../../ServiceSearch/Actions';
import * as channelSearchActions from '../../ChannelSearch/Actions';
import * as serviceAndChannelActions from '../../ServiceAndChannel/Actions';

// components
import ServiceAndChannelInfoBox from './ServiceAndChannelInfoBox';

// selectors
import * as ServiceAndChannelCommonSelectors from '../Selectors';
import * as CommonSelectors from '../../../../Common/Selectors'

// types
import { searchSectionTabs } from '../../Common/Helpers/confirmationTypes';

const messages = defineMessages({
    
    serviceInfoSectionTitle: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.Title",
        defaultMessage: "Valitse palvelut ja asiointikanavat"
    },  
    serviceInfoSectionReadOnlyTitle: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ReadOnly.Title",
        defaultMessage: "Yhteenveto"
    },  
    serviceInfoSectionBaseDescription: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ServiceBaseDescription.Title",
        defaultMessage: "Ei valittuja palveluja."
    },    
    serviceInfoSectionConnectedDescription: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ServiceConnectedDescription.Title",
        defaultMessage: "Olet valinnut {serviceCount} palveluja."
    },    
    serviceSearchButtonTitle: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ServiceSearchButton.Title",
        defaultMessage: "Hae ja lisää palveluja"
    },
    channelInfoSectionBaseDescription: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ChannelBaseDescription.Title",
        defaultMessage: "Ei valittuja asiointikanavia."
    },    
    channelInfoSectionConnectedDescription: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ChannelConnectedDescription.Title",
        defaultMessage: "Olet valinnut {channelsCount} asiointikanavia."
    },
    channelSearchButtonTitle: {
        id: "Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.ChannelsSearchButton.Title",
        defaultMessage: "Hae ja lisää asiointikanavia"
    }, 
    serviceInfoSectionReadOnlyNotification:{
        id:"Containers.ServiceAndChannelRelations.ServiceAndChannelInfoSection.Notification",
        defaultMessage:"Alla näet yhteenvedon liitoksista. Jos haluat julkaista kaikki luonnotilassa olevat palvelut ja asiointikanavat yhdellä kertaa, paina Julkaise luonnokset -painiketta. Voit julkaista palvelut tai kanavat niiden omilla sivuilla erikseen."
    }
});

export const ServiceAndChannelInfoSection =  ({ countOfConnectedChannels, countOfConnectedServices, readOnly, intl, actions, anyDraft }) => {            
    const { formatMessage } = intl;
    
    const serviceDescription = countOfConnectedServices && countOfConnectedServices > 0 
                        ? formatMessage(messages.serviceInfoSectionConnectedDescription, { serviceCount: countOfConnectedServices }) 
                        : formatMessage(messages.serviceInfoSectionBaseDescription);

    const channelDescription = countOfConnectedChannels && countOfConnectedChannels > 0 
                        ? formatMessage(messages.channelInfoSectionConnectedDescription, { channelsCount: countOfConnectedChannels }) 
                        : formatMessage(messages.channelInfoSectionBaseDescription);    
    
    const onSearchServicesButtonClick = () => {
        actions.setChannelSearchTab(searchSectionTabs.SERVICE_SEARCH_TAB);
        actions.setServiceSearchExpanded(true); 
    }  
    
    const onSearchChannelsButtonClick = () => {
        actions.setChannelSearchTab(searchSectionTabs.CHANNEL_SEARCH_TAB);
        actions.setChannelSearchExpanded(true);
    }  
              
    return (              
            <div>  
                <div>
                    <h2> { readOnly ? formatMessage(messages.serviceInfoSectionReadOnlyTitle)  : formatMessage(messages.serviceInfoSectionTitle) } </h2>
                </div> 
                { !readOnly ?
                <div className="bordered connection-info">                
                    <div className="row">
                        <div className = "col-lg-6">              
                            <ServiceAndChannelInfoBox
                                    /*descriptionClass*/
                                    description = { serviceDescription }          
                                    buttonText = {formatMessage(messages.serviceSearchButtonTitle)}
                                    buttonClick = {onSearchServicesButtonClick}
                                    buttonRoute = ""
                                    buttonDisabled= { false }  
                                    showButton = { !countOfConnectedServices > 0 }                      
                                />
                        </div>    
                    
                        <div className = "col-lg-6 border-west">               
                            <ServiceAndChannelInfoBox
                                    /*descriptionClass*/
                                    description = { channelDescription }          
                                    buttonText = {formatMessage(messages.channelSearchButtonTitle)}
                                    buttonClick = {onSearchChannelsButtonClick}
                                    buttonRoute = ""
                                    buttonDisabled= { countOfConnectedServices == 0 }  
                                    showButton = { !countOfConnectedChannels > 0 }                      
                                /> 
                        </div>                 
                    </div>
                </div>
            : <div className="row">
                    <div className ="col-lg-10">
                        { anyDraft ? formatMessage(messages.serviceInfoSectionReadOnlyNotification) : null }
                    </div>
               </div>} 
        </div>
    );
}

ServiceAndChannelInfoSection.propTypes = {
   
};

function mapStateToProps(state, ownProps) {
  return {
     countOfConnectedServices: ServiceAndChannelCommonSelectors.getRelationConnectedServicesIds(state, ownProps).size,    
     countOfConnectedChannels: ServiceAndChannelCommonSelectors.getRelationNewConnectedChannelsIds(state, ownProps).toSet().toList().size,  
     anyDraft: ServiceAndChannelCommonSelectors.AnyConnectedServiceOrChannelIsPublishable(state, ownProps)
  }
}

const actions = [
    serviceSearchActions,
    channelSearchActions,
    serviceAndChannelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceAndChannelInfoSection));
