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
import { List, Map } from 'immutable'
import { getFormValues } from 'redux-form/immutable'

const getSelectedConnections = createSelector(
  getFormValues('connections'),
  formValues => (formValues && formValues.get('selectedConnections')) || List()
)

export const getServiceCollectionConnectionCount = createSelector(
  getFormValues('connections'),
  connections => {
    const services = connections && connections.get('selectedServices') || List()
    const channels = connections && connections.get('selectedChannels') || List()
    return services.size + channels.size
  }
)

const getSelectedASTIConnections = createSelector(
  getFormValues('ASTIConnections'),
  formValues => (formValues && formValues.get('connectionsByOrganizations')) || Map()
)

export const getEntityConnectionCount = createSelector(
  getSelectedConnections,
  connections => connections.size || null
)
export const getSelectedASTIConnectionsCount = createSelector(
  getSelectedASTIConnections,
  connections => {
    return connections.reduce((count, organizationConnections) => {
      return count + organizationConnections.size
    }, 0) || 0
  }
)

export const getSelectedFlatASTIConnections = createSelector(
  getFormValues('ASTIConnections'),
  formValues => (formValues && formValues.get('connectionsFlat')) || List()
)

export const getSelectedFlatASTIConnectionsCount = createSelector(
  getSelectedFlatASTIConnections,
  astiConnections => astiConnections.size || 0
)
