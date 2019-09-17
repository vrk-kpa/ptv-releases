/* eslint-disable no-undef */
/* eslint-disable no-return-assign */
/* eslint-disable react/jsx-indent */
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
import React, { PureComponent, Fragment } from 'react'
import { Map, List, Set } from 'immutable'
import { reduxForm } from 'redux-form/immutable'
import PropTypes from 'prop-types'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { formTypesEnum, BREAKPOINT_LG } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'
import { messages } from 'Routes/Admin/messages'
import { OrganizationDialog, organizationDialogName } from 'Routes/Search/components/SearchFilters/dialogs'
import SearchFilter, { SearchFilterCategory, SearchFilterList } from 'Routes/Search/components/SearchFilter'
import { getSelectedOrganizations } from 'Routes/Search/components/SearchFilters/selectors'
import { Button, Spinner, SimpleTable, Label } from 'sema-ui-components'
import { createGetEntityAction } from 'actions'
import { mergeInUIState } from 'reducers/ui'
import Spacer from 'appComponents/Spacer'
import RemoveMappingDialog from './RemoveMappingDialog'
import UpdateMappingDialog from './UpdateMappingDialog'
import { getSelectedLanguage } from 'Intl/Selectors'
import {
  closeOrganizationDialog,
  clearSelectedOrganization,
  clearSearchedOrganization,
  searchMapping,
  removeMapping,
  openRemoveOrganizationDialog,
  openUpdateOrganizationDialog,
  updateMapping
} from './actions'
import {
  getMappingIsFound,
  getMappingIsFetching,
  getResultsJS
} from './selectors'
import {
  columns
} from './columnDefinitions'
import EmptyTableMessage from 'appComponents/EmptyTableMessage'
import Paragraph from 'appComponents/Paragraph'
import withWindowDimensionsContext from 'util/redux-form/HOC/withWindowDimensions/withWindowDimensionsContext'

