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
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { localizeItem } from 'appComponents/Localize'
import {
  SearchFilter,
  injectFormName,
  withLabel,
  withFormStates,
  asComparable,
  asField
} from 'util/redux-form/HOC'
import { TreeListDisplay } from 'util/redux-form/fields'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withLazyLoading
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import * as nodeActions from 'Containers/Common/Actions/Nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'

const IndustrialClassNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue &&
      FintoSelectors.getFilteredIndustrialClassIds(state, ownProps) ||
      FintoSelectors.getIndustrialClassIds(state, ownProps)
    return { nodes }
  })
)(Nodes)

const IndustrialClassNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue
      const selector = isSearch && FintoSelectors.getFilteredIndustrialClass || FintoSelectors.getIndustrialClass
      const fromGD = FintoSelectors.getGeneralDescriptionIndustrialClassesIds(state, ownProps)
      const isFromGD = fromGD.includes(ownProps.id)
      return {
        node: selector(state, ownProps),
        checked: isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: isFromGD,
        defaultCollapsed: !isSearch
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  withLazyLoading({
    load: (props) => {
      props.actions.loadNodeChildren({
        treeNodeSchema: FintoSchemas.INDUSTRIAL_CLASS,
        treeType: 'IndustrialClass',
        node: props.node,
        contextId: props.contextId
      })
    }
  }),
  localizeItem({ input: 'node', output: 'node' }),
  withCustomLabel(NodeLabelCheckBox)
)(Node)

const messages = defineMessages({
  label: {
    id: 'Containers.Services.AddService.Step2.IndustrialClass.Title',
    defaultMessage: 'Toimialaluokka'
  },
  tooltip: {
    id: 'Containers.Services.AddService.Step2.IndustrialClass.Tooltip',
    defaultMessage: 'Valitse toimiala(t), jota lupa/ilmoitus/rekisteröinti koskee.'
  },
  listLabel: {
    id: 'Containers.Services.AddService.Step2.IndustrialClass.TargetList.Header',
    defaultMessage: 'Lisätyt toimialaluokat:'
  }
})

const TreeCompare = compose(
  withLabel(messages.label, messages.tooltip),
  asComparable(),
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.searchInTree
  }),
  SearchFilter.withSearchFilter({
    filterFunc: (props, value) => {
      if (value !== '') {
        props.searchInTree({
          searchSchema: FintoSchemas.INDUSTRIAL_CLASS_ARRAY,
          treeType: 'IndustrialClass',
          value,
          contextId: value
        })
      } else {
        props.clearTreeSearch('IndustrialClass', props.contextId)
      }
    },
    partOfTree: true
  }),
  asField()
)(RenderTreeView)

const IndustrialClassTree = ({
  intl: { formatMessage },
  isReadOnly,
  options,
  isCompareMode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
  return (
    <div className='row'>
      {!isReadOnly &&
      <div className={basicCompareModeClass}>
        <TreeCompare
          name='industrialClasses'
          component={RenderTreeView}
          NodesComponent={IndustrialClassNodes}
          NodeComponent={IndustrialClassNode}
          {...rest}
      />
      </div> || null
      }
      <div className={basicCompareModeClass}>
        <TreeListDisplay
          name='industrialClasses'
          label={formatMessage(!isReadOnly && messages.listLabel || messages.label)}
          selector={FintoSelectors.getIndustrialClassesForIdsJs}
          {...rest}
      />
      </div>
    </div>
  )
}
IndustrialClassTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array,
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  withFormStates
)(IndustrialClassTree)
