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
import { reduxForm } from 'redux-form/immutable'
import { OpeningHours } from 'util/redux-form/sections'
import ElectronicChannelBasic from 'Routes/Channels/routes/Electronic/components/ElectronicChannelBasic'
import { ReduxAccordion } from 'appComponents/Accordion'
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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { getElectronicChannel } from 'Routes/Channels/routes/Electronic/selectors'
import { electronicChannelBasicTransformer } from 'Routes/Channels/routes/Electronic/submitFilters'
import {
  openingHoursTransformer,
  phoneNumbersTransformer,
  areaInformationTransformer
} from 'util/redux-form/submitFilters'
import { electronicChannelBasicValidation } from 'Routes/Channels/routes/Electronic/validators'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'util/react-intl'
import { messages as commonFormMessages } from 'Routes/messages'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { EntitySchemas } from 'schemas'
import cx from 'classnames'
import styles from './styles.scss'
import { formTypesEnum } from 'enums'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { loadConnections } from '../../../../actions'

const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Channels.AddElectronicChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: verkkoasiointi'
  },
  formTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Header.Title',
    defaultMessage: 'Vaihe 1/2: Perustiedot'
  },
  formTitle2: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  }
})

const ElectronicChannelForm = ({
  handleSubmit,
  form,
  intl: { formatMessage },
  isReadOnly,
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
      <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['attachments', 'emails'])} />
      <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'ElectronicChannelAccordion'}>
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
          <ElectronicChannelBasic />
        </ReduxAccordion.Content>
        <ReduxAccordion.Title title={formatMessage(messages.formTitle2)} />
        <ReduxAccordion.Content>
          <div className='form-row'>
            <OpeningHours />
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
ElectronicChannelForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape,
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  inTranslation: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'channel/SaveElectronicChannel',
  schema: EntitySchemas.CHANNEL,
  transformers: [
    electronicChannelBasicTransformer,
    languagesAvailabilitiesTransformer,
    openingHoursTransformer(['openingHours']),
    phoneNumbersTransformer,
    areaInformationTransformer
  ],
  validators: [
    electronicChannelBasicValidation
  ]
})
const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getElectronicChannel)
}

const onSubmitFail = console.log

const getIsLoading = getIsFormLoading(formTypesEnum.ELECTRONICCHANNELFORM)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getElectronicChannel(state, ownProps)
      return {
        isLoading,
        initialValues,
        copyId: ownProps.copyId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.ELECTRONICCHANNELFORM,
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
  withPublishingDialog({
    customSuccesPublishCallback: (newId, id) => loadConnections(newId, id)
  }),
  withPreviewDialog
)(ElectronicChannelForm)
