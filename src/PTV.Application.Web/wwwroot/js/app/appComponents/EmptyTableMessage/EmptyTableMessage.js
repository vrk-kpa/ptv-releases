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
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import PropTypes from 'prop-types'
import styles from './styles.scss'
import cx from 'classnames'

export const { defaultEmptyMessage } = defineMessages({
  defaultEmptyMessage: {
    id: 'AppComponents.EmptyTableMessage.DefaultEmptyMessage',
    defaultMessage: 'No data'
  }
})

const EmptyTableMessage = ({
  message,
  colSpan,
  intl: { formatMessage },
  children,
  className
}) => {
  const messageClass = cx(
    styles.message,
    className
  )
  return (
    <tbody>
      <tr>
        <td colSpan={colSpan} className={messageClass}>
          {message || children || formatMessage(defaultEmptyMessage)}
        </td>
      </tr>
    </tbody>
  )
}
EmptyTableMessage.propTypes = {
  message: PropTypes.string,
  colSpan: PropTypes.number,
  intl: intlShape,
  children: PropTypes.any,
  className: PropTypes.string
}

export default compose(
  injectIntl
)(EmptyTableMessage)
