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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { Iterable, fromJS, List } from 'immutable'
import { PTVRadioGroup, PTVAutoComboBox, PTVTagSelect, PTVTag, PTVCheckBoxTwoLevelList } from '../../Components'
import PTVCheckBoxList from '../../Components/PTVCheckBoxList/PTVCheckBoxListNew'
import TextList from './textList'
import { AsyncComboBox } from './asyncComboBox'
import { getTranslatedItemTexts, getLanguageForTranslation } from '../../Intl/Selectors'

const getLocalTranslatedItem = (data, { id, language }) => {
  const resLanguage = fromJS(data.item).getIn(['translation', 'texts', language])
  const reslocale = fromJS(data.item).getIn(['translation', 'texts', data.locale])
//    console.log(res);
  return List(data.locale !== language ? [reslocale, resLanguage] : [resLanguage])
}

const localizeData = (state, item, language, getTranslationTextFunc = getTranslatedItemTexts) => {
  if (Iterable.isIterable(item)) {
    return localizeDataMap(state, item, language, getTranslationTextFunc)
  }
  return localizeDataJs(state, item, language, getTranslationTextFunc)
}

const localizeDataJs = (state, item, language, getTranslationTextFunc = getTranslatedItemTexts) => {
  let translation = null
  if (!item) {
    return item
  }
  const id = item.id
  if (id) {
    translation = getTranslationTextFunc(state, { id, language }).filter(l => l != null).reduce((p,c) => `${p} - ${c}`)
  }
    // console.log(id, translation, item)
  return { ...item, name: translation || item.name }
}

const localizeDataMap = (state, item, language, getTranslationTextFunc = getTranslatedItemTexts) => {
  let translation = null
  if (!item) {
    return item
  }

  const id = item.get('id')
  if (id) {
    translation = getTranslationTextFunc(state, { id, language }).filter(l => l != null).reduce((p,c) => `${p} - ${c}`)
  }
    // console.log('data', item.toJS(), id, translation)
  return translation ? item.set('name', translation) : item
}

const localizeList = (state, values, language) => {
  if (values) {
    return values.map(item => localizeData(state, item, language))
  }
  return values
}

const sort = (list, sortAttr) => {
  const nameAttribute = sortAttr
  if (Iterable.isIterable(list)) {
    return list.sortBy(x => x.get ? x.get(nameAttribute) : x[nameAttribute])
  }
  list.sort((a, b) => a[nameAttribute] < b[nameAttribute] ? -1 : 1)
  return list
}

// connect localization to component 'InnerComponent'
export const connectLocalizedComponent = (InnerComponent, options) => {
  const localizedOptions = options || {}

    // Localized component wrapper for 'InnetComponent'
  const LocalizedComponent = (props) => {
      let componentProps = { ...props }
      if (localizedOptions.selector) {
          componentProps[localizedOptions.selector] = (state, ownProps) => localizeList(state, props[localizedOptions.selector](state, ownProps), props.language)
        }
      if (localizedOptions.getItemFunc) {
          componentProps[localizedOptions.getItemFunc] = (item) => {
              const data = localizeData({item:item, locale: props.locale}, item, props.language, getLocalTranslatedItem)
              return props[localizedOptions.getItemFunc] ? props[localizedOptions.getItemFunc](data) : data
            }
        }
      if(localizedOptions.isSorted && componentProps[localizedOptions.isSorted]){
        sort(componentProps['options'],componentProps[localizedOptions.sortAttribute])
      }
      return <InnerComponent {...componentProps} />
    }

  const getLocale = (state) => localizedOptions.getItemFunc ? getLanguageForTranslation(state) : null

  // Map to state functions
  const mapStateToPropsLocalized = (state, ownProps) => {
    const single = {
      output: localizedOptions.output || 'values',
      input: localizedOptions.input || 'values'
    }
    let inputMapping = localizedOptions.inputMapping || []
    inputMapping = [...inputMapping, single] || [single]

      let result = {
           locale: getLocale(state)
        }
      inputMapping.map(x =>
            result[x.output] = localizedOptions.single
                ? localizeData(state, ownProps[x.input], ownProps.language)
                : localizeList(state, ownProps[x.input], ownProps.language)
        )

    return result
  }

  const mapStateToPropsLocalizedSingle = (state, ownProps) => {
      const item = localizeData(state, ownProps, ownProps.language)
      return {
          id: item.id,
          name: item.name,
          locale: getLocale(state)
        }
    }

    // Component creation
  if (localizedOptions.selector) {
    return LocalizedComponent
  }

  if (localizedOptions.single && !localizedOptions.input && !localizedOptions.output) {
    return connect(mapStateToPropsLocalizedSingle)(LocalizedComponent)
  }
  return connect(mapStateToPropsLocalized)(LocalizedComponent)
}

// Components
export const LocalizedComboBox = connectLocalizedComponent(PTVAutoComboBox)
export const LocalizedAsyncComboBox = connectLocalizedComponent(AsyncComboBox, {
  input: 'value',
  output: 'value',
  single: true,
  getItemFunc: 'formatOption'
})
export const LocalizedRadioGroup = connectLocalizedComponent(PTVRadioGroup, { input: 'items', output: 'items' })
export const LocalizedTagSelect = connectLocalizedComponent(PTVTagSelect, {
  inputMapping: [
        { input: 'options', output: 'options' },
        { input: 'value', output: 'value' }
  ],
  isSorted:'isSorted',
  sortAttribute:'sortAttribute'
})
export const LocalizedCheckBoxList = connectLocalizedComponent(PTVCheckBoxList, { input: 'box', output: 'box' })
export const LocalizedTwoLevelCheckBoxList = connectLocalizedComponent(
  PTVCheckBoxTwoLevelList,
  { input: 'data', output: 'data' }
)
export const LocalizedTag = connectLocalizedComponent(PTVTag, { single: true })
export const LocalizedTextList = connectLocalizedComponent(TextList, { selector: 'getListSelector' })

const Text = ({ name, className }) => (
  <div className={className}>{name}</div>
)
export const LocalizedText = connectLocalizedComponent(Text, { single: true })
