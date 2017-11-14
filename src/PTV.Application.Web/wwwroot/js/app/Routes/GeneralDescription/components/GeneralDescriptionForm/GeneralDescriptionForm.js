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
import GeneralDescriptionBasic from '../GeneralDescriptionBasic'
import GeneralDescriptionClassificationAndKeywords from '../GeneralDescriptionClassificationAndKeywords'
import { generalDescriptionBasicTransformer } from 'Routes/GeneralDescription/components/generalDescriptionTransformers'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'
import { Accordion } from 'appComponents/Accordion'
import {
  withEntityHeader,
  withBubbling,
  withEntityButtons,
  withFormStates,
  withEntityTitle,
  withNotification,
  withPublishingDialog,
  withConnectionStep,
  withPreviewDialog
} from 'util/redux-form/HOC'
import {
  LanguageComparisonSelect,
  ValidationMessages
} from 'appComponents'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { getGeneralDescription } from 'Routes/GeneralDescription/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { Label } from 'sema-ui-components'
import { EntitySchemas } from 'schemas'
import { formTypesEnum } from 'enums'
import { messages as commonFormMessages } from 'Routes/messages'
import { getGeneralDescription as getGeneralDescriptionAction } from 'Routes/GeneralDescription/actions'
import cx from 'classnames'
import styles from './styles.scss'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'

export const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.GeneralDescription.Add.Header.Title',
    defaultMessage: 'Lisää pohjakuvaus'
  },
  formTitle: {
    id: 'Containers.GeneralDescription.Add.Step1.Header.Title',
    defaultMessage: 'Perustiedot'
  },
  formTitle2: {
    id: 'Containers.GeneralDescription.Step2.Title',
    defaultMessage: 'Luokittelu ja asiasanat'
  },
  formTitle2InfoText: {
    id: 'Routes.GeneralDescription.GeneralDescriptionForm.Section2.InfoText',
    defaultMessage: 'Tämän osion tiedot eivät näy loppukäyttäjille.'
  }
})

const GeneralDescriptionForm = ({ handleSubmit, form, intl: { formatMessage }, isCompareMode }) => {
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
        <Accordion.Title
          title={formatMessage(messages.formTitle)}
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
          <GeneralDescriptionBasic />
        </Accordion.Content>
      </Accordion>
      <Accordion>
        <Accordion.Title title={formatMessage(messages.formTitle2)}>
          <Label infoLabel labelText={formatMessage(messages.formTitle2InfoText)} />
        </Accordion.Title>
        <Accordion.Content>
          <GeneralDescriptionClassificationAndKeywords />
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
GeneralDescriptionForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: PropTypes.object.isRequired,
  isCompareMode: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'generalDescription/SaveGeneralDescription',
  transformers: [
    generalDescriptionBasicTransformer
  ],
  schema: EntitySchemas.GENERAL_DESCRIPTION
})

const onSubmitFail = (values, x1, x2, x4) => console.log('values:\n', values, x2, x4)

const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.GENERALDESCRIPTIONFORM, getGeneralDescription)
}

const getIsLoading = getIsFormLoading(formTypesEnum.GENERALDESCRIPTIONFORM)

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    initialValues: getGeneralDescription(state, ownProps),
    isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state) || getIsLoading(state)
  })),
  reduxForm({
    form: formTypesEnum.GENERALDESCRIPTIONFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess,
    warn: validateForPublish
  }),
  withBubbling,
  withNotification,
  withFormStates,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  withConnectionStep,
  withEntityButtons({
    formNameToSubmit: formTypesEnum.GENERALDESCRIPTIONFORM
  }),
  withEntityHeader({
    entityId: null,
    getEndpoint: getGeneralDescriptionAction
  }),
  withPublishingDialog(),
  withPreviewDialog
)(GeneralDescriptionForm)
