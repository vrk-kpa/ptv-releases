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
import React from 'react'
import { render } from 'react-dom'
import { Router, browserHistory } from 'react-router'
import { addLocaleData } from 'react-intl'
import IntlProvider from './Intl/IntlProvider'
import { Provider } from 'react-redux'
import CreateRoutes from './Configuration/createRoutes'
import ConfigureStore from './Configuration/configureStore'
import enLocaleData from 'react-intl/locale-data/en'
import svLocaleData from 'react-intl/locale-data/sv'
import fiLocaleData from 'react-intl/locale-data/fi'
import ReduxBugReporter from 'util/redux-bug-reporter'
import 'styles/redux-bug-reporter.css'
import { syncHistoryWithStore } from 'react-router-redux'

if (!global.Intl) {
  global.Intl = require('intl')
}
if (!window.Intl) {
  window.Intl = require('intl')
}

// Clearing front page form state on refresh //
window.onbeforeunload = () => sessionStorage.removeItem('frontPageFormState')

addLocaleData(enLocaleData)
addLocaleData(svLocaleData)
addLocaleData(fiLocaleData)

const initialState = window.__INITIAL_STATE__
const store = ConfigureStore({ initialState })
const routes = CreateRoutes(store)
const history = syncHistoryWithStore(browserHistory, store, {
  selectLocationState: state => state.get('routing')
})

render(
  <Provider store={store}>
    <IntlProvider>
      <div>
        <Router history={history} children={routes} />
        {window.isBugReportAvailable() &&
          <ReduxBugReporter />}
      </div>
    </IntlProvider>
  </Provider>,
  document.getElementById('page-wrapper')
)
