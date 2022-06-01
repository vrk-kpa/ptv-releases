/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { compose } from 'redux'
import cx from 'classnames'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
// import 'Components/PTVTreeViewComponent/styles/PTVTreeView.scss'
import styles from '../styles.scss'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { withErrorConsumer } from 'util/redux-form/HOC/withErrorContext'
import { SharedConsumer } from 'appComponents/SharedContext'

const TreeViewDef = ({ nodes, simple, filterTree, className, isNotFound, ...props }) => {
  const treeClass = cx(
    styles.treeSelect,
    className,
    {
      [styles.hidden]: nodes && nodes.size === 0,
      [styles.simple]: simple,
      [styles.filterTree]: filterTree,
      [styles.display]: props.isReadOnly,
      [styles.error]: props.withError,
      [styles.warning]: props.withWarning,
      [styles.isNotFound]: isNotFound
    }
  )
  return (
    <div className={treeClass}>
      <SharedConsumer>
        {(value) => (value && typeof value.isAccordionOpen === 'boolean') ? (value.isAccordionOpen &&
        <props.NodesComponent
          nodes={nodes}
          className={props.styles.treeView}
          isRootLevel
          simple={simple}
          {...props}
        /> || null) : <props.NodesComponent
          nodes={nodes}
          className={props.styles.treeView}
          isRootLevel
          simple={simple}
          {...props}
        />}</SharedConsumer>
    </div>
  )
}

TreeViewDef.propTypes = {
  styles: PropTypes.object.isRequired,
  onNodeToggle: PropTypes.func,
  simple: PropTypes.bool,
  filterTree: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  nodes: ImmutablePropTypes.list,
  className: PropTypes.string,
  isNotFound: PropTypes.bool,
  withError: PropTypes.bool,
  withWarning: PropTypes.bool
}

TreeViewDef.defaultProps = {
  styles: {
    // treeView: 'expandable',
    node: styles.nodeWrap
    // nodeList: 'node-list',
    // nodeChildren: 'child-list'
  }
}

export const TreeView = compose(
  withErrorConsumer()
)(TreeViewDef)

export const SearchTree = compose(
  SearchFilter.withSearchFilter({ listProperty: 'nodes' })
)(TreeView)
