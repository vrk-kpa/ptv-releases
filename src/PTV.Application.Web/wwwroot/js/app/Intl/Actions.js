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
import { CALL_API, API_WEB_ROOT } from '../Middleware/Api'
import { IntlSchemas } from './Schemas'
import { apiCall2 } from 'Containers/Common/Actions'

/*
 * action types
 */

export const CHANGE_LANGUAGE = 'CHANGE_LANGUAGE'
export const TRANSLATED_DATA_CHANGED = 'TRANSLATED_DATA_CHANGED'

/*
 * action creators
 */

export function changeLanguage (language) {
  return {
    type: CHANGE_LANGUAGE,
    payload: { language }
  }
}

export function translatedDataChanged (data) {
  return {
    type: TRANSLATED_DATA_CHANGED,
    payload: { data }
  }
}

export const INTL_GET_TRANSLATED_DATA_REQUEST = 'INTL_GET_TRANSLATED_DATA_REQUEST'
export const INTL_GET_TRANSLATED_DATA_SUCCESS = 'INTL_GET_TRANSLATED_DATA_SUCCESS'
export const INTL_GET_TRANSLATED_DATA_FAILURE = 'INTL_GET_TRANSLATED_DATA_FAILURE'

export function getTranslatedData (id, data) {
  return {
    // id,
    [CALL_API]: {
      types: [INTL_GET_TRANSLATED_DATA_REQUEST, INTL_GET_TRANSLATED_DATA_SUCCESS, INTL_GET_TRANSLATED_DATA_FAILURE],
      payload: {
        endpoint: 'localization/GetTypeData'
        // data: data
      },
      schemas: IntlSchemas.TRANSLATED_ITEM_ARRAY
    }
  }
}

export const getMessages = () => {
  return apiCall2({
    keys: 'localizationMessages',
    payload: { endpoint: API_WEB_ROOT + 'GetMessages' },
    schemas: IntlSchemas.LOCALIZATION_MESSAGES_ARRAY
  })
}
