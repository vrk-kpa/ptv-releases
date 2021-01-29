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
import { ModalActions, Button, ModalContent } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { getAvailableLanguages } from './selectors'
import { getFormValues, change } from 'redux-form/immutable'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import withState from 'util/withState'
import Spacer from 'appComponents/Spacer'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { formTypesEnum } from 'enums'
import {
  Organization
} from 'util/redux-form/fields'
import OrganizationCell from '../../../../appComponents/Cells/OrganizationCell/OrganizationCell'
import styles from './styles.scss'

const messages = defineMessages({
  dialogTitle: {
    id: 'AdminMapping.UpdateMappingDialog.Title',
    defaultMessage: 'Muokkaa PTV-Tunniste'
  },
  dialogDescription: {
    id: 'AdminMapping.UpdateMappingDialog.Description',
    defaultMessage: 'Käytä harkintaa muokatessasi organisaation tunnistetta.'
  },
  actionConfirm: {
    id: 'Components.Buttons.SaveButton',
    defaultMessage: 'Tallenna'
  },
  actionCancel: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  },
  organizationTitle: {
    id: 'AdminMapping.UpdateMappingDialog.Organization.Title',
    defaultMessage: 'PTV-tunniste'
  }
})

const closeDialog = (mergeInUIState, dialogKey, dispatch) => {
  mergeInUIState({
    key: dialogKey,
    value: {
      isOpen: false
    }
  })
  mergeInUIState({
    key: 'updateMappingDialogParams',
    value: {
      ptvId:null,
      sahaId:null,
      selectedOrganizations:null
    }
  })
  dispatch(change(formTypesEnum.ADMINMAPPINGFORM, 'organizationId', null))
}

const UpdateMappingDialog = ({
  mergeInUIState,
  confirmAction,
  ptvId,
  sahaId,
  selectedOrganizations,
  languagesAvailabilities,
  organizationId,
  change,
  dispatch,
  intl: { formatMessage }
}) => {
  const dialogKey = 'updateMappingDialog'
  const handleConfirmAction = () => {
    confirmAction(ptvId, sahaId, selectedOrganizations, organizationId)
    closeDialog(mergeInUIState, dialogKey, dispatch)
  }
  const handleCancelAction = () => {
    closeDialog(mergeInUIState, dialogKey, dispatch)
  }
  return (
    <ModalDialog
      name={dialogKey}
      title={formatMessage(messages.dialogTitle)}
      contentLabel='updateMappingDialog'
      style={{ content: { maxWidth: '80rem', minWidth: 'auto' } }}
      className={styles.updateMappingDialog}
    >
      <ModalContent>
        <div className={styles.modalDescription}>{formatMessage(messages.dialogDescription)}</div>
        <div className='d-flex align-items-center'>
          <LanguageBarCell languagesAvailabilities={languagesAvailabilities} className={styles.languages} />
          <OrganizationCell organizationId={ptvId} />
        </div>
        <Spacer />
        <Organization
          name='organizationId'
          label={formatMessage(messages.organizationTitle)}
          tooltip={null}
          showAll
          skipValidation
        />
      </ModalContent>
      <ModalActions>
        <Button
          small
          onClick={handleConfirmAction}
          children={formatMessage(messages.actionConfirm)}
          disabled={!organizationId}
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

UpdateMappingDialog.propTypes = {
  mergeInUIState: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const formState = getFormValues(formTypesEnum.ADMINMAPPINGFORM)(state) || Map()
    return {
      organizationId: formState.get('organizationId') || null,
      languagesAvailabilities: getAvailableLanguages(state, { uiKey: 'updateMappingDialogParams' })
    }
  }, {
    mergeInUIState,
    change
  }),
  withState({
    redux: true,
    key: 'updateMappingDialogParams',
    initialState: {
      ptvId: null,
      sahaId: null,
      selectedOrganizations: null
    }
  }),
  withFormStates
)(UpdateMappingDialog)
