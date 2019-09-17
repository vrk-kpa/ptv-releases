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

import React from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import styles from '../styles.scss'
import ReactList from 'react-list'

export const LocalizedNodes = ({
  nodes,
  isRootLevel,
  placeholderComponent,
  ...props
}) => {
  if (nodes.size === 0) {
    return placeholderComponent || null
  }

  const renderNode = item => (
    <props.NodeComponent
      {...props}
      isRootLevel={isRootLevel}
      key={item.id}
      id={item.id}
      nodeLabel={item.name || item.label}
      className={props.styles.node}
    />
  )

  const renderItem = (index, key) => {
    var item = nodes.get(index)
    return renderNode(item)
  }

  return (
    <div className={styles.nodeList} role={isRootLevel ? undefined : 'group'}>
      {isRootLevel
        ? <div className={styles.reactListWrap}>
          <ReactList
            itemRenderer={renderItem}
            length={nodes.size}
            type={props.listType || 'variable'} /></div>
        : nodes.map(renderNode)
      }
    </div>
  )
}

LocalizedNodes.propTypes = {
  nodes: PropTypes.oneOfType([
    PropTypes.array,
    ImmutablePropTypes.list
  ]),
  isRootLevel: PropTypes.bool.isRequired,
  styles: PropTypes.object.isRequired,
  placeholderComponent: PropTypes.element,
  listType: PropTypes.string
}
