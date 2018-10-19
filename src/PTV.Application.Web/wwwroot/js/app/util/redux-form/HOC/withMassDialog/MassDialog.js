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
import { submit, formValues } from 'redux-form/immutable'
import { massToolTypes, timingTypes } from 'enums'
import MassPublish from 'appComponents/MassPublish'
import MassArchive from 'appComponents/MassArchive'
import ModalDialog from 'appComponents/ModalDialog'
import {
  ModalActions,
  ModalContent,
  Button,
  Spinner
} from 'sema-ui-components'
import { setMassRequestSent } from 'reducers/signalR'
import { getIsMassRequestSent } from 'selectors/signalR'
import { getIsAnyApproved } from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { mergeInUIState } from 'reducers/ui'
import styles from './styles.scss'

const messages = defineMessages({
  cancelButton: {
    id: 'MassDialog.CopyCancel.Title',
    defaultMessage: 'Keskeytä',
    description: 'Components.Buttons.CancelUpdateButton'
  },
  copyConfirmButton: {
    id: 'MassDialog.CopyConfirm.Title',
    defaultMessage: 'Kopioi',
    description: 'Components.Buttons.CopyButton'
  },
  copyDialogTitle: {
    id: 'MassDialog.Copy.Title',
    defaultMessage: 'Haluatko kopioida valitut sisällöt?'
  },
  copyDialogDescription: {
    id: 'MassDialog.Copy.Description',
    defaultMessage: 'Valitut sisällöt ja niiden kaikki kieliversiot kopiodaan toiselle organisaatiolle.'
  },
  archiveConfirmButton: {
    id: 'MassDialog.ArchiveConfirm.Title',
    defaultMessage: 'Arkistoi',
    description: 'Components.Buttons.ArchiveButton'
  },
  archiveDialogTitle: {
    id: 'MassDialog.Archive.Title',
    defaultMessage: 'Haluatko arkistoida valitut sisällöt?'
  },
  archiveDialogDescription: {
    id: 'MassDialog.Archive.Description',
    defaultMessage: 'Valitut sisällöt ja niiden kaikki kieliversiot arkistoituvat.'
  },
  publishConfirmButton: {
    id: 'Components.Buttons.PublishButton',
    defaultMessage: 'Julkaise'
  },
  publishDialogTitle: {
    id: 'AppComponents.MassPublishDialog.Title',
    defaultMessage: 'Massajulkaisu'
  },
  publishDialogDescription: {
    id: 'AppComponents.MassPublishDialog.Description',
    defaultMessage: 'Massajulkaise sisältöjä heti tai ajastetusti. Halutessasi voit poistaa sisältöjä julkaistavien listalta, ja tallentaa sisältölistan Tehtävät -sivulle itsellesi ja muille organisaatiosi käyttäjille.' // eslint-disable-line
  }
})

const MassDialog = ({
  intl: { formatMessage },
  isOpen,
  mergeInUIState,
  setMassRequestSent,
  submit,
  massToolType,
  formName,
  isAnyApproved,
  isMassRequestSent,
  timingType,
  publishAt,
  archiveAt,
  ...rest
}) => {
  const handleConfirm = () => {
    setMassRequestSent()
    submit(formName)
  }

  const handleCancel = () => {
    mergeInUIState({
      key: 'MassToolDialog',
      value: {
        isOpen: false
      }
    })
  }

  if (!massToolType) {
    return null
  }
  const confirmActionDisabled = massToolType === massToolTypes.PUBLISH &&
    (!isAnyApproved || (timingType === timingTypes.TIMED && !publishAt)) ||
    massToolType === massToolTypes.ARCHIVE && timingType === timingTypes.TIMED && !archiveAt
  return <ModalDialog name={'MassToolDialog'}
    title={formatMessage(messages[`${massToolType}DialogTitle`])}
    className={styles[massToolType]}>
    <ModalContent>
      <p className={styles.content}>{formatMessage(messages[`${massToolType}DialogDescription`])}</p>
      {massToolType === massToolTypes.PUBLISH &&
        <MassPublish closeDialog={handleCancel} {...rest} />
      }
      {massToolType === massToolTypes.ARCHIVE &&
        <MassArchive closeDialog={handleCancel} />
      }
    </ModalContent>
    <ModalActions>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleConfirm} disabled={confirmActionDisabled}>
          {isMassRequestSent && <Spinner inverse /> || formatMessage(messages[`${massToolType}ConfirmButton`])}
        </Button>
        <Button link onClick={handleCancel}>
          {formatMessage(messages.cancelButton)}
        </Button>
      </div>
    </ModalActions>
  </ModalDialog>
}

MassDialog.propTypes = {
  intl: intlShape.isRequired,
  mergeInUIState: PropTypes.func,
  setMassRequestSent: PropTypes.func,
  isOpen: PropTypes.bool,
  formName: PropTypes.string.isRequired,
  massToolType: PropTypes.string,
  submit: PropTypes.func.isRequired,
  isAnyApproved: PropTypes.bool,
  isMassRequestSent: PropTypes.bool,
  timingType: PropTypes.string,
  publishAt: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  archiveAt: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ])
}

export default compose(
  injectIntl,
  injectFormName,
  formValues({ massToolType: 'type', timingType: 'timingType', publishAt: 'publishAt', archiveAt: 'archiveAt' }),
  connect((state, ownProps) => ({
    isAnyApproved: getIsAnyApproved(state, ownProps),
    isMassRequestSent: getIsMassRequestSent(state),
    timingType: ownProps.timingType,
    publishAt: ownProps.publishAt,
    archiveAt: ownProps.archiveAt
  }), {
    submit,
    mergeInUIState,
    setMassRequestSent
  })
)(MassDialog)
