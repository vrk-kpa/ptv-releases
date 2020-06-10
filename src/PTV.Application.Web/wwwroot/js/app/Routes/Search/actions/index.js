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
import { apiCall3 } from 'actions'
import {
  getFormValues,
  change
} from 'redux-form/immutable'
import { EntitySchemas } from 'schemas'
import { List, Map, fromJS, OrderedSet } from 'immutable'
import { getTranslatableLanguages } from 'Routes/Search/selectors'
import {
  getSearchDomain,
  getDomainSearchIds,
  getDomainSearchSkip,
  getDomainSearchPageNumber
} from '../selectors'
import { EntitySelectors } from 'selectors'
import { getUiSortingData } from 'selectors/base'
import { getLanguageCode } from 'selectors/common'
import {
  formTypesEnum,
  contentTypes,
  searchContentTypeEnum,
  serviceContentTypes,
  channelContentTypes,
  publishingStatusCodesEnum
} from 'enums'

import { COMMON_NOTIFICATION_KEY } from 'reducers/notifications'

// Helpers //
const _createApiCall = (searchDomain, data, endpoint, schemas, canAbort = false, requestProps) => apiCall3({
  keys: ['frontPageSearch', searchDomain],
  payload: { endpoint, data },
  saveRequestData: true,
  clearRequest: ['prevEntities', 'services'],
  schemas,
  formName: COMMON_NOTIFICATION_KEY,
  canAbort,
  requestProps
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

export const removeSearchFilter = (value, filterName, formName) => ({ dispatch, getState }) => {
  const formValues = getFormValues(formName)(getState())

  if (!formValues.get(filterName)) {
    console.warn('Unknown filter name')
    return
  }

  const isLast = (formValues.get(filterName) || Map()).size === 1
  switch (filterName) {
    case 'languages':
      const code = getLanguageCode(getState(), { languageId: value })
      const translatedLanguagesCodes = getTranslatableLanguages(getState())
      dispatch(
        change(
          formName,
          filterName,
          isLast ? translatedLanguagesCodes : formValues.get(filterName).filter(f => f !== code)
        )
      )
      break
    case 'selectedPublishingStatuses':
      const isLastStatus = formValues.get(filterName).filter(s => s).size === 1
      const allStatusCodesList = List([
        publishingStatusCodesEnum.DRAFT,
        publishingStatusCodesEnum.PUBLISHED,
        publishingStatusCodesEnum.DELETED
      ])
      const allStatusMap = EntitySelectors.publishingStatuses.getEntities(getState())
        .filter((status, key) => allStatusCodesList.includes(status.get('code')))
        .map((status, key) => true)
      const newStatusMap = isLastStatus
        ? allStatusMap
        : formValues.get(filterName).update(value, status => !status)
      dispatch(
        change(
          formName,
          filterName,
          newStatusMap
        )
      )
      break
    case 'contentTypes':
      const filterValues = {
        [value]: value,
        [searchContentTypeEnum.SERVICE]: serviceContentTypes,
        [searchContentTypeEnum.CHANNEL]: channelContentTypes
      }[value]
      const newContentTypeList = formValues.get(filterName).filter(f => !filterValues.includes(f))
      const allContentTypeList = fromJS(contentTypes.filter(type => (
        type !== searchContentTypeEnum.SERVICE && type !== searchContentTypeEnum.CHANNEL
      ))).toOrderedSet()
      const compareCount = value === searchContentTypeEnum.SERVICE
        ? serviceContentTypes.length
        : value === searchContentTypeEnum.CHANNEL
          ? channelContentTypes.length
          : 1
      const isLastContent = formValues.get(filterName).filter(s => s).size === compareCount
      dispatch(
        change(
          formName,
          filterName,
          isLastContent ? allContentTypeList : newContentTypeList
        )
      )
      break
    default:
      dispatch(
        change(
          formName,
          filterName,
          isLast ? OrderedSet() : formValues.get(filterName).filter(f => f !== value)
        )
      )
      break
  }
}
