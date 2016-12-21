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
import shortId from 'shortid';

//Actions
import * as commonActions from '../Actions';
import * as organizationActions from  '../../Organization/Actions';

// components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';
import { PTVTextArea, PTVAutoComboBox, PTVTextInput, PTVCheckBox } from '../../../../../Components';
import Business from '../../../../Common/Business/Business';


// selectors
import * as CommonOrganizationSelectors from '../Selectors';
import * as OrganizationSelectors from '../../Organization/Selectors';

const OrganizationDescriptionContainer = props => {
    const validators = [PTVValidatorTypes.IS_REQUIRED];
    
    const onAddBusiness = (entity) => {
        props.actions.onOrganizationEntityAdd('business', entity, entityId);
    }  

    const onInputChange = (input, isSet=false) => value => {
        props.actions.onOrganizationInputChange(input, entityId, value, isSet);        
    }
              
    const onInputCheckboxChange = (input, isSet=false) => value => {
        props.actions.onOrganizationInputChange(input, entityId, value.target.checked, isSet);        
    }
    
    const onInputBussinesObjectChange = (input, isSet=false) => value => {
        props.actions.onOrganizationObjectChange(id, object, isSet);        
    }

    const { formatMessage } = props.intl;
    const { messages, readOnly, language, translationMode,description, name, alternateName, isAlternateNameUsedAsDisplayName, businessId, organizationId, entityId } = props;
    const translatableAreaRO = readOnly && translationMode == "none";
    const sharedProps = { readOnly, language, translationMode };
    return (
            <div>          
                <div className="row form-group">
                    <PTVTextInput
                        componentClass="col-lg-6"
                        label={formatMessage(messages.organizationNameTitle)}
                        placeholder={formatMessage(messages.organizationNamePlaceholder)}
                        value={name} 
                        blurCallback= {onInputChange('organizationName')}
                        name="organizationName"
                        validators = {validators}
                        readOnly= { readOnly && translationMode == "none" } 
                        disabled= { translationMode == "view" }
                        maxLength = {100}
                    />
                    <div className="col-xs-12 col-lg-6">
                        <div className="row">
                            <PTVTextInput
                                componentClass="col-xs-12"
                                label={formatMessage(messages.organizationAlternativeNameTitle)}
                                tooltip={formatMessage(messages.organizationAlternativeNumberTooltip)}
                                value={alternateName}
                                blurCallback= {onInputChange('organizationAlternateName')}
                                name="organizationAlternative"
                                readOnly= { readOnly && translationMode == "none" } 
                                disabled= { translationMode == "view" }
                                validators= {isAlternateNameUsedAsDisplayName ? validators : null}
                                maxLength = {100}
                            />
                            { !readOnly ?
                                <div className="col-xs-12"> 
                                    <PTVCheckBox
                                        className="strong"
                                        id={'chckUseAsDisplayName'}                                        
                                        isSelectedSelector={ CommonOrganizationSelectors.isAlternateNameUsedAsDisplayNameSelected }
                                        onClick={onInputCheckboxChange('isAlternateNameUsedAsDisplayName')}                                     
                                        readOnly = { readOnly || translationMode == "view" || translationMode == "edit" }
                                        showCheck = { true } >
                                        <FormattedMessage {...messages.isAlternateNameUsedAsDisplayName} />
                                    </PTVCheckBox>                               
                                        
                                </div>
                            : null }
                        </div>
                    </div>
                </div>
                <div className="row form-group">
                
                    <Business {...sharedProps}
                        componentClass="col-lg-6"
                        messages = { messages } 
                        businessId =  { businessId || shortId.generate() } 
                        isNew = { businessId === null }
                        onAddBusiness = { onAddBusiness }                  
                    />                
                
                    <PTVTextInput
                        componentClass="col-lg-6"
                        label={formatMessage(messages.organizationIdTitle)}
                        tooltip={formatMessage(messages.organizationIdTooltip)}
                        blurCallback={onInputChange('organizationId')}
                        value= { organizationId }
                        name="organizationId"
                        maxLength = {100}
                        readOnly= { readOnly && translationMode == "none" } 
                        disabled= { translationMode == "view" }
                    />
                </div>
                <div className="row form-group">
                    <PTVTextArea
                        componentClass="col-lg-12"
                        minRows={6}
                        maxLength={500}
                        label={ formatMessage(messages.organizationDescriptionTitle) }
                        placeholder={ formatMessage(messages.organizationDescriptionPlaceholder) }
                        tooltip={ formatMessage(messages.organizationDescriptionTooltip) }
                        value={ description }
                        name='organizationDescription'
                        blurCallback= {onInputChange('description')}
                        disabled = { translationMode == "view" }
                    readOnly = { translatableAreaRO }
                    />
                </div>
        </div>
    );
}

OrganizationDescriptionContainer.propTypes = {
        description: PropTypes.string.isRequired,
        readOnly: PropTypes.bool,
    };

function mapStateToProps(state, ownProps) {
  return {      
      name: CommonOrganizationSelectors.getName(state, ownProps), 
      alternateName: CommonOrganizationSelectors.getAlternateName(state, ownProps),
      isAlternateNameUsedAsDisplayName: CommonOrganizationSelectors.isAlternateNameUsedAsDisplayNameSelected(state, ownProps),
      businessId: CommonOrganizationSelectors.getBusinessId(state, ownProps),
      description: CommonOrganizationSelectors.getDescription(state, ownProps),     
      organizationId: CommonOrganizationSelectors.getOrganizationStringId(state, ownProps),
      entityId: CommonOrganizationSelectors.getOrganizationId(state, ownProps)
  
      }
}

const actions = [
    commonActions,
    organizationActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationDescriptionContainer));
