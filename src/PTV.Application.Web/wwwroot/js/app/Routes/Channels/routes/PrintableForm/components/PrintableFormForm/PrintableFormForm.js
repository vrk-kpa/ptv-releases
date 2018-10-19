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
import { branch } from 'recompose'
import { reduxForm } from 'redux-form/immutable'
import { ReduxAccordion } from 'appComponents/Accordion'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withNotification from 'util/redux-form/HOC/withNotification'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import withEntityExpireWarning from 'util/redux-form/HOC/withEntityExpireWarning'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'util/react-intl'
import { EntitySchemas } from 'schemas'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { getPrintableForm } from 'Routes/Channels/routes/PrintableForm/selectors'
import { messages as commonFormMessages } from 'Routes/messages'
import { printableFormTransformer, addressTransformer } from 'Routes/Channels/routes/PrintableForm/transformers'
import { phoneNumbersTransformer, areaInformationTransformer } from 'util/redux-form/submitFilters'
import { connect } from 'react-redux'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { formTypesEnum } from 'enums'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import PrintableFormBasic from '../PrintableFormBasic'
import cx from 'classnames'
import styles from './styles.scss'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { loadConnections } from '../../../../actions'

const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Channels.AddPrintableFormChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: tulostettava lomake'
  },
  formTitle: {
    id: 'Components.AddPrintableFormChannel.SubTitle1',
    defaultMessage: 'Vaihe 1/1: Perustiedot'
  }
})

const PrintableFormForm = ({
  handleSubmit,
  form,
  isCompareMode,
  inTranslation,
  intl: { formatMessage }
}) => {
  const formClass = cx(
    styles.form,
    {
      [styles.compareMode]: isCompareMode
    }
  )
  return <form onSubmit={handleSubmit} className={formClass}>
    <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['attachments', 'emails'])} />
    <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'printableChannelAccordion'}>
      <ReduxAccordion.Title title={formatMessage(messages.formTitle)}
        validateFields={[
          'name', 'organization'
        ]}
      />
      <div className='row'>
        <div className='col-lg-12'>
          <ValidationMessages form={form} top />
        </div>
      </div>
      <ReduxAccordion.Content>
        <PrintableFormBasic />
      </ReduxAccordion.Content>
    </ReduxAccordion>
    <div className='row'>
      <div className='col-lg-12'>
        <ValidationMessages form={form} />
      </div>
    </div>
  </form>
}
PrintableFormForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  isCompareMode: PropTypes.bool,
  form: PropTypes.string.isRequired,
  intl: intlShape,
  inTranslation: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'channel/SavePrintableFormChannel',
  schema: EntitySchemas.CHANNEL,
  transformers: [
    addressTransformer,
    printableFormTransformer,
    languagesAvailabilitiesTransformer,
    phoneNumbersTransformer,
    areaInformationTransformer
  ]
})
const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getPrintableForm)
}

const getIsLoading = getIsFormLoading(formTypesEnum.PRINTABLEFORM)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getPrintableForm(state, ownProps)
      return {
        isLoading,
        initialValues,
        copyId: ownProps.copyId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.PRINTABLEFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitSuccess,
    warn: validateForPublish
  }),
  withBubbling,
  withNotification(),
  withAutomaticSave({
    draftJSFields: ['description']
  }),
  withEntityExpireWarning,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  branch(({ isInReview }) => !isInReview, withConnectionStep),
  withEntityButtons({
    getHasErrorSelector: createGetHasError(['attachments', 'emails'])
  }),
  withEntityHeader({
    entityId: null
  }),
  withFormStates,
  withPublishingDialog({
    customSuccesPublishCallback: (newId, id) => loadConnections(newId, id)
  }),
  withPreviewDialog
)(PrintableFormForm)
