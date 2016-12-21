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
import React,  {Component, PropTypes} from 'react';
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';

//components    
import AdditionalInformation from '../../../../ServiceAndChannels/Common/Pages/RelationAccordionGrid/ChannelRelationAdditionalInformation';
import { PTVIcon, PTVTooltip, PTVLabel, PTVTranslationText } from '../../../../../../Components';
import { ButtonAdd } from '../../../../../../Containers/Common/Buttons';
import { ColumnFormatter } from '../../../../../Common/PublishStatusContainer';
import cx from 'classnames';
import TableNameFormatter from '../../../../../Common/tableNameFormatter';

//actions
import * as commonServiceAndChannelActions from '../../../Common/Actions'
import * as channelActions from '../../../ChannelSearch/Actions';

//selectors
import * as CommonServiceAndChannelSelectors from '../../Selectors';
import * as CommonSelectors from '../../../../../Common/Selectors';

//Messages
import * as Messages from '../../../ServiceAndChannel/Messages';

export const ChannelRelationAccordion = (props) => {
    
    
  
  const { formatMessage } = props.intl;   
  const { id, actions,
          channelRelation, showingAdditional, infoExists, 
          channelId, serviceId, serviceUiId, chargeType, description, chargeTypeAdditionalInformation, isNew,
          channelType, channelName, publishingStatusId, componentClass, chargeTypeName,
          readOnly, language
        } = props; 
        
  const renderAccordionItem = (id) => (
      <ChannelRelationAccordion id={id} />
  )  
  
  const onToggleAdditional = (channelRelationId, value) => {
       actions.onChannelRelationInputChange('showingAdditional', channelRelationId, value);
  } 
      
  const onRemove = (serviceUiId, channelRelationId) => { 
       actions.onConnectedChannelListChange(serviceUiId, channelRelationId);  
       //update channel search
       if (!isNew)
       {
          actions.loadChannelSearchResults();
       }
  } 

  const formatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;        
    }

    const chargeTypeAddInfoClass = chargeType ? "col-lg-6" : "col-lg-offset-6 col-lg-6";

  return (
    <div className={cx(componentClass, {'highlighted': isNew})}>
        
        <div className="row">
            <div className="col-lg-offset-4 col-lg-8"> 
                
                { !readOnly ?
                    <div className="flex-container flex-centered justified-between">
                        <div>  {formatPublishingStatus(publishingStatusId, null)} </div>
                        <div className="w150 ellipsis nowrap"> 
                           <TableNameFormatter 
                            content = { channelName }
                            language = { language }
                        />
                        </div>
                        <div className="w100 ellipsis">
                            <PTVTranslationText  id= { channelType.get('id') } name= { channelType.get('name') }></PTVTranslationText>
                        </div>
                        <div className="w120">   
                            <ButtonAdd 
                                onClick={() => onToggleAdditional(id, !showingAdditional)} 
                                className="button-link left-aligned">
                                {!infoExists ? formatMessage(Messages.channelRelationAccordionMessages.addAdditionalInformation):formatMessage(Messages.channelRelationAccordionMessages.editAdditionalInformation) }
                                <PTVIcon name={showingAdditional ? "icon-angle-up" : "icon-angle-down"}/>
                            </ButtonAdd>
                        </div>
                        <div> 
                            <PTVIcon
                                name= {"icon-trash"}
                                onClick={() => onRemove(serviceUiId, id)}
                            />
                        </div>
                    </div>                           
                : 
                    <div className="channel-accordion-item padded">
                        <div className="row">

                            <div className="col-lg-1"> {formatPublishingStatus(publishingStatusId, null)} </div>
                            <div className="col-lg-11">

                                <div className="row">
                                    <div className="col-lg-6"><span className="brand-color bold">{channelName}</span></div>
                                    <div className="col-lg-6">
                                        <PTVTranslationText  id= { channelType.get('id') } name= { channelType.get('name') }></PTVTranslationText>
                                    </div>
                                </div>

                                { description ?
                                    <div className="flex-container">
                                        <PTVLabel labelClass="separator"> {formatMessage(Messages.channelRelationAdditionalInformationMessages.descriptionTitle)} </PTVLabel>
                                        <PTVLabel> { description } </PTVLabel>
                                    </div>
                                : null }

                                { chargeType || chargeTypeAdditionalInformation ? 
                                    <div className="row">
                                        { chargeType ?
                                            <div className="col-lg-6">
                                                <div className="flex-container">
                                                    <PTVLabel labelClass="separator"> {formatMessage(Messages.channelRelationAdditionalInformationMessages.chargeTypeTitle)} </PTVLabel>
                                                    <PTVLabel> <PTVTranslationText  id= { chargeType } name= { chargeTypeName }></PTVTranslationText> </PTVLabel>
                                                </div>
                                            </div>
                                        : null }
                                        { chargeTypeAdditionalInformation ?
                                            <div className={chargeTypeAddInfoClass}>
                                                <div className="flex-container">
                                                    <PTVLabel labelClass="separator"> {formatMessage(Messages.channelRelationAdditionalInformationMessages.chargeTypeAdditionalInfoTitle)} </PTVLabel>
                                                    <PTVLabel> { chargeTypeAdditionalInformation } </PTVLabel>
                                                </div>
                                            </div>
                                        : null }
                                    </div>
                                : null }

                            </div>

                        </div>
                    </div>
                }

            </div>
        </div>
          
        {showingAdditional && !readOnly ?
            <AdditionalInformation
                id= { id }
                language = { language }
                componentClass = "channel-relation-info" 
            />                    
            : null      
        }            
             
    </div>
  );
};

ChannelRelationAccordion.propTypes = {

};

ChannelRelationAccordion.defaultProps = {

};

function mapStateToProps(state, ownProps) {
    const channelRelation = CommonServiceAndChannelSelectors.getChannelRelationWithServiceChannel(state, ownProps);  
    const chargeTypeId = channelRelation.get('chargeType');
    const description = channelRelation.get('description');
    const chargeTypeAdditionalInformation = CommonServiceAndChannelSelectors.getChannelRelationChargeTypeAdditionalInformation(state, ownProps);
    const infoExists = chargeTypeId!=null || description != "" || chargeTypeAdditionalInformation !=""; 
  return {
      channelRelation,
      //Channel relations
      channelId: channelRelation.get('channelId'),
      isNew: channelRelation.get('isNew'),
      serviceId: channelRelation.get('service'),
      chargeType: chargeTypeId,
      description: description,    
      showingAdditional: channelRelation.get('showingAdditional') === true ? true : false,  
      chargeTypeAdditionalInformation:chargeTypeAdditionalInformation, 
      chargeTypeName: CommonSelectors.getChargeTypesNameForId(state, {id:chargeTypeId}),   
      //Channel
      channelType: channelRelation.get('channelType'), 
      channelName: channelRelation.get('channelName'), 
      publishingStatusId: channelRelation.get('publishingStatus'),
      infoExists  
      
  }
}

const actions = [
    commonServiceAndChannelActions,
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelRelationAccordion));