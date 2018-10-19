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
import { connect } from 'react-redux'
import { reduxForm } from 'redux-form/immutable'
import { Accordion } from 'appComponents/Accordion'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'util/react-intl'
import { getOrganizationFormInitialValues } from 'Routes/Organization/selectors'
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
import withNotification from 'util/redux-form/HOC/withNotification'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import OrganizationFormBasic from 'Routes/Organization/components/OrganizationFormBasic'
import ContactInformationCollections from 'Routes/Organization/components/ContactInformationCollections'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { messages as commonFormMessages } from 'Routes/messages'
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
import { formTypesEnum } from 'enums'
import { getOrganization, loadOrganizationsData } from 'Routes/Organization/actions'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'
import cx from 'classnames'
import styles from './styles.scss'
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'

const OrganizationForm = ({
  handleSubmit,
  form,
  isCompareMode,
  intl: { formatMessage }
}) => {
  const formClass = cx(
    styles.form,
    {
      [styles.compareMode]: isCompareMode
    }
  )
  return (
    <form onSubmit={handleSubmit} className={formClass}>
      <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['webPages', 'emails'])} />
      <Accordion>
        <Accordion.Title title={formatMessage(messages.formTitle)}
          validateFields={[
            'name', 'organization'
          ]}
      />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <Accordion.Content>
          <div>
            <OrganizationFormBasic />
            <ContactInformationCollections />
          </div>
        </Accordion.Content>
      </Accordion>
      <div className='row'>
        <div className='col-lg-12'>
          <ValidationMessages form={form} />
        </div>
      </div>
    </form>
  )
}
OrganizationForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool,
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

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getOrganizationFormInitialValues)
}

const onSubmitFail = (...args) => console.log(args)

const handleArchiveResponse = (response) => {
  const data = response && response.response && response.response.result ? response.response.result : null
  if (data && data.anyConnected) {
    return data.subOrganizationsConnected
      ? messages.subOrganizationText
      : messages.serviceAndChannelText
  }
  return null
}

const getIsLoading = getIsFormLoading(formTypesEnum.ORGANIZATIONFORM)

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
    warn: validateForPublish
  }),
  withBubbling,
  withNotification(),
  withFormStates,
  withAutomaticSave({
    draftJSFields: ['description']
  }),
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  withEntityButtons({
    getHasErrorSelector: createGetHasError(['webPages', 'emails'])
  }),
  withEntityHeader({
    entityId: null,
    handleArchiveResponse: handleArchiveResponse
  }),
  withPublishingDialog({
    customSuccesPublishCallback: () => loadOrganizationsData()
  })
)(OrganizationForm)
