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
import { injectIntl, intlShape } from 'util/react-intl'
import {
  Modal,
  ModalActions,
  ModalTitle,
  ModalContent,
  Button
} from 'sema-ui-components'
import withState from 'util/withState'
import styles from './styles.scss'

const NavigationDialog = ({
  intl: { formatMessage },
  isOpen,
  updateUI,
  confirmAction,
  defaultAction,
  dialogTitle,
  dialogDescription,
  dialogConfirmTitle,
  dialogCancelTitle,
  navigateTo,
  external,
  history,
  ...rest
}) => {
  const hideDialog = () => {
    updateUI({
      'isOpen': false,
      navigateTo: null,
      defaultAction: null,
      external: false
    })
  }
  const handleConfirm = () => {
    if (!external) {
      defaultAction && typeof defaultAction === 'function' && defaultAction()
      confirmAction && typeof confirmAction === 'function' && confirmAction()
    }
    if (navigateTo) {
      if (external) {
        window.location.href = navigateTo
      } else {
        history.push(navigateTo)
      }
    }
    hideDialog()
  }

  const handleCancel = () => {
    hideDialog()
  }

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={handleCancel}
      contentLabel='Navigation_Dialog'
    >
      <ModalTitle title={formatMessage(dialogTitle)} />
      {dialogDescription && <ModalContent>
        <p className={styles.content}>{formatMessage(dialogDescription)}</p>
      </ModalContent>}
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={handleConfirm}>
            {formatMessage(dialogConfirmTitle)}</Button>
          <Button small secondary onClick={handleCancel}>
            {formatMessage(dialogCancelTitle)}
          </Button>
        </div>
      </ModalActions>
    </Modal>
  )
}

NavigationDialog.propTypes = {
  intl: intlShape.isRequired,
  updateUI: PropTypes.func,
  isOpen: PropTypes.bool,
  confirmAction: PropTypes.func,
  defaultAction: PropTypes.func,
  dialogTitle: PropTypes.object,
  dialogDescription: PropTypes.object,
  dialogConfirmTitle: PropTypes.object,
  dialogCancelTitle: PropTypes.object,
  navigateTo: PropTypes.string,
  external: PropTypes.bool,
  history: PropTypes.shape({
    push: PropTypes.func.isRequired
  })
}

export default compose(
  injectIntl,
  withState({
    key: 'navigationDialog',
    initialState: {
      isOpen: false,
      navigateTo: null,
      defaultAction: null,
      external: false
    },
    redux: true
  })
)(NavigationDialog)
