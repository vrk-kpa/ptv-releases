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
import { reduxForm, formValueSelector, change, reset } from 'redux-form/immutable'
import { defineMessages, FormattedMessage, injectIntl } from 'react-intl'
import {
  Label,
  Button,
  Spinner,
  ModalActions
} from 'sema-ui-components'
import {
  Fulltext,
  ServiceType,
  GeneralDescriptionTable
} from 'util/redux-form/fields'
import {
  updateUI
} from 'util/redux-ui/action-reducer'
import { mergeInUIState } from 'reducers/ui'
import { apiCall3 } from 'actions'
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
import { Spacer, ModalDialog, Tag } from 'appComponents'
import {
  asContainer,
  withFormStates
} from 'util/redux-form/HOC'
import styles from './styles.scss'
import { serviceDescriptionMessages } from '../Messages'
import { API_CALL_CLEAN, deleteApiCall } from 'Containers/Common/Actions'
import ClearModal from './ClearModal'
import { formTypesEnum } from 'enums'
import { getContentLanguageCode } from 'selectors/selections'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import { Map } from 'immutable'
import {
  getUiSortingData
} from 'Routes/FrontPage/routes/Search/selectors'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import injectFormName from 'util/redux-form/HOC/injectFormName'

export const messages = defineMessages({
  gdSelectTitle: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.Title',
    defaultMessage: 'Palvelun pohjakuvaus'
  },
  gdNameEmpty: {
    id: 'Routes.Service.components.GeneralDescriptionSelect.Name.Empty',
    defaultMessage: 'Ei valittua pohjakuvausta'
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
  },
  dialogOverwriteNameText: {
    id: 'Containers.Services.NameOverwriteDialog',
    defaultMessage: 'Do you want overwrite service name by general description name?'
  },
  buttonOk: {
    id: 'Containers.Services.NameOverwriteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Services.NameOverwriteDialog.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
}
)

const getGeneralDescriptionSearchFormSelector = formValueSelector(formTypesEnum.GENERALDESCRIPTIONSEARCHFORM)
const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)
const GeneralDescriptionHeaderComponent = ({
  label,
  updateUI,
  isLoading,
  change,
  language,
  reset,
  deleteApiCall,
  serviceName,
  generalDescriptionId,
  hasGeneralDescription,
  selectedGDName,
  publishedGDLanguages,
  languagesAvailabilities,
  mergeInUIState,
  isEmptyLabel,
  isReadOnly,
  isAddingNewLanguage,
  intl: { formatMessage }
}) => {
  const _handleSelect = (event) => {
    event.preventDefault()
    deleteApiCall(['service', 'generalDescription', 'search'])
    existNameToOverride()
    ? mergeInUIState({
      key: 'overwriteNameFromGDDialog',
      value: {
        isOpen: true
      }
    })
    : overwriteName()
  }

  const existNameToOverride = () => {
    let exist = false
    languagesAvailabilities.forEach((v, k) => {
      exist = exist || (publishedGDLanguages.contains(v.get('code')) && serviceName.get(v.get('code')))
    })
    return exist
  }

  const overwriteName = () => {
    let newServiceName = Map()
    languagesAvailabilities.forEach((v, k) => {
      newServiceName = publishedGDLanguages.contains(v.get('code'))
      ? newServiceName.set(v.get('code'), selectedGDName.get(v.get('code')))
      : newServiceName.set(v.get('code'), serviceName.get(v.get('code')))
    })
    change(formTypesEnum.SERVICEFORM, 'name', newServiceName)
    selectGD()
    mergeInUIState({
      key: 'overwriteNameFromGDDialog',
      value: {
        isOpen: false
      }
    })
  }

  const cancelOverwrite = () => {
    selectGD()
    mergeInUIState({
      key: 'overwriteNameFromGDDialog',
      value: {
        isOpen: false
      }
    })
  }

  const selectGD = () => {
    change(formTypesEnum.SERVICEFORM, 'generalDescriptionId', generalDescriptionId)
    reset(formTypesEnum.GENERALDESCRIPTIONSEARCHFORM)
    mergeInUIState({
      key: 'generalDescriptionSelectContainer',
      value: {
        isCollapsed: true
      }
    })
  }

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
      <ModalDialog name='overwriteNameFromGDDialog'
        title={formatMessage(messages.dialogOverwriteNameText)}>
        <ModalActions>
          <div className={styles.buttonGroup}>
            <Button small onClick={overwriteName}>
              {formatMessage(messages.buttonOk)}</Button>
            <Button small secondary onClick={cancelOverwrite}>
              {formatMessage(messages.buttonCancel)}
            </Button>
          </div>
        </ModalActions>
      </ModalDialog>
      <ClearModal onClear={handleOnClear} />
      <div className={styles.gdHeader}>
        {!isEmptyLabel
          ? <div className={styles.attachedGD}>
            <Label labelText={formatMessage(serviceDescriptionMessages.optionConnectedDescriptionTitle)} />
            <Tag
              message={label}
              isRemovable={!isEmptyLabel && hasGeneralDescription}
              onTagRemove={handleCrossClick}
              normal
            />
          </div>
          : <Label labelText={label} />
        }
        {isLoading && <Spinner /> || <Button
          type='button'
          children={formatMessage(messages.gdSelectButton)}
          onClick={_handleSelect}
          medium
          disabled={!generalDescriptionId}
        />}
      </div>
    </div>)
}

