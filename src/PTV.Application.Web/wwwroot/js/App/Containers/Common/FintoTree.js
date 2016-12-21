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
import React, {PropTypes, Component} from 'react';
import {Map, List} from 'immutable';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';

import PTVTreeViewSelect from '../../Components/PTVTreeViewComponent/PTVTreeViewSelect';
// import * as serviceStep2Actions from '../../Actions/ServiceStep2Actions';
// import * as mainActions from '../../Actions';
import * as nodeActions from './Actions/Nodes';
import mapDispatchToProps from '../../Configuration/MapDispatchToProps';
import { callApiDirect } from '../../Middleware/Api';
import { PTVLabel, PTVCheckBox, PTVTagInput, PTVAutoComboBox } from '../../Components';
// import TreeNode from '../../../../Common/TreeNode';
// import * as CommonServiceSelectors from '../../../Common/Selectors';
import * as CommonServiceSelectors from '../Services/Common/Selectors';
import * as CommonSelectors from './Selectors';
import * as CommonOrganizationSelectors from '../Manage/Organizations/Common/Selectors';
import { CommonSchemas } from '../Common/Schemas';
import { OrganizationSchemas } from '../Manage/Organizations/Organization/Schemas';

const types = {
    ServiceClass: "ServiceClass",
    OntologyTerm: "OntologyTerm",
    LifeEvent: "LifeEvent",
    IndustrialClass: "IndustrialClass",
    Organization: "Organization"
}

const searchSchemas = {
	ServiceClass: CommonSchemas.FILTERED_SERVICE_CLASS_ARRAY,
	OntologyTerm: CommonSchemas.ONTOLOGY_TERM_ARRAY,
	LifeEvent: CommonSchemas.FILTERED_LIFE_EVENTS_ARRAY,
	// IndustrialClass: CommonSchemas.FILTERED_INDUSTRIAL_CLASS_ARRAY,
	IndustrialClass: CommonSchemas.INDUSTRIAL_CLASS_ARRAY,
    Organization: OrganizationSchemas.FILTERED_ORGANIZATION_ARRAY
}

const nodeSchemas = {
	ServiceClass: CommonSchemas.SERVICE_CLASS,
	OntologyTerm: CommonSchemas.ONTOLOGY_TERM,
	LifeEvent: CommonSchemas.LIFE_EVENT,
	IndustrialClass: CommonSchemas.INDUSTRIAL_CLASS,
    Organization: OrganizationSchemas.ORGANIZATION,
}

const dataSelectors = {
    ServiceClass: {
        getNode: CommonServiceSelectors.getServiceClass,
        getFilteredNode: CommonServiceSelectors.getFilteredServiceClass,
        getListNode: CommonServiceSelectors.getServiceClassWithFiltered,
        getTopIds: CommonServiceSelectors.getTopServiceClasses,
    },
    LifeEvent: {
        getNode: CommonServiceSelectors.getLifeEvent,
        getListNode: CommonServiceSelectors.getLifeEventWithFiltered,
        getFilteredNode: CommonServiceSelectors.getFilteredLifeEvent,
        getTopIds: CommonServiceSelectors.getTopLifeEvents,
    },
    OntologyTerm: {
        getNode: CommonServiceSelectors.getOntologyTerm,
        getFilteredNode: CommonServiceSelectors.getOntologyTerm,
        getTopIds: CommonServiceSelectors.getTopOntologyTerms,
        renderCustomSearch: true, 
    },
    IndustrialClass: {
        getNode: CommonServiceSelectors.getIndustrialClass,
        getFilteredNode: CommonServiceSelectors.getIndustrialClass,
        // getChildren: CommonServiceSelectors.getFilteredIndustrialClasses,
        //getChildren: CommonServiceSelectors.getIndustrialClasses,
        getTopIds: CommonServiceSelectors.getTopIndustrialClasses,
    },
    Organization: {
        getNode: CommonOrganizationSelectors.getOrganizationForId,
        getListNode: CommonOrganizationSelectors.getOrganizationWithFiltered,
        getFilteredNode: CommonOrganizationSelectors.getFilteredOrganization,
        getTopIds: CommonOrganizationSelectors.getTopOrganizations,
    }

}
// console.log('data selector', dataSelectors)

