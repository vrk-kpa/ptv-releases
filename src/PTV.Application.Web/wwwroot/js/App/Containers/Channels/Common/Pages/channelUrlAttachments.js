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
import { injectIntl } from 'react-intl';
import {connect} from 'react-redux';
import shortId from 'shortid';

//actions
import * as channelActions from '../Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonSelectors from '../../Common/Selectors';

// components
import ChannelAttachmentContainer from './Attachments';
import { PTVAddItem } from '../../../../Components';



export const ChannelUrlAttachments = ({ messages, readOnly, intl, urlAttachments, actions, language, translationMode, channelId }) => {
    
    const onRemovelAttachments = (id) => {
        actions.onChannelListChange('urlAttachments', channelId, id, language);
    }

    const onAddUrlAttachments = (entity) => {
        actions.onChannelEntityAdd('urlAttachments', entity, channelId, language);
    }
    
    const onAddButtonClick = (param) => {
        onAddUrlAttachments( urlAttachments.size == 0 ? [{ id: shortId.generate() }, { id: shortId.generate() }]:[{ id: shortId.generate() }]);
    }

    const sharedProps = { readOnly, language, translationMode };

    const renderAttachmentContainer = (id, index, isNew) => {        
        return (
		        <ChannelAttachmentContainer {...sharedProps}
                   key={ id }
                   id = { id }         
                   isNew = { isNew }                     
                   urlCheckerMessages = { messages }
                   onAddUrlAttachment = { onAddUrlAttachments }
                />
	    );
    }


    return (
        <PTVAddItem
            items = { urlAttachments }
            readOnly = { readOnly && translationMode == 'none' }
            renderItemContent = { renderAttachmentContainer }
            messages = {{ "tooltip": intl.formatMessage(messages.attachmentsInfo),"label": intl.formatMessage(messages.attachmentsLabel) }}
            onAddButtonClick = { onAddButtonClick }
            onRemoveItemClick = { onRemovelAttachments }
            collapsible = { translationMode == 'none' }
            multiple = { translationMode == 'none' ||  translationMode == 'edit' }/>
    )
}

function mapStateToProps(state, ownProps) {

  return {
      urlAttachments: CommonSelectors.getUrlAttachments(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),
  }
}
const actions = [
    channelActions
];
export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelUrlAttachments));
