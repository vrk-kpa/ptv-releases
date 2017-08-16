/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import React from 'react'
import { Route, IndexRoute, IndexRedirect } from 'react-router'

import Services from '../Routes/Service'
import Channels from '../Containers/Channels'
import Manage from '../Containers/Manage'
import Relations from '../Routes/Relations'
import FrontPageRoute from 'Routes/FrontPageV2'
import SearchView from 'Routes/FrontPageV2/views/SearchView'
import toolsPage from 'Routes/Tools/ToolsPage'
import currentIssuesPage from 'Routes/CurrentIssues'
import App from './App'

export default () => (
  <Route path='/' component={App}>
    <IndexRedirect to="/frontpage" />
    <Route path='service' component={Services.ServicePage}>
      <IndexRoute component={Services.LandingPage} />
      <Route path='SearchGeneralDescription' component={Services.GeneralDescriptionSearchPage} />
      <Route path='ManageService' component={Services.ManageServicePage} />
    </Route>
    <Route path='channels' component={Services.ServicePage}>
      <Route path=':view/:id' component={Channels} />
    </Route>
    <Route path='manage' component={Services.ServicePage}>
      <Route path=':view/:id' component={Manage} />
    </Route>
    <Route path='relations' component={Services.ServicePage}>
      <IndexRoute component={Relations.ServiceAndChannelPage} />
    </Route>
    <Route path='tool' component={toolsPage} />
    <Route path='currentIssues' component={currentIssuesPage} />
    <Route path='frontpage' component={FrontPageRoute}>
      <IndexRoute component={SearchView} />
      <Route path='search' component={SearchView} />
      <Route path='relations' component={Relations.ServiceAndChannelPage} />
    </Route>
  </Route>
)
