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
import React, {PropTypes} from 'react';
import PTVRadioButton from './PTVRadioButton';
import PTVLabel from '../PTVLabel';
import cx from 'classnames';
import { renderOptions, getRequiredLabel } from '../PTVComponent';
import { injectIntl, intlShape } from 'react-intl';

class PTVRadioGroup extends React.Component {

  handleChange = value => {
    if (this.props.onChange) this.props.onChange(value);
  };

  renderOption = (option) => {
      const data = { ...this.props, option}
      return renderOptions(data);
  }
  
  renderRadioButtons() {
    return this.props.items.map((item, idx) => {
      return (
        <li key={item.id}>
          <PTVRadioButton
            checked={item.id === this.props.value}
            disabled={this.props.disabled}
            small={this.props.small}
            key={item.id}
            label={this.renderOption(item)}
            onChange={this.handleChange} // eslint-disable-line react/jsx-no-bind
            value={item.id}
            name={this.props.name} />
            { this.props.showChildren === idx && this.props.value === item.id ? 
              this.props.children
            : null }            
        </li>
      );
    });
  }

  getCheckedMessage = (props) => {      
    const checkedValue = props.value;
                                
    return (props.items || []).filter(item => {
      const value = ((typeof item === 'string') ? item : item.id);
      return checkedValue == value;
    }).map((item, index, arr) => ( this.renderOption(item) ))
  }

  render() {    
    return (
      <div className={cx(this.props.className, {"horizontal": !this.props.verticalLayout}, "ptv-radiogroup")}>
        { !this.props.readOnly && this.props.radioGroupLegend ? 
          <PTVLabel tooltip = {this.props.tooltip} readOnly={this.props.readOnly}> 
            {getRequiredLabel(this.props, this.props.radioGroupLegend)}
          </PTVLabel>
        : null}
        { !this.props.readOnly ?
          <ul>
            {this.renderRadioButtons()}
          </ul>
        : this.props.showDefaultInReadonly ?
          <div>
            <div>
              <PTVLabel labelClass="main"> 
                {getRequiredLabel(this.props, this.props.radioGroupLegend)}
              </PTVLabel>
            </div>
            <div>
              <PTVLabel> 
                {this.getCheckedMessage(this.props)}
              </PTVLabel>
            </div>
          </div>
        : null }
      </div>
    );
  }
}

PTVRadioGroup.propTypes = {
  children: PropTypes.node,
  showChildren: PropTypes.number,
  className: PropTypes.string,
  disabled: PropTypes.bool,
  intl: intlShape.isRequired,
  items(props, name){
      if (!props.children && !props.items){
          return new Error('Your component has no children. In this case, you should specify an items array.')
      }
  },
  onChange: PropTypes.func,
  name: PropTypes.string.isRequired,
  small: PropTypes.bool,
  value: PropTypes.any,
  useFormatMessageData: PropTypes.bool,
  optionRenderer: PropTypes.func,
  verticalLayout: PropTypes.bool,
  showDefaultInReadonly: PropTypes.bool
};

PTVRadioGroup.defaultProps = {
  className: '',
  disabled: false,
  verticalLayout: false,
  showDefaultInReadonly: true
};

export default injectIntl(PTVRadioGroup);
