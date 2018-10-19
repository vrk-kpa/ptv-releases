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
  getFormValues,
  submit,
  initialize
} from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { OrderedSet } from 'immutable'
import Spacer from 'appComponents/Spacer'
import { MassToolLink } from '../MassTool'

import {
  getSelectedPublishingStatusesIds,
  getFrontPageSearchFormIsFetching,
  getTranslatableLanguages,
  getDomainSearchPageNumber,
  getDomainSearchPageSize,
  getDomainSearchTotal,
  getDomainSearchCount,
  getSearchDomain,
  getIsFrontPageSearching,
  isSelectedServiceContentType,
  isSelectedGdContentType
} from '../../selectors'
import { getUiSortingData } from 'selectors/base'
import { getUserOrganization } from 'selectors/userInfo'
import { Button, Spinner } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  CommonSearchFields,
  ServiceSearchFields
} from './partial'
import {
  fetchEntities,
  untouchAll
} from '../../actions'
import { PublishingStatus } from 'util/redux-form/fields'
import withState from 'util/withState'
import cx from 'classnames'
import styles from './styles.scss'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import SearchFilterLanguage from '../SearchFilterLanguage'
import { formTypesEnum, userRoleTypesEnum } from 'enums'
import { getRole } from 'appComponents/Security/selectors'

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
  },
  languageFilterTitle: {
    id: 'FrontPage.SelectLanguage.Title',
    defaultMessage: 'Kielivalinta'
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
  render () {
    const {
      handleSubmit,
      searchFormIsFetching,
      serviceSelected,
      gdSelected,
      intl: { formatMessage },
      submitSucceeded,
      isSearching,
      total,
      dirty,
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
            {areSubFiltersVisible &&
            <Spacer />
            }
            {areSubFiltersVisible &&
            <div className='row'>
              <div className='col-lg-3'>
                <PublishingStatus name='selectedPublishingStatuses' />
              </div>
              <div className='col-md-5'>
                <SearchFilterLanguage
                  filterName='languageSearch'
                  title={formatMessage(messages.languageFilterTitle)}
                />
              </div>
              <div className='col-lg-15'>
                {(serviceSelected || gdSelected) && <ServiceSearchFields gdSelected={gdSelected} />}
              </div>
            </div>
            }
            <Spacer />
            <div className={styles.formActions}>
              <Button
                small
                type='submit'
                children={isSearching && <Spinner /> || formatMessage(messages.buttonSearch)}
                disabled={isSearching}
              />
              {submitSucceeded && !isSearching &&
              (total !== 0 && <div>{formatMessage(messages.searchResults) + ` ${count}/${total}`}</div> ||
              <div>{formatMessage(messages.noSearchResults)}</div>)}
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
  total: PropTypes.number.isRequired,
  count: PropTypes.number.isRequired,
  intl: intlShape.isRequired,
  areSubFiltersVisible: PropTypes.bool.isRequired,
  mergeInUIState: PropTypes.func.isRequired,
  frontPageFormState: PropTypes.object,
  resubmit: PropTypes.func.isRequired,
  serviceSelected: PropTypes.bool.isRequired,
  gdSelected: PropTypes.bool.isRequired,
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
    dispatch(untouchAll())
  })
}
const resubmit = formValues => ({ dispatch, getState }) => {
  if (formValues) {
    dispatch(initialize(formTypesEnum.FRONTPAGESEARCH, formValues))
    dispatch(submit(formTypesEnum.FRONTPAGESEARCH))
  }
}
export default compose(
  injectIntl,
  connect(state => {
    const initialValues = {
      selectedPublishingStatuses: getSelectedPublishingStatusesIds(state),
      organizationIds: OrderedSet([getUserOrganization(state)]),
      languages: getTranslatableLanguages(state)
    }
    return {
      searchFormIsFetching: getFrontPageSearchFormIsFetching(state),
      initialValues,
      searchDomain: getSearchDomain(state),
      isSearching: getIsFrontPageSearching(state),
      total: getDomainSearchTotal(state),
      page: getDomainSearchPageNumber(state),
      pageSize: getDomainSearchPageSize(state),
      count: getDomainSearchCount(state),
      serviceSelected: isSelectedServiceContentType(state, { formName: formTypesEnum.FRONTPAGESEARCH }),
      gdSelected: isSelectedGdContentType(state, { formName: formTypesEnum.FRONTPAGESEARCH }),
      userRole: getRole(state)
    }
  }, {
    resubmit
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
