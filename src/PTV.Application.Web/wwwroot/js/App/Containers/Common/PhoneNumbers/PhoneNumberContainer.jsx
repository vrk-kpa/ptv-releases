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
import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { injectIntl } from 'react-intl';
import classNames from 'classnames';

// components
import * as PTVValidatorTypes from '../../../Components/PTVValidators';
import { PTVTextArea, PTVTextInput, PTVRadioGroup } from '../../../Components';
import { ButtonDelete } from '../Buttons';
import Info from '../AdditionalInfoComponent';
import PhoneNumber from './PhoneNumber';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

export const PhoneNumberContainer = ({phoneId, actions, intl, readOnly, language, translationMode, splitContainer, count, startOrder,infoMaxLength,
    componentClass, messages, isNew, onAddPhoneNumber, shouldValidate, phoneTypeId, phoneTypes, withType,
    chargeTypes, chargeTypeId, chargeDescription, phoneNumber, additionalInformation, prefixNumber, isOtherCostSelected, children }) => {

    
   const onInputChange = input => value => {
        if (!isNew) {
            actions.onEntityInputChange('phoneNumbers', phoneId, input, value);
        } else {
            onAddPhoneNumber({
                id: phoneId,
                [input]: value
            })
        }
    }

   const renderOption = (option) => {
        const costTypes = {
            Charged: messages.phoneCostAllCosts,
            Free: messages.phoneCostFree,
            Other: messages.phoneCostOther
        };

        const message = costTypes[option.code];
        if (message) {
            return intl.formatMessage(message);
        }
        return option.code;
    }

   const validators = [PTVValidatorTypes.IS_REQUIRED];
   const translatableAreaRO = readOnly && translationMode == "none";
   const { formatMessage } = intl;
   const sharedProps = { readOnly, language, translationMode, splitContainer };
   const phoneCostDescClass = splitContainer ? "col-xs-12" : "col-sm-12 col-md-6 col-lg-4";

    return (
         <div className= { classNames("phone-container item-row", componentClass) }>
                {withType?
                <div className = "row">
                    <PTVRadioGroup
                        radioGroupLegend= { formatMessage(messages.phoneTypesTitle) }
                        name='phoneTypes'
                        value={ phoneTypeId }   
                        items={ phoneTypes }                         
                        tooltip= { formatMessage(messages.phoneTypesTooltip)}
                        onChange={ onInputChange('typeId')}
                        useFormatMessageData={ true }
                        verticalLayout={ true }
                        className="col-xs-12"
                        validators={ shouldValidate ? validators : [] }
                        readOnly= { readOnly || translationMode == 'view' || translationMode == 'edit'}
                        />
                </div>:null}
            
                <PhoneNumber {...sharedProps}
                    messages = { messages }
                    phoneId = { phoneId }
                    isNew = { isNew }
                    onAddPhoneNumber = { onAddPhoneNumber }
                    shouldValidate = { shouldValidate }
                    startOrder = { startOrder } 
                    infoMaxLength = { 150 }                       
                />                   
                
                <div className = "row">
                    <PTVRadioGroup
                        name = 'phoneNumberChargeTypes'
                        value = { chargeTypeId }
                        items = { chargeTypes }
                        radioGroupLegend =  {formatMessage(messages.chargeTypeTitle)}
                        tooltip = {formatMessage(messages.chargeTypeTooltip)}
                        onChange = { onInputChange('chargeTypeId') }
                        verticalLayout = { true }
                        className="col-xs-12"
                        useFormatMessageData={true}
                        optionRenderer = {renderOption}
                        readOnly= { readOnly || translationMode == 'view' || translationMode == 'edit'}
                        showDefaultInReadonly = { phoneNumber || translationMode == 'view' || translationMode == 'edit' }
                    />
                    <PTVTextArea
                        componentClass = { phoneCostDescClass }
                        minRows = { 2 }
                        maxLength = { 150 }
                        name = 'phoneNumberCostDescription'
                        label = { formatMessage(messages.costDescriptionTitle) }
                        tooltip = { formatMessage(messages.costDescriptionTooltip) } 
                        placeholder = { formatMessage(messages.costDescriptionPlaceholder) }
                        value = { chargeDescription }
                        validators = { isOtherCostSelected ? validators : [] }
                        blurCallback = { onInputChange('chargeDescription') }
                        disabled = { translationMode == "view" }
                        readOnly = { translatableAreaRO }
                    />                                                      
                </div>
                <div className = "row">
                    { children } 
                </div>
            </div>
    );
};

PhoneNumberContainer.propTypes = {
    phoneId: PropTypes.string.isRequired,
    messages: PropTypes.object.isRequired,
    readOnly: PropTypes.bool,
    shouldValidate: PropTypes.bool,
    isNew: PropTypes.bool,
    componentClass: PropTypes.string,
    count: PropTypes.number,
    infoMaxLength :PropTypes.number,
    onAddPhoneNumber: PropTypes.func,  
    withType: PropTypes.bool, 
};

PhoneNumberContainer.defaultProps = {
  isNew: false,
  readOnly: false,
  shouldValidate: false,
  withType: false,
}

function mapStateToProps(state, ownProps) {    
    const otherCostId = CommonSelectors.getChargeTypeId(state, 'other');
    const chargeTypeId = CommonSelectors.getPhoneNumberChargeTypeId(state, ownProps.phoneId) ||  CommonSelectors.getChargeTypeId(state, 'charged');
    const phoneTypeId = CommonSelectors.getPhoneNumberType(state, ownProps.phoneId) || CommonSelectors.getPhoneNumberTypeId(state, 'phone');
  return {
      isOtherCostSelected: otherCostId == chargeTypeId,
      chargeTypes: CommonSelectors.getChargeTypesObjectArray(state),
      phoneTypes: CommonSelectors.getPhoneNumberTypesObjectArray(state) ,
      chargeTypeId: chargeTypeId,
      chargeDescription: CommonSelectors.getPhoneNumberChargeDescription(state, ownProps.phoneId),
      phoneNumber: CommonSelectors.getPhoneNumberPhoneNumber(state, ownProps.phoneId),
      additionalInformation: CommonSelectors.getPhoneNumberAdditionalInformation(state, ownProps.phoneId), 
      prefixNumber: CommonSelectors.getPhoneNumberPrefixNumber(state, ownProps.phoneId), 
      phoneTypeId: phoneTypeId        
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PhoneNumberContainer));
