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
import { Map, List } from 'immutable'

import { getEntities, getJS, getIdFromProps, getParameterFromProps } from '../Containers/Common/Selectors'
// import translatedMessages from './Messages';

const getTexts = (map, code) => map ? map.getIn([code, 'texts']) : null
const getIntl = state => state.get('intl')

export const getSelectedLanguage = createSelector(
    getIntl,
    intl => intl.get('selectedLanguage') || 'fi'
)

export const getIntlLocale = createSelector(
    getSelectedLanguage,
    lang => lang.length > 2 ? 'fi' : lang || 'fi'
)

// get ui messages
// export const getIntlUiMessagesJS = createSelector(
//     getSelectedLanguage,
//     selectedLanguage => translatedMessages[selectedLanguage]
// )

export const getLocalizationMessages = createSelector(
    getEntities,
    entities => entities.get('localizationMessages') || null
)

export const getIsLocalizationMessagesLoaded = createSelector(
    getLocalizationMessages,
    tData => tData != null
)

export const getLocalizationMessagesTexts = createSelector(
    [getLocalizationMessages, getSelectedLanguage],
    (texts, selectedLanguage) => getTexts(texts, selectedLanguage) || null
)

export const getIntlMessagesJS = createSelector(
    getLocalizationMessagesTexts,
    texts => getJS(texts, {})
)

// get translated data
export const getTranslationItems = createSelector(
    getEntities,
    entities => entities.get('translatedItems') || Map()
)

export const getTranslationItem = createSelector(
    [getTranslationItems, getIdFromProps],
    (texts, id) => texts.get(id) || Map()
)

export const getLanguageForTranslation = createSelector(
  [getIntlLocale, getParameterFromProps('language')],
  (locale, language) => locale // language || locale
)

export const getTranslatedItemText = createSelector(
    [getTranslationItem, getLanguageForTranslation],
    (item, language) => item.getIn(['texts', language])
)
