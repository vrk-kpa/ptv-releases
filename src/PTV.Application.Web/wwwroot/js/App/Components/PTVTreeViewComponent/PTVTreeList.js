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
import PTVTag from '../PTVTag';
import './Styles/PTVTreeList.scss';
import {connect} from 'react-redux';

const PTVTreeListItem = ({onNodeRemove, id, name, isDisabled, readOnly}) => {

// treeText: ownProps.readOnly && treeList ? treeList.map((x) =>{ return x.get('name') }).join(", ") : ''
    return (
        <PTVTag
            id= { id }
            name= { name }
            onTagRemove= { onNodeRemove }
            isDisabled= { isDisabled }
            readOnly= { readOnly }
        /> 
    )
}

function mapStateToPropsItem(state, ownProps) {
    const item = ownProps.getItemSelector(state, ownProps);
    
  return {
    // serviceClasses: CommonServiceSelectors.getServiceClasses(state).map(sc => sc.get('id')),
    name: item ? item.get('name') : ownProps.id,
    isDisabled: ownProps.isDisabledSelector ? ownProps.isDisabledSelector(state, ownProps) : false
  }
}

const PTVTreeListItemConnected = connect(mapStateToPropsItem)(PTVTreeListItem);

const renderlistItems = (props) => {
    return (
        props.treeList.map((id) => <PTVTreeListItemConnected id={id} onNodeRemove={props.onNodeRemove} getItemSelector={ props.getItemSelector} readOnly={props.readOnly} isDisabledSelector={props.isDisabledSelector}/>)
    )
}

const PTVTreeList = (props) => {
    
    return (
        <div className={ !props.readOnly ? props.className : "readonly" }>
            { props.header ? <PTVLabel readOnly={props.readOnly} >{ props.header }</PTVLabel> : null }
            
            <div className='content-added'>
                <ul>
                    { renderlistItems(props)}
                </ul>
            </div>

        </div>
    );
    
}

PTVTreeList.propTypes = {
    onRemoveNode: PropTypes.func,
    treeList: PropTypes.object.isRequired
}

function mapStateToProps(state, ownProps) {
    const treeList = ownProps.listSelector(state, ownProps);
  return {
    treeList
  }
}

export default connect(mapStateToProps)(PTVTreeList);
