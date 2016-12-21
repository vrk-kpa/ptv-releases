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
import * as serviceSearchActions from '../../ServiceSearch/Actions';

// components
import ServiceAndChannelTag from './ServiceAndChannelTag'

// selectors
import * as ServiceSearchSelectors from '../../ServiceSearch/Selectors';
import * as CommonOrganisationSelectors from '../../../../Manage/Organizations/Common/Selectors';
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'
import * as CommonSelectors from '../../../../Common/Selectors'

// messages
import { publishingStatusMessages } from '../../../../Common/PublishStatusContainer'

// types
import { publishingStatuses } from '../../../../Common/Enums'

export const ServiceSearchCriteriaTags =  ({ statusDraftId, localizationId, keyToState, statusPublishedId, organizationId, serviceTypeId, serviceClassId, ontologyWord,
        intl, actions }) => {            
    
    const { formatMessage } = intl;    

    const listTagRemove = (input) => value => {
        actions.onServiceSearchListChange(input, value, false);
    }

    const inputTagRemove = (input) => value => {
        actions.onServiceSearchInputChange(input, null, false);
    }


    return (          
            <div className="clearfix"> 
                <ul>
                    <ServiceAndChannelTag
                        id= { statusDraftId }
                        publishingStatusId= { statusDraftId }
                        text= { formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelDraft) }
                        onTagRemove= { listTagRemove('selectedPublishingStatuses') } 
                        getIsSelectedSelector= { ServiceSearchSelectors.getIsSelectedPublishingStatus }    
                    />                   
                    <ServiceAndChannelTag
                        id= { statusPublishedId }
                        publishingStatusId= { statusPublishedId }
                        text= { formatMessage(publishingStatusMessages.publishStatusContainerStatusLabelPublished) }
                        onTagRemove= { listTagRemove('selectedPublishingStatuses') }
                        getIsSelectedSelector= { ServiceSearchSelectors.getIsSelectedPublishingStatus }                          
                    />
                    <ServiceAndChannelTag
                        id= { localizationId }
                        keyToState= { keyToState }
                        getTextSelector= { CommonSelectors.getLanguageToName }
                        onTagRemove= { inputTagRemove('localizationId') }
                        isSelected= { localizationId }                          
                    />
                    <ServiceAndChannelTag
                        id= { organizationId }
                        getTextSelector= { CommonOrganisationSelectors.getOrganizationNameForId }
                        onTagRemove= { inputTagRemove('organizationId') }
                        isSelected= { organizationId }                          
                    />
                    <ServiceAndChannelTag
                        id= { serviceTypeId }
                        getTextSelector= { CommonServiceSelectors.geServiceTypeNameForId }
                        onTagRemove= { inputTagRemove('serviceTypeId') }
                        isSelected= { serviceTypeId }     
                        useFormatMessageData                     
                    />
                    <ServiceAndChannelTag
                        id= { serviceClassId }
                        getTextSelector= { CommonServiceSelectors.geServiceClassNameForId }
                        onTagRemove= { inputTagRemove('serviceClassId') }
                        isSelected= { serviceClassId }                          
                    />
                    <ServiceAndChannelTag
                        id= { 'ServiceSearchlOntologyWord' }
                        text= { ontologyWord }
                        onTagRemove= { inputTagRemove('ontologyWord') }
                        isSelected= { ontologyWord != null && ontologyWord != "" }                          
                    />
                </ul>                                                       
            </div>
       );
}

ServiceSearchCriteriaTags.propTypes = {
   
};

function mapStateToProps(state, ownProps) {
    const statusDraftId = CommonSelectors.getPublishingStatusId(state, publishingStatuses.draft);
    const statusPublishedId = CommonSelectors.getPublishingStatusId(state, publishingStatuses.published);
    const localizationId = CommonSelectors.getLanguageTo(state, ownProps);
    const organizationId= ServiceSearchSelectors.getOrganizationId(state);
    const serviceTypeId= ServiceSearchSelectors.getServiceTypeId(state);
    const serviceClassId= ServiceSearchSelectors.getServiceClassId(state);
  return {
     statusDraftId,
     statusPublishedId,
     localizationId,
     organizationId,
     serviceTypeId,
     serviceClassId,
     ontologyWord: ServiceSearchSelectors.getOntologyWord(state)
  }
}

const actions = [
    serviceSearchActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceSearchCriteriaTags));