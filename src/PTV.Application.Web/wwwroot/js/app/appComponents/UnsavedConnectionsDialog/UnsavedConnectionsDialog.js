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
import { defineMessages, injectIntl } from 'react-intl'
import { Modal, ModalActions, ModalContent, ModalTitle, Button } from 'sema-ui-components'
import { compose } from 'redux'
import withState from 'util/withState'
import styles from './styles.scss'

const messages = defineMessages({
  dialogTitle: {
    id: 'AppComponents.UnsavedConnectionsDialog.Title',
    defaultMessage: 'Oletko varma, että haluat aloittaa alusta?'
  },
  dialogText: {
    id: 'AppComponents.UnsavedConnectionsDialog.Text',
    defaultMessage: 'Menetät kaikki tallentamattomat tiedot. Työpöytä ja hakutulokset tyhjenevät.'
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

const UnsavedConnectionsDialog = (
  { intl: { formatMessage },
  updateUI,
  isOpen,
  successAction,
  ...rest
}) => {
  const handleCancelAction = () => {
    return updateUI('isOpen', false)
  }
  const handleConfirmAction = () => {
    updateUI('isOpen', false)
    successAction && typeof successAction === 'function' && successAction()
  }
  return <Modal
    isOpen={isOpen}
    onRequestClose={handleCancelAction}
    contentLabel='Unsaved connections changes dialog'
    style={{ content: { maxWidth: '30rem', minWidth: 'auto' } }}
  >
    <ModalTitle title={formatMessage(messages.dialogTitle)} />
    <ModalContent>
      {formatMessage(messages.dialogText)}
    </ModalContent>
    <ModalActions>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleConfirmAction}>{formatMessage(messages.confirmButtonTitle)}</Button>
        <Button link onClick={handleCancelAction}>{formatMessage(messages.closeButtonTitle)}</Button>
      </div>
    </ModalActions>
  </Modal>
}

UnsavedConnectionsDialog.propTypes = {
  intl: PropTypes.object.isRequired,
  updateUI: PropTypes.func.isRequired,
  isOpen: PropTypes.bool.isRequired,
  successAction: PropTypes.func
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: 'UnsavedConnectionsDialog',
    initialState: {
      isOpen: false
    }
  })
)(UnsavedConnectionsDialog)
