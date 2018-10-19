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

import { Set } from 'immutable'
import moment from 'moment'

export const keyIn = keys => {
  const keySet = Set(keys)
  return (v, k) => keySet.has(k)
}

export const capitalizeFirstLetter = (text) => {
  return text && text.charAt(0).toUpperCase() + text.slice(1)
}

export const toLowerCaseFirstLetter = (text) => {
  return text && text.charAt(0).toLowerCase() + text.slice(1)
}

export const copyToClip = data => {
  var id = 'mycustom-clipboard-textarea-hidden-id'
  var existsTextarea = document.getElementById(id)
  if (!existsTextarea) {
    var textarea = document.createElement('textarea')
    textarea.id = id
    // Place in top-left corner of screen regardless of scroll position.
    textarea.style.position = 'fixed'
    textarea.style.top = 0
    textarea.style.left = 0
    // Ensure it has a small width and height. Setting to 1px / 1em
    // doesn't work as this gives a negative w/h on some browsers.
    textarea.style.width = '1px'
    textarea.style.height = '1px'
    // We don't need padding, reducing the size if it does flash render.
    textarea.style.padding = 0
    // Clean up any borders.
    textarea.style.border = 'none'
    textarea.style.outline = 'none'
    textarea.style.boxShadow = 'none'
    // Avoid flash of white box if rendered for any reason.
    textarea.style.background = 'transparent'
    document.querySelector('body').appendChild(textarea)
    existsTextarea = document.getElementById(id)
  }
  existsTextarea.value = data
  existsTextarea.select()
  document.execCommand('copy')
}

export const isPastDate = date => date && moment(date).endOf('day').isBefore() || false
