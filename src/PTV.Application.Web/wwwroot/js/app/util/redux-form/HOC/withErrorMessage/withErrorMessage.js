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
import React from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { List } from 'immutable'
import { fieldPropTypes } from 'redux-form/immutable'
import { validationMessageTypes } from 'util/redux-form/validators/types'
import { getMessages, isErrorWarning } from './functions'
import WarningComponent from './Warnings'
import withContentLanguage from 'util/redux-form/HOC/withContentLanguage'

const getWarning = (meta, language, formatMessage, warnings) => {
  const formWarnings = getMessages(meta.warning, language, formatMessage, validationMessageTypes.visible)
  if (formWarnings && formWarnings.size || warnings && warnings.size) {
    return (
      <WarningComponent
        formWarnings={formWarnings}
        formatMessage={formatMessage}
        otherWarnings={warnings}
      />
    )
  }
  return null
}

const getError = (meta, language, formatMessage, props) => {
  const errors = (meta.dirty || meta.submitFailed || meta.touched) &&
    getMessages(meta.error, language, formatMessage, null) || List()
  let warningsAsErrors = isErrorWarning(meta, props) &&
    getMessages(meta.warning, language, formatMessage, validationMessageTypes.asError) || List()

  const all = errors
    .concat(warningsAsErrors)
    .concat(getMessages(meta.warning, language, formatMessage, validationMessageTypes.asErrorVisible))
  return all && all.size ? all : null
}

const withErrorMessage = WrappedComponent => {
  const ErrorMessageComponent = ({
    meta = {},
    hideMessages,
    intl: { formatMessage },
    warnings,
    ...rest
  }) => {
    // meta.error && console.log(rest, meta.error, rest)
    const error = !hideMessages && getError(meta, rest.language, formatMessage, rest) || null
    const warning = !hideMessages && getWarning(meta, rest.language, formatMessage, warnings) || null
    return (
      <WrappedComponent
        error={!!error}
        errorMessage={error}
        warning={warning}
        meta={meta}
        {...rest}
      />
    )
  }
  ErrorMessageComponent.propTypes = {
    input: fieldPropTypes.input,
    meta: PropTypes.shape({
      warning: PropTypes.oneOfType([
        PropTypes.object,
        PropTypes.string,
        PropTypes.array
      ]),
      ...fieldPropTypes.meta
    }),
    intl: intlShape,
    hideMessages: PropTypes.bool,
    warnings: ImmutablePropTypes.list
  }
  return compose(
    injectIntl,
    withContentLanguage
  )(ErrorMessageComponent)
}

export default withErrorMessage
