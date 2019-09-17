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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import {
  reduxForm,
  formValueSelector,
  change,
  reset
} from 'redux-form/immutable'
import { defineMessages, FormattedMessage, injectIntl, intlShape } from 'util/react-intl'
import {
  Label,
  Button,
  Spinner
} from 'sema-ui-components'
import {
  Fulltext,
  ServiceType,
  GeneralDescriptionType,
  DataTable
} from 'util/redux-form/fields'
import { mergeInUIState } from 'reducers/ui'
import { apiCall3, createGetEntityAction } from 'actions'
import { EntitySchemas } from 'schemas'
import { EntitySelectors } from 'selectors'
import { connect } from 'react-redux'
import {
  getGneralDescriptionSearchIsFetching,
  getGneralDescriptionSearchResults
} from 'Routes/Service/selectors'
import {
  getIsGDAvailableInContentLanguage,
  getGDPublishedLanguages
} from 'Routes/Service/components/ServiceComponents/selectors'
import Spacer from 'appComponents/Spacer'
import Tag from 'appComponents/Tag'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asContainer from 'util/redux-form/HOC/asContainer'
import styles from './styles.scss'
import { serviceDescriptionMessages } from '../Messages'
import { API_CALL_CLEAN, deleteApiCall } from 'actions'
import ClearModal from './ClearModal'
import OverwriteModal from './OverwriteModal'
import LoadMoreButton from '../LoadMoreButton'
import { formTypesEnum } from 'enums'
import { getContentLanguageCode } from 'selectors/selections'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import { getTranslationLanguageCodes } from 'selectors/common'
import { getNewServiceName } from './selectors'
import { getUiSortingData } from 'selectors/base'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import { PTVIcon } from 'Components'
import ButtonCell from 'appComponents/Cells/ButtonCell'
import NameCell from 'appComponents/Cells/NameCell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import GeneralDescriptionTypeCell from 'appComponents/Cells/GeneralDescriptionTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import asComparable from 'util/redux-form/HOC/asComparable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'

export const messages = defineMessages({
  gdSelectTitle: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.Title',
    defaultMessage: 'Palvelun pohjakuvaus'
  },
  gdNameEmpty: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.Name.Empty',
    defaultMessage: 'Ei valittua pohjakuvausta'
  },
  gdUseOfDescriptionEmpty: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.UseOfGeneralDescription.Empty',
    defaultMessage: 'Use of general description is not provided.'
  },
  gdUseOfDescriptionInfoTitle: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.UseOfGeneralDescription.InfoTitle',
    defaultMessage: 'Pohjakuvauksen käyttöohjeteksti ei näy loppukäyttäjille, mutta on ohjeena palvelun ylläpitäjälle.'
  },
  gdSelectButton: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.Button.Select.Title',
    defaultMessage: 'Vahvista'
  },
  gdSelectInfo: {
    id: 'Containers.Services.AddService.Step1.Service.ConnectGeneralDescription.Tooltip',
    defaultMessage: 'Palvelun pohjakuvaus on yleispätevä ja valtakunnallisesti kattava palvelun kuvaus. Valitse, haluatko liittää palveluusi pohjakuvauksen. Kun pohjakuvaus liitetään, sen tiedot näytetään osana palvelun tietoja.'
  },
  searchGdTitle: {
    id: 'Containers.Services.AddService.Step1.GeneralDescription.Link.Change.Title',
    defaultMessage: 'Vaihda pohjakuvaus'
  },
  searchGdButtonTitle: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
  },
  clearGdButtonTitle: {
    id: 'Components.Buttons.ClearList',
    defaultMessage: 'Tyhjennä lista'
  }
})

