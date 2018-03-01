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
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Map, Iterable } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { getIsFormDisabled } from 'selectors/formStates'
import { injectFormName } from 'util/redux-form/HOC'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'

const asLocalizable = WrappedComponent => {
  const InnerComponent = ({
    isLocalized,
    language,
    comparisionLanguageCode,
    isDisabled,
    ...rest
  }) => {
    const localizedOnChange = (input, isCompareField = false) => newValue => {
      const value = input.value || Map()
      input.onChange(
        value.set(language, newValue)
      )
    }

    const localizedOnBlur = (input, isCompareField = false) => (event, newValue, previousValue) => {
      const value = input.value || Map()
      input.onBlur(value)
    }

    const getLocalizedValue = (value, defaultValue = null, isCompareField = false) => {
      if (isDisabled) {
        return ''
      } else if (value && Iterable.isIterable(value)) {
        return value.get(language) || defaultValue
      } else {
        return defaultValue
      }
    }
    return isLocalized && (
      <WrappedComponent
        {...rest}
        localizedOnChange={localizedOnChange}
        localizedOnBlur={localizedOnBlur}
        getCustomValue={getLocalizedValue}
        getValue={getLocalizedValue}
        language={language}
      />
    ) || <WrappedComponent {...rest} />
  }
  InnerComponent.propTypes = {
    language: PropTypes.string.isRequired,
    isLocalized: PropTypes.bool
  }
  InnerComponent.defaultProps = {
    language: 'fi',
    isLocalized: true
  }
  return compose(
    injectFormName,
    withLanguageKey,
    connect(
      (state, { formName, compare, languageKey }) => {
        const language = getContentLanguageCode(state, { formName, languageKey }) || 'fi'
        const comparisionLanguageCode = getSelectedComparisionLanguageCode(state)
        return {
          language: compare && comparisionLanguageCode || language
        }
      }
    )
  )(InnerComponent)
}

export default asLocalizable
