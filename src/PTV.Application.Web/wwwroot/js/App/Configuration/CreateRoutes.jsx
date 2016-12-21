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
import React from 'react';
import ReactDom, { render } from 'react-dom';
import { Route, IndexRoute, IndexRedirect } from 'react-router';

import Services from '../routes/service/index';
import Channels from '../Containers/Channels';
import Manage from '../Containers/Manage';
import Relations from '../routes/relations/index';
import SubMenuChannels from '../Containers/Channels/Common/Pages/SubMenu';
import SubMenuManage from '../Containers/Manage/SubMenu';
import frontPage from '../routes/frontPage/frontpage';
import toolsPage from '../routes/tools/toolspage';
import App from './App';

export default function createRoutes() {

    return (
      <Route path="/" component={App}>
        <IndexRoute components={{content: frontPage, subMenu: null}} />
        <Route path="service" components={{content: Services.ServicePage, subMenu: null}}>
          <IndexRoute component={Services.LandingPage} />
          <Route path="SearchGeneralDescription" component={Services.GeneralDescriptionSearchPage} />
          <Route path="ManageService" component={Services.ManageServicePage} />
        </Route>       
        <Route path="channels" components={{content: Services.ServicePage, subMenu: SubMenuChannels}}>
          <IndexRedirect to="/channels/search/eChannel" />
          <Route path=":view/:id" component={Channels} />
        </Route>
        <Route path="manage" components={{content: Services.ServicePage, subMenu: SubMenuManage}}>
          <IndexRedirect to="/manage/search/organizations" />
          <Route path=":view/:id" component={Manage} />
        </Route>
        <Route path="relations" components={{content: Services.ServicePage, subMenu: null}}>
          <IndexRoute component={Relations.ServiceAndChannelPage} />
        </Route>
        <Route path="tool" components={{content: toolsPage, subMenu: null}}/>
        <Route path="frontpage" components={{content: frontPage, subMenu: null}}/>
      </Route>
  );
}