const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)
const GeneralDescriptionHeaderComponent = ({
  label,
  change,
  language,
  reset,
  generalDescriptionId,
  mergeInUIState,
  isReadOnly,
  isAddingNewLanguage,
  intl: { formatMessage }
}) => {
  const handleCrossClick = (event) => {
    mergeInUIState({
      key: 'clearSelectedGeneralDescription',
      value: {
        isOpen: true
      }
    })
  }
  const handleOnClear = e => {
    reset(formTypesEnum.GENERALDESCRIPTIONSEARCHFORM)
    change(formTypesEnum.SERVICEFORM, 'generalDescriptionId', null)
    change(formTypesEnum.GENERALDESCRIPTIONSEARCHFORM, 'generalDescriptionId', null)
  }

  return (
    isReadOnly || isAddingNewLanguage
      ? <div>{label}</div>
      : <div className='form-row'>
        <ClearModal onClear={handleOnClear} />
        <div className={styles.gdHeader}>
          {generalDescriptionId
            ? <div className={styles.attachedGD}>
              <Label labelText={formatMessage(serviceDescriptionMessages.optionConnectedDescriptionTitle)} />
              <Tag
                message={label}
                isRemovable
                onTagRemove={handleCrossClick}
                normal
              />
            </div>
            : <Label labelText={label} />
          }
        </div>
      </div>)
}

GeneralDescriptionHeaderComponent.propTypes = {
  label: PropTypes.node.isRequired,
  generalDescriptionId: PropTypes.string,
  mergeInUIState: PropTypes.func.isRequired,
  reset: PropTypes.func.isRequired,
  change: PropTypes.func.isRequired,
  intl: intlShape.isRequired,
  isReadOnly: PropTypes.bool,
  isAddingNewLanguage: PropTypes.bool,
  language: PropTypes.string
}

const GeneralDescriptionHeader = compose(
  injectIntl,
  withFormStates,
  connect((state, ownProps) => {
    const generalDescriptionId = getServiceFormSelector(state, 'generalDescriptionId')
    const serviceName = getServiceFormSelector(state, 'name')
    const generalDescription = EntitySelectors.generalDescriptions.getEntity(state, { id: generalDescriptionId })
    const selectedGeneralDescriptionName = generalDescription && generalDescription.get('name')
    const language = getContentLanguageCode(state, { formName: formTypesEnum.SERVICEFORM })
    const selectedGeneralDescription = selectedGeneralDescriptionName &&
      selectedGeneralDescriptionName.get(language)
    const isGDAvailable = getIsGDAvailableInContentLanguage(state, { formName: formTypesEnum.SERVICEFORM })
    return {
      generalDescriptionId,
      label: (isGDAvailable && selectedGeneralDescription) ||
      (generalDescriptionId
        ? ownProps.intl.formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)
        : <FormattedMessage {...messages.gdNameEmpty} />),
      language,
      serviceName,
      selectedGDName: selectedGeneralDescriptionName
    }
  }, {
    change,
    reset,
    mergeInUIState
  }))(GeneralDescriptionHeaderComponent)

const GeneralDescriptionLabelComponent = ({
  label,
  useOfGD,
  useOfGDInfoTitle,
  isEmptyLabel,
  mergeInUIState,
  loadPreviewEntity,
  generalDescriptionId
}) => {
  const handleOnPreviewClick = () => {
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: 'generalDescriptionForm',
        isOpen: true,
        entityId: generalDescriptionId
      }
    })
  }
  return (
    <div className='row'>
      <div className='col'>
        <div className={styles.attachedGDLabel}>
          {generalDescriptionId &&
          <PTVIcon name='icon-eye' onClick={handleOnPreviewClick} componentClass={styles.previewIcon} />
          }

          <Label
            labelText={label}
            emptyLabel={isEmptyLabel}
          />
        </div>
        {generalDescriptionId && useOfGD !== undefined && <div className={styles.useOfGD}>
          <Label
            labelText={useOfGD}
          />
          <Label
            labelText={useOfGDInfoTitle}
            infoLabel
          />
        </div>}
      </div>
    </div>
  )
}

