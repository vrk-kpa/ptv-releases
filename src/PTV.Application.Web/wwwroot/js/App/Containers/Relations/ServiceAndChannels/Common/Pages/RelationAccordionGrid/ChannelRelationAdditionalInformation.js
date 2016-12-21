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
import cx from 'classnames';
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps';

//Components
import ChargeType from '../../../../../Common/chargeTypeCombo';
import { PTVTextArea, PTVLabel, PTVButton } from '../../../../../../Components';

//Selectors
import * as CommonServiceAndChannelSelectors from '../../Selectors';

//Actions
import * as commonServiceAndChannelActions from '../../../Common/Actions'

//Messages
import * as Messages from '../../../ServiceAndChannel/Messages';

export const ChannelRelationAdditionalInformation = props => {
    
    const onInputChange = (input, isSet=false) => value => {
        const relationChannelId = props.id;
        props.actions.onChannelRelationInputChange(input, relationChannelId, value, isSet);
    }
    
    const { formatMessage } = props.intl;
    const { readOnly, intl, actions, 
            description, language, chargeTypeAdditionalInformation,
            componentClass 
          } = props;

 return (     
     <div className={componentClass}>
        
        <div className="header">
            <h4 className="header-title">{formatMessage(Messages.channelRelationAdditionalInformationMessages.heading)}</h4>
        </div>

        <div className="body">
            <div className="row">
                <div className="col-lg-6">
                    <PTVLabel> {formatMessage(Messages.channelRelationAdditionalInformationMessages.headerTitle) } </PTVLabel>
                </div>                 
            </div>
                
            <div className="row form-group">
                <PTVTextArea
                    componentClass="col-xs-12"
                    minRows={6}
                    maxLength={500}
                    label={ formatMessage(Messages.channelRelationAdditionalInformationMessages.descriptionTitle) }
                    placeholder={ formatMessage(Messages.channelRelationAdditionalInformationMessages.descriptionPlaceholder) }
                    tooltip={ formatMessage(Messages.channelRelationAdditionalInformationMessages.descriptionTooltip) }
                    value={ description }
                    name='description'
                    blurCallback={ onInputChange('description')}
                    disabled = { false }
                    readOnly = { readOnly }
                />
            </div>
            
            <div className="row form-group">
                <ChargeType
                    componentClass= "col-xs-6"
                    id= {props.id}
                    label= { Messages.channelRelationAdditionalInformationMessages.chargeTypeTitle }
                    tooltip= { Messages.channelRelationAdditionalInformationMessages.chargeTypeTooltip }
                    changeCallback= { onInputChange('chargeType') }                        
                    language= { language }
                    selector= { CommonServiceAndChannelSelectors.getChargeType }
                    readOnly= { readOnly }/>
                <PTVTextArea
                    componentClass="col-xs-6"
                    minRows={ 2 }
                    maxLength={ 500 }
                    label={ formatMessage(Messages.channelRelationAdditionalInformationMessages.chargeTypeAdditionalInfoTitle) }
                    value={ chargeTypeAdditionalInformation }
                    blurCallback={ onInputChange('chargeTypeAdditionalInformation') }
                    name='chargeTypeAdditionalInformation'                        
                    disabled = { false }
                    readOnly = { readOnly }/>                    
            </div>
        </div>            
     </div>     
 );
     
}

ChannelRelationAdditionalInformation.propTypes = {
        actions: PropTypes.object
    };

function mapStateToProps(state, ownProps) {
 
    return {          
        description: CommonServiceAndChannelSelectors.getChannelRelationDescription(state, ownProps),
        chargeTypeAdditionalInformation: CommonServiceAndChannelSelectors.getChannelRelationChargeTypeAdditionalInformation(state, ownProps),
        language: 'fi', //TODO change
    }
}

const actions = [
    commonServiceAndChannelActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelRelationAdditionalInformation));