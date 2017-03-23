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
import { PTVAutoComboBox } from '../../../../Components';
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';
import { YesNoRadio } from '../../../../Containers/Common/YesNoSelection';

// selectors
import * as CommonSelectors from '../Selectors';


export const ChannelAuthenticationSign = ({ messages, isOnLineSign, numberOfSigns, readOnly, actions, intl, language, translationMode, splitContainer, channelId }) => {

    const validators = [PTVValidatorTypes.IS_REQUIRED];

    const onInputChange = (input, isSet=false) => value => {
        actions.onChannelInputChange(input, channelId, value, isSet, language);
    }

    const createArray = (number) => {
        var numbers =[];
        for(var i=1;i<number;i++){
            numbers.push({id:i.toString(), name:i});
        }
        return numbers;
    }

    return (
         <div className="row form-group">
            <YesNoRadio
                name="AddElectronicChannelOnlineSignGroup"
                value={ isOnLineSign }
                radioGroupLegend={ intl.formatMessage(messages.authenticationSignLabel) }
                tooltip={ intl.formatMessage(messages.authenticationSignInfo) }
                onChange={ onInputChange('isOnLineSign') }
                readOnly= { readOnly || translationMode == 'view' || translationMode == 'edit'}
                splitContainer = { splitContainer }
            />
            { isOnLineSign ?
            <PTVAutoComboBox
                componentClass={splitContainer ? "col-xs-12" : "col-sm-6"}
                value = { numberOfSigns }
                values = { createArray(10) }
                label = { intl.formatMessage(messages.authenticationNumberSignLabel) }
                changeCallback = { onInputChange('numberOfSigns') }
                name='numberOfSigns'
                validators = { validators }
                order = { 120 }
                readOnly= { readOnly || translationMode == 'view' || translationMode == 'edit'}
                className = "limited w280"
                /> : null }
        </div>
    )
}

function mapStateToProps(state, ownProps) {
  return {
      isOnLineAuthentication : CommonSelectors.getIsOnLineAuthentication(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),
      isOnLineSign: CommonSelectors.getIsOnLineSign(state, ownProps),
      numberOfSigns: CommonSelectors.getNumberOfSignsString(state, ownProps),   
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelAuthenticationSign));