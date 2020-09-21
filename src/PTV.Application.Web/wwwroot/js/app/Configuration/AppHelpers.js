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
import ReactDOM from 'react-dom'
import { fromJS } from 'immutable'
import { ptvCookieTokenName, pahaCookieTokenName } from 'enums'

if (!window.location.origin) {
  window.location.origin = window.location.protocol + '//' + window.location.hostname + (window.location.port ? ':' + window.location.port : '')
}

const windowLocation = window.location.origin

const APP_URL = windowLocation

export const getCookie = (name) => {
  return document.cookie.split('; ').reduce((r, v) => {
    const parts = v.split('=')
    return parts[0] === name ? decodeURIComponent(parts[1]) : r
  }, '')
}

export const setCookie = (key, value) => {
  document.cookie = key + '=' + value + '; path=/'
}

export const deleteCookie = (key) => {
  document.cookie = key + '=' + '' + '; path=/'
}

export const getAuthToken = () => getCookie(ptvCookieTokenName) || null
export const getPahaAuthToken = () => getCookie(pahaCookieTokenName) || null

export function getAppUrl () {
  return APP_URL// .replace('://', '://uiapi.');
}

function buildApiRootUrl () {
  var url = ''

  if (typeof (getApiUrl) === 'function') {
    url = getApiUrl()
  }

  return url !== '' ? url : (getAppUrl() + '/api/')
}

export function getLogoAddress () {
  var url = ''

  if (typeof (getLogoUrl) === 'function') {
    url = getLogoUrl()
  }

  return url
}

export function isPahaInUse () {
  let isEnabled = ''

  if (typeof (isPAHALoginEnabled) === 'function') {
    isEnabled = isPAHALoginEnabled()
  }

  return isEnabled === true
}

export function isFakeAuthenticationInUse () {
  let isEnabled = ''

  if (typeof (isFakeAuthenticationAvailable) === 'function') {
    isEnabled = isFakeAuthenticationAvailable()
  }

  return isEnabled === true
}

export function isBasicLoginEnabled () {
  return !isPahaInUse() && !isFakeAuthenticationInUse()
}

export const API_ROOT = buildApiRootUrl()
export const API_WEB_ROOT = getAppUrl() + '/api/'

console.log('URL: ' + API_ROOT)
console.log('URL: ' + API_WEB_ROOT)

export function scrollTo (ref) {
  if (ref) {
    var node = ReactDOM.findDOMNode(ref)
    window.scrollTo(0, node.offsetTop)
  }
}

export const loadState = () => {
  try {
    const serializedState = localStorage.getItem('state')
    if (serializedState === null) {
      return undefined
    }

    const json = fromJS(JSON.parse(serializedState))
    return json
  } catch (err) {
    return undefined
  }
}

export const saveState = state => {
  try {
    const serializedState = JSON.stringify(state)
    localStorage.setItem('state', serializedState)
  } catch (err) {
    console.error(err)
  }
}
