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
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button } from 'sema-ui-components'
import { clearSelection } from '../actions'
import { mergeInUIState } from 'reducers/ui'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'

const messages = defineMessages({
  dialogTitle: {
    id: 'MassToolCartEmptyDialog.Title',
    defaultMessage: 'Haluatko tyhjentää sisältökorin?'
  },
  dialogDescription: {
    id: 'MassToolCartEmptyDialog.Description',
    defaultMessage: 'Paina Kyllä-nappia, jos haluat tyhjentää sisältökorin. Paina Keskeytä-nappia, jos et halua tyhjentää sisältökoria.' // eslint-disable-line
  },
  actionConfirm: {
    id: 'Components.Buttons.Accept',
    defaultMessage: 'Kyllä'
  },
  actionCancel: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  }
})

const closeDialog = (mergeInUIState, dialogKey) => {
  mergeInUIState({
    key: dialogKey,
    value: {
      isOpen: false
    }
  })
}

const MassToolCartEmptyDialog = ({
  title,
  description,
  clearSelection,
  mergeInUIState,
  intl: { formatMessage }
}) => {
  const dialogKey = 'massToolCartEmptyDialog'
  const handleConfirmAction = () => {
    clearSelection()
    closeDialog(mergeInUIState, dialogKey)
  }
  const handleCancelAction = () => {
    closeDialog(mergeInUIState, dialogKey)
  }
  return (
    <ModalDialog
      name={dialogKey}
      title={formatMessage(messages.dialogTitle)}
      description={formatMessage(messages.dialogDescription)}
      contentLabel='Mass tool cart empty dialog'
      style={{ content: { maxWidth: '30rem', minWidth: 'auto' } }}
    >
      <ModalActions>
        <Button
          small
          onClick={handleConfirmAction}
          children={formatMessage(messages.actionConfirm)}
        />
        <Button
          link
          onClick={handleCancelAction}
          children={formatMessage(messages.actionCancel)}
        />
      </ModalActions>
    </ModalDialog>
  )
}

MassToolCartEmptyDialog.propTypes = {
  title: PropTypes.node,
  description: PropTypes.node,
  clearSelection: PropTypes.func,
  mergeInUIState: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(null, {
    clearSelection,
    mergeInUIState
  })
)(MassToolCartEmptyDialog)
