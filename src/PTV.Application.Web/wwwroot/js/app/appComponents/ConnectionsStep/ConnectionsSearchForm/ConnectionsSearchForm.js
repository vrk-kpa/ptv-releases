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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { Button, Spinner } from 'sema-ui-components'
import { reduxForm, reset } from 'redux-form/immutable'
import { API_CALL_CLEAN } from 'actions'
import { compose } from 'redux'
import {
  Fulltext,
  ShouldShowSuggestedChannelsCheckbox,
  ChannelType,
  Organization,
  ServiceOrChannelType
} from 'util/redux-form/fields'
import {
  getServiceSuggestedChannels,
  getIsShowSuggestedChannelsVisible,
  getConnectionSearchCount,
  getConnectionSearchIsFetching,
  getConnectionSearchReturnedPageNumber,
  getConnectionSearchTotal,
  getConnectionSearchIsMoreAvailable
} from '../selectors'
import { loadConnectableEntities } from '../actions'
import Spacer from 'appComponents/Spacer'
import { SearchIcon } from 'appComponents/Icons'
import { SearchButton } from 'appComponents/Buttons'
import styles from './styles.scss'
import { formTypesEnum, entityTypesEnum } from 'enums'
import { injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { messages } from './messages'
import {
  getShowAllOrganizations,
  getInitialValues,
  getIsServiceCollectionFilterVisible,
  getIsChannelFilterVisible
} from './selectors'

const getKeys = searchMode => {
  switch (searchMode) {
    case 'channels': return ['connectionsServiceSearch']
    case 'serviceCollections': return ['connectionsContentSearch']
    default: return ['connectionsChannelSearch']
  }
}

class SearchConnectionsForm extends Component {
  handleKeySubmit = event => {
    if (event.key === 'Enter') {
      event.preventDefault()
      this.props.handleSubmit()
    }
  }

  handleClear = event => {
    event.preventDefault()
    this.props.dispatch({
      type: API_CALL_CLEAN,
      keys: getKeys(this.props.searchMode)
    })
    this.props.resetForm(formTypesEnum.SEARCHCONNECTIONSFORM)
  }

  render () {
    const searchResultsMessageTemplate = this.props.intl.formatMessage(messages.searchResults) + ` ${this.props.count}/${this.props.total}`
    const {
      isChannelFilterVisible,
      isServiceCollectionFilterVisible,
      isMore,
      total,
      submitSucceeded,
      isFetching,
      pageNumber,
      isShowAll,
      intl: { formatMessage },
      isShowSuggestedChannelsVisible
    } = this.props

    return (
      <div className={styles.formHeadDark}>
        <div className='row'>
          <div className='col-lg-8'>
            <div className={styles.searchFieldContainer}>
              <Fulltext name='name' label={null} onKeyDown={this.handleKeySubmit} big className={styles.searchField} />
              <SearchButton
                type='submit'
                onClick={this.props.handleSubmit}
                isSearching={isFetching && !pageNumber}
                disabled={isFetching}
                className={styles.searchButton}
                spinnerClass={styles.searchSpinner}
              >
                <SearchIcon size={20} inverse />
              </SearchButton>
            </div>
          </div>
          {isChannelFilterVisible && (
            <div className='col-lg-8'>
              <ChannelType
                labelPosition='inside' />
            </div>
          )}
          {isServiceCollectionFilterVisible && (
            <div className='col-lg-8'>
              <ServiceOrChannelType
                labelPosition='inside'
                name='contentType' />
            </div>
          )}
          {isChannelFilterVisible ? (
            <div className='col-lg-8'>
              <Organization
                name='organizationId'
                labelPosition='inside'
                showAll
                skipValidation
              />
            </div>
          ) : (
            <div className='col-lg-8'>
              <Organization
                name='organizationId'
                labelPosition='inside'
                showAll={isShowAll}
                skipValidation />
            </div>
          )}
        </div>

        <Spacer />

        <div className={styles.formFooter}>
          <div className={styles.formActions}>
            <div className={styles.buttonGroup}>
              <Button
                onClick={this.props.handleSubmit}
                type='submit'
                children={isFetching && !pageNumber && <Spinner /> || formatMessage(messages.searchButton)}
                small
                disabled={isFetching}
              />
              <Button
                onClick={this.handleClear}
                secondary
                children={formatMessage(messages.clearButtonTitle)}
                small
              />
            </div>
            {isShowSuggestedChannelsVisible && (
              <ShouldShowSuggestedChannelsCheckbox className={styles.shouldShowSuggestedCheckbox} />
            )}
          </div>
          {submitSucceeded && !isFetching &&
            (total !== 0 &&
            <div className={styles.resultInfo}>{ isMore
              ? formatMessage(messages.moreResultsAvailable) + searchResultsMessageTemplate
              : searchResultsMessageTemplate}
            </div> ||
            <div>{formatMessage(messages.noSearchResults)}</div>)}
        </div>
      </div>
    )
  }
}
SearchConnectionsForm.propTypes = {
  searchMode: PropTypes.oneOf(['channels', 'services', 'generalDescriptions', 'serviceCollections']),
  isChannelFilterVisible: PropTypes.bool,
  isServiceCollectionFilterVisible: PropTypes.bool,
  isMore: PropTypes.bool,
  handleSubmit: PropTypes.func,
  intl: intlShape,
  dispatch: PropTypes.func,
  resetForm: PropTypes.func,
  total: PropTypes.number,
  count: PropTypes.number,
  isFetching: PropTypes.bool,
  pageNumber: PropTypes.number,
  isShowSuggestedChannelsVisible: PropTypes.bool,
  isShowAll: PropTypes.bool,
  submitSucceeded: PropTypes.bool
}

const onSubmit = (formValues, dispatch, props) => {
  dispatch(({ dispatch, getState }) => {
    dispatch({
      type: API_CALL_CLEAN,
      keys: getKeys(props.searchMode)
    })
    dispatch(
      loadConnectableEntities(getState(), props)
    )
  })
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { searchMode }) => {
    const entryKey = {
      [entityTypesEnum.SERVICES]: 'connectionsChannelSearch',
      [entityTypesEnum.GENERALDESCRIPTIONS]: 'connectionsChannelSearch',
      [entityTypesEnum.CHANNELS]: 'connectionsServiceSearch',
      [entityTypesEnum.SERVICECOLLECTIONS]: 'connectionsContentSearch'
    }[searchMode]
    return {
      total: getConnectionSearchTotal(state, { entryKey }),
      count: getConnectionSearchCount(state, { entryKey }),
      isFetching: getConnectionSearchIsFetching(state, { entryKey }),
      pageNumber: getConnectionSearchReturnedPageNumber(state, { entryKey }),
      isMore: getConnectionSearchIsMoreAvailable(state, { entryKey }),
      isChannelFilterVisible: getIsChannelFilterVisible(state),
      isServiceCollectionFilterVisible: getIsServiceCollectionFilterVisible(state),
      initialValues: getInitialValues(state, { formName: formTypesEnum.SEARCHCONNECTIONSFORM }),
      searchMode,
      suggestedChannels: getServiceSuggestedChannels(state),
      isShowSuggestedChannelsVisible: getIsShowSuggestedChannelsVisible(state),
      isShowAll:getShowAllOrganizations(state)
    }
  }, {
    resetForm: reset
  }),
  reduxForm({
    form: formTypesEnum.SEARCHCONNECTIONSFORM,
    onSubmit
  })
)(SearchConnectionsForm)
