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

// move to selectors
const getTreeApiPath = (state, entityId, keyToState, treeType) => {
  return entityId ? [treeType, entityId, 'search'] : [treeType, 'search']
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

export function searchInList ({ searchSchema, treeType, value, keyToState, contextId }) {
  return (props) => {
    return CommonActions.apiCall(
      getTreeApiPath(props.getState(), contextId, keyToState, treeType),
      { endpoint: 'service/GetFilteredList', data: { searchValue: value, treeType: treeType } },
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
    return CommonActions.apiCall(
      ['nodes', node.get('id')],
      { endpoint: 'service/GetFintoTree', data: { treeItem: node, treeType: treeType } },
      [],
      treeNodeSchema)(props)
  }
}
