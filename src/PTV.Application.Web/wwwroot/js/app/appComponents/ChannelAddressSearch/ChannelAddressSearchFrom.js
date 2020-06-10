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
import { compose } from 'redux'
import {
  reduxForm,
  formValueSelector
} from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  Button,
  Spinner
} from 'sema-ui-components'
import {
  Street,
  AddressNumber,
  PostalCode,
  PostOffice
} from 'util/redux-form/fields'
import { mergeInUIState } from 'reducers/ui'
import { connect } from 'react-redux'
import {
  getAddressSearchIsFetching,
  isSearchExists,
  isAddressSearchResultExists,
  getAddressSearchCount,
  getAddressSearchTotal
} from './selectors/index'
import Spacer from 'appComponents/Spacer'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import styles from './styles.scss'
import { API_CALL_CLEAN } from 'actions'
import { formTypesEnum, addressUseCasesEnum, addressTypesEnum } from 'enums'
import SearchResult from './SearchResult'
import { loadAddresses } from './actions'
import withPath from 'util/redux-form/HOC/withPath'
import injectFormName from 'util/redux-form/HOC/injectFormName'

export const messages = defineMessages({
  addressSearchTitle: {
    id: 'AppComponents.ChannelsAddressSearch.Title',
    defaultMessage: 'Hae samasta katuosoitteesta löytyvät muut palvelupaikat'
  },
  addressSearchTooltip: {
    id: 'AppComponents.ChannelsAddressSearch.Tooltip',
    defaultMessage: 'Hae samasta katuosoitteesta löytyvät muut palvelupaikat'
  },
  clearButton: {
    id: 'Components.Buttons.ClearButton',
    defaultMessage: 'Tyhjennä'
  },
  searchButton: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
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

const ChannelAddressSearchFrom = props => {
  const {
    dispatch,
    searchIsFetching,
    reset,
    intl: { formatMessage },
    submit,
    isReadOnly,
    isAddingNewLanguage,
    searchEnabled,
    searchExists,
    resultExists,
    total,
    count
  } = props

  const _handleSubmit = e => {
    e.preventDefault()
    submit()
  }
  const _handleOnClear = e => {
    e.preventDefault()
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['channel', 'addresses', formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM]
    })
    reset()
  }

  const showResult = searchExists && resultExists && !searchIsFetching
  const showEmpty = searchExists && !resultExists && !searchIsFetching
  return (
    <div>
      {!isReadOnly && !isAddingNewLanguage &&
        <div>
          <div>
            <div className={showResult ? styles.searchForm : styles.searchFormEmpty}>
              <div className={styles.searchFormRow}>
                <div className='row'>
                  <div className='col-lg-8'>
                    <Street
                      addressUseCase={addressUseCasesEnum.VISITING}
                      tooltip={null}
                      skipValidation
                    />
                  </div>
                  <div className='col-lg-4'>
                    <AddressNumber skipValidation />
                  </div>
                  <div className='col-lg-6'>
                    <PostalCode
                      streetIdField='street'
                      addressType={addressTypesEnum.STREET}
                      skipValidation />
                  </div>
                  <div className='col-lg-6'>
                    <PostOffice />
                  </div>
                </div>
              </div>
              <div className={styles.searchFormRow}>
                <Spacer marginSize='m10' />
              </div>
              <div className={styles.formActions}>
                <div className={styles.buttonGroup}>
                  <Button
                    small
                    onClick={_handleSubmit}
                    secondary
                    children={searchIsFetching && <Spinner /> || formatMessage(messages.searchButton)}
                    disabled={searchIsFetching || !searchEnabled}
                  />
                  <Button
                    small
                    onClick={_handleOnClear}
                    secondary
                    children={formatMessage(messages.clearButton)}
                  />
                </div>
                {showEmpty && <div>{formatMessage(messages.noSearchResults)}</div>}
                {showResult && <div>{formatMessage(messages.searchResults) + ` ${count}/${total}`}</div>}
              </div>
              <div className={styles.searchFormRow}>
                {showResult && <Spacer marginSize='m0' /> }
              </div>
            </div>
            {showResult && <SearchResult top />}
          </div>
        </div>
      }
    </div>
  )
}

ChannelAddressSearchFrom.propTypes = {
  submit: PropTypes.func.isRequired,
  searchIsFetching: PropTypes.bool.isRequired,
  intl: intlShape.isRequired,
  reset: PropTypes.func,
  searchEnabled: PropTypes.bool,
  dispatch: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isAddingNewLanguage: PropTypes.bool,
  searchExists: PropTypes.bool,
  resultExists: PropTypes.bool,
  total: PropTypes.any,
  count: PropTypes.any
}

const onSubmit = (values, dispatch) => {
  dispatch(({ dispatch, getState }) => {
    dispatch(
      loadAddresses(getState(), dispatch, values, formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM)
    )
  })
}

export default compose(
  connect((state, ownProps) => {
    const getFormValues = formValueSelector(formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM)
    const props = { formName: formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM, ...ownProps }
    return {
      searchIsFetching: getAddressSearchIsFetching(state, props),
      searchEnabled: getFormValues(state, 'streetName') &&
        getFormValues(state, 'streetNumber') &&
        getFormValues(state, 'postalCode'),
      searchExists: !!isSearchExists(state, props),
      resultExists: isAddressSearchResultExists(state, props),
      total: getAddressSearchTotal(state, props),
      count: getAddressSearchCount(state, props)
    }
  }, {
    mergeInUIState
  }),
  injectIntl,
  reduxForm({
    form: formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM,
    onSubmit
  }),
  injectFormName,
  withFormStates,
  withPath
)(ChannelAddressSearchFrom)
