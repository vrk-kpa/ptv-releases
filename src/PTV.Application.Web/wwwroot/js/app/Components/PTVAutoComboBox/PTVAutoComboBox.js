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
import ReactDOM from 'react-dom';
import Select from 'react-select';
import VirtualizedSelect from 'react-virtualized-select';
import { defineMessages, injectIntl, intlShape, formattedMessage } from 'react-intl';
import PTVLabel from '../PTVLabel';
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel, renderOptions } from '../PTVComponent';
import shortid from 'shortid';
import styles from './styles.scss';

const messages = defineMessages({
  placeholder: {
    id: "Components.AutoCombobox.Placeholder",
    defaultMessage: "- valitse -"
  },
  searching: {
    id: "Components.AutoCombobox.Searching",
    defaultMessage: "Haetaan..."
  },
  noResults: {
    id: "Components.AutoCombobox.NoResults",
    defaultMessage: "Ei hakutuloksia"
  },
  typeToSearch: {
    id: "Components.AutoCombobox.TypeToSearch",
    defaultMessage: "Kirjoita hakusana"
  },
  clearValue: {
    id: "Components.AutoCombobox.ClearValue",
    defaultMessage: "Selkeää arvo"
  }
});

class PTVAutoComboBox extends Component {

  id = '';

  constructor(props) {
    super(props);
    this.updateValue = this.updateValue.bind(this);
    this.id = shortid.generate();
    this.renderOption = this.renderOption.bind(this);
    this.renderValue = this.renderValue.bind(this);
  }

  updateValue = (selectedItem) => {
    if (this.props.changeCallback){
      var id = selectedItem ? selectedItem.id : null;
      this.props.changeCallback(
        id,
        selectedItem
      );
    }
  }

  renderOption = (option) => {
    if (this.props.renderOption){
      return this.props.renderOption(option)
    }

    const data = { ...this.props, option}
    return renderOptions(data);
  }

  renderValue = (option) => {
    if (this.props.valueRenderer){
      return this.props.valueRenderer(option)
    }

    const data = { ...this.props, option}
    return renderOptions(data);
  }

  filterOption = (option, filter) => {
    if (this.props.filterOption) {
      if( !this.props.filterOption(option, filter) ) {
        return false;
      }
    }
      let optionValue = this.renderOption(option);
      if (optionValue == null){
        return false;
      }
      optionValue = optionValue.toString();
      if (this.props.ignoreCase === true) {
          optionValue = optionValue.toLowerCase();
      }
      return this.props.matchPos === 'start' ? optionValue.substr(0, filter.length) === filter : optionValue.indexOf(filter) !== -1;
  }

  // used for async combo, returns all data, filtering done by server
  filterOptions = (options) => options;

  getSelectedValue = () => {
    const { values, value } = this.props;
      if (Array.isArray(values)) {
        var index = values.findIndex((x) => { return (value == x.id) });
        return index > -1 ? values[index] : null;
      }
      return null;
  }

  // height needed for react-virtualized-select,
  // currently we compute number of rows based on average
  // characters count per row
  // TODO: check if better solution exists
  getDynamicHeight = (object) => {
    const base = 42;
    const avgPixels = 8;

    if (!this._offsetWidth) {
      return base;
    }

    if (object.option) {
      const tresholdCharCount = Math.ceil(this._offsetWidth / avgPixels);
      const optionCharCount = object.option.name && object.option.name.length;
      const rowCount = Math.ceil(optionCharCount / tresholdCharCount);
      if ( rowCount > 1 ) {
        return rowCount * base - 17;
      }
    }

    return base;
  }

  renderReadonlyLabel = (value, htmlId) => {
    let text = "";
    const selectedValue = value && value.id ?
        value :
        this.getSelectedValue();

    if (selectedValue){
        text = this.renderValue(selectedValue);

        return (
          <div className={this.props.autoClass} id={htmlId}>
                <PTVLabel
                    htmlFor={this.props.name + '_' + (this.props.id || this.id)}>
                    { text }
                </PTVLabel>
        </div>
        )
    }
    return null;
  }

  getVirtualSelectWidth = (refEl) => {
    const virtualSelect = ReactDOM.findDOMNode(refEl);
    return virtualSelect ? virtualSelect.offsetWidth : null;
  }

  componentDidUpdate() {
    this._offsetWidth = this.getVirtualSelectWidth(this.refs.virtualSelect);
  }

  componentDidMount() {
    this._offsetWidth = this.getVirtualSelectWidth(this.refs.virtualSelect);
  }

