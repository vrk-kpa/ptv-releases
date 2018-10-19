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
import * as FintoSelectors from './selectors'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import TreeShortList from '../TreeShortList'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asField from 'util/redux-form/HOC/asField'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  ChildrenIconNode,
  NodeLabelCheckBox,
  withCustomLabel,
  withLazyLoading,
  withFocus,
  withTreeNavigation,
  asNode
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { EntitySchemas } from 'schemas'
import { loadNodeChildren } from 'Containers/Common/Actions/Nodes'
import { localizeItem, languageTranslationTypes } from 'appComponents/Localize'
import { commonAppMessages } from 'util/redux-form/messages'
import Placeholder from 'appComponents/Placeholder'
import { Label } from 'sema-ui-components'
import { createOrganizationNameWithStatus } from 'util/redux-form/fields/Organization/selectors'

const OrganizationNodes = compose(
  connect((state, ownProps) => {
    const nodes = FintoSelectors.getOrganizationIds(state, ownProps)
    return {
      nodes,
      listType: 'simple'
    }
  }),
  withFocus()
)(Nodes)

const getOrganizationLabel = createOrganizationNameWithStatus('node', node => node.get('name'))

const OrganizationCheckbox = compose(
  injectIntl,
  connect(
    (state, { node, intl }) => ({
      node: node.set('name', getOrganizationLabel(state, { id: node.get('publishingStatus'), node, intl }))
    })
  )
)(NodeLabelCheckBox)

const OrganizationNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const node = FintoSelectors.getOrganization(state, ownProps)
      return {
        node,
        checked: ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: ownProps.disabled,
        defaultCollapsed: true
      }
    }, {
      loadNodeChildren
    }
  ),
  withLazyLoading({
    load: (props) => {
      props.loadNodeChildren({
        treeNodeSchema: EntitySchemas.ORGANIZATION,
        treeType: 'Organization',
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
  withCustomLabel(OrganizationCheckbox),
  asNode,
  withTreeNavigation
)(ChildrenIconNode)

const TreeSuggested = compose(
  asField()
)(RenderTreeView)

const OrganizationTree = ({
  intl: { formatMessage },
  ...rest
}) => {
  return (
    <Fragment>
      <TreeShortList
        name='organizationIds'
        // selector={FintoSelectors.getSelectedTreeOrganizationEntities}
        NodesComponent={OrganizationNodes}
        NodeComponent={OrganizationNode}
        showSelected
        {...rest} />
      {rest.searchValue &&
        <Fragment>
          <Label labelText={formatMessage(commonAppMessages.searchFilter)} />
          <TreeSuggested
            name='organizationIds'
            // selector={FintoSelectors.getSuggestedTreeOrganizationEntities}
            NodesComponent={OrganizationNodes}
            NodeComponent={OrganizationNode}
            placeholderComponent={<Placeholder placeholder={formatMessage(commonAppMessages.emptySearch)} />}
            {...rest}
          />
        </Fragment>
      }
    </Fragment>
  )
}
OrganizationTree.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withFormStates,
  asDisableable
)(OrganizationTree)
