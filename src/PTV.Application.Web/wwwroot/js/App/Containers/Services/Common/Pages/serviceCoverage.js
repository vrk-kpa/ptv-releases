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
import { defineMessages, injectIntl } from 'react-intl';

// actions
import * as mainActions from '../../Service/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// Components
// import { PTVAutoComboBox, PTVTextInput, PTVTagSelect } from '../../../../Components';
import { LocalizedRadioGroup } from '../../../Common/localizedData';
import Municipalities from '../../../Common/municipalities';

// Selectors
import * as ServiceSelectors from '../../Service/Selectors';
import * as CommonSelectors from '../../../Common/Selectors';
import * as CommonServiceSelectors from '../Selectors';

export const ServiceCoverage = ({readOnly, messages, language, translationMode, actions, serviceCoverageTypeId, serviceCoverageTypes, selectedMunicipalities, keyToState, intl: {formatMessage}}) => {

const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }

const isMunicipalitySelectionVisible = () => {
        var selectedCode = "";
        if (serviceCoverageTypeId && serviceCoverageTypes){
            selectedCode = serviceCoverageTypes.find(x => x.id ===  serviceCoverageTypeId).code;
        }
        return selectedCode == "Local";
    }

const nonTranslatableRO = readOnly || translationMode == "view" || translationMode == "edit";
return (
            <div className="row form-group">
                <LocalizedRadioGroup
                    radioGroupLegend={formatMessage(messages.title)}
                    tooltip={formatMessage(messages.tooltip)}
                    name='serviceCoverage'
                    value={ serviceCoverageTypeId }
                    onChange={ onInputChange('serviceCoverageTypeId', true)}
                    items={ serviceCoverageTypes }
                    verticalLayout={ true }
                    className="col-xs-12 col-sm-4 col-lg-6 imited w480"
                    readOnly={ nonTranslatableRO }
                    language={ language }
                />
                {isMunicipalitySelectionVisible(serviceCoverageTypeId, serviceCoverageTypes) ?

                <Municipalities
                    componentClass= "col-xs-12 col-sm-4 col-lg-6"
                    id= "serviceCoverageMunicipalities"
                    label= { messages.municipalitiesTitle }
                    tooltip= { messages.municipalitiesTooltip }
                    placeholder= { messages.municipalitiesTooltip }
                    changeCallback= { onInputChange('municipalities', true) }
                    language={ language }
                    keyToState = { keyToState }
                    selector={
                      readOnly
                        ? ServiceSelectors.getOrderedSelectedMunicipalitiesJS
                        : ServiceSelectors.getSelectedMunicipalitiesJS
                    }
                    readOnly= { nonTranslatableRO }
                />
                : null }
            </div>)
}

function mapStateToProps(state, ownProps) {
  return {
   serviceCoverageTypeId : ServiceSelectors.getCoverageTypeId(state, ownProps),
   serviceCoverageTypes: CommonServiceSelectors.getCoverageTypesObjectArray(state, ownProps),
  }
}

const actions = [
    mainActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceCoverage));
