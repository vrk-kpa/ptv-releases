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
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import withState from 'util/withState'

const messages = defineMessages({
  dialogTitle: {
    id: 'AdminMapping.RemoveMappingDialog.Title',
    defaultMessage: 'Oletko varma, että haluat arkistoida organisaation?'
  },
  dialogDescription: {
    id: 'AdminMapping.RemoveMappingDialog.Description',
    defaultMessage: 'Organisaation tiedot, palvelut ja asiontikanavat arkistoidaan.'
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
  mergeInUIState({
    key: 'removeMappingDialogParams',
    value: {
      ptvId:null,
      sahaId:null,
      selectedOrganizations:null
    }
  })
}

const RemoveMappingDialog = ({
  mergeInUIState,
  confirmAction,
  ptvId,
  sahaId,
  selectedOrganizations,
  intl: { formatMessage }
}) => {
  const dialogKey = 'removeMappingDialog'
  const handleConfirmAction = () => {
    confirmAction(ptvId, sahaId, selectedOrganizations)
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
      contentLabel='RemoveMappingDialog'
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

RemoveMappingDialog.propTypes = {
  mergeInUIState: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(state => ({
  }), {
    mergeInUIState
  }),
  withState({
    redux: true,
    key: 'removeMappingDialogParams',
    initialState: {
      ptvId: null,
      sahaId: null,
      selectedOrganizations: null
    }
  }),
)(RemoveMappingDialog)
