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
import { Field } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withFocus
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { getServiceTypesIds,
  getServiceType
} from './selectors'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const ServiceTypeNodes = compose(
  injectFormName,
  connect((state, ownProps) => {
    const nodes = getServiceTypesIds(state, ownProps)
    return { nodes, listType: 'simple' }
  }),
  withFocus()
)(Nodes)

const ServiceTypeNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const node = getServiceType(state, ownProps)
      const checked = ownProps.value && ownProps.value.get(ownProps.id) || false
      return {
        node,
        checked: checked,
        isLeaf: true
      }
    }
  ),
  localizeItem({
    input: 'node',
    output: 'node',
    languageTranslationType: languageTranslationTypes.locale
  }),
  withCustomLabel(NodeLabelCheckBox)
)(Node)

const ServiceTypeSearchTree = ({
  ...rest
}) => (
  <Field
    name='serviceTypes'
    component={RenderTreeView}
    NodesComponent={ServiceTypeNodes}
    NodeComponent={ServiceTypeNode}
    simple
    {...rest}
  />
)
ServiceTypeSearchTree.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withFormStates
)(ServiceTypeSearchTree)

