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
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';

// actions
import * as channelActions from '../Actions';
import * as commonActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import Step1 from './Steps1';
import Step2 from './Steps2';
import Step3 from './Steps3';
import Step4 from './Steps4';
import PageContainer from '../../../Common/PageContainer';
import ChannelServiceStep from '../../Common/Pages/channelServiceStep';

// messages
import * as CommonMessages from '../../Common/Messages';
import * as Messages from '../Messages';
import { channelServicesMessages } from '../../Common/Messages';

// styles
import '../../Common/Styles/Container.scss';

// types
import { channelTypes } from '../../Common/Helpers/types';

// selectors
import * as ChannelSelectors from '../Selectors';
import * as CommonChannelSelectors from '../../Common/Selectors';
import * as CommonSelectors from '../../../Common/Selectors';

const LocationChannelContainer = props => {
    const { step1IsFetching, step2IsFetching, step3IsFetching, step4IsFetching, step1AreDataValid, step2AreDataValid, step3AreDataValid, step4AreDataValid, channelServiceStepIsFething, channelServiceStepAreDataValid, readOnly } = props;       
    
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
                statusEndpoint = 'channel/GetChannelStatus'
                basePath='/channels'
                searchPath={'/search/' + props.keyToState}
                getEntityStatusSelector = { CommonChannelSelectors.getPublishingStatus }
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
                    child: Step1
                    },{ 
                    subTitle: Messages.messages.subTitle2,
                    subTitleView: Messages.messages.subTitle2View,
                    saveStepAction: props.actions.saveStep2Changes,
                    loadAction: props.actions.getStep2,
                    isFetching: step2IsFetching,
                    areDataValid: step2AreDataValid,
                    child: Step2
                        },{ 
                    subTitle: Messages.messages.subTitle3,
                    subTitleView: Messages.messages.subTitle3View,
                    saveStepAction: props.actions.saveStep3Changes,
                    loadAction: props.actions.getStep3,
                    isFetching: step3IsFetching,
                    areDataValid: step3AreDataValid,
                    child: Step3
                        },{ 
                    subTitle: Messages.messages.subTitle4,
                    subTitleView: Messages.messages.subTitle4View,
                    saveStepAction: props.actions.saveStep4Changes,
                    loadAction: props.actions.getStep4,
                    isFetching: step4IsFetching,
                    areDataValid: step4AreDataValid,
                    child: Step4
                        },{
                    subTitle: channelServicesMessages.title,
                    subTitleView: channelServicesMessages.title,
                    loadAction: props.actions.getChannelServiceStep,
                    isFetching: channelServiceStepIsFething,
                    areDataValid: channelServiceStepAreDataValid,
                    readOnlyVisible: true,
                    child: ChannelServiceStep
                    }]}
            />
    );
}

function mapStateToProps(state, ownProps) {
    const keyToState = channelTypes.SERVICE_LOCATION;
   return {
       step1IsFetching: CommonSelectors.getStep1isFetching(state, {keyToState}),
       step2IsFetching: CommonSelectors.getStep2isFetching(state, {keyToState}),     
       step3IsFetching: CommonSelectors.getStep3isFetching(state, {keyToState}),     
       step4IsFetching: CommonSelectors.getStep4isFetching(state, {keyToState}),
       step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState }),
       step2AreDataValid: CommonSelectors.getStep2AreDataValid(state, { keyToState }),
       step3AreDataValid: CommonSelectors.getStep3AreDataValid(state, { keyToState }),
       step4AreDataValid: CommonSelectors.getStep4AreDataValid(state, { keyToState }),
       keyToState,
       channelServiceStepIsFething: CommonSelectors.getChannelServiceStepIsFetching(state, { keyToState }),     
       channelServiceStepAreDataValid: CommonSelectors.getChannelServiceStepAreDataValid(state, { keyToState }),         
  }
}

const actions = [
     channelActions,
     commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(LocationChannelContainer);
