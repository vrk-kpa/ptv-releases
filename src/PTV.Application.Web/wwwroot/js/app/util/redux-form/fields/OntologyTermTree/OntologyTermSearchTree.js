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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { getOntologyTerm, getOntologyTermIds } from './selectors'
import { List } from 'immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asField from 'util/redux-form/HOC/asField'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withFocus
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { commonAppMessages } from 'util/redux-form/messages'
import Placeholder from 'appComponents/Placeholder'
import { Label } from 'sema-ui-components'

const OntologyTermNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.id ? List() : getOntologyTermIds(state, ownProps)
    return { nodes, listType: 'simple' }
  }),
  withFocus()
)(Nodes)

const OntologyTermNode = compose(
  injectFormName,
  injectIntl,
  connect(
    (state, ownProps) => {
      return {
        node: getOntologyTerm(state, ownProps),
        checked: ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: ownProps.disabled,
        defaultCollapsed: true,
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

const TreeCompare = compose(
  asField()
)(RenderTreeView)

const OntologyTermSearchTree = ({
  intl: { formatMessage },
  options,
  ...rest
}) => {
  return (
    <Fragment>
      <TreeCompare
        name='ontologyTerms'
        NodesComponent={OntologyTermNodes}
        NodeComponent={OntologyTermNode}
        showSelected
        filterSelected
        {...rest}
      />
      {rest.searchValue &&
        <Fragment>
          <Label labelText={formatMessage(commonAppMessages.searchFilter)} />
          <TreeCompare
            name='ontologyTerms'
            NodesComponent={OntologyTermNodes}
            NodeComponent={OntologyTermNode}
            placeholderComponent={<Placeholder placeholder={formatMessage(commonAppMessages.emptySearch)} />}
            {...rest}
          />
        </Fragment>
      }
    </Fragment>
  )
}
OntologyTermSearchTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array
}

export default compose(
  injectIntl,
  withFormStates
)(OntologyTermSearchTree)
