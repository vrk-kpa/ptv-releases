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
import { createSelector } from 'reselect';
import { Map, List } from 'immutable';

import { getEntities, getJS, getObjectArray } from '../Containers/Common/Selectors';
import translatedMessages from './Messages';

const getTexts = (map, code) => map ? map.getIn([code, 'texts']) : null;
const getIntl = state => state.get('intl');

export const getSelectedLanguage = createSelector(
    getIntl,
    intl => intl.get('selectedLanguage') || 'fi'
)

export const getIntlLocale = createSelector(
    getSelectedLanguage,
    lang => lang.length > 2 ? 'fi' : lang || 'fi'
)

export const getIntlUiMessagesJS = createSelector(
    getSelectedLanguage,
    selectedLanguage => translatedMessages[selectedLanguage]
)

export const getTranslationData = createSelector(
    getEntities,
    entities => entities.get('translatedData') || null
)

export const getIsTranslatedDataLoaded = createSelector(
    getTranslationData,
    tData => tData != null
)

export const getTranslatedTexts = createSelector(
    [getTranslationData, getIntlLocale],
    (texts, selectedLanguage) => getTexts(texts, selectedLanguage) || null
);

export const getTranslatedTextsJS = createSelector(
    getTranslatedTexts,
    texts => getJS(texts, [])
);

export const getIntlMessagesJS = createSelector(
    getIntlUiMessagesJS, getTranslatedTextsJS,
    (ui, data) => ({...ui, ...data}) 
);

