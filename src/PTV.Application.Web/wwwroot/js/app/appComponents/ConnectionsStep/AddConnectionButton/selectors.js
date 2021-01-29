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
import { getFormValues } from 'redux-form/immutable'
import { createSelector } from 'reselect'
import { List } from 'immutable'
import { getParameterFromProps } from 'selectors/base'

// regular connections
export const getSelectedConnections = createSelector(
  getFormValues('connections'),
  formValues => {
    if (!formValues) {
      return List()
    }
    const connections = formValues.get('selectedConnections') || List()
    const services = formValues.get('selectedServices') || List()
    const channels = formValues.get('selectedChannels') || List()
    return connections.concat(services).concat(channels)
  })

const getSelectedConnectionsIds = createSelector(
  getSelectedConnections,
  selectedConnections => (
    selectedConnections &&
    selectedConnections.map(connection => connection.get('id'))
  ) || List()
)

export const getIsSelected = createSelector(
  [getSelectedConnectionsIds, getParameterFromProps('id')],
  (selectedIds, id) => (
    selectedIds &&
    selectedIds.some(selectedId => selectedId === id)
  ) || false
)
