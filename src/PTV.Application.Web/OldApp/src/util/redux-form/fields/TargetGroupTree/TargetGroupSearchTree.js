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
import { Field } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  LocalizedNodes,
  ChildrenIconNode,
  asNode,
  NodeLabelCheckBox,
  withCustomLabel,
  withFocus,
  withTreeNavigation
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import * as nodeActions from 'actions/nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import {
  getOrderedTargetGroups,
  getTargetGroup
} from './selectors'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const TargetGroupNodes = compose(
  injectFormName,
  connect((state, ownProps) => {
    const nodes = getOrderedTargetGroups(state, ownProps)
    return { nodes, listType: 'simple' }
  }),
  withFocus()
)(LocalizedNodes)

const TargetGroupNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const node = getTargetGroup(state, ownProps)
      const checked = ownProps.value && ownProps.value.get(ownProps.id) || false
      const collapsed = node.get('children').find(x => ownProps.value.contains(x))
      return {
        node,
        checked: checked,
        isLeaf: node.get('isLeaf'),
        defaultCollapsed: !collapsed
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  localizeItem({
    input: 'node',
    output: 'node',
    languageTranslationType: languageTranslationTypes.locale
  }),
  withCustomLabel(NodeLabelCheckBox),
  asNode,
  withTreeNavigation
)(ChildrenIconNode)

const TargetGroupSearchTree = ({
  ...rest
}) => (
  <Field
    name='targetGroups'
    component={RenderTreeView}
    NodesComponent={TargetGroupNodes}
    NodeComponent={TargetGroupNode}
    simple
    {...rest}
  />
)
TargetGroupSearchTree.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withFormStates
)(TargetGroupSearchTree)

