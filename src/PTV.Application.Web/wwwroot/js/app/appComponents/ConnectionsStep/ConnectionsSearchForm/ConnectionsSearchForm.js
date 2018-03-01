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
import { connect } from 'react-redux'
import { Button } from 'sema-ui-components'
import { reduxForm, reset } from 'redux-form/immutable'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'
import { EntitySchemas } from 'schemas'
import { apiCall3 } from 'actions'
import { compose } from 'redux'
import {
  Fulltext,
  FrontPageOrganization,
  ChannelType,
  Organization } from 'util/redux-form/fields'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getConnectionChannelSearchTotal,
  getConnectionChannelSearchCount,
  getIsConnectionChannelPageSearching,
  getConnectionServiceSearchTotal,
  getIsConnectionServicePageSearching,
  getConnectionServiceSearchCount
} from '../ConnectionsSearchTable/ConnectionsSearchTable'
import { getSelectedEntityType, getSelectedEntityId } from 'selectors/entities/entities'
import { Spacer } from 'appComponents'
import { createSelector } from 'reselect'
import { Map } from 'immutable'
import styles from './styles.scss'
import { formTypesEnum, entityTypesEnum } from 'enums'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { getUserOrganization } from 'selectors/userInfo'
import { getServiceConnectionSearchIsMoreAvailable,
  getChannelConnectionSearchIsMoreAvailable } from 'appComponents/ConnectionsStep/selectors'

const messages = defineMessages({
  clearButtonTitle: {
    id: 'ModalDialog.ClearSelectedGeneralDescription.Button.Clear',
    defaultMessage: 'Tyhjennä'
  },
  searchButton: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
  },
  moreResultsAvailable: {
    id: 'ConnectionsStep.ConnectionsSearchForm.MoreResultsAvailable',
    defaultMessage: 'More results available'
  },
  noSearchResults: {
    id: 'FrontPageSearchFrom.NoResults',
    defaultMessage: 'Ei hakutuloksia'
  },
  searchResults: {
    id: 'FrontPageSearchFrom.ResultsCount',
    defaultMessage: 'Hakutuloksia:'
  }
})

const getInitialValues = createSelector(
  [getContentLanguageCode, getSelectedEntityId, getSelectedEntityType, getUserOrganization],
  (language, id, type, organizationId) => Map({
    language,
    id,
    type,
    organizationId
  })
)
const getIsChannelFilterVisible = createSelector(
  getSelectedEntityType,
  entityType => {
    const result = entityType !== entityTypesEnum.CHANNELS
    return result
  }
)

const SearchConnectionsForm = ({
  isChannelFilterVisible,
  handleSubmit,
  resetForm,
  dispatch,
  count,
  isMore,
  searchMode,
  total,
  submitSucceeded,
  isSearching,
  intl: { formatMessage }
}) => {
  const handleKeySubmit = (event) => {
    if (event.key === 'Enter') {
      event.preventDefault()
      handleSubmit()
    }
  }
  const handleClear = e => {
    e.preventDefault()
    dispatch({
      type: API_CALL_CLEAN,
      keys: searchMode === 'channels' ? ['connectionsServiceSearch'] : ['connectionsChannelSearch']
    })
    resetForm(formTypesEnum.SEARCHCONNECTIONSFORM)
  }

  const searchResultsMessageTemplate = formatMessage(messages.searchResults) + ` ${count}/${total}`

  return (
    <div>
      <div>
        <div className='row'>
          <div className='col-lg-8'>
            <Fulltext name='name' label={null} onKeyDown={handleKeySubmit} big />
          </div>
          {isChannelFilterVisible
          ? <div className='col-lg-8'>
            <FrontPageOrganization name='organizationId' labelPosition='inside' />
          </div>
          : <div className='col-lg-8'>
            <Organization
              name='organizationId'
              labelPosition='inside'
              skipValidation />
          </div>}
          {isChannelFilterVisible &&
          <div className='col-lg-8'>
            <ChannelType labelPosition='inside' />
          </div>}
        </div>
      </div>
      <Spacer />
      <div className={styles.formFooter}>
        <div className={styles.buttonGroup}>
          <Button
            onClick={handleSubmit}
            type='submit'
            children={formatMessage(messages.searchButton)}
            small
          />
          <Button
            onClick={handleClear}
            secondary
            children={formatMessage(messages.clearButtonTitle)}
            small
          />
        </div>
        {submitSucceeded && !isSearching &&
          (total !== 0 &&
          <div>{ isMore
            ? formatMessage(messages.moreResultsAvailable) + searchResultsMessageTemplate
            : searchResultsMessageTemplate}
          </div> ||
          <div>{formatMessage(messages.noSearchResults)}</div>)}
      </div>
    </div>
  )
}
SearchConnectionsForm.propTypes = {
  searchMode: PropTypes.oneOf(['channels', 'services', 'generalDescriptions']),
  isChannelFilterVisible: PropTypes.bool,
  isMore: PropTypes.bool,
  handleSubmit: PropTypes.func,
  intl: intlShape,
  dispatch: PropTypes.func,
  resetForm: PropTypes.func,
  total: PropTypes.number,
  count: PropTypes.number
}