GeneralDescriptionLabelComponent.propTypes = {
  label: PropTypes.string,
  useOfGD: PropTypes.any,
  useOfGDInfoTitle: PropTypes.string,
  isEmptyLabel: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  loadPreviewEntity: PropTypes.func,
  generalDescriptionId: PropTypes.string
}

const GeneralDescriptionLabel = compose(
  injectIntl,
  withLanguageKey,
  injectFormName,
  asComparable(),
  asLocalizable,
  connect((state, ownProps) => {
    const generalDescriptionId = ownProps.generalDescriptionId || getServiceFormSelector(state, 'generalDescriptionId')
    const generalDescription = EntitySelectors.generalDescriptions.getEntity(state, { id: generalDescriptionId })

    const gdDescription = {
      label: ownProps.intl.formatMessage(messages.gdNameEmpty),
      useOfGD: ownProps.intl.formatMessage(messages.gdUseOfDescriptionEmpty),
      useOfGDInfoTitle: ''
    }

    if (generalDescriptionId) {
      const selectedGeneralDescriptionName = generalDescription &&
      generalDescription.get('name') &&
      generalDescription.get('name').get(ownProps.language)

      const selectedGeneralDescriptionUseOfGD = generalDescription &&
        generalDescription.get('generalDescriptionTypeAdditionalInformation') &&
        generalDescription.get('generalDescriptionTypeAdditionalInformation').get(ownProps.language)

      gdDescription.label = selectedGeneralDescriptionName ||
        ownProps.intl.formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)
      gdDescription.useOfGD = selectedGeneralDescriptionUseOfGD
      gdDescription.useOfGDInfoTitle = ownProps.intl.formatMessage(messages.gdUseOfDescriptionInfoTitle)
    }

    return {
      generalDescriptionId,
      isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state),
      ...gdDescription
    }
  }, {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  }),
)(GeneralDescriptionLabelComponent)

const getColumnsDefinition = ({
  radioOnChange,
  handleGDSelection,
  previewOnClick,
  formName,
  formProperty,
  formatMessage }) => (onSort) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.name)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => <NameCell viewIcon viewOnClick={previewOnClick}{...rowData} />
        ]
      }
    },
    {
      property: 'serviceTypeId',
      header: {
        label: formatMessage(CellHeaders.generalDescriptionHeaderTitle)
      },
      cell: {
        formatters: [
          (serviceTypeId, { rowData }) => <ServiceTypeCell {...rowData} />
        ]
      }
    },
    {
      property: 'generalDescriptionTypeId',
      header: {
        label: formatMessage(CellHeaders.generalDescriptionType)
      },
      cell: {
        formatters: [
          (generalDescriptionTypeId, { rowData }) => <GeneralDescriptionTypeCell {...rowData} />
        ]
      }
    },
    {
      property: 'id',
      cell: {
        formatters: [
          (id, { rowData }) => (
            <ButtonCell {
              ...{
                ...{
                  formName: formTypesEnum.SERVICEFORM,
                  formProperty,
                  onClick: handleGDSelection
                },
                ...rowData
              }}
            />
          )
        ]
      }
    }/*,
    {
      property: 'generalDescriptionsTypeAdditionalInformation',
      header: {
        label: formatMessage(CellHeaders.generalDescriptionsTypeAdditionalInformation)
      },
      cell: {
        formatters: [
          (generalDescriptionsTypeAdditionalInformation, { rowData }) => <GeneralDescriptionTypeAdditionalInformationCell {...rowData} />
        ]
      }
    } */
  ]
}

