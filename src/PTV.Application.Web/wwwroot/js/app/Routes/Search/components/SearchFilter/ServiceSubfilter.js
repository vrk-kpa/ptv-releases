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
import React, { Fragment } from 'react'
import SearchFilterList from '../SearchFilterList'
import SubfilterDialog, {
  industrialClassDialogName,
  IndustrialClassDialog
} from './dialogs'
import { FintoSchemas } from 'schemas'
import {
  ServiceClassSearchTree,
  TargetGroupSearchTree,
  LifeEventSearchTree,
  OntologyTermSearchTree,
  ServiceTypeSearchTree,
  GeneralDescriptionTypeSearchTree
} from 'util/redux-form/fields'
import { messages } from './messages'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import {
  getIsOnlyGDSelected,
  getAreAllOptionsSelected,
  getSelectedOptions
} from './selectors'
import { EntitySelectors } from 'selectors'
import { getIsKR1Selected, getIsKR2Selected } from 'Routes/Search/selectors'
import { searchInList } from 'actions/nodes'

const serviceClassDialogName = 'serviceClassDialog'
const targetGroupDialogName = 'targetGroupDialog'
const lifeEventDialogName = 'lifeEventDialog'
const keywordDialogName = 'keywordDialog'
const serviceTypeDialogName = 'serviceTypeDialog'
const gdTypeDialogName = 'gdTypeDialog'

