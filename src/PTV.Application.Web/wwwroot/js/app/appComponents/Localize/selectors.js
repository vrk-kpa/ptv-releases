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
import createCachedSelector from 're-reselect'
import { EntitySelectors, BaseSelectors } from 'selectors'
import { getContentLanguageCode, getSelectedComparisionLanguageCode } from 'selectors/selections'
import { getIntlLocale, getLocalizationMessages } from 'Intl/Selectors'
import { Iterable, List, Map } from 'immutable'
import { languageTranslationTypes, getOptions } from './configuration'

const getTranslationTextsBase = createSelector(
  [EntitySelectors.translatedItems.getEntities],
  items => items.map(x => x.get('texts'))
)

const getDefaultTexts = createSelector(
  EntitySelectors.translatedItems.getEntities,
  items => items.map(x => x.get('defaultText'))
)

const getMessages = createSelector(
  getLocalizationMessages,
  messages => messages && messages
    .map((x, k) => x.get('texts').map(t => Map({ [k]: t })))
    .reduce((p, c) => p.mergeDeep(c), Map()) ||
    Map()
)

const getTranslationTexts = createSelector(
  [getTranslationTextsBase, getMessages],
  (data, messages) => data.merge(messages)
)

const getTranslatedItem = (items, id, languages, notFound = null, defaultTexts) =>
  // console.log(languages, languages && languages.toJS && languages.toJS()) ||
  List(languages.split(',')).map(l => items.getIn([id, l]))
    .filter(l => l != null)
    .reduce((p, c) => `${p} - ${c}`) ||
    notFound || (defaultTexts && defaultTexts.get(id))

const localizeData = options => (translatedItems, item, language, defaultTexts, intl) => {
  if (Iterable.isIterable(item)) {
    return localizeDataMap(options)(translatedItems, item, language, defaultTexts, intl)
  }
  return localizeDataJs(options)(translatedItems, item, language, defaultTexts, intl)
}

const mergeTranslatedItems = (options, translatedItems, item, id) => {
  if (typeof options.getTranslationTexts === 'function') {
    const texts = options.getTranslationTexts(item)
    return texts && translatedItems.merge({ [id]: texts }) || translatedItems
  }
  return translatedItems
}

const localizeDataJs = options => (translatedItems, item, language, defaultTexts, intl) => {
  let translation = null
  if (!item) {
    return item
  }
  const id = typeof options.getId === 'function' ? options.getId(item) : item[options.idAttribute]
  if (id) {
    translation = getTranslatedItem(
      mergeTranslatedItems(options, translatedItems, item, id),
      id,
      language,
      options.getDefaultTranslationText(id, item[options.nameAttribute], intl),
      defaultTexts
    )
    // .toArray().join(' - ')
  }
  return { ...item, [options.nameAttribute]: translation || item[options.nameAttribute] || '' }
}

