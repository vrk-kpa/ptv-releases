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
import PropTypes from 'prop-types'
import { reduxForm, reset, change } from 'redux-form/immutable'
import { API_CALL_CLEAN } from 'actions'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Map, fromJS } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import Spacer from 'appComponents/Spacer'
import SearchFieldSubmit from 'appComponents/SearchFieldSubmit'
import { createSelector } from 'reselect'
import SearchFilters from 'Routes/Connections/components/SearchFilters'
import SearchFormFooter from 'Routes/Connections/components/SearchFormFooter'
import { entityTypesEnum } from 'enums'
import { getHasResults, getHasResultCountOverPage } from 'Routes/Connections/selectors'
import { searchServices } from 'Routes/Connections/actions'
import styles from './styles.scss'
import { getUserOrganization } from 'selectors/userInfo'
import {
  getPublishingStatusDraftId,
  getPublishingStatusPublishedId
} from 'selectors/common'
import { Label } from 'sema-ui-components'
import connectionMessages from '../../messages'

const messages = defineMessages({
  searchLabel: {
    id: 'Routes.Connections.Components.SearchServicesForm.SearchField.Title',
    defaultMessage: 'Hae palveluita'
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
  setFormValue,
  showFilterMessage
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
      <Label labelText={formatMessage(messages.searchLabel)} />
      <SearchFieldSubmit label={null} />
      <SearchFilters entityType={entityTypesEnum.SERVICES} />
      <Spacer marginSize='m0' />
      <SearchFormFooter onClickClear={handleClear} />
      {showFilterMessage && <Label labelText={formatMessage(connectionMessages.resultCountInfo)} />}
      {hasResults && <Spacer marginSize='m0' />}
    </form>
  )
}
SearchServicesForm.propTypes = {
  intl: intlShape,
  handleSubmit: PropTypes.func,
  hasResults: PropTypes.bool,
  showFilterMessage: PropTypes.bool,
  dispatch: PropTypes.func,
  resetForm: PropTypes.func,
  setFormValue: PropTypes.func.isRequired
}

const onSubmit = (formValues, dispatch) => searchServices(formValues, dispatch, 0)

export default compose(
  injectIntl,
  connect(state => ({
    hasResults: getHasResults(state),
    initialValues: getInitialValues(state),
    showFilterMessage: getHasResultCountOverPage(state)
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
