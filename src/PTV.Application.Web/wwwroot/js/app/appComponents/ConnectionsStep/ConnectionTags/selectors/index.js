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
import { createSelector } from 'reselect'
import {
  getEntitiesForIds,
  getParameterFromProps
} from 'selectors/base'
import {
  getSelectedEntityConcreteType,
  getEntity
} from 'selectors/entities/entities'
import { EntitySelectors } from 'selectors'
import { Map, List } from 'immutable'
import { getFormInitialValues } from 'redux-form/immutable'
import { entityConcreteTypesEnum } from 'enums'

export const getAttachedGeneralDescriptionId = createSelector(
  getEntity,
  entity => entity.get('generalDescriptionId') || null
)

export const getIsGeneralDescriptionAttached = createSelector(
  getAttachedGeneralDescriptionId,
  id => !!id || false
)

export const getGeneralDescription = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getAttachedGeneralDescriptionId],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionConnectionIds = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('connections') || List()
)

export const getGDConnectionsForIds = createSelector(
  [EntitySelectors.connections.getEntities, getGeneralDescriptionConnectionIds],
  (connections, connectionIds) => getEntitiesForIds(connections, connectionIds) || List()
)

export const getIsProposedChannel = createSelector(
  [getSelectedEntityConcreteType, getIsGeneralDescriptionAttached, getGDConnectionsForIds, getParameterFromProps('entityId'), getParameterFromProps('isSuggestedChannel')],
  (entityConcreteType, isGDAttached, connections, id, isSuggested) => entityConcreteType === entityConcreteTypesEnum.SERVICE &&
    isGDAttached && connections.some(connection => connection.get('id') === id) || !!isSuggested
)

const getEntityInitialConnections = createSelector(
  getFormInitialValues('connections'),
  formValues => (formValues && formValues.get('selectedConnections')) || Map()
)

export const getIsNewConnection = createSelector(
  [getEntityInitialConnections, getParameterFromProps('entityId')],
  (connections, id) => connections.every(connection => connection.get('id') !== id) || false
)
