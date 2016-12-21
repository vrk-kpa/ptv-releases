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
import { PTVTextArea } from '../../../../Components';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonServiceSelectors from '../Selectors';
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';
                    
const ServiceDescriptions = ({ 
    messages, readOnly, language, translationMode, 
    shortDescriptions,
    description, 
    serviceUsage, 
    userInstruction,
    descriptionFromGeneralDescription,
    descriptionAttached,
    serviceType, serviceTypes,
    actions, intl }) => {
   
    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }
   
    const serviceUsagePlaceholder = (code) => {
        switch (code) {
            case "Service": return  intl.formatMessage(messages.conditionOfServiceUsageServicePlaceholder);
            case "Permission": return  intl.formatMessage(messages.conditionOfServiceUsagePermissionPlaceholder);
            case "Notice": return  intl.formatMessage(messages.conditionOfServiceUsageNoticePlaceholder);
            case "Registration": return  intl.formatMessage(messages.conditionOfServiceUsageRegistrationPlaceholder);                        
            default: return  intl.formatMessage(messages.conditionOfServiceUsageServicePlaceholder);
        }        
    };

    const getSelectedServiceTypeCode = () => {               
        const index = serviceTypes.findIndex(x=>x.id==serviceType);
        return index>-1 ? serviceTypes[index].code : "";
    }    

    const validators = [PTVValidatorTypes.IS_REQUIRED];
    const translatableAreaRO =  readOnly && translationMode == "none";
    const generalDescriptionRO = readOnly || translationMode == "edit";

    return (
            <div>
                <div className="row form-group">
                    <PTVTextArea
                        componentClass="col-xs-12"
                        minRows={ 6 }
                        maxLength={ 150 }
                        label={ intl.formatMessage(messages.shortDescriptionTitle) }
                        placeholder={ intl.formatMessage(messages.shortDescriptionPlaceholder) }
                        tooltip={ intl.formatMessage(messages.shortDescriptionTooltip) }
                        value={ shortDescriptions }
                        name='serviceShortDescription'
                        blurCallback={ onInputChange('shortDescriptions') }
                        validators = { validators }
                        order={ 20 }
                        disabled = { translationMode == "view" }
                        readOnly = { translatableAreaRO }/>
                </div>

               { descriptionAttached ?
                    <div className="row form-group">
                        <PTVTextArea
                            componentClass="col-xs-12"
                            name='serviceGeneralDescription'
                            label={ intl.formatMessage(messages.descriptionTitle) }
                            tooltip={ intl.formatMessage(messages.descriptionTooltip) }
                            value= { descriptionFromGeneralDescription }
                            disabled = { !generalDescriptionRO }
                            readOnly = { generalDescriptionRO }/>
                    </div>
                : null} 

                <div className="row form-group">
                    <PTVTextArea
                        componentClass="col-xs-12"
                        minRows={ 6 }
                        maxLength={ 4000 }
                        size="full"
                        name='serviceDescription'
                        label=  { descriptionAttached ? null :  intl.formatMessage(messages.descriptionTitle)}
                        placeholder={ intl.formatMessage(messages.descriptionPlaceholder) }
                        tooltip={ intl.formatMessage(messages.descriptionTooltip) }
                        value= { description }
                        blurCallback={ onInputChange('description') }
                        validators = { descriptionAttached ? null : validators }
                        order={30}
                        disabled = { translationMode == "view" }
                        readOnly = { translatableAreaRO }/>
                </div>

                <div className="row form-group">
                    <PTVTextArea
                        componentClass="col-xs-12"
                        minRows={ 6 }
                        maxLength={ 4000 }
                        size="full"
                        label= { intl.formatMessage(messages.conditionOfServiceUsageTitle) }
                        placeholder={ serviceUsagePlaceholder(getSelectedServiceTypeCode()) }
                        tooltip={ intl.formatMessage(messages.conditionOfServiceUsageTooltip) }
                        value={ serviceUsage }
                        name='conditionOfUsage'
                        blurCallback={ onInputChange('serviceUsage') }
                        disabled = { translationMode == "view" }
                        readOnly = { translatableAreaRO }/>
                </div>

                <div className="row form-group">
                    <PTVTextArea
                        componentClass="col-xs-12"
                        minRows={ 6 }
                        maxLength={ 4000 }
                        size="full"
                        label={ intl.formatMessage(messages.serviceUserInstructionTitle) }
                        placeholder={ intl.formatMessage(messages.serviceUserInstructionPlaceholder) }
                        tooltip={ intl.formatMessage(messages.serviceUserInstructionTooltip) }
                        value={ userInstruction }
                        name='serviceUserInstruction'
                        blurCallback={ onInputChange('userInstruction') }
                        disabled = { translationMode == "view" }
                        readOnly = { translatableAreaRO }/>
                </div>
            </div>
    );
}

function mapStateToProps(state, ownProps) {

  return {
    shortDescriptions: ServiceSelectors.getShortDescriptions(state, ownProps),
    description: ServiceSelectors.getDescription(state, ownProps),
    serviceUsage: ServiceSelectors.getServiceUsage(state, ownProps),
    userInstruction: ServiceSelectors.getUserInstruction(state, ownProps),
    descriptionFromGeneralDescription: ServiceSelectors.getDescriptionFromGeneralDescription(state, ownProps),
    descriptionAttached: ServiceSelectors.getIsGeneralDescriptionSelectedAndAttached(state, ownProps),
    serviceType: ServiceSelectors.getServiceType(state, ownProps),
    serviceTypes: CommonServiceSelectors.getServiceTypesObjectArray(state, ownProps),  
  }
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceDescriptions));



