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

// components
import * as PTVValidatorTypes from '../../../Components/PTVValidators';
import { PTVTextArea, PTVTextInput } from '../../../Components';
import Info from '../AdditionalInfoComponent'
import cx from 'classnames';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

export const PhoneNumber = ({phoneId, actions, intl, readOnly, language, translationMode, splitContainer, count, startOrder,infoMaxLength, withInfo, 
    componentClass, messages, isNew, onAddPhoneNumber, shouldValidate, phoneNumber, additionalInformation, prefixNumber }) => {
    
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
   
   const validators = [PTVValidatorTypes.IS_REQUIRED];
   const sharedProps = { readOnly, language, translationMode, splitContainer };
   const { formatMessage } = intl;

   const phoneNumberContainerClass = splitContainer ? "col-xs-12" : "col-lg-7";
   const phonePrefixClass = splitContainer ? "col-xs-12 col-lg-4" : "col-xs-12 col-md-4";
   const phoneNumberClass = splitContainer ? "col-xs-12 col-lg-8" : "col-xs-12 col-md-8";
   const infoClass = splitContainer ? "col-xs-12" : "col-lg-5";

    return (
            <div className="row form-group">
                <div className={phoneNumberContainerClass}>
                    <div className="float-children clearfix">                 
                        <PTVTextInput
                            //componentClass={phonePrefixClass}
                            label = { formatMessage(messages.prefixTitle) }                       
                            value = { prefixNumber }
                            blurCallback = {  onInputChange('prefixNumber') }
                            maxLength = { 4 }
                            size = "w120"
                            name = "phoneNumberPrefix"
                            placeholder = { formatMessage( messages.prefixPlaceHolder) }
                            order = { startOrder }
                            validators={shouldValidate ? validators : []}
                            readOnly= { readOnly && translationMode == "none" } 
                            disabled= { translationMode == "view" }
                            typingRules="containsPhoneNumberChars" />
                        <PTVTextInput
                            //componentClass={phoneNumberClass}
                            label = { formatMessage(messages.title) }
                            tooltip = { messages.tooltip ? formatMessage(messages.tooltip) : null}
                            value = { phoneNumber }
                            blurCallback = { onInputChange('number') }
                            maxLength = { 20 }
                            size = "w140"
                            name = "phoneNumber"
                            placeholder = { formatMessage(messages.placeholder) }
                            order = { startOrder }
                            validators={ shouldValidate || ( additionalInformation.length > 0) ? validators : []}
                            typingRules="containsPhoneNumberChars"
                            readOnly= { readOnly && translationMode == "none" } 
                            disabled= { translationMode == "view" } />
                    </div>
                </div>
                <div className={infoClass}>
                    { withInfo ?
                        <Info
                            {...sharedProps}
                            //componentClass = {"col-sm-12 col-md-4 col-lg-6"}
                            info = { additionalInformation }
                            handleInfoChange = { onInputChange('additionalInformation') }
                            maxLength = { infoMaxLength }
                            name = { "phoneNumberInfo" }
                            label = { formatMessage(messages.infoTitle) }
                            tooltip = { formatMessage(messages.infoTooltip) }
                            placeholder = { formatMessage(messages.infoPlaceholder) }
                            size = "full"
                        />
                    : null }
                </div>
            </div>
    );
};

PhoneNumber.propTypes = {
    phoneId: PropTypes.string.isRequired,
    messages: PropTypes.object.isRequired,
    readOnly: PropTypes.bool,
    shouldValidate: PropTypes.bool,
    withInfo: PropTypes.bool,    
    isNew: PropTypes.bool,
    componentClass: PropTypes.string,
    count: PropTypes.number,    
    onAddPhoneNumber: PropTypes.func,   
};

PhoneNumber.defaultProps = {
  isNew: false,
  readOnly: false,
  shouldValidate: false,
  withInfo: true,
}

function mapStateToProps(state, ownProps) {
  return {
      phoneNumber: CommonSelectors.getPhoneNumberPhoneNumber(state, ownProps.phoneId),
      additionalInformation: CommonSelectors.getPhoneNumberAdditionalInformation(state, ownProps.phoneId), 
      prefixNumber: CommonSelectors.getPhoneNumberPrefixNumber(state, ownProps.phoneId),       
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PhoneNumber));
