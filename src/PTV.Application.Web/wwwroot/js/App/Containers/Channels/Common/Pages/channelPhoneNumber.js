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
import React, {PropTypes} from 'react';
import {connect} from 'react-redux';

// actions
import * as channelActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import PhoneNumbers from '../../../Common/PhoneNumbers';
import ChannelEmail from './channelEmail'

// selectors
import * as CommonSelectors from '../Selectors';

export const ChannelPhoneNumber = ({messages, channelType, readOnly, language, translationMode, splitContainer, phoneNumber, actions, channelId, shouldValidate, children, collapsible, withType}) => {

    const onAddPhoneNumber = (entity) => {
        actions.onChannelEntityAdd('phoneNumber', entity, channelId, language);
    }
    
    const sharedProps = { readOnly, language, translationMode, splitContainer };
    
    return(
        <PhoneNumbers {...sharedProps}
            messages = { messages }
            item = { phoneNumber }
            shouldValidate = { shouldValidate }
            onAddPhoneNumber = { onAddPhoneNumber }
            collapsible = { collapsible }
            children = { children } 
            withType = { withType }           
        />
    )
}

ChannelPhoneNumber.propTypes = {
    shouldValidate: PropTypes.bool,
    collapsible: PropTypes.bool,
    withType: PropTypes.bool,
};

ChannelPhoneNumber.defaultProps = {
    shouldValidate: true,
    collapsible: false,
    withType: false
};

function mapStateToProps(state, ownProps) {

  return {
      phoneNumber: CommonSelectors.getPhoneNumber(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(ChannelPhoneNumber);
