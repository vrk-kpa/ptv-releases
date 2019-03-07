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
import { EntitySchemas } from 'schemas'
import {
  getAddressSearchPageNumber,
  getAddressSearchResultsIds
} from '../selectors'
import { getUiSortingData } from 'selectors/base'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'
import { Map } from 'immutable'
import { getContentLanguageCode } from 'selectors/selections'

export const loadMoreAddresses = (state, formValues, formName) => {
  // const state = getState()
  const form = formName || formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM
  const pageNumber = getAddressSearchPageNumber(state, { formName:form })
  const prevEntities = getAddressSearchResultsIds(state, { formName:form }).toJS()
  const sortingData = getUiSortingData(state, { contentType: form })
  let values = formValues || getFormValues(form)(state)
  const languageCode = getContentLanguageCode(state)
  return apiCall3({
    keys: ['channel', 'addresses', form],
    payload: {
      endpoint: 'channel/ServiceLocationChannelsSearch',
      data: {
        pageNumber: pageNumber + 1,
        prevEntities,
        sortData: sortingData.size > 0 ? [sortingData] : [],
        languageCode,
        ...values.toJS()
      }
    },
    schemas: EntitySchemas.GET_SEARCH(EntitySchemas.ADDRESS),
    saveRequestData: true,
    clearRequest: ['prevEntities']
  })
}

export const loadAddresses = (state, dispatch, formValues, formName, sort) => {
  const form = formName || formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM
  const sortingData = getUiSortingData(state, { contentType: form })
  let values = formValues || getFormValues(form)(state) || Map()
  const languageCode = getContentLanguageCode(state)
  if (!sort) {
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['channel', 'addresses', form]
    })
  } else {
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['channel', 'addresses', form, 'prevEntities']
    })
  }
  return apiCall3(
    {
      payload: {
        endpoint:
        'channel/ServiceLocationChannelsSearch',
        data: {
          street: values.get('street'),
          streetNumber: values.get('streetNumber'),
          postalCode: values.get('postalCode'),
          languageCode,
          sortData: sortingData.size > 0 ? [sortingData] : []
        }
      },
      keys: ['channel', 'addresses', form],
      schemas: EntitySchemas.GET_SEARCH(EntitySchemas.ADDRESS),
      formName: formTypesEnum.SERVICEFORM,
      saveRequestData: true
    })
}

