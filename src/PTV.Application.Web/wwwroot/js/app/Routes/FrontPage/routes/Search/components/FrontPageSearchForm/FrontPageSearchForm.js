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
import React, { Component, PropTypes } from 'react'
import { reduxForm, getFormValues, submit, initialize } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Map, fromJS } from 'immutable'
import { Spacer } from 'appComponents'
import {
  getSelectedPublishingStatusesIds,
  getUserOrganization,
  getFrontPageSearchFormIsFetching,
  getTranslatableLanguagesCodeArray,
  getDomainSearchPageNumber,
  getDomainSearchPageSize,
  getDomainSearchTotal,
  getDomainSearchCount,
  getUiSortingData
} from 'Routes/FrontPage/routes/Search/selectors'
import { Button, Spinner } from 'sema-ui-components'
import { getSearchDomain, getIsFrontPageSearching } from '../../selectors'
import { defineMessages, injectIntl } from 'react-intl'
import {
  CommonSearchFields,
  ServiceSearchFields,
  GeneralDescriptionSearchFields,
  OrganizationSearchFields
} from './partial'
import {
  fetchServices,
  fetchChannels,
  fetchGeneralDescriptions,
  fetchOrganizations,
  untouchAll
} from 'Routes/FrontPage/routes/Search/actions'
import { PublishingStatus, Language } from 'util/redux-form/fields'
import withState from 'util/withState'
import cx from 'classnames'
import styles from './styles.scss'
import { withPreviewDialog } from 'util/redux-form/HOC'

const messages = defineMessages({
  buttonSearch: {
    id: 'Components.Button.Search.Title',
    defaultMessage: 'Hae'
  },
  searchResults: {
    id: 'FrontPageSearchFrom.ResultsCount',
    defaultMessage: 'Hakutuloksia:'
  },
  noSearchResults: {
    id: 'FrontPageSearchFrom.NoResults',
    defaultMessage: 'Ei hakutuloksia'
  }
})

class FrontPageSearchForm extends Component {
  componentDidMount () {
    this.props.resubmit()
  }
  toggleSubFilters = () => {
    this.props.updateUI('areSubFiltersVisible', !this.props.areSubFiltersVisible)
  }
  render () {
    const {
      handleSubmit,
      searchDomain,
      searchFormIsFetching,
      intl: { formatMessage, locale },
      page,
      submitSucceeded,
      isSearching,
      pageSize,
      total,
      dirty,
      count,
      areSubFiltersVisible
    } = this.props
    let domainSearchFields = null
    switch (searchDomain) {
      case 'services':
        domainSearchFields = <ServiceSearchFields />
        break
      case 'generalDescriptions':
        domainSearchFields = <GeneralDescriptionSearchFields />
        break
      case 'organizations':
        domainSearchFields = <OrganizationSearchFields />
        break
    }
    const searchFormClass = cx(
      {
        [styles.isFetching]: searchFormIsFetching
      }
    )
    return (
      <form onSubmit={handleSubmit} className={searchFormClass}>
        {searchFormIsFetching ? <Spinner />
        : <div>
          <CommonSearchFields
            areSubFiltersVisible={this.props.areSubFiltersVisible}
            toggleSubFilters={this.toggleSubFilters}
          />
          {areSubFiltersVisible &&
            <Spacer />
          }
          {areSubFiltersVisible &&
            <div className='row'>
              <div className='col-lg-3'>
                <PublishingStatus name='selectedPublishingStatuses' />
              </div>
              <div className='col-lg-5'>
                <Language
                  clearable={false}
                  locale={locale}
                />
              </div>
              <div className='col-lg-15'>
                {domainSearchFields}
              </div>
            </div>
          }
          <Spacer />
          <div className={styles.formActions}>
            <Button
              small
              type='submit'
              children={formatMessage(messages.buttonSearch)}
            />
            {submitSucceeded && !isSearching &&
              (total !== 0 && <div>{formatMessage(messages.searchResults) + ` ${count}/${total}`}</div> ||
              <div>{formatMessage(messages.noSearchResults)}</div>)}
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
  searchDomain: PropTypes.string.isRequired,
  page: PropTypes.number.isRequired,
  pageSize: PropTypes.number.isRequired,
  total: PropTypes.number.isRequired,
  count: PropTypes.number.isRequired,
  intl: PropTypes.object.isRequired,
  areSubFiltersVisible: PropTypes.bool.isRequired,
  updateUI: PropTypes.func.isRequired,
  resubmit: PropTypes.func.isRequired
}

const onSubmit = (_, dispatch, { searchDomain }) => {
  dispatch(({ dispatch, getState }) => {
    const state = getState()
    let formValues = getFormValues('frontPageSearch')(state)
    const sortingData = getUiSortingData(getState(), { contentType: searchDomain })
    sessionStorage.setItem('frontPageFormState', JSON.stringify(formValues.toJS()))
    const selectedPublishingStatuses = formValues.get('selectedPublishingStatuses')
      .filter(value => value)
      .keySeq()
    formValues = formValues.set('selectedPublishingStatuses', selectedPublishingStatuses)
    switch (searchDomain) {
      case 'services':
        dispatch(fetchServices({
          ...formValues.toJS(),
          sortData: sortingData.size > 0 ? [sortingData] : []
        }))
        break
      case 'eChannel':
      case 'webPage':
      case 'printableForm':
      case 'phone':
      case 'serviceLocation':
        dispatch(fetchChannels({
          ...formValues.toJS(),
          channelType: searchDomain,
          sortData: sortingData.size > 0 ? [sortingData] : []
        }))
        break
      case 'generalDescriptions':
        dispatch(fetchGeneralDescriptions({
          ...formValues.toJS(),
          sortData: sortingData.size > 0 ? [sortingData] : []
        }))
        break
      case 'organizations':
        dispatch(fetchOrganizations({
          ...formValues.toJS(),
          sortData: sortingData.size > 0 ? [sortingData] : []
        }))
        break
    }
    dispatch(untouchAll())
  })
}
const resubmit = () => ({ dispatch, getState }) => {
  let formValues = sessionStorage.getItem('frontPageFormState')
  if (formValues) {
    formValues = fromJS(JSON.parse(formValues))
    dispatch(initialize('frontPageSearch', formValues))
    dispatch(submit('frontPageSearch'))
  }
}
export default compose(
  injectIntl,
  connect(state => {
    const initialValues = {
      selectedPublishingStatuses: getSelectedPublishingStatusesIds(state),
      organizationId: getUserOrganization(state),
      languages: getTranslatableLanguagesCodeArray(state)
    }
    return {
      searchFormIsFetching: getFrontPageSearchFormIsFetching(state),
      initialValues,
      searchDomain: getSearchDomain(state),
      isSearching: getIsFrontPageSearching(state),
      total: getDomainSearchTotal(state),
      page: getDomainSearchPageNumber(state),
      pageSize: getDomainSearchPageSize(state),
      count: getDomainSearchCount(state)
    }
  }, {
    resubmit
  }),
  withState({
    redux: true,
    key: 'commonSearchFields',
    initialState: {
      areSubFiltersVisible: false
    }
  }),
  reduxForm({
    form: 'frontPageSearch',
    destroyOnUnmount : false,
    enableReinitialize: true,
    onSubmit
  }),
  withPreviewDialog
)(FrontPageSearchForm)
