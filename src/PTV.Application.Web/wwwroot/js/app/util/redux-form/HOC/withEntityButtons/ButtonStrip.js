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
import { submit, change, isDirty, isSubmitting } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Button, Spinner } from 'sema-ui-components'
import styles from './styles.scss'
import cx from 'classnames'
import { setReadOnly } from 'reducers/formStates'
import { mergeInUIState } from 'reducers/ui'
import {
  formActions,
  formActionsTypesEnum,
  formEntityTypes,
  formTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { apiCall3 } from 'actions'
import { defineMessages, intlShape } from 'util/react-intl'
import { EntitySchemas } from 'schemas'
import { OwnOrgSecurityPublish, Security } from 'appComponents/Security'
import { property } from 'lodash'
import EntitySelectors, {
  getSelectedEntityId,
  getEntity,
  getPreviousInfoVersion
} from 'selectors/entities/entities'
import { getPublishingStatusPublishedId } from 'selectors/common'
import { getIsEntityArchived, isEntityOnlyEdit, getUrlCheckingAbort } from './selectors'
import { getIsFormDisabled } from 'selectors/formStates'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'

const messages = defineMessages({
  saveButton: {
    id: 'Entity.SaveButton',
    defaultMessage: 'Tallenna luonnoksena',
    description: {
      en: 'Save as draft',
      sv: 'Spara som utkast'
    }
  },
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  },
  cancelUpdateButton: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  },
  publishButton: {
    id: 'Entity.PublishButton',
    defaultMessage: 'Tallenna ja julkaise',
    description: {
      en: 'Save and publish',
      sv: 'Spara och publicera'
    }
  }
})

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

