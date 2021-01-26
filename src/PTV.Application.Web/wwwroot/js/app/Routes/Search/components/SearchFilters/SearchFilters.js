/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import SearchFilter, { SearchFilterCategory, SearchFilterList } from '../SearchFilter'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import {
  getContentTypeMessages,
  getAreServiceSubfiltersVisible,
  getAreAllContentTypesSelected,
  getAreNoContentTypesSelected,
  getAreAllOrganizationsSelected,
  getSelectedOrganizations,
  getAreAllLanguagesSelected,
  getSelectedLanguages,
  getAreNoLanguagesSelected,
  getSelectedPublishingStatuses,
  getAreAllPublishingStatusesSelected,
  getAreNoPublishingStatusesSelected
} from './selectors'
import PropTypes from 'prop-types'
import {
  ContentTypeDialog,
  contentTypeDialogName,
  organizationDialogName,
  OrganizationDialog,
  languageDialogName,
  LanguageDialog,
  publishingStatusDialogName,
  PublishingStatusDialog
} from './dialogs'
import ServiceSubfilter from './ServiceSubfilter'
import { messages } from './messages'
import styles from './styles.scss'

const SearchFilters = props => {
  const {
    selectedContentTypes,
    searchContentTypes,
    displayServiceSubfilters,
    noContentTypes,
    allOrganizations,
    selectedOrganizations,
    selectedLanguages,
    allLanguages,
    noLanguages,
    selectedPublishingStatuses,
    allPublishingStatuses,
    noPublishingStatuses
  } = props

  return (
    <div className={styles.searchFilters}>
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              isFirst
              category={messages.contentTypeFilterTitle}
              dialogName={contentTypeDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              isFirst
              items={selectedContentTypes}
              isAllSelected={searchContentTypes}
              filterName='contentTypes'
            />
          </div>
          <ContentTypeDialog
            isAllSelected={searchContentTypes}
            nothingSelected={noContentTypes}
          />
        </div>
      </SearchFilter>
      {displayServiceSubfilters && <ServiceSubfilter />}
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              isFirst
              category={messages.organizationFilterTitle}
              dialogName={organizationDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              isFirst
              items={selectedOrganizations}
              isAllSelected={allOrganizations}
              filterName='organizationIds'
            />
          </div>
        </div>
        <OrganizationDialog isAllSelected={allOrganizations} />
      </SearchFilter>
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              category={messages.languageFilterTitle}
              dialogName={languageDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              items={selectedLanguages}
              isAllSelected={allLanguages}
              filterName='languages'
            />
          </div>
        </div>
        <LanguageDialog
          isAllSelected={allLanguages}
          nothingSelected={noLanguages}
        />
      </SearchFilter>
      <SearchFilter>
        <div className='row'>
          <div className='col-6 col-md-5 col-lg-3'>
            <SearchFilterCategory
              isLast
              category={messages.publishingStatusFilterTitle}
              dialogName={publishingStatusDialogName}
            />
          </div>
          <div className='col-18 col-md-19 col-lg-21'>
            <SearchFilterList
              isLast
              items={selectedPublishingStatuses}
              isAllSelected={allPublishingStatuses}
              filterName='selectedPublishingStatuses'
            />
          </div>
        </div>
        <PublishingStatusDialog
          isAllSelected={allPublishingStatuses}
          nothingSelected={noPublishingStatuses}
        />
      </SearchFilter>
    </div>
  )
}

SearchFilters.propTypes = {
  selectedContentTypes: PropTypes.array,
  searchContentTypes: PropTypes.bool,
  displayServiceSubfilters: PropTypes.bool,
  noContentTypes: PropTypes.bool,
  allOrganizations: PropTypes.bool,
  selectedOrganizations: PropTypes.array,
  selectedLanguages: PropTypes.array,
  allLanguages: PropTypes.bool,
  noLanguages: PropTypes.bool,
  selectedPublishingStatuses: PropTypes.array,
  allPublishingStatuses: PropTypes.bool,
  noPublishingStatuses: PropTypes.bool
}

export default compose(
  injectFormName,
  connect((state, ownProps) => {
    return {
      selectedContentTypes: getContentTypeMessages(state, ownProps),
      displayServiceSubfilters: getAreServiceSubfiltersVisible(state, ownProps),
      searchContentTypes: getAreAllContentTypesSelected(state, ownProps),
      noContentTypes: getAreNoContentTypesSelected(state, ownProps),
      allOrganizations: getAreAllOrganizationsSelected(state, ownProps),
      selectedOrganizations: getSelectedOrganizations(state, ownProps),
      allLanguages: getAreAllLanguagesSelected(state, ownProps),
      selectedLanguages: getSelectedLanguages(state, ownProps),
      noLanguages: getAreNoLanguagesSelected(state, ownProps),
      selectedPublishingStatuses: getSelectedPublishingStatuses(state, ownProps),
      allPublishingStatuses: getAreAllPublishingStatusesSelected(state, ownProps),
      noPublishingStatuses: getAreNoPublishingStatusesSelected(state, ownProps)
    }
  })
)(SearchFilters)
