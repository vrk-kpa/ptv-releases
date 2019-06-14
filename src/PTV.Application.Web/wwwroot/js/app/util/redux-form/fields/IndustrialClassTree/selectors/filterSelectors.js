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
import { getParameterFromProps } from 'selectors/base'
import { Map, List } from 'immutable'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import createCachedSelector from 're-reselect'

const getIndustrialClassTranslations = createCachedSelector(
  [EntitySelectors.translatedItems.getEntities, EnumsSelectors.industrialClasses.getEnums],
  (translatedItems, ids) => {
    return translatedItems.filter(item => ids.some(id => id === item.get('id')))
  }
)((_, { searchValue }) => searchValue || '')

const getSearchedTranslations = createCachedSelector(
  [getIndustrialClassTranslations, getParameterFromProps('searchValue')],
  (translations, search) => {
    if (!search || search.length < 3 || !translations) {
      return List()
    }

    const lowerSearch = search.toLowerCase()
    const result = translations.filter(translation => {
      const texts = translation.get('texts')
      return texts && texts.some(text => text.toLowerCase().indexOf(lowerSearch) !== -1)
    })

    return result
  }
)((_, { searchValue }) => searchValue || '')

const getSearchedIds = createCachedSelector(
  getSearchedTranslations,
  translations => translations.map(translation => translation.get('id')).toSet().toList()
)((_, { searchValue }) => searchValue || '')

export const getIndustrialClassIds = createCachedSelector(
  [getSearchedIds,
    getParameterFromProps('value'),
    getParameterFromProps('showSelected'),
    getParameterFromProps('filterSelected')],
  (searchedIds, selected, showSelected, filterSelected) => showSelected
    ? selected.toList()
    : (filterSelected && searchedIds.filter(id => !selected.has(id)) || searchedIds) || List()
)((_, { searchValue, value, showSelected, filterSelected }) => {
  return showSelected
    ? (value && JSON.stringify(value.toJS()) || '')
    : searchValue
})

export const getIndustrialClass = createCachedSelector(
  EntitySelectors.industrialClasses.getEntity,
  entity => entity || Map()
)((_, { id }) => id)
