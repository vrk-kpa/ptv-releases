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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { localizeItem } from 'appComponents/Localize'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import withLabel from 'util/redux-form/HOC/withLabel'
import asComparable from 'util/redux-form/HOC/asComparable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asField from 'util/redux-form/HOC/asField'
import { TreeListDisplay } from 'util/redux-form/fields/TreeListDisplay'
import { compose } from 'redux'
import { RenderTreeView } from 'util/redux-form/renders'
import {
  LocalizedNodes,
  Node,
  NodeLabelCheckBox,
  NodeLabel,
  withCustomLabel,
  withLazyLoading
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import * as nodeActions from 'actions/nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import CommonMessages from 'util/redux-form/messages'
const IndustrialClassNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue && FintoSelectors.getOrderedFilteredIndustrialClasses(state, ownProps) ||
      FintoSelectors.getOrderedIndustrialClasses(state, ownProps)
    return { nodes, listType: 'simple' }
  })
)(LocalizedNodes)

const CustomNodeLabelChecbox = compose(
  asDisableable)((props) => props.disabled ? <NodeLabel {...props} /> : <NodeLabelCheckBox {...props} />)

const IndustrialClassNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue
      const selector = FintoSelectors.getIndustrialClass
      const fromGD = FintoSelectors.getGeneralDescriptionIndustrialClassesIds(state, ownProps)
      const isFromGD = fromGD.includes(ownProps.id)
      const node = selector(state, ownProps)
      return {
        node,
        checked: isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: isFromGD || !node.get('isLeaf'),
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
  withCustomLabel(CustomNodeLabelChecbox)
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
    defaultMessage: 'Lisätyt toimialaluokat: (voit poistaa klikkaamalla rastia)'
  },
  gdListLabel: {
    id: 'Containers.Services.AddService.Step2.IndustrialClass.GeneralDescriptionList.Header',
    defaultMessage: 'Pohjakuvauksesta tulevat toimialaluokat:'
  },
  gdListTooltip: {
    id: 'Containers.Services.AddService.Step2.IndustrialClass.GeneralDescriptionList.Tooltip',
    defaultMessage: 'Et voi poistaa valintoja, mutta voit lisätä uusia.'
  }
})

const TreeCompare = compose(
  withLabel(messages.label, messages.tooltip),
  asComparable(),
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.clearTreeSearch
  }),
  SearchFilter.withSearchFilter(),
  asField()
)(RenderTreeView)

const IndustrialClassTree = ({
  intl: { formatMessage },
  isReadOnly,
  options,
  isCompareMode,
  industrialClassAny,
  gdIndustrialClassAny,
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
        {(gdIndustrialClassAny || isReadOnly) && <TreeListDisplay
          name='gdIndustrialClasses'
          label={formatMessage(!isReadOnly && messages.gdListLabel || messages.label)}
          tooltip={!isReadOnly && formatMessage(messages.gdListTooltip) || null}
          selector={FintoSelectors.getGDIndustrialClassesForIdsJs}
          invalidItemTooltip={formatMessage(CommonMessages.invalidItemTooltip)}
          {...rest}
        />}
        {industrialClassAny && <TreeListDisplay
          name='industrialClasses'
          label={!isReadOnly && formatMessage(messages.listLabel) || null}
          selector={FintoSelectors.getIndustrialClassesForIdsJs}
          renderAsRemovable={!isReadOnly}
          invalidItemTooltip={formatMessage(CommonMessages.invalidItemTooltip)}
          {...rest}
        />}
      </div>
    </div>
  )
}
IndustrialClassTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array,
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  gdIndustrialClassAny: PropTypes.bool.isRequired,
  industrialClassAny: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  asDisableable,
  withFormStates,
  connect((state, ownProps) => ({
    industrialClassAny: FintoSelectors.isAnyIndustrialClass(state, ownProps),
    gdIndustrialClassAny: FintoSelectors.isAnyGDIndustrialClass(state, ownProps)
  })),
)(IndustrialClassTree)
