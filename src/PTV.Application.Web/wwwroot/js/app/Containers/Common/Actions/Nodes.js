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
import * as CommonActions from './'
import * as CommonSelectors from '../Selectors'

export const SEARCH_IN_TREE_REQUEST = 'SEARCH_IN_TREE_REQUEST'
export const SEARCH_IN_TREE_SUCCESS = 'SEARCH_IN_TREE_SUCCESS'
export const SEARCH_IN_TREE_FAILURE = 'SEARCH_IN_TREE_FAILURE'

// move to selectors
const getTreeApiPath = (state, entityId, keyToState, treeType) => {
  const pageEntityId = entityId || (keyToState ? CommonSelectors.getPageEntityId(state, { keyToState }) : null)
  return pageEntityId ? [treeType, pageEntityId, 'search'] : [treeType, 'search']
}

export function searchInTree ({ searchSchema, treeType, value, id, keyToState, contextId }) {
  return (props) => {
    return CommonActions.apiCall(
      getTreeApiPath(props.getState(), contextId, keyToState, treeType),
      { endpoint: 'service/GetFilteredTree', data: { id, searchValue: value, treeType: treeType } },
      [],
      searchSchema,
      null, null, true)(props)
  }
}

export const clearTreeSearch = (treeType, contextId, keyToState) => {
  return (props) => {
    return CommonActions.clearApiCall(
      getTreeApiPath(props.getState(), contextId, keyToState, treeType),
      { model: { result: null, id: null, searchValue: null } }
    )
  }
}

export function loadNodeChildren ({ treeNodeSchema, treeType, node, keyToState, contextId }) {
  return (props) => {
    const pageEntityId = contextId || CommonSelectors.getPageEntityId(props.getState(), { keyToState })
    return CommonActions.apiCall(
      ['nodes', node.get('id'), pageEntityId],
      { endpoint: 'service/GetFintoTree', data: { treeItem: node, treeType: treeType } },
      [],
      treeNodeSchema)(props)
  }
}

export const UPDATE_NODE = 'UPDATE_NODE'

export function updateNode (id, property, value) {
  const keyPath = Array.isArray(id) ? id : [id]
  return () => ({
    type: UPDATE_NODE,
    payload: {
      id: keyPath,
      property,
      value
    }
  })
}
