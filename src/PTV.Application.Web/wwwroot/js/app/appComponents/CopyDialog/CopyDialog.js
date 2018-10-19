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
import {
  Modal,
  ModalTitle,
  ModalActions,
  ModalContent,
  Button
} from 'sema-ui-components'
import { formPaths, formTypesEnum, entityTypesEnum } from 'enums'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withState from 'util/withState'
import { getIsCopyEnabled, getFirstCopiedLanguage } from './selectors'
import { getSelectedEntityId, getSelectedEntityType } from 'selectors/entities/entities'
import { setContentLanguage } from 'reducers/selections'
import { setReadOnly } from 'reducers/formStates'
import { reset } from 'redux-form/immutable'
import LanguagesCopyForm from './LanguagesCopyForm'
import styles from './styles.scss'

const messages = defineMessages({
  cancelButton: {
    id: 'Components.Buttons.Cancel',
    defaultMessage: 'Peruuta'
  },
  copyButton: {
    id: 'Components.Buttons.Copy',
    defaultMessage: 'Kopioi'
  },
  copyLink: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.CopyLink.Title',
    defaultMessage: 'Kopioi pohjaksi'
  },
  copyDialogTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.CopyDialog.Title',
    defaultMessage: 'Valitse kopioitavat kieliversiot'
  },
  copyServiceDialogDescription: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.CopyDialog.Description',
    defaultMessage: 'Valitse kieliversiot, jotka haluat kopioida uuden palvelun pohjaksi.'
  },
  copyChannelDialogDescription: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ChannelCopyDialog.Description',
    defaultMessage: 'Choose the language versions you want to copy to the new channel.'
  },
  copyGDDialogDescription: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.GDCopyDialog.Description',
    defaultMessage: 'Choose the language versions you want to copy to the new general description'
  },
  copyServiceCollectionDialogDescription: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityLabel.ServiceCollectionCopyDialog.Description',
    defaultMessage: 'Choose the language versions you want to copy to the new service collection'
  }
})

const CopyDialog = ({
  intl: { formatMessage },
  formName,
  isOpen,
  updateUI,
  setContentLanguage,
  setReadOnly,
  resetForm,
  entityId,
  entityType,
  history,
  languageId,
  languageCode,
  isCopyEnabled
}) => {
  const handleCopyLinkOnClick = () => {
    resetForm(formTypesEnum.COPYENTITYFORM)
    updateUI('isOpen', true)
  }

  const acceptCopy = () => {
    updateUI('isOpen', false)
    history.push(`${formPaths[formName]}?copy=${entityId}`)
    setContentLanguage({ id: languageId, code: languageCode })
  }

  const cancelCopy = () => {
    updateUI('isOpen', false)
  }

  const handleOnClose = () => {
    updateUI('isOpen', !isOpen)
  }

  const copyDialogDescription = {
    [entityTypesEnum.CHANNELS]: messages.copyChannelDialogDescription,
    [entityTypesEnum.SERVICES]: messages.copyServiceDialogDescription,
    [entityTypesEnum.GENERALDESCRIPTIONS]: messages.copyGDDialogDescription,
    [entityTypesEnum.SERVICECOLLECTIONS]: messages.copyGDDialogDescription
  }[entityType]

  return (
    <div>
      <Button link onClick={handleCopyLinkOnClick}>{(formatMessage(messages.copyLink))}</Button>
      <Modal isOpen={isOpen} onRequestClose={handleOnClose} contentLabel='Copy_Dialog' >
        <ModalTitle title={formatMessage(messages.copyDialogTitle)}>
          <div>{formatMessage(copyDialogDescription)}</div>
        </ModalTitle>
        <ModalContent>
          <LanguagesCopyForm />
        </ModalContent>
        <ModalActions>
          <div className={styles.buttonGroup}>
            <Button small onClick={acceptCopy} disabled={!isCopyEnabled}>
              {formatMessage(messages.copyButton)}
            </Button>
            <Button link onClick={cancelCopy}>
              {formatMessage(messages.cancelButton)}
            </Button>
          </div>
        </ModalActions>
      </Modal>
    </div>
  )
}

CopyDialog.propTypes = {
  intl: intlShape.isRequired,
  history: PropTypes.object.isRequired,
  formName: PropTypes.string.isRequired,
  entityId: PropTypes.string.isRequired,
  entityType: PropTypes.string.isRequired,
  updateUI: PropTypes.func,
  setContentLanguage: PropTypes.func,
  setReadOnly: PropTypes.func,
  resetForm: PropTypes.func,
  isOpen: PropTypes.bool,
  languageId: PropTypes.string,
  languageCode: PropTypes.string,
  isCopyEnabled: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: ({ formName }) => `${formName}CopyDialog`,
    initialState: {
      isOpen: false
    }
  }),
  connect((state, { formName }) => {
    const firstCopiedLanguage = getFirstCopiedLanguage(state, { formName })
    return {
      entityId: getSelectedEntityId(state),
      languageId: firstCopiedLanguage.get('id') || '',
      languageCode: firstCopiedLanguage.get('code') || '',
      isCopyEnabled: getIsCopyEnabled(state, { formName }),
      entityType: getSelectedEntityType(state)
    }
  }, {
    setContentLanguage,
    setReadOnly,
    resetForm: reset
  })
)(CopyDialog)
