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
import * as FintoSelectors from './selectors'
import { Fields, change } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, FormattedMessage, intlShape } from 'react-intl'
import { localizeItem } from 'appComponents/Localize'
import {
  SearchFilter,
  asGroup,
  injectFormName,
  withFormStates,
  asComparable,
  asField
} from 'util/redux-form/HOC'
import { TreeListDisplay } from 'util/redux-form/fields'
import { compose } from 'redux'
import { messages } from './messages'
import CommonMessages from 'util/redux-form/messages'
import { RenderAsyncSelect } from 'util/redux-form/renders'
import {
  Nodes,
  Node,
  NodeLabelCheckBox,
  withCustomLabel,
  withDetails,
  TreeView
} from 'util/redux-form/renders/RenderTreeView/TreeView'
import { FintoSchemas, EntitySchemas } from 'schemas'
import * as nodeActions from 'Containers/Common/Actions/Nodes'
import { Label, Button, Spinner } from 'sema-ui-components'
import withState from 'util/withState'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import { apiCall3 } from 'actions'
import { formTypesEnum } from 'enums'
import { fromJS, OrderedSet } from 'immutable'

const OntologyTermNodes = compose(
  connect((state, ownProps) => {
    const nodes = FintoSelectors.getOntologyTermIds(state, ownProps)
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
      const fromGD = FintoSelectors.getGeneralDescriptionOntologyTermsIds(state, ownProps)
      const isFromGD = fromGD.includes(ownProps.id)
      const isCurrentNode = isSearch && isSearch.value === ownProps.id
      return {
        node: FintoSelectors.getOntologyTerm(state, ownProps),
        checked: isFromGD || ownProps.value && ownProps.value.get(ownProps.id) || false,
        disabled: isFromGD,
        defaultCollapsed: !isSearch,
        isCurrentNode,
        isRootLevel: false
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

const getSearchData = (input) => {
  return {
    searchValue: input,
    treeType: 'OntologyTerm',
    resultType: 'List'
    // languages: [props.selectedLanguage, props.language]
  }
}

const formatResponse = data =>
  data.map(({ id, name }) => ({
    label: name,
    value: id
  }))

const OntologySearch = ({ onChange, searchValue, ...rest }) => {
  return (
    <div className={styles.ontologyTermSearch}>
      <RenderAsyncSelect
        url='service/GetFilteredList'
        input={{ value: searchValue }}
        meta
        filterOption={() => true}
        getQueryData={getSearchData}
        formatResponse={formatResponse}
        autosize={false}
        {...rest}
        onChange={onChange}
      />
    </div>
  )
}

const formSchemas = {
  [formTypesEnum.SERVICEFORM]: EntitySchemas.SERVICE_FORM,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: EntitySchemas.GENERAL_DESCRIPTION
}

const GroupField = compose(
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
    annotationIsFetching: FintoSelectors.annotationIsFetching(state, ownProps)
  })),
  injectIntl)(
  ({ isOpen, updateUI, intl: { formatMessage }, dispatch, formName, annotationData, annotationIsFetching, ...rest }) => {
    const _handleAnnotationClick = () => {
      updateUI('isOpen', true)
    }
    const getAnnotationSuccess = (data) => {
      const result = fromJS(data.response.entities.ontologyTerms)
      if (result) {
        dispatch(change(formName, 'annotationTerms', result.keySeq().toOrderedSet()))
      }
    }
    const _handleGetAnnotationClick = () => {
      dispatch(
        ({ getState }) => {
          const data = FintoSelectors.getAnnotationData(getState(), { formName: formName })
          dispatch(apiCall3({
            keys: ['OntologyTerm', 'searchAnnotations'],
            payload: {
              endpoint: 'common/getAnnotations',
              data: data
            },
            schemas: formSchemas[formName],
            successNextAction: getAnnotationSuccess,
            formName
          }))
        }
      )
    }
    const isProd = getEnvironmentType() === 'prod'
    const treeVisible = isProd || isOpen
    return (
      <div>
        { !isProd
          ? <div>
            <Label labelText={formatMessage(messages.annotationTitle)} />
            <div className='form-row'>
              <Button
                type='button'
                children={annotationIsFetching && <Spinner /> || formatMessage(messages.annotationButton)}
                onClick={_handleGetAnnotationClick}
                medium
                secondary={annotationIsFetching}
            />
            </div>
          </div>
        : null
        }
        <div className='form-row'>
          {!treeVisible && <Button
            children={formatMessage(messages.treeLink)}
            onClick={_handleAnnotationClick}
            medium
            link
          />}
          {treeVisible && <OntologyTermTreeComponent {...rest} />}
        </div>
      </div>
    )
  }
  )

const OntologyTermTreeComponent = compose(
  injectIntl,
  injectFormName,
  connect(null, {
    searchInTree: nodeActions.searchInTree,
    clearTreeSearch: nodeActions.clearTreeSearch
  }),
  SearchFilter.withSearchFilter({
    filterFunc: (props, value) => {
      console.log('search', props, value)
      if (value) {
        console.log('searching', value)
        props.searchInTree({
          searchSchema: FintoSchemas.ONTOLOGY_TERM_ARRAY,
          treeType: 'OntologyTerm',
          id: value.value,
          contextId: value.value
        })
      } else {
        console.log('clearing', value)
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
      NodesComponent={OntologyTermNodes}
      NodeComponent={OntologyTermNode}
      {...rest}
    />
  )
  )//x

const RenderOntologyTreeView = (fields) => {
  const newOntologyValue = (fields.ontologyTerms.input.value || OrderedSet())
  const newAnnotationValue = (fields.annotationTerms.input.value || OrderedSet())
  return (
    <div>
      <TreeView
        label={fields.label}
        onChange={(id, checked) => {
          fields.ontologyTerms.input.onChange(checked ? newOntologyValue.add(id) : newOntologyValue.delete(id))
          if (!checked) {
            fields.annotationTerms.input.onChange(newAnnotationValue.delete(id))
          }
        }}
        value={newOntologyValue.union(newAnnotationValue)}
        className='ontologyTerm'
        {...fields}
      />
    </div>
  )
}

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
    showAnnotationList: FintoSelectors.isAnyAnnotationTerm(state, ownProps)
  }))
)(({
  intl: { formatMessage },
  isReadOnly,
  showAnnotationList,
  isCompareMode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
  return (
    <div className={basicCompareModeClass}>
      <TreeListDisplay
        name='ontologyTerms'
        label={!isReadOnly && formatMessage(messages.targetListHeader) || formatMessage(CommonMessages.ontologyTerms)}
        selector={FintoSelectors.getOntologyTermsForIdsJs}
        {...rest}
      />
      {showAnnotationList && <TreeListDisplay
        name='annotationTerms'
        label={formatMessage(messages.annotationListHeader)}
        selector={FintoSelectors.getAnnotationTermsForIdsJs}
        {...rest}
      />}
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
