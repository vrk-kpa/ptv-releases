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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { reduxForm } from 'redux-form/immutable'
import ServiceCollectionBasic from '../ServiceCollectionBasic'
import { serviceCollectionTransformer } from '../ServiceCollectionTransformers'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import { Accordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { getServiceCollection } from 'Routes/ServiceCollection/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { EntitySchemas } from 'schemas'
import { messages as commonFormMessages } from 'Routes/messages'
import { formTypesEnum } from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import validate from 'util/redux-form/util/validate'
import { isRequired } from 'util/redux-form/validators'
import cx from 'classnames'
import styles from './styles.scss'

export const messages = defineMessages({
  entityTitleNew: {
    id: 'Routes.ServiceCollection.Main.Title',
    defaultMessage: 'Lisää palvelukokonaisuus'
  },
  formTitle: {
    id: 'Routes.ServiceCollection.Header.Title',
    defaultMessage: 'Perustiedot'
  },
  organizationTitle: {
    id: 'Routes.ServiceCollection.Organization.Title',
    defaultMessage: 'Vastuuorganisaatio'
  }
})

const basicPublishValidators = [
  { path: 'name', validate: isRequired('name') },
  { path: 'organization', validate: isRequired('organization') }
]
const publishValidators = [
  ...basicPublishValidators
]
const fieldsRequiredForSave = [
  'name',
  'organization'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)

class ServiceCollectionForm extends Component {
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
      intl: { formatMessage },
      isCompareMode
    } = this.props
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect />
        <Accordion>
          <Accordion.Title
            publishFields={basicPublishFields}
            saveFields={fieldsRequiredForSave}
            title={formatMessage(messages.formTitle)}
          />
          <div className='row'>
            <div className='col-lg-12'>
              <ValidationMessages form={form} top />
            </div>
          </div>
          <Accordion.Content>
            <ServiceCollectionBasic />
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
}
ServiceCollectionForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  isCompareMode: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'serviceCollection/SaveServiceCollection',
  transformers: [
    languagesAvailabilitiesTransformer,
    serviceCollectionTransformer
  ],
  schema: EntitySchemas.SERVICECOLLECTION
})

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getServiceCollection)
}

const onSubmitFail = (...args) => console.log(args)

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICECOLLECTIONFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.serviceCollections.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getServiceCollection(state, ownProps)
      return {
        isLoading,
        initialValues,
        copyId: ownProps.copyId
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.SERVICECOLLECTIONFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess,
    warn
  }),
  withBubbling({
    framed: true,
    padded: true
  }),
  withAutomaticSave({
    draftJSFields: [
      'description'
    ]
  }),
  withEntityNotification,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  withConnectionStep,
  withEntityButtons({
    formNameToSubmit: formTypesEnum.SERVICECOLLECTIONFORM
  }),
  withEntityHeader({
    entityId: null
  }),
  withFormStates,
  withPublishingDialog(),
  withPreviewDialog
)(ServiceCollectionForm)
