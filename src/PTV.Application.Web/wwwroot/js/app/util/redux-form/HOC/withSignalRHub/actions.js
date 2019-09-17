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
import { getAuthToken } from 'Configuration/AppHelpers'
import { setHubRequestSent, clearHubRequestSent } from 'reducers/signalR'

export const start = (hubConnection, onStarted) => {
  if (!hubConnection) {
    console.log('No hub detected')
    return
  }

  return getAuthToken() && hubConnection.start()
    .then(() => {
      console.log('SignalR started')
      typeof onStarted === 'function' && onStarted()
    })
    .catch(err => {
      console.log('SignalRConnection error: ', err, err.statusCode)
      if (err.statusCode !== 401 && err.statusCode !== 0) {
        setTimeout(() => start(hubConnection), 500)
      }
      return Promise.reject(err)
    })
}

const invoke = ({ hubConnection, hubName, action, data, dispatch }) => {
  hubConnection
    .invoke(action, data)
    .catch(err => {
      console.error(err.toString(), err)
      dispatch(clearHubRequestSent({ hubName }))
    })
}

export const invokeHub = ({ hubConnection, hubName, action, data }) => ({ dispatch }) => {
  if (hubConnection) {
    dispatch(setHubRequestSent({ hubName }))
    if (hubConnection.connection.connectionState !== 1) {
      start(hubConnection, () => invoke({ hubConnection, hubName, action, data, dispatch }))
        .catch(() => dispatch(clearHubRequestSent(hubName)))
    } else {
      invoke({ hubConnection, hubName, action, data, dispatch })
    }
  }
}
