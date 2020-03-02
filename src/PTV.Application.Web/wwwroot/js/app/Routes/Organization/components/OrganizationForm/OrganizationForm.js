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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { reduxForm } from 'redux-form/immutable'
import { ReduxAccordion } from 'appComponents/Accordion'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'util/react-intl'
import { getOrganizationFormInitialValues, getAdditionalQualityCheckData } from 'Routes/Organization/selectors'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { EntitySchemas } from 'schemas'
import { organizationBasicTransformer } from 'Routes/Organization/transformers'
import {
  addressesTransformer,
  phoneNumbersTransformer,
  streetAddressesTransformer } from 'util/redux-form/submitFilters'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import OrganizationFormBasic from 'Routes/Organization/components/OrganizationFormBasic'
import ContactInformationCollections from 'Routes/Organization/components/ContactInformationCollections'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { messages as commonFormMessages } from 'Routes/messages'
import { formTypesEnum } from 'enums'
import { loadOrganizationsData } from 'Routes/Organization/actions'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import validate from 'util/redux-form/util/validate'
import areaInformationValidation from 'util/redux-form/validators/areaInformation'
import cx from 'classnames'
import styles from './styles.scss'
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'
import { isRequired, isEqual } from 'util/redux-form/validators'
import { validationMessageTypes } from 'util/redux-form/validators/types'
import { loadUserOrganizationRoles } from 'actions/init'
import { collapseExpandMainAccordions } from 'Routes/actions'
import { getMainAccordionItemCountByForm } from 'util/helpers'
import { withOrganizationQualityAgent } from 'Routes/Organization/hoc'
import { directQualityEntityCheck, qualityCheckCancelChanges } from 'actions/qualityAgent'
import { getLanguageAvailabilityCodes } from 'selectors/selections'

export const messages = defineMessages({
  formTitle: {
    id: 'Containers.Manage.Organizations.Manage.Main.SubTitle.Step1',
    defaultMessage: 'Perustiedot'
  },
  entityTitleNew: {
    id: 'Containers.Manage.Organizations.Manage.Main.Title',
    defaultMessage: 'Uusi organisaatio'
  },
  serviceAndChannelText: {
    id: 'Containers.Organizations.DeleteDialog.ServiceAndChannels',
    defaultMessage:
      'You have services and channels connected to this organization. Do you want to archive all services and channels?'
  },
  subOrganizationText: {
    id: 'Containers.Organizations.DeleteDialog.SubOrganization',
    defaultMessage:
      'You have sub-organization(s), services and channels connected to this organization. ' +
      'Do you want to archive sub-organization and services and channels?'
  }
})

const basicPublishValidators = [
  { path: 'organizationType', validate: isRequired() },
  { path: 'organization', validate: isRequired() },
  { path: 'name', validate: isRequired() },
  {
    path: 'name',
    validate: isEqual('shortDescription')(),
    type: validationMessageTypes.asErrorVisible
  },
  { path: 'shortDescription', validate: isRequired() },
  {
    path: 'shortDescription',
    validate: isEqual('name')(),
    type: validationMessageTypes.asErrorVisible
  },
  { path: 'areaInformation', validate: areaInformationValidation() }
]
const publishValidators = [
  ...basicPublishValidators
]
const fieldsRequiredForSave = [
  'name'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)

class OrganizationForm extends Component {
  render () {
    const formClass = cx(
      styles.form,
      {
        [styles.compareMode]: isCompareMode
      }
    )
    const {
      handleSubmit,
      form,
      isCompareMode,
      isInReview,
      intl: { formatMessage }
    } = this.props
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['webPages', 'emails'])} />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <ReduxAccordion
          isExclusive={false}
          reduxKey={'organizationAccordion'}
          initialIndex={getMainAccordionItemCountByForm(form)}
          alwaysOpen={isInReview}
        >
          <ReduxAccordion.Title
            publishFields={basicPublishFields}
            saveFields={fieldsRequiredForSave}
            title={formatMessage(messages.formTitle)}
          />
          <ReduxAccordion.Content>
            <div>
              <OrganizationFormBasic />
              <ContactInformationCollections />
            </div>
          </ReduxAccordion.Content>
        </ReduxAccordion>
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} />
          </div>
        </div>
      </form>
    )
  }
}
OrganizationForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool,
  isInReview: PropTypes.bool,
  intl: intlShape
}

const onSubmit = handleOnSubmit({
  url: 'organization/SaveOrganization',
  transformers: [
    languagesAvailabilitiesTransformer,
    organizationBasicTransformer,
    addressesTransformer(['visitingAddresses']),
    addressesTransformer(['postalAddresses']),
    phoneNumbersTransformer,
    streetAddressesTransformer('visitingAddresses'),
    streetAddressesTransformer('postalAddresses')
  ],
  schema: EntitySchemas.ORGANIZATION
})

const entityType = 'organization'

const qualityCheck = (store, formName, languages) => {
  const state = store.getState()
  const options = {
    formName,
    entityType,
    profile: 'VRKo',
    languages
  }
  const data = getAdditionalQualityCheckData(state, { formName })
  directQualityEntityCheck(data, store, options)
}

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getOrganizationFormInitialValues)
  dispatch((store) => {
    qualityCheck(store, props.form, Object.keys(result.name))
  })
  collapseExpandMainAccordions(dispatch, props.form)
}

const onEdit = ({ formName }) => store => {
  qualityCheck(store, formName, getLanguageAvailabilityCodes(store.getState()))
  collapseExpandMainAccordions(store.dispatch, formName)
}

const onCancel = ({ formName }) => store => {
  store.dispatch(qualityCheckCancelChanges({ formName }, entityType))
  collapseExpandMainAccordions(store.dispatch, formName)
}

const onSubmitFail = console.log

const handleArchiveResponse = (response) => {
  const data = response && response.response && response.response.result ? response.response.result : null
  if (data && data.anyConnected) {
    return data.subOrganizationsConnected
      ? messages.subOrganizationText
      : messages.serviceAndChannelText
  }
  return null
}

const successArchiveAction = (dispatch) => {
  dispatch(loadUserOrganizationRoles())
}

const getIsLoading = getIsFormLoading(formTypesEnum.ORGANIZATIONFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => ({
      initialValues: getOrganizationFormInitialValues(state, ownProps),
      isLoading: EntitySelectors.organizations.getEntityIsFetching(state) || getIsLoading(state)
    })),
  reduxForm({
    form: formTypesEnum.ORGANIZATIONFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess,
    warn,
    shouldError: () => true
  }),
  withBubbling({
    framed: true,
    padded: true
  }),
  withEntityNotification,
  withFormStates,
  withAutomaticSave({
    draftJSFields: ['description', 'shortDescription']
  }),
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  withEntityButtons({
    getHasErrorSelector: createGetHasError(['webPages', 'emails']),
    onEdit,
    onCancel
  }),
  withEntityHeader({
    entityId: null,
    handleArchiveResponse: handleArchiveResponse,
    successArchiveAction: successArchiveAction
  }),
  withPublishingDialog({
    customSuccesPublishCallback: () => loadOrganizationsData()
  }),
  withOrganizationQualityAgent
)(OrganizationForm)
