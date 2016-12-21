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
import { connect } from 'react-redux';
import classnames from 'classnames';
import {Map, List } from 'immutable';

const onToggle = ({node, onNodeToggle, showChildren}) => () => {
    // var isCollapsed = !node.get('isCollapsed');
    // var areChildrenLoaded = node.get('areChildrenLoaded');
    // if (!node.get('areChildrenLoaded')) {
    //     node = node.set('isFetching', currentToggle)
    // }

    onNodeToggle(node.set('isCollapsed', showChildren));
}

const Node = (props) => {
        const {node, id, className, isRootLevel, renderNode, styles, children, renderChildren, isFetching, showChildren} = props;
        // const childrenProps = {
        //     nodes: node.get('children'),
        //     className: styles.nodeChildren,
        //     renderNode: renderNode,
        //     isRootLevel: false,
        //     getNode
        // }
        // console.log('node', nodeId, node);
        return (

            <li className={classnames(className, "clearfix")} key={id}>
                { renderNode ? renderNode(node, isRootLevel) : node.get('name')}
                { node.get('isLeaf')
					? null
					: isFetching ? <div className="loading"></div> : 
					<span onClick={ onToggle(props) } className={ classnames('icon', 'icon-toggle', !showChildren ? '': 'open') }></span>
				}
                { 
                    showChildren ? 
                        renderChildren({nodes: children}) 
                        : null 
                }
            </li>
        );
}

function mapStateToProps(state, ownProps) {
    const node =  ownProps.getNode(state, ownProps);
    const children = node.get('children') || List();
    const isCollapsed = ownProps.getNodeIsCollapsed(state, ownProps);
  return {
      node,
      isFetching: ownProps.getNodeIsFetching(state, ownProps),
      showChildren: !isCollapsed && children.size > 0 && node.get('areChildrenLoaded'),
      children: children
  }
}

export const PTVNode = connect(mapStateToProps/*, actions*/)(Node)

// export default {
//     PTVNode: ConnectedNode,
//     PTVTreeChildren: ConnectedNode
// }