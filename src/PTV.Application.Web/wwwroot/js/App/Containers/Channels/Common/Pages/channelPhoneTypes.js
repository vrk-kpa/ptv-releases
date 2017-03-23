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

// components
// import { PTVRadioGroup } from '../../../../Components';
// localized components
import { LocalizedRadioGroup } from '../../../Common/localizedData';


// actions
import * as channelActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonChannelSelectors from '../Selectors';
import * as CommonSelectors from '../../../Common/Selectors';


                    
const ChannelPhoneTypes = ({ messages, validators, readOnly, channelType, language, translationMode, channelId, phoneType, phoneTypes, actions, intl }) => {
   
    const onObjectChange = (input, isSet=false) => value => {
        actions.onChannelObjectChange(channelId, {['support'] : {[input] : value } }, isSet, language);
    }
   
       
    return (
        <div className = "row form-group">
            <div className="col-xs-12">
                <LocalizedRadioGroup
                    radioGroupLegend= {intl.formatMessage(messages.phoneTypesLabel)}
                    name='channelPhoneTypes'
                    value={ phoneType }   
                    items={ phoneTypes }                         
                    tooltip= {intl.formatMessage(messages.phoneTypesInfo)}
                    onChange={ onObjectChange('typeId')}
                    verticalLayout={ true }
                    validators={ validators }
                    readOnly= { readOnly || translationMode == 'view' || translationMode == 'edit' }
                    >                          
                </LocalizedRadioGroup>
            </div>
        </div>
    );
}

function mapStateToProps(state, ownProps) {

  return {
      channelId: CommonChannelSelectors.getChannelId(state, ownProps),
      phoneType: CommonChannelSelectors.getPhoneType(state, ownProps),
      phoneTypes: CommonSelectors.getPhoneNumberTypesObjectArray(state)      
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelPhoneTypes));



