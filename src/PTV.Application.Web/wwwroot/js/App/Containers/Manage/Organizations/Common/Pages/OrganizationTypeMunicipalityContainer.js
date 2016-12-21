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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import Immutable, {Map} from 'immutable';
import {connect} from 'react-redux';

import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import { PTVRadioGroup, PTVTextArea, PTVAutoComboBox, PTVTextInput, PTVLabel } from '../../../../../Components';
import { callApiDirect } from '../../../../../Middleware/Api';

///Selectors
import * as CommonSelectors from '../../../../../Containers/Common/Selectors';
import * as CommonOrganizationSelectors from '../../Common/Selectors';
import * as OrganizationSelectors from '../../Organization/Selectors';

//Actions
import * as commonActions from '../Actions';
import * as organizationActions from  '../../Organization/Actions';
                    
const OrganizationTypeMunicipalityContainer = props => {
    
    const { messages, readOnly, language, translationMode, actions } = props;
    const { organizationId, organizationType, organizationTypes, isMunicipalityInputShown, municipality } = props;
    const { formatMessage } = props.intl;
     
    const validators = [PTVValidatorTypes.IS_REQUIRED];   
    
    const onInputChange = (input, isSet=false) => value => {
        props.actions.onOrganizationInputChange(input, organizationId, value, isSet);  
        //isMunicipalityInputShown ? props.actions.onOrganizationInputChange('municipality', organizationId, null, isSet) : null;      
    }

    const onMunicipalityInputChange = input => (value, object) =>{
        props.actions.onOrganizationEntityAdd(input, object, organizationId);
    }
        
    const municipalityDataOptions = (input, callBack) => {
            if (input == "" || input.length < 3){
                callBack(null, { options: []})
                return;
            }

            const call = callApiDirect('organization/GetMunicipalities', input)
                .then((json) => {

                return { options: json.model.map(x => {
                        return {
                            id: x.id,
                            municipalityName : x.name,
                            municipalityCode : x.municipalityCode,
                            name: x.municipalityCode + " " + x.name,
                            code: x.municipalityCode
                        }
                    }),
                    complete: true};
            });
            return call;
        }
        
    const onRenderMunicipalityValue = (options) => {
        return options.municipalityName ? options.municipalityName : options.name;
    }    
       
    return (
            <div className="row form-group">
                <PTVAutoComboBox
                    value={ organizationType }
                    label={ formatMessage(messages.organizationTypeTitle) }
                    name='organizationType'
                    values={ organizationTypes }
                    changeCallback= { onInputChange('organizationTypeId')}
                    componentClass="col-lg-6"
                    validators={ validators }
                    useFormatMessageData={ true }
                    inputProps={{'maxLength':'50'}}
                    readOnly= { readOnly || translationMode == "view" || translationMode == "edit" } />
                    
                 { isMunicipalityInputShown ? 
                    <div> 
                        <PTVAutoComboBox
                            componentClass="col-md-6 col-lg-3"
                            async={true}
                            name='Municipality'
                            label={formatMessage(messages.municipalityNameTitle)}
                            values= {municipalityDataOptions}
                            changeCallback= { onMunicipalityInputChange('municipality')}
                            value= { municipality ? municipality : '' }
                            renderValue={ onRenderMunicipalityValue }
                            validators={ validators }
                            tooltip = { formatMessage(messages.municipalityNameTooltip) }
                            placeholder = { formatMessage(messages.municipalityNamePlaceholder) }
                            inputProps={{'maxLength':'20'}}
                            readOnly={ readOnly || translationMode == "view" || translationMode == "edit" }
                            />
                            
                        <div className="ol-md-6 col-lg-3">
                            <PTVLabel labelClass={ readOnly ? "main" : null }>
                                {formatMessage(messages.municipalityNumberTitle)}
                            </PTVLabel>
                            <div>
                                <PTVLabel>
                                    { municipality ? municipality.municipalityCode : ''}
                                </PTVLabel>
                            </div>
                        </div>
                    </div>                     
                 : null }  
                    
            </div>  
            

                   
    );
}

function mapStateToProps(state, ownProps) {
    
  const organizationTypeId = CommonOrganizationSelectors.getOrganizationTypeForCode(state, 'municipality');
  
  return {
      organizationId: CommonOrganizationSelectors.getOrganizationId(state),
      organizationType: CommonOrganizationSelectors.getOrganizationType(state),
      organizationTypes: CommonOrganizationSelectors.getOrganizationTypesObjectArray(state),      
      isMunicipalityInputShown: CommonOrganizationSelectors.getIsOrganizationTypeSelected(state, organizationTypeId), 
      municipality: CommonOrganizationSelectors.getMunicipalityJS(state)
 }
}


const actions = [
    commonActions,
    organizationActions
];


export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationTypeMunicipalityContainer));



