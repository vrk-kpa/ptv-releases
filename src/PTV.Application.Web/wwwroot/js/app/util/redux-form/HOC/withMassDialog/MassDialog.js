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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { submit, formValues } from 'redux-form/immutable'
import { massToolTypes } from 'enums'
import MassPublish from 'appComponents/MassPublish'
import MassArchive from 'appComponents/MassArchive'
import ModalDialog from 'appComponents/ModalDialog'
import {
  ModalActions,
  ModalContent,
  Button,
  Spinner
} from 'sema-ui-components'
import MassToolDataTable from 'Routes/Search/components/MassTool/MassToolDataTable'
import { getIsMassRequestSent } from 'selectors/signalR'
import {
  getLastModifiedVersionInfoIsFetching,
  getIsConfirmButtonDisabled
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { mergeInUIState } from 'reducers/ui'
import cx from 'classnames'
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
  },
  restoreConfirmButton: {
    id: 'MassDialog.RestoreConfirm.Title',
    defaultMessage: 'Palauta arkistosta',
    description: 'Components.Buttons.RestoreButton'
  },
  restoreDialogTitle: {
    id: 'MassDialog.Restore.Title',
    defaultMessage: 'Arkistosta palauttaminen massana'
  },
  restoreDialogDescription: {
    id: 'MassDialog.Restore.Description',
    defaultMessage: 'Palauta arkistosta sisältöjä heti tai ajastetusti. Halutessasi voit poistaa sisältöjä arkistosta palautettavien listalta.' // eslint-disable-line max-len
  }
})

const MassDialog = ({
  intl: { formatMessage },
  mergeInUIState,
  submit,
  massToolType,
  formName,
  isMassRequestSent,
  isDataLoading,
  isConfirmButtonDisabled,
  ...rest
}) => {
  const handleConfirm = () => {
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
  const dialogClass = cx(
    styles.massDialog,
    styles[massToolType]
  )
  const summaryClass = massToolType === massToolTypes.ARCHIVE ? 'col-lg-15 mb-4' : 'col-24 mb-4'
  return <ModalDialog name={'MassToolDialog'}
    title={formatMessage(messages[`${massToolType}DialogTitle`])}
    className={dialogClass}>
    <ModalContent>
      <div className='row'>
        <div className={summaryClass}>
          <p className={styles.content}>{formatMessage(messages[`${massToolType}DialogDescription`])}</p>
        </div>
      </div>
      <div className='row'>
        <div className={summaryClass}>
          {massToolType === massToolTypes.PUBLISH &&
            <MassPublish closeDialog={handleCancel} {...rest} />
          }
          {massToolType !== massToolTypes.PUBLISH
            ? isDataLoading && <Spinner /> ||
            <MassToolDataTable inSummary />
            : null
          }
        </div>
        {massToolType === massToolTypes.ARCHIVE && (
          <div className='col-lg-9 pl-lg-4'>
            <MassArchive closeDialog={handleCancel} />
          </div>
        )}
      </div>
    </ModalContent>
    <ModalActions>
      <div className={styles.buttonGroup}>
        <Button small onClick={handleConfirm} disabled={isConfirmButtonDisabled || isDataLoading}>
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
  setHubRequestSent: PropTypes.func,
  formName: PropTypes.string.isRequired,
  massToolType: PropTypes.string,
  submit: PropTypes.func.isRequired,
  isConfirmButtonDisabled: PropTypes.bool,
  isMassRequestSent: PropTypes.bool,
  isDataLoading: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  formValues({ massToolType: 'type', timingType: 'timingType', publishAt: 'publishAt', archiveAt: 'archiveAt' }),
  connect((state, ownProps) => ({
    isConfirmButtonDisabled: getIsConfirmButtonDisabled(state, ownProps),
    isMassRequestSent: getIsMassRequestSent(state, { hubName: 'massTool' }),
    isDataLoading: getLastModifiedVersionInfoIsFetching(state)
  }), {
    submit,
    mergeInUIState
  })
)(MassDialog)
