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

import React, { PropTypes } from 'react'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { compose } from 'redux'
import cx from 'classnames'
import { Map, List } from 'immutable'
import Infinite from 'react-infinite'
import ReactList from 'react-list';
import ui from 'Utils/redux-ui'
import { PTVTooltip, PTVCheckBox } from 'Components'
import ChildrenIcon from './ChildrenIcon'

export const Nodes = ({ nodes, className, isRootLevel, nodeHeights, ...props }) => {
  if (nodes.size === 0) {
    return null
  }

  const renderItem = (index, key) => {
    var node = nodes.get(index)
    var nodeId = node.get('id')
    return <props.NodeComponent
              {...props}
              key={node}
              id={node}
              className={props.styles.node}
              isRootLevel={isRootLevel}
            />
  }

  return (
    // TODO: check solution for inner lazy render lists
    <ul className={cx(className)}>
      {isRootLevel
        ? <div style={{overflow: 'auto', maxHeight:330}}>
          <ReactList
            itemRenderer={renderItem}
            length={nodes.size}
            type='variable' /></div>
        : nodes.map((nodeId) =>
          <props.NodeComponent
            {...props}
            key={nodeId}
            id={nodeId}
            className={props.styles.node}
          />
        )}
    </ul>
  )
}

export const NodeLabel = (props) => {
  const { node, name } = props
  // console.log(node.toJS())
  return <span>{node.get('name') || name}</span>
}

export const NodeCheckBox = ({
  id,
  node,
  isRootLevel,
  onChange,
  showTooltip,
  ...props
}) => {
  const onNodeSelectFunc = (event) => {
    onChange(id, event.target.checked)
  }

  return (
    <div className={cx('ptv-tree-node', { 'small': !isRootLevel })}>
      <PTVCheckBox
        {...props}
        noCheckableLabel
        small={!isRootLevel}
        id={id}
        nodeId={id}
        onClick={onNodeSelectFunc}
        {...props}
      >
        <PTVTooltip
          readOnly={!showTooltip}
          labelContent={node.get('name')}
          tooltip={node.get('name')}
          attachToBody
        />
      </PTVCheckBox>
    </div>
  )
}

export const NodeContainer = ({ id, className, children }) => (
  <li className={cx(className, 'clearfix')} key={id}>
    {children}
  </li>
)

const NodeDef = ({
  NodeLabelComponent = NodeLabel,
  isRootLevel,
  onLoadChildren,
  ui,
  uiKey,
  uiPath,
  resetUI,
  updateUI,
  className,
  ...props
}) => {
  const { node } = props
  const areChildrenLoaded = node.get('areChildrenLoaded')
  const isCollapsed = !areChildrenLoaded || ui.isCollapsed //get('isCollapsed')
  const onToggle = () => {
    if (areChildrenLoaded) {
      updateUI('isCollapsed', !isCollapsed)
    } else if (onLoadChildren) {
      onLoadChildren(node)
    }
  }
  return (
    <NodeContainer className={className} id={props.id}>
      <NodeLabelComponent isRootLevel={isRootLevel} {...props} />
      <ChildrenIcon
        isRootLevel={isRootLevel}
        isLeaf={node.get('isLeaf')}
        {...props}
        onClick={onToggle}
        isCollapsed={isCollapsed} />
      {
        isCollapsed
        ? null
        : <props.NodesComponent {...props} />
      }
    </NodeContainer>
  )
}

NodeDef.propTypes = {
  node: ImmutablePropTypes.map.isRequired,
  isCollapsed: PropTypes.bool,
  isRootLevel: PropTypes.bool,
  className: PropTypes.string,
  ui: PropTypes.object.isRequired,
  uiKey: PropTypes.any,
  uiPath: PropTypes.any,
  resetUI: PropTypes.func,
  updateUI: PropTypes.func.isRequired,
  onLoadChildren: PropTypes.func
}

export const Node = compose(
  ui({
    persist: true,
    state: {
      isCollapsed: false
    }
  })
)(NodeDef)
