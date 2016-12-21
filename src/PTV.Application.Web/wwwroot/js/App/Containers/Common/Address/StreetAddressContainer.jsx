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
import { ButtonDelete } from '../Buttons';
import { PTVLabel, PTVTextInput, PTVTextArea, PTVRadioGroup, PTVPreloader, PTVAutoComboBox, PTVLabelCustomComponent } from '../../../Components';
import { callApiDirect } from '../../../Middleware/Api';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

// enums
// import { streetAddressTypes } from '../Enums';

const messages = defineMessages({
    streetTitle : {
        id: "Containers.Channels.Address.Street.Title",
        defaultMessage: "Katuosoite"
    },
    streetNumberTitle : {
        id: "Containers.Channels.Address.StreetNumber.Title",
        defaultMessage: "Katunumero"
    },
    streetPlaceholder : {
        id: "Containers.Channels.Address.Street.Placeholder",
        defaultMessage: "esim. Mannerheimintie"
    },
    streetNumberPlaceholder : {
        id: "Containers.Channels.Address.StreetNumber.Placeholder",
        defaultMessage: "esim. 12 A 23"
    },
    poboxTitle : {
        id: "Containers.Channels.Address.POBox.Title",
        defaultMessage: "Postilokero-osoite"
    },
    streetPoboxTitle : {
        id: "Containers.Channels.Address.StreetPOBox.Title",
        defaultMessage: "Katuosoite / postilokero"
    },
    poboxPlaceholder : {
        id: "Containers.Channels.Address.POBox.Placeholder",
        defaultMessage: "esim. PL-205"
    },
    postalCode: {
        id: "Containers.Channels.Address.PostalCode.Title",
        defaultMessage: "Postinumero"
    },
    postOffice: {
        id: "Containers.Channels.Address.PostOffice.Title",
        defaultMessage: "Postitoimipaikka"
    },
    streetAddressType: {
        id: "Containers.Channels.Address.Type.Street",
        defaultMessage: "Katuosoite"
    },
    poboxAddressType: {
        id: "Containers.Channels.Address.Type.POBox",
        defaultMessage: "Postilokero-osoite"
    },
    additionalInformation: {
        id: "Containers.Channels.Address.AdditionalInformation.Title",
        defaultMessage: "Osoitteen lisätiedot"
    },
    title: {
        id: "Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Title",
        defaultMessage: "Toimitusosoite"
    },
    descriptionTitle: {
        id: "Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Description.Title",
        defaultMessage: "Sanallinen kuvaus"
    },
    descriptionInfo: {
        id: "Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Description.Tooltip",
        defaultMessage: "Sanallinen kuvaus"
    },
    descriptionPlaceholder: {
        id: "Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Description.PlaceHolder",
        defaultMessage: "Kirjoita sanallinen kuvaus"
    },
    mapCoordinatesTitle: {
        id: "Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.Title",
        defaultMessage: "Osoitteen perusteella haetut sijaintikoordinaatit: {latitude}, {longtitude}"
    },
    mapCoordinatesNotReceivedTitle: {
        id: "Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.NotReceived.Title",
        defaultMessage: "Antamaasi osoitetta ei löytynyt."
    }
});

const streetAddressTypes = [
            {
                id: 'Street',
                message: messages.streetAddressType
            },
            {
                id: 'PostBox',
                message: messages.poboxAddressType
            }];

