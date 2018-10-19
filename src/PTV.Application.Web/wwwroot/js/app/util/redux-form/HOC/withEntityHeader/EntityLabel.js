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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getFormValue } from 'selectors/base'
import { localizeProps } from 'appComponents/Localize'
import {
  getSelectedEntityConcreteType,
  getSelectedEntityId,
  getEntity,
  getPreviousInfoVersion
} from 'selectors/entities/entities'
import {
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusModifiedId,
  getPublishingStatusDraftId,
  getPublishingStatusPublishedId
} from 'selectors/common'
import { entityConcreteTypesEnum,
  formActions,
  formActionsTypesEnum,
  formEntityTypes,
  formPaths,
  formTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { entityConcreteTexts } from 'util/redux-form/messages'
import { injectIntl, intlShape } from 'util/react-intl'
import messages, { dialogMessages } from './messages'
import { toLowerCaseFirstLetter } from 'util/helpers'
import { Button, ModalActions } from 'sema-ui-components'
import styles from './styles.scss'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import ModalDialog from 'appComponents/ModalDialog'
import CopyDialog from 'appComponents/CopyDialog/CopyDialog'
import { mergeInUIState } from 'reducers/ui'
import ImmutablePropTypes from 'react-immutable-proptypes'
import moment from 'moment'
import { Map } from 'immutable'
import { Security } from 'appComponents/Security'
import { withRouter } from 'react-router'

// why it cannot be in enums???? it throws weird exception
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

const EntityLabel = ({
  intl: { formatMessage },
  concreteEntityType,
  serviceType,
  entityId,
  isReadOnly,
  dispatch,
  linksVisible,
  formName,
  linkToModified,
  linkToPublished,
  isArchived,
  canBeArchived,
  history,
  canBeWithdrawed,
  disableLinks,
  handleArchiveResponse,
  isInReview,
  isCopyVisible
}) => {
  const concreteType = formatMessage(entityConcreteTexts[concreteEntityType])
  const handleArchiveClick = () => {
    handleArchiveResponse
      ? dispatch(
        apiCall3({
          keys: [formEntityTypes[formName], 'lock'],
          payload: {
            endpoint: formActions[formActionsTypesEnum.ARCHIVEENTITY][formName] + '?id=' + entityId
          },
          successNextAction: handleArchiveSuccess,
          formName
        })
      )
      : handleArchiveSuccess()
  }

  const handleArchiveSuccess = (response) => {
    //  const handleArchiveClick = () => {
    const description = handleArchiveResponse && handleArchiveResponse(response)
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelArchiveDialog`,
      value: {
        isOpen: true,
        description: description
          ? formatMessage(description)
          : formatMessage(dialogMessages[formName]['archive'].text)
      }
    }))
  }

  const acceptArrchive = () => {
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelArchiveDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.ARCHIVEENTITY][formName],
          data: { id: entityId }
        },
        schemas: formSchemas[formName],
        formName
      })
    )
  }

  // const handleWithdrawClick = () => {
  //   dispatch(
  //     apiCall3({
  //       keys: [formEntityTypes[formName], 'lock'],
  //       payload: {
  //         endpoint: formActions[formActionsTypesEnum.WITHDRAWENTITY][formName] + '?id=' + entityId
  //       },
  //       successNextAction: handleWithdrawSuccess,
  //       formName
  //     })
  //   )
  // }

  // const handleWithdrawSuccess = () => {
  const handleWithdrawClick = () => {
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelWithdrawDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const acceptWithdraw = () => {
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelWithdrawDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.WITHDRAWENTITY][formName],
          data: { id: entityId }
        },
        schemas: formSchemas[formName],
        formName
      })
    )
  }

  // const handleRestoreClick = () => {
  //   dispatch(
  //     apiCall3({
  //       keys: [formEntityTypes[formName], 'lock'],
  //       payload: {
  //         endpoint: formActions[formActionsTypesEnum.RESTOREENTITY][formName] + '?id=' + entityId
  //       },
  //       successNextAction: handleRestoreSuccess,
  //       formName
  //     })
  //   )
  // }

  // const handleRestoreSuccess = () => {
  const handleRestoreClick = () => {
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelRestoreDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const acceptRestore = () => {
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelRestoreDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'load'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.RESTOREENTITY][formName],
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
      key: `${formName}EntityLabelArchiveDialog`,
      value: {
        isOpen: false
      }
    }))
  }

  const cancelWithdraw = () => {
    unLockEntity()
    dispatch(mergeInUIState({
      key: `${formName}EntityLabelWithdrawDialog`,
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

  const handleLinkToPublish = () => {
    history.push(`${formPaths[formName]}/${linkToPublished.get('idToEntity')}`)
  }

  const handleLinkToModified = () => {
    history.push(`${formPaths[formName]}/${linkToModified.get('idToEntity')}`)
  }

  const checkOrganization = securityOrganizationCheckTypes.byOrganization
  const permisionType = entityId ? permisionTypes.update : permisionTypes.create

  return (<div className={styles.entityLabel}>
    <div>
      {concreteType}{serviceType && (' (' + serviceType + ')')}
      {!isInReview && <Fragment>
        <Security
          formName={formName}
          checkOrganization={checkOrganization}
          permisionType={permisionType}
        >
          {linksVisible && entityId && isReadOnly &&
          (!isArchived &&
            <div className={styles.actions}>
              {canBeArchived &&
                <div className={styles.archiveEntityButton}>
                  <Button link disabled={disableLinks} onClick={handleArchiveClick}>
                    {(formatMessage(messages.archiveEntityButton) + ' ' + toLowerCaseFirstLetter(concreteType))}
                  </Button>
                </div>
              }
              {!linkToModified && canBeWithdrawed &&
                <div className={styles.withdrawEntityButton}>
                  <Button link disabled={disableLinks} onClick={handleWithdrawClick}>
                    {(formatMessage(messages.withdrawButton))}
                  </Button>
                </div>
              }
            </div> ||
            <div className={styles.actions}>
              <div className={styles.restoreEntityButton}>
                <Button link disabled={disableLinks} onClick={handleRestoreClick}>
                  {(formatMessage(messages.restoreButton))}
                </Button>
              </div>
            </div>
          )}
        </Security>
        <div className={styles.actions}>
          {linkToPublished &&
            <div className={styles.linkToPublish}>
              (<Button link disabled={disableLinks} onClick={handleLinkToPublish}>
                {(formatMessage(messages.linkToPublish, { date: linkToPublished.get('dateOfEntity') }))}
              </Button>)
            </div>
          }
          {linkToModified &&
            <div className={styles.linkToModified}>
              (<Button link disabled={disableLinks} onClick={handleLinkToModified}>
                {(formatMessage(messages.linkToModified, { date: linkToModified.get('dateOfEntity') }))}
              </Button>)
            </div>
          }
          {isCopyVisible && linksVisible &&
            <CopyDialog history={history} />
          }
        </div>
        <ModalDialog name={`${formName}EntityLabelArchiveDialog`}
          title={formatMessage(dialogMessages[formName]['archive'].title)}
          onRequestClose={unLockEntity}>
          <ModalActions>
            <div className={styles.buttonGroup}>
              <Button small onClick={acceptArrchive}>
                {formatMessage(messages.acceptButton)}</Button>
              <Button link onClick={cancelArrchive}>
                {formatMessage(messages.cancelButton)}
              </Button>
            </div>
          </ModalActions>
        </ModalDialog>
        <ModalDialog name={`${formName}EntityLabelWithdrawDialog`}
          title={formatMessage(dialogMessages[formName]['withdraw'].title)}
          description={formatMessage(dialogMessages[formName]['withdraw'].text)}
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
        <ModalDialog name={`${formName}EntityLabelRestoreDialog`}
          title={formatMessage(dialogMessages[formName]['restore'].title)}
          description={formatMessage(dialogMessages[formName]['restore'].text)}
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
      </Fragment>}
    </div>
  </div>)
}

EntityLabel.propTypes = {
  concreteEntityType: PropTypes.string.isRequired,
  entityId: PropTypes.string.isRequired,
  intl: intlShape,
  serviceType: PropTypes.string,
  linkToModified: ImmutablePropTypes.map,
  linkToPublished: ImmutablePropTypes.map,
  canBeArchived: PropTypes.bool.isRequired,
  linksVisible: PropTypes.bool.isRequired,
  isReadOnly: PropTypes.bool.isRequired,
  formName: PropTypes.string.isRequired,
  dispatch: PropTypes.func.isRequired,
  isArchived: PropTypes.bool,
  canBeWithdrawed: PropTypes.bool.isRequired,
  history: PropTypes.object.isRequired,
  isInReview: PropTypes.bool,
  isCopyVisible: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  withRouter,
  connect(
    (state, { formName, handleArchiveResponse, hideCopy }) => {
      const entity = getEntity(state)
      const entityPS = entity.get('publishingStatus')
      const previousInfo = getPreviousInfoVersion(state)
      const entityId = getSelectedEntityId(state)
      const modifiedStatusId = getPublishingStatusModifiedId(state)
      const publishedStatusId = getPublishingStatusPublishedId(state)
      const linkToModified = publishedStatusId === entityPS && previousInfo.get('lastModifiedId') &&
        Map({
          idToEntity: previousInfo.get('lastModifiedId'),
          dateOfEntity: moment(previousInfo.get('modifiedOfLastModified')).format('DD.MM.YYYY')
        }) || null
      const linkToPublished = modifiedStatusId === entityPS && previousInfo.get('lastPublishedId') &&
        Map({
          idToEntity: previousInfo.get('lastPublishedId'),
          dateOfEntity: moment(previousInfo.get('modifiedOfLastPublished')).format('DD.MM.YYYY')
        }) || null
      const concreteEntityType = getSelectedEntityConcreteType(state)
      return {
        concreteEntityType,
        type: getFormValue('serviceType')(state, { formName }),
        entityId,
        canBeArchived: entityPS !== getPublishingStatusDeletedId(state),
        canBeWithdrawed: entityPS !== getPublishingStatusDraftId(state) &&
          entityPS !== modifiedStatusId,
        isArchived: entityPS === getPublishingStatusOldPublishedId(state) ||
          entityPS === getPublishingStatusDeletedId(state),
        formName,
        linkToModified,
        linkToPublished,
        handleArchiveResponse,
        // temporary condition for showing copy link
        isCopyVisible: concreteEntityType !== entityConcreteTypesEnum.ORGANIZATION &&
        concreteEntityType !== entityConcreteTypesEnum.GENERALDESCRIPTION && !hideCopy && !!entityId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  localizeProps({
    nameAttribute: 'serviceType',
    idAttribute: 'type'
  })
)(EntityLabel)
