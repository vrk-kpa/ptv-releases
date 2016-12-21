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

// components
import Languages from '../../../Common/languages';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';

const ServiceLanguages = ({ messages, readOnly, language, translationMode, actions }) => {
   
    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }
    const validators = [PTVValidatorTypes.IS_REQUIRED];

    return (
            <div className="row form-group">
                <Languages
                    componentClass= "col-xs-12"
                    id= "availableLanguages"
                    label= { messages.title }
                    tooltip= { messages.tooltip }
                    placeholder= { messages.placeholder }
                    changeCallback= { onInputChange('languages', true) }
                    validators= { validators }
                    order= {40}
                    selector= { ServiceSelectors.getSelectedLanguagesItemsJS }
                    keyToState= { 'any' }
                    language= { language }
                    readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }/>
            </div>
    );
}

const actions = [
    serviceActions
];

export default connect(null, mapDispatchToProps(actions))(ServiceLanguages);



