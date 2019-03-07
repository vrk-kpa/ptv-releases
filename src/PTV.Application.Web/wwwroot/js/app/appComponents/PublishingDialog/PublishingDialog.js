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
  Spinner,
  Tabs,
  Tab
} from 'sema-ui-components'
import { compose } from 'redux'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { LanguagesTable } from 'util/redux-form/fields'
import PublishingCheckbox from './PublishingCheckbox'
import withState from 'util/withState'
import { connect } from 'react-redux'
import {
  getEntityCanBeAnyPublished,
  getSelectedEntityId
} from 'selectors/entities/entities'
import { getFormIsAnyLanguageToPublish, getFormLanguagesAvailabilitiesTransformed } from 'selectors/form'
import { formActions, formActionsTypesEnum, formEntityTypes, formTypesEnum, formPaths } from 'enums'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { EntitySelectors } from 'selectors'
import { getPublishingStatusPublishedId, getExpireOn } from 'selectors/common'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { setReadOnly } from 'reducers/formStates'
import { reset, getFormInitialValues } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import Scroll from 'react-scroll'
const scroll = Scroll.scroller
import styles from './styles.scss'
import DateTimeInputCell from 'appComponents/Cells/DateTimeInputCell/DateTimeInputCell'
import { isScheduleDirty } from './selectors'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'

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
  },
  publishTab: {
    id: 'Components.PublishingEntityForm.Tabs.Publish',
    defaultMessage: 'JULKAISE HETI'
  },
  arrangeTab: {
    id: 'Components.PublishingEntityForm.Tabs.Arrange',
    defaultMessage: 'JULKAISE AJASTETUSI'
  },
  validFromCell: {
    id: 'Components.PublishingEntityForm.Cell.ValidFrom',
    defaultMessage: 'Julkaisupäivä'
  },
  scheduleTitle: {
    id: 'Components.PublishingEntityForm.Button.Schedule.Title',
    defaultMessage: 'Julkaise ajastetusti'
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
  isDirty,
  activeIndex,
  updateUI,
  isReadOnly,
  isPublishAvailable,
  languagesAvailabilities,
  expireOn,
  isPublishing,
  entityId,
  customSuccesPublishCallback,
  resetForm,
  publishStatusId,
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
    updateUI({ 'isOpen': false, 'activeIndex': 0 })
    resetForm(formName)
    dispatch(
      mergeInUIState({
        key: formName + 'InputCell',
        value: { active:false }
      })
    )
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

  const handleSuccesPublishOperation = (data) => {
    updateUI('isOpen', false)
    if (data.response.result.result !== entityId && rest.history) {
      formPaths[formName] && rest.history && rest.history.replace(`${formPaths[formName]}/${data.response.result.result}`)
    }
    resetForm(formName)
    dispatch(
      mergeInUIState({
        key: formName + 'InputCell',
        value: { active:false }
      })
    )
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['entityHistory']
    })
    return typeof customSuccesPublishCallback === 'function' &&
      customSuccesPublishCallback(data.response.result.result, entityId)
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

  const handleOnSchedule = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'publishEntity'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.SCHEDULEENTITY][formName],
          data: { id: entityId, languagesAvailabilities, publishAction:'SchedulePublish' }
        },
        schemas: formSchemas[formName],
        successNextAction: handleSuccesPublishOperation,
        formName
      })
    )
  }

  const handleOnChange = activeIndex => updateUI('activeIndex', activeIndex)

  const prependColumns = [{
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
  }]

  return (
    <Modal isOpen={isOpen} onRequestClose={handleOnClose} contentLabel='' className={styles.publishDialog}>
      <ModalTitle title={formatMessage(messages.dialogTitle)}>
        <div>{formatMessage(messages.dialogDescription)}</div>
      </ModalTitle>
      <ModalContent>
        <Tabs index={activeIndex} onChange={handleOnChange}>
          <Tab label={formatMessage(messages.publishTab)}>
            <div className={styles.publishingTab}>
              <LanguagesTable
                isArchived={false}
                customSetup
                prependColumns={prependColumns}
                appendColumns={[{
                  property: 'validFrom',
                  header: {
                    formatters: [props => <HeaderFormatter label={messages.validFromCell} />]
                  },
                  cell: {
                    formatters: [
                      (name, { rowData, rowIndex }) => {
                        return (
                          <DateTimeInputCell
                            name={`languagesAvailabilities[${rowIndex}].validFrom`}
                            valuePath={['languagesAvailabilities', rowIndex, 'validFrom']}
                            label={null}
                            type='from'
                            isCompareMode={false}
                            compare={false}
                            readOnly
                            showInitialDate={rowData.newStatusId !== publishStatusId}
                            formatMessage={formatMessage}
                          />
                        )
                      }
                    ]
                  }
                }]}
              />
            </div>
          </Tab>
          <Tab label={formatMessage(messages.arrangeTab)}>
            <div className={styles.publishingTab}>
              <LanguagesTable
                isArchived={false}
                customSetup
                prependColumns={prependColumns}
                appendColumns={[{
                  property: 'validFrom',
                  header: {
                    formatters: [props => <HeaderFormatter label={messages.validFromCell} />]
                  },
                  cell: {
                    formatters: [
                      (name, { rowData, rowIndex }) => {
                        return (
                          <DateTimeInputCell
                            name={`languagesAvailabilities[${rowIndex}].validFrom`}
                            valuePath={['languagesAvailabilities', rowIndex, 'validFrom']}
                            label={null}
                            type='from'
                            isCompareMode={false}
                            compare={false}
                            readOnly={rowData.statusId === publishStatusId || !rowData.canBePublished || rowData.newStatusId !== publishStatusId}
                            formatMessage={formatMessage}
                            showInitialDate={rowData.newStatusId !== publishStatusId}
                            filterDate={expireOn}
                          />
                        )
                      }
                    ]
                  }
                }]}
              />
            </div>
          </Tab>
        </Tabs>
      </ModalContent>
      <ModalActions>
        {activeIndex
          ? <Button onClick={() => handleOnSchedule()}
            small
            disabled={!isPublishAvailable || isPublishing || !isDirty}
          >
            {isPublishing && <Spinner /> || formatMessage(messages.scheduleTitle)}
          </Button>
          : <Button onClick={() => handleOnSubmit()}
            small
            disabled={!isPublishAvailable || isPublishing}
          >
            {isPublishing && <Spinner /> || formatMessage(messages.submitTitle)}
          </Button>
        }
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
  customSuccesPublishCallback: PropTypes.any,
  resetForm: PropTypes.func.isRequired,
  activeIndex: PropTypes.number,
  publishStatusId: PropTypes.string,
  isReadOnly: PropTypes.bool,
  isDirty: PropTypes.bool,
  expireOn: PropTypes.number.isRequired
}

export default compose(
  injectIntl,
  withFormStates,
  withState({
    redux: true,
    key: ({ formName }) => `${formName}PublishingDialog`,
    initialState: {
      isOpen: false,
      activeIndex: 0
    }
  }),
  connect((state, { formName }) => {
    const initialValues = getFormInitialValues(formName)(state)
    return {
      isPublishAvailable: getFormIsAnyLanguageToPublish(state, { formName }) && getEntityCanBeAnyPublished(state),
      languagesAvailabilities: getFormLanguagesAvailabilitiesTransformed(state, { formName }),
      entityId: getSelectedEntityId(state),
      expireOn: getExpireOn(state),
      publishStatusId:getPublishingStatusPublishedId(state),
      isPublishing: EntitySelectors[formEntityTypes[formName]].getEntityDialogIsPublishing(state),
      isDirty: isScheduleDirty(state, { formName, initial: initialValues })
    }
  }, {
    resetForm: reset,
    mergeInUIState
  })
)(PublishingDialog)