const onSubmit = (formValues, dispatch, { searchMode }) => {
  formValues = formValues.toJS()
  switch (searchMode) {
    case 'channels':
      dispatch(
        apiCall3({
          keys: ['connectionsServiceSearch'],
          payload: {
            endpoint: 'service/GetConnectableServices',
            data: formValues
          },
          saveRequestData: true,
          schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE)
        })
      )
      break
    case 'services':
      dispatch(
        apiCall3({
          keys: ['connectionsChannelSearch'],
          payload: {
            endpoint: 'channel/GetConnectableChannels',
            data: formValues
          },
          saveRequestData: true,
          schemas: EntitySchemas.GET_SEARCH(EntitySchemas.CHANNEL)
        })
      )
      break
    case 'generalDescriptions':
      dispatch(
        apiCall3({
          keys: ['connectionsChannelSearch'],
          payload: {
            endpoint: 'channel/GetConnectableChannels',
            data: formValues
          },
          saveRequestData: true,
          schemas: EntitySchemas.GET_SEARCH(EntitySchemas.CHANNEL)
        })
      )
      break
  }
}

export default compose(
  injectIntl,
  connect((state, { searchMode }) => ({
    total: (searchMode === 'channels' && getConnectionServiceSearchTotal(state)) ||
     ((searchMode === 'services' || searchMode === 'generalDescriptions') &&
     getConnectionChannelSearchTotal(state)) || 0,
    count: (searchMode === 'channels' && getConnectionServiceSearchCount(state)) ||
     ((searchMode === 'services' || searchMode === 'generalDescriptions') &&
     getConnectionChannelSearchCount(state)) || 0,
    isSearching: (searchMode === 'channels' && getIsConnectionServicePageSearching(state)) ||
     ((searchMode === 'services' || searchMode === 'generalDescriptions') &&
     getIsConnectionChannelPageSearching(state)) || false,
    isChannelFilterVisible: getIsChannelFilterVisible(state),
    initialValues: getInitialValues(state, { formName: formTypesEnum.SEARCHCONNECTIONSFORM }),
    isMore: (searchMode === 'channels' && getServiceConnectionSearchIsMoreAvailable(state)) ||
      ((searchMode === 'services' || searchMode === 'generalDescriptions') &&
        getChannelConnectionSearchIsMoreAvailable(state)) ||
      false,
    searchMode
  }), {
    resetForm: reset
  }),
  reduxForm({
    form: formTypesEnum.SEARCHCONNECTIONSFORM,
    onSubmit
  })
)(SearchConnectionsForm)
