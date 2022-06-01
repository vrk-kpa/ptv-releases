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
import { createSelector } from 'reselect'
import { getFormValueWithPath, getParameterFromProps } from 'selectors/base'
import { combinePathAndField } from 'util/redux-form/util'
import { EntitySelectors } from 'selectors'
import { List } from 'immutable'

export const getLanguageId = createSelector(
  [EntitySelectors.languages.getEntities, getParameterFromProps('languageCode')],
  (languages, languageCode) => {
    const languagesList = languages && languages.toList() || List()
    const languageEntity = languagesList.filter((value, key) => value.get('code') === languageCode).first()
    return languageEntity && languageEntity.get('id')
  }
)

export const getFormValue = createSelector(
  getFormValueWithPath(props => combinePathAndField(props.path, props.fieldName)),
  value => value
)

export const getDefaultStreetName = createSelector(
  [getFormValue, EntitySelectors.translatedItems.getEntities, getParameterFromProps('languageCode')],
  (id, translations, languageCode) => {
    const translation = translations.get(id)
    const texts = translation && translation.get('texts')
    // Language fallback: try content language, then default Finnish, then default for Åland islands
    return texts && (texts.get(languageCode) || texts.get('fi') || texts.first())
  }
)
