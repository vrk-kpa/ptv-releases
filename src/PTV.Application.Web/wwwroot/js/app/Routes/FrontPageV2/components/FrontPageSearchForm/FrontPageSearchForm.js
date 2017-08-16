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
import Divider from 'Components/Divider/Divider'
import {
  getSelectedPublishingStatusesIds,
  getUserOrganization,
  getFrontPageSearchFormIsFetching,
  getTranslatableLanguagesCodeArray,
  getDomainSearchPageNumber,
  getDomainSearchPageSize,
  getDomainSearchTotal,
  getDomainSearchIsSubmitting,
  getDomainSearchCount,
  getUiSortingData
} from 'Routes/FrontPageV2/selectors'
import { Button } from 'sema-ui-components'
import { getSearchDomain } from 'Containers/Common/Selectors'
import { PTVPreloader } from 'Components'
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
} from 'Routes/FrontPageV2/actions'

const messages = defineMessages({
  buttonSearch: {
    id: 'Components.Button.Search.Title',
    defaultMessage: 'Hae'
  },
  resultsCount: {
    id: 'FrontPageSearchFrom.ResultsCount',
    defaultMessage: 'Hakutuloksia: '
  },
  noResults: {
    id: 'FrontPageSearchFrom.NoResults',
    defaultMessage: 'Ei Hakutuloksia'
  }
})

class FrontPageSearchForm extends Component {
  componentDidMount () {
    this.props.resubmit()
  }
  render () {
    const {
      handleSubmit,
      searchDomain,
      searchFormIsFetching,
      intl: { formatMessage },
      page,
      pageSize,
      total,
      dirty,
      count,
      isSubmitting,
      submitSucceeded
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
    return (
      <form onSubmit={handleSubmit} className='ptv-toolbox-container'>
        {searchFormIsFetching ? <PTVPreloader />
        : <div>
          <CommonSearchFields />
          <Divider />
          {domainSearchFields}
          {(searchDomain === 'services' || searchDomain === 'generalDescriptions') && <Divider />}
          <div
            style={{
              display: 'flex',
              flexDirection: 'row',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <div className='form-group'>
              <Button
                small
                type='submit'
                children={formatMessage(messages.buttonSearch)}
              />
            </div>
            {!isSubmitting && submitSucceeded &&
              <div>
                {total === 0
                  ? <b>{formatMessage(messages.noResults)}</b>
                  : <b>{formatMessage(messages.resultsCount)} {`${count}/${total}`}</b>}
              </div>}
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
  count: PropTypes.number,
  isSubmitting: PropTypes.bool,
  submitSucceeded: PropTypes.bool,
  intl: PropTypes.object.isRequired
}

const onSubmit = (_, dispatch, { searchDomain }) => {
  dispatch(({ getState, dispatch }) => {
    const state = getState()
    const formValues = getFormValues('frontPageSearch')(state)
    const sortingData = getUiSortingData(getState(), { contentType: searchDomain })
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
    sessionStorage.setItem('frontPageFormState', JSON.stringify(formValues.toJS()))
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
      total: getDomainSearchTotal(state),
      isSubmitting: getDomainSearchIsSubmitting(state),
      page: getDomainSearchPageNumber(state),
      pageSize: getDomainSearchPageSize(state),
      count: getDomainSearchCount(state)
    }
  }, {
    resubmit
  }),
  reduxForm({
    form: 'frontPageSearch',
    destroyOnUnmount : false,
    enableReinitialize: true,
    onSubmit
  })
)(FrontPageSearchForm)
