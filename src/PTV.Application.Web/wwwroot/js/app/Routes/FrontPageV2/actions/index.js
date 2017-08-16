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
import { apiCall } from 'Containers/Common/Actions'
import { getFormValues } from 'redux-form/immutable'
import { ServiceSchemas } from 'Containers/Services/Service/Schemas'
import { CommonServiceSchemas } from 'Containers/Services/Common/Schemas'
import { CommonChannelsSchemas } from 'Containers/Channels/Common/Schemas'
import { CommonSchemas } from 'Containers/Common/Schemas'
import { GeneralDescriptionSchemas } from 'Containers/Manage/GeneralDescriptions/GeneralDescriptions/Schemas'
import { OrganizationSchemas } from 'Containers/Manage/Organizations/Organization/Schemas'
import { List } from 'immutable'
import {
  getSearchDomain,
  getDomainSearchIds,
  getDomainSearchSkip,
  getDomainSearchPageNumber
} from '../selectors'
// Helpers //
const _createApiCall = (searchDomain, data, endpoint, schemas, enumSchemas = []) => apiCall(
  ['frontPageSearch', searchDomain],
  { endpoint, data },
  enumSchemas,
  schemas,
  undefined,
  undefined,
  true
)
const _createFetchMoreApiCall = (state, endpoint, schemas, additionalData) => {
  const searchDomain = getSearchDomain(state)
  const previousSearchIds = getDomainSearchIds(state)
  const values = getFormValues('frontPageSearch')(state)
  const skip = getDomainSearchSkip(state)
  const pageNumber = getDomainSearchPageNumber(state)
  return _createApiCall(
    searchDomain,
    {
      ...values.toJS(),
      ...additionalData,
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
  return apiCall(
    ['frontPageSearch', 'form'],
    { endpoint:'common/GetFrontPageSearch' },
    [
      CommonSchemas.PUBLISHING_STATUS_ARRAY,
      OrganizationSchemas.ORGANIZATION_ARRAY_GLOBAL,
      CommonSchemas.SERVICE_CLASS_ARRAY,
      CommonServiceSchemas.SERVICE_TYPE_ARRAY,
      CommonSchemas.TARGET_GROUP_ARRAY,
      CommonSchemas.PHONE_NUMBER_TYPE_ARRAY,
      CommonSchemas.CHARGE_TYPE_ARRAY
    ],
    [],
  )
}

// Service actions //
export const fetchServices = values => _createApiCall(
  'services',
  { ...values, services: List(), prevEntities: List() },
  'service/SearchServices',
  [ServiceSchemas.SERVICE_ARRAY]
)
export const fetchMoreServices = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'service/SearchServices',
      [ServiceSchemas.SERVICE_ARRAY]
    )
  )
}
// Channel actions //
export const fetchChannels = values => _createApiCall(
  values.channelType,
  { ...values, channels: List(), prevEntities: List() },
  'channel/ChannelSearchResult',
  [CommonChannelsSchemas.CHANNEL_ARRAY]
)
export const fetchMoreChannels = ({ dispatch, getState }) => {
  const state = getState()
  dispatch(
    _createFetchMoreApiCall(
      state,
      'channel/ChannelSearchResult',
      [CommonChannelsSchemas.CHANNEL_ARRAY],
      { channelType: getSearchDomain(state) }
    )
  )
}
// General description actions //
export const fetchGeneralDescriptions = values => _createApiCall(
  'generalDescriptions',
  { ...values, generalDescriptions: List(), prevEntities: List() },
  'generaldescription/v2/SearchGeneralDescriptions',
  [GeneralDescriptionSchemas.GENERAL_DESCRIPTION_ARRAY]
)
export const fetchMoreGeneralDescriptions = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'generaldescription/v2/SearchGeneralDescriptions',
      [GeneralDescriptionSchemas.GENERAL_DESCRIPTION_ARRAY]
    )
  )
}
// Organization actions //
export const fetchOrganizations = values => _createApiCall(
  'organizations',
  { ...values, organizations: List(), prevEntities: List() },
  'organization/SearchOrganizations',
  [OrganizationSchemas.ORGANIZATION_ARRAY],
)
export const fetchMoreOrganizations = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'organization/SearchOrganizations',
      [OrganizationSchemas.ORGANIZATION_ARRAY],
    )
  )
}

export const UNTOUCH_ALL_FRONT_PAG_SEARCH = "UNTOUCH_ALL_FRONT_PAG_SEARCH"
export const untouchAll = ()=> ({
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
