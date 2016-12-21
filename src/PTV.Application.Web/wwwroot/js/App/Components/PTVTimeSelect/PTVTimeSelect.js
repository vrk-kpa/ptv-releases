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
import PTVAutoComboBox from '../../Components/PTVAutoComboBox';
import moment from 'moment';
import styles from './styles.scss'
import cx from 'classnames';
import {connect} from 'react-redux';

const getTimeOptions = (step) => {
    console.log('create options - ', step);
    const availableTimes = [];
    for (let i = 0; i < 24*60; i += step) {
        // adding one day due to react select which treats zero value as falsy and shows placeholder instead
        let value = moment.utc(i*60*1000).add(1, 'day');// - moment().startOf('day');
        availableTimes.push({ id: value.valueOf(), name: value.format('HH:mm'), clearable: true });
        
    }
    return availableTimes;
}

const timeOptions = getTimeOptions(15);

const getFilteredTimeOptions = (timeOptions, treshold) => {
        // const filteredOptions = 
        //     customfilterOptions ? 
        //     customfilterOptions(timeOptions, treshold) :
        return treshold ? timeOptions.filter(x => x.id > treshold) : timeOptions;
}

const PTVTimeSelect = ({ value, values, isDefaultValue, placeholder, minTicks, className, componentClass, onChange, showIcon, treshold, disabled, customfilterOptions, validators } = props) => {   

    // const getFilteredTimeOptions = (input, callBack) => {
    //     const filteredOptions = 
    //         customfilterOptions ? 
    //         customfilterOptions(timeOptions, treshold) : 
    //         ( treshold ? timeOptions.filter(x => x.id > treshold) : timeOptions);
    //     console.log('filter', input, timeOptions.length, treshold, filteredOptions)
    //     callBack(null, { options: filteredOptions })
    //     }

    return (
        <div className={cx(componentClass, "ptv-timeselect")}>
            <PTVAutoComboBox
                // async = { true }
                // autoload = { true }
                value = { value }
                // values = { getFilteredTimeOptions }
                values = { values }
                
                changeCallback = { onChange }
                name = { "timeSelect" }
                disabled = { disabled }
                className = { className }
                placeholder = { placeholder }
                clearable = { !isDefaultValue }
                validators = { validators }
            />
        </div>
    );
}

PTVTimeSelect.propTypes = {
    value: PropTypes.number,
    placeholder: PropTypes.string,
    minTicks: PropTypes.array,
    className: PropTypes.string,
    componentClass: PropTypes.string,
    onChange: PropTypes.func.isRequired,
    showIcon: PropTypes.bool,
    treshold: PropTypes.number,
    disabled: PropTypes.bool
}

PTVTimeSelect.defaultProps = {
    showIcon: false,
    componentClass: '',
}

const mapStateToProps = (state, ownProps) => {
    const treshold = ownProps.getTresholdSelector ? ownProps.getTresholdSelector(state, ownProps) : ownProps.treshold;
    return {
        treshold,
        value: ownProps.getValueSelector ? ownProps.getValueSelector(state, ownProps) : ownProps.value,
        values: getFilteredTimeOptions(timeOptions, treshold),
        isDefaultValue: ownProps.getIsDefaultValueSelector ? ownProps.getIsDefaultValueSelector(state, ownProps) : false
    }
}

export default connect(mapStateToProps)(PTVTimeSelect);