const ServiceSubfilter = props => {
  const {
    isOnlyGDs,
    serviceClassesSelected,
    serviceClassesAreAllSelected,
    targetGroupsSelected,
    targetGroupsAreAllSelected,
    lifeEventsSelected,
    lifeEventsAreAllSelected,
    ontologyTermsSelected,
    ontologyTermsAreAllSelected,
    industrialClassesSelected,
    industrialClassesAreAllSelected,
    serviceTypesSelected,
    serviceTypesAreAllSelected,
    generalDescriptionTypesSelected,
    generalDescriptionTypesAreAllSelected,
    isKR1Selected,
    isKR2Selected
  } = props

  return (
    <Fragment>
      <SearchFilterList
        category={messages.serviceClassFilterTitle}
        items={serviceClassesSelected}
        isAllSelected={serviceClassesAreAllSelected}
        dialogName={serviceClassDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={serviceClassesAreAllSelected}
          storeValueName='serviceClasses'
          hasSearch
          searchFieldName='serviceClassSearch'
          treeTypeName='ServiceClass'
          searchSchema={FintoSchemas.SERVICE_CLASS_ARRAY}
          dialogName={serviceClassDialogName}
          dialogTitleMessage={messages.serviceClassFilterTitle}
          TreeComponent={ServiceClassSearchTree}
        />
      </SearchFilterList>
      <SearchFilterList
        category={messages.targetGroupFitlerTitle}
        items={targetGroupsSelected}
        isAllSelected={targetGroupsAreAllSelected}
        dialogName={targetGroupDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={targetGroupsAreAllSelected}
          storeValueName='targetGroups'
          dialogName={targetGroupDialogName}
          dialogTitleMessage={messages.targetGroupFitlerTitle}
          TreeComponent={TargetGroupSearchTree}
        />
      </SearchFilterList>
      {isKR1Selected && <SearchFilterList
        category={messages.lifeEventFilterTitle}
        items={lifeEventsSelected}
        isAllSelected={lifeEventsAreAllSelected}
        dialogName={lifeEventDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={lifeEventsAreAllSelected}
          storeValueName='lifeEvents'
          hasSearch
          searchFieldName='lifeEventSearch'
          treeTypeName='LifeEvent'
          searchSchema={FintoSchemas.LIFE_EVENT_ARRAY}
          dialogName={lifeEventDialogName}
          dialogTitleMessage={messages.lifeEventFilterTitle}
          TreeComponent={LifeEventSearchTree}
        />
      </SearchFilterList>}
      <SearchFilterList
        category={messages.ontologyTermFilterTitle}
        items={ontologyTermsSelected}
        isAllSelected={ontologyTermsAreAllSelected}
        dialogName={keywordDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={ontologyTermsAreAllSelected}
          storeValueName='ontologyTerms'
          hasSearch
          searchFieldName='ontologyTermSearch'
          treeTypeName='OntologyTerm'
          searchSchema={FintoSchemas.ONTOLOGY_TERM_ARRAY}
          dialogName={keywordDialogName}
          dialogTitleMessage={messages.ontologyTermFilterTitle}
          TreeComponent={OntologyTermSearchTree}
          searchAction={searchInList}
        />
      </SearchFilterList>
      {isKR2Selected && <SearchFilterList
        category={messages.industrialClassFilterTitle}
        items={industrialClassesSelected}
        isAllSelected={industrialClassesAreAllSelected}
        dialogName={industrialClassDialogName}
      >
        <IndustrialClassDialog isAllSelected={industrialClassesAreAllSelected} />
      </SearchFilterList>}
      {isOnlyGDs &&
        <SearchFilterList
          category={messages.serviceTypeFilterTitle}
          items={serviceTypesSelected}
          isAllSelected={serviceTypesAreAllSelected}
          dialogName={serviceTypeDialogName}
        >
          <SubfilterDialog
            hasAllOption
            isAllSelected={serviceTypesAreAllSelected}
            storeValueName='serviceTypes'
            dialogName={serviceTypeDialogName}
            dialogTitleMessage={messages.serviceTypeFilterTitle}
            TreeComponent={ServiceTypeSearchTree}
          />
        </SearchFilterList>
      }
      {isOnlyGDs &&
        <SearchFilterList
          category={messages.generalDescriptionFilterTitle}
          items={generalDescriptionTypesSelected}
          isAllSelected={generalDescriptionTypesAreAllSelected}
          dialogName={gdTypeDialogName}
        >
          <SubfilterDialog
            hasAllOption
            isAllSelected={generalDescriptionTypesAreAllSelected}
            storeValueName='generalDescriptionTypes'
            dialogName={gdTypeDialogName}
            dialogTitleMessage={messages.generalDescriptionFilterTitle}
            TreeComponent={GeneralDescriptionTypeSearchTree}
          />
        </SearchFilterList>
      }
    </Fragment>
  )
}

ServiceSubfilter.propTypes = {
  isOnlyGDs: PropTypes.bool,
  serviceClassesSelected: PropTypes.array,
  serviceClassesAreAllSelected: PropTypes.bool,
  targetGroupsSelected: PropTypes.array,
  targetGroupsAreAllSelected: PropTypes.bool,
  lifeEventsSelected: PropTypes.array,
  lifeEventsAreAllSelected: PropTypes.bool,
  ontologyTermsSelected: PropTypes.array,
  ontologyTermsAreAllSelected: PropTypes.bool,
  industrialClassesSelected: PropTypes.array,
  industrialClassesAreAllSelected: PropTypes.bool,
  serviceTypesSelected: PropTypes.array,
  serviceTypesAreAllSelected: PropTypes.bool,
  generalDescriptionTypesSelected: PropTypes.array,
  generalDescriptionTypesAreAllSelected: PropTypes.bool,
  isKR1Selected: PropTypes.bool.isRequired,
  isKR2Selected: PropTypes.bool.isRequired
}

const dataTypes = [
  'serviceClasses',
  'targetGroups',
  'lifeEvents',
  'ontologyTerms',
  'industrialClasses',
  'serviceTypes',
  'generalDescriptionTypes'
]

const createSelectors = type => {
  return {
    getSelectedOptions: getSelectedOptions(type, EntitySelectors[type].getEntities),
    getAreAllOptionsSelected: getAreAllOptionsSelected(type)
  }
}

const selectors = dataTypes.reduce(
  (all, type) => {
    all[type] = createSelectors(type)
    return all
  }, {}
)

export default compose(
  injectFormName,
  connect((state, ownProps) => {
    const selectionOptions = Object.keys(selectors).reduce(
      (data, type) => {
        data[`${type}Selected`] = selectors[type].getSelectedOptions(state, ownProps)
        data[`${type}AreAllSelected`] = selectors[type].getAreAllOptionsSelected(state, ownProps)
        return data
      },
      {}
    )

    return {
      isOnlyGDs: getIsOnlyGDSelected(state, ownProps),
      ...selectionOptions,
      isKR1Selected: getIsKR1Selected(state, ownProps),
      isKR2Selected: getIsKR2Selected(state, ownProps)
    }
  })
)(ServiceSubfilter)
