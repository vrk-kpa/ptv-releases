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
import { RenderTextFieldDisplay, RenderTreeView } from 'util/redux-form/renders'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import withLabel from 'util/redux-form/HOC/withLabel'
import asComparable from 'util/redux-form/HOC/asComparable'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl } from 'util/react-intl'
import { compose } from 'redux'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import { localizeList } from 'appComponents/Localize'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { getHospitalRegions } from './selectors'

const messages = defineMessages({
  label: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Hospital.Title',
    defaultMessage: 'Valitse pääsääntöiset Sairaanhoitopiirit, joisaa organisaatio toimii:'
  }
})

const AreaCheckBox = props =>
  <NodeLabelCheckBox
    {...props}
    checked={props.value && props.value.get(props.id) || false}
  />

AreaCheckBox.propTypes = {
  value: ImmutablePropTypes.set,
  id: PropTypes.string
}

const AreaNodes = compose(
  connect(
    (state, ownProps) => ({
      nodes: getHospitalRegions(state, ownProps)
    })
  ),
  localizeList({
    isSorted: true,
    input: 'nodes',
    output: 'nodes'
  }),
  SearchFilter.filterListComponent(
    (node, { searchValue }) => {
      return searchValue === '' || node.get('name').toLowerCase().indexOf(searchValue) > -1
    },
    'nodes'
  )
)(Nodes)

const AreaNode = compose(
  withCustomLabel(AreaCheckBox)
)(({ id, ...rest }) =>
  <Node
    {...rest}
    id={id.get('id')}
    node={id}
    showChildren
    isLeaf
  />
)

const AreaTypeHospitalRegion = props => (
  <Field
    name='hospitalRegions'
    component={RenderTreeView}
    NodesComponent={AreaNodes}
    NodeComponent={AreaNode}
    {...props}
  />
)

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  withLabel(messages.label, null, true),
  SearchFilter.withSearchFilter({
    partOfTree: true
  })
)(AreaTypeHospitalRegion)
