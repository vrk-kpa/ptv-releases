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
import { PTVLabel } from '../../../../../Components';
import { LocalizedComboBox, LocalizedAsyncComboBox } from '../../../../Common/localizedData';
import { callApiDirect } from '../../../../../Middleware/Api';
import { renderOptions } from '../../../../../Components/PTVComponent';

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

    const validators = [PTVValidatorTypes.IS_REQUIRED]

    const onInputChange = (input, isSet=false) => value => {
        props.actions.onLocalizedOrganizationInputChange(input, organizationId, value, isSet, language);
    }

    const onMunicipalityInputChange = input => (value, object) =>{
        props.actions.onLocalizedOrganizationEntityAdd(input, object, organizationId, language);
    }

    const municipalityDataOptions = x => {

                        return { ...x,
                            name: x.code + " " + x.name,
                        };
        }

    const renderTypeOption = option =>{
        if(!option.isDisabled){
            return <div className="leaf">
                        {renderOptions({option})}
                    </div>
        }
        else{
            return <div className="root">{renderOptions({option})}</div>
        }
    }

    return (
            <div className="row form-group">
                <LocalizedComboBox
                    value={ organizationType }
                    label={ formatMessage(messages.organizationTypeTitle) }
                    name='organizationType'
                    values={ organizationTypes }
                    changeCallback= { onInputChange('organizationTypeId')}
                    componentClass="col-lg-6"
                    inputProps={{'maxLength':'50'}}
                    readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }
                    optionRenderer = {renderTypeOption}
                    searchable={false}
                    className='full'
                    language={language}
                    validatedField={messages.organizationTypeTitle}
                    validators={validators}
                    />

                 { isMunicipalityInputShown ?
                    <div className="col-lg-6">
                        <div className="row">
                          <LocalizedAsyncComboBox
                              componentClass="col-md-6"
                              name='Municipality'
                              label={formatMessage(messages.municipalityNameTitle)}
                              validatedField={messages.municipalityNameTitle}
                              endpoint = 'organization/GetMunicipalities'
                              minCharCount={1}
                              formatOption = {municipalityDataOptions}

                              onChange= { onMunicipalityInputChange('municipality')}
                              value= { municipality }
                              validators={ validators }
                              tooltip = { formatMessage(messages.municipalityNameTooltip) }
                              placeholder = { formatMessage(messages.municipalityNamePlaceholder) }
                              inputProps={{'maxLength':'20'}}
                              readOnly={ readOnly || translationMode == "view" || translationMode == "edit" }
                              language={language}
                              />

                          <div className="col-md-6">
                              <PTVLabel labelClass={ readOnly ? "main" : null }>
                                  {formatMessage(messages.municipalityNumberTitle)}
                              </PTVLabel>
                              <div>
                                  <PTVLabel>
                                      { municipality ? municipality.code : ''}
                                  </PTVLabel>
                              </div>
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
      organizationId: CommonOrganizationSelectors.getOrganizationId(state,ownProps),
      organizationType: CommonOrganizationSelectors.getOrganizationType(state,ownProps),
      organizationTypes: CommonOrganizationSelectors.getOrganizationTypesObjectArray(state,ownProps),
      isMunicipalityInputShown: CommonOrganizationSelectors.getIsOrganizationTypeSelected(state, { id: organizationTypeId , language : ownProps.language}),
      municipality: CommonOrganizationSelectors.getMunicipalityJS(state,ownProps)
 }
}


const actions = [
    commonActions,
    organizationActions
];


export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationTypeMunicipalityContainer));



