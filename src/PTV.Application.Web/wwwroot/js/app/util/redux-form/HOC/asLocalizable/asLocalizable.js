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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { Map, Iterable } from 'immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import withContentLanguage from 'util/redux-form/HOC/withContentLanguage'

const asLocalizable = WrappedComponent => {
  class InnerComponent extends Component {
    customOnChange = (input, isCompareField = false) => newValue => {
      const value = input.value || Map()
      input.onChange(
        value.set(this.props.language, newValue)
      )
    }

    customOnBlur = (input, isCompareField = false) => (event, newValue, previousValue) => {
      const value = input.value || Map()
      input.onBlur(value)
    }

    getLocalizedValue = (value, defaultValue = null, isCompareField = false) => {
      if (this.props.isDisabled) {
        return ''
      } else if (value && Iterable.isIterable(value)) {
        return value.get(this.props.language) || defaultValue
      } else {
        return defaultValue
      }
    }

    render () {
      const {
        isLocalized,
        language,
        ...rest
      } = this.props

      return isLocalized && (
        <WrappedComponent
          {...rest}
          customOnChange={this.customOnChange}
          customOnBlur={this.customOnBlur}
          getCustomValue={this.getLocalizedValue}
          getValue={this.getLocalizedValue}
          language={language}
        />
      ) || <WrappedComponent {...rest} />
    }
  }
  InnerComponent.propTypes = {
    language: PropTypes.string.isRequired,
    isLocalized: PropTypes.bool,
    isDisabled: PropTypes.bool
  }
  InnerComponent.defaultProps = {
    language: 'fi',
    isLocalized: true
  }
  return compose(
    injectFormName,
    withLanguageKey,
    withContentLanguage
  )(InnerComponent)
}

export default asLocalizable