class AdminMapping extends PureComponent {
  handleOnClose = () => {
    this.props.clearSelectedOrganization()
  }
  hnadleOnClear =() => {
    this.props.clearSelectedOrganization()
    this.props.clearSearchedOrganization()
  }
  handleOnSearch = (selectedOrganizations) => {
    this.props.closeOrganizationDialog()
    this.props.searchMapping(selectedOrganizations)
  }
  handlePreviewOnClick = (entityId) => {
    this.props.mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formTypesEnum.ORGANIZATIONFORM,
        isOpen: true,
        entityId: null
      }
    })
    this.props.loadPreviewEntity(entityId, formTypesEnum.ORGANIZATIONFORM, 'loadPreview')
  }
  handleOnRemoveClick = (ptvId, sahaId, selectedOrganizations) => {
    this.props.openRemoveOrganizationDialog(ptvId, sahaId, selectedOrganizations)
  }
  handleRemove = (ptvId, sahaId, selectedOrganizations) => {
    this.props.removeMapping(ptvId, sahaId, selectedOrganizations)
  }
  handleOnUpdateClick = (ptvId, sahaId, selectedOrganizations) => {
    this.props.openUpdateOrganizationDialog(ptvId, sahaId, selectedOrganizations)
  }
  handleUpdate = (ptvId, sahaId, selectedOrganizations, newOrganizationId) => {
    this.props.updateMapping(ptvId, sahaId, selectedOrganizations, newOrganizationId)
  }

  render () {
    const {
      selectedOrganizations,
      intl: { formatMessage },
      isMappingFound,
      isFetching,
      languageCode,
      dimensions,
      rows
    } = this.props
    const selectedCount = selectedOrganizations.length
    const rowCount = rows.length
    const cardBreakpoint = dimensions.windowWidth <= BREAKPOINT_LG
    const mappingTableClass = cx(
      styles.sahaPtvMappingTable,
      {
        [styles.cards]: cardBreakpoint,
        [styles.mappingFound]: isMappingFound
      }
    )
    console.log('AM: ', dimensions, cardBreakpoint, BREAKPOINT_LG)
    return (
      <div className={styles.sahaPtvMapping}>
        <OrganizationDialog
          onlySelected
          description={formatMessage(messages.adminMappingOrganizationDialogDescription)}
        />
        <RemoveMappingDialog confirmAction={this.handleRemove} />
        <UpdateMappingDialog confirmAction={this.handleUpdate} />
        <SimpleTable
          columns={columns({
            previewOnClick: this.handlePreviewOnClick,
            removeClick: this.handleOnRemoveClick,
            selectedOrganizations,
            formatMessage,
            updateClick: this.handleOnUpdateClick,
            languageCode: languageCode
          })}
          scrollable
          columnWidths={['15%', '22%', '15%', '7%', '7%', '15%', '15%', '4%']}
          tight
          borderless
          className={mappingTableClass}
          defaultCards={false}
          asCards={cardBreakpoint}
        >
          <SimpleTable.Extra>
            <div className={styles.searchHeader}>
              <Paragraph>{formatMessage(messages.adminMappingHeader)}</Paragraph>
              <SearchFilter>
                <div className='d-flex align-items-center'>
                  <SearchFilterCategory
                    category={messages.searchOrganizationTitle}
                    dialogName={organizationDialogName}
                    className={styles.organizationSearchFilterCategory}
                  />
                  <SearchFilterList
                    items={selectedOrganizations}
                    className={styles.organizationSearchFilterList}
                  />
                </div>
              </SearchFilter>
              <div className={styles.formActions}>
                <div className={styles.buttonGroup}>
                  <Button
                    small
                    onClick={() => this.handleOnSearch(selectedOrganizations)}
                    children={formatMessage(messages.searchButton)}
                    disabled={selectedCount === 0}
                  />
                  <Button
                    small
                    onClick={this.hnadleOnClear}
                    secondary
                    children={formatMessage(messages.clearButton)}
                    disabled={rowCount === 0 && selectedCount === 0}
                  />
                </div>
                {rows.length > 0 && (
                  <Fragment>
                    <Label
                      infoLabel
                      className={styles.serachResultLabel}
                      labelText={formatMessage(messages.searchCountTitle) + ` (${rows.length})`}
                    />
                    <Spacer marginSize='m0' />
                  </Fragment>
                )}
              </div>
              {isMappingFound && !cardBreakpoint && <Spacer marginSize='m0' />}
            </div>
          </SimpleTable.Extra>

          {isMappingFound && (
            <Fragment>
              <SimpleTable.Header />
              {isFetching && <tbody><tr><td><div className={styles.spinner}><Spinner /></div></td></tr></tbody>}
              {!isFetching && rowCount === 0 && (
                <EmptyTableMessage message={formatMessage(messages.emptyMappingMessage)} />
              )}
              {!isFetching && rowCount > 0 && (
                <SimpleTable.Body
                  rowKey='id'
                  rows={rows}
                />
              )}
            </Fragment>
          )}

        </SimpleTable>
      </div>
    )
  }
}

AdminMapping.propTypes = {
  selectedOrganizations: PropTypes.array,
  intl: intlShape,
  loadPreviewEntity: PropTypes.func,
  isFetching: PropTypes.bool,
  rows: PropTypes.array,
  mergeInUIState: PropTypes.func,
  closeOrganizationDialog: PropTypes.func,
  clearSelectedOrganization: PropTypes.func,
  clearSearchedOrganization: PropTypes.func,
  searchMapping: PropTypes.func,
  removeMapping: PropTypes.func,
  openRemoveOrganizationDialog: PropTypes.func,
  isMappingFound: PropTypes.bool,
  dimensions: PropTypes.object
}

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const items = getSelectedOrganizations(state, { formName: formTypesEnum.ADMINMAPPINGFORM, ...ownProps })
      return {
        isMappingFound: getMappingIsFound(state, ownProps),
        selectedOrganizations:items,
        isFetching: getMappingIsFetching(state, ownProps),
        rows: getResultsJS(state),
        initialValues: Map({ organizationId:null, guid:null }),
        languageCode: getSelectedLanguage(state, ownProps)
      }
    }, {
      closeOrganizationDialog,
      clearSelectedOrganization,
      clearSearchedOrganization,
      searchMapping,
      mergeInUIState,
      loadPreviewEntity: createGetEntityAction,
      removeMapping,
      openRemoveOrganizationDialog,
      openUpdateOrganizationDialog,
      updateMapping
    }),
  reduxForm({
    form: formTypesEnum.ADMINMAPPINGFORM,
    destroyOnUnmount: false
  }),
  withBubbling(),
  withWindowDimensionsContext
)(AdminMapping)
