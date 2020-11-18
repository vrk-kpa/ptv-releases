/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import React, { PureComponent } from 'react'
import { PropTypes } from 'prop-types'

const withValidation = (options = {}) => ComposedComponent => {
  class ComponentValidation extends PureComponent {
    getValidation = (createValidate, value, allValues, props, name) => {
      var validate = createValidate({
        label: options.label || this.props.label,
        fieldProps: this.props
      })
      return validate(value, allValues, props, name)
    }
    getValidate = validators => (value, allValues, props, name) => {
      if (typeof validators === 'function') {
        return this.getValidation(validators, value, allValues, props, name)
      } else if (Array.isArray(validators)) {
        const errors = validators
          .map(createValidate => this.getValidation(createValidate, value, allValues, props, name))
          .filter(x => !!x)
          .reduce((result, error) => {
            if (result) {
              result.errors.concat(error.errors)
              return result
            }
            return error
          }, null)
        return errors && errors.size === 0 ? undefined : errors
      } else {
        console.log('WithValidation props error!')
      }
    }

    render () {
      return this.props.skipValidation &&
        <ComposedComponent
          {...this.props}
        /> ||
        <ComposedComponent
          {...this.props}
          validate={options.validate && this.getValidate(options.validate)}
          // warn={options.warn && this.getValidate(options.warn)}
        />
    }
  }

  ComponentValidation.propTypes = {
    skipValidation: PropTypes.bool,
    getCustomValue: PropTypes.func,
    label: PropTypes.string
  }

  return ComponentValidation
}

export default withValidation
