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
import SearchFilter, { SearchFilterCategory, SearchFilterList } from '../SearchFilter'
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
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              category={messages.serviceClassFilterTitle}
              dialogName={serviceClassDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              items={serviceClassesSelected}
              isAllSelected={serviceClassesAreAllSelected}
            />
          </div>
        </div>
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
          searchPlaceholder={messages.serviceClassFilterSearchPlaceholder}
          TreeComponent={ServiceClassSearchTree}
        />
      </SearchFilter>
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              category={messages.targetGroupFitlerTitle}
              dialogName={targetGroupDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              items={targetGroupsSelected}
              isAllSelected={targetGroupsAreAllSelected}
            />
          </div>
        </div>
        <SubfilterDialog
          hasAllOption
          isAllSelected={targetGroupsAreAllSelected}
          storeValueName='targetGroups'
          dialogName={targetGroupDialogName}
          dialogTitleMessage={messages.targetGroupFitlerTitle}
          TreeComponent={TargetGroupSearchTree}
        />
      </SearchFilter>
      {isKR1Selected && <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              category={messages.lifeEventFilterTitle}
              dialogName={lifeEventDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              items={lifeEventsSelected}
              isAllSelected={lifeEventsAreAllSelected}
            />
          </div>
        </div>
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
          searchPlaceholder={messages.lifeEventFilterSearchPlaceholder}
          TreeComponent={LifeEventSearchTree}
        />
      </SearchFilter>}
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              category={messages.ontologyTermFilterTitle}
              dialogName={keywordDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              items={ontologyTermsSelected}
              isAllSelected={ontologyTermsAreAllSelected}
            />
          </div>
        </div>
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
          searchPlaceholder={messages.ontologyTermFilterSearchPlaceholder}
          TreeComponent={OntologyTermSearchTree}
          searchAction={searchInList}
        />
      </SearchFilter>
      {isKR2Selected && <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              category={messages.industrialClassFilterTitle}
              dialogName={industrialClassDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              items={industrialClassesSelected}
              isAllSelected={industrialClassesAreAllSelected}
            />
          </div>
        </div>
        <IndustrialClassDialog isAllSelected={industrialClassesAreAllSelected} />
      </SearchFilter>}
      {isOnlyGDs &&
        <SearchFilter>
          <div className='row'>
            <div className='col-6 col-md-5 col-lg-3'>
              <SearchFilterCategory
                category={messages.serviceTypeFilterTitle}
                dialogName={serviceTypeDialogName}
              />
            </div>
            <div className='col-18 col-md-19 col-lg-21'>
              <SearchFilterList
                items={serviceTypesSelected}
                isAllSelected={serviceTypesAreAllSelected}
              />
            </div>
          </div>
          <SubfilterDialog
            hasAllOption
            isAllSelected={serviceTypesAreAllSelected}
            storeValueName='serviceTypes'
            dialogName={serviceTypeDialogName}
            dialogTitleMessage={messages.serviceTypeFilterTitle}
            TreeComponent={ServiceTypeSearchTree}
          />
        </SearchFilter>
      }
      {isOnlyGDs &&
        <SearchFilter>
          <div className='row'>
            <div className='col-6 col-md-5 col-lg-3'>
              <SearchFilterCategory
                category={messages.generalDescriptionFilterTitle}
                dialogName={gdTypeDialogName}
              />
            </div>
            <div className='col-18 col-md-19 col-lg-21'>
              <SearchFilterList
                items={generalDescriptionTypesSelected}
                isAllSelected={generalDescriptionTypesAreAllSelected}
              />
            </div>
          </div>
          <SubfilterDialog
            hasAllOption
            isAllSelected={generalDescriptionTypesAreAllSelected}
            storeValueName='generalDescriptionTypes'
            dialogName={gdTypeDialogName}
            dialogTitleMessage={messages.generalDescriptionFilterTitle}
            TreeComponent={GeneralDescriptionTypeSearchTree}
          />
        </SearchFilter>
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
