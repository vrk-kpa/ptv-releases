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
import { createSelector } from 'reselect'
import { Map } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { getGeneralDescription } from 'Routes/Service/components/ServiceComponents/selectors'
import { EditorState, convertFromRaw } from 'draft-js'

export const getGeneralDescriptionDescriptionData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('description') || Map()
)

export const getIsGDDescription = createSelector(
      [getGeneralDescriptionDescriptionData, getContentLanguageCode],
      (description, lang) => {
        const langContent = description.get(lang) || null
        const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
        return (raw && raw.getCurrentContent().hasText()) || false
      }
  )

export const getIsGDDescriptionCompare = createSelector(
    [getGeneralDescriptionDescriptionData, getSelectedComparisionLanguageCode],
    (description, lang) => {
      const langContent = description.get(lang) || null
      const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
      return (raw && raw.getCurrentContent().hasText()) || false
    }
)

export const getGDDescriptionValue = createSelector(
  [getGeneralDescriptionDescriptionData, getContentLanguageCode, getIsGDDescription],
  (description, lang, hasValue) => {
    if (hasValue) {
      const editorState = description && description.size > 0 &&
        description.map(l =>
          EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)

export const getGDDescriptionCompareValue = createSelector(
  [getGeneralDescriptionDescriptionData, getSelectedComparisionLanguageCode, getIsGDDescription],
  (description, lang, hasValue) => {
    if (hasValue) {
      const editorState = description && description.size > 0 &&
        description.map(l =>
          EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)

