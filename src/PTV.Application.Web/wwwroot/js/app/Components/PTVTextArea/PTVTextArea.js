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
import PTVLabel from '../PTVLabel';
import styles from './styles.scss'
import '../styles/PTVCommon.scss'
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent';
//import {is} from 'immutable';
import shortid from 'shortid';
import cx from 'classnames';

export class PTVTextArea extends Component {

    id = '';

    constructor(props) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.handleLeave = this.handleLeave.bind(this);
        this.state = {...this.state,
            charCount: this.props.value ? this.props.value.length : 0,
            componentValue: this.props.value || '',
            isChangingDirty : false
        };
        this.id = shortid.generate();
    }

    charCounter = 0;

    handleChange (event) {
        if (typeof(this.props.changeCallback) === 'function')
        {
            console.log("PTVTextArea - HandleChange - potencial performance issue")
            this.props.changeCallback(event.target.value);
        }
        this.setState({charCount: event.target.value.length, componentValue:event.target.value});
        this.state.isChangingDirty = true;
    }
    handleLeave(){
        if (typeof(this.props.blurCallback) === 'function')
        {
            this.props.blurCallback(this.state.componentValue);
        }
        this.state.isChangingDirty = false;
    }

    renderCounterLabel = (displayData, counterClass) => {
            return <PTVLabel
                labelClass={counterClass || "counter"}>
                {displayData}
            </PTVLabel>
    }

    render() {
        const {disabled, readOnly} = this.props;
        const value = this.state.isChangingDirty ? this.state.componentValue : this.props.value || '';

        if (readOnly && value === '') {
            return null
        }
        return (
            <div className={this.props.componentClass}>
                <div className={this.props.labelClass}>
                    { this.props.label ?
                        <PTVLabel
                            htmlFor={this.props.name + '_' + (this.props.id || this.id)}
                            readOnly={readOnly}
                            tooltip={this.props.tooltip}>
                                {getRequiredLabel(this.props)}
                        </PTVLabel>
                    : null }
                </div>

                {!readOnly ?
                    <div className={cx("ptv-textarea", this.props.className, this.props.size)}>
                        <textarea
                            disabled={disabled || readOnly}
                            onChange={this.handleChange}
                            onBlur={this.handleLeave}
                            className="form-control"
                            maxLength={this.props.maxLength}
                            rows={this.props.minRows}
                            id={this.props.name + '_' + (this.props.id || this.id) }
                            placeholder={this.props.placeholder}
                            value= { value }></textarea>
                        <small className="text-muted">{this.props.textMuted}</small>

                        {this.renderCounterLabel(this.props.disabled ?
                            this.state.charCount
                            : (this.props.maxLength ?
                                this.state.charCount + "/" + this.props.maxLength
                                : null), this.props.counterClass)}
                    </div>
                :
                    <PTVLabel>{ value }</PTVLabel>
                }
                <ValidatePTVComponent {...this.props} valueToValidate={ value } />
            </div>
        );
    }
};

PTVTextArea.propTypes = {
  layoutClass: PropTypes.string,
  value: PropTypes.any,
  validators: React.PropTypes.array,
  disabled: PropTypes.bool,
  name: PropTypes.string.isRequired,
  size: PropTypes.string
};

PTVTextArea.defaultProps = {
  id: undefined,
  size: 'full',
  value: ''
}

export default composePTVComponent(PTVTextArea);
