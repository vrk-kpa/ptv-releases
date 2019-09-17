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
import { createStore, applyMiddleware } from 'redux'
import reducers from 'Configuration/reducer'
import promiseMiddleware from 'redux-promise-middleware'
import apiMiddleware from 'Middleware/Api'
import throttle from 'lodash/throttle'
import { composeWithDevTools } from 'redux-devtools-extension'
import { loadState, saveState } from 'Configuration/AppHelpers'
import { getSelectedLanguage } from 'Intl/Selectors'
import { connectRouter, routerMiddleware } from 'connected-react-router/immutable'
import { fromJS } from 'immutable'

const thunk = ({ dispatch, getState }) => next => action => {
  if (typeof action === 'function') {
    action = action({ dispatch, getState })
  }
  if (action) {
    // console.log('call: ', action)
    return next(action)
  }
}

const logger = store => next => action => {
  const before = store.getState().toJS()
  console.log(action && action.type, action, before)
  const result = next(action)
  console.log(`next state for ${action.type}`, store.getState().toJS())
  return result
}

const initialState = fromJS(loadState() || {})
const configureStore = ({
  history
}) => {
  const store = createStore(
    connectRouter(history)(reducers),
    initialState,
    composeWithDevTools({
      actionsBlacklist: [
        'SET_IN_UI_STATE',
        'API_CALL_REQUEST',
        'API_CALL_SUCCESS',
        '@@redux-form/REGISTER_FIELD',
        '@@redux-form/UNREGISTER_FIELD'
      ]
    })(
      applyMiddleware(
        routerMiddleware(history),
        thunk,
        apiMiddleware,
        promiseMiddleware({
          promiseTypeSuffixes: ['START', 'SUCCES', 'ERROR']
        })
        // , logger
      )
    )
  )
  window.store = store
  store.subscribe(throttle(() => {
    const state = store.getState()
    saveState({
      intl: {
        selectedLanguage: getSelectedLanguage(state)
      }
    })
  }, 2000))
  return store
}

export default configureStore
