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
import React, { Component, PropTypes } from 'react';
import PTVLabel from '../../Components/PTVLabel';
import PTVLabelCustomComponent from '../../Components/PTVLabelCustomComponent';
import PTVDateTimePicker from '../../Components/PTVDateTimePicker';
import PTVAutoComboBox from '../../Components/PTVAutoComboBox';
import PTVTimeSelect from '../../Components/PTVTimeSelect';
import moment from 'moment';
import cx from 'classnames';
import { enumFactory } from './Enums/EnumFactory';

const dateTimeInputsTypes = enumFactory(['start', 'end']);

const DateTimeValidityInputs = (props) => {

    // onStartDateChange = (value) => {
    //     props.onChange("start", value);
    // }

    // onEndDateChange = (value) => {
    //     props.onChange("end", value);
    // }

    const onChangeValid = (type) => (value) => {
        props.onChange(type, value);
    }

    const getValidationFunction = (type) => {
        const filter = props.filterDateTime[type] ? props.filterDateTime[type] : null;
        if (filter) {
            if (type === 'start' && !isNaN(Number(props.end)) && Math.abs(Number(props.end)) > 0) {
                return validBefore;
            } else if (type === 'end' && !isNaN(Number(props.start)) && Math.abs(Number(props.start)) > 0) {
                return validAfter;
            }
        }
        return () => { return true };
    }

    const validAfter = (current) => {
        const treshold = moment(props.start);
        return current.isAfter(treshold);
    }

    const validBefore = (current) => {
        const treshold = moment(props.end);
        return current.isBefore(treshold);
    }

    const renderDefault = (type) => {
        const value = type === 'start' ? props.start : props.end;
        const validDate = getValidationFunction(type);
        // const onChangeValid = type === 'start' ? onStartDateChange : onEndDateChange;
        return (
            <div>
                <PTVDateTimePicker
                    value={value}
                    mode={props.mode}
                    onChangeValid={onChangeValid(type)}
                    relatedValue = { type === 'start' ? props.end : props.start }
                    isValidDate={validDate}
                    readOnly={props.readOnly}
                    inputProps={props.inputProps}
                    validatedField = { props.validatedFieldLabels[type] }
                    validators = { props.validators }
                    />
            </div>
        );
    }

    const getFilteredTimes = (treshold, values) => {
        return values.filter(val => val.id > treshold);
    }

    const getValueFunc = (type) => () => type === 'start' ? props.start : props.end;

    const getSelectorFor = (type, isValue) => {
        if (props.getSelectorFor){
            return props.getSelectorFor(type, isValue);
        }
        return isValue ?
                getValueFunc(type)
                : null;
    }

    const getTresholdSelector = (type) => {
        if (props.getTresholdSelector){
            return props.getTresholdSelector(type);
        }
        return null;
    }

    const renderDropDown = (type) => {
        // const value = type === 'start' ? props.start : props.end;
        //const values = props.filterDateTime[type] ? getFilteredTimes(props.filterDateTime[type], props.values) : props.values;
        return (
            <PTVTimeSelect
                id ={ props.id }
                openingHoursId = { props.openingHoursId }
                getValueSelector = { getSelectorFor(type, true)}
                getIsDefaultValueSelector = { getSelectorFor(type, false) }
                //values = { values }
                onChange = { onChangeValid(type) }
                step = { 15 }
                validators = { props.validators }
                //readOnly = { readOnly }
                disabled = { props.disabled }
                getTresholdSelector = { getTresholdSelector(type) }
                className = { props.className }
                placeholder = " "
            />
        );
    }

    const renderDateTime = (type) => {
        const showLabels = props.showLabels && props.showLabels.start && props.showLabels.end;
        const label = showLabels ? type === 'start' ? props.showLabels.start : props.showLabels.end : null;
        const itemClass = cx({
                "separator": props.showSeparator && type ==="end",
                "float-children": props.componentClass ==="horizontal"
            }, "datetime-item");

        return (
            <div className={itemClass}>
                { label ?
                    <PTVLabel readOnly = {props.readOnly}>{label}</PTVLabel>
                : null }
                { props.showAsDropdown ? renderDropDown(type) : renderDefault(type) }
            </div>
        );
    }

    return (
            <div className={cx("datetime-container", props.componentClass)}>
                <div className={cx({"float-children": props.componentClass ==="horizontal"})}>
                    { renderDateTime('start') }

                    { !props.singleInput ?
                        renderDateTime('end')
                    : null }
                </div>
            </div>
    );
}

DateTimeValidityInputs.propTypes = {
    label: PropTypes.string,
    componentClass: PropTypes.string,
    mode: PropTypes.string,
    readOnly: PropTypes.bool,
    start: PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.number
    ]),
    end: PropTypes.oneOfType([
      PropTypes.string,
      PropTypes.number
    ]),
    onChange: PropTypes.func.isRequired,
    showLabels: PropTypes.object,
    showSeparator: PropTypes.bool,
    inputProps: PropTypes.object,
    showAsDropdown: PropTypes.bool,
    singleInput: PropTypes.bool,
    filterDateTime: PropTypes.object
}

DateTimeValidityInputs.defaultProps = {
    showSeparator: true,
    singleInput: false,
    componentClass: '',
    mode: 'date'
}

export default DateTimeValidityInputs;
