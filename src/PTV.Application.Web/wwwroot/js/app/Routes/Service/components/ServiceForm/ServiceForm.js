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
import ServiceBasic from '../ServiceBasic'
import ServiceClassificationAndKeywords from '../ServiceClassificationAndKeywords'
import ServiceProducerInfo from '../ServiceProducerInfo'
import { serviceBasicTransformer } from '../serviceTransformers'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'
import { Accordion } from 'appComponents/Accordion'
import {
  withBubbling,
  withConnectionStep,
  withEntityButtons,
  withEntityHeader,
  withEntityTitle,
  withFormStates,
  withNotification,
  withPublishingDialog,
  withPreviewDialog
} from 'util/redux-form/HOC'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import {
  LanguageComparisonSelect,
  ValidationMessages
} from 'appComponents'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { getService } from 'Routes/Service/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { EntitySchemas } from 'schemas'
import { messages as commonFormMessages } from 'Routes/messages'
import { formTypesEnum } from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'
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

const ServiceForm = ({
  handleSubmit,
  form,
  intl: { formatMessage },
  isCompareMode
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
      <Accordion>
        <Accordion.Title validateFields={[
          'name', 'organization'
        ]}
          title={formatMessage(messages.formTitle)}
        />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <Accordion.Content>
          <ServiceBasic />
        </Accordion.Content>
      </Accordion>
      <Accordion>
        <Accordion.Title
          title={formatMessage(messages.formTitle2)}
          helpText={formatMessage(messages.formTitle2InfoText)} />
        <Accordion.Content>
          <ServiceClassificationAndKeywords />
        </Accordion.Content>
      </Accordion>
      <Accordion>
        <Accordion.Title
          title={formatMessage(messages.formTitle3)}
          />
        <Accordion.Content>
          <ServiceProducerInfo />
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
ServiceForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: PropTypes.object.isRequired,
  isCompareMode: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'service/SaveService',
  transformers: [
    languagesAvailabilitiesTransformer,
    serviceBasicTransformer
  ],
  schema: EntitySchemas.SERVICE
})

const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.SERVICEFORM, getService)
}

const onSubmitFail = (...args) => console.log(args)

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICEFORM)

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    initialValues: getService(state, ownProps),
    isLoading: EntitySelectors.services.getEntityIsFetching(state) || getIsLoading(state)
  })),
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
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  withConnectionStep,
  withEntityButtons({
    formNameToSubmit: formTypesEnum.SERVICEFORM
  }),
  withEntityHeader({
    entityId: null
  }),
  withFormStates,
  withPublishingDialog(),
  withPreviewDialog
)(ServiceForm)
