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
import ChargeType from '../../../Common/chargeTypeCombo';
import { PTVTextArea } from '../../../../Components';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonServiceSelectors from '../Selectors';
import * as ServiceSelectors from '../../Service/Selectors'
                    
const ServiceChargeTypes = ({ messages, readOnly, language, translationMode, additionalInformation, actions, intl }) => {
   
    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }
   
       
    return (
            <div className="row form-group">
                <ChargeType
                    componentClass= "col-xs-6"
                    id= "chargeTypes"
                    label= { messages.title }
                    tooltip= { messages.tooltip }
                    changeCallback= { onInputChange('chargeType', true) }                        
                    order= {35}
                    language= { language }
                    filterCode= 'Other'
                    selector= { ServiceSelectors.getChargeType }
                    readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }/>
                <PTVTextArea
                    componentClass="col-xs-6"
                    minRows={ 2 }
                    maxLength={ 500 }
                    label={ intl.formatMessage(messages.additionalInfoTitle) }
                    value={ additionalInformation }
                    blurCallback={ onInputChange('additionalInformation') }
                    order={ 36 }
                    name='additionalInformation'                        
                    disabled = { translationMode == "view" }
                    readOnly = { readOnly && translationMode == "none" }/>                    
            </div>
    );
}

function mapStateToProps(state, ownProps) {

  return {
    additionalInformation: ServiceSelectors.getAdditionalInformation(state, ownProps),  
  }
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceChargeTypes));



