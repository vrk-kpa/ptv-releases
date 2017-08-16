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
import cx from 'classnames';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

//Components
import { ButtonCancel, ButtonContinue } from '../../../../Common/Buttons';
import { PTVOverlay } from '../../../../../Components';

//Actions
import * as commonServiceAndChannelActions from '../../Common/Actions'

//Styles
import * as overlayStyles from './OverlayStyles';

//Messages
import * as messages from '../../Common/Messages';

export const ServiceAndChannelConfirmOverlay = props => {
    
    const { formatMessage } = props.intl;
    const { componentClass, title, description, acceptCallback, cancelCallback, closeCallBack, isVisible, actions
          } = props;
          
    const onDefaultClose = () =>{
         actions.clearRelationsConfirmation();
    }
 
 return (     
    <div className={componentClass}>
        <PTVOverlay
                title= {title ? title : formatMessage(messages.confirmationMessages.serviceConfirmationTitle) }
                dialogStyles= { overlayStyles.confirmationDialogStyles }
                overlayStyles = { overlayStyles.overlayStyles }
                contentStyles = { overlayStyles.contentStyles }
                isVisible = { isVisible }
                onCloseClicked = { closeCallBack ? closeCallBack : onDefaultClose }
            >
            {props.isVisible ?
                <div>
                    <p className='text'>{description}</p>
                    <div className='button-group centered'>
                        <ButtonContinue onClick={ acceptCallback }>{formatMessage(messages.confirmationMessages.confirmAcceptButton)} </ButtonContinue>              
                        <ButtonCancel onClick={ cancelCallback ? cancelCallback : onDefaultClose }> {formatMessage(messages.confirmationMessages.confirmCancelButton)} </ButtonCancel>                   
                    </div>                    
                </div>
            : null }
        </PTVOverlay>      
    </div>    
 );     
}

ServiceAndChannelConfirmOverlay.propTypes = {
        actions: PropTypes.object
    };

const actions = [
    commonServiceAndChannelActions,
];

export default connect(null, mapDispatchToProps(actions))(injectIntl(ServiceAndChannelConfirmOverlay));
