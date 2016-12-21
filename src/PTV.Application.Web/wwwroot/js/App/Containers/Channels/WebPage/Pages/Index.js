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
import React from 'react';
import {connect} from 'react-redux';

// containers
import Step1 from './Step1';
import PageContainer from '../../../Common/PageContainer';
import ChannelServiceStep from '../../Common/Pages/channelServiceStep';

// actions
import * as channelActions from '../Actions';
import * as commonActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// styles
import '../../Common/Styles/Container.scss';

// messages
import * as Messages from '../Messages';
import * as CommonMessages from '../../Common/Messages';
import { channelServicesMessages } from '../../Common/Messages';

// selectors
import * as CommonChannelSelectors from '../../Common/Selectors';
import * as CommonSelectors from '../../../Common/Selectors';

// types
import { channelTypes } from '../../Common/Helpers/types';

const Container = props => {
        const { step1IsFetching, step1AreDataValid, channelServiceStepIsFething, channelServiceStepAreDataValid, readOnly } = props;
        return (
            <PageContainer { ...props } className="card channel-page"
                    confirmDialogs = { [{ type: 'cancel', messages: [Messages.cancelMessages.buttonOk, Messages.cancelMessages.buttonCancel, Messages.cancelMessages.text] },
                                         { type: 'delete', messages: [Messages.deleteMessages.buttonOk, Messages.deleteMessages.buttonCancel, Messages.deleteMessages.text] },
                                         { type: 'save', messages: [CommonMessages.saveDraftMessages.buttonOk, CommonMessages.saveDraftMessages.buttonCancel, CommonMessages.saveDraftMessages.text] }] }
                    readOnly={ props.readOnly }
                    isTranslatable= { true }
                    deleteAction={ props.actions.deleteChannel }
                    publishAction={ props.actions.publishChannel }
                    saveAction={ props.actions.saveAllChanges }
                    removeServerResultAction = { props.actions.removeServerResult }
                    invalidateAllSteps= { props.actions.cancelAllChanges }
                    basePath='/channels'
                    searchPath={'/search/' + props.keyToState}
                    getEntityStatusSelector = { CommonChannelSelectors.getPublishingStatus }
                    statusEndpoint = 'channel/GetChannelStatus'
                    steps= { [{
                        mainTitle: Messages.messages.mainTitle,
                        mainTitleView: Messages.messages.mainTitleView,
                        mainText: Messages.messages.mainText,
                        mainTextView: Messages.messages.mainTextView,
                        subTitle: Messages.messages.subTitle1,
                        subTitleView: Messages.messages.subTitle1View,
                        saveStepAction: props.actions.saveStep1Changes,
                        loadAction: props.actions.getStep1,
                        isFetching: step1IsFetching,
                        areDataValid: step1AreDataValid,
                        child: Step1 },
                        {
                        subTitle: channelServicesMessages.title,
                        subTitleView: channelServicesMessages.title,
                        loadAction: props.actions.getChannelServiceStep,
                        isFetching: channelServiceStepIsFething,
                        areDataValid: channelServiceStepAreDataValid,
                        readOnlyVisible: true,
                        child: ChannelServiceStep
                        }]}/>
       );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.WEB_PAGE;
   return {
       step1IsFetching: CommonSelectors.getStep1isFetching(state, {keyToState}),
       step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState }),
       keyToState,
       channelServiceStepIsFething: CommonSelectors.getChannelServiceStepIsFetching(state, { keyToState }),     
       channelServiceStepAreDataValid: CommonSelectors.getChannelServiceStepAreDataValid(state, { keyToState }),   
  }
}

const actions = [
    channelActions,
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(Container);