const localizeDataMap = options => (translatedItems, item, language, defaultTexts, intl) => {
  let translation = null
  if (!item) {
    return item
  }

  const id = typeof options.getId === 'function' ? options.getId(item) : item.get(options.idAttribute)
  if (id) {
    translation = getTranslatedItem(
      mergeTranslatedItems(options, translatedItems, item, id),
      id,
      language,
      options.getDefaultTranslationText(id, item.get(options.nameAttribute), intl),
      defaultTexts
    )
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
const localizeListData = options => (translatedItems, values, language, defaultTexts) => {
  if (values && values.map) {
    const resultValues = values.map(item => localizeData(options)(translatedItems, item, language, defaultTexts))
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

const getLanguagesKey = languages => languages.toArray().join()

const getLanguagesForTranslation = options => createSelector(
  [getIntlLocale, getFieldLanguage],
  (locale, language) => {
    // console.log(options)
    switch (options.languageTranslationType) {
      case languageTranslationTypes.locale:
        // console.log('locale', options.languageTranslationType, languageTranslationTypes.locale)
        return locale || ''
      case languageTranslationTypes.data:
        // console.log('data', options.languageTranslationType, languageTranslationTypes.data)
        return language || ''
      case languageTranslationTypes.both:
      default:
        // console.log('both, default', options.languageTranslationType, languageTranslationTypes.both)
        return locale === language ? locale : getLanguagesKey(List([locale, language]).filter(l => l != null))
    }
  }
)

// let index = 0
export const createSelectors = options => {
  // temp counter for performance debugging
  // const build = index++
  // let allLanguages = Map()
  // // let languageMap = {}

  // const getLanguages = createSelector(
  //   getLanguagesForTranslation(options),
  //   (currentLanguages) => {
  //     const key = getLanguagesKey(currentLanguages)
  //     allLanguages = allLanguages.set(key, currentLanguages)
  //     // allLanguages[key] = currentLanguages
  //     // languageMap = languageMap[key] = currentLanguages
  //     return allLanguages.reduce((p, v, k) => p.set(k, k), Map())
  //   }
  // )

  // get translated values in list
  const getTranslatedList = createCachedSelector([
    getLanguagesForTranslation(options),
    getTranslationTexts,
    BaseSelectors.getParameterFromProps('values'),
    getDefaultTexts
  ],
  (languages, texts, values, defaultTexts) => {
    // console.log('list', build, values)
    const x = localizeListData(options)(texts, values, languages, defaultTexts)
    // console.log(languages, languages, options)
    return x
  }
  )(
    getLanguagesForTranslation(options)
  )

  // const getTranslatedList = createSelector(
  //   [getTranslatedListAll, getLanguagesForTranslation(options)],
  //   (languagesTranslations, languages) => languagesTranslations.get(getLanguagesKey(languages))
  // )

  const getTranslatedItem = createSelector(
    [
      getTranslationTexts,
      BaseSelectors.getParameterFromProps('item'),
      getLanguagesForTranslation(options),
      getDefaultTexts,
      BaseSelectors.getParameterFromProps('intl')
    ],
    (texts, item, languages, defaultTexts, intl) => {
      // console.log('ti', build, item, item && item.toJS && item.toJS())
      return localizeData(options)(texts, item, languages, defaultTexts, intl)
    }
  )

  const getTranslatedProps = createSelector(
    [
      getTranslationTexts,
      BaseSelectors.getParameterFromProps(options.idAttribute),
      BaseSelectors.getParameterFromProps(options.nameAttribute),
      getLanguagesForTranslation(options),
      getDefaultTexts
    ],
    (texts, id, name, languages, defaultTexts) => {
      // console.log('tp', id, name)
      return localizeData(options)(texts, { [options.idAttribute]: id, [options.nameAttribute]: name }, languages, defaultTexts)
    }
  )
  return {
    getTranslatedList,
    getTranslatedItem,
    getTranslatedProps
  }
}

export const createTranslatedListSelector = (selector, options) => {
  if (typeof selector !== 'function') {
    console.error(`Wrong type of selector parametr. it should be a function but was ${typeof selector}.`)
  }
  const innerOptions = getOptions(options)
  // console.log(selector, options, innerOptions)
  const getLanguages = getLanguagesForTranslation(innerOptions)

  return createCachedSelector([
    getLanguages,
    getTranslationTexts,
    selector,
    getDefaultTexts
  ],
  (languages, texts, values, defaultTexts) => {
    // console.log('list', build, values)
    const x = localizeListData(innerOptions)(texts, values, languages, defaultTexts)
    // console.log('localize', languages, values)
    return x
  }
  )(
    (state, props) => {
      const x = getLanguages(state, props)
      const key = innerOptions.key(state, props)
      // console.log('key: ', key, x)
      return key + x
    }
  )
}

export const createTranslatedIdSelector = (idSelector, options) => {
  if (typeof idSelector !== 'function') {
    console.error(`Wrong type of selector parametr. it should be a function but was ${typeof idSelector}.`)
  }
  const innerOptions = getOptions(options)
  // console.log(selector, options, innerOptions)
  const getLanguages = getLanguagesForTranslation(innerOptions)

  return createCachedSelector([
    getLanguages,
    getTranslationTexts,
    idSelector,
    getDefaultTexts
  ],
  (languages, texts, id, defaultTexts) => {
    // console.log('list', build, values)
    const x = localizeData(innerOptions)(texts, { [innerOptions.idAttribute]: id }, languages, defaultTexts)
    // console.log('localize', languages, values)
    return x[innerOptions.nameAttribute]
  }
  )(
    (state, props) => {
      const x = getLanguages(state, props)
      const id = idSelector(state, props)
      const key = innerOptions.key(state, props)
      // console.log('key: ', key, x)
      return key + x + id
    }
  )
}
