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
const _createFetchMoreApiCall = (state, endpoint, schemas) => {
  const searchDomain = getSearchDomain(state)
  const previousSearchIds = getDomainSearchIds(state)
  const values = getFormValues('frontPageSearch')(state)
  const skip = getDomainSearchSkip(state)
  const pageNumber = getDomainSearchPageNumber(state)
  return _createApiCall(
    searchDomain,
    {
      ...values.toJS(),
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
  { ...values, services: List() },
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
  { ...values, channels: List() },
  'channel/ChannelSearchResult',
  [CommonChannelsSchemas.CHANNEL_ARRAY]
)
export const fetchMoreChannels = ({ dispatch, getState }) => {
  dispatch(
    _createFetchMoreApiCall(
      getState(),
      'channel/ChannelSearchResult',
      [CommonChannelsSchemas.CHANNEL_ARRAY]
    )
  )
}
// General description actions //
export const fetchGeneralDescriptions = values => _createApiCall(
  'generalDescriptions',
  { ...values, generalDescriptions: List() },
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
  { ...values, organizations: List() },
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
