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
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';

import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

/// App Containers
import StepContainer from '../../../../Common/StepContainer';
import PageContainer from '../../../../Common/PageContainer';

import Step1 from './Steps/Step1';

//Actions
import * as organizationActions from '../Actions';

/// Styles
import '../../../../Common/Styles/StepContainer.scss';

//Messages
import * as Messages from '../Messages';

//Selectors
import * as CommonOrganizationSelectors from '../../Common/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';

const ManageContainer = props => {
    
        const { step1IsFetching, step1AreDataValid } = props;
        
        return (
            <PageContainer {...props} className="card service-page"
                    confirmDialogs = { [{ type: 'delete', messages: [Messages.deleteMessages.buttonOk, Messages.deleteMessages.buttonCancel, Messages.deleteMessages.text] },
                                        { type: 'cancel', messages: [Messages.cancelMessages.buttonOk, Messages.cancelMessages.buttonCancel, Messages.cancelMessages.text] },
                                        { type: 'save', messages: [Messages.saveDraftMessages.buttonOk, Messages.saveDraftMessages.buttonCancel, Messages.saveDraftMessages.text] }] }
                    readOnly={ props.readOnly }
                    //isTranslatable= { true }
                    deleteAction={ props.actions.deleteOrganization }
                    publishAction={ props.actions.publishOrganization }
                    saveAction={ props.actions.saveAllChanges }
                    removeServerResultAction = { props.actions.removeServerResult }
                    basePath = '/manage'
                    getEntityStatusSelector = { CommonOrganizationSelectors.getPublishingStatus }
                    statusEndpoint = 'organization/GetOrganizationStatus'
                    invalidateAllSteps= { props.actions.cancelAllChanges }
                    steps= { [{
                        mainTitle: Messages.messages.mainTitle,
                        mainTitleView: Messages.messages.mainTitleView,
                        mainText: Messages.messages.mainText,
                        mainTextView: Messages.messages.mainTextView,
                        subTitle: Messages.messages.subTitleStep1,
                        subTitleView: Messages.messages.subTitleViewStep1,
                        saveStepAction: props.actions.saveStep1Changes,
                        loadAction: props.actions.getStep1,
                        isFetching: step1IsFetching,
                        areDataValid: step1AreDataValid,
                        child: Step1
                        }
                     ] }>
        </PageContainer>
       );
    }

ManageContainer.propTypes = {
            actions: PropTypes.object            
        };

function mapStateToProps(state, ownProps) {
    const keyToState = 'organization';
    return {        
       step1IsFetching: CommonSelectors.getStep1isFetching(state, {keyToState}),
       step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState: 'organization' }),
       keyToState
  }
}

const actions = [
    organizationActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(ManageContainer);