const ButtonStrip = ({
  inside,
  isReadOnly,
  formName,
  onlyEdit,
  entityId,
  dispatch,
  isDirty,
  isArchived,
  modifiedExist,
  isDialogLoading,
  isEntityLoading,
  isFormDisabled,
  updateUI,
  buttonClicked,
  intl: { formatMessage },
  inTranslation,
  location,
  onEdit,
  isInReview,
  abort
}) => {
  const buttonGroupClass = cx(
    styles.buttonGroup,
    { [styles.inside]: inside }
  )

  const lockSuccess = () => {
    dispatch(
      setReadOnly({
        form: formName,
        value: false
      })
    )
    onEdit && typeof onEdit === 'function' && dispatch(onEdit({ formName }))
  }

  const handleOnEdit = () => {
    !isArchived && dispatch(change(formName, 'publish', false))
    updateUI('buttonClicked', 'Edit')
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'loadEntity'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.LOCKENTITY][formName],
          data: { id: entityId }
        },
        formName,
        successNextAction: lockSuccess
      })
    )
  }

  const handleOnCancel = () => {
    updateUI('buttonClicked', 'Cancel')
    dispatch(mergeInUIState({
      key: `${formName}CancelDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const handleOnSave = () => {
    abort && abort()
    updateUI('buttonClicked', 'Save')
    const withValidation = property('state.includeValidation')(location)
    dispatch(
      change(
        formName,
        'action',
        withValidation ? 'saveAndValidate' : 'save'
      )
    )
    dispatch(submit(formName))
    dispatch(mergeInUIState({
      key: 'languageVersionsVisible',
      value: { areLanguageVersionsVisible:false }
    }))
  }

  const validateSuccess = () => {
    dispatch(mergeInUIState({
      key: `${formName}PublishingDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const handleOnPublish = () => {
    abort && abort()
    updateUI('buttonClicked', 'Publish')
    if (!isReadOnly) {
      dispatch(
        change(
          formName,
          'action',
          'saveAndPublish'
        )
      )
      dispatch(submit(formName))
    } else {
      dispatch(
        apiCall3({
          keys: [formEntityTypes[formName], 'loadDialog'],
          payload: {
            endpoint: formActions[formActionsTypesEnum.GETVALIDATEDENTITY][formName],
            data: { id: entityId }
          },
          schemas: formSchemas[formName],
          successNextAction: validateSuccess,
          formName
        })
      )
    }
    dispatch(mergeInUIState({
      key: 'languageVersionsVisible',
      value: { areLanguageVersionsVisible:false }
    }))
  }
  const checkOrganization = securityOrganizationCheckTypes.byOrganization
  const permisionType = entityId ? permisionTypes.update : permisionTypes.create
  return (
    <div>
      {!modifiedExist && ((isReadOnly && !entityId) || (isReadOnly && onlyEdit)
        ? <Security
          formName={formName}
          checkOrganization={checkOrganization}
          permisionType={permisionType}
        >
          <div className={buttonGroupClass}>
            <Button
              children={formatMessage(messages.cancelUpdateButton)}
              medium
              secondary
              disabled
            />
            <Button
              children={isEntityLoading && <Spinner /> || formatMessage(messages.updateButton)}
              onClick={handleOnEdit}
              medium
              secondary
              disabled={isFormDisabled || isEntityLoading || inTranslation}
            />
            {!isInReview &&
            <Button
              children={formatMessage(messages.publishButton)}
              medium
              secondary
              disabled
            /> || null
            }
          </div>
        </Security>
        : <div className={buttonGroupClass}>
          {!isArchived && <Security
            formName={formName}
            checkOrganization={checkOrganization}
            permisionType={permisionType}
          >
            <Button
              children={isEntityLoading && buttonClicked === 'Cancel' && <Spinner /> ||
                formatMessage(messages.cancelUpdateButton)}
              onClick={handleOnCancel}
              medium
              secondary
              disabled={isFormDisabled || isEntityLoading || isDialogLoading || inTranslation || isReadOnly}
            />
          </Security>}
          <Security
            formName={formName}
            checkOrganization={checkOrganization}
            permisionType={permisionType}
          >

            {((!entityId || onlyEdit) || (!isReadOnly && !!entityId)) &&
              <Button
                children={isDialogLoading && buttonClicked === 'Save' && <Spinner /> ||
                  formatMessage(messages.saveButton)}
                onClick={handleOnSave}
                medium
                secondary={isArchived || !!entityId}
                disabled={isFormDisabled || isDialogLoading || isEntityLoading || inTranslation}
              /> ||
              <Button
                children={isEntityLoading && buttonClicked === 'Edit' && <Spinner /> ||
                  formatMessage(messages.updateButton)}
                onClick={handleOnEdit}
                medium
                secondary
                disabled={isFormDisabled || isEntityLoading || isDialogLoading || inTranslation}
              />
            }
          </Security>
          {!isInReview && <OwnOrgSecurityPublish
            formName={formName}
            checkOrganization={checkOrganization}
            permisionType={isReadOnly
              ? permisionTypes.publish
              : permisionTypes.publish | permisionType
            }
          >
            <Button
              children={isDialogLoading && buttonClicked === 'Publish' && <Spinner /> ||
                formatMessage(messages.publishButton)}
              onClick={handleOnPublish}
              medium
              secondary={isArchived || !entityId || onlyEdit}
              disabled={isFormDisabled || isDialogLoading || isEntityLoading || inTranslation}
            />
          </OwnOrgSecurityPublish>
          }
        </div>
      )}
    </div>
  )
}

ButtonStrip.propTypes = {
  inside: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  onlyEdit: PropTypes.bool,
  isDirty: PropTypes.bool.isRequired,
  buttonClicked: PropTypes.string.isRequired,
  isDialogLoading: PropTypes.bool.isRequired,
  isArchived: PropTypes.bool.isRequired,
  isFormDisabled: PropTypes.bool.isRequired,
  modifiedExist: PropTypes.bool.isRequired,
  isEntityLoading: PropTypes.bool.isRequired,
  formName: PropTypes.string,
  intl: intlShape,
  dispatch: PropTypes.func.isRequired,
  updateUI: PropTypes.func.isRequired,
  entityId: PropTypes.string,
  onEdit: PropTypes.func,
  location: PropTypes.object,
  inTranslation: PropTypes.bool,
  isInReview: PropTypes.bool,
  abort: PropTypes.func
}

export default compose(
  connect((state, { formName }) => {
    const entity = getEntity(state)
    const entityPS = entity.get('publishingStatus')
    const previousInfo = getPreviousInfoVersion(state)
    const publishedStatusId = getPublishingStatusPublishedId(state)
    const modifiedExist = !!(publishedStatusId === entityPS && previousInfo.get('lastModifiedId'))
    return {
      isInReview: getShowReviewBar(state),
      isDirty: isDirty(formName)(state),
      isFormDisabled: getIsFormDisabled(formName)(state),
      isArchived: getIsEntityArchived(state),
      entityId: getSelectedEntityId(state),
      onlyEdit: isEntityOnlyEdit(state),
      isDialogLoading: isSubmitting(formName)(state) ||
        EntitySelectors[formEntityTypes[formName]].getEntityDialogIsFetching(state),
      isEntityLoading: isSubmitting(formName)(state) ||
        EntitySelectors[formEntityTypes[formName]].getEntityIsLoading(state),
      modifiedExist,
      abort: getUrlCheckingAbort(state)
    }
  })
)(ButtonStrip)