  render ()
  {
    const {formatMessage} = this.props.intl;
    const { readOnly } = this.props;
    const disabled = this.props.disabled === undefined ? false : this.props.disabled;

    const cntHtmlId = this.props.name + '_cnt_' + (this.props.id || this.id);
    const htmlInputId = this.props.name + '_' + (this.props.id || this.id);

    if (readOnly && !this.props.value) {
      return null;
    }

    return(
      <div id={"combo" + htmlInputId + "Container"} className={this.props.componentClass}>
        { this.props.label ?
        <div className={this.props.labelClass}>
          <div>
            <PTVLabel
              htmlFor={'combo' + htmlInputId}
              readOnly= {readOnly}
              tooltip={this.props.tooltip}
            >
              {getRequiredLabel(this.props)}
            </PTVLabel>
            {this.props.iconAction && this.props.iconAction()}
          </div>
        </div>
        :
        null }

          { !readOnly ?
          <div className={this.props.autoClass} id={cntHtmlId}>
          {this.props.async ?
                <Select.Async
                  name={htmlInputId}
                  className={this.props.className}
                  disabled={disabled || readOnly}
                  autoload={this.props.autoload}
                  cache={false}
                  loadOptions={this.props.values}
                  filterOptions={this.filterOptions}
                  optionRenderer={this.renderOption}
                  valueRenderer={this.renderValue}
                  valueKey="id"
                  labelKey="name"
                  matchPos={this.props.matchPos}
                  ignoreCase={this.props.ignoreCase}
                  ignoreAccents={false}
                  value={this.props.value}
                  clearable={this.props.clearable}
                  onChange={this.updateValue}
                  placeholder={this.props.placeholder ? this.props.placeholder : formatMessage(messages.placeholder)}
                  inputProps={{'id': 'combo' + htmlInputId, ...this.props.inputProps}}
                  searchingText={formatMessage(messages.searching)}
                  noResultsText={formatMessage(messages.noResults)}
                  clearValueText={formatMessage(messages.clearValue)}
                  searchPromptText={formatMessage(messages.typeToSearch)}
              />
              : !this.props.virtualized ?
                <Select
                name={htmlInputId}
                className={this.props.className}
                disabled={disabled || readOnly}
                options={this.props.values}
                filterOption={this.filterOption}
                optionRenderer={this.renderOption}
                valueRenderer={this.renderValue}
                valueKey="id"
                labelKey="name"
                matchPos={this.props.matchPos}
                ignoreCase={this.props.ignoreCase}
                ignoreAccents={false}
                value={this.props.value}
                clearable = {this.props.clearable}
                onChange={this.updateValue}
                clearValueText={formatMessage(messages.clearValue)}
                placeholder={this.props.placeholder ? this.props.placeholder : formatMessage(messages.placeholder)}
                inputProps={{'id': 'combo' + htmlInputId, ...this.props.inputProps}}
                noResultsText={formatMessage(messages.noResults)}
                useFormatMessageData={this.props.useFormatMessageData}
                searchable={this.props.searchable}
              /> : <VirtualizedSelect
                      labelKey='name'
                      valueKey='id'
                      ref='virtualSelect'
                      name={htmlInputId}
                      className={this.props.className}
                      disabled={disabled || readOnly}
                      options={this.props.values}
                      matchPos={this.props.matchPos}
                      ignoreCase={this.props.ignoreCase}
                      ignoreAccents={false}
                      value={this.props.value}
                      optionHeight= { this.getDynamicHeight }
                      noResultsText={formatMessage(messages.noResults)}
                      clearable = {this.props.clearable}
                      clearValueText={formatMessage(messages.clearValue)}
                      inputProps={{'id': 'combo' + htmlInputId, ...this.props.inputProps}}
                      onChange={this.updateValue}
                      useFormatMessageData={this.props.useFormatMessageData}
                      placeholder={this.props.placeholder ? this.props.placeholder : formatMessage(messages.placeholder)}
                 />
              }
          <ValidatePTVComponent {...this.props} valueToValidate={ this.props.value } />
        </div>
        : this.renderReadonlyLabel(this.props.value, cntHtmlId)
        }
    </div>
    );
  }
};

PTVAutoComboBox.propTypes = {
  intl: intlShape.isRequired,
  changeCallback: PropTypes.func,
  disabled: PropTypes.bool,
  autoload: PropTypes.bool,
  values: PropTypes.any.isRequired,
  name: PropTypes.string,
  inputProps: PropTypes.object,
  optionRenderer: PropTypes.func,
  filterOption: PropTypes.func,
  useFormatMessageData: PropTypes.bool,
  labelClass: PropTypes.string,
  autoClass: PropTypes.string,
  componentClass: PropTypes.string,
  clearable: PropTypes.bool,
  iconAction: PropTypes.func
};

PTVAutoComboBox.defaultProps = {
  useFormatMessageData: false,
  clearable: true,
  id: undefined,
  labelClass: '',
  autoClass: '',
  componentClass: '',
  ignoreCase: true,
  autoload: false
}

export default injectIntl(composePTVComponent(PTVAutoComboBox));
