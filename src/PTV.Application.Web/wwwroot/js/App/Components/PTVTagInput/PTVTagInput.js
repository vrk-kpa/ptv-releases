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
import React,  {Component} from 'react';
import TagInput from 'react-tokeninput';
import Option from 'react-tokeninput/lib/option';
import Immutable, { Map, Seq } from 'immutable';
import styles from './styles.scss';
import PTVLabel from '../PTVLabel';
import shortId from 'shortid';

export class PTVTagInput extends Component {

    constructor(props) {
        super(props);
        this.state = { menuOptions: this.props.options, alreadySelected: false }
    }

    handleRemove = value => {
        this.props.onRemove(value);
    }

    handleSelect = value => {
        if (typeof value === 'string') {
            const newItem = Map({id: shortId.generate(), name: value, isNew: true });
            this.state.alreadySelected = true;
            this.setState({menuOptions: this.props.options.set(newItem.get('id'), newItem)});
            this.props.onSelect(newItem.toJS());
        }
        else if (typeof value ==='object' && !this.props.value.includes(value.id)) {
            this.props.onSelect(value);
            this.state.alreadySelected = true;
        }
    }

    renderComboboxOptions = () => {

        return this.state.menuOptions.filter(item => !this.props.value.includes(item.get('id'))).map(value => {
        return (
                <Option
                key={value.get('id')}
                value={value.toJS()}
                >{value.get('name')}</Option>
                );
            }).toArray();
    }

    handleInputChanges = input => {
        if (!this.state.alreadySelected) {
            this.setState({menuOptions: this.props.options.filter((option) => option.get('name').indexOf(input) != -1)});
        }
        else {
            this.setState({alreadySelected: false});
        }
    }

    render() {
    if (this.props.readOnly && Array.isArray(this.props.value) && this.props.value.length === 0) {
        return null 
    }

    return(
        <div className="tag-input row" >
            <div className= { this.props.labelClass }>
                <PTVLabel tooltip= { this.props.tooltip } readOnly= { this.props.readOnly }>{ this.props.label }</PTVLabel>
            </div>
            <div className= { this.props.tagInputClass + " dropdown-list" }>
                { !this.props.readOnly ?
                    <TagInput
                    menuContent= { this.renderComboboxOptions() }
                    placeholder= { this.props.placeholder }
                    selected= { this.props.value.map(id => this.props.options.get(id).toJS()).toArray() }
                    onSelect= { this.handleSelect }
                    onInput= { this.handleInputChanges }
                    onRemove= { this.handleRemove }
                    showListOnFocus= { true }
                    />
                    :<PTVLabel>
                    { this.props.value ? this.props.value.map((x) =>{ return this.props.options.get(x).get('name') }).join(", ") : null }
                </PTVLabel>}
            </div>               
        </div>
)}
};

export default PTVTagInput;
