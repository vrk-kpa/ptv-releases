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
import Immutable, {Map} from 'immutable';
import cx from 'classnames';

// components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';
import { PTVTextInput, PTVTextArea, PTVUrlChecker } from '../../../../../Components';
import { ButtonDelete } from '../../../../Common/Buttons';
import '../../Styles/ChannelAttachmentContainer.scss';

// actions
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as commonActions from '../../../../Common/Actions';
import * as commonChannelActions from '../../../Common/Actions';

// selectors
import * as CommonChannelSelectors from '../../Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';

// schemas
import { CommonChannelsSchemas } from '../../../Common/Schemas';

const messages = defineMessages({
     attachmentLabel: {
        id: "Container.ChannelAttachmentContainer.NameLabel",
        defaultMessage: "Nimi"
        },
     attachmentDescription: {
        id: "Container.ChannelAttachmentContainer.DescriptionLabel",
        defaultMessage: "Kuvaus"
        }
   });

export const ChannelAttachmentContainer = ({intl, actions, name, description, urlAddress, urlExists, urlCheckerMessages,
    readOnly, language, translationMode, urlIsFetching, urlAreDataValid, id, componentClass, isNew, onAddUrlAttachment }) => {
   
    const validators = [PTVValidatorTypes.IS_REQUIRED];
    const { formatMessage } = intl;

    const onInputChange = input => value => {
        if (!isNew) {
            actions.onChannelInputChange(input, id, value, false, language, 'urlAttachments');
        } else {
            onAddUrlAttachment([{
                id: id,
                [input]: value
            }])
        }
    }

    const translatableAreaRO = readOnly && translationMode == "none";

    const checkUrl = value => {
         actions.apiCall(['urlAttachments', id], 
            { endpoint: 'channel/CheckUrl', data: { urlAddress: value, id: id } }, null, CommonChannelsSchemas.URL_ATTACHMENT);
    }

        return (
            <div className={cx("channel-attachment-container item-row", componentClass)}>

                <div className="row">
                    <PTVTextInput
                        componentClass="col-md-6"
                        label={ formatMessage(messages.attachmentLabel) }
                        value={ name }
                        name='attachmentName'
                        blurCallback={ onInputChange('name') }
                        maxLength = { 100 }
                        name="AttachmentName"
                        readOnly= { readOnly && translationMode == "none" } 
                        disabled= { translationMode == "view" }
                        />
                    <div className="col-md-6">
                        <PTVTextArea
                            minRows={ 1 }
                            maxLength={ 150 }
                            name='attachmentDescription'
                            label={ formatMessage(messages.attachmentDescription) }
                            value={ description }
                            blurCallback={onInputChange('description') }
                            disabled = { translationMode == "view" }
                            readOnly = { translatableAreaRO }
                            />
                    </div>
                </div>

                <PTVUrlChecker
                    messages = { urlCheckerMessages }                    
                    id = { id }
                    value = { urlAddress }
                    checkUrlCallback = { checkUrl }
                    blurCallback = { onInputChange('urlAddress') }
                    showPreloader = { urlIsFetching }
                    showMessage ={ urlAreDataValid }
                    urlExists = { urlExists }
                    readOnly= { readOnly && translationMode == "none" } 
                    disabled= { translationMode == "view" }
                    translationMode = { translationMode }
                    maxLength= { 500 }
                    />

            </div>
       );
};

ChannelAttachmentContainer.propTypes = {
    id: PropTypes.string.isRequired,
    name: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
    urlAddress: PropTypes.string.isRequired,
    urlExists: PropTypes.any.isRequired,
    urlIsFetching: PropTypes.bool.isRequired,
    urlAreDataValid: PropTypes.bool.isRequired,
    urlCheckerMessages: PropTypes.object.isRequired,
    readOnly: PropTypes.bool,
    componentClass: PropTypes.string,  
    isNew: PropTypes.bool,
    onAddWebPage: PropTypes.func,      
};

ChannelAttachmentContainer.defaultProps = {
  readOnly: false,
}

function mapStateToProps(state, ownProps) {  
  return {
      name: CommonChannelSelectors.getChannelUrlAttachmentName(state, ownProps),
      description: CommonChannelSelectors.getChannelUrlAttachmentDescription(state, ownProps),
      urlAddress: CommonChannelSelectors.getChannelUrlAttachmentUrlAddress(state, ownProps),
      urlExists: CommonChannelSelectors.getChannelUrlAttachmentUrlExists(state, ownProps),
      urlIsFetching: CommonSelectors.getUrlAttachmentIsFetching(state, ownProps),
      urlAreDataValid: CommonSelectors.getUrlAttachmentAreDataValid(state, ownProps),
  }
}

const actions = [
    commonChannelActions,
    commonActions
];
export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelAttachmentContainer));

