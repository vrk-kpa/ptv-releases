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
import {
  Modal,
  ModalTitle,
  ModalActions,
  ModalContent,
  Button,
  Spinner
} from 'sema-ui-components'
import { compose } from 'redux'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import Tooltip from 'appComponents/Tooltip'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { LanguagesTable } from 'util/redux-form/fields'
import PublishingCheckbox from './PublishingCheckbox'
import withState from 'util/withState'
import { connect } from 'react-redux'
import {
  getEntityCanBeAllPublished,
  getEntityCanBeAnyPublished,
  getSelectedEntityId
} from 'selectors/entities/entities'
import {
  getFormIsAnyLanguageToPublish,
  getFormIsAnyLanguageToSchedule,
  getFormLanguagesAvailabilitiesTransformed
} from 'selectors/form'
import { formActions, formActionsTypesEnum, formEntityTypes, formTypesEnum, formPaths } from 'enums'
import { apiCall3, API_CALL_CLEAN } from 'actions'
import { EntitySchemas } from 'schemas'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { EntitySelectors } from 'selectors'
import { getPublishingStatusPublishedId, getExpireOn } from 'selectors/common'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { setReadOnly } from 'reducers/formStates'
import { reset, getFormSyncErrors } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import Scroll from 'react-scroll'
const scroll = Scroll.scroller
import styles from './styles.scss'
import DateTimeInputCell from 'appComponents/Cells/DateTimeInputCell/DateTimeInputCell'
import { canArchiveAstiEntity } from 'Routes/Service/selectors'
import moment from 'moment'
import { getDisabledLanguageIds, isEntityPublished } from './selectors'
import { Iterable } from 'immutable'
import { isEmpty } from 'lodash'
import ErrorLabel from 'appComponents/ErrorLabel'
import { collapseExpandMainAccordions } from 'Routes/actions'

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
  dialogDescriptionInfo: {
    id: 'Components.PublishingEntityDialog.Information',
    defaultMessage: 'When the content is published on the current day before 1pm, it is shown on Suomi.fi service on the next morning.' // eslint-disable-line max-len
  },
  dialogDescriptionTooltip: {
    id: 'Components.PublishingEntityDialog.Information.Tooltip',
    defaultMessage: 'Info icon text.'
  },
  closeDialogButtonTitle: {
    id: 'Components.ModalDialog.Buttons.Close.Title',
    defaultMessage: 'Peruuta'
  },
  submitTitle: {
    id: 'Components.PublishingEntityForm.Button.Submit.Title',
    defaultMessage: 'Julkaise'
  },
  validFromCell: {
    id: 'Components.PublishingEntityForm.Cell.ValidFrom',
    defaultMessage: 'Julkaisupäivä'
  },
  validToCell: {
    id: 'Components.PublishingEntityForm.Cell.ValidTo',
    defaultMessage: 'Arkistoitumispäivä'
  },
  validFromTooltip: {
    id: 'Components.PublishingEntityForm.Cell.ValidFrom.Tooltip',
    defaultMessage: 'Select date to schedule the publishing.'
  },
  draftValidFromTooltip: {
    id: 'Components.PublishingEntityForm.Cell.ValidFrom.Draft.Tooltip',
    defaultMessage: 'If you publish today, it will be visible on suomi.fi the next day. You can change the date to schedule the publishing.' // eslint-disable-line max-len
  },
  validToTooltip: {
    id: 'Components.PublishingEntityForm.Cell.ValidTo.Tooltip',
    defaultMessage: 'The data needs to be checked and updated at least once a year or it will be automatically archived.' // eslint-disable-line max-len
  },
  informationMissingLabel: {
    id: 'Components.PublishingEntityDialog.MissingInformation',
    defaultMessage: 'Pakollisia tietoja puuttuu. Tarkista tiedot ennen julkaisua.'
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
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noschema',
  [formTypesEnum.SERVICEOLD]: EntitySchemas.SERVICE_FORM
}

const HeaderFormatter = compose(
  injectIntl
)(({
  intl: { formatMessage },
  label
}) => <div>{formatMessage(label)}</div>)

