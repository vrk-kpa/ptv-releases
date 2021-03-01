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
import * as FintoSelectors from './selectors'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { localizeItem } from 'appComponents/Localize'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import withLabel from 'util/redux-form/HOC/withLabel'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { TreeListDisplay } from 'util/redux-form/fields/TreeListDisplay'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import asField from 'util/redux-form/HOC/asField'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withLazyLoading
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import * as nodeActions from 'actions/nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import CommonMessages from 'util/redux-form/messages'

const DigitalAuthorizationNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue &&
      FintoSelectors.getFilteredDigitalAuthorizationIds(state, ownProps) ||
      FintoSelectors.getDigitalAuthorizationIds(state, ownProps)
    return { nodes }
  })
)(Nodes)

const DigitalAuthorizationNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue
      const selector = isSearch && FintoSelectors.getFilteredDigitalAuthorization ||
        FintoSelectors.getDigitalAuthorization
      return {
        node: selector(state, ownProps),
        checked: ownProps.value && ownProps.value.get(ownProps.id) || false,
        defaultCollapsed: !isSearch && !ownProps.isRootLevel,
        NodeLabelComponent: !ownProps.isRootLevel && NodeLabelCheckBox || undefined
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  withLazyLoading({
    load: (props) => {
      props.actions.loadNodeChildren({
        treeNodeSchema: FintoSchemas.DIGITAL_AUTHORIZATION,
        treeType: 'DigitalAuthorization',
        node: props.node,
        contextId: props.contextId
      })
    }
  }),
  localizeItem({ input: 'node', output: 'node' }),
)(Node)

const messages = defineMessages({
  label: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Title',
    defaultMessage: 'Digitaalinen lupa'
  },
  tooltip: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Tooltip',
    defaultMessage: 'Digitaalinen lupa'
  },
  listLabel: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.TargetList.Header',
    defaultMessage: 'Lisätyt tiedot'
  },
  info: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Info',
    defaultMessage: 'Notification text that user need to be check that selections are same as done under Digital authorizations'
  }
})

const TreeCompare = compose(
  asField(),
  withLabel(messages.label, messages.tooltip),
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.clearTreeSearch
  }),
  SearchFilter.withSearchFilter({
    filterFunc: (props, value) => {
      if (value !== '') {
        props.searchInTree({
          searchSchema: FintoSchemas.DIGITAL_AUTHORIZATION_ARRAY,
          treeType: 'DigitalAuthorization',
          value,
          contextId: value
        })
      } else {
        props.clearTreeSearch('DigitalAuthorization', props.contextId)
      }
    },
    partOfTree: true
  })
)(RenderTreeView)

const DigitalAuthorizationTree = ({
  intl: { formatMessage },
  options,
  isReadOnly,
  ...rest
}) => (
  <div className='row'>
    {!isReadOnly &&
      <div className='col-lg-12 mb-2 mb-lg-0'>
        <TreeCompare
          name='digitalAuthorizations'
          NodesComponent={DigitalAuthorizationNodes}
          NodeComponent={DigitalAuthorizationNode}
          {...rest}
        />
      </div>
    }
    <div className='col-lg-12'>
      <TreeListDisplay
        name='digitalAuthorizations'
        label={isReadOnly && formatMessage(messages.label) || formatMessage(messages.listLabel)}
        selector={FintoSelectors.getDigitalAuthorizationsForIdsJs}
        invalidItemTooltip={formatMessage(CommonMessages.invalidItemTooltip)}
        {...rest}
      />
    </div>
  </div>
)
DigitalAuthorizationTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array,
  isReadOnly: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  asDisableable,
  withFormStates
)(DigitalAuthorizationTree)
