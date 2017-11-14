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
import { createSelector } from 'reselect'
import { Map, List, Set } from 'immutable'
import { EntitySelectors } from 'selectors'
import { getApiCalls, getParameterFromProps, getFormStates } from 'selectors/base'
import { getEntity, getSelectedEntityId } from 'selectors/entities/entities'
import { EditorState, convertFromRaw } from 'draft-js'
import { getFormValues } from 'redux-form/immutable'

export const getChannelType = createSelector(
  [EntitySelectors.channelTypes.getEntities, getParameterFromProps('input')],
  (channelTypes, input) => {
    const channelTypeId = input.value.get('channelTypeId')
    return channelTypeId && channelTypes.get(channelTypeId) || Map()
  }
)

export const getChannelCode = createSelector(
  getChannelType,
  channelType => channelType.get('code') || ''
)

export const getConnectionSubmiting = createSelector(
  getApiCalls,
  apiCalls => apiCalls.getIn(['connectionsSubmiting', 'isFetching']) || false
)

export const getConnections = createSelector(
  getEntity,
  entity => entity.get('connections') || List()
)

export const getSelectedConnections = createSelector(
  [getConnections, EntitySelectors.connections.getEntities],
  (ids, connections) => {
    const selectedConnections = ids
      .map(id => connections.get(id))
      .map(connection => {
        const astiDetails = Map()
        // isAsti //
          .set('isASTIConnection', connection.getIn(['astiDetails', 'isASTIConnection']) || false)
        // Basic Information //
        const basicInformation = Map()
          // Description //
          .set('description', connection.getIn(['basicInformation', 'description']))
          // Additioanal Information //
          .set('additionalInformation', connection.getIn(['basicInformation', 'additionalInformation']) || null)
          // Charge Type //
          .set('chargeType', connection.getIn(['basicInformation', 'chargeType']) || null)
        // Digital Authorization //
        const digitalAuthorization = Map()
          // Digital Authorizations //
          .update('digitalAuthorizations', () => {
            const digitalAuthorizations = connection.getIn(['digitalAuthorization', 'digitalAuthorizations'])
            return digitalAuthorizations.toOrderedSet()
          })
        const keyIn = keys => {
          const keySet = Set(keys)
          return (v, k) => keySet.has(k)
        }
        return Map({
          basicInformation,
          digitalAuthorization,
          astiDetails
        }).mergeDeep(
          connection.filter(keyIn([
            'id',
            'astiDetails',
            'unificRootId',
            'name',
            'languagesAvailabilities',
            'organizationId',
            'channelType',
            'channelTypeId',
            'serviceTypeId',
            'modified',
            'modifiedBy'
          ]))
        )
      })
    return selectedConnections || List()
  }
)

export const getConnectionsFormInitialValues = createSelector(
  [getSelectedEntityId, getSelectedConnections],
  (selectedEntityId, selectedConnections) => Map({
    id: selectedEntityId,
    selectedConnections: selectedConnections
  })
)

export const isConnectionRowReadOnly = index => createSelector(
  getFormStates,
  formStates => {
    const value = formStates.getIn(['connections', 'readOnlyIndex', index])
    return typeof value === 'undefined'
      ? true // If we have no value start as readOnly
      : value
  }
)
export const getIsAnyEntityConnected = createSelector(
  getFormValues('connections'),
  formValues => formValues && formValues.get('selectedConnections').size > 0 || false
)
export const isConnectionsReadOnly = createSelector(
  getFormStates,
  formStates => {
    const value = formStates.getIn(['connections', 'readOnly'])
    return typeof value === 'undefined'
      ? true // If we have no value start as readOnly
      : value
  }
)
