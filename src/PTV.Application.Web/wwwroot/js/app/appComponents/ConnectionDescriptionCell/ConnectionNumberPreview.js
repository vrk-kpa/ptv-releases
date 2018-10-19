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
import { connect } from 'react-redux'
import { getDialCode } from './selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  isLocalNumber: {
    id: 'ReduxForm.Renders.RenderIsLocalNumber.title',
    defaultMessage: 'Palvelunumero, johon ei voi soittaa ulkomailta.'
  }
})

const ConnectionNumberPreview = ({
  number,
  dialCode,
  intl: { formatMessage },
  ...rest
}) => {
  const phoneNumber = number.get('phoneNumber') || ''
  const isLocal = number.get('isLocalNumber')
  return (
    isLocal
    ? <div className={styles.previewItem}>
      {`${phoneNumber} (${formatMessage(messages.isLocalNumber)})`}
    </div>
    : <div className={styles.previewItem}>
      {`${dialCode} ${phoneNumber}`}
    </div>
  )
}

ConnectionNumberPreview.propTypes = {
  number: PropTypes.array,
  dialCode: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    dialCode: getDialCode(state, ownProps)
  }))
)(ConnectionNumberPreview)
