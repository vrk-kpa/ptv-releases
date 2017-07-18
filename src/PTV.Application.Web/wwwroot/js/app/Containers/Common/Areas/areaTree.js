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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl } from 'react-intl'
import { List } from 'immutable'
import { getDefaultTranslationText } from 'Containers/Common/FintoTree'

// selectors
import * as CommonSelectors from 'Containers/Common/Selectors'

// components
import { localizeList } from 'appComponents/Localize'
import { PTVLabelCustomComponent } from 'Components'
import { Nodes, Node, NodeCheckBox, SearchTree } from 'appComponents/TreeView'
import { filterComponent, filterListComponent } from 'appComponents/SearchFilter'

// enums
import { areaTypes } from 'Containers/Common/Enums'

const configuration = {
  [areaTypes.MUNICIPALITY]: {
    sourceSelector: CommonSelectors.getMunicipalitiesList,
    getItemSelector: CommonSelectors.getMunicipality
  },
  [areaTypes.PROVINCE]: {
    sourceSelector: CommonSelectors.getProvinciesList,
    getItemSelector: CommonSelectors.getProvince
  },
  [areaTypes.BUSINESS_REGION]: {
    sourceSelector: CommonSelectors.getBusinessRegionsList,
    getItemSelector: CommonSelectors.getBusinessRegion
  },
  [areaTypes.HOSPITAL_REGION]: {
    sourceSelector: CommonSelectors.getHospitalRegionsList,
    getItemSelector: CommonSelectors.getHospitalRegion
  }
}

const getSelectors = (type) => configuration[type] || {}

const mapStateToCombo = () => (state, ownProps) => ({
  isSelectedSelector: CommonSelectors.getIsSelectedSelector(ownProps.listSelector)
})

const AreaCheckBox = connect(mapStateToCombo())(NodeCheckBox)

const AreaNodes = compose(
  injectIntl,
  connect((state, ownProps) => {
    const conf = getSelectors(ownProps.selectedType)
    const nodeIds = conf.sourceSelector && conf.sourceSelector(state, ownProps) || List()
    const nodes = conf.getItemSelector && nodeIds.map(id => conf.getItemSelector(state, { ...ownProps, id }) || id)
    return {
      nodes,
      getDefaultTranslationText: getDefaultTranslationText(ownProps.intl)
    }
  }),
  localizeList({
    isSorted: true,
    input: 'nodes',
    output: 'nodes'
  }),
  filterListComponent(
    (node, { searchText }) => {
      return searchText === '' || node.get('name').toLowerCase().indexOf(searchText) > -1
    },
    'nodes'
  )
)(Nodes)

const AreaNode = compose(
  connect((state, ownProps) => ({
    id: ownProps.id.get('id'),
    node: ownProps.id,
    // node: ownProps.itemSelector && ownProps.itemSelector(state, ownProps),
    showChildren: true,
    NodeLabelComponent: AreaCheckBox,
    isLeaf: true
  })) //,
  // filterComponent(
  //   ({ searchText, node }) => {
  //     console.log(searchText, node.get('name'), searchText === '' || node.get('name').toLowerCase().indexOf(searchText))
  //     return searchText === '' || node.get('name').toLowerCase().indexOf(searchText) > -1
  //   }
  // )
  // // ,  localizeItem({ input: 'node', output: 'node' })
)(Node)

const AreaTree = ({
  configuration,
  componentClass,
  id,
  validators,
  order,
  readOnly,
  label,
  ...rest
}) => {
  return (
    <PTVLabelCustomComponent
      {...rest}
      componentClass={componentClass}
      id={id}
      label={label}
      validators={validators}
      order={order}
      readOnly={readOnly}
    >
      <SearchTree {...rest}
        NodesComponent={AreaNodes}
        NodeComponent={AreaNode}
      />
    </PTVLabelCustomComponent>
  )
}

AreaTree.propTypes = {
  readOnly: PropTypes.bool.isRequired,
  order: PropTypes.number,
  validators: PropTypes.any,
  id: PropTypes.string.isRequired,
  componentClass: PropTypes.string,
  configuration: PropTypes.object.isRequired,
  label: PropTypes.string
}

export default AreaTree
