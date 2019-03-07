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

const serviceClassDialogName = 'serviceClassDialog'
const targetGroupDialogName = 'targetGroupDialog'
const lifeEventDialogName = 'lifeEventDialog'
const keywordDialogName = 'keywordDialog'
const serviceTypeDialogName = 'serviceTypeDialog'
const gdTypeDialogName = 'gdTypeDialog'

const ServiceSubfilter = props => {
  const {
    isOnlyGDs,
    selectedServiceClasses,
    allServiceClasses,
    selectedTargetGroups,
    allTargetGroups,
    selectedLifeEvents,
    allLifeEvents,
    selectedKeywords,
    allKeywords,
    selectedGDTypes,
    allGDTypes,
    selectedIndustrialClasses,
    allIndustrialClasses,
    selectedServiceTypes,
    allServiceTypes,
    isKR1Selected,
    isKR2Selected
  } = props

  return (
    <Fragment>
      <SearchFilterList
        category={messages.serviceClassFilterTitle}
        items={selectedServiceClasses}
        isAllSelected={allServiceClasses}
        dialogName={serviceClassDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={allServiceClasses}
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
        items={selectedTargetGroups}
        isAllSelected={allTargetGroups}
        dialogName={targetGroupDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={allTargetGroups}
          storeValueName='targetGroups'
          dialogName={targetGroupDialogName}
          dialogTitleMessage={messages.targetGroupFitlerTitle}
          TreeComponent={TargetGroupSearchTree}
        />
      </SearchFilterList>
      {isKR1Selected && <SearchFilterList
        category={messages.lifeEventFilterTitle}
        items={selectedLifeEvents}
        isAllSelected={allLifeEvents}
        dialogName={lifeEventDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={allLifeEvents}
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
        category={messages.keywordFilterTitle}
        items={selectedKeywords}
        isAllSelected={allKeywords}
        dialogName={keywordDialogName}
      >
        <SubfilterDialog
          hasAllOption
          isAllSelected={allKeywords}
          storeValueName='ontologyTerms'
          hasSearch
          searchFieldName='ontologyTermSearch'
          treeTypeName='OntologyTerm'
          searchSchema={FintoSchemas.ONTOLOGY_TERM_ARRAY}
          dialogName={keywordDialogName}
          dialogTitleMessage={messages.keywordFilterTitle}
          TreeComponent={OntologyTermSearchTree}
        />
      </SearchFilterList>
      {isKR2Selected && <SearchFilterList
        category={messages.industrialClassFilterTitle}
        items={selectedIndustrialClasses}
        isAllSelected={allIndustrialClasses}
        dialogName={industrialClassDialogName}
      >
        <IndustrialClassDialog isAllSelected={allIndustrialClasses} />
      </SearchFilterList>}
      {isOnlyGDs &&
        <SearchFilterList
          category={messages.serviceTypeFilterTitle}
          items={selectedServiceTypes}
          isAllSelected={allServiceTypes}
          dialogName={serviceTypeDialogName}
        >
          <SubfilterDialog
            hasAllOption
            isAllSelected={allServiceTypes}
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
          items={selectedGDTypes}
          isAllSelected={allGDTypes}
          dialogName={gdTypeDialogName}
        >
          <SubfilterDialog
            hasAllOption
            isAllSelected={allGDTypes}
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
  selectedServiceClasses: PropTypes.array,
  allServiceClasses: PropTypes.bool,
  selectedTargetGroups: PropTypes.array,
  allTargetGroups: PropTypes.bool,
  selectedLifeEvents: PropTypes.array,
  allLifeEvents: PropTypes.bool,
  selectedKeywords: PropTypes.array,
  allKeywords: PropTypes.bool,
  selectedGDTypes: PropTypes.array,
  selectedIndustrialClasses: PropTypes.array,
  selectedServiceTypes: PropTypes.array,
  allGDTypes: PropTypes.bool,
  allIndustrialClasses: PropTypes.bool,
  allServiceTypes: PropTypes.bool,
  isKR1Selected: PropTypes.bool.isRequired,
  isKR2Selected: PropTypes.bool.isRequired
}

export default compose(
  injectFormName,
  connect((state, ownProps) => {
    return {
      isOnlyGDs: getIsOnlyGDSelected(state, ownProps),
      selectedServiceClasses: getSelectedOptions('serviceClasses', EntitySelectors.serviceClasses.getEntities)(state, ownProps),
      allServiceClasses: getAreAllOptionsSelected('serviceClasses')(state, ownProps),
      selectedTargetGroups: getSelectedOptions('targetGroups', EntitySelectors.targetGroups.getEntities)(state, ownProps),
      allTargetGroups: getAreAllOptionsSelected('targetGroups')(state, ownProps),
      selectedLifeEvents: getSelectedOptions('lifeEvents', EntitySelectors.lifeEvents.getEntities)(state, ownProps),
      allLifeEvents: getAreAllOptionsSelected('lifeEvents')(state, ownProps),
      selectedKeywords: getSelectedOptions('ontologyTerms', EntitySelectors.ontologyTerms.getEntities)(state, ownProps),
      allKeywords: getAreAllOptionsSelected('ontologyTerms')(state, ownProps),
      selectedIndustrialClasses: getSelectedOptions('industrialClasses', EntitySelectors.industrialClasses.getEntities)(state, ownProps),
      allIndustrialClasses: getAreAllOptionsSelected('industrialClasses')(state, ownProps),
      selectedServiceTypes: getSelectedOptions('serviceTypes', EntitySelectors.serviceTypes.getEntities)(state, ownProps),
      allServiceTypes: getAreAllOptionsSelected('serviceTypes')(state, ownProps),
      selectedGDTypes: getSelectedOptions('generalDescriptionTypes', EntitySelectors.generalDescriptionTypes.getEntities)(state, ownProps),
      allGDTypes: getAreAllOptionsSelected('generalDescriptionTypes')(state, ownProps),
      isKR1Selected: getIsKR1Selected(state, ownProps),
      isKR2Selected: getIsKR2Selected(state, ownProps)
    }
  })
)(ServiceSubfilter)
