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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import cx from 'classnames'
import { withStateHandlers } from 'recompose'
import styles from '../styles.scss'

export const withLazyLoading = options => InnerNode => {
  const FintoNode = props => {
    const onToggle = (node) => {
      if (!node.get('areChildrenLoaded')) {
        options && options.load(props)
      }
    }

    return (
      <InnerNode
        {...props}
        onLoadChildren={onToggle}
      />
    )
  }

  return FintoNode
}

export const asNode = InnerNode => {
  class TreeNodeDef extends Component {
    node = (propertyName) => {
      if (!this.props.node) {
        return null
      }

      return propertyName ? this.props.node.get(propertyName) : this.props.node
    }

    isLeaf = () => !!(this.props.isLeaf === undefined ? this.node('isLeaf') : this.props.isLeaf)

    areChildrenLoaded = () => this.node('areChildrenLoaded') || !this.props.onLoadChildren || false

    isCollapsed = () => {
      return !this.areChildrenLoaded() ||
        (this.props.isCollapsed === null ? this.props.defaultCollapsed : this.props.isCollapsed)
    }

    onToggle = () => {
      if (this.isLeaf()) {
        return
      }

      const { onLoadChildren, toggleCollapsed } = this.props
      if (this.areChildrenLoaded()) {
        toggleCollapsed(this.isCollapsed())
      } else if (onLoadChildren) {
        onLoadChildren(this.node())
        toggleCollapsed(this.isCollapsed())
      }
    }

    render () {
      const {
        onLoadChildren,
        defaultCollapsed,
        isCollapsed,
        toggleCollapsed,
        className,
        renderDetail,
        isRootLevel,
        sizeClass,
        nodeClass,
        wrapClass,
        containerClass,
        ...rest
      } = this.props

      const nodeContainerClass = cx(
        styles.nodeContainer,
        styles[sizeClass],
        containerClass
      )
      const nodeWrapClass = cx(
        styles.nodeWrap,
        {
          [styles.focusedTreeItem]: rest.focusedTreeItem === rest.id
        },
        wrapClass
      )
      const isLeaf = this.isLeaf()
      const nodeCssClass = cx(
        styles.node,
        nodeClass,
        {
          [styles.rootNode]: isRootLevel,
          [styles.hasVisibleChildren]: !isLeaf && !this.isCollapsed()
        },
      )

      return (
        <div className={nodeContainerClass} key={rest.id}>
          <div className={nodeWrapClass}>
            <div
              className={nodeCssClass}
              role='treeitem'
              aria-selected={rest.checked}
              // aria-level ???
              // aria-setsize ???
              // aria-posinset ???
              aria-expanded={isLeaf ? undefined : !isCollapsed}>
              <InnerNode
                isCollapsed={this.isCollapsed()}
                onToggle={this.onToggle}
                isRootLevel={isRootLevel}
                {...rest}
                isLeaf={isLeaf}
              />
              {renderDetail && renderDetail(rest)}
            </div>
            {
              this.isCollapsed()
                ? null
                : <rest.NodesComponent isRootLevel={false} {...rest} />
            }
          </div>
        </div>
      )
    }
  }

  TreeNodeDef.propTypes = {
    node: ImmutablePropTypes.map.isRequired,
    defaultCollapsed: PropTypes.bool,
    isCollapsed: PropTypes.bool,
    isRootLevel: PropTypes.bool,
    className: PropTypes.string,
    onLoadChildren: PropTypes.func,
    renderDetail: PropTypes.func,
    toggleCollapsed: PropTypes.func.isRequired,
    sizeClass: PropTypes.string,
    nodeClass: PropTypes.string,
    wrapClass: PropTypes.string,
    containerClass: PropTypes.string
  }
  TreeNodeDef.defaultProps = {
    sizeClass: 'auto',
    defaultCollapsed: true
  }
  return compose(
    // UI state is held in HOC state insted of redux store for performance reasons //
    /*
      withStateHandlers(
        initialState: Object | (props: Object) => any,
        stateUpdaters: {
          [key: string]: (state:Object, props:Object) => (...payload: any[]) => Object
        }
      )
    */
    withStateHandlers({
      isCollapsed: null
    }, {
      toggleCollapsed: () => (isCollapsed) => ({
        isCollapsed: !isCollapsed
      })
    })
  )(TreeNodeDef)
}

