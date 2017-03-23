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
import { injectIntl } from 'react-intl';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import {connect} from 'react-redux';
import { Link, browserHistory } from 'react-router';

// components
import { PTVButton, PTVLabel } from '../../../../Components';
import ChannelServiceTable from './channelServiceTable';

// actions
import * as commonServiceAndChannelActions from '../../../Relations/ServiceAndChannels/Common/Actions';
import * as relationServiceSearchActions from '../../../Relations/ServiceAndChannels/ServiceSearch/Actions';

// selectors
import * as CommonSelectors from '../../Common/Selectors';

//messages
import { channelServicesMessages } from '../../Common/Messages';

export const channelServiceStep = ({intl: {formatMessage}, language, entityId, actions, numberOfConnectedServices, keyToState, simpleView }) => {
    const sharedProps = { language };
    const handleRelationClick = () => {
        actions.onServiceSearchListRemove();
        actions.onChannelSearchListRemove();
        actions.onServiceAndChannelsListRemove();    
        actions.loadChannelServices(entityId, language);
        browserHistory.push({ pathname : '/relations', state: { channelId : entityId, keyToState : keyToState}});
    }
       
    const descriptionEdit = simpleView ? 
                                formatMessage(channelServicesMessages.descriptionNumberOfConnectedServices,{serviceCount:numberOfConnectedServices})                                
                              : formatMessage(channelServicesMessages.descriptionNumberOfConnectedServices,{serviceCount:numberOfConnectedServices})  + ' ' + formatMessage(channelServicesMessages.descriptionEdit)   
    const descriptionAdd = simpleView ?
                                formatMessage(channelServicesMessages.descriptionZeroConnectedServices,{serviceCount:numberOfConnectedServices}) 
                              : formatMessage(channelServicesMessages.descriptionZeroConnectedServices,{serviceCount:numberOfConnectedServices}) + ' ' + formatMessage(channelServicesMessages.descriptionAdd ) 
    
    return (        
        <div className="row form-group">
            <div className="col-xs-12">
                {numberOfConnectedServices && numberOfConnectedServices > 0 ?
                    <div>
                        <div className="row">                            
                            <div className="col-lg-8">
                                <PTVLabel> { descriptionEdit }</PTVLabel>
                            </div>                            
                            { !simpleView ?
                            <div className="col-lg-4">                                                                   
                                <PTVButton className="right" onClick={handleRelationClick}>
                                    {formatMessage(channelServicesMessages.buttonEdit)}
                                </PTVButton>
                            </div>
                            : null }
                        </div> 
                        <ChannelServiceTable
                            messages= {channelServicesMessages}
                            keyToState = {keyToState}
                            language = {language}
                        />                                              
                    </div>
                : 
                    <div className="row">
                            <div className="col-lg-8">
                                <PTVLabel>{ descriptionAdd }</PTVLabel>
                            </div>
                            { !simpleView ?
                            <div className="col-lg-4">                                      
                                <PTVButton className="right" onClick={handleRelationClick}>
                                    {formatMessage(channelServicesMessages.buttonAdd)}
                                </PTVButton>
                            </div>
                           : null } 
                    </div>
                }
            </div>
        </div>
    );
}

function mapStateToProps(state, ownProps) {
    return {
        numberOfConnectedServices: CommonSelectors.getNumberOfConnectedServices(state, ownProps)
  }
}
const actions = [
    commonServiceAndChannelActions,
    relationServiceSearchActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(channelServiceStep));
