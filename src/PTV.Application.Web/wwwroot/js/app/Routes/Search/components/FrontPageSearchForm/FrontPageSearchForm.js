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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import {
  reduxForm,
  getFormValues
} from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import ToggleButton from 'appComponents/Buttons/ToggleButton'
import SearchFilter from '../SearchFilter'
import { MassToolLink } from '../MassTool'

import {
  getFrontPageSearchFormIsFetching,
  getSearchAbort,
  getDomainSearchPageNumber,
  getDomainSearchPageSize,
  getDomainSearchTotal,
  getDomainSearchCount,
  getSearchDomain,
  getIsFrontPageSearching,
  getFrontPageInitialValues
} from '../../selectors'
import { getUiSortingData } from 'selectors/base'
import { Button, Spinner } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { CommonSearchFields } from './partial'
import {
  fetchEntities
  // untouchAll
} from '../../actions'
import withState from 'util/withState'
import cx from 'classnames'
import styles from './styles.scss'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import {
  formTypesEnum,
  userRoleTypesEnum
} from 'enums'
import { getRole } from 'appComponents/Security/selectors'
import { clearForm, resubmit } from './actions'

const messages = defineMessages({
  buttonSearch: {
    id: 'Components.Button.Search.Title',
    defaultMessage: 'Hae'
  },
  buttonReset: {
    id: 'Components.Button.Reset.Title',
    defaultMessage: 'Tyhjennä'
  },
  buttonStop: {
    id: 'Components.Button.Stop.Title',
    defaultMessage: 'Pysäytä haku',
    description: { en: 'Stop search' }
  },
  searchResults: {
    id: 'FrontPageSearchFrom.ResultsCount',
    defaultMessage: 'Hakutuloksia:'
  },
  noSearchResults: {
    id: 'FrontPageSearchFrom.NoResults',
    defaultMessage: 'Ei hakutuloksia'
  },
  toggleFilterTitle: {
    id: 'FrontPageSearchForm.ToggleFilter.Title',
    defaultMessage: 'Rajaa hakua suodattimilla'
  },
  toggleFilterTooltip: {
    id: 'FrontPageSearchForm.ToggleFilter.Tooltip',
    defaultMessage: 'Toggle placeholder.'
  }
})

class FrontPageSearchForm extends Component {
  componentDidMount () {
    this.props.resubmit(this.props.frontPageFormState)
  }
  toggleSubFilters = () => {
    this.props.mergeInUIState({
      key: 'frontPageSearchForm',
      value: {
        areSubFiltersVisible: !this.props.areSubFiltersVisible
      }
    })
  }

  onClear = e => {
    e.preventDefault()
    this.props.reset()
    this.props.clearForm()
  }

  onStop = e => {
    e.preventDefault()
    this.props.abort && this.props.abort()
  }

  render () {
    const {
      handleSubmit,
      searchFormIsFetching,
      intl: { formatMessage },
      submitSucceeded,
      isSearching,
      total,
      count,
      areSubFiltersVisible,
      userRole
    } = this.props
    const searchFormClass = cx(
      {
        [styles.isFetching]: searchFormIsFetching
      }
    )

    return (
      <form onSubmit={handleSubmit}>
        {searchFormIsFetching ? <Spinner />
          : <div className={searchFormClass}>
            <CommonSearchFields
              areSubFiltersVisible={this.props.areSubFiltersVisible}
              toggleSubFilters={this.toggleSubFilters}
            />
            <div className={styles.subFiltersToggle} >
              <ToggleButton
                onClick={this.toggleSubFilters}
                showIcon
                isCollapsed={!areSubFiltersVisible}
                tooltip={formatMessage(messages.toggleFilterTooltip)}
                stickyTooltip
              >
                <span
                  className={styles.subFiltersTitle}>
                  {formatMessage(messages.toggleFilterTitle)}
                </span>
              </ToggleButton>
            </div>
            {areSubFiltersVisible && <SearchFilter />}
            <div className={styles.formActions}>
              <div className={styles.buttonGroup}>
                <Button
                  small
                  type='submit'
                  children={isSearching && <Spinner /> || formatMessage(messages.buttonSearch)}
                  disabled={isSearching}
                />
                {isSearching && this.props.abort
                  ? <Button
                    small
                    secondary
                    type='button'
                    onClick={this.onStop}
                    children={formatMessage(messages.buttonStop)}
                    disabled={!isSearching}
                  />
                  : <Button
                    small
                    secondary
                    onClick={this.onClear}
                    children={formatMessage(messages.buttonReset)}
                    disabled={isSearching}
                  />
                }
                {submitSucceeded && !isSearching &&
                  (total !== 0 && <div>{formatMessage(messages.searchResults) + ` ${count}/${total}`}</div> ||
                  <div>{formatMessage(messages.noSearchResults)}</div>)}
              </div>
              {userRole !== userRoleTypesEnum.SHIRLEY && <MassToolLink /> || null}
            </div>
          </div>
        }
      </form>
    )
  }
}
FrontPageSearchForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  searchFormIsFetching: PropTypes.bool.isRequired,
  submitSucceeded: PropTypes.bool.isRequired,
  isSearching: PropTypes.bool.isRequired,
  total: PropTypes.number.isRequired,
  count: PropTypes.number.isRequired,
  intl: intlShape.isRequired,
  areSubFiltersVisible: PropTypes.bool.isRequired,
  mergeInUIState: PropTypes.func.isRequired,
  reset: PropTypes.func.isRequired,
  abort: PropTypes.func,
  frontPageFormState: PropTypes.object,
  resubmit: PropTypes.func.isRequired,
  clearForm: PropTypes.func.isRequired,
  userRole: PropTypes.string
}

const onSubmit = (_, dispatch, { updateUI }) => {
  dispatch(({ dispatch, getState }) => {
    const state = getState()
    let formValues = getFormValues('frontPageSearch')(state)
    const sortingData = getUiSortingData(getState(), { contentType: 'entities' })
    updateUI('frontPageFormState', formValues)
    const selectedPublishingStatuses = formValues.get('selectedPublishingStatuses')
      .filter(value => value)
      .keySeq()
    formValues = formValues.set('selectedPublishingStatuses', selectedPublishingStatuses)
    dispatch(fetchEntities({
      ...formValues.toJS(),
      sortData: sortingData.size > 0 ? [sortingData] : []
    }))
    // not needed, form is reinitialized in resubmit action
    // dispatch(untouchAll())
  })
}

export default compose(
  injectIntl,
  connect(state => {
    return {
      searchFormIsFetching: getFrontPageSearchFormIsFetching(state),
      initialValues: getFrontPageInitialValues(state),
      searchDomain: getSearchDomain(state),
      isSearching: getIsFrontPageSearching(state),
      total: getDomainSearchTotal(state),
      page: getDomainSearchPageNumber(state),
      pageSize: getDomainSearchPageSize(state),
      count: getDomainSearchCount(state),
      userRole: getRole(state),
      abort: getSearchAbort(state)
    }
  }, {
    resubmit,
    clearForm
  }),
  withState({
    redux: true,
    keepImmutable: true,
    key: 'frontPageSearchForm',
    initialState: {
      areSubFiltersVisible: false,
      frontPageFormState: null
    }
  }),
  reduxForm({
    form: formTypesEnum.FRONTPAGESEARCH,
    destroyOnUnmount : false,
    enableReinitialize: true,
    onSubmit
  }),
  withPreviewDialog
)(FrontPageSearchForm)
