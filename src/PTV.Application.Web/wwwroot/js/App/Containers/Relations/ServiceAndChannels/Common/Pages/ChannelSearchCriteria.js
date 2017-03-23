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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

//Actions
import * as channelSearchActions from '../../ChannelSearch/Actions';

// components
import PublishingStatusContainer from '../../../../Common/PublishStatusContainer';
import Organizations from '../../../../Common/organizations';
// import { PTVAutoComboBox } from '../../../../../Components';
import { LocalizedComboBox } from '../../../../Common/localizedData';
import LanguageSelect from '../../../../Common/languageSelect';

// selectors
import * as ChannelSearchSelectors from '../../ChannelSearch/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors'

export const ChannelSearchCriteria =  ({ 
        messages, organizationId, channelTypeId, channelTypes,
        keyToState, intl, actions }) => {            
    
    const { formatMessage } = intl;    

    const onInputChange = (input, isSet=false) => value => {
        actions.onChannelSearchInputChange(input, value, isSet);
    }
    const onListChange = (input) => (value, isAdd) => {
        actions.onChannelsSearchListChange(input, value, isAdd);
    }

    return (          
            <div>  
                <div className="form-group">
                    <PublishingStatusContainer onChangeBoxCallBack={ onListChange('selectedPublishingStatuses') }
                        tooltip= { formatMessage(messages.channelSearchBoxPublishingStatusTooltip) }
                        isSelectedSelector= { ChannelSearchSelectors.getIsSelectedPublishingStatus } 
                        labelClass= "col-xs-12"
                        contentClass= "col-xs-12"  
                        keyToState= { keyToState }
                        multiple
                        />
                </div>                                                        
                <div className="form-group">
                    <Organizations
                        value={ organizationId }
                        label={ formatMessage(messages.channelSearchBoxOrganizationTitle) }
                        tooltip={ formatMessage(messages.channelSearchBoxOrganizationTooltip) }
                        name='organizationId'
                        changeCallback={ onInputChange('organizationId')}
                        className="limited w320"
                        inputProps= { {'maxLength': '100'} }
                        />
                </div>                    
                <div className="form-group">
                    <LocalizedComboBox
                        value={channelTypeId}
                        id="channelType"
                        label={ formatMessage(messages.channelSearchBoxChannelTypeTitle) }
                        tooltip={ formatMessage(messages.channelSearchBoxChannelTypeTooltip) }
                        name='channelType'
                        values={ channelTypes }
                        changeCallback={ onInputChange('channelTypeId') }
                        className="limited w320" 
                        />
                </div>                
            </div>
       );
}

ChannelSearchCriteria.propTypes = {
   
};

function mapStateToProps(state, ownProps) {
  return {
     organizationId: ChannelSearchSelectors.getOrganizationId(state),
     channelTypeId: ChannelSearchSelectors.getChannelTypeId(state),
     channelTypes: CommonSelectors.getChannelTypesObjectArray(state),     
  }
}

const actions = [
    channelSearchActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelSearchCriteria));