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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectFormName } from 'util/redux-form/HOC'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getFormValue } from 'selectors/base'
import { getContentLanguageId } from 'selectors/selections'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import {
  getPublishingStatusDraftId,
  getPublishingStatusDeletedId,
  getPublishingStatusModifiedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusPublishedId } from 'selectors/common'
import { capitalizeFirstLetter } from 'util/helpers'
import {
  getSelectedEntityId,
  getEntity,
  getCanBeTranslated
} from 'selectors/entities/entities'
import { Button, ModalActions } from 'sema-ui-components'
import messages, { dialogMessages } from './messages'
import { injectIntl, intlShape } from 'react-intl'
import styles from './styles.scss'
import {
  formActions,
  formPaths,
  formActionsTypesEnum,
  formEntityTypes,
  formTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { mergeInUIState } from 'reducers/ui'
import { ModalDialog, TranslationOrderDialog } from 'appComponents'
import { Map } from 'immutable'
import { browserHistory } from 'react-router'
import { Security } from 'appComponents/Security'
import { getShowErrorsAction } from 'reducers/notifications'

// why it cannot be in enums???? it throws weird exception
const formSchemas = {
  [formTypesEnum.SERVICEFORM]: EntitySchemas.SERVICE_FORM,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: EntitySchemas.GENERAL_DESCRIPTION,
  [formTypesEnum.ORGANIZATIONFORM]: EntitySchemas.ORGANIZATION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PHONECHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PRINTABLEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.SERVICELOCATIONFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.WEBPAGEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noschema'
}

const LanguageLabel = ({
  intl: { formatMessage },
  language,
  status,
  entityId,
  languageId,
  formName,
  linksVisible,
  isReadOnly,
  dispatch,
  canBeArchived,
  modifiedExist,
  disableLinks,
  canBeWithdrawed,
  canBeRestored,
  isArchived,
  linkToTranslations
}) => {
  const handleArchiveLanguageClick = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'lock'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.ARCHIVELANGUAGE][formName] + '?id=' + entityId
        },
        successNextAction: handleSuccessArchiveLanguageClick,
        formName
      })
    )
  }

  const handleSuccessArchiveLanguageClick = () => {
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelArchiveDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const acceptArrchive = () => {
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelArchiveDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.ARCHIVELANGUAGE][formName],
          data: { id: entityId,
            languageId }
        },
        schemas: formSchemas[formName],
        successNextAction: successAction,
        onErrorAction: errorAction,
        formName
      })
    )
  }

  const handleWithdrawLanguageClick = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'lock'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.WITHDRAWLANGUAGE][formName] + '?id=' + entityId
        },
        successNextAction: handleSuccessWithdrawLanguageClick,
        formName
      })
    )
  }

  const handleSuccessWithdrawLanguageClick = () => {
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelWithdrawDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const acceptWithdraw = () => {
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelWithdrawDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.WITHDRAWLANGUAGE][formName],
          data: { id: entityId,
            languageId }
        },
        schemas: formSchemas[formName],
        successNextAction: successAction,
        formName
      })
    )
  }

  const handleRestoreClick = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'lock'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.RESTORELANGUAGE][formName] + '?id=' + entityId
        },
        successNextAction: handleSuccessRestoreClick,
        formName
      })
    )
  }

  const handleSuccessRestoreClick = () => {
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelRestoreDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const acceptRestore = () => {
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelRestoreDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.RESTORELANGUAGE][formName],
          data: { id: entityId,
            languageId }
        },
        schemas: formSchemas[formName],
        successNextAction: successAction,
        formName
      })
    )
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

  const cancelArrchive = () => {
    unLockEntity()
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelArchiveDialog`,
      value: {
        isOpen: false
      }
    }))
  }

  const cancelWithdraw = () => {
    unLockEntity()
    dispatch(mergeInUIState({
      key: `${formName}LanguageLabelWithdrawDialog`,
      value: {
        isOpen: false
      }
    }))
  }

  const cancelRestore = () => {
    unLockEntity()
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelRestoreDialog`,
      value: {
        isOpen: false
      }
    }))
  }

  const successAction = ({ response: { result: { result } } }) => {
    browserHistory.replace(`${formPaths[formName]}/${result}`)
  }

  const errorAction = (data) => () => {
    if (data && data[0] && data[0].code === 'Common.Exception.ArchiveLanguage') {
      dispatch(mergeInUIState({
        key: `${formName}EntityLabelArchiveDialog`,
        value: {
          isOpen: true
        }
      }))
    } else {
      dispatch(getShowErrorsAction(formName, true)(data))
    }
  }

  const checkOrganization = securityOrganizationCheckTypes.byOrganization
  const permisionType = entityId ? permisionTypes.update : permisionTypes.create

  return (<div className={styles.languageLabel}>
    { /* TODO:HONZA */}
    <div className={styles.archiveButtons}>
      {capitalizeFirstLetter(language) + ' / ' + status}
    </div>
    <Security
      formName={formName}
      checkOrganization={checkOrganization}
      permisionType={permisionType}
        >
      {!modifiedExist && linksVisible && entityId && isReadOnly && !isArchived &&
        <div className={styles.actions}>
          {canBeArchived && <div>
            <Button link disabled={disableLinks} onClick={handleArchiveLanguageClick}>
              {formatMessage(messages.archiveLanguageButton)}
            </Button>
          </div>}
          {canBeRestored && <div>
            <Button link disabled={disableLinks} onClick={handleRestoreClick}>
              {(formatMessage(messages.restoreButton))}
            </Button>
          </div>}
          {!canBeRestored && canBeWithdrawed && <div>
            <Button link disabled={disableLinks} onClick={handleWithdrawLanguageClick}>
              {formatMessage(messages.withdrawButton)}
            </Button>
          </div>}
          { linkToTranslations && <TranslationOrderDialog />}
        </div>
      }
    </Security>
    <ModalDialog name={`${formName}LanguageLabelArchiveDialog`}
      title={formatMessage(messages.archiveLanguageDialogTitle)}
      description={formatMessage(messages.archiveLanguageDialogText, { language: capitalizeFirstLetter(language) })}
      onRequestClose={unLockEntity}>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={acceptArrchive}>
            {formatMessage(messages.acceptButton)}
          </Button>
          <Button link onClick={cancelArrchive}>
            {formatMessage(messages.cancelButton)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
    <ModalDialog name={`${formName}LanguageLabelWithdrawDialog`}
      title={formatMessage(messages.withdrawLanguageDialogTitle)}
      description={formatMessage(messages.withdrawLanguageDialogText, { language: capitalizeFirstLetter(language) })}
      onRequestClose={unLockEntity}>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={acceptWithdraw}>
            {formatMessage(messages.acceptButton)}
          </Button>
          <Button link onClick={cancelWithdraw}>
            {formatMessage(messages.cancelButton)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
    <ModalDialog name={`${formName}LanguageLabelRestoreDialog`}
      title={formatMessage(messages.restoreLanguageDialogTitle)}
      description={formatMessage(messages.restoreLanguageDialogText, { language: capitalizeFirstLetter(language) })}
      onRequestClose={unLockEntity}>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={acceptRestore}>
            {formatMessage(messages.acceptButton)}
          </Button>
          <Button link onClick={cancelRestore}>
            {formatMessage(messages.cancelButton)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
  </div>)
}

LanguageLabel.propTypes = {
  language: PropTypes.string.isRequired,
  formName: PropTypes.string.isRequired,
  languageId: PropTypes.string.isRequired,
  canBeArchived: PropTypes.bool.isRequired,
  modifiedExist: PropTypes.bool.isRequired,
  linksVisible: PropTypes.bool.isRequired,
  canBeWithdrawed: PropTypes.bool.isRequired,
  isReadOnly: PropTypes.bool.isRequired,
  isArchived: PropTypes.bool.isRequired,
  entityId: PropTypes.string.isRequired,
  dispatch: PropTypes.func.isRequired,
  status: PropTypes.string.isRequired,
  canBeRestored: PropTypes.bool.isRequired,
  intl: intlShape,
  linkToTranslations: PropTypes.bool
}

export default compose(
  injectFormName,
  withFormStates,
  injectIntl,
  connect(
    (state, { formName }) => {
      const entity = getEntity(state)
      const entityPS = entity.get('publishingStatus')
      const formLanguages = getFormValue('languagesAvailabilities')(state, { formName })
      const contentLanguageId = getContentLanguageId(state, { formName })
      const selectedLanguage = formLanguages.filter(x => x.get('languageId') === contentLanguageId).first() ||
        formLanguages.first() || Map()
      const isArchived = entityPS === getPublishingStatusOldPublishedId(state) ||
        entityPS === getPublishingStatusDeletedId(state)
      const statusId = isArchived && getPublishingStatusDeletedId(state) ||
        (selectedLanguage.get('statusId') || getPublishingStatusDraftId(state))
      const publishedStatusId = getPublishingStatusPublishedId(state)
      const previousInfo = entity.get('previousInfo') || Map()
      const modifiedExist = !!(publishedStatusId === entityPS && previousInfo.get('lastModifiedId'))
      const languageCode = selectedLanguage.get('code')
      const canBeTranslated = getCanBeTranslated(state, { languageCode })
      return {
        languageId: selectedLanguage.get('languageId'),
        statusId,
        formName,
        entityId: getSelectedEntityId(state),
        canBeArchived: statusId !== getPublishingStatusDeletedId(state),
        canBeRestored: statusId === getPublishingStatusDeletedId(state),
        canBeWithdrawed: statusId !== getPublishingStatusDraftId(state) &&
        statusId !== getPublishingStatusModifiedId(state),
        isArchived,
        modifiedExist,
        linkToTranslations: canBeTranslated
      }
    }
  ),
  localizeProps({
    nameAttribute: 'language',
    idAttribute: 'languageId',
    languageTranslationType: languageTranslationTypes.locale
  }),
  localizeProps({
    nameAttribute: 'status',
    idAttribute: 'statusId',
    languageTranslationType: languageTranslationTypes.locale
  })
)(LanguageLabel)