GeneralDescriptionHeaderComponent.propTypes = {
  label: PropTypes.string.isRequired,
  generalDescriptionId: PropTypes.string.isRequired,
  updateUI: PropTypes.func.isRequired,
  mergeInUIState: PropTypes.func.isRequired,
  change: PropTypes.func.isRequired,
  isLoading: PropTypes.bool.isRequired,
  isEmptyLabel: PropTypes.bool.isRequired,
  intl: PropTypes.object.isRequired,
  isReadOnly: PropTypes.bool,
  isAddingNewLanguage: PropTypes.bool
}

const GeneralDescriptionHeader = compose(
  injectIntl,
  withFormStates,
  connect((state, ownProps) => {
    const generalDescriptionId = getGeneralDescriptionSearchFormSelector(state, 'generalDescriptionId')
    const hasGeneralDescription = !!getServiceFormSelector(state, 'generalDescriptionId')
    const serviceName = getServiceFormSelector(state, 'name')
    const languagesAvailabilities = getServiceFormSelector(state, 'languagesAvailabilities')
    const generalDescription = EntitySelectors.generalDescriptions.getEntity(state, { id: generalDescriptionId })
    const selectedGeneralDescriptionName = generalDescription && generalDescription.get('name')
    const language = getContentLanguageCode(state, { formName: formTypesEnum.SERVICEFORM })
    const selectedGeneralDescription = selectedGeneralDescriptionName &&
      selectedGeneralDescriptionName.get(language)
    const isGDAvailable = getIsGDAvailableInContentLanguage(state, ownProps)
    const publishedGDLanguages = getGDPublishedLanguages(state, ownProps)
    return {
      generalDescriptionId,
      hasGeneralDescription,
      isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state),
      label: (isGDAvailable && selectedGeneralDescription) ||
      (generalDescriptionId
        ? ownProps.intl.formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)
        : <FormattedMessage {...messages.gdNameEmpty} />),
      isEmptyLabel: !generalDescriptionId,
      language,
      serviceName,
      selectedGDName: selectedGeneralDescriptionName,
      publishedGDLanguages,
      languagesAvailabilities
    }
  }, {
    change,
    reset,
    updateUI,
    deleteApiCall,
    mergeInUIState
  }))(GeneralDescriptionHeaderComponent)

const GeneralDescriptionLabelComponent = ({
  label,
  isEmptyLabel
}) => {
  return (
    <div className='row'>
      <div className='col'>
        <Label
          labelText={label}
          emptyLabel={isEmptyLabel}
        />
      </div>
    </div>
  )
}

GeneralDescriptionLabelComponent.propTypes = {
  label: PropTypes.string.isRequired,
  isEmptyLabel: PropTypes.bool
}

