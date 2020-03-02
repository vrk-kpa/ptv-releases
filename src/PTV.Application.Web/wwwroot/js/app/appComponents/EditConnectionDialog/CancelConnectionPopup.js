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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Button } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withState from 'util/withState'
import styles from './styles.scss'
import Popup from 'appComponents/Popup'

const messages = defineMessages({
  dialogTitle: {
    id: 'CancelConnectionPopup.Title',
    defaultMessage: 'Are you sure you want to close without saving?'
  },
  dialogText: {
    id: 'Routes.Connections.CancelConnectionPopup.Text',
    defaultMessage: 'All changes will be lost.'
  },
  confirmButtonTitle: {
    id: 'Components.Buttons.Accept',
    defaultMessage: 'Kyllä'
  },
  closeButtonTitle: {
    id: 'Components.Buttons.Cancel',
    defaultMessage: 'Peruuta'
  }
})

const CancelConnectionPopup = (
  { intl: { formatMessage },
    updateUI,
    isOpen,
    onCancelSuccess,
    trigger,
    ...rest
  }) => {
  const handleCancelDialog = () => {
    return updateUI('isOpen', false)
  }
  const cancelSuccess = () => {
    typeof onCancelSuccess === 'function' && onCancelSuccess()
    updateUI('isOpen', false)
  }
  const handleConfirmDialog = () => {
    updateUI('isOpen', false)
    cancelSuccess()
  }
  return <Popup
    trigger={trigger}
    open={isOpen}
    onClose={handleCancelDialog}
    position='top center'
    maxWidth='mW300'
    className={styles.screenCenter}
  >
    <div>
      <h5 className={styles.popupTitle}>{formatMessage(messages.dialogTitle)}</h5>
      <span className={styles.popupContent}>{formatMessage(messages.dialogText)}</span>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleConfirmDialog}>
          {formatMessage(messages.confirmButtonTitle)}</Button>
        <Button small secondary onClick={handleCancelDialog}>
          {formatMessage(messages.closeButtonTitle)}
        </Button>
      </div>
    </div>
  </Popup>
}

CancelConnectionPopup.propTypes = {
  intl: intlShape.isRequired,
  updateUI: PropTypes.func.isRequired,
  isOpen: PropTypes.bool.isRequired,
  onCancelSuccess: PropTypes.func,
  count: PropTypes.number,
  trigger: PropTypes.any
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: 'CancelConnectionPopup',
    initialState: {
      isOpen: false,
      onCancelSuccess: null
    }
  }),
  connect(state => ({
  }), {
  })
)(CancelConnectionPopup)
