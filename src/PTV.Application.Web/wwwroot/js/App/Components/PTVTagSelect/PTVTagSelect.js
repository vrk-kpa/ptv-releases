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
import Select from 'react-select';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import Immutable from 'immutable';
import ImmutablePropTypes from 'react-immutable-proptypes';
import PTVLabel from '../PTVLabel'
import '../Styles/PTVCommon.scss';
import { composePTVComponent, ValidatePTVComponent, renderOptions, getRequiredLabel } from '../PTVComponent';
import cx from 'classnames';
import styles from './styles.scss';

const messages = defineMessages({
    placeholder: {
        id: "Components.AutoCombobox.Placeholder",
        defaultMessage: "- valitse -"
    },
    noResults: {
        id: "Components.AutoCombobox.NoResults",
        defaultMessage: "Ei hakutuloksia"
    },
    removeSelection: {
        id: "Components.AutoCombobox.RemoveSelection",
        defaultMessage: "Paina askelpalautinta poistaaksesi valinnan \"{label}\""
    }
});

const PTVTagSelect = props => {
    const updateValue = (selectedObjects) => {
      props.changeCallback(Immutable.fromJS(selectedObjects.map(obj => obj.id)));
    }

    const filterOption = (option, filter) => {
        let optionValue = renderOption(option);
        if (!optionValue){
            return false;
        }
        optionValue = optionValue.toString();
        if (props.ignoreCase === true) {
            optionValue = optionValue.toLowerCase();
        }
        return props.matchPos === 'start' ? optionValue.substr(0, filter.length) === filter : optionValue.indexOf(filter) !== -1;
    }

    const renderOption = (option) => {
        const data = { ...props, option}
        return renderOptions(data);
    }
    
    const renderOptionItems = (props) => {      
        return props.value ? props.value.map((item) =>  renderOption(item)).join(", ") : ''
    }

    const {disabled, readOnly} = props;
    const {formatMessage} = props.intl;
    
    const selectedItems = props.value;
    if (props.readOnly && Array.isArray(selectedItems) && selectedItems.length === 0) {
        return null
    }

    let dynamicProps = {}
    if (props.useFormatMessageData){
        dynamicProps.filterOption = filterOption;
    } 

    return(
        <div id={"combo" + props.id + "Container"} className={cx("tag-select ", props.componentClass, props.className)} >
            <PTVLabel htmlFor={ "combo" + props.id } labelClass={ props.labelClass } readOnly={ props.readOnly } tooltip={ props.tooltip}>{getRequiredLabel(props) }
            </PTVLabel>
            {!props.readOnly ?
            <div className = { props.autoClass }>
            <Select
                {...dynamicProps} 
                multi={ true }
                name={ "combo" + props.id + "value" }
                options={ props.options }
                optionRenderer={ renderOption }
                valueRenderer={ renderOption }
                valueKey="id"
                labelKey="name"
                    matchPos={props.matchPos}
                    ignoreCase={props.ignoreCase}
                    ignoreAccents={false}
                value={ props.value }
                onChange={ updateValue }
                placeholder={ props.placeholder || formatMessage(messages.placeholder) }
                inputProps={ { 'maxLength':'20', 'id': 'combo' + props.id } }
                noResultsText={ formatMessage(messages.noResults) }
                backspaceToRemoveMessage={ formatMessage(messages.removeSelection, { label: "{label}" }) }
            />
            <ValidatePTVComponent { ...props } valueToValidate={ props.value } />
            </div>
            :
            <div>
                <PTVLabel className = { props.autoClass }>
                    {renderOptionItems(props)}
                </PTVLabel>
                                                    
            </div>
            }
            </div>
    )
};

PTVTagSelect.propTypes = {
  intl: intlShape.isRequired,
//   options: ImmutablePropTypes.list.isRequired,
//   value: ImmutablePropTypes.list.isRequired,
  options: PropTypes.array.isRequired,
  value: PropTypes.array,
  optionRenderer: PropTypes.func,
  useFormatMessageData: PropTypes.bool
};

PTVTagSelect.defaultProps = {
  useFormatMessageData: false,
  ignoreCase: true
}

export default injectIntl(composePTVComponent(PTVTagSelect));
