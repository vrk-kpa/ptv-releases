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
import PublishingStatusContainer from '../../../../Common/PublishStatusContainer';
import Organizations from '../../../../Common/organizations';
import { PTVAutoComboBox, PTVTextInput } from '../../../../../Components'
import LanguageSelect from '../../../../Common/languageSelect';

// selectors
import * as ServiceSearchSelectors from '../../ServiceSearch/Selectors';
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'

export const ServiceSearchCriteria =  ({ 
        messages, organizationId, serviceTypeId, serviceTypes, serviceClassId, serviceClasses, ontologyWord,
        keyToState, intl, actions }) => {            
    
    const { formatMessage } = intl;    

    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceSearchInputChange(input, value, isSet);
    }
    const onListChange = (input) => (value, isAdd) => {
        actions.onServiceSearchListChange(input, value, isAdd);
    }

    return (          
            <div>  
                <div className="form-group">
                    <PublishingStatusContainer 
                        onChangeBoxCallBack={ onListChange('selectedPublishingStatuses') }
                        tooltip= { formatMessage(messages.publishingStatusTooltip) }
                        isSelectedSelector= { ServiceSearchSelectors.getIsSelectedPublishingStatus }
                        keyToState= { keyToState }
                        labelClass= "col-xs-12"
                        contentClass= "col-xs-12" 
                        multiple                                   
                        />
                </div>                            
                
                <div className="form-group">
                    <Organizations
                        value={ organizationId }
                        id="4"
                        label={formatMessage(messages.organizationComboTitle)}
                        tooltip={formatMessage(messages.organizationComboTooltip)}
                        name='organizationId'
                        changeCallback={ onInputChange('organizationId') }
                        virtualized= { true }
                        className="limited w320"
                        inputProps= { {'maxLength': '100'} } />
                </div>
                <div className="form-group">
                        <PTVAutoComboBox
                            value={serviceTypeId}
                            id="3"
                            label={formatMessage(messages.serviceTypeComboTitle)}
                            tooltip={formatMessage(messages.serviceTypeComboTooltip)}
                            name='serviceType'
                            values={serviceTypes}
                            changeCallback={ onInputChange('serviceTypeId')}
                            className="limited w320" 
                            useFormatMessageData = {true}
                            />
                </div>        
                
                <div className="form-group">
                        <PTVAutoComboBox
                            value={serviceClassId}
                            id="3"
                            label={formatMessage(messages.serviceClassComboTitle)}
                            tooltip={formatMessage(messages.serviceClassComboTooltip)}
                            name='serviceClassId'
                            values={serviceClasses}
                            changeCallback={ onInputChange('serviceClassId')}
                            className="limited w320" />
                </div>
                <div className="form-group">
                        <PTVTextInput
                            value={ontologyWord}
                            id="2"
                            placeholder={formatMessage(messages.ontologyKeysPlaceholder)}
                            label={formatMessage(messages.ontologyKeysTitle)}
                            tooltip={formatMessage(messages.ontologyKeysTooltip)}
                            name='ontologyWord'
                            changeCallback={ onInputChange('ontologyWord')}
                            />
                </div>               
            </div>
       );
}

ServiceSearchCriteria.propTypes = {
   
};

function mapStateToProps(state, ownProps) {
  return {
     organizationId: ServiceSearchSelectors.getOrganizationId(state),
     serviceTypeId: ServiceSearchSelectors.getServiceTypeId(state),
     serviceTypes: CommonServiceSelectors.getServiceTypesObjectArray(state),
     serviceClassId: ServiceSearchSelectors.getServiceClassId(state),
     serviceClasses: CommonServiceSelectors.getServiceClassesObjectArray(state),
     ontologyWord: ServiceSearchSelectors.getOntologyWord(state)
  }
}

const actions = [
    serviceSearchActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceSearchCriteria));