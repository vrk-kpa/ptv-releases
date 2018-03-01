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
  getDomainSearchPageNumber,
  getUiSortingData
} from '../selectors'
import { COMMON_NOTIFICATION_KEY } from 'reducers/notifications'

// Helpers //
const _createApiCall = (searchDomain, data, endpoint, schemas, enumSchemas = null) => apiCall3({
  keys: ['frontPageSearch', searchDomain],
  payload: { endpoint, data },
  saveRequestData: true,
  clearRequest: ['prevEntities', 'services'],
  schemas,
  formName: COMMON_NOTIFICATION_KEY
})
const _createFetchMoreApiCall = (state, endpoint, schemas, additionalData) => {
  const searchDomain = getSearchDomain(state)
  const previousSearchIds = getDomainSearchIds(state)
  let values = getFormValues('frontPageSearch')(state)
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

export const loadFrontPageData = () => {
  return apiCall3({
    keys: ['frontPageSearch', 'form'],
    payload: { endpoint:'common/GetFrontPageSearch' },
    schemas: EntitySchemas.ENUM_COLLECTION,
    formName: COMMON_NOTIFICATION_KEY
  })
}

// Service actions //
export const fetchServices = values => _createApiCall(
  'services',
  { ...values, result: { services: List() }, prevEntities: List() },
  'service/SearchServices',
  EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE)
)
export const fetchMoreServices = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'service/SearchServices',
      EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE)
    )
  )
}
// Channel actions //
export const fetchChannels = values => _createApiCall(
  values.channelType,
  { ...values, result: { channels: List() }, prevEntities: List() },
  'channel/ChannelSearchResult',
  EntitySchemas.GET_SEARCH(EntitySchemas.CHANNEL)
)
export const fetchMoreChannels = ({ dispatch, getState }) => {
  const state = getState()
  dispatch(
    _createFetchMoreApiCall(
      state,
      'channel/ChannelSearchResult',
      EntitySchemas.GET_SEARCH(EntitySchemas.CHANNEL),
      { channelType: getSearchDomain(state) }
    )
  )
}
// General description actions //
export const fetchGeneralDescriptions = values => _createApiCall(
  'generalDescriptions',
  { ...values, result: { generalDescriptions: List() }, prevEntities: List() },
  'generaldescription/v2/SearchGeneralDescriptions',
  EntitySchemas.GET_SEARCH(EntitySchemas.GENERAL_DESCRIPTION)
)
export const fetchMoreGeneralDescriptions = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'generaldescription/v2/SearchGeneralDescriptions',
      EntitySchemas.GET_SEARCH(EntitySchemas.GENERAL_DESCRIPTION)
    )
  )
}
// Organization actions //
export const fetchOrganizations = values => _createApiCall(
  'organizations',
  { ...values, result: { organizations: List() }, prevEntities: List() },
  'organization/SearchOrganizations',
  EntitySchemas.GET_SEARCH(EntitySchemas.ORGANIZATION)
)
export const fetchMoreOrganizations = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'organization/SearchOrganizations',
      EntitySchemas.GET_SEARCH(EntitySchemas.ORGANIZATION)
    )
  )
}

export const UNTOUCH_ALL_FRONT_PAG_SEARCH = 'UNTOUCH_ALL_FRONT_PAG_SEARCH'
export const untouchAll = () => ({
  type: UNTOUCH_ALL_FRONT_PAG_SEARCH
})

export const SET_ENTITY_ID = 'SET_ENTITY_ID'
export const onRecordSelect = (id, keyToState, languageId) => {
  return {
    type: SET_ENTITY_ID,
    pageSetup: {
      id,
      keyToState,
      languageFrom: languageId
    }
  }
}

export const SET_LANGUAGE_TO = 'SET_LANGUAGE_TO'
export function setLanguageTo (keyToState, languageId, languageCode) {
  return () => ({
    type: SET_LANGUAGE_TO,
    payload: {
      keyToState,
      languageId,
      languageCode
    }
  })
}

export const loadMore = () => store => {
  const searchDomain = getSearchDomain(store.getState())
  switch (searchDomain) {
    case 'services':
      fetchMoreServices(store)
      break
    case 'eChannel':
    case 'webPage':
    case 'printableForm':
    case 'phone':
    case 'serviceLocation':
    case 'channels':
      fetchMoreChannels(store)
      break
    case 'generalDescriptions':
      fetchMoreGeneralDescriptions(store)
      break
    case 'organizations':
      fetchMoreOrganizations(store)
      break
  }
}
