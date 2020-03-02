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
import { getFormValueWithPath } from 'selectors/base'
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

const getFormLanguageCodes = createSelector(
  getFormValueWithPath(props => 'languages'),
  languageCodes => languageCodes
)

const getFormLanguages = createSelector(
  [getFormLanguageCodes, EntitySelectors.languages.getEntities],
  (codes, languages) => {
    return languages.filter((value, key) => codes.some(code => code === value.get('code')))
  }
)

const getTranslatedLanguages = createTranslatedListSelector(
  getFormLanguages, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getAreAllLanguagesSelected = createSelector(
  [getFormLanguageCodes, EnumsSelectors.translationLanguages.getEntities],
  (languageCodes, availableLanguages) => {
    return languageCodes.size === availableLanguages.size
  }
)

export const getAreNoLanguagesSelected = createSelector(
  getFormLanguageCodes,
  languageCodes => !languageCodes || languageCodes.size === 0
)

export const getSelectedLanguages = createSelector(
  getTranslatedLanguages,
  languages => {
    const result = []
    languages.forEach((entry, key) => {
      result.push({
        label: entry.get('name'),
        value: key
      })
    })
    return result
  }
)