export const withDetails = renderDetail => InnerNode =>
  props => <InnerNode renderDetail={renderDetail} {...props} />

export const withCustomLabel = LabelComponent => Node =>
  props => <Node NodeLabelComponent={LabelComponent} {...props} />
export const withIcon = IconComponent => Node => props => <Node Icon={IconComponent} {...props} />
export const withoutIcon = Node => props => <Node Icon={() => null} {...props} />

export const withFocus = getNodeIds => Node => {
  class FocusComponent extends Component {
    componentWillMount () {
      // console.log('mounted', this.props)
      if (typeof this.props.setFocusCallback === 'function') {
        this.props.setFocusCallback(this.props.id, this.getNext)
      }
    }

    componentWillUnmount () {
      // console.log('unmounted', this.props)
      if (typeof this.props.removeFocusCallback === 'function') {
        this.props.removeFocusCallback(this.props.id, this.getNext)
      }
    }

    getNodeIds = () => {
      return this.props.nodes && this.props.nodes.map(node => typeof node === 'string' ? node : node.get('id'))
    }

    createFocusItem = (id, parentId) => {
      return {
        focus: id,
        parent: parentId || this.props.id
      }
    }

    getNext = (id, type, skipLevel) => {
      // console.log('get for', this.props.id, id, type, skipLevel)
      if ((this.props.id === id || !id && !this.props.id) && this.props.nodes && this.props.nodes.size) {
        // console.log('get sub if possible', this.props)
        switch (type) {
          case 'up':
            if (skipLevel) {
              return this.createFocusItem(this.getNodeIds().last())
            }
            break
          case 'down':
            if (!skipLevel) {
              return this.createFocusItem(this.getNodeIds().first())
            }
        }
      }

      if (this.props.nodes && this.props.nodes.size) {
        if (id && this.getNodeIds().includes(id)) {
          const currentIndex = this.getNodeIds().indexOf(id)
          // console.log('get siblings', id, this.props.nodes && this.props.nodes.toJS(), skipLevel)
          switch (type) {
            case 'down':
              return this.createFocusItem(this.getNodeIds().get(currentIndex + 1))
            case 'up':
              if (skipLevel) {
                return this.createFocusItem(id)
              }
              return currentIndex
                ? this.createFocusItem(null, this.getNodeIds().get(currentIndex - 1))
                : this.props.id && this.createFocusItem(this.props.id) || null
          }
        }
      }
      return null
    }

    render () {
      return <Node {...this.props} />
    }
  }
  return FocusComponent
}

export const withTreeNavigation = Node => {
  class TreeNavigationComponent extends Component {
    componentWillReceiveProps (next) {
      // console.log('try register', typeof next.setTreeNaviagationCallback === 'function',
      //   'compare', next.focusedTreeItem, this.props.focusedTreeItem,
      //   'is mee', next.id, next.focusedTreeItem === next.id)
      if (
        typeof next.setTreeNaviagationCallback === 'function' &&
        next.focusedTreeItem !== this.props.focusedTreeItem &&
        next.focusedTreeItem === next.id
      ) {
        // console.log('call register')
        next.setTreeNaviagationCallback(this.toggle)
      }
    }

    toggle = type => {
      switch (type) {
        case 'right':
          this.props.isCollapsed &&
            typeof this.props.onToggle === 'function' &&
            this.props.onToggle()
          break
        case 'left':
          !this.props.isCollapsed &&
            typeof this.props.onToggle === 'function' &&
            this.props.onToggle()
          break
      }
    }

    render () {
      return <Node {...this.props} />
    }
  }
  return TreeNavigationComponent
}
