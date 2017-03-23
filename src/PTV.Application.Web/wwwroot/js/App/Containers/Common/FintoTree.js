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
import React, { PropTypes, Component } from 'react'
import { Map, List } from 'immutable'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'

import PTVTreeViewSelect from '../../Components/PTVTreeViewComponent/PTVTreeViewSelect'
// import * as serviceStep2Actions from '../../Actions/ServiceStep2Actions';
// import * as mainActions from '../../Actions';
import * as nodeActions from './Actions/Nodes'
import mapDispatchToProps from '../../Configuration/MapDispatchToProps'
// import { PTVLabel, PTVCheckBox, PTVTagInput } from '../../Components';
import { LocalizedAsyncComboBox } from '../Common/localizedData'
// import TreeNode from '../../../../Common/TreeNode';
// import * as CommonServiceSelectors from '../../../Common/Selectors';
import * as CommonServiceSelectors from '../Services/Common/Selectors'
import * as CommonServiceAndChannelsSelectors from '../Relations/ServiceAndChannels/Common/Selectors'
import * as CommonSelectors from './Selectors'
import * as IntlSelectors from '../../Intl/Selectors'
import * as CommonOrganizationSelectors from '../Manage/Organizations/Common/Selectors'
import { CommonSchemas } from '../Common/Schemas'
import { OrganizationSchemas } from '../Manage/Organizations/Organization/Schemas'

const messages = defineMessages({
  notFound: {
    id: 'TreeItem.TranlationMissing',
    defaultMessage: '{name} (Not available in Finnish)'
  }
})

export const getDefaultTranslationText = intl => (id, name) => intl.formatMessage(messages.notFound, { name })

const types = {
  ServiceClass: 'ServiceClass',
  OntologyTerm: 'OntologyTerm',
  LifeEvent: 'LifeEvent',
  IndustrialClass: 'IndustrialClass',
  Organization: 'Organization',
  DigitalAuthorization: 'DigitalAuthorization'
}

const searchSchemas = {
  ServiceClass: CommonSchemas.FILTERED_SERVICE_CLASS_ARRAY,
  OntologyTerm: CommonSchemas.ONTOLOGY_TERM_ARRAY,
  LifeEvent: CommonSchemas.FILTERED_LIFE_EVENTS_ARRAY,
	// IndustrialClass: CommonSchemas.FILTERED_INDUSTRIAL_CLASS_ARRAY,
  IndustrialClass: CommonSchemas.INDUSTRIAL_CLASS_ARRAY,
  Organization: OrganizationSchemas.FILTERED_ORGANIZATION_ARRAY,
  DigitalAuthorization: CommonSchemas.FILTERED_DIGITAL_AUTHORIZATION_ARRAY
}

const nodeSchemas = {
  ServiceClass: CommonSchemas.SERVICE_CLASS,
  OntologyTerm: CommonSchemas.ONTOLOGY_TERM,
  LifeEvent: CommonSchemas.LIFE_EVENT,
  IndustrialClass: CommonSchemas.INDUSTRIAL_CLASS,
  Organization: OrganizationSchemas.ORGANIZATION_GLOBAL,
  DigitalAuthorization: CommonSchemas.DIGITAL_AUTHORIZATION
}

