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
import { apiCall3 } from 'actions'
import { getFormValues } from 'redux-form/immutable'
import { EntitySchemas } from 'schemas'
import { List } from 'immutable'
import {
  getSearchDomain,
  getDomainSearchIds,
  getDomainSearchSkip,
  getDomainSearchPageNumber
} from '../selectors'
import { getUiSortingData } from 'selectors/base'
import { formTypesEnum } from 'enums'

import { COMMON_NOTIFICATION_KEY } from 'reducers/notifications'

// Helpers //
const _createApiCall = (searchDomain, data, endpoint, schemas, canAbort = false) => apiCall3({
  keys: ['frontPageSearch', searchDomain],
  payload: { endpoint, data },
  saveRequestData: true,
  clearRequest: ['prevEntities', 'services'],
  schemas,
  formName: COMMON_NOTIFICATION_KEY,
  canAbort
})
const _createFetchMoreApiCall = (state, endpoint, schemas, additionalData) => {
  const searchDomain = getSearchDomain(state)
  const previousSearchIds = getDomainSearchIds(state)
  let values = getFormValues(formTypesEnum.FRONTPAGESEARCH)(state)
  const selectedPublishingStatuses = values.get('selectedPublishingStatuses')
    .filter(value => value)
    .keySeq()
  values = values.set('selectedPublishingStatuses', selectedPublishingStatuses)
  const skip = getDomainSearchSkip(state)
  const pageNumber = getDomainSearchPageNumber(state)
  const sortingData = getUiSortingData(state, { contentType: searchDomain })
  return _createApiCall(
    searchDomain,
    {
      ...values.toJS(),
      ...additionalData,
      sortData: sortingData.size > 0 ? [sortingData] : [],
      skip,
      pageNumber,
      [searchDomain]: List(),
      prevEntities: previousSearchIds.toJS()
    },
    endpoint,
    schemas
  )
}

// entities actions

export const fetchEntities = values => _createApiCall(
  'entities',
  { ...values, result: { entities: List() }, prevEntities: List() },
  'common/SearchEntities',
  EntitySchemas.GET_SEARCH(EntitySchemas.SEARCH),
  true
)
export const fetchMoreEntities = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'common/SearchEntities',
      EntitySchemas.GET_SEARCH(EntitySchemas.SEARCH)
    )
  )
}

export const loadMore = () => store => {
  fetchMoreEntities(store)
}

export const UNTOUCH_FRONTPAGE_FORM = 'UNTOUCH_FRONTPAGE_FORM'
export const untouchAll = () => ({ type: UNTOUCH_FRONTPAGE_FORM })
