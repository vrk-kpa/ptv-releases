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
import React,  {Component, PropTypes} from 'react';
import PTVLabel from '../PTVLabel';
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent';
import * as TypingRulesHelper from '../PTVTypingRules';
import shortid from 'shortid';
import cx from 'classnames';
import {is} from 'immutable';
import styles from './styles.scss';

class PTVTextInput extends Component{

  id = '';

  constructor(props) {
    super(props);
    this.handleChange = this.handleChange.bind(this);
    this.handleLeave = this.handleLeave.bind(this);
    this.handleOnKeyUp = this.handleOnKeyUp.bind(this);
    this.state={
      ...this.state,
      componentValue : this.props.value || '',
      isChangingDirty : false,
      timer: 0
    };
    this.id = shortid.generate();
  }

  isCharacterAllowed = (event) => {
    if (TypingRulesHelper[this.props.typingRules] && TypingRulesHelper[this.props.typingRules](event.target.value)) {
      return true; 
    }

    return false;
  }

  handleChange = (event) => {
    if (this.props.typingRules && !this.isCharacterAllowed(event)) {
      return null;
    }
    
    if (this.props.changeCallback) {
      if (this.props.stopChangeTimeout && this.props.stopChangeTimeout > 0) {
        clearTimeout(this.state.timer);
        var eventTargetValue= event.target.value;
        this.setState({ timer: setTimeout( () => this.props.changeCallback(eventTargetValue), this.props.stopChangeTimeout )});
      } else {
        this.props.changeCallback(event.target.value);
      }
    }

    this.setState({ componentValue:event.target.value, isChangingDirty: true });
   }

  handleLeave = () => {
    if (this.props.blurCallback) {
      this.props.blurCallback(this.state.componentValue);
    }

    this.setState({ isChangingDirty: false });
  }

  handleOnKeyUp = (event) => {
    if (event.keyCode == 13 && this.props.onEnterCallBack) {
      this.props.onEnterCallBack()
    }
  }

  handleFocus =  (event) => {
    if (this.props.focusCallback) {
      this.props.focusCallback(event.target.value);
    }

    this.setState({ isChangingDirty: false });
  }

  render() {
    const { disabled, readOnly } = this.props;
    const value = this.state.isChangingDirty ? this.state.componentValue : this.props.value || '';

    if (readOnly && value === '') {
      return null 
    }
    return (
      <div className={ cx('label-input', this.props.componentClass, this.state.errorClass) }>

        {this.props.label ?
          <div className={this.props.labelClass}>
            <PTVLabel
              tooltip={this.props.tooltip} readOnly= {readOnly} htmlFor={this.props.name + '_' + (this.props.id || this.id)}>{getRequiredLabel(this.props)}
            </PTVLabel>
          </div>
        :null}
        {!readOnly ?
        <div className={ cx(this.props.inputclass, this.props.className) }>
          <input
            type="text"
            maxLength={ this.props.maxLength }
            className={cx("ptv-textinput form-control", this.props.className,  this.props.size)}
            id={ this.props.name + '_' + (this.props.id || this.id) }
            placeholder={ this.props.placeholder }
            value= { value }
            //onChange={ this.handleChange }
            onInput={ this.handleChange } // replaces onChange until react issue #7211 is fixed 
            onKeyUp={ this.handleOnKeyUp }
            onBlur={ this.handleLeave }
            onFocus= { this.handleFocus }     
            disabled={disabled}       
          />{this.props.children}
          { this.props.textMuted ? <small className='text-muted'>{ this.props.textMuted }</small> : null }
          <ValidatePTVComponent {...this.props} valueToValidate={ this.state.isChangingDirty ? this.state.componentValue : this.props.value || '' } />
        </div>
        :
        <div className={ this.props.inputclass }>
              <PTVLabel                
                  htmlFor={this.props.name + '_' + (this.props.id || this.id)}>
                  {this.props.value}
              </PTVLabel>
        </div>}
      </div>
    );
  }
};

PTVTextInput.propTypes = {
  label: PropTypes.string,
  componentClass: PropTypes.string,
  inputclass: PropTypes.string,
  name: PropTypes.string.isRequired,
  placeholder: PropTypes.string,
  textMuted: PropTypes.string,
  maxLength: PropTypes.number,
  onEnterCallBack: PropTypes.func,
  changeCallback: PropTypes.func,
  blurCallback: PropTypes.func,
  stopChangeTimeout: PropTypes.number,
  size: PropTypes.string,
  typingRules: PropTypes.string
}

PTVTextInput.defaultProps = {
  componentClass: '',
  inputclass: '',
  placeholder: '',
  value: '',
  id: undefined,
  size: 'full'
}

export default composePTVComponent(PTVTextInput);