const PublishingDialog = props => {
  const {
    formName: propsFormName,
    dispatch,
    isOpen,
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
    canArchiveAsti,
    isAnyPublishable,
    isAnySchedulable,
    disablePublishLanguageIds,
    isEntityPublished,
    isSyncErrorInForm,
    areAllPublishable,
    ...rest
  } = props

  const formName = (propsFormName === formTypesEnum.SERVICEFORM) ? formTypesEnum.SERVICEOLD : propsFormName

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
      formPaths[formName] && rest.history &&
        rest.history.replace(`${formPaths[formName]}/${data.response.result.result}`)
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
    collapseExpandMainAccordions(dispatch, formName)
    return typeof customSuccesPublishCallback === 'function' &&
      customSuccesPublishCallback(data.response.result.result, entityId)
  }

  const handleOnSubmit = () => {
    isAnyPublishable && handlePublish()
    isAnySchedulable && !isAnyPublishable && handleSchedule()
  }
  const handlePublish = () => {
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

  const handleSchedule = () => {
    dispatch(
      apiCall3({
        keys: [formEntityTypes[formName], 'publishEntity'],
        payload: {
          endpoint: formActions[formActionsTypesEnum.SCHEDULEENTITY][formName],
          data: { id: entityId, languagesAvailabilities }
        },
        schemas: formSchemas[formName],
        successNextAction: handleSuccesPublishOperation,
        formName
      })
    )
  }
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
              disabled={disablePublishLanguageIds.has(rowData.languageId)}
            />
          )
        }
      ]
    }
  }, {
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
              readOnly={!rowData.canBePublished ||
                rowData.newStatusId !== publishStatusId ||
                disablePublishLanguageIds.has(rowData.languageId) ||
                rowData.statusId === publishStatusId
              }
              showInitialDate={rowData.newStatusId !== publishStatusId}
              formatMessage={formatMessage}
              filterDate={expireOn}
              filterDatePath={['languagesAvailabilities', rowIndex, 'validTo']}
              defaultValue={moment().utc().startOf('day')}
              ignoreDefaultValue
              currentDays
              includeTooltip
              tooltipProps={{
                tooltip: isEntityPublished
                  ? formatMessage(messages.validFromTooltip)
                  : formatMessage(messages.draftValidFromTooltip),
                hideOnScroll: true,
                indent: 'i5'
              }}
            />
          )
        }
      ]
    }
  }, {
    property: 'validTo',
    header: {
      label: <HeaderFormatter label={messages.validToCell} />
    },
    cell: {
      formatters: [
        (validTo, { rowIndex, rowData }) => {
          return <DateTimeInputCell
            name={`languagesAvailabilities[${rowIndex}].validTo`}
            valuePath={['languagesAvailabilities', rowIndex, 'validTo']}
            label={null}
            type='to'
            isCompareMode={false}
            compare={false}
            readOnly={rowData.newStatusId !== publishStatusId ||
              !canArchiveAsti ||
              disablePublishLanguageIds.has(rowData.languageId)
            }
            showInitialDate={rowData.newStatusId !== publishStatusId}
            formatMessage={formatMessage}
            filterDatePath={['languagesAvailabilities', rowIndex, 'validFrom']}
            futureDays
            includeTooltip
            tooltipProps={{
              tooltip: formatMessage(messages.validToTooltip),
              hideOnScroll: true,
              indent: 'i5'
            }}
          />
        }
      ]
    }
  }]

  const publishDisabled = isSyncErrorInForm || !isPublishAvailable || isPublishing
  const tableClass = isSyncErrorInForm ? styles.hasError : undefined
  return (
    <Modal isOpen={isOpen} onRequestClose={handleOnClose} contentLabel='' className={styles.publishDialog}>
      <ModalTitle title={formatMessage(messages.dialogTitle)}>
        <div>{formatMessage(messages.dialogDescription)}</div>
        <div className={styles.descriptionInfo}>
          {formatMessage(messages.dialogDescriptionInfo)}
          <Tooltip withinText tooltip={formatMessage(messages.dialogDescriptionTooltip)} hideOnScroll />
        </div>
      </ModalTitle>
      <ModalContent>
        <div className={styles.publishingTab}>
          <LanguagesTable
            columnWidths={['10%', '20%', '20%', '10%', '20%', '10%', '10%']}
            isArchived={false}
            customSetup
            prependColumns={prependColumns}
            tight
            componentClass={tableClass}
          />
        </div>
        {!areAllPublishable && <ErrorLabel labelText={formatMessage(messages.informationMissingLabel)} />}
      </ModalContent>
      <ModalActions>
        <Button onClick={() => handleOnSubmit()}
          small
          disabled={publishDisabled}
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
  publishStatusId: PropTypes.string,
  isReadOnly: PropTypes.bool,
  expireOn: PropTypes.number.isRequired,
  canArchiveAsti: PropTypes.bool,
  isAnyPublishable: PropTypes.bool,
  isAnySchedulable: PropTypes.bool,
  disablePublishLanguageIds: PropTypes.any,
  isEntityPublished: PropTypes.bool,
  isSyncErrorInForm: PropTypes.bool,
  areAllPublishable: PropTypes.bool
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
  connect((state, { formName }) => {
    const isAnyPublishable = getFormIsAnyLanguageToPublish(state, { formName })
    const isAnySchedulable = getFormIsAnyLanguageToSchedule(state, { formName })
    const disablePublishLanguageIds = getDisabledLanguageIds(state)
    const syncWarnings = getFormSyncErrors(formName)(state)
    return {
      areAllPublishable: getEntityCanBeAllPublished(state),
      isPublishAvailable: (isAnyPublishable || isAnySchedulable) && getEntityCanBeAnyPublished(state),
      languagesAvailabilities: getFormLanguagesAvailabilitiesTransformed(state, { formName }),
      entityId: getSelectedEntityId(state),
      expireOn: getExpireOn(state),
      publishStatusId:getPublishingStatusPublishedId(state),
      isPublishing: EntitySelectors[formEntityTypes[formName]].getEntityDialogIsPublishing(state),
      canArchiveAsti: canArchiveAstiEntity(state),
      isAnySchedulable,
      isAnyPublishable,
      disablePublishLanguageIds,
      isEntityPublished: isEntityPublished(state),
      isSyncErrorInForm: Iterable.isIterable(syncWarnings) ? syncWarnings.size > 0 : !isEmpty(syncWarnings)
    }
  }, {
    resetForm: reset,
    mergeInUIState
  })
)(PublishingDialog)
