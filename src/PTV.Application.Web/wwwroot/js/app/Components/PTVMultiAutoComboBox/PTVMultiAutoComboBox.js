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
import Select from 'react-select';
import { defineMessages, injectIntl, intlShape, formattedMessage } from 'react-intl';
import PTVLabel from '../PTVLabel';
// import styles from './styles.scss';

const messages = defineMessages({
    placeholder: {
        id: "Components.AutoCombobox.Placeholder",
        defaultMessage: "- valitse -"
    }
});

export class PTVMultiAutoComboBox extends Component {

    constructor(props) {
        super(props);
        this.updateValue = this.updateValue.bind(this);
    }

    updateValue(selectedItems) {
        if (this.props.changeCallback){
            this.props.changeCallback(
                selectedItems
            );
        }
    }

    render ()
    {
        const {formatMessage} = this.props.intl;
        return(
            <div className={"auto-combo-box " + this.props.componentClass}>
              <PTVLabel htmlFor={"combo" + this.props.id} labelClass={this.props.labelClass} tooltip={this.props.tooltip}>{this.props.label}</PTVLabel>
              <div className = {this.props.autoClass} id={"combo" + this.props.id}>
              <Select
                  name="form-field-name"
                  options={this.props.values}
                  valueKey="id"
                  labelKey="name"
                  value={this.props.value}
                  onChange={this.updateValue}
                  placeholder={formatMessage(messages.placeholder)}
                  mutli={true}
                  simpleValue={true}
              />
              </div>
            </div>
    )}
};

PTVMultiAutoComboBox.propTypes = {
  intl: intlShape.isRequired,
  changeCallback: PropTypes.func
};

export default injectIntl(PTVMultiAutoComboBox);
