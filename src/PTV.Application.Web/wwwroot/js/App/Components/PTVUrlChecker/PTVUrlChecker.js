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
import React,  {Component} from 'react';
import cx from 'classnames';
import PTVTextInput from '../PTVTextInput';
import PTVButton from '../PTVButton';
import PTVLabel from '../PTVLabel';
import PTVIcon from '../PTVIcon';
import PTVPreloader from '../PTVPreloader';
import * as ValidationHelper from '../PTVValidations';
import * as PTVValidatorTypes from '../PTVValidators';
import '../Styles/PTVCommon.scss';
import styles from './styles.scss';
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent';
import { injectIntl } from 'react-intl';

class PTVUrlChecker extends Component{

    constructor(props) {
        super(props);
        this.state = {
                    urlValue: this.props.value || '',
                    buttonDisabled: this.props.value ? this.props.value.length == 0 : true
                };
        this.handleUrlChange = this.handleUrlChange.bind(this);
        this.handleCheckUrl = this.handleCheckUrl.bind(this);
        this.blurCallback = this.blurCallback.bind(this);
    }

    handleUrlChange(value){
        this.setState({urlValue:value, buttonDisabled:value.length == 0});
        if(this.props.changeCallback){
            this.props.changeCallback(value, this.props.id);
        }
    }

    handleCheckUrl(){
        this.props.checkUrlCallback(this.state.urlValue, this.props.id);
    }

    blurCallback(){
        this.props.blurCallback(this.state.urlValue, this.props.id);
    }

    validators = [PTVValidatorTypes.IS_URL];

    render() {
        const { disabled, readOnly, translationMode } = this.props;

        if (readOnly && !this.props.value) {
            return null;
        }

        const { formatMessage } = this.props.intl;

        return (

            <div className={this.props.componentClass}>

                <div className="row">

                    <div className="col-xs-12">
                        <PTVLabel
                            tooltip = { formatMessage(this.props.messages.tooltip) }
                            readOnly = {readOnly}
                            htmlFor = {this.props.name + '_' + (this.props.id || this.id)}>
                            {getRequiredLabel(this.props, formatMessage(this.props.messages.label), this.props.inputValidators )}
                        </PTVLabel>
                    </div>

                    <div className={readOnly || translationMode === "view" ? "col-xs-12" : "col-sm-6 col-lg-7"}>
                        <PTVTextInput
                            componentClass = { cx("url-check-input", { "wrap-content": readOnly }) }
                            //label = { this.props.label }
                            value = { this.props.value }
                            blurCallback = { this.blurCallback }
                            changeCallback = { this.handleUrlChange }
                            onEnterCallBack = { this.props.onEnterCallBack }
                            maxLength = { this.props.maxLength }
                            name = "UrlChecker"
                            validators = { this.props.inputValidators ? this.props.inputValidators.concat(this.validators) : this.validators }
                            validatedField = { formatMessage(this.props.messages.label) }
                            order = { this.props.order }
                            tooltip = { formatMessage(this.props.messages.tooltip) }
                            placeholder = { formatMessage(this.props.messages.placeholder) }
                            readOnly = { readOnly }
                            disabled = { disabled } />
                    </div>

                    {!readOnly && !disabled ?
                        <div className="col-xs-8 col-sm-4 col-lg-4">
                            <PTVButton
                                onClick = { this.handleCheckUrl }
                                disabled = { this.state.buttonDisabled || this.props.showPreloader}
                                secondary>
                                    { formatMessage(this.props.messages.button) }
                            </PTVButton>
                        </div>
                    : null}

                    {!readOnly && !disabled ?
                        <div className="col-xs-4 col-sm-2 col-lg-1">
                            { this.props.showMessage ?
                                <PTVIcon
                                    tooltip = { this.props.urlExists == null ? formatMessage(this.props.messages.checkerInfo) : null }
                                    className = { this.props.urlExists != null ? this.props.urlExists ? 'color-leaf' : 'color-chili-crimson' : 'color-entrepreneur' }
                                    name = { this.props.urlExists != null ? (this.props.urlExists ? 'icon-check' : 'icon-cross') : 'icon-info' }
                                />
                            : this.props.showPreloader ?
                                <PTVPreloader className="small" />
                            : null }
                        </div>
                    : null }

                </div>

            </div>
        );
    }
};

PTVUrlChecker.propTypes = {
  label: React.PropTypes.string,
  button: React.PropTypes.string,
  value: React.PropTypes.string,
  order: React.PropTypes.number,
  inputValidators: React.PropTypes.array,
  onEnterCallBack: React.PropTypes.func,
  changeCallback: React.PropTypes.func,
  blurCallback: React.PropTypes.func,
  maxLength: React.PropTypes.number,
}

PTVUrlChecker.defaultProps = {
  maxLength: 100
}

export default injectIntl(composePTVComponent(PTVUrlChecker));
