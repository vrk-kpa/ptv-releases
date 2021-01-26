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
import { Modal, ModalActions, ModalContent, ModalTitle, Button } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withState from 'util/withState'
import styles from './styles.scss'

const messages = defineMessages({
  dialogTitle: {
    id: 'Routes.Connections.OrganizationConnectionsDialog.Title',
    defaultMessage: 'Do you want to edit all organization connections?'
  },
  dialogText: {
    id: 'Routes.Connections.OrganizationConnectionsDialog.Text',
    defaultMessage: 'There are {count} services with connections. This operation can take longer time.'
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

const EditOrganizationConnectionDialog = (
  { intl: { formatMessage },
    updateUI,
    isOpen,
    onCancelSuccess,
    count,
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
  return <Modal
    isOpen={isOpen}
    onRequestClose={handleCancelDialog}
    contentLabel='Edit Organization Connection Dialog'
    style={{ content: { maxWidth: '30rem', minWidth: 'auto' } }}
  >
    <ModalTitle title={formatMessage(messages.dialogTitle)} />
    <ModalContent>
      {formatMessage(messages.dialogText, {count:count})}
    </ModalContent>
    <ModalActions>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleConfirmDialog}>{formatMessage(messages.confirmButtonTitle)}</Button>
        <Button link onClick={handleCancelDialog}>{formatMessage(messages.closeButtonTitle)}</Button>
      </div>
    </ModalActions>
  </Modal>
}

EditOrganizationConnectionDialog.propTypes = {
  intl: intlShape.isRequired,
  updateUI: PropTypes.func.isRequired,
  isOpen: PropTypes.bool.isRequired,
  onCancelSuccess: PropTypes.func,
  count: PropTypes.number
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: 'editOrganizationConnectionDialog',
    initialState: {
      isOpen: false
    }
  }),
  connect(state => ({
  }), {
  })
)(EditOrganizationConnectionDialog)
