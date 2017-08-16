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
import { PTVTextInputNotEmpty } from '../../../../Components';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';

const ServiceNames = ({ messages, readOnly, language, translationMode, serviceName, alternateServiceName, actions, intl }) => {

    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }

    const validators = [PTVValidatorTypes.IS_REQUIRED];
    const validatorsNotEmpty = [PTVValidatorTypes.IS_NOT_EMPTY]
    const componentClass = readOnly || translationMode !== 'none' ? 'col-xs-12' : 'col-lg-6'
    return (
            <div className="row form-group">
                <PTVTextInputNotEmpty
                    componentClass={componentClass}
                    label={ intl.formatMessage(messages.nameTitle) }
                    validatedField={messages.nameTitle}
                    placeholder={ intl.formatMessage(messages.namePlaceholder) }
                    tooltip={ intl.formatMessage(messages.nameTooltip) }
                    value={ serviceName }
                    blurCallback={ onInputChange('serviceName') }
                    maxLength = { 100 }
                    name="serviceName"
                    validators = { validators }
                    order={ 10 }
                    readOnly= { readOnly && translationMode == "none" }
                    disabled= { translationMode == "view" }/>
                <PTVTextInputNotEmpty
                    tooltip={ intl.formatMessage(messages.alternateNameTooltip) }
                    componentClass={componentClass}
                    placeholder={ intl.formatMessage(messages.alternateNamePlaceholder) }
                    label={ intl.formatMessage(messages.alternateNameTitle) }
                    validatedField={messages.alternateNameTitle}
                    value={ alternateServiceName }
                    blurCallback={ onInputChange('alternateServiceName') }
                    maxLength = { 100 }
                    validators = { validatorsNotEmpty }
                    name='alternativeName'
                    readOnly= { readOnly && translationMode == "none" }
                    disabled= { translationMode == "view" }/>
            </div>
    );
}

function mapStateToProps(state, ownProps) {

  return {
    serviceName: ServiceSelectors.getServiceName(state, ownProps),
    alternateServiceName: ServiceSelectors.getAlternateServiceName(state, ownProps),
  }
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceNames));



