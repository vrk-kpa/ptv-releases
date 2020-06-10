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
import React from 'react'
import { render } from 'react-dom'
import { Route } from 'react-router-dom'
import { addLocaleData } from 'util/react-intl'
import IntlProvider from './Intl/IntlProvider'
import { Provider } from 'react-redux'
import configureStore from 'store/configureStore'
// import routerInitMiddleware from 'Configuration/createRoutes'
import enLocaleData from 'react-intl/locale-data/en'
import svLocaleData from 'react-intl/locale-data/sv'
import fiLocaleData from 'react-intl/locale-data/fi'
import { createBrowserHistory } from 'history'
import { ConnectedRouter } from 'react-router-redux'
import App from 'Routes/App'

if (!global.Intl) {
  global.Intl = require('intl')
}
if (!window.Intl) {
  window.Intl = require('intl')
}
window.onbeforeunload = () => sessionStorage.removeItem('frontPageFormState')

addLocaleData(enLocaleData)
addLocaleData(svLocaleData)
addLocaleData(fiLocaleData)

const history = createBrowserHistory({
  getUserConfirmation (dialogKey, callback) {
    // use "message" as Symbol-based key
    const dialogTrigger = window[Symbol.for(dialogKey)]

    if (dialogTrigger) {
    // delegate to dialog and pass callback through
      return dialogTrigger(callback)
    }

    // Fallback to allowing navigation
    callback(true)
  }
})

const store = configureStore({ history })

render(
  <Provider store={store}>
    <IntlProvider>
      <ConnectedRouter history={history}>
        <div>
          <Route path='/' component={App} />
        </div>
      </ConnectedRouter>
    </IntlProvider>
  </Provider>,
  document.getElementById('page-wrapper')
)
