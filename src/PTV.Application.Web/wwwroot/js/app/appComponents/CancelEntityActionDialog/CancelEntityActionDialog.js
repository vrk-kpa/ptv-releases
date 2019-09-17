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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Modal, ModalActions, ModalContent, ModalTitle, Button } from 'sema-ui-components'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withState from 'util/withState'
import styles from './styles.scss'
import { formTypesEnum } from 'enums'
import { buttonMessages } from 'Routes/messages'

export const dialogMessages = {
  [formTypesEnum.SERVICEFORM]: {
    archiveEntity: defineMessages({
      title: {
        id: 'AppComponents.CancelEntityAction.Service.Archive.Title',
        defaultMessage: 'The service can\'t be archived'
      },
      text: {
        id: 'AppComponents.CancelEntityAction.Service.Archive.Text',
        defaultMessage: 'The service is included into ASTI contract and can\'t be archived.'
      }
    }),
    archiveLanguage: defineMessages({
      title: {
        id: 'AppComponents.CancelEntityAction.Service.ArchiveLanguage.Title',
        defaultMessage: 'The service language can\'t be archived'
      },
      text: {
        id: 'AppComponents.CancelEntityAction.Service.ArchiveLanguage.Text',
        defaultMessage: 'The service is included into ASTI contract and can\'t be archived.'
      }
    })
  },
  [formTypesEnum.SERVICELOCATIONFORM]: {
    archiveEntity: defineMessages({
      title: {
        id: 'AppComponents.CancelEntityAction.ServiceLocationChannel.Archive.Title',
        defaultMessage: 'The service location can\'t be archived.'
      },
      text: {
        id: 'AppComponents.CancelEntityAction.ServiceLocationChannel.Archive.Text',
        defaultMessage: 'The service location is included into ASTI contract and can\'t be archived.'
      }
    }),
    archiveLanguage: defineMessages({
      title: {
        id: 'AppComponents.CancelEntityAction.ServiceLocationChannel.ArchiveLanguage.Title',
        defaultMessage: 'The service location language can\'t be archived.'
      },
      text: {
        id: 'AppComponents.CancelEntityAction.ServiceLocationChannel.ArchiveLanguage.Text',
        defaultMessage: 'The service location is included into ASTI contract and can\'t be archived.'
      }
    })
  }
}

const CancelEntityActionDialog = (
  { intl: { formatMessage },
    formName,
    updateUI,
    isOpen,
    action,
    ...rest
  }) => {
  const handleCancelCancelDialog = () => {
    return updateUI('isOpen', false)
  }
  const getText = (postfix) => {
    return dialogMessages[formName] &&
            dialogMessages[formName][action] &&
            formatMessage(dialogMessages[formName][action][postfix]) ||
            'no text defined'
  }
  return <Modal
    isOpen={isOpen}
    onRequestClose={handleCancelCancelDialog}
    contentLabel='Entity action cancel dialog'
    style={{ content: { maxWidth: '30rem', minWidth: 'auto' } }}
  >
    <ModalTitle title={getText('title')} />
    <ModalContent>
      {getText('text')}
    </ModalContent>
    <ModalActions>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleCancelCancelDialog}>{formatMessage(buttonMessages.close)}</Button>
      </div>
    </ModalActions>
  </Modal>
}

CancelEntityActionDialog.propTypes = {
  intl: intlShape.isRequired,
  updateUI: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  isOpen: PropTypes.bool.isRequired,
  action:PropTypes.string.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: ({ formName, action }) => `${formName}${action}CancelDialog`,
    initialState: {
      isOpen: false
    }
  })
)(CancelEntityActionDialog)
