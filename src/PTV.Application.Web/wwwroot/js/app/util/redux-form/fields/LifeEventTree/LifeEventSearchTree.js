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
import * as FintoSelectors from './selectors'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asField from 'util/redux-form/HOC/asField'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  LocalizedNodes,
  ChildrenIconNode,
  NodeLabelCheckBox,
  withCustomLabel,
  withLazyLoading,
  withFocus,
  withTreeNavigation,
  asNode
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import { loadNodeChildren } from 'actions/nodes'
import { Label } from 'sema-ui-components'
import { commonAppMessages } from 'util/redux-form/messages'
import Placeholder from 'appComponents/Placeholder'

const LifeEventNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue && !ownProps.showSelected &&
      FintoSelectors.getOrderedFilteredLifeEvents(state, ownProps) ||
      FintoSelectors.getOrderedLifeEvents(state, ownProps)
    return { nodes }
  }),
  withFocus()
)(LocalizedNodes)

const LifeEventNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue && !ownProps.showSelected
      const selector = isSearch && FintoSelectors.getFilteredLifeEvent || FintoSelectors.getLifeEvent
      return {
        node: selector(state, ownProps),
        disabled: ownProps.disabled,
        checked:  !ownProps.disabled && ownProps.value && ownProps.value.get(ownProps.id) || false,
        defaultCollapsed: !isSearch
      }
    }, {
      loadNodeChildren
    }
  ),
  withLazyLoading({
    load: (props) => {
      props.loadNodeChildren({
        treeNodeSchema: FintoSchemas.LIFE_EVENT,
        treeType: 'LifeEvent',
        node: props.node,
        contextId: props.contextId
      })
    }
  }),
  withCustomLabel(NodeLabelCheckBox),
  asNode,
  withTreeNavigation
)(ChildrenIconNode)

const TreeCompare = compose(
  asField()
)(RenderTreeView)

const LifeEventSearchTree = ({
  intl: { formatMessage },
  options,
  lifeEventAny,
  ...rest
}) => {
  return (
    <div>
      <TreeCompare
        name='lifeEvents'
        NodesComponent={LifeEventNodes}
        NodeComponent={LifeEventNode}
        showSelected
        {...rest} />
      <Label labelText={formatMessage(commonAppMessages.searchFilter)} />
      <TreeCompare
        name='lifeEvents'
        NodesComponent={LifeEventNodes}
        NodeComponent={LifeEventNode}
        placeholderComponent={<Placeholder placeholder={formatMessage(commonAppMessages.emptySearch)} />}
        filterSelected
        {...rest}
      />
    </div>
  )
}
LifeEventSearchTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array,
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  lifeEventAny: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  withFormStates,
  connect((state, ownProps) => ({
    lifeEventAny: FintoSelectors.isAnyLifeEvent(state, ownProps)
  })),
)(LifeEventSearchTree)
