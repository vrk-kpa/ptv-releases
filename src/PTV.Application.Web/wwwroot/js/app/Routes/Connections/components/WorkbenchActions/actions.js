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
import { change } from 'redux-form/immutable'
import {
  setConnectionsEntity,
  setConnectionsMainEntity,
  clearConnectionsActiveEntity,
  setShouldAddChildToAllEntities,
  setConnectionsView
} from 'reducers/selections'
import { List, fromJS } from 'immutable'
import { API_CALL_CLEAN } from 'actions'
import {
  getMainEntityConnections,
  GetServiceResultJS,
  getResults
} from 'Routes/Connections/selectors'
import {
  makeCurrentFormStateInitial
} from 'Routes/Connections/actions'

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
    change('searchServicesConnections', 'fulltext', null),
    change('searchChannelsConnections', 'fulltext', null),
    setConnectionsMainEntity(null),
    setConnectionsEntity(null),
    setConnectionsView(false),
    clearConnectionsActiveEntity(),
    setShouldAddChildToAllEntities(true),
    clearChannelSearchResults(),
    clearServiceSearchResults(),
    makeCurrentFormStateInitial()
  ].forEach(dispatch)
}

const createItem = (item, dispatch) => {
  let childs
  dispatch(({ getState }) => {
    const state = getState()
    childs = getMainEntityConnections(state, { id:item.id })
  })
  const groupedChilds = childs.groupBy(child =>
    child.getIn(['astiDetails', 'isASTIConnection'])
      ? 'asti'
      : 'nonAsti'
  )
  return fromJS({
    mainEntity: item,
    childs: groupedChilds.get('nonAsti') || [],
    astiChilds: groupedChilds.get('asti') || []
  })
}

export const addItemToWorkBench = (index, dispatch) => {
  let childs
  let item
  dispatch(({ getState }) => {
    const state = getState()
    item = GetServiceResultJS(state, { index: index, contentType: 'summaryMainConnection' })
    childs = getMainEntityConnections(state, { id:item.id })
  })
  const groupedChilds = childs.groupBy(child =>
    child.getIn(['astiDetails', 'isASTIConnection'])
      ? 'asti'
      : 'nonAsti'
  )
  const resultItem = fromJS({
    mainEntity: item,
    childs: groupedChilds.get('nonAsti') || [],
    astiChilds: groupedChilds.get('asti') || []
  })
  dispatch(change('connectionsWorkbench', 'connections[0]', resultItem))
}

export const addAllItemsToWorkBench = (dispatch) => {
  let workResult
  dispatch(({ getState }) => {
    const state = getState()
    const allItems = getResults(state)
    workResult = allItems.map(v => createItem(v.toJS(), dispatch))
  })
  dispatch(change('connectionsWorkbench', 'connections', workResult))
}
