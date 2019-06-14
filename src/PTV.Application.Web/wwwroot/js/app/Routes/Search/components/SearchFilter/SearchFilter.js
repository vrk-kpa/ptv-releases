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
import SearchFilterList from '../SearchFilterList'
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

const SearchFilter = props => {
  const {
    selectedContentTypes,
    allContentTypes,
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
    <div className={styles.searchFilter}>
      <SearchFilterList
        category={messages.contentTypeFilterTitle}
        items={selectedContentTypes}
        isAllSelected={allContentTypes}
        dialogName={contentTypeDialogName}
        nothingSelected={noContentTypes}
        isFirst
      >
        <ContentTypeDialog
          isAllSelected={allContentTypes}
          nothingSelected={noContentTypes}
        />
      </SearchFilterList>
      {displayServiceSubfilters && <ServiceSubfilter />}
      <SearchFilterList
        category={messages.organizationFilterTitle}
        items={selectedOrganizations}
        isAllSelected={allOrganizations}
        dialogName={organizationDialogName}
      >
        <OrganizationDialog isAllSelected={allOrganizations} />
      </SearchFilterList>
      <SearchFilterList
        category={messages.languageFilterTitle}
        items={selectedLanguages}
        isAllSelected={allLanguages}
        nothingSelected={noLanguages}
        dialogName={languageDialogName}
      >
        <LanguageDialog
          isAllSelected={allLanguages}
          nothingSelected={noLanguages}
        />
      </SearchFilterList>
      <SearchFilterList
        category={messages.publishingStatusFilterTitle}
        items={selectedPublishingStatuses}
        isAllSelected={allPublishingStatuses}
        nothingSelected={noPublishingStatuses}
        dialogName={publishingStatusDialogName}
      >
        <PublishingStatusDialog
          isAllSelected={allPublishingStatuses}
          nothingSelected={noPublishingStatuses}
        />
      </SearchFilterList>
    </div>
  )
}

SearchFilter.propTypes = {
  selectedContentTypes: PropTypes.array,
  allContentTypes: PropTypes.bool,
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
      allContentTypes: getAreAllContentTypesSelected(state, ownProps),
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
)(SearchFilter)
