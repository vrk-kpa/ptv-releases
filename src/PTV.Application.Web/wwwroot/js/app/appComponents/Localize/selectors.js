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
import { createMapSelector, createObjectSelector } from 'reselect-map'
import { EntitySelectors, BaseSelectors } from 'selectors'
import { getContentLanguageCode, getSelectedComparisionLanguageCode } from 'selectors/selections'
import { getIntlLocale } from 'Intl/Selectors'
import { Iterable, List, Map } from 'immutable'

const getTranslationTexts = createSelector(
  [EntitySelectors.translatedItems.getEntities],
  items => items.map(x => x.get('texts'))
)

const getTranslatedItem = (items, id, languages, notFound = null) =>
 // console.log(languages, languages && languages.toJS && languages.toJS()) ||
  languages.map(l => items.getIn([id, l]))
    .filter(l => l != null)
    .reduce((p, c) => `${p} - ${c}`) ||
    notFound || ''

const localizeData = options => (translatedItems, item, language) => {
  if (Iterable.isIterable(item)) {
    return localizeDataMap(options)(translatedItems, item, language)
  }
  return localizeDataJs(options)(translatedItems, item, language)
}

const localizeDataJs = options => (translatedItems, item, language) => {
  let translation = null
  if (!item) {
    return item
  }
  const id = item[options.idAttribute]
  if (id) {
    translation = getTranslatedItem(
      translatedItems,
      id,
      language,
      options.getDefaultTranslationText(id, item[options.nameAttribute])
    )
     // .toArray().join(' - ')
  }
  return { ...item, [options.nameAttribute]: translation || item[options.nameAttribute] || '' }
}

const localizeDataMap = options => (translatedItems, item, language) => {
  let translation = null
  if (!item) {
    return item
  }

  const id = item.get(options.idAttribute)
  if (id) {
    translation = getTranslatedItem(
      translatedItems,
      id,
      language,
      options.getDefaultTranslationText(id, item.get(options.nameAttribute))
    )
      // .toArray().join(' - ')
  }
  return translation ? item.set(options.nameAttribute, translation) : item
}

const sort = (list, options, language) => {
  const nameAttribute = options.nameAttribute
  if (Iterable.isIterable(list)) {
    return list.sortBy(x => x.get ? x.get(nameAttribute) : x[nameAttribute])
  }
  list.sort((a, b) => {
    const aname = a[nameAttribute]
    return typeof aname === 'string' ? aname.localeCompare(b[nameAttribute], language) : 0
  })
  return list
}

// const localizeListData = options => (translatedItems, values, language) => {
//   if (values && values.map) {
//     const resultValues = values.map(item => localizeData(options)(translatedItems, item, language))
//     return (typeof options.isSorted === 'function' && options.isSorted(resultValues, options)) || (options.isSorted
//       ? sort(resultValues, options, language)
//       : resultValues)
//   }
//   return values
// }

// PTV-2876 The 'Other responsible organisations' must be in alphabetical order
// code fi required to be forced
// if you need to return it back, just uncomment the selector above and comment this one
const localizeListData = options => (translatedItems, values, language) => {
  if (values && values.map) {
    const resultValues = values.map(item => localizeData(options)(translatedItems, item, language))
    return (typeof options.isSorted === 'function' && options.isSorted(resultValues, options)) || (options.isSorted
      ? sort(resultValues, options, 'fi')
      : resultValues)
  }
  return values
}

const getFieldLanguage = createSelector([
  getContentLanguageCode,
  getSelectedComparisionLanguageCode,
  BaseSelectors.getParameterFromProps('compare')
], (
  contentLanguage,
  comparisonLanguage,
  compare
) =>
  compare && comparisonLanguage ||
  contentLanguage ||
  null
)

const getLanguagesForTranslation = options => createSelector(
  [getIntlLocale, getFieldLanguage],
  (locale, language) =>
  (locale !== language && !options.translateLocaleOnly) && List([locale, language]).filter(l => l != null) || List([locale])
)

const getLanguagesKey = languages => languages.toArray().join()

// let index = 0
export const createSelectors = options => {
  // temp counter for performance debugging
  // const build = index++
  let allLanguages = Map()
  // let languageMap = {}

  const getLanguages = createSelector(
    getLanguagesForTranslation(options),
    (currentLanguages) => {
      const key = getLanguagesKey(currentLanguages)
      allLanguages = allLanguages.set(key, currentLanguages)
      // allLanguages[key] = currentLanguages
      // languageMap = languageMap[key] = currentLanguages
      return allLanguages.reduce((p, v, k) => p.set(k, k), Map())
    }
  )

  // get translated values in list
  const getTranslatedListAll = createMapSelector(
    [getLanguages, getTranslationTexts, BaseSelectors.getParameterFromProps('values')],
    (languages, texts, values, key) => {
      // console.log('list', build, values)
      const x = localizeListData(options)(texts, values, allLanguages.get(languages))
      // console.log(key, languages, x)
      return x
    }
  )

  const getTranslatedList = createSelector(
    [getTranslatedListAll, getLanguagesForTranslation(options)],
    (languagesTranslations, languages) => languagesTranslations.get(getLanguagesKey(languages))
  )

  const getTranslatedItem = createSelector(
    [
      getTranslationTexts,
      BaseSelectors.getParameterFromProps('item'),
      getLanguagesForTranslation(options)
    ],
    (texts, item, languages) => {
      // console.log('ti', build, item, item && item.toJS && item.toJS())
      return localizeData(options)(texts, item, languages)
    }
  )

  const getTranslatedProps = createSelector(
    [
      getTranslationTexts,
      BaseSelectors.getParameterFromProps(options.idAttribute),
      BaseSelectors.getParameterFromProps(options.nameAttribute),
      getLanguagesForTranslation(options)
    ],
    (texts, id, name, languages) => {
      // console.log('tp', id, name)
      return localizeData(options)(texts, { [options.idAttribute]: id, [options.nameAttribute]: name }, languages)
    }
  )
  return {
    getTranslatedList,
    getTranslatedItem,
    getTranslatedProps
  }
}
