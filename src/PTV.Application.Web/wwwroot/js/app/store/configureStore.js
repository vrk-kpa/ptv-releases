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

const initialState = fromJS(loadState() || {})
const configureStore = ({
  history
}) => {
  const store = createStore(
    connectRouter(history)(reducers),
    initialState,
    composeWithDevTools(
      applyMiddleware(
        routerMiddleware(history),
        thunk,
        apiMiddleware,
        promiseMiddleware({
          promiseTypeSuffixes: ['START', 'SUCCES', 'ERROR']
        })
      )
    )
  )
  // window.store = store
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
