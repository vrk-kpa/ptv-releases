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
import appReducer from './reducer'
import { applyMiddleware, compose, createStore } from 'redux'
import shortid from 'shortid'
import promiseMiddleware from 'redux-promise-middleware'
import apiMiddleware from '../Middleware/Api'
import { loadState, saveState } from './AppHelpers'
import throttle from 'lodash/throttle'
import { storeEnhancer as bugReporter } from 'util/redux-bug-reporter'
import { getSelectedLanguage } from 'Intl/Selectors'

// NODE_ENV is not working at the moment //
let shouldShowBugReporter = window.isBugReportAvailable()

const injectMiddleware = deps => ({ dispatch, getState }) => next => action => {
  if (typeof action === 'function') {
    const toDispatch = action({ ...deps, dispatch, getState })
    if (toDispatch) {
      return next(toDispatch)
    }
  } else {
    return next(action)
  }
}

export default function configureStore (options) {
  const {
    initialState = loadState(),
    platformDeps = {},
    platformMiddleware = []
  } = options

  const middleware = [
    ...platformMiddleware,
    injectMiddleware({
      ...platformDeps,
      getUid: () => shortid.generate(),
      now: () => Date.now()
    }),
    apiMiddleware,
    promiseMiddleware({
      promiseTypeSuffixes: [
        'START',
        'SUCCESS',
        'ERROR'
      ]
    })
  ]

  const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
    ? window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
    : compose
  const createStoreWithMiddleware =
    composeEnhancers(
      shouldShowBugReporter
        ? bugReporter
        : _ => _,
      applyMiddleware(...middleware),
    )

  const store = createStoreWithMiddleware(createStore)(appReducer, initialState)

  store.subscribe(throttle(() => {
    saveState({
      intl: {
        selectedLanguage: getSelectedLanguage(store.getState())
      }
    })
  }, 2000))

  return store
}
