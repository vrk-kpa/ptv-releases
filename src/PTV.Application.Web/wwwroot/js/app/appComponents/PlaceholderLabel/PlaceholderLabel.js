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
import { compose } from 'redux'
import { Label } from 'sema-ui-components'
import { injectIntl, intlShape } from 'util/react-intl'
import cx from 'classnames'
import styles from './styles.scss'

const PlaceholderLabel = ({
  placeholder,
  padded,
  className,
  intl: { formatMessage },
  ...rest
}) => {
  const placeholderClass = cx(
    styles.placeholder,
    {
      [styles.padded]: padded
    },
    className
  )
  const formatText = text => text && typeof text === 'object' && text.id && formatMessage(text) || text
  return (
    <Label
      infoLabel
      className={placeholderClass}
      labelText={formatText(placeholder)}
      {...rest}
    />
  )
}

PlaceholderLabel.propTypes = {
  placeholder: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  padded: PropTypes.bool,
  className: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl
)(PlaceholderLabel)
