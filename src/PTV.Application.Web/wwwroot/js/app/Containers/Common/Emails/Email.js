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
import Info from '../AdditionalInfoComponent'
import EmailComponent from '../EmailComponent';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

export const Email = ({emailId, actions, intl, readOnly, language, translationMode, splitContainer, count, startOrder,infoMaxLength, maxLength,
    withInfo, componentClass, messages, isNew, onAddEmail, shouldValidate, email, additionalInformation }) => {

   const onInputChange = input => value => {
        if (!isNew) {
            actions.onEntityInputChange('emails', emailId, input, value);
        } else {
            onAddEmail({
                id: emailId,
                [input]: value
            })
        }
    }

   const validators = [PTVValidatorTypes.IS_REQUIRED];

   const { formatMessage } = intl;
   const sharedProps = { readOnly, language, translationMode, splitContainer };
   const emailClass = splitContainer ? "col-xs-12 col-lg-6" : "col-sm-8 col-md-6 col-lg-4";
   const infoClass = splitContainer ? "col-xs-12 col-lg-6" : "col-sm-4 col-md-6 col-lg-8";

    return (
            <div className="row">
                <div className={emailClass}>
                    <EmailComponent {...sharedProps}
                        email = { email }
                        handleEmailChange = { onInputChange('email') }
                        maxLength = { maxLength }
                        name = { "email" }
                        label = { formatMessage(messages.title) }
                        validatedField={messages.title}
                        tooltip = { messages.tooltip ? formatMessage(messages.tooltip) : null }
                        placeholder = { formatMessage(messages.placeholder) }
                        isRequired = { additionalInformation && additionalInformation.length>0 }
                    />
                </div>
                <div className={infoClass}>
                    { withInfo ?
                        <Info {...sharedProps}
                            componentClass={infoClass}
                            info = { additionalInformation }
                            handleInfoChange = { onInputChange('additionalInformation') }
                            maxLength = { infoMaxLength }
                            name = { "emailAdditionalInfo" }
                            label = { formatMessage(messages.infoLabel) }
                            tooltip = { formatMessage(messages.infoTooltip) }
                            placeholder = { formatMessage(messages.infoPlaceholder) }
                        />
                    : null }
                </div>
            </div>
    );
};

Email.propTypes = {
    emailId: PropTypes.string.isRequired,
    messages: PropTypes.object.isRequired,
    readOnly: PropTypes.bool,
    shouldValidate: PropTypes.bool,
    withInfo: PropTypes.bool,
    isNew: PropTypes.bool,
    componentClass: PropTypes.string,
    count: PropTypes.number,
    onAddEmail: PropTypes.func,
};

Email.defaultProps = {
  isNew: false,
  readOnly: false,
  shouldValidate: false,
  withInfo: true
}

function mapStateToProps(state, ownProps) {
  return {
      email: CommonSelectors.getEmailEmail(state, ownProps.emailId),
      additionalInformation: CommonSelectors.getEmailAdditionalInformation(state, ownProps.emailId),
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Email));
