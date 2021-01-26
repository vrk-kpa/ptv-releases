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
import React, { Component } from 'react'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  ChildrenIconNode,
  NodeLabelCheckBox,
  withCustomLabel,
  withFocus,
  withTreeNavigation,
  asNode
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { List, fromJS } from 'immutable'
import { withProps } from 'recompose'
import { searchContentTypeEnum } from 'enums'

const contentTypeOptions = fromJS([
  { id: searchContentTypeEnum.SERVICE,
    expanded: true,
    nodes: [
      { id: searchContentTypeEnum.SERVICESERVICE },
      { id: searchContentTypeEnum.SERVICEPROFESSIONAL },
      { id: searchContentTypeEnum.SERVICEPERMIT }
    ]
  },
  { id: searchContentTypeEnum.CHANNEL,
    expanded: true,
    nodes: [
      { id: searchContentTypeEnum.ECHANNEL },
      { id: searchContentTypeEnum.WEBPAGE },
      { id: searchContentTypeEnum.PRINTABLEFORM },
      { id: searchContentTypeEnum.PHONE },
      { id: searchContentTypeEnum.SERVICELOCATION }
    ]
  },
  { id: searchContentTypeEnum.ORGANIZATION },
  { id: searchContentTypeEnum.GENERALDESCRIPTION },
  { id: searchContentTypeEnum.SERVICECOLLECTION }
])

const ContentTypeNodes = compose(
  injectFormName,
  withProps(ownProps => ({
    nodes: ownProps.nodes || List(),
    listType: 'simple'
  })),
  withFocus()
)(Nodes)

const checkboxSelection = (id, checked, props) => {
  if (props.nodes.size) {
    props.onValueChange(
      checked
        ? props.value.union(props.nodes.map(ct => ct.get('id')))
        : props.value.subtract(props.nodes.map(ct => ct.get('id')))
    )
  } else {
    props.onChange(id, checked)
  }
}

class ContentCheckbox extends Component {
  onChange = (id, checked) => checkboxSelection(id, checked, this.props)

  render () {
    return <NodeLabelCheckBox {...this.props} onChange={this.onChange} />
  }
}

const ContentTypeNode = compose(
  injectFormName,
  injectIntl,
  withProps(
    ({ options, messages, value, intl: { formatMessage }, ...rest }) => {
      const node = rest.id
      const id = node.get('id')
      const checked = value && value.get(node.get('id')) || false
      const nodes = node.get('nodes') || List()
      const selectedSubNodes = nodes.filter(x => value.has(x.get('id'))).size || 0
      const partiallyChecked = selectedSubNodes && (selectedSubNodes < nodes.size) || false
      const formattedNode = node.set(
        'name',
        formatMessage(messages[id]) + (selectedSubNodes && ` (${selectedSubNodes})` || '')
      )

      return {
        id,
        node: formattedNode,
        nodes,
        partiallyChecked,
        checked: checked || selectedSubNodes && selectedSubNodes === nodes.size,
        isLeaf: !node.has('nodes'),
        tabIndex: -1,
        defaultCollapsed: !node.get('expanded')
      }
    }
  ),
  withCustomLabel(ContentCheckbox),
  asNode,
  withTreeNavigation
)(ChildrenIconNode)

const ContentTypeSearchTree = ({
  ...rest
}) => (
  <Field
    name='contentTypes'
    component={RenderTreeView}
    NodesComponent={ContentTypeNodes}
    NodeComponent={ContentTypeNode}
    simple
    nodes={contentTypeOptions}
    {...rest}
  />
)
ContentTypeSearchTree.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withFormStates
)(ContentTypeSearchTree)

