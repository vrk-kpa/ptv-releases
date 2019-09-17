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
import { branch } from 'recompose'
import { reduxForm } from 'redux-form/immutable'
import GeneralDescriptionBasic from '../GeneralDescriptionBasic'
import GeneralDescriptionClassificationAndKeywords from '../GeneralDescriptionClassificationAndKeywords'
import { generalDescriptionBasicTransformer } from 'Routes/GeneralDescription/components/generalDescriptionTransformers'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import { ReduxAccordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { getGeneralDescription } from 'Routes/GeneralDescription/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { Label } from 'sema-ui-components'
import { EntitySchemas } from 'schemas'
import { formTypesEnum } from 'enums'
import { messages as commonFormMessages } from 'Routes/messages'
// import { getGeneralDescription as getGeneralDescriptionAction } from 'Routes/GeneralDescription/actions'
import cx from 'classnames'
import styles from './styles.scss'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import validate from 'util/redux-form/util/validate'
import {
  isRequired,
  isRequiredDraftJs,
  isNotEmpty
} from 'util/redux-form/validators'

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

const basicPublishValidators = [
  { path: 'name', validate: isRequired() },
  { path: 'shortDescription', validate: isRequired() },
  { path: 'description', validate: isRequiredDraftJs() }
]
const classificationPublishValidators = [
  { path: 'targetGroups', validate: isNotEmpty() },
  { path: 'serviceClasses', validate: isNotEmpty() },
  { path: 'ontologyTerms', validate: isNotEmpty() }
]
const publishValidators = [
  ...basicPublishValidators,
  ...classificationPublishValidators
]
const fieldsRequiredForSave = [
  'name',
  'organization'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)
const classificationPublishFields = classificationPublishValidators
  .map(validator => validator.path)

class GeneralDescriptionForm extends Component {
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
      inTranslation,
      isCompareMode
    } = this.props
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect />
        <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'generalDescriptionAccordion1'}>
          <ReduxAccordion.Title
            publishFields={basicPublishFields}
            saveFields={fieldsRequiredForSave}
            title={formatMessage(messages.formTitle)}
          />
          <div className='row'>
            <div className='col-lg-12'>
              <ValidationMessages form={form} top />
            </div>
          </div>
          <ReduxAccordion.Content>
            <GeneralDescriptionBasic />
          </ReduxAccordion.Content>
          <ReduxAccordion.Title
            publishFields={classificationPublishFields}
            title={formatMessage(messages.formTitle2)}
          >
            <Label infoLabel labelText={formatMessage(messages.formTitle2InfoText)} />
          </ReduxAccordion.Title>
          <ReduxAccordion.Content>
            <GeneralDescriptionClassificationAndKeywords />
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
GeneralDescriptionForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  isCompareMode: PropTypes.bool,
  inTranslation: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'generalDescription/SaveGeneralDescription',
  transformers: [
    generalDescriptionBasicTransformer
  ],
  schema: EntitySchemas.GENERAL_DESCRIPTION
})

const onSubmitFail = (values, x1, x2, x4) => console.log('values:\n', values, x2, x4)

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getGeneralDescription)
}

const getIsLoading = getIsFormLoading(formTypesEnum.GENERALDESCRIPTIONFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    initialValues: getGeneralDescription(state, ownProps),
    isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state) || getIsLoading(state),
    isInReview: getShowReviewBar(state)
  })),
  reduxForm({
    form: formTypesEnum.GENERALDESCRIPTIONFORM,
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
  withFormStates,
  withAutomaticSave({
    draftJSFields: [
      'description',
      'conditionOfServiceUsage',
      'userInstruction',
      'backgroundDescription'
    ]
  }),
  withEntityNotification,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  branch(({ isInReview }) => !isInReview, withConnectionStep),
  withEntityButtons({
    formNameToSubmit: formTypesEnum.GENERALDESCRIPTIONFORM
  }),
  withEntityHeader({
    entityId: null,
    successArchiveAction: () => {}
  }),
  withPublishingDialog(),
  withPreviewDialog
)(GeneralDescriptionForm)
