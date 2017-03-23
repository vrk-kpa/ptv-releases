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
import { connect } from 'react-redux';

// actions
import * as channelActions from '../Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';
import PublishingStatus from '../../Common/Pages/channelPublishingStatus';
import ChannelDescriptionContainer from '../../Common/Pages/ChannelDescriptionContainer';
import ChannelPhoneNumber from '../../Common/Pages/channelPhoneNumber';
import ChannelEmail from '../../Common/Pages/channelEmail';
import ChannelUrl from '../../Common/Pages/channelUrl';
import ChannelLanguages from '../../Common/Pages/channelLanguages';
import LanguageLabel from '../../../Common/languageLabel';

// types
import { channelTypes } from '../../../Common/Enums';

// selectors
import * as CommonSelectors from '../../../Common/Selectors';
import { getPhoneNumberEntity, getSelectedEmailEntity } from 'Containers/Channels/Common/Selectors'

// messages
import * as Messages from '../Messages';

export const Step1 = ({readOnly, keyToState, language, translationMode, splitContainer, isPhoneNumberFilled}) => {
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
            {
                    // Add ChannelPhoneNumber with ChannelEmail  to new component
                    // containsData= { step1Form.support && step1Form.support.get('id') && (step1Form.support.get('number') || step1Form.support.get('email') || step1Form.support.get('chargeDescription') || step1Form.support.get('chargeTypes').find((i) => i.get('isSelected')))}
                }
            {(!readOnly || readOnly && isPhoneNumberFilled) &&
              <ChannelPhoneNumber {...sharedProps}
                  messages= { Messages.phoneNumberMessages }
                  shouldValidate = { false }
                  collapsible>
                  <ChannelEmail {...sharedProps}
                  messages = { Messages.emailMessages }
                  />
              </ChannelPhoneNumber>
            }
            <ChannelLanguages {...sharedProps}
                messages= { Messages.languageMessages }
                validators = { validators }
            />
        </div>
    );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.WEB_PAGE
    const phoneNumber = getPhoneNumberEntity(state, { keyToState })
    const email = getSelectedEmailEntity(state, { keyToState })
    const isPhoneNumberFilled = (
        email !== null &&
        email.has('email') &&
        email.get('email') !== '' &&
        email.get('email') !== null
      ) || (
        phoneNumber !== null &&
        phoneNumber.has('number') &&
        phoneNumber.get('number') !== '' &&
        phoneNumber.get('number') !== null
      )
  return {
      keyToState,
      isPhoneNumberFilled
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps,mapDispatchToProps(actions))(Step1);
