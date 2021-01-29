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

import { Set, List, Map } from 'immutable'
import moment from 'moment'
import { EditorState, ContentState } from 'draft-js'
import { mainAccordionItemCountByForm, sortDirectionTypesEnum } from 'enums'

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

export function tryToJs (value) {
  return value && value.toJS
    ? value.toJS()
    : value
}

/**
 * UI Immutable sort
 *
 * @param {List}  list       List to be sorted.
 * @param {string} direction Sorting direction (one of ['asc', 'desc']).
 * @return {List}            List of sorted indices.
 */
export const getSortedListIndices = (list, direction = sortDirectionTypesEnum.ASC, uiLanguage) => {
  if (!List.isList(list)) {
    console.warn('Sorting skipped. Data has to be an immutable list.')
    return list.map((v, k) => k)
  }
  const listWithIndexes = list.map((v, k) => Map({ index: k, value: v }))
  const sortedList = listWithIndexes.sort((a, b) => {
    const nameA = (a.get('value') && String(a.get('value')) || '').toLowerCase()
    const nameB = (b.get('value') && String(b.get('value')) || '').toLowerCase()
    return direction.toLowerCase() === sortDirectionTypesEnum.ASC ? nameA.localeCompare(nameB, uiLanguage) : nameB.localeCompare(nameA, uiLanguage)
  })
  return sortedList.map(v => v.get('index'))
}

/**
 * UI sorting of data in Array
 *
 * @param {array}  data      Data to be sorted.
 * @param {string} column    Name of the column which is sorted.
 * @param {string} direction Sorting direction (one of ['asc', 'desc']).
 * @return {array}           Sorted data.
 */
export const sortUIData = (data, column, direction = 'asc') => {
  if (!Array.isArray(data)) {
    console.warn('Sorting skipped. Data has to be an array.')
    return data
  }
  if (!column || (column && typeof column !== 'string')) {
    return data
  }
  const sortedData = data.sort((a, b) => {
    let nameA = a[column].toUpperCase()
    let nameB = b[column].toUpperCase()
    if (nameA < nameB) {
      return direction.toLowerCase() === 'asc' ? -1 : 1
    }
    if (nameA > nameB) {
      return direction.toLowerCase() === 'asc' ? 1 : -1
    }
    return 0
  })
  return sortedData
}

export const isFunction = (func) => {
  return typeof func === 'function'
}
/**
 * Key codes definition
 */
export const keyCodes = Object.freeze({
  'BACKSPACE': 8,
  'TAB': 9,
  'RETURN': 13,
  'ESC': 27,
  'SPACE': 32,
  'PAGEUP': 33,
  'PAGEDOWN': 34,
  'END': 35,
  'HOME': 36,
  'LEFT': 37,
  'UP': 38,
  'RIGHT': 39,
  'DOWN': 40
})

/**
 * Gets plain text from editor state
 */
export const getPlainText = editorState => {
  if (editorState) {
    if (typeof editorState.getCurrentContent === 'function') {
      return editorState.getCurrentContent().getPlainText()
    }
    return editorState
  }
  return ''
}

export const createEditorStateFromText = text => EditorState.createWithContent(ContentState.createFromText(text))

export const getMainAccordionItemCountByForm = form => [...Array(mainAccordionItemCountByForm[form]).keys()]