const GeneralDescriptionSearchForm = ({
  form,
  dispatch,
  gdSearchIsFetching,
  label,
  change,
  customChange,
  apiCall3,
  mergeInUIState,
  language,
  gdSearchResults,
  reset,
  intl: { formatMessage },
  submit,
  isReadOnly,
  isAddingNewLanguage,
  serviceName,
  languagesAvailabilities
}) => {
  const _handleSubmit = e => {
    e.preventDefault()
    submit()
  }
  const _handleOnClear = e => {
    e.preventDefault()
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['service', 'generalDescription', 'search']
    })
    reset()
  }

  const _handleGDSelection = (generalDescriptionId) => {
    customChange(formTypesEnum.SERVICEFORM, 'generalDescriptionId', generalDescriptionId)
    customChange(formTypesEnum.GENERALDESCRIPTIONSEARCHFORM, 'generalDescriptionId', generalDescriptionId)
    apiCall3(
      {
        payload:
        {
          endpoint: 'generalDescription/GetGeneralDescription',
          data: { id: generalDescriptionId, language, onlyPublished: true }
        },
        keys: ['generalDescriptions', 'load'],
        schemas: EntitySchemas.GENERAL_DESCRIPTION,
        formName: formTypesEnum.SERVICEFORM,
        successNextAction: _applyGDSelection
      }
    )
  }

  const _applyGDSelection = () => ({ getState }) => {
    const state = getState()
    const generalDescriptionId = getServiceFormSelector(state, 'generalDescriptionId')
    const publishedGDLanguages = getGDPublishedLanguages(state, { formName: formTypesEnum.SERVICEFORM })
    const newServiceName = getNewServiceName(state, { generalDescriptionId, formName: formTypesEnum.SERVICEFORM })
    deleteApiCall(['service', 'generalDescription', 'search'])
    existNameToOverride(publishedGDLanguages)
      ? mergeInUIState({
        key: 'overwriteNameFromGDDialog',
        value: {
          isOpen: true
        }
      })
      : selectGD(generalDescriptionId, newServiceName)
  }

  const selectGD = (generalDescriptionId, newServiceName) => {
    customChange(formTypesEnum.SERVICEFORM, 'name', newServiceName)
    customChange(formTypesEnum.SERVICEFORM, 'generalDescriptionId', generalDescriptionId)
  }

  const existNameToOverride = (publishedGDLanguages) => {
    let exist = false
    languagesAvailabilities.forEach((v, k) => {
      exist = exist || (publishedGDLanguages.contains(v.get('code')) && serviceName.get(v.get('code')))
    })
    return exist
  }

  const _handlePreviewClick = (generalDescriptionId) => {
    const formName = formTypesEnum.GENERALDESCRIPTIONFORM
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true,
        entityId: null
      }
    })
    apiCall3(
      {
        payload:
        {
          endpoint: 'generalDescription/GetGeneralDescription',
          data: { id: generalDescriptionId, language, onlyPublished: true }
        },
        keys: ['generalDescriptions', 'loadPreview'],
        schemas: EntitySchemas.GENERAL_DESCRIPTION,
        formName: formTypesEnum.SERVICEFORM
      })
  }

  return (
    <div>
      <OverwriteModal
        formName={formTypesEnum.GENERALDESCRIPTIONFORM}
        name='overwriteNameFromGDDialog' />
      {!isReadOnly && !isAddingNewLanguage &&
        <div>
          <div>
            <GeneralDescriptionHeader isReadOnly={isReadOnly} isAddingNewLanguage={isAddingNewLanguage} />
          </div>

          <Label
            labelText={formatMessage(messages.searchGdTitle)}
            labelPosition='top'
          />
          <div className={styles.nestedForm}>
            <div className={styles.nestedFormRow}>
              <div className='row'>
                <div className='col-lg-8'>
                  <Fulltext big name='name' label={null} />
                </div>
                <div className='col-lg-8'>
                  <ServiceType name='serviceType' labelPosition='inside' />
                </div>
                <div className='col-lg-8'>
                  <GeneralDescriptionType name='generalDescriptionType' labelPosition='inside' readOnlyValues />
                </div>
              </div>
            </div>
            <div className={styles.nestedFormRow}>
              <Spacer />
            </div>
            <div className={styles.nestedFormRow}>
              <div className={styles.buttonGroup}>
                <Button
                  small
                  onClick={_handleSubmit}
                  children={gdSearchIsFetching && <Spinner /> || formatMessage(messages.searchGdButtonTitle)}
                  disabled={gdSearchIsFetching}
                />
                <Button
                  small
                  onClick={_handleOnClear}
                  secondary
                  children={formatMessage(messages.clearGdButtonTitle)}
                />
              </div>
            </div>
            { gdSearchIsFetching ? <div className={styles.spinner}><Spinner /></div>
              : gdSearchResults.length > 0 && <DataTable
                name='generalDescriptionId'
                rows={gdSearchResults}
                columnsDefinition={getColumnsDefinition({
                  handleGDSelection: _handleGDSelection,
                  previewOnClick: _handlePreviewClick,
                  formName: form,
                  formProperty: 'generalDescriptionId',
                  formatMessage
                })}
                borderless
                scrollable
                columnWidths={['10%', '30%', '15%', '35%', '10%']}
                sortOnClick={submit}
              /> }
          </div>
          <LoadMoreButton />
        </div> || <GeneralDescriptionLabel />}
    </div>
  )
}