const GeneralDescriptionLabel = compose(
  injectIntl,
  withLanguageKey,
  injectFormName,
  connect((state, ownProps) => {
    const generalDescriptionId = ownProps.generalDescriptionId || getServiceFormSelector(state, 'generalDescriptionId')
    const generalDescription = EntitySelectors.generalDescriptions.getEntity(state, { id: generalDescriptionId })
    const languageCode = getContentLanguageCode(state, { formName: ownProps.formName, languageKey: ownProps.languageKey })
    const selectedGeneralDescription = generalDescription &&
      generalDescription.get('name') &&
      generalDescription.get('name').get(languageCode)
    const isGDAvailable = getIsGDAvailableInContentLanguage(state, ownProps)
    return {
      isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state),
      label: ownProps.generalDescriptionId
      ? generalDescription.get('name') && generalDescription.get('name').get(languageCode) ||
        ownProps.intl.formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)
      : (isGDAvailable && selectedGeneralDescription) ||
        (generalDescriptionId
          ? ownProps.intl.formatMessage(serviceDescriptionMessages.languageVersionNotAvailable)
          : ownProps.intl.formatMessage(messages.gdNameEmpty))
    }
  }))(GeneralDescriptionLabelComponent)

const GeneralDescriptionSearchForm = ({
  form,
  dispatch,
  gdSearchIsFetching,
  label,
  change,
  apiCall3,
  mergeInUIState,
  updateUI,
  language,
  generalDescriptionId,
  gdSearchResults,
  reset,
  intl: { formatMessage },
  submit,
  isReadOnly,
  isAddingNewLanguage
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
  const _handleRadioOnChange = (generalDescriptionId) => {
    change('generalDescriptionId', generalDescriptionId)
    apiCall3(
      {
        payload:
        {
          endpoint: 'generalDescription/GetGeneralDescription',
          data: { id: generalDescriptionId, language, onlyPublished: true }
        },
        keys: ['generalDescriptions', 'load'],
        schemas: EntitySchemas.GENERAL_DESCRIPTION,
        formName: formTypesEnum.SERVICEFORM
      })
  }

  const _handlePreviewClick = (generalDescriptionId) => {
    const formName = formTypesEnum.GENERALDESCRIPTIONFORM
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true
      }
    })
    apiCall3(
      {
        payload:
        {
          endpoint: 'generalDescription/GetGeneralDescription',
          data: { id: generalDescriptionId, language, onlyPublished: true }
        },
        keys: ['generalDescriptions', 'load'],
        schemas: EntitySchemas.GENERAL_DESCRIPTION,
        formName: formTypesEnum.SERVICEFORM
      })
  }

  return (
    <div>
      <div>
        <GeneralDescriptionHeader isReadOnly={isReadOnly} isAddingNewLanguage={isAddingNewLanguage} />
      </div>
      {!isReadOnly && !isAddingNewLanguage &&
        <div>
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
                  children={formatMessage(messages.searchGdButtonTitle)}
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
            : gdSearchResults.length > 0 && <GeneralDescriptionTable
              rows={gdSearchResults}
              formName={form}
              formProperty='generalDescriptionId'
              radioOnChange={_handleRadioOnChange}
              previewOnClick={_handlePreviewClick}
              borderless
              scrollable
              columnWidths={['10%', '10%', '30%', '15%', '35%']}
              sortOnClick={submit}
            /> }
          </div>
        </div>
      }
    </div>
  )
}

GeneralDescriptionSearchForm.propTypes = {
  submit: PropTypes.func.isRequired,
  change: PropTypes.func.isRequired,
  apiCall3: PropTypes.func.isRequired,
  updateUI: PropTypes.func.isRequired,
  gdSearchIsFetching: ImmutablePropTypes.list.isRequired,
  form: PropTypes.string.isRequired,
  generalDescriptionId: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  gdSearchResults: PropTypes.string.isRequired,
  intl: PropTypes.object.isRequired,
  mergeInUIState: PropTypes.func,
  reset: PropTypes.func,
  dispatch: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isAddingNewLanguage: PropTypes.bool
}

const onSubmit = (values, dispatch) => {
  dispatch(({ dispatch, getState }) => {
    const sortingData = getUiSortingData(getState(), { contentType: 'generalDescriptionSearch' })
    dispatch(apiCall3(
      {
        payload: {
          endpoint:
          'generaldescription/v2/SearchGeneralDescriptions',
          data: {
            name: values.get('name'),
            languages: ['fi'],
            onlyPublished: true,
            serviceTypeId: values.get('serviceType'),
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
      isAddingNewLanguage: getIsAddingNewLanguage(ownProps.formName)(state)
    }
  }, {
    change,
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
