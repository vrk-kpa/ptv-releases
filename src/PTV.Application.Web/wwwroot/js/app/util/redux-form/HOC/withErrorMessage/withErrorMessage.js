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
import { compose } from 'redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { Iterable } from 'immutable'
import { fieldPropTypes } from 'redux-form/immutable'

const localizeMessage = (error, options, formatMessage) =>
  error && error.id && (
    options
      ? formatMessage(error, options)
      : formatMessage(error)
  ) || error

const getMessage = (error, formatMessage) => {
  if (Array.isArray(error)) {
    error = error[0]
  }
  return error && (
    Iterable.isIterable(error)
      ? localizeMessage(error.get('message'), error.get('options'), formatMessage)
      : localizeMessage(error.message, error.options, formatMessage)
  ) || ''
}
const isLanguageError = (error, language) => {
  if (!language) {
    return error
  }
  const languages = Iterable.isIterable(error) ? error.get('languages') : error.languages
  return !languages || languages.includes(language)
}
const isError = (meta, input, showError, language) => {
  return showError // Field using RenderTextField provided showError behavior
    ? typeof showError === 'function'
      ? showError(meta, input) // Calculate value
      : showError // Use provided value
    // Default behavior
    : ((meta.dirty || meta.submitFailed) && meta.error && isLanguageError(meta.error, language)) ||
  // warnings are used for publishing errors, not blocking submit
  (!meta.visited && meta.warning && isLanguageError(meta.warning, language))
}
const withErrorMessage = WrappedComponent => {
  const ErrorMessageComponent = ({
    meta = {},
    showError,
    intl: { formatMessage },
    ...rest
  }) => {
    // meta.error && console.log(rest, meta.error, rest)

    return (
      <WrappedComponent
        error={!!isError(meta, rest.input, showError, rest.language)}
        errorMessage={getMessage(meta.error || meta.warning, formatMessage)}
        meta={meta}
        {...rest}
      />
    )
  }
  ErrorMessageComponent.propTypes = {
    input: fieldPropTypes.input,
    meta: fieldPropTypes.meta,
    intl: intlShape,
    showError: PropTypes.oneOfType([
      PropTypes.bool,
      PropTypes.func
    ])
  }
  return compose(
    injectIntl
  )(ErrorMessageComponent)
}

export default withErrorMessage
