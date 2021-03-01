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
import {
  Modal,
  ModalActions,
  ModalTitle,
  ModalContent,
  Button
} from 'sema-ui-components'
import { messages } from './messages'
import styles from './styles.scss'
import { injectIntl, intlShape } from 'util/react-intl'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import withState from 'util/withState'
import { addressTypesEnum } from 'enums'

const AddressTypeDialog = props => {
  const {
    intl: { formatMessage },
    isOpen,
    updateUI,
    onConfirm,
    selectedType
  } = props

  const hideDialog = () => {
    updateUI({
      'isOpen': false,
      selectedType
    })
  }

  const handleCancel = () => {
    hideDialog()
  }

  const handleConfirm = () => {
    onConfirm && onConfirm(selectedType)
    hideDialog()
  }

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={handleCancel}
      contentLabel='Address_Type_Dialog'
    >
      <ModalTitle title={formatMessage(messages.dialogTitle)} />
      <ModalContent>
        <p className={styles.content}>{formatMessage(messages.dialogDescription)}</p>
      </ModalContent>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={handleConfirm}>
            {formatMessage(messages.dialogConfirmTitle)}</Button>
          <Button small secondary onClick={handleCancel}>
            {formatMessage(messages.dialogCancelTitle)}
          </Button>
        </div>
      </ModalActions>
    </Modal>
  )
}

AddressTypeDialog.propTypes = {
  intl: intlShape.isRequired,
  isOpen: PropTypes.bool,
  updateUI: PropTypes.func,
  onConfirm: PropTypes.func,
  selectedType: PropTypes.string
}

export default compose(
  injectIntl,
  withState({
    key: 'addressTypeDialog',
    initialState: {
      isOpen: false,
      selectedType: addressTypesEnum.STREET
    },
    redux: true
  })
)(AddressTypeDialog)
