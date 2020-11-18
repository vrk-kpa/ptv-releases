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
import { formTypesEnum } from 'enums'
import { Map } from 'immutable'
import { Button, Spinner } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { intlShape, injectIntl } from 'util/react-intl'
import { EntitySchemas } from 'schemas'
import { getGneralDescriptionSearchResultsIds } from 'Routes/Service/selectors'
import { getFormValues } from 'redux-form/immutable'
import { apiCall3 } from 'actions'
import { createSelector } from 'reselect'
import { getApiCalls, getUiSortingData } from 'selectors/base'
import { buttonMessages } from 'Routes/messages'
import { getSelectedLanguage } from 'Intl/Selectors'

class LoadMoreButton extends Component {
  get shouldShow () {
    return !this.props.isEmpty && this.props.isMoreAvailable
  }
  handleLoadMore = e => {
    e.preventDefault()
    this.props.loadMore()
  }
  render () {
    return this.shouldShow && (
      <div style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: '1em'
      }}>
        {this.props.isLoadingMore
          ? <Spinner />
          : <Button
            children={`${this.props.intl.formatMessage(buttonMessages.showMore)}`}
            onClick={this.handleLoadMore}
            small secondary
          />}
      </div>
    )
  }
}
LoadMoreButton.propTypes = {
  intl: intlShape,
  loadMore: PropTypes.func,
  isLoadingMore: PropTypes.bool,
  isEmpty: PropTypes.bool
}

const getApiCall = createSelector(
  getApiCalls,
  apiCalls => {
    return apiCalls.getIn(['service', 'generalDescription', 'search']) || Map()
  }
)
const getSkip = createSelector(
  getApiCall,
  apiCall => {
    return apiCall.getIn(['result', 'skip']) || 0
  }
)
const getPageNumber = createSelector(
  getApiCall,
  apiCall => {
    return apiCall.getIn(['result', 'pageNumber']) || 0
  }
)
const getIsMoreAvailable = createSelector(
  getApiCall,
  apiCall => {
    return !!apiCall.getIn(['result', 'moreAvailable'])
  }
)
const getIsLoadingMore = createSelector(
  [getApiCall, getGneralDescriptionSearchResultsIds],
  (apiCall, resultIds) => {
    const isFetching = !!apiCall.get('isFetching')
    const isEmpty = resultIds.size === 0
    return isFetching && !isEmpty
  }
)
const getIsEmpty = createSelector(
  getGneralDescriptionSearchResultsIds,
  resultIds => resultIds.size === 0
)

export default compose(
  injectIntl,
  connect(
    state => ({
      sortingData: getUiSortingData(
        state,
        { contentType: 'generalDescriptionSearch' }
      ),
      isLoadingMore: getIsLoadingMore(state),
      isMoreAvailable: getIsMoreAvailable(state),
      isEmpty: getIsEmpty(state)
    }), {
      loadMore: () => ({ getState, dispatch }) => {
        const state = getState()
        const sortingData = getUiSortingData(
          state,
          { contentType: formTypesEnum.GENERALDESCRIPTIONSEARCHFORM }
        )
        const values = getFormValues('generalDescriptionSearchForm')(state)
        const pageNumber = getPageNumber(state)
        const skip = getSkip(state)
        const previousData = getGneralDescriptionSearchResultsIds(state).toJS()
        const lang = getSelectedLanguage(state)
        dispatch(
          apiCall3({
            payload: {
              endpoint: 'generaldescription/v2/SearchGeneralDescriptions',
              data: {
                name: values.get('name'),
                languages: [lang],
                serviceTypeId: values.get('serviceType'),
                generalDescriptionTypeId: values.get('generalDescriptionType'),
                sortData: sortingData.size > 0 ? [sortingData] : [],
                pageNumber,
                skip,
                previousData
              }
            },
            keys: ['service', 'generalDescription', 'search'],
            schemas: EntitySchemas.GET_SEARCH(EntitySchemas.GENERAL_DESCRIPTION),
            formName: formTypesEnum.SERVICEFORM,
            saveRequestData: true,
            clearRequest: ['previousData']
          })
        )
      }
    }
  )
)(LoadMoreButton)
