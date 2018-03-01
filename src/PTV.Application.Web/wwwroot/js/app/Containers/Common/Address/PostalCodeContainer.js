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
import React, { Component, PropTypes } from 'react';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';

import ImmutablePropTypes from 'react-immutable-proptypes';
import * as PTVValidatorTypes from '../../../Components/PTVValidators';
import { PTVLabel, PTVAutoComboBox } from '../../../Components';
import { LocalizedAsyncComboBox, LocalizedComboBox, connectLocalizedComponent } from '../../Common/localizedData';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

const messages = defineMessages({
    postalCode: {
        id: "Containers.Channels.Address.PostalCode.Title",
        defaultMessage: "Postinumero"
    },
    postOffice: {
        id: "Containers.Channels.Address.PostOffice.Title",
        defaultMessage: "Postitoimipaikka"
    },
    municipality: {
        id: "Containers.Channels.Address.Municipality.Title",
        defaultMessage: "Kunnan nimi ja numero"
    }
});

const PostalCodeContainer = ({readOnly, language, translationMode, addressId, actions, postOffice, municipalities, selectedMunicipality,
    isNew, postalCode, intl, onAddAddress, isAddressManadatory, afterChange, isMunicipalityVisible}) => {

    const validators = [PTVValidatorTypes.IS_REQUIRED];

    const onRenderPostalCodesValue = (options) => {
        return { name: options.code};
    }

    const onRenderMunicipalityOption = (options) => {
      return `${options.name} - ${options.code}`
    }

    const postalNumberDataOptions = x => {
                        return { ...x,
                            name: x.code + " " + (x.name || x.postOffice),
                        }
        }

    const onInputChange = input => (value, object) => {
        if (isNew)
        {
            onAddAddress([{
                id: addressId,
                [input]: input == 'postalCode' ? object : value
            }])
        }
        else
        {
            if (input == 'postalCode')
            {
                actions.onLocalizedEntityAdd({id: addressId, postalCode: object, municipalityId: object ? object.municipalityId : null }, CommonSchemas.ADDRESS, language)
            }
            else
            {
                actions.onLocalizedEntityInputChange('addresses', addressId, input, value, language)
            }
        }

        afterChange && typeof afterChange === 'function' && afterChange()
    }

    return (
            <div className="row">
                <LocalizedAsyncComboBox
                        componentClass={"col-md-4"}
                        name='PostalCode'
                        label={intl.formatMessage(messages.postalCode)}
                        validatedField={messages.postalCode}
                        endpoint='channel/GetPostalCodes'
                        minCharCount={1}
                        formatValue= { onRenderPostalCodesValue }
                        formatOption= { postalNumberDataOptions }
                        onChange = { onInputChange('postalCode') }
                        value= { postalCode && postalCode.size > 0 ? postalCode.toJS() : null }
                        // valueRenderer={onRenderPostalCodesValue}
                        validators={ isAddressManadatory ? validators : [] }
                        inputProps={{'maxLength':'5'}}
                        readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }
                        language={language}
                />
                <div className={"col-md-4"}>
                    <PTVLabel labelClass={ readOnly ? "main" : null }>
                        { intl.formatMessage(messages.postOffice) }
                    </PTVLabel>
                    <div>
                        <PTVLabel labelClass="form-value">
                            { (postalCode ? postalCode.get('name') : null) || postOffice }
                        </PTVLabel>
                    </div>
                </div>
                { isMunicipalityVisible && postalCode && postalCode.size > 0 ?
                <LocalizedComboBox
                    label={intl.formatMessage(messages.municipality)}
                    name='Municipality'
                    value={ selectedMunicipality }
                    values={municipalities}
                    changeCallback={onInputChange('municipalityId')}
                    readOnly={readOnly || translationMode == "view" || translationMode == "edit"}
                    optionRenderer={onRenderMunicipalityOption}
                    componentClass="col-md-4"
                    language={language}
                    />
                : null }
            </div>
    )
}

PostalCodeContainer.propTypes = {
        intl: intlShape.isRequired,
        isMunicipalityVisible: PropTypes.bool,
        afterChange: PropTypes.func,
        isAddressManadatory: PropTypes.bool,
    };

PostalCodeContainer.defaultProps = {
    isAddressManadatory: false,
    isMunicipalityVisible: false,
};

function mapStateToProps(state, ownProps) {
    const props = {id: ownProps.addressId, language: ownProps.language }
    const postalCode = CommonSelectors.getSelectedPostalCode(state, props);
    const municipalities = CommonSelectors.getMunicipalitiesObjectArray(state);

  return {
      postalCode,
      postOffice: CommonSelectors.getPostalPostOffice(state, props),
      municipalities,
      selectedMunicipality: CommonSelectors.getSelectedMunicipalityId(state, props)
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(connectLocalizedComponent(injectIntl(PostalCodeContainer), { input: 'postalCode', output: 'postalCode', single: true}));
