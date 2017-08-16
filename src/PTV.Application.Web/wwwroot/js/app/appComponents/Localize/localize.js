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
import { getTranslatedItemTexts, getIntlLocale } from '../../Intl/Selectors'

const getLocalTranslatedItem = (item, { id, language }) => {
  const res = fromJS(item).getIn(['translation', 'texts', language])
//    console.log(res);
  return List([res])
}

const localizeData = options => (state, item, language, getTranslationTextFunc = getTranslatedItemTexts) => {
  if (Iterable.isIterable(item)) {
    return localizeDataMap(options)(state, item, language, getTranslationTextFunc)
  }
  return localizeDataJs(options)(state, item, language, getTranslationTextFunc)
}

const localizeDataJs = options => (state, item, language, getTranslationTextFunc = getTranslatedItemTexts) => {
  let translation = null
  if (!item) {
    return item
  }
  const id = item[options.idAttribute]
  if (id) {
    translation = getTranslationTextFunc(state, { id, language })
      .map((l, i) => l || options.getDefaultTranslationText(id, item[options.nameAttribute], i ? language : ''))
      .toArray().join(' - ')
  }
    // console.log(id, translation, item)
  return { ...item, [options.nameAttribute]: translation || item[options.nameAttribute] }
}

const localizeDataMap = options => (state, item, language, getTranslationTextFunc = getTranslatedItemTexts) => {
  let translation = null
  if (!item) {
    return item
  }

  const id = item.get(options.idAttribute)
  if (id) {
    translation = getTranslationTextFunc(state, { id, language })
      .map((l, i) => l || options.getDefaultTranslationText(id, item.get(options.nameAttribute), i ? language : ''))
      .toArray().join(' - ')
  }
  // console.log('data', item.toJS(), id, translation)
  return translation ? item.set(options.nameAttribute, translation) : item
}

const sort = (list, options) => {
  const nameAttribute = options.nameAttribute
  if (Iterable.isIterable(list)) {
    return list.sortBy(x => x.get ? x.get(nameAttribute) : x[nameAttribute])
  }
  list.sort((a, b) => a[nameAttribute] < b[nameAttribute] ? -1 : 1)
  return list
}

const localizeListData = options => (state, values, language) => {
  if (values) {
    const resultValues = values.map(item => localizeData(options)(state, item, language))
    return options.isSorted
      ? sort(resultValues, options)
      : resultValues
  }
  return values
}

const getDefaultTranslationText = (id, name) => name

const getOptions = (options, componentProps) => {
  let innerOptions = options || {}
  const single = {
    input: innerOptions.input || 'values'
  }
  single.output = innerOptions.output || single.input
  let inputMapping = innerOptions.inputMapping || []
  inputMapping = [...inputMapping, single] || [single]

  return {
    inputMapping,
    idAttribute: innerOptions.idAttribute || 'id',
    nameAttribute: innerOptions.nameAttribute || 'name',
    isSorted: innerOptions.isSorted,
    getDefaultTranslationText:
      innerOptions.getDefaultTranslationText ||
      componentProps.getDefaultTranslationText ||
      getDefaultTranslationText
  }
}

const getLocale = options => state => options.getItemFunc ? getIntlLocale(state) : null

// localize
export const localizeList = options => InnerComponent => {
  const LocalizedComponent = props => {
    return (
      <InnerComponent {...props} />
    )
  }

  return connect(
    (state, ownProps) => {
      const innerOptions = getOptions(options, ownProps)
      let result = {
        locale: getLocale(innerOptions)(state)
      }
      innerOptions.inputMapping.map(x => {
        result[x.output] = localizeListData(innerOptions)(state, ownProps[x.input], ownProps.language)
      })

      return result
    }
  )(LocalizedComponent)
}

export const localizeItem = options => InnerComponent => {
  const LocalizedComponent = props => {
    return (
      <InnerComponent {...props} />
    )
  }

  return connect(
    (state, ownProps) => {
      const innerOptions = getOptions(options, ownProps)
      let result = {
        locale: getLocale(innerOptions)(state)
      }
      innerOptions.inputMapping.map(x => {
        result[x.output] = localizeData(innerOptions)(state, ownProps[x.input], ownProps.language)
      })

      return result
    }
  )(LocalizedComponent)
}

export const localizeProps = options => InnerComponent => {
  const LocalizedComponent = props => {
    return (
      <InnerComponent {...props} />
    )
  }

  return connect(
    (state, ownProps) => {
      const innerOptions = getOptions(options, ownProps)
      let result = localizeData(innerOptions)(state, ownProps, ownProps.language)
      result.locale = getLocale(state)
      return result
    }
  )(LocalizedComponent)
}

// export const localizeSelector = options => InnerComponent => {
//   const mapSelectors = props => {
//     const innerOptions = getOptions(options, ownProps)
//     let result = { ...props }
//     innerOptions.inputMapping.map(selectorName => {
//       const selector = innerOptions.selector && (props[innerOptions.selector] || innerOptions.selectorFunc)
//       if (selector) {
//         result[selectorName] = (selectorState, selectorProps) => {
//           const data = selector(selectorState, selectorProps)
//           return localizeData(innerOptions)(selectorState, data, props.language)
//         }
//       }
//     })

//     return result
//   }

//   const LocalizedComponent = props => {
//     const mappedProps = mapSelectors(props)
//     return (
//       <InnerComponent {...mappedProps} />
//     )
//   }

//   return LocalizedComponent
// }
