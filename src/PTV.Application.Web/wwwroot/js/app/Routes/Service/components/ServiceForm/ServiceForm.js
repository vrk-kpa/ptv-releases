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
import { branch } from 'recompose'
import { reduxForm, formValueSelector, change } from 'redux-form/immutable'
import ServiceBasic from '../ServiceBasic'
import ServiceClassificationAndKeywords from '../ServiceClassificationAndKeywords'
import ServiceProducerInfo from '../ServiceProducerInfo'
import { serviceBasicTransformer } from '../serviceTransformers'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import { ReduxAccordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withEntityExpireWarning from 'util/redux-form/HOC/withEntityExpireWarning'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withNotification from 'util/redux-form/HOC/withNotification'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { getService } from 'Routes/Service/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { EntitySchemas } from 'schemas'
import { messages as commonFormMessages } from 'Routes/messages'
import { formTypesEnum } from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'
import { getGDPublishedLanguages } from 'Routes/Service/components/ServiceComponents/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { Map } from 'immutable'
import cx from 'classnames'
import styles from './styles.scss'

export const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Services.AddService.Header.Title',
    defaultMessage: 'Lisää uusi palvelu'
  },
  formTitle: {
    id: 'Containers.Services.AddService.Step1.Header.Title',
    defaultMessage: 'Vaihe 1/4: Palvelun perustiedot'
  },
  formTitle2: {
    id: 'Containers.Services.AddService.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/4: Luokittelu ja ontologiakäsitteet'
  },
  formTitle2InfoText: {
    id: 'Routes.Service.ServiceForm.Section2.InfoText',
    defaultMessage: 'Tämän osion tiedot eivät näy loppukäyttäjille.'
  },
  formTitle3: {
    id: 'Containers.Services.AddService.Step3.Header.Title',
    defaultMessage: 'Vaihe 3/4: Palvelun tuottaminen ja alueelliset tiedot'
  }
}
)

const handleNewLanguageResponse = (newLanguageCode, { getState, dispatch }) => {
  const state = getState()
  const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)

  const serviceName = getServiceFormSelector(state, 'name') || Map()
  const generalDescriptionId = getServiceFormSelector(state, 'generalDescriptionId')
  const generalDescription = generalDescriptionId && EntitySelectors.generalDescriptions.getEntity(state,
    { id: generalDescriptionId })
  const generalDescriptionName = generalDescription && generalDescription.get('name') || Map()
  const publishedGDLanguages = getGDPublishedLanguages(state, { formName:formTypesEnum.SERVICEFORM })

  let someNameIsOverwritenWithGD = false
  if (generalDescriptionName && generalDescriptionName.size > 0) {
    serviceName.forEach((name, langCode) => {
      if (publishedGDLanguages.contains(langCode)) {
        someNameIsOverwritenWithGD = name === generalDescriptionName.get(langCode)
        if (someNameIsOverwritenWithGD) return
      }
    })

    if (someNameIsOverwritenWithGD && publishedGDLanguages.contains(newLanguageCode)) {
      let nameGD = generalDescriptionName.get(newLanguageCode)
      let newServiceNames = serviceName.set(newLanguageCode, nameGD)

      dispatch(
        change(
          formTypesEnum.SERVICEFORM,
          'name',
          newServiceNames
        )
      )
    }
  }
}

const ServiceForm = ({
  handleSubmit,
  form,
  intl: { formatMessage },
  isCompareMode,
  inTranslation
}) => {
  const formClass = cx(
    styles.form,
    {
      [styles.compareMode]: isCompareMode
    }
  )

  return (
    <form onSubmit={handleSubmit} className={formClass}>
      <LanguageComparisonSelect />
      <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'serviceAccordion1'} >
        <ReduxAccordion.Title
          validateFields={[
            'name', 'organization'
          ]}
          title={formatMessage(messages.formTitle)}
        />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <ReduxAccordion.Content>
          <ServiceBasic />
        </ReduxAccordion.Content>
      </ReduxAccordion>
      <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'serviceAccordion2'}>
        <ReduxAccordion.Title
          title={formatMessage(messages.formTitle2)}
          helpText={formatMessage(messages.formTitle2InfoText)} />
        <ReduxAccordion.Content>
          <ServiceClassificationAndKeywords />
        </ReduxAccordion.Content>
      </ReduxAccordion>
      <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'serviceAccordion3'}>
        <ReduxAccordion.Title
          title={formatMessage(messages.formTitle3)}
        />
        <ReduxAccordion.Content>
          <ServiceProducerInfo />
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
ServiceForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  isCompareMode: PropTypes.bool,
  inTranslation: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'service/SaveService',
  transformers: [
    languagesAvailabilitiesTransformer,
    serviceBasicTransformer
  ],
  schema: EntitySchemas.SERVICE
})

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getService)
}

const onSubmitFail = (...args) => console.log(args)

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICEFORM)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.services.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getService(state, ownProps)
      return {
        isLoading,
        initialValues,
        copyId: ownProps.copyId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.SERVICEFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess,
    warn: validateForPublish
  }),
  withBubbling,
  withNotification(),
  withAutomaticSave({
    draftJSFields: [
      'description',
      'conditionOfServiceUsage',
      'userInstruction'
    ]
  }),
  withEntityExpireWarning,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  branch(({ isInReview }) => !isInReview, withConnectionStep),
  withEntityButtons({
    formNameToSubmit: formTypesEnum.SERVICEFORM
  }),
  withEntityHeader({
    entityId: null,
    handleNewLanguageResponse: handleNewLanguageResponse
  }),
  withFormStates,
  withPublishingDialog(),
  withPreviewDialog
)(ServiceForm)
