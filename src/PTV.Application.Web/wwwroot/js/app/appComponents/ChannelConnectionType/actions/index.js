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
import { entityTypesEnum } from 'enums'
import { EntitySchemas } from 'schemas'
import {
  getConnectedServices,
  getConnectionTypeNotCommonId
} from '../selectors'
import { mergeInUIState } from 'reducers/ui'
import { change } from 'redux-form/immutable'

export const loadConnectedServices = (id, formName) => ({ getState, dispatch }) => {
  dispatch(apiCall3({
    keys: [entityTypesEnum.CHANNELS, 'connectedItems'],
    payload: {
      endpoint: 'channel/GetConnectedServicesWithDifferentOrganization',
      data: { id }
    },
    schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE),
    formName,
    successNextAction: response => {
      const state = getState()
      const connectedServices = getConnectedServices(state)
      // hide the dialog and proceed with the change of channel connection type to 'not common'
      if (connectedServices.length === 0) {
        const notCommonId = getConnectionTypeNotCommonId(state)
        dispatch(change(formName, 'connectionType', notCommonId))
        dispatch(mergeInUIState({
          key: 'notCommonChannelDialog',
          value: {
            isOpen: false
          }
        }))
      }
    }
  }))
}
