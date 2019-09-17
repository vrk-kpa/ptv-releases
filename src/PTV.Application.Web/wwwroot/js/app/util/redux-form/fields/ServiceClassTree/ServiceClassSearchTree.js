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
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'
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

const ServiceClassNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue && !ownProps.showSelected &&
      FintoSelectors.getOrderedFilteredServiceClasses(state, ownProps) ||
      FintoSelectors.getOrderedServiceClasses(state, ownProps)
    return { nodes, listType: 'simple' }
  }),
  withFocus()
)(LocalizedNodes)

const ServiceClassNode = compose(
  injectFormName,
  injectIntl,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue && !ownProps.showSelected
      const selector = isSearch && FintoSelectors.getFilteredServiceClass || FintoSelectors.getServiceClass
      return {
        node: selector(state, ownProps),
        checked: ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: ownProps.disabled,
        defaultCollapsed: !isSearch
      }
    }, {
      loadNodeChildren
    }
  ),
  withLazyLoading({
    load: (props) => {
      props.loadNodeChildren({
        treeNodeSchema: FintoSchemas.SERVICE_CLASS,
        treeType: 'ServiceClass',
        node: props.node,
        contextId: props.contextId
      })
    }
  }),
  localizeItem({
    input: 'node',
    output: 'node',
    languageTranslationType: languageTranslationTypes.locale
  }),
  withCustomLabel(NodeLabelCheckBox),
  asNode,
  withTreeNavigation
)(ChildrenIconNode)

const TreeCompare = compose(
  asField()
)(RenderTreeView)

const ServiceClassSearchTree = ({
  intl: { formatMessage },
  options,
  ...rest
}) => {
  return (
    <div>
      <TreeCompare
        name='serviceClasses'
        NodesComponent={ServiceClassNodes}
        NodeComponent={ServiceClassNode}
        showSelected
        {...rest} />
      <Label labelText={formatMessage(commonAppMessages.searchFilter)} />
      <TreeCompare
        name='serviceClasses'
        NodesComponent={ServiceClassNodes}
        NodeComponent={ServiceClassNode}
        placeholderComponent={<Placeholder placeholder={formatMessage(commonAppMessages.emptySearch)} />}
        filterSelected
        {...rest}
      />
    </div>
  )
}
ServiceClassSearchTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array
}

export default compose(
  injectIntl,
  withFormStates
)(ServiceClassSearchTree)
