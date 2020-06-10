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
// import React from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Iterable } from 'immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'

export const formatOptions = (options, formatMessage) => {
  return Object.keys(options).reduce((acc, curr) => {
    if (typeof options[curr] === 'object') {
      acc[curr] = localizeMessage(options[curr], null, formatMessage)
    } else {
      acc[curr] = options[curr]
    }
    return acc
  }, {})
}

export const localizeMessage = (item, options, formatMessage) =>
  item && item.id && (
    options
      ? formatMessage(item, formatOptions(options, formatMessage))
      : formatMessage(item)
  ) || item

const getOptions = (error, language) => {
  return typeof error.options === 'function'
    ? error.options(error, language)
    : error.options
}

const getMessage = (error, formatMessage, language) => {
  if (!error) {
    return ''
  }

  const errorObject = error && Iterable.isIterable(error) ? error.toJS() : error
  return localizeMessage(errorObject.message, getOptions(errorObject, language), formatMessage)
}

const ValidationMessage = ({ intl: { formatMessage }, language, error }) =>
  getMessage(error, formatMessage, language)

ValidationMessage.propTypes = {
  intl: intlShape,
  language: PropTypes.string,
  error: PropTypes.oneOfType([
    PropTypes.object,
    ImmutablePropTypes.map
  ])
}

export default compose(
  injectIntl
)(ValidationMessage)
