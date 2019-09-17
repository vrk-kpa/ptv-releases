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
import { EnumsSelectors } from 'selectors'
import {
  getIsInTranslation,
  getFormName
} from 'selectors/entities/entities'
import { getIsCompareMode } from 'selectors/formStates'
import { getSelectedComparisionLanguage } from 'selectors/selections'
import { getLanguageAvailabilities } from './selectors'
import { setContentLanguage, setComparisionLanguage } from 'reducers/selections'
import { mergeInUIState } from 'reducers/ui'

export const changeContentLanguage = language => ({ dispatch, getState }) => {
  const state = getState()
  const languages = EnumsSelectors.translationLanguages.getEntitiesMap(state)
  const formName = getFormName(state)
  const isCompareMode = getIsCompareMode(formName)(state)
  const selectedComparisionLanguage = getSelectedComparisionLanguage(state)
  const entityLanguageAvailabilities = getLanguageAvailabilities(state, { formName })

  const languageEntity = languages.get(language.get('languageId')) || language
  const languageCode = languageEntity.get('code')
  const languageId = languageEntity.get('id') || languageEntity.get('languageId')
  const isInTranslation = getIsInTranslation(state, { languageCode })
  const comparisionLanguageSameAsEntityLanguage =
    selectedComparisionLanguage.get('code') === languageCode ||
    selectedComparisionLanguage.get('id') === languageId

  if (
    isCompareMode &&
    comparisionLanguageSameAsEntityLanguage &&
    entityLanguageAvailabilities.size > 1
  ) {
    const newComparisionLanguageId = entityLanguageAvailabilities
      .find(language => language.get('languageId') !== selectedComparisionLanguage.get('id'))
      .get('languageId')
    const newComparisionLanguage = languages.get(newComparisionLanguageId)
    dispatch(setComparisionLanguage({
      id: newComparisionLanguage.get('id'),
      code: newComparisionLanguage.get('code')
    }))
  }

  dispatch(setContentLanguage({ id: languageId, code: languageCode }))

  // collapse accordions when in translation
  dispatch(mergeInUIState({
    key: ['translationAccordion', 'translationAccordionConnection', 'translationAccordionConnectionASTI'],
    value: {
      activeIndex: isInTranslation ? -1 : 0
    }
  }))
}