const dataSelectors = {
  ServiceClass: {
      getNode: CommonServiceSelectors.getServiceClass,
      getFilteredNode: CommonServiceSelectors.getFilteredServiceClass,
      getListNode: CommonServiceSelectors.getServiceClassWithFiltered,
      getTopIds: CommonServiceSelectors.getTopServiceClasses
    },
  LifeEvent: {
      getNode: CommonServiceSelectors.getLifeEvent,
      getListNode: CommonServiceSelectors.getLifeEventWithFiltered,
      getFilteredNode: CommonServiceSelectors.getFilteredLifeEvent,
      getTopIds: CommonServiceSelectors.getTopLifeEvents
    },
  OntologyTerm: {
      getNode: CommonServiceSelectors.getOntologyTerm,
      getFilteredNode: CommonServiceSelectors.getOntologyTerm,
      getTopIds: CommonServiceSelectors.getTopOntologyTerms,
      renderCustomSearch: true
    },
  IndustrialClass: {
      getNode: CommonServiceSelectors.getIndustrialClass,
      getFilteredNode: CommonServiceSelectors.getIndustrialClass,
        // getChildren: CommonServiceSelectors.getFilteredIndustrialClasses,
        // getChildren: CommonServiceSelectors.getIndustrialClasses,
      getTopIds: CommonServiceSelectors.getTopIndustrialClasses
    },
  Organization: {
      getNode: CommonOrganizationSelectors.getOrganizationForId,
      getListNode: CommonOrganizationSelectors.getOrganizationWithFiltered,
      getFilteredNode: CommonOrganizationSelectors.getFilteredOrganization,
      getTopIds: CommonOrganizationSelectors.getTopOrganizations
    },
  DigitalAuthorization: {
      getNode: CommonServiceAndChannelsSelectors.getDigitalAuthorization,
      getFilteredNode: CommonServiceAndChannelsSelectors.getFilteredDigitalAuthorization,
      getListNode: CommonServiceAndChannelsSelectors.getDigitalAuthorizationWithFiltered,
      getTopIds: CommonServiceAndChannelsSelectors.getTopDigitalAuthorizations
    }
}
// console.log('data selector', dataSelectors)

const CommonTree = (treeType, showMissingTranslation) => (props) => {
  const handleSearchInTree = (value) => {
      props.actions.searchInTree({ searchSchema: searchSchemas[treeType], treeType, value, contextId: props.contextId })
    }

  const handleNodeToggle = (node) => {
      if (!node.get('areChildrenLoaded'))        {
          props.actions.loadNodeChildren({ treeNodeSchema: nodeSchemas[treeType], treeType, node, contextId: props.contextId })
          return
        }

      props.actions.updateNode(props.contextId ? [props.contextId, node.get('id')] : node.get('id'), 'isCollapsed', node.get('isCollapsed'))
    }

  const onClearSearch = () => {
      props.actions.clearTreeSearch(treeType, props.contextId)
    }

  const getSearchData = (input) => {
      return {
          searchValue: input,
          treeType: 'OntologyTerm',
          resultType: 'List',
          language: props.selectedLanguage
        }
    }

    // const ontologyTermOptions = (x) => {
    //         return {
    //                         id: x.id,
    //                         name: x.name,
    //                         code: x.name,
    //                     };
    // }

  const onOntologyTermSearch = (id, value) => {
      if (value) {
          props.actions.searchInTree({ searchSchema: searchSchemas[treeType], treeType, value: value.name, id: value.id, contextId: props.contextId })
        }        else {
          props.actions.clearTreeSearch(treeType, props.contextId)
        }
    }

  const renderCustomSearch = () => (

      <LocalizedAsyncComboBox
              componentClass='list-search-field'
              name='OntologyTerm'
              value ={props.searchItem}
              endpoint ='service/GetFilteredList'
              language={props.language}
              getCallData= {getSearchData}
              onChange ={onOntologyTermSearch}
              readOnly={props.readOnly}
                        />

    )

  const onOntologyTermNodeSelect = (node) => {
      if (node) {
          props.actions.searchInTree({ searchSchema: searchSchemas[treeType], treeType, value: node.get('name'), id: node.get('id'), contextId: props.contextId })
        }
    }

    	const { getNode, getListNode, tree, readOnly, validators, order, isSearching, language } = props
    	// const { formatMessage } = props.intl;
  const renderCustomSearchFunc = dataSelectors[treeType].renderCustomSearch ? renderCustomSearch : null
  const onNodeClickFunc = dataSelectors[treeType].renderCustomSearch ? onOntologyTermNodeSelect : null
        // console.log('finto', renderCustomSearch)
  return (
          <PTVTreeViewSelect
            getDefaultTranslationText = { showMissingTranslation ? getDefaultTranslationText(props.intl) : null}
                   treeType= {treeType}
                   treeViewClass= {props.treeViewClass}
                   treeViewInnerClass ={props.treeViewInnerClass}
                   resultsClass= 'col-md-6'
                   readOnlyClass ='col-md-12'
                   labelTooltip ={props.labelTooltip}
                   label ={props.label}
                   onNodeToggle= {handleNodeToggle}
                   onNodeSelect ={props.onNodeSelect}
                   onNodeClick ={onNodeClickFunc}
                   onSearchInTree= {handleSearchInTree}
                   onClearSearch= {onClearSearch}
                   onNodeRemove= {props.onNodeRemove}
                   treeSource ={tree}
                   getSelectedSelector ={props.getSelectedSelector}
                   getReadOnlySelectedSelector ={props.getReadOnlySelectedSelector}
                   getValidationSelector = {props.getValidationSelector}
                   treeTargetHeader={props.treeTargetHeader}
                   isSearching ={isSearching}
                   validators={validators} order={order}
                   readOnly= {readOnly}
                   getNode={getNode}
                   getListNode= {getListNode}
                   getNodeIsCollapsed={CommonSelectors.getNodeIsCollapsed}
                   getNodeIsFetching= {CommonSelectors.getNodeIsFetching}
                   isSelectedSelector= {props.isSelectedSelector}
                   isDisabledSelector= {props.isDisabledSelector}
                   isListDisabledSelector ={props.isListDisabledSelector}
                   renderCustomSearch= {renderCustomSearchFunc}
                   language ={language}
                   nodeShowTooltip= {props.nodeShowTooltip}
                   contextId ={props.contextId}
                   children= {props.children}
                   renderTreeLabel= {props.renderTreeLabel}
                   renderTreeButton ={props.renderTreeButton}
                   treeLinkLabel ={props.treeLinkLabel}
                   minHeight= {props.minHeight}
                   tagPostfix= {props.tagPostfix}
                   tagColor ={props.tagColor}
                   tagType ={props.tagType}
                   tagTooltipContent={props.tagTooltipContent}
                   tagTooltipCallback={props.tagTooltipCallback}
                />
        )
}

