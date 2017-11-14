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
import { renderOptions } from '../PTVComponent';
import Immutable from 'immutable';
import cx from 'classnames';
import styles from './styles.scss'
import shortid from 'shortid';
import { injectIntl, intlShape } from 'react-intl';

class PTVCheckBoxList extends Component {

  constructor(props) {
    super(props);
    this.state = {
      list: this.props.box
    }
    this.renderCheckBox = this.renderCheckBox.bind(this);
  }

  renderLabel = (option) => {
    const data = {...this.props, option}

    return renderOptions(data);
  }

  renderCheckBox(item, i) {
    var self = this;
    var onChange = function (event) {
      var value = event.target.checked;
      self.props.box.forEach((sub) => {
        if (sub.id == item.id){
          sub.isSelected=value;
        }
      });
      self.props.onChangeBox(Immutable.fromJS(self.props.box));
    };

    return (
      <li key={i}>
        <PTVCheckBox
          key={item.id}
          id={ this.props.id + '_' + shortid.generate()}
          isSelected={item.isSelected}
          checkBoxClass={item.customClass}
          onClick={onChange}
          chbType={ this.props.chbType }
          >
            {this.renderLabel(item)}
        </PTVCheckBox>
      </li>
    );
  }

  isCheckBoxSelected = (chb) => {
    return chb.isSelected === true;
  }

  render () {
    var collection = this.props.box.map(this.renderCheckBox);
    const { readOnly } = this.props;
    let hasSomeValue = this.props.box.some(this.isCheckBoxSelected);
    
    if (readOnly && !hasSomeValue) {
      return null;
    }
    
    return (
      <div className={this.props.checkBoxListClass}>
        <fieldset className={cx('checkbox-list checkbox-group')}>
          {this.props.label ?
            <div className={this.props.labelClass}>
              <PTVLabel
                htmlFor={"checkbox-list-label" + this.props.id}
                tooltip={this.props.tooltip}
                readOnly={readOnly}
                > {this.props.label}
              </PTVLabel>
            </div>
          : null }

          {!readOnly && this.props.box.length > 0 ?
            <ul className= {cx(this.props.boxClass, this.props.horizontalLayout ? 'checkbox-group-horizontal' : 'checkbox-group-vertical')}>
              {collection}
            </ul>    
          :null }
          
          {readOnly ?
            <PTVLabel                        
                  className = {cx(this.props.boxClass)}> 
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
  box: PropTypes.array,
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
  id: 'checkBoxListType'
};

export default injectIntl(PTVCheckBoxList);
