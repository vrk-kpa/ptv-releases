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
import React, { PureComponent } from 'react'
import { PropTypes } from 'prop-types'

const withValidation = (options = {}) => ComposedComponent => {
  class ComponentValidation extends PureComponent {
    getValidation = (validate, value, allValues, props, name) => {
      var f = validate(options.label, this.props.getCustomValue || (() => value))
      return f(value, allValues, props, name)
    }
    getValidate = (value, allValues, props, name) => {
      if (typeof options.validate === 'function') {
        return this.getValidation(options.validate, value, allValues, props, name)
      } else if (Array.isArray(options.validate)) {
        const errors = options.validate
          .map(validate => this.getValidation(validate, value, allValues, props, name))
          .filter(x => !!x)
        return errors.length === 0 ? undefined : errors
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
          validate={this.getValidate}
        />
    }
  }

  ComponentValidation.propTypes = {
    skipValidation: PropTypes.bool,
    getCustomValue: PropTypes.func
  }

  return ComponentValidation
}

export default withValidation
