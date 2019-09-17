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
import { Route, Switch, Redirect } from 'react-router-dom'
import PropTypes from 'prop-types'
import ElectronicChannel from 'Routes/Channels/routes/Electronic'
import WebPage from 'Routes/Channels/routes/WebPage'
import PrintableForm from 'Routes/Channels/routes/PrintableForm'
import PhoneChannel from 'Routes/Channels/routes/Phone'
import ServiceLocation from 'Routes/Channels/routes/ServiceLocation'

const Channels = ({ computedMatch: { path } }) => (
  <div>
    <Switch>
      <Route path={`${path}/electronic`} component={ElectronicChannel} />
      <Route path={`${path}/phone`} component={PhoneChannel} />
      <Route path={`${path}/printableForm`} component={PrintableForm} />
      <Route path={`${path}/serviceLocation`} component={ServiceLocation} />
      <Route path={`${path}/webPage`} component={WebPage} />
      <Redirect to={`${path}/electronic`} />
    </Switch>
  </div>)

Channels.propTypes = {
  computedMatch: PropTypes.object
}

export default Channels

// export default store => ({
//   path: '/channels',
//   childRoutes: [
//     ElectronicChannelRoute(store),
//     WebPageChannelRoute(store),
//     PrintableFormRoute(store),
//     PhoneChannelRoute(store),
//     ServiceLocationRoute(store)
//   ],
//   getComponent (nextState, cb) {
//     cb(null, Channels)
//   }
// })
