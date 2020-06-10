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
import { handleActions } from 'redux-actions'
import { Map } from 'immutable'

export const SET_ROW_LANGUAGE = 'SET_ROW_LANGUAGE'
export const INCREMENT_COUNT = 'INCREMENT_COUNT'
export const DECREMENT_COUNT = 'DECREMENT_COUNT'

export const setRowLanguage = (
  idRow,
  languageId,
  languageCode
) => (
  {
    type: SET_ROW_LANGUAGE,
    payload: { idRow, languageId, languageCode }
  }
)

export const initialState = Map({ counter: 0 })

export default handleActions({
  [SET_ROW_LANGUAGE]: (state, { payload }) => state
    .setIn([payload.idRow, 'languageCode'], payload.languageCode)
    .setIn([payload.idRow, 'languageId'], payload.languageId),
  [INCREMENT_COUNT]: (state, { payload }) => state
    .set('counter', state.get('counter') + 1),
  [DECREMENT_COUNT]: (state, { payload }) => state
    .set('counter', state.get('counter') - 1)
}, initialState)
