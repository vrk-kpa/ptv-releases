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
import {YesNoCombo} from '../../../../Containers/Common/YesNoSelection';
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';

// selectors
import * as CommonSelectors from '../Selectors';


export const ChannelOnLineAuthentication = ({ messages, isOnLineAuthentication, readOnly, actions, intl, language, translationMode, splitContainer, channelId }) => {

    const validators = [PTVValidatorTypes.IS_REQUIRED];

    const onInputChange = (input, isSet=false) => value => {
        actions.onChannelInputChange(input, channelId, value, isSet, language);
    }
    return (
        <div className="row">
            <YesNoCombo
                value={ isOnLineAuthentication }
                label={ intl.formatMessage(messages.authenticationListLabel) }
                validatedField={messages.authenticationListLabel}
                tooltip={ intl.formatMessage(messages.authenticationListInfo) }
                onChange={ onInputChange('isOnLineAuthentication') }
                name="onlineAuthentication"
                validators={ validators }
                order={ 100 }
                readOnly= { readOnly || translationMode == 'edit' || translationMode == 'view' }
                splitContainer = { splitContainer }
            />
        </div>
    )
}

function mapStateToProps(state, ownProps) {

  return {
      isOnLineAuthentication : CommonSelectors.getIsOnLineAuthentication(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelOnLineAuthentication));
