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
import ChannelRelationAccordion from './ChannelRelationAccordion';
import { PTVIcon, PTVTooltip } from '../../../../../../Components';
import { ButtonAdd } from '../../../../../../Containers/Common/Buttons';
import styles from './styles.scss';
import { ColumnFormatter } from '../../../../../Common/PublishStatusContainer';
import cx from 'classnames';
import TableNameFormatter from '../../../../../Common/tableNameFormatter';

//actions
import * as commonServiceAndChannelActions from '../../../Common/Actions'

//selectors
import * as CommonServiceAndChannelSelectors from '../../Selectors';
import * as CommonSelectors from '../../../../../Common/Selectors';

import * as Messages from '../../../ServiceAndChannel/Messages'

export const ServiceAccordion = (props) => {
    
  const { formatMessage } = props.intl;  
  const { uiId, connectedService, actions, 
          showingChildren, showingToggle,
          name, publishingStatusId, connectedChannelsCount,
          componentClass, readOnly, language
         } = props; 
  
  const onToggleShowingChildren = (serviceUiId, value) => {
      actions.onConnectedServiceInputChange('showingChildren', serviceUiId, value);
  } 
  
  const renderAccordionItem = (id) => {
    const className = cx("channel-accordion", !readOnly ? "border-north" : null);
    return (
    <ChannelRelationAccordion 
        key={id}
        id={id} 
        serviceUiId={props.uiId}
        componentClass={className}
        readOnly={readOnly}
        language = {language}
        keyToState = {id}
    />
    )
  }

  const btnLabel = showingChildren ? formatMessage(Messages.serviceAccordionMessages.hideConnectedChannelsTitle) : formatMessage(Messages.serviceAccordionMessages.showConnectedChannelsTitle);
  const iconName = showingChildren ? "icon-angle-up" : "icon-angle-down";

  const formatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;        
    }

  return (

    <div className={componentClass}>        
        <div className={cx("row", "valign", { padded: readOnly ? true : false })}>
            <div className="col-lg-1">
               {formatPublishingStatus(publishingStatusId, null)}
            </div>
            <div className={cx("col-lg-3", { "brand-color bold": readOnly ? true : false })}>
                <div className="text-truncate"> 
                    <TableNameFormatter 
                        content = { name }
                        language = { language }
                    />  
                </div>
            </div>                 

            <div className="col-lg-8"> 
                <div className="row">           
                    <div className="col-lg-3"> {formatMessage(Messages.serviceAccordionMessages.connectedChannelsCount)} {connectedChannelsCount}</div>
                    <div className="col-lg-7">  
                        { showingToggle ? 
                            <div>
                                <ButtonAdd 
                                    onClick={() => onToggleShowingChildren(uiId, !showingChildren)} 
                                    className="button-link"
                                >
                                    { btnLabel }
                                    <PTVIcon
                                        name={iconName}  
                                    />
                                </ButtonAdd>
                            </div>
                            : null } 
                    </div>                         
                </div>  
            </div> 
        </div>    
        
        { showingChildren ? 
        <div className={ readOnly ? "border-north" : null }>
            {
                connectedService && connectedService.get('channelRelations') && connectedService.get('channelRelations').map((id) => 
                     renderAccordionItem(id)
                    ).toArray()
            }               
        </div>     
        : null }
    </div>
  );
};

ServiceAccordion.propTypes = {
    expanded: PropTypes.bool,
};

ServiceAccordion.defaultProps = {
    expanded: false,
};

function mapStateToProps(state, ownProps) {
        
    const connectedService = CommonServiceAndChannelSelectors.getConnectedServiceEntity(state, { id: ownProps.uiId, language : ownProps.language});   
    const service = CommonServiceAndChannelSelectors.getService(state, {id: connectedService.get('service'), language : ownProps.language});
    const channelRelations = connectedService.get('channelRelations');
    const connectedChannelsCount = channelRelations ? channelRelations.size : 0;
  return {
      connectedService,     
      name: service.get('name'), 
      publishingStatusId: service.get('publishingStatusId'), 
      connectedChannelsCount: connectedChannelsCount,       
      showingChildren: connectedService.get('showingChildren') === true ? true : connectedService.get('showingChildren') === false ? false : (ownProps.isExpanded && connectedChannelsCount < 10),
      showingToggle: connectedChannelsCount > 0 ? true : false    
  }
}

const actions = [
    commonServiceAndChannelActions,
    
    
    
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceAccordion));
