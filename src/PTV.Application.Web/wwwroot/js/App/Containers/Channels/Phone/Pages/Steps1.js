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
import Immutable, {Map} from 'immutable';
import {defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// actions
import * as phoneChannelActions from '../Actions';

// components
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';
import PublishingStatus from '../../Common/Pages/channelPublishingStatus';
import ChannelLanguages from '../../Common/Pages/channelLanguages';
import ChannelUrl from '../../Common/Pages/channelUrl';
import ChannelDescriptionContainer from '../../Common/Pages/ChannelDescriptionContainer';
import ChannelPhoneNumber from '../../Common/Pages/channelPhoneNumber';
import ChannelEmail from '../../Common/Pages/channelEmail'; 
import { PTVLabel } from '../../../../Components'
import LanguageLabel from '../../../Common/languageLabel';

// types
import { channelTypes } from '../../Common/Helpers/types';

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
            <ChannelPhoneNumber {...sharedProps}
                messages= { Messages.phoneNumberMessages }
                shouldValidate = { true }   
                withType/>                                      
            <ChannelUrl {...sharedProps}
                messages = { Messages.urlMessages }
            />
            <PTVLabel labelClass="section-head">{ formatMessage(Messages.supportMessages.supportLabel) }</PTVLabel>
            <div className="row">   
                <ChannelEmail {...sharedProps}
                    messages = { Messages.emailMessages }
                />
            </div>
            <ChannelLanguages {...sharedProps}
                messages = { Messages.languageMessages }
                validators = { validators }
            />                
        </div>
    );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.PHONE
  return {
      keyToState
  }
}

const actions = [    
    phoneChannelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Step1));

