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
import { ModalActions, Button } from 'sema-ui-components'
import ModalDialog from 'appComponents/ModalDialog'
import { mergeInUIState } from 'reducers/ui'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import styles from '../styles.scss'
import messages from '../messages'
import { apiCall3 } from 'actions'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { connect } from 'react-redux'
import { formEntityTypes, formActions, formTypesEnum, formActionsTypesEnum, entityCoverageTypesEnum } from 'enums'
import { injectIntl, intlShape } from 'util/react-intl'
import { EntitySchemas } from 'schemas'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'

const formSchemas = {
  [formTypesEnum.SERVICEFORM]: EntitySchemas.SERVICE_FORM,
  [formTypesEnum.SERVICECOLLECTIONFORM]: EntitySchemas.SERVICECOLLECTION,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: EntitySchemas.GENERAL_DESCRIPTION,
  [formTypesEnum.ORGANIZATIONFORM]: EntitySchemas.ORGANIZATION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PHONECHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PRINTABLEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.SERVICELOCATIONFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.WEBPAGEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noschema'
}

const EntityStatusChangeDialog = ({
  formName,
  entityId,
  languageId,
  dispatch,
  title,
  description,
  action,
  coverage,
  acceptButtonTitle,
  cancelButtonTitle,
  successNextAction,
  onErrorAction,
  intl: { formatMessage }
}) => {
  const acceptStatusChange = () => {
    dispatch(mergeInUIState({
      key: `${formName}${action}Dialog`,
      value: {
        isOpen: false
      }
    }))
    const data = {
      [entityCoverageTypesEnum.ENTITY]: { id: entityId },
      [entityCoverageTypesEnum.LANGUAGE]: { id: entityId, languageId }
    }[coverage]
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[action][formName],
          data: data
        },
        schemas: formSchemas[formName],
        successNextAction: successNextAction && typeof successNextAction === 'function' && successNextAction,
        onErrorAction: onErrorAction && typeof onErrorAction === 'function' && onErrorAction,
        formName
      })
    )
    if (coverage === entityCoverageTypesEnum.LANGUAGE) {
      dispatch({
        type: API_CALL_CLEAN,
        keys: ['entityHistory']
      })
    }
  }
  const cancelStatusChange = () => {
    unLockEntity()
    dispatch(mergeInUIState({
      key: `${formName}${action}Dialog`,
      value: {
        isOpen: false
      }
    }))
  }
  const unLockEntity = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'unLock'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.UNLOCKENTITY][formName],
          data: { id: entityId }
        },
        schemas: formSchemas[formName],
        formName
      })
    )
  }
  return (
    <ModalDialog name={`${formName}${action}Dialog`}
      title={title}
      description={description}
      onRequestClose={unLockEntity}>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={acceptStatusChange}>
            {acceptButtonTitle || formatMessage(messages.acceptButton)}
          </Button>
          <Button link onClick={cancelStatusChange}>
            {cancelButtonTitle || formatMessage(messages.cancelButton)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
  )
}

EntityStatusChangeDialog.propTypes = {
  dispatch: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  entityId: PropTypes.string.isRequired,
  languageId: PropTypes.string,
  title: PropTypes.string,
  description: PropTypes.string,
  action: PropTypes.string.isRequired,
  coverage: PropTypes.string.isRequired,
  acceptButtonTitle: PropTypes.string,
  cancelButtonTitle: PropTypes.string,
  successNextAction: PropTypes.func,
  onErrorAction: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, { formName }) => ({
    entityId: getSelectedEntityId(state),
    formName: formName
  }))
)(EntityStatusChangeDialog)