const mapStateToProps = (treeType) => (state, ownProps) => {
    // console.log('finto selector', treeType, dataSelectors[treeType], dataSelectors);
  const contextId = ownProps.entityId || CommonSelectors.getPageEntityIdNotGenerated(state, ownProps)
  const props = {
      keyToState: contextId ? [treeType, contextId] : treeType,
      language: ownProps.language
    }

  const filteredData = CommonSelectors.getSearchIds(state, props)
  const showFilteredData = filteredData != null

  return {
    searchItem: CommonSelectors.getNodeSearchItemJS(state, props),
    isSearching: CommonSelectors.getSearchIsFetching(state, props),
    getNode: showFilteredData ? dataSelectors[treeType].getFilteredNode : dataSelectors[treeType].getNode,
    getListNode: dataSelectors[treeType].getListNode,
    tree: filteredData || dataSelectors[treeType].getTopIds(state),
    selectedLanguage: IntlSelectors.getSelectedLanguage(state),
    contextId
  }
}

const actions = [
  nodeActions
]

const connectTreeComponent = (treeType, showMissingTranslation = true) => {
  return compose(
    connect(mapStateToProps(treeType), mapDispatchToProps(actions)),
    injectIntl
  )(CommonTree(treeType, showMissingTranslation))
}
// const ConnectedTree = connect(mapStateToProps, mapDispatchToProps(actions))(CommonTree);

export const ServiceClassTree = connectTreeComponent(types.ServiceClass)
export const LifeEventTree = connectTreeComponent(types.LifeEvent)
export const OntologyTermTree = connectTreeComponent(types.OntologyTerm)
export const IndustrialClassTree = connectTreeComponent(types.IndustrialClass)
export const OrganizationTree = connectTreeComponent(types.Organization, false)
export const DigitalAuthorizationTree = connectTreeComponent(types.DigitalAuthorization)

export default {
  ServiceClassTree,
  LifeEventTree,
  OntologyTermTree,
  IndustrialClassTree,
  OrganizationTree,
  DigitalAuthorizationTree
}
