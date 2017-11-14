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
import { handleActions } from 'redux-actions'
import { Map } from 'immutable'

export const LOAD_MAP = 'LOAD_MAP'
export const MAP_IS_LOADED = 'MAP_IS_LOADED'
export const ENABLE_MAP_EDIT = 'ENABLE_MAP_EDIT'
export const DISABLE_MAP_EDIT = 'DISABLE_MAP_EDIT'

export const loadMap = (
  id
  ) => (
  {
    type: LOAD_MAP,
    payload: id
  }
)

export const mapIsLoaded = (
  id
  ) => (
  {
    type: MAP_IS_LOADED,
    payload: id
  }
)

export const enableMapEdit = (
  id
  ) => (
  {
    type: ENABLE_MAP_EDIT,
    payload: id
  }
)

export const disableMapEdit = (
  id
  ) => (
  {
    type: DISABLE_MAP_EDIT,
    payload: id
  }
)

export const initialState = Map()

export default handleActions({
  [LOAD_MAP]: (state, { payload }) => state.setIn([payload, 'isLoading'], true),
  [MAP_IS_LOADED]: (state, { payload }) => state.setIn([payload, 'isLoading'], false),
  [ENABLE_MAP_EDIT]: (state, { payload }) => state.setIn([payload, 'isEditEnabled'], true),
  [DISABLE_MAP_EDIT]: (state, { payload }) => state.setIn([payload, 'isEditEnabled'], false)
}, initialState)
