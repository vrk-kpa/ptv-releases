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
import cx from 'classnames';
import React, { PropTypes, Component } from'react';
import ReactDOM  from 'react-dom';
import {connect} from 'react-redux';
import PTVTreeView from './PTVTreeView';
import PTVTreeList from './PTVTreeList';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import PTVConfirmDialog from '../PTVConfirmDialog';
import Immutable, { List, Map, Record } from 'immutable';
import * as ValidationHelper from '../PTVValidations';
import '../Styles/PTVCommon.scss';
import './Styles/PTVTreeView.scss';
import PTVLabel from '../PTVLabel';
import PTVLableInput from '../PTVTextInput';
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent';
import PTVCheckBox from '../PTVCheckBox';

const messages = defineMessages({
    deleteModalBody: {
        id: "Components.Modal.DeleteBody",
        defaultMessage: "Are you sure you want to remove class {parent} and the subclasses {childrens}?"
    },
    deleteModalAccpet: {
        id: "Components.Button.Ok",
        defaultMessage: "Kyllä"
    }
});

class PTVTreeViewSelect extends Component {
    constructor(props) {
      super(props);
      this.state = {
            ...this.state,
			initialized: false,
            deleteConfirm: false,
            parentToDelete:"",
            childrensToDelete:""};
    }
	
  	// getParents = (nodeId,tree, storage = { parents: List() } ) => {
	// 	var storage = storage;
  	// 	tree.forEach((item) => {
	// 		  if (item.get('id') == nodeId) {
	// 			  storage.alreadyFound = true;
	// 			  return false;
	// 		  } else {
	// 			  if (storage.alreadyFound) {
	// 				  return false;
	// 			  }
	// 			  else {
	// 				  storage.alreadyFound = this.getParents(nodeId, item.get('children'), storage).alreadyFound;
	// 				  if (storage.alreadyFound) {
	// 					  storage.parents = storage.parents.push(item);
	// 				  }
	// 			  }
	// 		  }
	// 	  });

	// 	return storage;
  	// }

	onNodeSelect = (event) => {
    	this.props.onNodeSelect(event.target.id, event.target.checked);
    }

	onNodeClick = (node) => () => {
		if (this.props.onNodeClick){
			this.props.onNodeClick(node);
		}
	}

	onToggle = (node) => {
		this.props.onNodeToggle(node);
	}

	renderNode = (node, isRootLevel) => {
		//const checkboxClass = isRootLevel ? "" : "small";
		const showLabelOutsideCheckbox = this.props.onNodeClick != null;
	    return (
			<div className="ptv-tree-node" >
				<PTVCheckBox
					noCheckableLabel={true}
					small={!isRootLevel} id={node.get('id')} 
					isSelectedSelector={this.props.isSelectedSelector} 
					language={this.props.language} 
					isDisabledSelector={this.props.isDisabledSelector} 
					onClick={this.onNodeSelect}
					className={showLabelOutsideCheckbox ? "outside-label" : null}
				>
					{showLabelOutsideCheckbox ? null : node.get('name')}
				</PTVCheckBox>
				{ showLabelOutsideCheckbox ?
					<div onClick={this.onNodeClick(node)}>
						<PTVLabel>
							{ node.get('name') }
						</PTVLabel>
					</div>
					: null
				}
			</div>
	    );
  	}

  	render() {		
        const {formatMessage} = this.props.intl;
		//console.log('treeselect')
  	    return (
		!this.props.readOnly ? 
		<div className={cx(this.state.errorClass, this.props.className)}>
			<PTVConfirmDialog
				show={this.state.deleteConfirm}
				acceptCallback={this.acceptRemoveCallback}
				closeCallback={this.closeCallback}
				text={formatMessage(messages.deleteModalBody, {parent:this.state.parentToDelete, childrens:this.state.childrensToDelete})}
				acceptButton ={formatMessage(messages.deleteModalAccpet)}
				/>
			<div className="row">
				<div className="col-xs-12">
					<PTVLabel labelClass = { this.props.labelClassName } tooltip={ this.props.labelTooltip }>{ getRequiredLabel(this.props) }</PTVLabel>
				</div>
			
				<PTVTreeView
					className = { this.props.treeViewClass }
					//labelClassName = { this.props.labelClassName }
					//labelTooltip = { this.props.labelTooltip }
					//labelContent = { getRequiredLabel(this.props) }
					tree={ this.props.treeSource }
					renderNode={ this.renderNode }
					onNodeToggle={ this.onToggle }
					searchPlaceholder={ this.props.treeSourceSearchPlaceHolder }
					onNodeSelect={ this.onNodeSelect }
					handleToggle={ this.onToggle }
					searchable={ true }
					renderCustomSearch={this.props.renderCustomSearch}
					isSearching = { this.props.isSearching }
					searchInTree = { this.props.onSearchInTree }
					clearSearch = { this.props.onClearSearch }
					getNode = { this.props.getNode }
					getNodeIsCollapsed = { this.props.getNodeIsCollapsed }
					getNodeIsFetching = { this.props.getNodeIsFetching }
				/>
  	    	<PTVTreeList 
			  	className= {this.props.resultsClass } 
				header={ this.props.treeTargetHeader } 
				onNodeRemove={ this.props.onNodeRemove } 
				listSelector = { this.props.getSelectedSelector } 
				readOnly={ this.props.readOnly }
				getItemSelector={ this.props.getListNode ? this.props.getListNode : this.props.getNode }
				isDisabledSelector={this.props.isDisabledSelector} 
				language = { this.props.language }
			/>
				<div className="col-xs-12">
			  	<ValidatePTVComponent {...this.props} valueToValidate={ this.props.isAnySelected ? 'valid' : '' } />
				</div>
			</div>
		</div>
			:	<PTVTreeList 
			  		className= {this.props.readOnlyClass } 
					header={ this.props.label } 
					onNodeRemove={ this.props.onNodeRemove } 
					listSelector = { this.props.getSelectedSelector } 
					readOnly={ this.props.readOnly }
					getItemSelector={ this.props.getListNode ? this.props.getListNode : this.props.getNode }
					language = { this.props.language }
				/>
		);
  }
}

PTVTreeViewSelect.propTypes = {
    treeViewClass: PropTypes.string,
    resultsClass: PropTypes.string,
	labelClassName: PropTypes.string,
	labelTooltip: PropTypes.string,
	labelContent: PropTypes.string,
}

PTVTreeViewSelect.defaultProps = {
    treeViewClass: 'col-md-6',
    resultsClass: 'col-md-6',
}

function mapStateToProps(state, ownProps) {   
  return {
    isAnySelected: ownProps.getSelectedSelector(state, ownProps).size > 0
  }
}


export default connect(mapStateToProps)(injectIntl(composePTVComponent(PTVTreeViewSelect)));
