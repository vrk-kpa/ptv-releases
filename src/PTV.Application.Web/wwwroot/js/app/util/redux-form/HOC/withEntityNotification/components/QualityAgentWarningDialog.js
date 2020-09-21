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
import { connect } from 'react-redux'
import {
  getEntityUnificRoot
} from 'selectors/entities/entities'
import { getHasQualityError } from '../selectors'
import { injectIntl, intlShape } from 'util/react-intl'
import NotificationDialog from './NotificationDialog'
import cx from 'classnames'
import styles from '../styles.scss'
import messages from '../messages'

const QualityAgentDialog = ({
  intl: { formatMessage },
  hasError
}) => {
  if (!hasError) {
    return null
  }
  const dialogClass = cx(
    styles.withBullets,
    styles.withoutDescription
  )
  return (<NotificationDialog
    type='warn'
    asAlert
    title={formatMessage(messages.qualityAgentTitle)}
    className={dialogClass}
    closeLabel={formatMessage(messages.closeLabel)}
  >
    <ul>
      <li>{formatMessage(messages.qualityAqentDescription)}</li>
    </ul>
  </NotificationDialog> || null
  )
}

QualityAgentDialog.propTypes = {
  hasError: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, { unificRootId }) => {
    const id = getEntityUnificRoot(state)
    return {
      hasError: getHasQualityError(state, { id })
    }
  })
)(QualityAgentDialog)
