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
import {
  getOntologyTermIds,
  getGeneralDescriptionOntologyTermsIds,
  getOntologyTerm,
  getFormOntologyTermsIds,
  getAnnotationIsFetching,
  getNonAnnotationTermIds,
  getFormAnnotationTermsCount,
  getIsAnyOntologyTerm,
  getOntologyTermsCount,
  getGeneralDescriptionOntologyTermsCount,
  getReadOnlyOntologyThermsIdsJs,
  getGDOntologyTermsForIdsJs,
  getOntologyTermsForIdsJs,
  getAnnotationTermsForIds
} from './selectors'
import { Fields } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import { localizeItem, localizeList } from 'appComponents/Localize'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import asComparable from 'util/redux-form/HOC/asComparable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import asGroup from 'util/redux-form/HOC/asGroup'
import { TreeListDisplay, SortedTreeListDisplay } from 'util/redux-form/fields/TreeListDisplay'
import { compose } from 'redux'
import { messages } from './messages'
import CommonMessages from 'util/redux-form/messages'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withDetails,
  TreeView
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas } from 'schemas'
import * as nodeActions from 'Containers/Common/Actions/Nodes'
import { Label } from 'sema-ui-components'
import withState from 'util/withState'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import { OrderedSet } from 'immutable'
import OntologySearch from './OntologySearch'
import GetAnnotationButton from './GetAnnotationButton'

const OntologyTermNodes = compose(
  connect((state, ownProps) => {
    const nodes = getOntologyTermIds(state, ownProps)
    return { nodes }
  })
)(Nodes)

const OntologyNodeSelect = compose(
  injectIntl
)(
  ({ intl: { formatMessage }, id, onSearchValueChange, node, isCurrentNode }) => {
    const onClick = () => onSearchValueChange({ value: id, label: node && node.get('name') })
    const nodeChildrenCount = node && node.get('children').size
    if (isCurrentNode) return null
    return <div onClick={onClick} className={styles.target} title={formatMessage(messages.ontologyGoToNodeTooltip)}>
      {nodeChildrenCount
        ? <PTVIcon name='icon-level-up' />
        : <PTVIcon name='icon-level-down' />
      }
    </div>
  }
)

const OntologyTermNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const isSearch = ownProps.searchValue
      const fromGD = getGeneralDescriptionOntologyTermsIds(state, ownProps)
      const isFromGD = fromGD.includes(ownProps.id)
      const isCurrentNode = isSearch && isSearch.value === ownProps.id
      const node = getOntologyTerm(state, ownProps)
      return {
        node,
        checked: isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: isFromGD,
        defaultCollapsed: !isSearch,
        isCurrentNode,
        isRootLevel: false,
        nodeClass: styles.ontologyNode
      }
    }, {
      loadNodeChildren: nodeActions.loadNodeChildrens
    }
  ),
  localizeItem({ input: 'node', output: 'node' }),
  withCustomLabel(NodeLabelCheckBox),
  withDetails(props =>
    <div className={styles.ontologyNodeSelect}>
      <OntologyNodeSelect {...props} />
    </div>)
)(Node)

const AnnotationTermNodes = compose(
  connect((state, ownProps) => {
    const nodeEntities = getAnnotationTermsForIds(state, ownProps)
    return {
      nodes: nodeEntities
    }
  }),
  localizeList({
    input: 'nodes',
    isSorted: true
  })
)(Nodes)

const AnnotationTermNode = compose(
  injectFormName,
  connect(
    (state, ownProps) => {
      const id = ownProps.id.get('id')
      const fromGD = getGeneralDescriptionOntologyTermsIds(state, ownProps)
      const isFromGD = fromGD.includes(id)
      const fromService = getFormOntologyTermsIds(state, ownProps)
      const isFromService = fromService.includes(id)
      return {
        id,
        node: ownProps.id,
        checked: isFromGD || isFromService || ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: isFromGD,
        isLeaf: true
      }
    }
  ),
  withCustomLabel(NodeLabelCheckBox)
)(Node)

const GroupField = compose(
  withErrorDisplay('ontologyTerms'),
  asGroup({
    title: CommonMessages.ontologyTerms,
    tooltip: <FormattedMessage {...messages.tooltip} />,
    required: true
  }),
  asComparable(),
  withState({
    initialState: {
      isOpen: false
    }
  }),
  connect((state, ownProps) => ({
    annotationIsFetching: getAnnotationIsFetching(state, ownProps),
    nonAnnotationTermIds: getNonAnnotationTermIds(state, ownProps)
  })),
  injectIntl)(
  ({
    isOpen,
    updateUI,
    intl: { formatMessage },
    dispatch,
    formName,
    annotationData,
    nonAnnotationTermIds,
    annotationIsFetching,
    ...rest
  }) => {
    return (
      <div>
        <div className='form-row'>
          <p className='small'>{formatMessage(messages.annotationTitle)}</p>
          <Label labelText={formatMessage(messages.treeLink)} labelPosition='top' />
          {<OntologyTermTreeComponent {...rest} />}
        </div>
      </div>
    )
  }
)

