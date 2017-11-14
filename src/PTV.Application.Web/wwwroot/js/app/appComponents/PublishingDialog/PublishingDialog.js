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
import {
  Modal,
  ModalTitle,
  ModalActions,
  ModalContent,
  Button,
  Spinner
} from 'sema-ui-components'
import { compose } from 'redux'
import CellHeaders from 'appComponents/CellHeaders'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { LanguagesTable } from 'util/redux-form/fields'
import PublishingCheckbox from './PublishingCheckbox'
import withState from 'util/withState'
import { connect } from 'react-redux'
import {
  getEntityCanBeAnyPublished,
  getSelectedEntityId
} from 'selectors/entities/entities'
import { getFormIsAnyLanguageToPublish, getFormLanguagesAvailabilitiesTransformed } from 'selectors/form'
import { formActions, formActionsTypesEnum, formEntityTypes, formTypesEnum } from 'enums'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { EntitySelectors } from 'selectors'
import { withFormStates } from 'util/redux-form/HOC'
import { setReadOnly } from 'reducers/formStates'
import Scroll from 'react-scroll'
const scroll = Scroll.scroller

const messages = defineMessages({
  dialogTitle: {
    id: 'Components.PublishingEntityDialog.Title',
    defaultMessage: 'Kieliversioiden näkyvyys'
  },
  dialogDescription: {
    id: 'Components.PublishingEntityDialog.Description',
    defaultMessage: 'Valitse, mitkä kieliversiot haluat julkaista. ' +
      'Jos valitset, että kieliversio näkyy vain PTV:ssä, ja klikkaat Julkaise-nappia, ' +
      'tällöin myös palvelun nykyinen kieliversio piilotetaan loppukäyttäjiltä.'
  },
  closeDialogButtonTitle: {
    id: 'Components.ModalDialog.Buttons.Close.Title',
    defaultMessage: 'Peruuta'
  },
  submitTitle: {
    id: 'Components.PublishingEntityForm.Button.Submit.Title',
    defaultMessage: 'Julkaise'
  }
})

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

const HeaderFormatter = compose(
  injectIntl
)(({
  intl: { formatMessage },
  label
}) => <div>{formatMessage(label)}</div>)

const PublishingDialog = ({
  name,
  formName,
  dispatch,
  isOpen,
  updateUI,
  isReadOnly,
  isPublishAvailable,
  languagesAvailabilities,
  isPublishing,
  entityId,
  customSuccesPublishCallback,
  intl: { formatMessage },
  ...rest
}) => {
  const lockSuccess = link => () => {
    dispatch(
      setReadOnly({
        form: formName,
        value: false
      })
    )
    // console.log(link)
    scroll.scrollTo(link)
  }

  const handleOnClose = () => {
    updateUI('isOpen', false)
  }

  const handleOnErrorClick = (link) => {
    isReadOnly && dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'lock'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.LOCKENTITY][formName],
          data: { id: entityId }
        },
        formName,
        successNextAction: () => lockSuccess(link)
      })
    )

    updateUI('isOpen', false)
  }

  const handleSuccesPublishOperation = () => {
    updateUI('isOpen', false)
    return customSuccesPublishCallback
  }

  const handleOnSubmit = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'publishEntity'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.PUBLISHENTITY][formName],
          data: { id: entityId, languagesAvailabilities }
        },
        schemas: formSchemas[formName],
        successNextAction: handleSuccesPublishOperation,
        formName
      })
    )
  }

  return (
    <Modal isOpen={isOpen} onRequestClose={handleOnClose} contentLabel='' >
      <ModalTitle title={formatMessage(messages.dialogTitle)}>
        <div>{formatMessage(messages.dialogDescription)}</div>
      </ModalTitle>
      <ModalContent>
        <LanguagesTable
          customSetup
          prependColumns={[{
            property: 'name',
            header: {
              formatters: [props => <HeaderFormatter label={CellHeaders.publishTitle} />]
            },
            cell: {
              formatters: [
                (name, { rowData, rowIndex }) => {
                  return (
                    <PublishingCheckbox
                      {...rowData}
                      onClick={handleOnErrorClick}
                      rowIndex={rowIndex}
                    />
                  )
                }
              ]
            }
          }]}
        />
      </ModalContent>
      <ModalActions>
        <Button onClick={handleOnSubmit}
          small
          disabled={!isPublishAvailable || isPublishing}
        >
          {isPublishing && <Spinner /> || formatMessage(messages.submitTitle)}
        </Button>
        <Button onClick={handleOnClose}
          link
          disabled={isPublishing}>
          {formatMessage(messages.closeDialogButtonTitle)}
        </Button>
      </ModalActions>
    </Modal>
  )
}
PublishingDialog.propTypes = {
  name: PropTypes.string,
  formName: PropTypes.string,
  entityId: PropTypes.string,
  isOpen: PropTypes.bool,
  isPublishing: PropTypes.bool.isRequired,
  languagesAvailabilities: ImmutablePropTypes.list.isRequired,
  isPublishAvailable: PropTypes.bool.isRequired,
  updateUI: PropTypes.func,
  dispatch: PropTypes.func.isRequired,
  intl: intlShape,
  customSuccesPublishCallback: PropTypes.any
}

export default compose(
  injectIntl,
  withFormStates,
  withState({
    redux: true,
    key: ({ formName }) => `${formName}PublishingDialog`,
    initialState: {
      isOpen: false
    }
  }),
  connect((state, { formName }) => ({
    isPublishAvailable: getFormIsAnyLanguageToPublish(state, { formName }) && getEntityCanBeAnyPublished(state),
    languagesAvailabilities: getFormLanguagesAvailabilitiesTransformed(state, { formName }),
    entityId: getSelectedEntityId(state),
    isPublishing: EntitySelectors[formEntityTypes[formName]].getEntityDialogIsPublishing(state)
  }))
)(PublishingDialog)
