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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withState from 'util/withState'
import { mergeInUIState } from 'reducers/ui'
import { cancelReview } from './actions'
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button } from 'sema-ui-components'
import styles from './styles.scss'

const messages = defineMessages({
  dialogText: {
    id: 'MassTool.CancelReviewDialog.Text',
    defaultMessage: 'Are you sure you want to cancel review process?'
  },
  dialogDescription: {
    id: 'MassTool.CancelReviewDialog.Description',
    defaultMessage: 'If you cancel, you will lose the information done during the mass publishing process.'
  },
  dialogActionConfirm: {
    id: 'MassTool.CancelReviewDialog.Confirm.Title',
    defaultMessage: 'Kyllä',
    description: 'Components.Buttons.Accept'
  },
  dialogActionCancel: {
    id: 'MassTool.CancelReviewDialog.Cancel.Title',
    defaultMessage: 'Peruuta',
    description: 'Components.Buttons.Cancel'
  }
})

const CancelReviewDialog = ({
  updateUI,
  isOpen,
  cancelReview,
  mergeInUIState,
  intl: { formatMessage }
}) => {
  const handleCancelAction = () => {
    mergeInUIState({
      key: 'cancelReviewDialog',
      value: {
        isOpen: false
      }
    })
  }

  const handleConfirmAction = () => {
    cancelReview()
    mergeInUIState({
      key: 'cancelReviewDialog',
      value: {
        isOpen: false
      }
    })
  }
  return (
    <ModalDialog name='cancelReviewDialog'
      title={formatMessage(messages.dialogText)}
      description={formatMessage(messages.dialogDescription)}>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={handleConfirmAction}>
            {formatMessage(messages.dialogActionConfirm)}</Button>
          <Button small secondary onClick={handleCancelAction}>
            {formatMessage(messages.dialogActionCancel)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
  )
}

CancelReviewDialog.propTypes = {
  updateUI: PropTypes.func,
  isOpen: PropTypes.bool,
  cancelReview: PropTypes.func,
  mergeInUIState: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(null, {
    cancelReview,
    mergeInUIState
  })
)(CancelReviewDialog)
