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
import { injectIntl } from 'react-intl';

// components
import { PTVTextArea, PTVLabel } from '../../../../Components';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonServiceSelectors from '../Selectors';
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';

const ServiceTypeAdditionalInfo = ({ 
    messages, readOnly, language, translationMode, 
    additionalInformationTasks,
    additionalInformationDeadLine, 
    additionalInformationProcessingTime,
    additionalInformationValidityTime,
    selectedServiceType,
    actions, intl }) => {
   
    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }

    const serviceUsageInfoTitle = (code) => {         
        switch (code) {           
            case "Permission": return intl.formatMessage(messages.permissionTitle);
            case "Notice": return intl.formatMessage(messages.noticeTitle);
            case "Registration": return intl.formatMessage(messages.registrationTitle);                        
            default: return intl.formatMessage(messages.permissionTitle);
        }        
    };
    const serviceUsageInfoTasksPlaceholder = (code) => {
        switch (code) {           
            case "Permission": return intl.formatMessage(messages.tasksPermissionPlaceholder);
            case "Notice": return intl.formatMessage(messages.tasksNoticePlaceholder);
            case "Registration": return intl.formatMessage(messages.tasksRegistrationPlaceholder);                        
            default: return intl.formatMessage(messages.permissionTitle);
        }        
    };
    const validators = [PTVValidatorTypes.IS_REQUIRED];
       
    return (
            selectedServiceType != 'Service' ?
                <div>
                    <div className="row">
                        <div className="col-xs-12">
                            <PTVLabel><strong>{ serviceUsageInfoTitle(selectedServiceType) }</strong></PTVLabel>
                        </div> 
                    </div>                     
                    <div className="row form-group">
                        <PTVTextArea
                            componentClass="col-xs-12"
                            minRows={2}
                            maxLength={500}
                            size="full"
                            label={intl.formatMessage(messages.tasksTitle)}
                            placeholder={ serviceUsageInfoTasksPlaceholder(selectedServiceType) }
                            tooltip={intl.formatMessage(messages.tasksTooltip)}
                            value={ additionalInformationTasks }
                            name='serviceTask'
                            blurCallback={ onInputChange('additionalInformationTasks') }
                            disabled = { translationMode == "view" }
                            order={ 37 }
                            readOnly = { readOnly && translationMode == "none" }/>
                        <PTVTextArea
                            componentClass="col-xs-12"
                            minRows={2}
                            maxLength={500}
                            size="full"
                            label={intl.formatMessage(messages.deadlineTitle)}
                            placeholder={intl.formatMessage(messages.deadlinePlaceholder)}
                            tooltip={intl.formatMessage(messages.deadlineTootltip)}
                            value={ additionalInformationDeadLine }
                            name='serviceDeadline'
                            blurCallback={ onInputChange('additionalInformationDeadLine') }
                            disabled = { translationMode == "view" }
                            order={ 38 }
                            readOnly = { readOnly && translationMode == "none" }/>
                        <PTVTextArea
                            componentClass="col-xs-12"
                            minRows={2}
                            maxLength={500}
                            size="full"
                            label={intl.formatMessage(messages.processingTimeTitle)}
                            placeholder={intl.formatMessage(messages.processingTimePlaceholder)}
                            tooltip={intl.formatMessage(messages.processingTimeTootltip)}
                            value={ additionalInformationProcessingTime }
                            name='serviceProcessingTime'
                            blurCallback={ onInputChange('additionalInformationProcessingTime') }
                            disabled = { translationMode == "view" }
                            order={ 39 }
                            readOnly = { readOnly && translationMode == "none" }/>
                        <PTVTextArea
                            componentClass="col-xs-12"
                            minRows={2}
                            maxLength={500}
                            size="full"
                            label={intl.formatMessage(messages.validityTimeTitle)}
                            placeholder={intl.formatMessage(messages.validityTimePlaceholder)}
                            tooltip={intl.formatMessage(messages.validityTimeTootltip)}
                            value={ additionalInformationValidityTime }
                            name='serviceValidityTime'
                            blurCallback={ onInputChange('additionalInformationValidityTime') }
                            disabled = { translationMode == "view" }
                            order={ 40 }
                            validators = { validators }
                            readOnly = { readOnly && translationMode == "none" }/>
                    </div>
                </div>   
            :null
    );
}

function mapStateToProps(state, ownProps) {

  return {
    selectedServiceType: ServiceSelectors.getSelctedServiceTypeCode(state, ownProps),
    additionalInformationTasks: ServiceSelectors.getAdditionalInformationTasks(state, ownProps),
    additionalInformationDeadLine: ServiceSelectors.getAdditionalInformationDeadLine(state, ownProps),
    additionalInformationProcessingTime: ServiceSelectors.getAdditionalInformationProcessingTime(state, ownProps),
    additionalInformationValidityTime: ServiceSelectors.getAdditionalInformationValidityTime(state, ownProps),  
  }
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceTypeAdditionalInfo));



