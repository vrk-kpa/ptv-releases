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
import { connect } from 'react-redux'
import { injectFormName } from 'util/redux-form/HOC'
import withState from 'util/withState'
import { setReadOnly, setIsAddingNewLanguage, setNewlyAddedLanguage, setCompareMode } from 'reducers/formStates'
import { setComparisionLanguage, clearContentLanguage, setSelectedEntity } from 'reducers/selections'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { browserHistory } from 'react-router'
import { apiCall3 } from 'actions'
import { reset, initialize } from 'redux-form/immutable'
import { formActions, formActionsTypesEnum, formEntityTypes } from 'enums'
import { getAutomaticSavedState } from 'selectors/automaticSave'
import styles from './styles.scss'

const messages = defineMessages({
  dialogTitle: {
    id: 'Containers.Channels.ElectronicChannel.Cancel.Title',
    defaultMessage: 'Haluatko keskeyttää tietojen lisäämisen?'
  },
  dialogText: {
    id: 'Containers.Channels.ElectronicChannel.Cancel.Text',
    defaultMessage: 'Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat.'
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

const CancelDialog = (
  { intl: { formatMessage },
  name,
  formName,
  setNewlyAddedLanguage,
  setReadOnly,
  setIsAddingNewLanguage,
  setCompareMode,
  setComparisionLanguage,
  clearContentLanguage,
  setSelectedEntity,
  updateUI,
  entityId,
  isOpen,
  resetForm,
  initializeForm,
  apiCall3,
  reloadForm,
  automaticSaveState,
  ...rest
}) => {
  const handleCancelCancelDialog = () => {
    return updateUI('isOpen', false)
  }
  const cancelSuccess = () => {
    setReadOnly({
      form: formName,
      value: true
    })
    setIsAddingNewLanguage({
      form: formName,
      value: false
    })
    setCompareMode({
      form: formName,
      value: false
    })
    setComparisionLanguage({
      id: null,
      code: null
    })
    updateUI('isOpen', false)
  }
  const handleConfirmCancelDialog = () => {
    updateUI('isOpen', false)
    setIsAddingNewLanguage({
      form: formName,
      value: false
    })
    setCompareMode({
      form: formName,
      value: false
    })
    setComparisionLanguage({
      id: null,
      code: null
    })
    clearContentLanguage()
    setSelectedEntity({ id: null })
    if (!entityId) {
      browserHistory.push('/frontpage/search')
    } else {
      apiCall3({
        keys: [formEntityTypes[formName], 'loadEntity'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.UNLOCKENTITY][formName],
          data: { id: entityId }
        },
        formName,
        successNextAction: cancelSuccess
      })
      initializeForm(formName, automaticSaveState)
    }
  }
  return <Modal
    isOpen={isOpen}
    onRequestClose={handleCancelCancelDialog}
    contentLabel='Entity cancel dialog'
    style={{ content: { maxWidth: '30rem', minWidth: 'auto' } }}
  >
    <ModalTitle title={formatMessage(messages.dialogTitle)} />
    <ModalContent>
      {formatMessage(messages.dialogText)}
    </ModalContent>
    <ModalActions>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleConfirmCancelDialog}>{formatMessage(messages.confirmButtonTitle)}</Button>
        <Button link onClick={handleCancelCancelDialog}>{formatMessage(messages.closeButtonTitle)}</Button>
      </div>
    </ModalActions>
  </Modal>
}

CancelDialog.propTypes = {
  name: PropTypes.string.isRequired,
  intl: PropTypes.object.isRequired,
  updateUI: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  setNewlyAddedLanguage: PropTypes.func.isRequired,
  setReadOnly: PropTypes.func.isRequired,
  setIsAddingNewLanguage: PropTypes.func.isRequired,
  setCompareMode: PropTypes.func.isRequired,
  setComparisionLanguage: PropTypes.func.isRequired,
  clearContentLanguage: PropTypes.func.isRequired,
  setSelectedEntity: PropTypes.func.isRequired,
  entityId: PropTypes.string,
  isOpen: PropTypes.bool.isRequired,
  resetForm: PropTypes.func.isRequired,
  reloadForm: PropTypes.func.isRequired,
  apiCall3: PropTypes.func.isRequired,
  initializeForm: PropTypes.func,
  automaticSaveState: PropTypes.func
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: ({ formName }) => `${formName}CancelDialog`,
    initialState: {
      isOpen: false
    }
  }),
  connect(state => ({
    entityId: getSelectedEntityId(state),
    automaticSaveState: getAutomaticSavedState(state)
  }), {
    setNewlyAddedLanguage,
    setReadOnly,
    setIsAddingNewLanguage,
    setCompareMode,
    setComparisionLanguage,
    clearContentLanguage,
    setSelectedEntity,
    resetForm: reset,
    initializeForm: initialize,
    apiCall3
  })
)(CancelDialog)
