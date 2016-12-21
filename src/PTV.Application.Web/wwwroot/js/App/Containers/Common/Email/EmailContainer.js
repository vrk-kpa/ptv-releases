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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import ImmutablePropTypes from 'react-immutable-proptypes';
import classNames from 'classnames';
import Email from '../EmailComponent';
import Info from '../AdditionalInfoComponent'
import { ButtonDelete } from '../Buttons';

class EmailContainer extends Component {

    constructor(props){
        super(props);
    }

    static propTypes = {
        email: PropTypes.object.isRequired,
        onEmailChange: PropTypes.func,
        onAditionalInformationChange: PropTypes.func,
    };

    onEmailChange = (email) => {
        this.props.onEmailChange(this.props.email.id, email);
    };
    
    onAdditionalInfoChange = (info) => {
        this.props.onAdditionalInfoChange(this.props.email.id, info);
    };
    
     onRemoveButtonClick = () => {
        this.props.onRemoveButtonClick(this.props.email.id)
    }

    render () {
        const { formatMessage } = this.props.intl;
        const { email, componentClass, readOnly } = this.props;
        return (
            
            <div className= { classNames("email-container item-row", componentClass) }>
                <div className="row">
                   <Email
                     email = { email.email }                     
                     handleEmailChange = { this.onEmailChange }                     
                     maxLength = { this.props.maxLength }                     
                     name = { "email" }
                     label = { formatMessage(this.props.messages.label) }
                     tooltip = { formatMessage(this.props.messages.tooltip) }
                     placeholder = { formatMessage(this.props.messages.placeholder) }
                     isRequired = { email.additionalInformation && email.additionalInformation.length>0 }
                     readOnly = { readOnly }
                   />
                   { this.props.onAdditionalInfoChange ? 
                   <Info
                     info = { email.additionalInformation }
                     handleInfoChange = { this.onAdditionalInfoChange }
                     maxLength = { this.props.infoMaxLength }
                     name = { "emailAdditionalInfo" }
                     label = { formatMessage(this.props.messages.infoLabel) }
                     tooltip = { formatMessage(this.props.messages.infoTooltip) }
                     placeholder = { formatMessage(this.props.messages.infoPlaceholder) }
                     readOnly = { readOnly }
                   />
                   :null}
                </div>  
               { email.canBeRemoved && !readOnly?
                    
                    <div className="remove-item">
                        <ButtonDelete 
                            item={email.id}
                            onClick={this.onRemoveButtonClick}
                            secondary={true}
                            withIcon={true}
                            iconName="icon-times-circle" />
                    </div>
                    : null }         
            </div>
       );
    };
};

export default injectIntl(EmailContainer);
