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
import shortid from 'shortid'
import * as ValidationHelper from './PTVValidations'
import * as PTVValidatorTypes from './PTVValidators'
import { injectIntl } from 'react-intl'
import cx from 'classnames'
import deepEqual from 'lodash/isEqual'

export const renderOptions = ({ optionRenderer, useFormatMessageData, option, intl }) => {
  if (optionRenderer) {
    return optionRenderer(option)
  }
  if (useFormatMessageData) {
    const { formatMessage } = intl
    const message = option.message && option.message.id && option.message.defaultMessage
			? option.message
			: { id: option.id, defaultMessage: option.name }

    return formatMessage(message)
  }
  return option.name
}

export const renderOptionsImmutable = ({ optionRenderer, useFormatMessageData, option, intl }) => {
  if (optionRenderer) {
    return optionRenderer(option)
  }
  if (useFormatMessageData) {
    const { formatMessage } = intl
    const message = option.get('message') && option.get('message').get('id') && option.get('message').get('defaultMessage')
			? option.get('message')
			: { id: option.get('id'), defaultMessage: option.get('name') }

    return formatMessage(message)
  }
  return option.get('name')
}

export function getRequiredLabel (props, label, ownValidators) {
  let changedLabel = label || props.label

  if (props.disabled) {
    return changedLabel
  }

  let validators = props.validators
  if (ownValidators) {
    validators = props.validators ? props.validators.concat(ownValidators) : ownValidators
  }

  if (!props.readOnly && validators != null && validators.length > 0) {
    		return ValidationHelper.getRequiredLabel(validators, changedLabel, props.skipAsterisk)
    	}

  return changedLabel
}

export function composePTVComponent (ComponentToCompose) {
  class ComposedComponent extends Component {

    componentWillMount () {
      if (this.isControlValidated && this.context.ValidationManager != null && !this.props.readOnly && !this.props.disabled) {
        this.context.ValidationManager.attachComponentToParent(this)
      }
    }

    componentWillReceiveProps (props) {
      if (this.props.readOnly != props.readOnly) {
        if (props.readOnly && this.isControlValidated && this.context.ValidationManager) {
          this.context.ValidationManager.detachComponentFromParent(this.uniqueId)
        }
        if (!props.readOnly && !props.disabled && this.isControlValidated && this.context.ValidationManager) {
          this.context.ValidationManager.attachComponentToParent(this)
        }
      } else if (!props.readOnly && !props.disabled && this.props.validators != props.validators) {
        if (props.validators && props.validators.length > 0) {
          this.isControlValidated = true
          this.context.ValidationManager.attachComponentToParent(this)
        } else {
          this.context.ValidationManager.detachComponentFromParent(this.uniqueId)
          this.isControlValidated = false
          this.state.message = ''
          this.state.isValid = true
        }
      }
    }

    constructor (props) {
      super(props)
      this.isControlValidated = this.props.validators != null && this.props.validators.length > 0
      this.uniqueId = shortid.generate()
      this.state = { isValid: false, isTouched: false, message: '', errorClass: '', forceValidation: false }
      this.validate = this.validate.bind(this)
      this.validateComponent = this.validateComponent.bind(this)
      this.validatedField = this.props.validatedField || this.props.label
    }

    componentWillUnmount () {
      if (this.isControlValidated && this.context.ValidationManager) {
        this.context.ValidationManager.detachComponentFromParent(this.uniqueId)
      }
    }

    validateComponent () {
      this.setState({ forceValidation: !this.state.forceValidation })
    }

    validate (value) {
      if (this.isControlValidated && this.props.validators) {
        let isValid = this.props.validators.map((validator, index) => {
          switch (validator.rule) {
            case PTVValidatorTypes.IS_REQUIRED.rule:
              return ValidationHelper.IsRequired(value, validator.errorMessage)
            case PTVValidatorTypes.IS_MORE_THAN_MAX.rule:
              return ValidationHelper.isMoreThanMax(value, this.props.maxLength || 2500,
                { ...validator.errorMessage,
                  formatArguments: { maximumOfAllowedCharacters : this.props.maxLength || 2500 } })
            case PTVValidatorTypes.IS_ITEM_MORE_THAN_MAX.rule:
              return ValidationHelper.isItemMoreThanMax(value, this.props.maxLength || 150,
                { ...validator.errorMessage,
                  formatArguments: { maximumOfAllowedCharacters : this.props.maxLength || 150 } })
            case PTVValidatorTypes.IS_NOT_EMPTY.rule:
              return ValidationHelper.IsNotEmpty(value, validator.errorMessage)
            case PTVValidatorTypes.IS_EMAIL.rule:
              return ValidationHelper.IsEmail(value, validator.errorMessage)
            case PTVValidatorTypes.IS_URL.rule:
              return ValidationHelper.IsUrl(value, validator.errorMessage)
            case PTVValidatorTypes.IS_POSTALCODE.rule:
              return ValidationHelper.IsPostalCode(value, validator.errorMessage)
            case PTVValidatorTypes.IS_BUSINESSID.rule:
              return ValidationHelper.IsBusinessId(value, validator.errorMessage)
            case PTVValidatorTypes.IS_DATETIME.rule:
              return ValidationHelper.IsValidDateTime(value, validator.errorMessage)
            case PTVValidatorTypes.IS_REQUIRED_NOT_EMPTY_GUID.rule:
              return ValidationHelper.IsRequiredNotEmptyGuid(value, validator.errorMessage)
          }
        })

        if (isValid) {
          let result = { isValid: true, message: '' }
          isValid.forEach((validator) => {
            if (!validator.valid) {
              result.isValid = false
              result.message = validator.reason
              this.setState({ isValid: false, isTouched: true })
              return
            }
          })

          this.state.message = result.message
          this.setState({ isValid: result.isValid, isTouched: true })
        }

        this.context.ValidationManager.notifyMe(this)
      }
    }

    render () {
      let className = cx(this.props.className, 'composed-component')
      if (this.isControlValidated && !this.props.readOnly) {
        className = this.state.isValid || !this.state.isTouched ? className : cx(className, 'has-error')
      }

      return (<ComponentToCompose {...this.props}
        {...this.state}
        className={className}
        isValid={this.state.isValid}
        validate={this.validate} />)
    }
    }

  ComposedComponent.contextTypes = {
    ValidationManager: React.PropTypes.object
  }

  ComposedComponent.propTypes = {
    readOnly: React.PropTypes.bool
  }

  ComposedComponent.defaultProps = {
    readOnly: false
  }

  return injectIntl(ComposedComponent)
}

class ValidatePTV extends Component {

  constructor (props) {
    super(props)
  }

  componentWillReceiveProps (nextProps) {
    if (this.props.validate && typeof this.props.validate === 'function') {
      if (!deepEqual(this.props.valueToValidate, nextProps.valueToValidate) || (this.props.forceValidation != nextProps.forceValidation)) {
        this.props.validate(nextProps.valueToValidate)
      }
    } else {
      console.error('Component for validation must be composed!')
    }
  }

  render () {
    return (this.props.isValid || !this.props.isTouched ? null : <p className='has-error'>{this.props.intl.formatMessage(this.props.message, this.props.message.formatArguments)}</p>)
  }
}

export const ValidatePTVComponent = injectIntl(ValidatePTV)
