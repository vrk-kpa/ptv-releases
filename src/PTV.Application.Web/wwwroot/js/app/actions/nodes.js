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
import { apiCall3, clearApiCall } from 'actions'
import { getIntlLocale } from 'Intl/Selectors'

// move to selectors
const getTreeApiPath = (state, entityId, keyToState, treeType) => {
  return entityId ? [treeType, entityId, 'search'] : [treeType, 'search']
}

export const searchInTree = ({ searchSchema, treeType, value, id, keyToState, contextId }) =>
  ({ dispatch, getState }) => {
    dispatch(apiCall3({
      keys: getTreeApiPath(getState(), contextId, keyToState, treeType),
      payload: {
        endpoint: 'service/GetFilteredTree',
        data: {
          id,
          searchValue: value,
          treeType: treeType,
          languages: [getIntlLocale(getState())]
        }
      },
      schemas: searchSchema,
      saveRequestData: true
    }))
  }

export const searchInList = ({ searchSchema, treeType, value, keyToState, contextId, languageCode }) =>
  ({ dispatch, getState }) => {
    dispatch(apiCall3({
      keys: getTreeApiPath(getState(), contextId, keyToState, treeType),
      payload: {
        endpoint: 'service/GetFilteredList',
        data: {
          searchValue: value,
          treeType: treeType,
          languages: [languageCode]
        }
      },
      schemas: searchSchema,
      saveRequestData: true
    }))
  }

export const clearTreeSearch = (treeType, contextId, keyToState) =>
  ({ dispatch, getState }) => {
    dispatch(clearApiCall(
      getTreeApiPath(getState(), contextId, keyToState, treeType),
      { model: { result: null, id: null, searchValue: null } }
    ))
  }

export const loadNodeChildren = ({ treeNodeSchema, treeType, node, keyToState, contextId }) =>
  ({ dispatch, getState }) => {
    dispatch(apiCall3({
      keys: ['nodes', node.get('id')],
      payload: { endpoint: 'service/GetFintoTree', data: { treeItem: node, treeType: treeType } },
      schemas: treeNodeSchema
    }))
  }
