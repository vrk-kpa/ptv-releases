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
import { change } from 'redux-form/immutable'
import {
  setConnectionsEntity,
  setConnectionsMainEntity,
  clearConnectionsActiveEntity,
  setShouldAddChildToAllEntities
} from 'reducers/selections'
import { List } from 'immutable'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'

const clearChannelSearchResults = () => ({
  type: API_CALL_CLEAN,
  keys: ['connections', 'channelSearch']
})
const clearServiceSearchResults = () => ({
  type: API_CALL_CLEAN,
  keys: ['connections', 'serviceSearch']
})

export const resetConnectionsWorkbench = () => ({ dispatch }) => {
  [
    change('connectionsWorkbench', 'connections', List()),
    change('connectionsWorkbench', 'mainEntityType', null),
    change('searchServicesConnections', 'fulltext', null),
    change('searchChannelsConnections', 'fulltext', null),
    setConnectionsMainEntity(null),
    setConnectionsEntity(null),
    clearConnectionsActiveEntity(),
    setShouldAddChildToAllEntities(true),
    clearChannelSearchResults(),
    clearServiceSearchResults()
  ].forEach(dispatch)
}
