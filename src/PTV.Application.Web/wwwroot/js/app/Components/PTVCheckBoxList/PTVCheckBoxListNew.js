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
import PTVCheckBox from '../PTVCheckBox';
import PTVLabel from '../PTVLabel';
import { renderOptionsImmutable } from '../PTVComponent';
import ImmutablePropTypes from 'react-immutable-proptypes';
import {Map, List} from 'immutable';
import cx from 'classnames';
import styles from './styles.scss'
import shortid from 'shortid';
import { injectIntl, intlShape } from 'react-intl';

class PTVCheckBoxList extends Component {

  constructor(props) {
    super(props);
  }

  renderLabel = (option) => {
    const data = {...this.props, option}

    return renderOptionsImmutable(data);
  }

  renderCheckBox = (item, i) => {
    const onChange = (event) => this.props.onChangeBox(item.get('id'), event.target.checked);

    return (
      <li key={i}>
        <PTVCheckBox
          key={item.id}
          id={ item.get('id') }
          isSelectedSelector={ this.props.isSelectedSelector }
          checkBoxClass={item.customClass}
          onClick={ onChange}
          chbType={ this.props.chbType }
          >
            {this.renderLabel(item)}
        </PTVCheckBox>
      </li>
    );
  }

  render () {
    const { readOnly } = this.props;
    let hasSomeValue = this.props.selected.size;
    
    if (readOnly && !hasSomeValue) {
      return null;
    }
    
    return (
      <div className={this.props.checkBoxListClass}>
        <fieldset className={cx('checkbox-list checkbox-group')}>
          {this.props.label ?
            <div className={this.props.labelBoxClass}>
              <PTVLabel
                htmlFor={"checkbox-list-label" + this.props.id}
                labelClass={this.props.labelClass}
                tooltip={this.props.tooltip}
                readOnly={readOnly}
                > {this.props.label}
              </PTVLabel>
            </div>
          : null }

          {!readOnly && this.props.box.size > 0 ?
            <ul className= {cx(this.props.inputBoxClass, this.props.horizontalLayout ? 'checkbox-group-horizontal' : 'checkbox-group-vertical')}>
              {
                this.props.box.map(this.renderCheckBox).toArray()
              }
            </ul>    
          :null }
          
          {readOnly ?
            <PTVLabel                        
                  className = {cx(this.props.inputBoxClass)}> 
                  { 
                      //collection.map((x) => { return this.getSelectedMessages(x.props.children)})
                      this.props.box.map((x) => { return this.getSelectedMessages(x)})
                  }                     
            </PTVLabel>          
          : null }
          
        </fieldset>
      </div>
    );
  }
   
   getSelectedMessages(item)
   {     
      return item.isSelected ? this.renderLabel(item)  + " / " : null;     
      //return item.isSelected ? item.props.children + " / " : null;
   }  
}

PTVCheckBoxList.propTypes = {
  box: ImmutablePropTypes.list,
  selected: ImmutablePropTypes.map,
  label: PropTypes.any,
  labelClass: PropTypes.string,
  tooltip: PropTypes.string,
  horizontalLayout: PropTypes.bool,
  onChangeBox: PropTypes.func.isRequired,
  useFormatMessageData: PropTypes.bool,
  optionRenderer: PropTypes.func,
  chbType: PropTypes.string,
};

PTVCheckBoxList.defaultProps = {
  useFormatMessageData: false,
  selected: new Map(),
  box: List(),
  id: 'checkBoxListType'
};

export default injectIntl(PTVCheckBoxList);