export const StreetAddressContainer = ({readOnly, language, translationMode, splitContainer, addressId, actions, postOffice,
    showTypeSelection, isAdditionalInformationVisible, isAdditionalInformationTextAreaVisible, customMessages, additionalInformation, streetType, street, isNew,
    poBox, streetNumber, coordinateState, coordinatesHidden, postalCode, intl, onAddAddress, isAddressManadatory, latitude, longtitude}) => {

    const validators = [PTVValidatorTypes.IS_REQUIRED];
    
     const onRenderPostalCodesValue = (options) => {
        return options.code;
    }   

    const translatableAreaRO = readOnly && translationMode == "none";

     const renderTypeSelection = () => {
        if (!showTypeSelection){
            return null;
        }

        return (
            <div className="row">
                	<PTVRadioGroup
                        name="StreetAddressType"
                		value={ streetType }
                		onChange={ onInputChange('streetType') }
                        items={ streetAddressTypes }
                        className='col-lg-5'
                        useFormatMessageData={true}
                        readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }
                    />
                </div>
        );
    }

    const postalNumberDataOptions = (input, callBack) => {
            if (input == "" || input.length < 3){
                callBack(null, { options: []})
                return;
            }

            const call = callApiDirect('channel/GetPostalCodes', input)
                .then((json) => {

                return { options: json.model.map(x => {
                        return {
                            id: x.id,
                            name: x.code + " " + x.postOffice,
                            code: x.code,
                            postOffice: x.postOffice
                        }
                    }),
                    complete: true};
            });
            return call;
        }

     const loadAddressCoordinates = () =>
     {
        setTimeout(() => actions.apiCall(['addresses'], { endpoint: 'common/GetAddress', data: { addressId, language } }, [], CommonSchemas.ADDRESS), 2000);
        return (<PTVPreloader className="left" small={true}/>);
     } 

     const getAddressTextFieldProps = (type, formatMessage, componentClass, readOnly) => {
        let props = {
            componentClass,
            label: formatMessage(messages.streetTitle),
            placeholder: formatMessage(messages.streetPlaceholder),
            tooltip: formatMessage(customMessages.tooltip),
            name: "streetTitle",
            blurCallback: onInputChange('street'),
            maxLength: 100,
            value: street,
            readOnly: readOnly || translationMode == "view" || translationMode == "edit" ,
            disabled: translationMode == "view" 
                        
        }
        switch (type){
            case 'PostBox':
                props.label = formatMessage(messages.poboxTitle),
                props.placeholder = formatMessage(messages.poboxPlaceholder),
                props.name = "poboxTitle",
                props.blurCallback = onInputChange('poBox'),
                props.maxLength = 30,
                props.value = poBox;
                return props;
            case 'StreetNumber':
                props.label = formatMessage(messages.streetNumberTitle),
                props.placeholder = formatMessage(messages.streetNumberPlaceholder),
                props.name = "streetNumber",
                props.blurCallback = onInputChange('streetNumber'),
                props.maxLength = 30,
                props.tooltip = null,
                props.value = streetNumber;
                return props;
            case 'Both':
            // now it is similar as street
                props.label = formatMessage(messages.streetPoboxTitle);
                props.value = [street, poBox].filter(x => x).join(' / ');
                return props;
        }
        // default is street
        return props;
    }

     const renderAddressTextFields = () => {
         // obsolete
        // if (streetType == "Both" && !readOnly && translationMode == "none"){
        //     const propsStreet = getAddressTextFieldProps('Street', intl.formatMessage, "col-md-7", readOnly);
        //     const propsStreetNumber = getAddressTextFieldProps('StreetNumber', intl.formatMessage, "col-md-2", readOnly);
        //     const propsPobox = getAddressTextFieldProps('PostBox', intl.formatMessage, "col-md-3", readOnly);
        //     propsStreet.label = null;
        //     propsStreet.tooltip = null;
        //     propsStreetNumber.label = null;
        //     propsStreetNumber.tooltip = null;
        //     propsPobox.label = null;
        //     return (
        //         <PTVLabelCustomComponent 
        //             componentClass={splitContainer ? "col-xs-12" : "col-md-6"}
        //             label={intl.formatMessage(messages.streetPoboxTitle) + ' - ' + intl.formatMessage(messages.streetNumberTitle)}
        //             value={street || poBox}
        //             validators={ isAddressManadatory ? validators : [] }
        //             tooltip={ intl.formatMessage(customMessages.tooltip) }
        //         >
        //             <div className="row">
        //                 {renderAddressTextField(propsStreet)}
        //                 {renderAddressTextField(propsStreetNumber)}
        //                 {renderAddressTextField(propsPobox)}
        //             </div>
        //         </PTVLabelCustomComponent>

        //     );
        // }

        const streetClass = splitContainer ? "col-md-9" : "col-md-4";
        const streetNumberClass = splitContainer ? "col-md-3" : "col-md-2";
        const props = getAddressTextFieldProps(streetType, intl.formatMessage, streetClass, readOnly);
        const propsStreetNumber = getAddressTextFieldProps('StreetNumber', intl.formatMessage, streetNumberClass, readOnly);
        props.validators = isAddressManadatory ? validators : [];
        return (
            <div>
                {renderAddressTextField(props)}
                {streetType != "PostBox" ? renderAddressTextField(propsStreetNumber) : null}
            </div>
        );
    }

    const renderAddressTextField = (props) => {
        
        return (
                    <PTVTextInput
                        {...props}
                    />
        );
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
                actions.onLocalizedEntityAdd({id: addressId, postalCode: object}, CommonSchemas.ADDRESS, language)
            }            
            else
            {
                actions.onLocalizedEntityInputChange('addresses', addressId, input, value, language)
            }            
        }
    }

    return (
        <div className="street-address item-row">
            { !(readOnly|| translationMode == "view" || translationMode == "edit") ? renderTypeSelection(showTypeSelection, streetType) : null }
            <div className="row">
                { renderAddressTextFields() }
                <PTVAutoComboBox
                        componentClass={splitContainer ? "col-md-4" : "col-md-3 col-sm-6"}
                        async={true}
                        name='PostalCode'
                        label={intl.formatMessage(messages.postalCode)}
                        values= { postalNumberDataOptions }
                        changeCallback= { onInputChange('postalCode') }
                        value= { postalCode ? postalCode.toJS() : null }
                        renderValue={onRenderPostalCodesValue}
                        validators={ isAddressManadatory ? validators : [] }
                        inputProps={{'maxLength':'5'}}
                        readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }
                    />
                    <div className={splitContainer ? "col-md-8" : "col-md-3 col-sm-6"}>
                        <PTVLabel labelClass={ readOnly ? "main" : null }>
                            { intl.formatMessage(messages.postOffice) }
                        </PTVLabel>
                        <div>
                            <PTVLabel labelClass="form-value">
                                { postOffice }
                            </PTVLabel>
                        </div>
                    </div>
            </div>
            { coordinatesHidden ? null : coordinateState.indexOf('loading') != -1 ? <div className="row"> {loadAddressCoordinates()}</div> : coordinateState == 'ok' && latitude != null && longtitude != null ?
            <div className="row">
                <PTVLabel labelClass="col-md-12 disabled">{intl.formatMessage(messages.mapCoordinatesTitle,{latitude, longtitude})}</PTVLabel>
            </div>
            : coordinateState != '' ?
            <div className="row">
                <PTVLabel labelClass="col-md-12 has-error">{intl.formatMessage(messages.mapCoordinatesNotReceivedTitle)}</PTVLabel>
            </div> : null
        }
            {isAdditionalInformationVisible ?
            <div className="row">    
                <PTVTextInput
                    componentClass={splitContainer ? "col-md-12" : "col-md-6"}
                    label= { intl.formatMessage(messages.additionalInformation) }
                    name="additionalInformation"
                    blurCallback = { onInputChange('additionalInformation')}
                    value={ additionalInformation }
                    maxLength={150}
                    readOnly= { readOnly && translationMode == "none" } 
                    disabled= { translationMode == "view" }
                />
            </div>
            : null }
            {isAdditionalInformationTextAreaVisible ?
            <div className="row">    
                <PTVTextArea
                    componentClass={splitContainer ? "col-md-6" : "col-md-12"}
                    label = { intl.formatMessage(customMessages.descriptionTitle) }
                    tooltip = { intl.formatMessage(customMessages.descriptionTootltip) }
                    placeholder = { intl.formatMessage(customMessages.descriptionPlaceholder) }
                    name="additionalInformation"
                    blurCallback = { onInputChange('additionalInformation')}
                    value={ additionalInformation }
                    maxLength={150}
                    readOnly= { translatableAreaRO } 
                    disabled= { translationMode == "view" }
                />
            </div>
            : null }

        </div>
    )
}

