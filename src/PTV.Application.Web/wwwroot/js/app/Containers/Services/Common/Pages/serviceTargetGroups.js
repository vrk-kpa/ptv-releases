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
import TargetGroups from '../../../Common/targetGroups';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonServiceSelectors from '../Selectors';
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';

const ServiceTargetGroups = ({ messages, readOnly, language, translationMode, actions, overideTargetGroups }) => {
   
    const onListChange = (input) => (value, isAdd, over) => {
        if(over){
            if(!(!isAdd && overideTargetGroups.contains(value))) {
                  actions.onListChange('overrideTargetGroups', value, language, !isAdd);            
            }
            if(!isAdd)
            {
                 actions.onListChange(input, value, language, isAdd);
            }           
        }else{
            actions.onListChange(input, value, language, isAdd);
        }
        
    }

    const validatorsTargetGroup = [{...PTVValidatorTypes.IS_REQUIRED, errorMessage: messages.errorMessageIsRequired}];
       
    return (
            <TargetGroups validators={ validatorsTargetGroup }
                readOnly = { readOnly || translationMode == "view" || translationMode == "edit" }
                onClick= { onListChange('targetGroups') } 
                language= { language }
                />
    );
}

function mapStateToProps(state, ownProps) {
  return {
    overideTargetGroups: ServiceSelectors.getOverrideTargetGroups(state, ownProps)    
  }
}

const actions = [
    serviceActions
];

export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(ServiceTargetGroups));



