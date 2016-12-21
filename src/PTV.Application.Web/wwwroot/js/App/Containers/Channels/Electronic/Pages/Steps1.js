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
import React, { PropTypes, Component } from 'react';
import { injectIntl } from 'react-intl';
import { connect } from 'react-redux';

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import * as electronicChannelActions from '../Actions';

// types
import { channelTypes } from '../../Common/Helpers/types';

// components
import ChannelDescriptionContainer from '../../Common/Pages/ChannelDescriptionContainer';
import ChannelUrl from '../../Common/Pages/channelUrl';
import ChannelPhoneNumber from '../../Common/Pages/channelPhoneNumber';
import ChannelEmail from '../../Common/Pages/channelEmail';
import ChannelUrlAttachments from '../../Common/Pages/channelUrlAttachments';
import ChannelOnLineAuthentication from '../../Common/Pages/channelOnLineAuthentication';
import ChannelAuthenticationSign from '../../Common/Pages/channelAuthenticationSign';
import { PTVLabel } from '../../../../Components';
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';
import PublishingStatus from '../../Common/Pages/channelPublishingStatus';
import LanguageLabel from '../../../Common/languageLabel';

// selectors
import * as CommonSelectors from '../../../Common/Selectors';

// messages
import * as Messages from '../Messages';

export const Step1 = ({intl: {formatMessage}, readOnly, keyToState, language, translationMode, splitContainer}) => {
    const validators = [PTVValidatorTypes.IS_REQUIRED];
    const sharedProps = { readOnly, translationMode, language, keyToState, splitContainer };

    return (
        <div className="step-1">
            <LanguageLabel {...sharedProps}
            />
            <PublishingStatus {...sharedProps}
            />
            <ChannelDescriptionContainer {...sharedProps}
                messages = { Messages.channelDescriptionMessages }
            />
            <ChannelUrl {...sharedProps}
                messages = { Messages.urlMessages }
                validators = { validators }
            />
            <PTVLabel labelClass="section-head">{ formatMessage(Messages.messages.authenticationLabel) }</PTVLabel>
            <ChannelOnLineAuthentication {...sharedProps}
                messages = { Messages.authenticationListMessages }
            />
            <ChannelAuthenticationSign {...sharedProps}
                messages = { Messages.authenticationSignMessages }
            />
            {
                    // Add ChannelPhoneNumber with ChannelEmail  to new component
                    // containsData= { step1Form.support && step1Form.support.get('id') && (step1Form.support.get('number') || step1Form.support.get('email') || step1Form.support.get('chargeDescription') || step1Form.support.get('chargeTypes').find((i) => i.get('isSelected')))}
            }
            <ChannelPhoneNumber {...sharedProps}
                messages= { Messages.phoneNumberMessages }
                shouldValidate = { false }   
                collapsible>
                <ChannelEmail {...sharedProps}
                messages = { Messages.emailMessages }
                />
            </ChannelPhoneNumber>        
            <ChannelUrlAttachments {...sharedProps}
                messages = { Messages.urlAttachmentMessages }
            />                               
        </div>
    );
}               

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.ELECTRONIC
  return {
      keyToState       
  }
}

const actions = [
    electronicChannelActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Step1));