StreetAddressContainer.propTypes = {
        intl: intlShape.isRequired,
        isAdditionalInformationVisible: PropTypes.bool,
        showTypeSelection: PropTypes.bool,        
        dataKey: PropTypes.string
    };

StreetAddressContainer.defaultProps = {
    canBeRemoved: false,
    shouldValidate: true,
    isTypeSelectionEnabled: false,
    isAdditionalInformationVisible: false,
};

function mapStateToProps(state, ownProps) {
    const props = {id: ownProps.addressId, language: ownProps.language }
    const street = CommonSelectors.getAddressStreet(state, props);
    const poBox = CommonSelectors.getAddressPOBox(state, props);
    const streetNumber = CommonSelectors.getAddressStreetNumber(state, props);
    const coordinateState = CommonSelectors.getAddressStreetCoordinatesState(state, props);
    const longtitude = CommonSelectors.getAddressStreetLongtitude(state, props);
    const latitude = CommonSelectors.getAddressStreetLatitude(state, props);
    const postalCode = CommonSelectors.getSelectedPostalCode(state, props);
    const additionalInformation = CommonSelectors.getAddressAditionalInformation(state, props);

    const isAddressManadatory = ownProps.shouldValidate || ((street || poBox || streetNumber || (!ownProps.isAdditionalInformationTextAreaVisible && additionalInformation)) ? true : false);
  return {
      streetType: ownProps.type || CommonSelectors.getAddressStreetType(state, props),
      street,
      poBox,
      streetNumber,
      coordinateState,
      longtitude,
      latitude,
      postalCode,
      postOffice: CommonSelectors.getPostalPostOffice(state, props),
      isAddressManadatory,
      additionalInformation,
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(StreetAddressContainer));