GeneralDescriptionSearchForm.propTypes = {
  submit: PropTypes.func.isRequired,
  change: PropTypes.func.isRequired,
  customChange: PropTypes.func.isRequired,
  apiCall3: PropTypes.func.isRequired,
  gdSearchIsFetching: PropTypes.bool.isRequired,
  form: PropTypes.string.isRequired,
  label: PropTypes.string,
  gdSearchResults: PropTypes.array.isRequired,
  intl: intlShape.isRequired,
  mergeInUIState: PropTypes.func,
  reset: PropTypes.func,
  dispatch: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isAddingNewLanguage: PropTypes.bool,
  serviceName: ImmutablePropTypes.map,
  languagesAvailabilities: ImmutablePropTypes.list,
  language: PropTypes.string
}

const onSubmit = (values, dispatch) => {
  dispatch(({ dispatch, getState }) => {
    const sortingData = getUiSortingData(getState(), { contentType: formTypesEnum.GENERALDESCRIPTIONSEARCHFORM })
    dispatch(apiCall3(
      {
        payload: {
          endpoint:
          'generaldescription/v2/SearchGeneralDescriptions',
          data: {
            name: values.get('name'),
            languages: getTranslationLanguageCodes(getState()),
            onlyPublished: true,
            serviceTypeId: values.get('serviceType'),
            generalDescriptionTypeId: values.get('generalDescriptionType'),
            sortData: sortingData.size > 0 ? [sortingData] : []
          }
        },
        keys: ['service', 'generalDescription', 'search'],
        schemas: EntitySchemas.GET_SEARCH(EntitySchemas.GENERAL_DESCRIPTION),
        formName: formTypesEnum.SERVICEFORM
      }))
  })
}

export default compose(
  connect((state, ownProps) => {
    return {
      gdSearchIsFetching: getGneralDescriptionSearchIsFetching(state, ownProps),
      gdSearchResults: getGneralDescriptionSearchResults(state, ownProps),
      language: getContentLanguageCode(state, { formName: formTypesEnum.SERVICEFORM }),
      initialValues: {
        generalDescriptionId: getServiceFormSelector(state, 'generalDescriptionId') &&
          getServiceFormSelector(state, 'generalDescriptionId')
      },
      isAddingNewLanguage: getIsAddingNewLanguage(ownProps.formName)(state),
      serviceName: getServiceFormSelector(state, 'name'),
      languagesAvailabilities: getServiceFormSelector(state, 'languagesAvailabilities')
    }
  }, {
    customChange: change,
    apiCall3,
    mergeInUIState
  }),
  injectIntl,
  withFormStates,
  asContainer({
    uiKey: 'generalDescriptionSelectContainer',
    title: messages.gdSelectTitle,
    tooltip: messages.gdSelectInfo,
    contentWhenCollapsed: GeneralDescriptionLabel
  }),
  reduxForm({
    form: formTypesEnum.GENERALDESCRIPTIONSEARCHFORM,
    onSubmit
  })
)(GeneralDescriptionSearchForm)
