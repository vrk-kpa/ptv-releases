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
import PTVButton from '../../../../../Components/PTVButton';
import cx from 'classnames';
import { Link, browserHistory } from 'react-router';

export const ServiceAndChannelInfoBox = (props) => {
  const componentClass = cx('header-section', props.componentClass);
  const { descriptionClass, buttonClass, showButton } = props;

  const buttonClick = (value) => {
      
      if (props.buttonClick){
          props.buttonClick(value)
      }
      
      if (props.buttonRoute){
          browserHistory.push({ pathname : props.buttonRoute});
      }     
  }

  return (
    <div className={componentClass}>
      <p className={descriptionClass}>{props.description}</p>
      
      {(showButton) ?        
          <PTVButton disabled={ props.buttonDisabled }
             onClick={ buttonClick }>{props.buttonText}
          </PTVButton>          
      : null }
      
    </div>
  );
};

ServiceAndChannelInfoBox.defaultProps = {
  showButton: true
}

export default ServiceAndChannelInfoBox;
