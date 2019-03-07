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
import React from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
// import { getIntlLocale } from 'Intl/Selectors'
import { createSelectors } from './selectors'
import { getOptions } from './configuration'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'

// const getLocale = options => state => options.getItemFunc ? getIntlLocale(state) : null

// localize
export const localizeList = options => InnerComponent => {
  const LocalizedListComponent = props => {
    return (
      <InnerComponent {...props} />
    )
  }

  const innerOptions = getOptions(options, {})
  const translationSelectors = createSelectors(innerOptions)

  return compose(
    withLanguageKey,
    connect(
      (state, ownProps) => {
        let result = {}
        //   locale: getLocale(innerOptions)(state)
        // }
        // console.log(ownProps)
        innerOptions.inputMapping.map(x => {
          result[x.output] = translationSelectors.getTranslatedList(state, { ...ownProps, values: ownProps[x.input] })
        })

        return result
      }
    )
  )(LocalizedListComponent)
}

export const localizeItem = options => InnerComponent => {
  const LocalizedItemComponent = props => {
    return (
      <InnerComponent {...props} />
    )
  }

  const mapStateToProps = () => {
    const innerOptions = getOptions(options, {})
    const translationSelectors = createSelectors(innerOptions)

    return (state, ownProps) => {
      // const innerOptions = getOptions(options, ownProps)
      let result = {
        // locale: getLocale(innerOptions)(state)
      }
      // console.log(ownProps)
      innerOptions.inputMapping.map(x => {
        result[x.output] = translationSelectors.getTranslatedItem(state, { ...ownProps, item: ownProps[x.input] })
      })

      return result
    }
  }

  return compose(
    withLanguageKey,
    connect(
      mapStateToProps()
    )
  )(LocalizedItemComponent)
}

export const localizeProps = options => InnerComponent => {
  const LocalizedPropsComponent = props => {
    return (
      <InnerComponent {...props} />
    )
  }

  const innerOptions = getOptions(options, {})
  const translationSelectors = createSelectors(innerOptions)

  return compose(
    withLanguageKey,
    connect(
      (state, ownProps) => {
        // const innerOptions = getOptions(options, ownProps)
        let result = translationSelectors.getTranslatedProps(state, ownProps)

        // result.locale = getLocale(state)
        // console.log('localizeProps', ownProps, result, options)
        return result
      }
    )
  )(LocalizedPropsComponent)
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
