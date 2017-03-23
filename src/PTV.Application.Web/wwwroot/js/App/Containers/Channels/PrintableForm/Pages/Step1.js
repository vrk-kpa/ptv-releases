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
import React, { Component } from 'react';
import { injectIntl } from 'react-intl';
import { connect } from 'react-redux';

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import * as channelActions from '../Actions';

// components
import PublishingStatus from '../../Common/Pages/channelPublishingStatus';
import ChannelDescriptionContainer from '../../Common/Pages/ChannelDescriptionContainer';
import ChannelAttachment from '../../Common/Pages/channelUrlAttachments';
import ChannelWebPages from  '../../Common/Pages/channelWebPages';
import ChannelFormIdentifier from '../../Common/Pages/channelFormIdentifier';
import ChannelPhoneNumber from '../../Common/Pages/channelPhoneNumber';
import ChannelEmail from '../../Common/Pages/channelEmail';
import ChannelAddresses from '../../Common/Pages/channelAddresses';
import LanguageLabel from '../../../Common/languageLabel';

// types
import { channelTypes } from '../../../Common/Enums';
import { addressTypes } from '../../../Common/Helpers/types';

// selectors
import * as CommonSelectors from '../../../Common/Selectors';
import { getPhoneNumberEntity, getSelectedEmailEntity } from 'Containers/Channels/Common/Selectors'

// messages
import * as Messages from '../Messages';

export const Step1 = ({intl: {formatMessage}, readOnly, keyToState, language, translationMode, splitContainer, isPhoneNumberFilled}) => {
    const sharedProps = { readOnly, translationMode, language, keyToState, splitContainer };

    return (
        <div>
            <div className="step-1">
                <LanguageLabel {...sharedProps}
                />
                <PublishingStatus {...sharedProps}
                />
                <ChannelDescriptionContainer {...sharedProps}
                    messages = { Messages.channelDescriptionMessages }
                />
                <ChannelFormIdentifier {...sharedProps}
                    messages= { Messages.formIdentifierMessages }
                />
                <ChannelWebPages {...sharedProps}
                    messages= { Messages.webPageMessages }
                    collapsible={false}
                    withTypes
                    customTypesSelector = { CommonSelectors.getPrintableFormUrlTypesObjectArray }
                />
                <ChannelAddresses {...sharedProps}
                    addressType= { addressTypes.DELIVERY }
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
                <ChannelAttachment {...sharedProps}
                    messages= { Messages.urlAttachmentsMessages }
                />
            </div>
        </div>
    );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.PRINTABLE_FORM;
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
    channelActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Step1));
