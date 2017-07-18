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
import * as serviceAndChannelActions from '../../ServiceAndChannel/Actions';
import * as commonActions from '../../Common/Actions';

// components
import { ButtonCancel, ButtonContinue, ButtonSave, ButtonArchive, ButtonPublish, ButtonUpdate } from '../../../../Common/Buttons';
import ServiceAndChannelConfirmOverlay from '../../Common/Pages/ServiceAndChannelConfirmOverlay';

// selectors
import * as CommonServiceAndChannelsSelectors from '../Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';

//meassages
import { buttonMessages, confirmationMessages } from '../Messages';
import { detailMessages } from '../../ServiceSearch/Messages';

// types
import { confirmationTypes } from '../Helpers/confirmationTypes';

export const ServiceAndChannelButtons =  ({ showButtons, readOnly, buttonSaveUp ,intl, actions, confirmationData, anyDraft, isFetching, language, channelRelationIds }) => {
    const { formatMessage } = intl;

    const handleSave = () =>{
        actions.saveRelations(language);
    }

    const handlePublish = () =>{
        actions.publishRelations(language);
    }

    const handleReadOnly = (readOnly) => () =>{
        //Collapse all channelRelations
        readOnly ? channelRelationIds.forEach(id => actions.onChannelRelationInputChange('showingAdditional', id, false)) : null;
        actions.setRelationsReadOnlyMode(readOnly);
    }

    const handleCancel = () =>{
        actions.setRelationsConfirmation(confirmationTypes.CLEAR_ALL_RELATINS, true);

    }

    const onClearServiceAndChannelsAccept = () =>{
        actions.onServiceAndChannelsListRemove();
        actions.onChannelRelationsEntityClearAll();
        actions.clearRelationsConfirmation();
    }

    return (
            showButtons?
            <div>
                {buttonSaveUp?
                <div  className="button-group">
                    {readOnly?
                     <ButtonUpdate onClick = {handleReadOnly(false)} disabled = {isFetching}/>
                    :<ButtonSave onClick = {handleSave} disabled = {isFetching}/>}
                </div>:null}
                {readOnly?
                    <div className="button-group">
                        {/*<ButtonPublish  onClick = {handlePublish} disabled = {isFetching || !anyDraft}>
                             {formatMessage(buttonMessages.publishButton)}
                        </ButtonPublish>*/}
                    </div>
                    :<div>
                        <ServiceAndChannelConfirmOverlay
                            isVisible = {confirmationData.has(confirmationTypes.CLEAR_ALL_RELATINS)}
                            description = {formatMessage(confirmationMessages.clearAllServiceAndChannelsTitle)}
                            acceptCallback = { () => onClearServiceAndChannelsAccept(confirmationData.get(confirmationTypes.CLEAR_ALL_RELATINS)) }
                        />
                        <div className="button-group">
                            <ButtonCancel onClick = {handleCancel} disabled = {isFetching}/>
                            <ButtonContinue  onClick = {handleReadOnly(true)} disabled = {isFetching}>
                                {formatMessage(buttonMessages.continueButton)}
                            </ButtonContinue>
                        </div>
                     </div> }
                {!buttonSaveUp?<div  className="button-group">
                    {readOnly?
                     <ButtonUpdate onClick = {handleReadOnly(false)} disabled = {isFetching}/>
                    :<ButtonSave onClick = {handleSave} disabled = {isFetching}/>}
                </div>:null}
            </div>:null
       );
}

ServiceAndChannelButtons.propTypes = {

};

function mapStateToProps(state, ownProps) {
  return {
     showButtons: CommonServiceAndChannelsSelectors.getRelationConnectedServicesIdsSize(state, ownProps) > 0,
     isFetching: CommonSelectors.getStepCommonIsFetching(state,ownProps),
     confirmationData: CommonServiceAndChannelsSelectors.getConfirmation(state, {keyToState:'serviceAndChannelConfirmation'}),
     anyDraft: CommonServiceAndChannelsSelectors.AnyConnectedServiceOrChannelIsPublishable(state, ownProps),
     channelRelationIds: CommonServiceAndChannelsSelectors.getRelationChannelRelationsIds(state, ownProps),
    }
}

const actions = [
    serviceAndChannelActions,
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceAndChannelButtons));
