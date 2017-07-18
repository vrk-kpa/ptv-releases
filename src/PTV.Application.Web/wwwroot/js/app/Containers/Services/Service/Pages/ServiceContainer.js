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
import React, { PropTypes, Component } from 'react';
import { compose } from 'redux';
import { connect } from 'react-redux';
import { injectIntl } from 'react-intl'

/// Actions
import * as serviceActions from '../Actions';
import * as commonActions from '../../../Common/Actions';

/// App Configuration
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

/// App Containers
import PageContainer from '../../../Common/PageContainer';
import StepContainer from '../../../Common/StepContainer';

import Step1 from './Steps/ServiceStep1Container';
import Step2 from './Steps/ServiceStep2Container';
import Step3 from './Steps/ServiceStep3Container';
import Step4 from './Steps/ServiceStep4Container';

/// Styles
import '../../Styles/LandingPage.scss';
import '../../Styles/StepContainer.scss';

/// Messages
import * as Messages from '../Messages';

/// Selectors
import * as CommonSelectors from '../../../Common/Selectors';
import * as ServiceSelectors from '../Selectors';

const ServiceContainer = props => {
        const { step1IsFetching, step2IsFetching, step3IsFetching, step4IsFetching,
            step1AreDataValid, step2AreDataValid, step3AreDataValid, step4AreDataValid, resultModel, keyToState } = props;

        return (
            <PageContainer {...props} className="card service-page"
                    confirmDialogs = { [{ type: 'delete', messages: [Messages.deleteMessages.buttonOk, Messages.deleteMessages.buttonCancel, Messages.deleteMessages.text] },
                                        { type: 'cancel', messages: [Messages.cancelMessages.buttonOk, Messages.cancelMessages.buttonCancel, Messages.cancelMessages.text] },
                                        { type: 'save', messages: [Messages.saveDraftMessages.buttonOk, Messages.saveDraftMessages.buttonCancel, Messages.saveDraftMessages.text] },
                                        { type: 'withdraw', messages: [Messages.withdrawMessages.buttonOk, Messages.withdrawMessages.buttonCancel, Messages.withdrawMessages.text] },
                                        { type: 'restore', messages: [Messages.restoreMessages.buttonOk, Messages.restoreMessages.buttonCancel, Messages.restoreMessages.text] }
                                        ] }
                    readOnly={ props.readOnly }
                    keyToState = { keyToState }
                    isTranslatable = { true }
                    deleteAction={ props.actions.deleteService }
                    withdrawAction={ props.actions.withdrawService }
                    restoreAction={ props.actions.restoreService }
                    saveAction={ props.actions.saveAllChanges }
                    removeServerResultAction = { props.actions.removeServerResult }
                    lockAction={ props.actions.lockService }
                    unLockAction={ props.actions.unLockService }
                    isLockedAction={ props.actions.isServiceLocked }
                    isEditableAction={ props.actions.isServiceEditable }
                    basePath = '/frontpage'
                    entitiesType='services'
                    getEntityStatusSelector = { ServiceSelectors.getPublishingStatus }
                    getUnificRootIdSelector = { ServiceSelectors.getUnificRootId }
                    statusEndpoint = 'service/GetServiceStatus'
                    invalidateAllSteps= { props.actions.cancelAllChanges }
                    steps= { [{
                        mainTitle: Messages.messages.mainTitle,
                        mainTitleView: Messages.messages.mainTitleView,
                        mainText: Messages.messages.mainText,
                        mainTextView: Messages.messages.mainTextView,
                        subTitle: Messages.messages.subTitle1,
                        subTitleView: Messages.messages.subTitle1View,
                        saveStepAction: props.actions.saveStep1Changes,
                        loadAction: props.actions.loadAddServiceStep1,
                        isFetching: step1IsFetching,
                        areDataValid: step1AreDataValid,
                        child: Step1
                    },
                    {
                      subTitle: Messages.messages.subTitle2,
                      subTitleView: Messages.messages.subTitle2View,
                      saveStepAction: props.actions.saveStep2Changes,
                      loadAction: props.actions.loadAddServiceStep2,
                      isFetching: step2IsFetching,
                      areDataValid: step2AreDataValid,
                      child: Step2
                     },
                     {
                      subTitle: Messages.messages.subTitle3,
                      subTitleView: Messages.messages.subTitle3View,
                      saveStepAction: props.actions.saveStep3Changes,
                      loadAction: props.actions.loadAddServiceStep3,
                      isFetching: step3IsFetching,
                      areDataValid: step3AreDataValid,
                      child: Step3
                     },
                     {
                       subTitle: Messages.messages.subTitle4,
                       subTitleView: Messages.messages.subTitle4,
                       saveStepAction: props.actions.saveStep4Changes,
                       loadAction: props.actions.loadStep4ChannelData,
                       isFetching: step4IsFetching,
                       areDataValid: step4AreDataValid,
                       child: Step4,
                       readOnlyVisible: true
                      }
                     ] }>
        </PageContainer>
       );
    }

ServiceContainer.propTypes = {
            actions: PropTypes.object,
            // readOnly: PropTypes.bool.isRequired
        };

function mapStateToProps(state, ownProps) {
    const keyToState = 'service';
   return {
       step1IsFetching: CommonSelectors.getStep1isFetching(state, { keyToState }),
       step2IsFetching: CommonSelectors.getStep2isFetching(state, { keyToState }),
       step3IsFetching: CommonSelectors.getStep3isFetching(state, { keyToState }),
       step4IsFetching: CommonSelectors.getStep4isFetching(state, { keyToState }),
       step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState, simpleView:ownProps.simpleView }),
       step2AreDataValid: CommonSelectors.getStep2AreDataValid(state, { keyToState, simpleView:ownProps.simpleView }),
       step3AreDataValid: CommonSelectors.getStep3AreDataValid(state, { keyToState, simpleView:ownProps.simpleView }),
       step4AreDataValid: CommonSelectors.getStep4AreDataValid(state, { keyToState, simpleView:ownProps.simpleView }),
       keyToState
  }
}

const actions = [
    serviceActions,
    commonActions
];

export default compose(
  injectIntl,
  connect(mapStateToProps, mapDispatchToProps(actions))
)(ServiceContainer)
