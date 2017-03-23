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
import { injectIntl } from 'react-intl';

// actions
import * as channelActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import { PTVTextInput } from '../../../../Components';

// selectors
import * as ChannelCommonSelectors from '../Selectors';
import * as CommonSelectors from '../../../Common/Selectors';

export const ChannelFormIdentifier = ({ messages, formIdentifier, readOnly, actions, intl, language, translationMode, channelId }) => {

    const onInputChange = (input, isSet=false) => value => {
        actions.onChannelInputChange(input, channelId, value, isSet, language);
    }
    return (
        <div className = "row form-group">
            <PTVTextInput
                componentClass = "col-md-6"
                label = { intl.formatMessage(messages.title) }
                tooltip = { intl.formatMessage(messages.tooltip) }
                value = { formIdentifier }
                blurCallback = { onInputChange('formIdentifier') }
                maxLength = { 100 }
                name = "formIdentifier"
                readOnly= { readOnly && translationMode == "none" } 
                disabled= { translationMode == "view" }
            />
        </div>
    )
}

function mapStateToProps(state, ownProps) {
  return {      
      formIdentifier: ChannelCommonSelectors.getFormIdentifier(state, ownProps),
      channelId: ChannelCommonSelectors.getChannelId(state, ownProps),
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelFormIdentifier));