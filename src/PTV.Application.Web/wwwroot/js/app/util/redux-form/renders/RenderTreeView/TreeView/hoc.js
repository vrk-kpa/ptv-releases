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

export const composeNode = InnerNode => {
  const TreeNodeDef = ({
  onLoadChildren,
    defaultCollapsed = true,
    isCollapsed,
    toggleCollapsed,
    className,
    renderDetail,
    isRootLevel,
    inline,
    sizeClass,
    ...props
}) => {
    const { node } = props
    const areChildrenLoaded = node.get('areChildrenLoaded')
    const _isCollapsed = !areChildrenLoaded || (isCollapsed === null ? defaultCollapsed : isCollapsed)
    const onToggle = () => {
      if (areChildrenLoaded) {
        toggleCollapsed(_isCollapsed)
      } else if (onLoadChildren) {
        onLoadChildren(node)
        toggleCollapsed(_isCollapsed)
      }
    }
    const nodeContainerClass = cx(
      styles.nodeContainer,
      styles[sizeClass],
      {
        [styles.inline]: inline
      }
    )
    const nodeClass = cx(
      styles.node,
      {
        [styles.rootNode]: isRootLevel
      }
    )
    return (
      <div className={nodeContainerClass} key={props.id}>
        <div className={styles.nodeWrap}>
          <div className={nodeClass}>
            <InnerNode
              isLeaf={node.get('isLeaf')}
              isCollapsed={_isCollapsed}
              onToggle={onToggle}
              isRootLevel={isRootLevel}
              {...props}
            />
            {renderDetail && renderDetail(props)}
          </div>
          {
            _isCollapsed
              ? null
              : <props.NodesComponent isRootLevel={false} {...props} />
          }
        </div>
      </div>
    )
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
    inline: PropTypes.bool,
    sizeClass: PropTypes.string
  }
  TreeNodeDef.defaultProps = {
    sizeClass: 'auto'
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

