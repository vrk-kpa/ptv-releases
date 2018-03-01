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
import ImmutablePropTypes from 'react-immutable-proptypes';
import { List } from 'immutable';
import classnames from 'classnames';
import Infinite from 'react-infinite';
import './styles/PTVTreeView.scss';

import { PTVNode, PTVTreeChildren } from './PTVNode';
import { DragDropContext, Backend as DragDropBackend } from "react-dnd";
import * as HTML5DragDropBackend from 'react-dnd-html5-backend';

import PTVLabel from '../PTVLabel';
import PTVIcon from '../PTVIcon';

class SearchInputComponent extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate() {
        return false;
    }

    render() {
        return (<input type="text" key='treeViewSearchInput' className="ptv-textinput full with-icon" placeholder={ this.props.placeholder } onChange={ this.props.onChange } disabled={this.props.disabled}/>)
    }
}

const SearchInput = SearchInputComponent;

class PTVTreeView extends Component {
	constructor(props) {
        super(props);
        this.state = { timer: 0 };
    };

    renderChildren = ({ nodes, className, isRootLevel}) => {
        const cn = className || this.props.styles.nodeChildren;
        const props = {
                        language: this.props.language,
                        isRootLevel,
                        className: this.props.styles.node,
                        renderNode: this.props.renderNode,
                        getNode: this.props.getNode,
                        nodeHeights: this.props.nodeHeights,
                        getNodeIsCollapsed: this.props.getNodeIsCollapsed,
                        getNodeIsFetching: this.props.getNodeIsFetching,
                        onNodeToggle: this.props.onNodeToggle,
                        renderChildren: this.renderChildren,
                        contextId: this.props.contextId,
                        getDefaultTranslationText: this.props.getDefaultTranslationText
                    };
                    //console.log(props);
        return (
            // TODO: check solution for inner lazy render lists
            <ul className={classnames(className)}>
                { isRootLevel ?
                <Infinite containerHeight={330} elementHeight={props.nodeHeights || 50}>
                {nodes.map((nodeId) =>
                //    <label><PTVTreeChildren/></label>
                    <PTVNode key={nodeId} id={nodeId} {...props} />
                )}
                </Infinite>
                :
                nodes.map((nodeId) =>
                //    <label><PTVTreeChildren/></label>
                    <PTVNode key={nodeId} id={nodeId} {...props} />
                )}
           </ul>
        );
    }



    searchInTree = (event) => {

        clearTimeout(this.state.timer);
        var eventTargetValue = event.target.value.toLowerCase();
        if (eventTargetValue.length > 2) {
            this.setState({ timer: setTimeout( () => {
                this.props.searchInTree(eventTargetValue);}, 600 )});
        } else if (eventTargetValue.length == 0) {
            this.props.clearSearch();
        }
    }


	renderSearchField = () => (
		<div key='treeViewSearchDiv' className='list-search-field'>
				{this.props.isSearching ? <div className="loading"></div> : <PTVIcon componentClass="magnifying-glass" name="icon-search" /> }
				<SearchInput placeholder={ this.props.searchPlaceholder } onChange={ this.searchInTree } disabled={this.props.isSearching}/>
		</div>
	)

    render() {
        // console.log('<treeview></treeview>')
        return (
            <div className= { this.props.className }>
                {this.props.searchable?
					this.props.renderCustomSearch ? this.props.renderCustomSearch() : this.renderSearchField()
					:null}
                <div className={classnames('list-search-results', { 'hidden': this.props.tree.size === 0 })}>
                    {
                        // this.props.children ?
                        // this.props.children :
                        // this.renderTree(this.props.tree, this.props.styles.treeView, true)
                        this.renderChildren({
                            nodes: this.props.tree,
                            className: this.props.styles.treeView,
                            isRootLevel: true
                        })
                    }
                </div>
            </div>
        );
    }
}

PTVTreeView.propTypes = {
    onNodeDoubleClick: PropTypes.func,
    tree: ImmutablePropTypes.list.isRequired,
    onNodeToggle: PropTypes.func,
    searchInTree: PropTypes.func
}

PTVTreeView.defaultProps = {
    styles: {
        treeView: 'expandable',
        node: 'node-parent',
        nodeList: 'node-list',
        nodeChildren: 'child-list'
    }
}

export default DragDropContext(
  HTML5DragDropBackend
  // TouchDragDropBackend({ enableMouseEvents: true })
)(PTVTreeView);
