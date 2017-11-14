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

export const getGDgetGDConditionOfServiceUsageValueData = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('conditionOfServiceUsage') || Map()
)

export const getIsGDConditionOfServiceUsage = createSelector(
      [getGDgetGDConditionOfServiceUsageValueData, getContentLanguageCode],
      (condition, lang) => {
        const langContent = condition.get(lang) || null
        const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
        return (raw && raw.getCurrentContent().hasText()) || false
      }
  )

export const getIsGDConditionOfServiceUsageCompare = createSelector(
    [getGDgetGDConditionOfServiceUsageValueData, getSelectedComparisionLanguageCode],
    (condition, lang) => {
      const langContent = condition.get(lang) || null
      const raw = langContent && EditorState.createWithContent(convertFromRaw(JSON.parse(langContent)))
      return (raw && raw.getCurrentContent().hasText()) || false
    }
)

export const getGDConditionOfServiceUsageValue = createSelector(
  [getGDgetGDConditionOfServiceUsageValueData, getContentLanguageCode, getIsGDConditionOfServiceUsage],
  (condition, lang, hasValue) => {
    if (hasValue) {
      const editorState = condition && condition.size > 0 &&
      condition.map(l =>
          EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)

export const getGDConditionOfServiceUsageCompareValue = createSelector(
  [getGDgetGDConditionOfServiceUsageValueData, getSelectedComparisionLanguageCode, getIsGDConditionOfServiceUsage],
  (condition, lang, hasValue) => {
    if (hasValue) {
      const editorState = condition && condition.size > 0 &&
        condition.map(l =>
          EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
        ) || Map()
      return editorState.get(lang)
    }
    return null
  }
)

