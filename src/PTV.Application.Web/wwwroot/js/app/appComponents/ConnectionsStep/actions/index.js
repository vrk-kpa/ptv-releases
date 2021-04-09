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
import { EntitySchemas } from 'schemas'
import {
  entityTypesEnum,
  formTypesEnum
} from 'enums'
import {
  getConnectionSearchIds,
  getConnectionSearchNextPageNumber
} from '../selectors'
import { getFormValues } from 'redux-form/immutable'
import { List } from 'immutable'
import { getSelectedLanguage } from 'Intl/Selectors'
import { getUiSortingData } from 'selectors/base'

const getSchema = searchMode => {
  let schema = EntitySchemas.SERVICE

  switch (searchMode) {
    case entityTypesEnum.SERVICES:
    case entityTypesEnum.GENERALDESCRIPTIONS:
      schema = EntitySchemas.CHANNEL
      break
    case entityTypesEnum.CHANNELS:
    case entityTypesEnum.SERVICECOLLECTIONS:
      schema = EntitySchemas.SERVICE
      break
  }

  return schema
}

export const loadConnectableEntities = (state, { searchMode, loadMoreEntities, sort, formName }) => {
  let entryKey = {
    [entityTypesEnum.SERVICES]: 'connectionsChannelSearch',
    [entityTypesEnum.GENERALDESCRIPTIONS]: 'connectionsChannelSearch',
    [entityTypesEnum.CHANNELS]: 'connectionsServiceSearch',
    [entityTypesEnum.SERVICECOLLECTIONS]: 'connectionsContentSearch'
  }[searchMode]
  const endpoint = {
    [entityTypesEnum.SERVICES]: 'channel/GetConnectableChannels',
    [entityTypesEnum.GENERALDESCRIPTIONS]: 'channel/GetConnectableChannels',
    [entityTypesEnum.CHANNELS]: 'service/GetConnectableServices',
    [entityTypesEnum.SERVICECOLLECTIONS]: 'serviceCollection/GetConnectableContent'
  }[searchMode]
  const entityType = {
    [entityTypesEnum.SERVICES]: entityTypesEnum.CHANNELS,
    [entityTypesEnum.GENERALDESCRIPTIONS]: entityTypesEnum.CHANNELS,
    [entityTypesEnum.CHANNELS]: entityTypesEnum.SERVICES,
    [entityTypesEnum.SERVICECOLLECTIONS]: EntitySchemas.GET_SEARCH(EntitySchemas.SEARCH)
  }[searchMode]
  const pageNumber = loadMoreEntities && getConnectionSearchNextPageNumber(state, { entryKey }) || 0
  const prevEntities = loadMoreEntities && getConnectionSearchIds(state, { entryKey, entityType }).toJS() || List()
  const language = getSelectedLanguage(state)
  let formValues = getFormValues(formTypesEnum.SEARCHCONNECTIONSFORM)(state)
  const sortingData = getUiSortingData(state, { contentType: formName })
  formValues = formValues.set('language', language).toJS()
  return apiCall3({
    keys: [entryKey],
    payload: {
      endpoint,
      data: {
        ...formValues,
        pageNumber,
        prevEntities,
        sortData: sortingData.size > 0 ? [sortingData] : []
      }
    },
    schemas: EntitySchemas.GET_SEARCH(getSchema(searchMode)),
    saveRequestData: true,
    clearRequest: ['prevEntities']
  })
}
