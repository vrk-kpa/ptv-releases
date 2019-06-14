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
import PropTypes from 'prop-types'
import { reduxForm, reset, change } from 'redux-form/immutable'
import { API_CALL_CLEAN } from 'actions'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Fulltext } from 'util/redux-form/fields'
import { apiCall3 } from 'actions'
import { Map, fromJS } from 'immutable'
import { EntitySchemas } from 'schemas'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import Spacer from 'appComponents/Spacer'
import { createSelector } from 'reselect'
import SearchFilters from 'Routes/Connections/components/SearchFilters'
import SearchFormFooter from 'Routes/Connections/components/SearchFormFooter'
import { entityTypesEnum } from 'enums'
import { getHasResults } from 'Routes/Connections/selectors'
import styles from './styles.scss'
import { getUserOrganization } from 'selectors/userInfo'
import {
  getPublishingStatusDraftId,
  getPublishingStatusPublishedId
} from 'selectors/common'

const messages = defineMessages({
  clearButtonTitle: {
    id: 'ModalDialog.ClearSelectedGeneralDescription.Button.Clear',
    defaultMessage: 'Tyhjennä'
  },
  searchButton: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
  },
  searchLabel: {
    id: 'Routes.Connections.Components.SearchServicesForm.SearchField.Title',
    defaultMessage: 'Hae palveluita'
  },
  searchPlaceholder: {
    id: 'Routes.Connections.Components.SearchServicesForm.SearchField.Placeholder',
    defaultMessage: 'Hae...'
  }
})

const getInitialValues = createSelector(
  [getUserOrganization, getPublishingStatusDraftId, getPublishingStatusPublishedId],
  (organizationId, draftId, publishedId) => Map({
    organizationId,
    selectedPublishingStatuses:fromJS({ [draftId]:true, [publishedId]:true })
  })
)

const SearchServicesForm = ({
  intl: { formatMessage },
  handleSubmit,
  dispatch,
  resetForm,
  hasResults,
  setFormValue
}) => {
  const handleClear = e => {
    e.preventDefault()
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['connections', 'serviceSearch']
    })
    resetForm('searchServicesConnections')
    setFormValue('searchServicesConnections', 'organizationId', null)
  }
  return (
    <form onSubmit={handleSubmit} className={styles.searchForm}>
      <Fulltext
        small
        label={formatMessage(messages.searchLabel)}
        placeholder={formatMessage(messages.searchPlaceholder)}
    />
      <SearchFilters entityType={entityTypesEnum.SERVICES} />
      <Spacer marginSize='m0' />
      <SearchFormFooter onClickClear={handleClear} />
      {hasResults && <Spacer marginSize='m0' />}
    </form>
  )
}
SearchServicesForm.propTypes = {
  intl: intlShape,
  handleSubmit: PropTypes.func,
  hasResults: PropTypes.bool,
  dispatch: PropTypes.func,
  resetForm: PropTypes.func,
  setFormValue: PropTypes.func.isRequired
}

const onSubmit = (formValues, dispatch) => {
  const selectedPublishingStatuses = formValues.get('selectedPublishingStatuses').filter(value => value).keySeq()
  formValues = formValues.set('selectedPublishingStatuses', selectedPublishingStatuses)
  // Will be removed after use multiselect
  formValues = formValues.set('serviceClasses', formValues.get('serviceClasses') ? [formValues.get('serviceClasses')] : [])
  formValues = formValues.set('ontologyTerms', formValues.get('ontologyTerms') ? [formValues.get('ontologyTerms')] : [])
  formValues = formValues.set('areaInformationTypes', formValues.get('areaInformationTypes') ? [formValues.get('areaInformationTypes')] : [])
  formValues = formValues.set('targetGroups', formValues.get('targetGroups') ? [formValues.get('targetGroups')] : [])
  formValues = formValues.set('lifeEvents', formValues.get('lifeEvents') ? [formValues.get('lifeEvents')] : [])
  formValues = formValues.set('industrialClasses', formValues.get('industrialClasses') ? [formValues.get('industrialClasses')] : [])

  formValues = formValues.toJS()
  dispatch(
    apiCall3({
      keys: [
        'connections',
        'serviceSearch'
      ],
      payload: {
        endpoint: 'service/GetConnectionsServices',
        data: formValues
      },
      saveRequestData: true,
      schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE)
    })
  )
}

export default compose(
  injectIntl,
  connect(state => ({
    hasResults: getHasResults(state),
    initialValues: getInitialValues(state)
  }), {
    resetForm: reset,
    setFormValue: change
  }),
  reduxForm({
    form: 'searchServicesConnections',
    onSubmit,
    destroyOnUnmount : false,
    enableReinitialize: true
  }),
)(SearchServicesForm)
