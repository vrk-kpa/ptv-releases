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
import { Link, browserHistory, Router } from 'react-router';

// components
import { PTVRadioGroup, PTVLabel, PTVButton } from '../../../../Components';

// actions
import * as serviceActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import * as pageContainerActions from '../../../Common/Actions/PageContainerActions';

// selectors
import * as CommonServiceSelectors from '../Selectors';
import * as ServiceSelectors from '../../Service/Selectors'

const ServiceGeneralDescription = ({
    messages, readOnly, language, translationMode,
    isGeneralDescriptionAttached,
    descriptionAttached,
    serviceId,
    serviceNameFromGeneralDescription,
    actions, intl }) => {

    const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }

    const nonTranslatableRO = readOnly || translationMode == "view" || translationMode == "edit";

    const generalDescriptionOptionConnectLabel = descriptionAttached ?
        intl.formatMessage(messages.optionConnectedDescriptionTitle) + ': ' + serviceNameFromGeneralDescription :
        intl.formatMessage(messages.optionConnectDescriptionTitle);
    const items = [
        {
            id: false,
            name: intl.formatMessage(messages.optionNoDescriptionTitle)
        },
        {
            id: true,
            name: generalDescriptionOptionConnectLabel
        }]

    const onGDSelect = () => {
            actions.onPageContainerObjectChange('serviceInfo', { serviceId, returnPath: "/service/manageService"});
            browserHistory.push({ pathname :'/manage/search/generalDescriptions'});
        }

    return (
            !nonTranslatableRO ?
            <div className="row form-group">
                <div className="col-xs-12">
                    <PTVRadioGroup
                        radioGroupLegend={ intl.formatMessage(messages.title) }
                        name='AddGeneralDescriptionGroup'
                        value={ isGeneralDescriptionAttached }
                        tooltip={ intl.formatMessage(messages.tooltip) }
                        onChange={ onInputChange('isGeneralDescriptionAttached', !isGeneralDescriptionAttached) }
                        items={ items }
                        showChildren={ 1 } >
                        <div className="related-item">
                            <PTVLabel>
                                <PTVButton
                                    type = 'link'
                                    onClick = { onGDSelect }
                                   >
                                    { descriptionAttached ? intl.formatMessage(messages.changeLink) : intl.formatMessage(messages.connectLink) }
                                </PTVButton>
                            </PTVLabel>
                        </div>
                    </PTVRadioGroup>
                </div>
            </div>
            :
            <div className="row">
                <div className="col-xs-12">
                    <PTVLabel><strong>{ intl.formatMessage(messages.connectedTitle) }</strong> { serviceNameFromGeneralDescription}</PTVLabel>
                </div>
            </div>
    );
}

function mapStateToProps(state, ownProps) {
  return {
    descriptionAttached: ServiceSelectors.getIsGeneralDescriptionSelectedAndAttached(state, ownProps),
    isGeneralDescriptionAttached: ServiceSelectors.getIsGeneralDescriptionSelected(state, ownProps),
    serviceId: CommonServiceSelectors.getServiceId(state, ownProps),
    serviceNameFromGeneralDescription: ServiceSelectors.getServiceNameFromGeneralDescriptionLocale(state, ownProps),
  }
}

const actions = [
    serviceActions,
    pageContainerActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceGeneralDescription));



