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
// import { PTVRadioGroup } from '../../../../Components';
import { LocalizedRadioGroup } from '../../../Common/localizedData';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonServiceSelectors from '../Selectors';
import * as ServiceSelectors from '../../Service/Selectors'

const ServiceTypes = ({ messages, readOnly, language, translationMode, serviceType, serviceTypes, descriptionAttached, serviceTypeFromGeneralDescription, actions, intl }) => {

    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }

    const typeId = descriptionAttached ? serviceTypeFromGeneralDescription : serviceType;
    return (
            <div className="row form-group">
                <div className="col-xs-12">
                    <LocalizedRadioGroup
                      language={language}
                        radioGroupLegend={intl.formatMessage(messages.title)}
                        name='ServiceTypeGroup'
                        value={ typeId }
                        tooltip={intl.formatMessage(messages.tooltip)}
                        onChange={ onInputChange('serviceTypeId')}
                        items={ serviceTypes }
                        verticalLayout={ true }
                        readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }
                        disabled= { descriptionAttached }
                        >
                    </LocalizedRadioGroup>
                </div>
            </div>
    );
}

function mapStateToProps(state, ownProps) {

  return {
    serviceType: ServiceSelectors.getServiceTypeId(state, ownProps),
    serviceTypes: CommonServiceSelectors.getServiceTypesObjectArray(state, ownProps),
    serviceTypeFromGeneralDescription: ServiceSelectors.getServiceTypeIdFromGeneralDescription(state, ownProps),
    descriptionAttached: ServiceSelectors.getIsGeneralDescriptionSelectedAndAttached(state, ownProps),
  }
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceTypes));



