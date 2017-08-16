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
import Email from './EmailContainer';
import { defineMessages, FormattedMessage } from 'react-intl';
import ImmutablePropTypes from 'react-immutable-proptypes';
import PTVButton from '../../../Components/PTVButton';

const messages = defineMessages({
    addEmail: {
        id: 'Containers.Channels.EmailGrid.AddButton.Title',
        defaultMessage: 'Lisää uusi sähköpostiosoite'
    }
});

let EmailGrid = ({emails, emailMessages, onEmailChange, onInfoChange, onAddClick, readOnly, onRemoveButtonClick}) => (
        <div className="row form-group">
            <div className="col-xs-12">
                {emails.map(email =>
		        <Email
                   key={ email.id }
                   email = { email }
                   onEmailChange = { onEmailChange }
                   onAdditionalInfoChange = { onInfoChange }
                   onRemoveButtonClick = { onRemoveButtonClick }
                   messages={ emailMessages }
                   readOnly= { readOnly }
                />
                ).toArray()}
                { !readOnly ?
                <div className="button-group add-item">
                    <PTVButton onClick = { onAddClick } secondary>
                        <FormattedMessage {...messages.addEmail} />
                    </PTVButton>
                </div> : null }

            </div>
        </div>
    );

EmailGrid.propTypes = {
    onAddClick: PropTypes.func.isRequired,
    onEmailChange: PropTypes.func,   
}

export default EmailGrid;
