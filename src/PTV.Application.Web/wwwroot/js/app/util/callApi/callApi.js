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
import { getAppUrl } from 'Configuration/AppHelpers'
import 'isomorphic-fetch'

const callApi = async (route, body, method = 'POST') => {
  let options = {
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${window.getAccessToken()}`,
      'access-control-allow-origin': '*'
    }
  }
  if (body) {
    options = {
      ...options,
      body: JSON.stringify(body),
      method
    }
  } else {
    options = {
      ...options,
      method: 'GET'
    }
  }
  const routeUrl = `${getAppUrl()}/api/${route}`
  const response = await fetch(routeUrl, options)
  if (response.status >= 400) {
    throw new Error(`Failed to fetch resource at ${route}`)
  }
  return response.json()
}

export default callApi
