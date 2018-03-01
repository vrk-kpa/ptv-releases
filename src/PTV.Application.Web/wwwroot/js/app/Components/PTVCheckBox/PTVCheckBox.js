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
import React, {Component, PropTypes} from 'react';
import { connect } from 'react-redux';
import styles from './styles.scss';
import PTVLabel from '../PTVLabel';
import cx from 'classnames';
import { composePTVComponent } from '../PTVComponent';

export const PTVCheckBox = ({id, isSelected, isDisabled, small, chbType, className, onClick, readOnly, showCheck, children, labelClass}) => {
  const clazz = cx({
    'checked': isSelected,
    'disabled': isDisabled,
    'small': small,
    'ptv-checkbox': true,
  }, chbType, className);
// console.log('checkbox')
  return (
    !readOnly ?
    <div className={clazz}>
        <PTVLabel
          htmlFor={id} labelClass={ labelClass }
        >{!readOnly?
          <input
            type="checkbox"
            id={id}
            onClick={onClick}
            checked={isSelected}
            disabled={isDisabled}
            onChange={()=>null}
          />:null}
          {!readOnly?showCheck?<div className="check"></div>:null:null}
          <span className="option">{children}</span>
        </PTVLabel>
    </div>
    : readOnly && isSelected ?
      <PTVLabel>
        <span className="option">{children}</span>
      </PTVLabel>
    : null
  );
};


PTVCheckBox.propTypes = {
  id: PropTypes.string.isRequired,
  isSelected: PropTypes.bool.isRequired,
  onClick: PropTypes.func.isRequired,
  verticalLabelLayout: PropTypes.bool,
  isDisabled: PropTypes.bool,
  showCheck: PropTypes.bool,
  isSelectedSelector: PropTypes.func,
  isDisabledSelector: PropTypes.func,
  children: PropTypes.any,
  className: PropTypes.string,
  chbType: PropTypes.string,
  small: PropTypes.bool,
};

PTVCheckBox.defaultProps = {
  isDisabled: false,
  isSelected: false,
  chbType: 'default',
  showCheck: true,
  id: 'checkBox',
  small: false
};

function mapStateToProps(state, ownProps) {
  const isDisabled = ownProps.isDisabled ? true : ownProps.isDisabledSelector ? ownProps.isDisabledSelector(state, ownProps) : false;
  const isSelected = ownProps.isSelectedSelector ? ownProps.isSelectedSelector(state, ownProps) : false;
  return {
    isSelected,
    isDisabled
  }
}

export default connect(mapStateToProps)(composePTVComponent(PTVCheckBox));