const OntologyTermTreeComponent = compose(
  injectIntl,
  injectFormName,
  injectSelectPlaceholder(),
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.clearTreeSearch
  }),
  SearchFilter.withSearchFilter({
    filterFunc: (props, value) => {
      if (value) {
        props.searchInTree({
          searchSchema: FintoSchemas.ONTOLOGY_TERM_ARRAY,
          treeType: 'OntologyTerm',
          id: value.value,
          contextId: value.value
        })
      } else {
        props.clearTreeSearch('OntologyTerm', props.contextId)
      }
    },
    isText: false,
    FilterComponent: OntologySearch,
    partOfTree: true
  })
)(
  ({ ...rest }) => (
    <Fields
      names={['ontologyTerms', 'annotationTerms']}
      component={RenderOntologyTreeView}
      {...rest}
    />
  )
)

const RenderOntologyTreeView = compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    annotationTermCount: getFormAnnotationTermsCount(state, { formName })
  }))
)(fields => {
  const newOntologyValue = (fields.ontologyTerms.input.value || OrderedSet())
  const newAnnotationValue = (fields.annotationTerms.input.value || OrderedSet())
  return (
    <div>
      <TreeView
        label={fields.intl.formatMessage(messages.treeLink)}
        onChange={(id, checked) => {
          fields.ontologyTerms.input.onChange(checked ? newOntologyValue.add(id) : newOntologyValue.delete(id))
        }}
        value={newOntologyValue}
        nodeLabelClass={styles.ontologyNodeLabelWrap}
        NodesComponent={OntologyTermNodes}
        NodeComponent={OntologyTermNode}
        {...fields}
      />
      <div className='form-row'>
        <Label labelText={fields.intl.formatMessage(messages.annotationLabel)} labelPosition='top' />
        <GetAnnotationButton className={styles.annotationButton} />
        {fields.annotationTermCount > 0 &&
          <TreeView
            label={fields.label}
            onChange={(id, checked) => {
              fields.ontologyTerms.input.onChange(checked ? newOntologyValue.add(id) : newOntologyValue.delete(id))
            }}
            value={newAnnotationValue}
            NodesComponent={AnnotationTermNodes}
            NodeComponent={AnnotationTermNode}
            {...fields}
            className={styles.fullBorder}
          />
        }
      </div>
    </div>
  )
})

const OntologyTermTree = ({
  options,
  ...rest
}) => {
  const basicCompareModeClass = rest.isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
  return (
    <div className='row'>
      {!rest.isReadOnly &&
        <div className={basicCompareModeClass}>
          <GroupField {...rest} />
        </div> ||
        null
      }
      <OntologyTermTreeLists {...rest} />
    </div>
  )
}
OntologyTermTree.propTypes = {
  options: PropTypes.array,
  intl: intlShape,
  isReadOnly: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool.isRequired
}

const OntologyTermTreeLists = compose(
  connect((state, ownProps) => ({
    showOntologyLists: getIsAnyOntologyTerm(state, ownProps),
    ontologyTermsCount: getOntologyTermsCount(state, ownProps),
    gdOntologyTermsCount: getGeneralDescriptionOntologyTermsCount(state, ownProps)
  }))
)(({
  intl: { formatMessage },
  isReadOnly,
  showOntologyLists,
  isCompareMode,
  ontologyTermsCount,
  gdOntologyTermsCount,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
  return (
    <div className={basicCompareModeClass}>

      {(isReadOnly) &&
        <SortedTreeListDisplay
          name='allReadOnlyOntologyTerms'
          label={formatMessage(CommonMessages.ontologyTerms)}
          selector={getReadOnlyOntologyThermsIdsJs}
          {...rest}
        />
      }

      {(gdOntologyTermsCount > 0 && !isReadOnly) &&
      <TreeListDisplay
        name='gdOntologyTerms'
        label={
          !isReadOnly && formatMessage(messages.targetListHeader, { count:gdOntologyTermsCount }) ||
          formatMessage(CommonMessages.ontologyTerms)
        }
        tooltip={!isReadOnly && formatMessage(messages.targetListHeaderTooltip)}
        selector={getGDOntologyTermsForIdsJs}
        {...rest}
      />}

      {(showOntologyLists && !isReadOnly) &&
        <div>
          <div className={styles.onologyTreeList} />
          <TreeListDisplay
            label={!isReadOnly && formatMessage(messages.annotationListHeader, { count:ontologyTermsCount })}
            name='ontologyTerms'
            selector={getOntologyTermsForIdsJs}
            renderAsRemovable={!isReadOnly}
            {...rest}
          />
        </div>}
    </div>
  )
})

OntologyTermTreeLists.propTypes = {
  intl: intlShape,
  isReadOnly: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  withFormStates,
)(OntologyTermTree)