const CommonTree = (treeType) => (props) => {


    const handleSearchInTree = (value) => {
        props.actions.searchInTree(searchSchemas[treeType], treeType, value);
    }

    const handleNodeToggle = (node) =>{
        if (!node.get('areChildrenLoaded'))
        {
            props.actions.loadNodeChildren(nodeSchemas[treeType], treeType, node);
            return;
        }

        props.actions.updateNode(node.get('id'), 'isCollapsed', node.get('isCollapsed'));
    }    

    const onClearSearch = () => {
        props.actions.clearSearch(treeType);
    }

    const renderCustomSearch = () => (
        
            <PTVAutoComboBox
                            componentClass="list-search-field"
                            async={true}
                            name='OntologyTerm'
                            value = { props.searchItem }
                            values= { ontologyTermOptions }
                            changeCallback= { onOntologyTermSearch }
                            readOnly={ props.readOnly }
                        />
        
    )

    const onOntologyTermSearch = (id, value) =>  {
        if (value){      
            props.actions.searchInTree(searchSchemas[treeType], treeType, value.name, value.id);
        }
        else {
            props.actions.clearSearch(treeType);
        }
    }

    const onOntologyTermNodeSelect = (node) =>  {
        if (node){      
            props.actions.searchInTree(searchSchemas[treeType], treeType, node.get('name'), node.get('id'));
        }
    }

    const ontologyTermOptions = (input, callBack) => {
            if (input == "" || input.length < 3){
                callBack(null, { options: []})
                return;
            }
            const data = { 
                searchValue: input, 
                treeType: 'OntologyTerm',
                resultType: 'List'
            };
            const call = callApiDirect('service/GetFilteredList', data)
                .then((json) => {

                return { options: json.model.map(x => {
                        return {
                            id: x.id,
                            name: x.name,
                            code: x.name,
                        }
                    }),
                    complete: true};
            });
            return call;
    }

    
    	const { showFilteredData, tree, readOnly, validators, order, isSearching, language } = props;
    	// const { formatMessage } = props.intl;
        const renderCustomSearchFunc = dataSelectors[treeType].renderCustomSearch ? renderCustomSearch : null;
        const onNodeClickFunc = dataSelectors[treeType].renderCustomSearch ? onOntologyTermNodeSelect : null;
        // console.log('finto', renderCustomSearch)
        return (
                 <PTVTreeViewSelect
                    treeViewClass = 'col-md-6'
                    resultsClass = 'col-md-6'
                    readOnlyClass = 'col-md-12'
                    labelTooltip = { props.labelTooltip }
                    label = { props.label }
                    onNodeToggle = { handleNodeToggle }
                    onNodeSelect = { props.onNodeSelect }
                    onNodeClick = { onNodeClickFunc }
                    onSearchInTree = { handleSearchInTree }
                    onClearSearch = { onClearSearch }
                    onNodeRemove = { props.onNodeRemove }
	                treeSource = { tree }
                    getSelectedSelector = { props.getSelectedSelector }
                    treeTargetHeader={ props.treeTargetHeader }
                    isSearching = { isSearching }
                    validators={ validators } order={ order }
                    readOnly = { readOnly }
                    getNode={ showFilteredData ? dataSelectors[treeType].getFilteredNode :  dataSelectors[treeType].getNode }
                    getListNode = { dataSelectors[treeType].getListNode }
                    getNodeIsCollapsed= { CommonSelectors.getNodeIsCollapsed }
                    getNodeIsFetching = { CommonSelectors.getNodeIsFetching }
                    isSelectedSelector = { props.isSelectedSelector }
                    isDisabledSelector = { props.isDisabledSelector }
                    renderCustomSearch = { renderCustomSearchFunc }
                    language = { language }
                 />
       );
}


const mapStateToProps = (treeType) => (state, ownProps) => {
    // console.log('finto selector', treeType, dataSelectors[treeType], dataSelectors);
    const props = { keyToState: treeType, language: ownProps.language };

    const filteredData = CommonSelectors.getSearchIds(state, props);
    
  return {
    searchItem: CommonSelectors.getNodeSearchItemJS(state, props),
    isSearching: CommonSelectors.getSearchIsFetching(state, props),
    showFilteredData: filteredData != null,
    tree: filteredData ? filteredData : dataSelectors[treeType].getTopIds(state),
  }
}

const actions = [
    nodeActions
];

const connectTreeComponent = treeType => {
    return connect(mapStateToProps(treeType), mapDispatchToProps(actions))(CommonTree(treeType));
}
// const ConnectedTree = connect(mapStateToProps, mapDispatchToProps(actions))(CommonTree);


export const ServiceClassTree = connectTreeComponent(types.ServiceClass); 
export const LifeEventTree = connectTreeComponent(types.LifeEvent); 
export const OntologyTermTree = connectTreeComponent(types.OntologyTerm); 
export const IndustrialClassTree = connectTreeComponent(types.IndustrialClass); 
export const OrganizationTree = connectTreeComponent(types.Organization); 

export default {
    ServiceClassTree,
    LifeEventTree,
    OntologyTermTree,
    IndustrialClassTree,
    OrganizationTree
}
