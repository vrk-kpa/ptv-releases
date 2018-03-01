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
// import { Field } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { localizeItem } from 'appComponents/Localize'
import {
  SearchFilter,
  injectFormName,
  withLabel,
  withFormStates,
  asDisableable,
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
  ToolTip,
  withCustomLabel,
  withDetails,
  withLazyLoading
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import * as nodeActions from 'Containers/Common/Actions/Nodes'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  tooltip: {
    id: 'Containers.Services.AddService.Step2.ServiceClass.Tooltip',
    defaultMessage: 'Palvelulle valitaan aihepiirin mukaan palveluluokka. Valitse vähintään yksi mahdollisimman tarkka palveluluokka. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen palveluluokka/luokat. Voit tarvittaessa lisätä palveluluokkia.'
  },
  listLabel: {
    id: 'Containers.Services.AddService.Step2.ServiceClass.TargetList.Header',
    defaultMessage: 'Valitut palveluluokat:'
  },
  tooltipLink: {
    id: 'Util.ReduxForm.Renders.TreeView.TooltipLink',
    defaultMessage: 'Lisätiedot'
  }
})

const ServiceClassNodes = compose(
  connect((state, ownProps) => {
    const nodes = ownProps.searchValue &&
      FintoSelectors.getFilteredServiceClassIds(state, ownProps) ||
      FintoSelectors.getServiceClassIds(state, ownProps)
    return { nodes }
  })
)(Nodes)

const ServiceClassNode = compose(
  injectFormName,
  injectIntl,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue
      const selector = isSearch && FintoSelectors.getFilteredServiceClass || FintoSelectors.getServiceClass
      const fromGD = FintoSelectors.getGeneralDescriptionServiceClassesIds(state, ownProps)
      const isFromGD = fromGD.includes(ownProps.id)
      return {
        node: selector(state, ownProps),
        checked: isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: ownProps.disabled || isFromGD,
        defaultCollapsed: !isSearch
      }
    },
    mapDispatchToProps([nodeActions])
  ),
  withLazyLoading({
    load: (props) => {
      props.actions.loadNodeChildren({
        treeNodeSchema: FintoSchemas.SERVICE_CLASS,
        treeType: 'ServiceClass',
        node: props.node,
        contextId: props.contextId
      })
    }
  }),
  localizeItem({ input: 'node', output: 'node' }),
  withCustomLabel(NodeLabelCheckBox),
  withDetails(props =>
    <ToolTip
      {...props}
      tooltipLink={props.intl.formatMessage(messages.tooltipLink)}
    />
  )
)(Node)

const TreeCompare = compose(
  withLabel(CommonMessages.serviceClasses, messages.tooltip, true),
  asComparable(),
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.clearTreeSearch
  }),
  SearchFilter.withSearchFilter({
    filterFunc: (props, value) => {
      if (value !== '') {
        props.searchInTree({
          searchSchema: FintoSchemas.SERVICE_CLASS_ARRAY,
          treeType: 'ServiceClass',
          value,
          contextId: value
        })
      } else {
        props.clearTreeSearch('ServiceClass', props.contextId)
      }
    },
    partOfTree: true
  }),
  asField()
)(RenderTreeView)

const ServiceClassTree = ({
  intl: { formatMessage },
  options,
  isReadOnly,
  isCompareMode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
  return (
    <div className='row'>
      {!isReadOnly &&
      <div className={basicCompareModeClass}>
        <TreeCompare
          name='serviceClasses'
          NodesComponent={ServiceClassNodes}
          NodeComponent={ServiceClassNode}
          {...rest}
        />
      </div> || null
      }
      <div className={basicCompareModeClass}>
        <TreeListDisplay
          name='serviceClasses'
          label={formatMessage(!isReadOnly && messages.listLabel || CommonMessages.serviceClasses)}
          selector={FintoSelectors.getServiceClassesForIdsJs}
          {...rest}
        />
      </div>
    </div>
  )
}
ServiceClassTree.propTypes = {
  intl: intlShape,
  options: PropTypes.array,
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  asDisableable,
  withFormStates
)(ServiceClassTree)
