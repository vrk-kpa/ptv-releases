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
import {
  getIsAlreadyLoaded,
  getVersioningId,
  getOperationIds,
  getPageNumber
} from './selectors'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { List } from 'immutable'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { entityTypesEnum } from 'enums'

export const loadConnectionHistory = (loadMore) => ({ dispatch, getState }) => {
  const state = getState()
  const isAlreadyLoaded = getIsAlreadyLoaded(state)
  if (loadMore || !isAlreadyLoaded) {
    const versioningId = getVersioningId(state)
    const entityType = getSelectedEntityType(state)
    const pageNumber = loadMore && getPageNumber(state) || 0
    const prevEntities = loadMore && getOperationIds(state).toJS() || List()
    let endpoint

    switch (entityType) {
      case entityTypesEnum.SERVICES:
        endpoint = `service/GetEntityHistory`
        break
      case entityTypesEnum.CHANNELS:
        endpoint = `channel/GetEntityHistory`
        break
      case entityTypesEnum.SERVICECOLLECTIONS:
        endpoint = `serviceCollection/GetEntityHistory`
        break
      case entityTypesEnum.ORGANIZATIONS:
        endpoint = `organization/GetEntityHistory`
        break
      case entityTypesEnum.GENERALDESCRIPTIONS:
        endpoint = `generalDescription/GetEntityHistory`
        break
      default:
        throw new Error(
          `EntityHistory: No endpoint for entityType ${entityType}`
        )
    }

    dispatch(
      apiCall3({
        keys: ['entityHistory', entityType, versioningId, 'connection'],
        payload: { endpoint,
          data: {
            id: versioningId,
            pageNumber,
            prevEntities
          }
        },
        schemas: EntitySchemas.GET_SEARCH(EntitySchemas.ENTITY_OPERATION),
        saveRequestData: true,
        clearRequest: ['prevEntities']
      })
    )
  }
}
