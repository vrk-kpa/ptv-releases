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
import { Sticky, StickyContainer } from 'react-sticky'
import { Accordion } from 'appComponents/Accordion'
import { injectIntl, intlShape, defineMessages, FormattedMessage } from 'react-intl'
import { OpeningHours } from 'util/redux-form/sections'
import {
  OpeningHoursPreview,
  LanguageComparisonSelect,
  ValidationMessages
} from 'appComponents'
import {
  withEntityButtons,
  withEntityHeader,
  withBubbling,
  withFormStates,
  withEntityTitle,
  withNotification,
  withPublishingDialog,
  withPreviewDialog,
  withConnectionStep,
  withAutomaticSave
} from 'util/redux-form/HOC'
import { EntitySchemas } from 'schemas'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { getServiceLocationChannel } from 'Routes/Channels/routes/ServiceLocation/selectors'
import {
  addressesTransformer,
  openingHoursTransformer,
  phoneNumbersTransformer,
  faxNumbersTransformer,
  areaInformationTransformer
} from 'util/redux-form/submitFilters'
import { serviceLocationChannelTransformer } from 'Routes/Channels/routes/ServiceLocation/transformers'
import { formTypesEnum } from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { messages as commonFormMessages } from 'Routes/messages'
import ServiceLocationBasic from '../ServiceLocationBasic'
import cx from 'classnames'
import styles from './styles.scss'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'

const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Channels.AddLocationChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: palvelupiste'
  },
  formTitle: {
    id: 'Containers.Channels.AddLocationChannel.StepContainer1.Title',
    defaultMessage: 'Vaihe 1/4: Perustiedot'
  },
  formTitle2: {
    id: 'Containers.Channels.AddLocationChannel.StepContainer4.Title',
    defaultMessage: 'Vaihe 4/4: Aukioloajat'
  }
})

const ServiceLocationForm = ({
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
      <LanguageComparisonSelect />
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
          <ServiceLocationBasic />
        </Accordion.Content>
        <Accordion.Title title={formatMessage(messages.formTitle2)} />
        <Accordion.Content>
          <div className='form-row'>
            <OpeningHours />
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
ServiceLocationForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool,
  intl: intlShape
}

const onSubmit = handleOnSubmit({
  url: 'channel/SaveServiceLocationChannel',
  schema: EntitySchemas.CHANNEL,
  transformers: [
    languagesAvailabilitiesTransformer,
    serviceLocationChannelTransformer,
    addressesTransformer(['visitingAddresses']),
    addressesTransformer(['postalAddresses']),
    openingHoursTransformer(['openingHours']),
    phoneNumbersTransformer,
    faxNumbersTransformer,
    areaInformationTransformer
  ]
})

const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.SERVICELOCATIONFORM, getServiceLocationChannel)
}

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICELOCATIONFORM)

export default compose(
  injectIntl,
  connect(state => ({
    initialValues: getServiceLocationChannel(state),
    isLoading: EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
  })),
  reduxForm({
    form: formTypesEnum.SERVICELOCATIONFORM,
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
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  withConnectionStep,
  withEntityButtons(),
  withEntityHeader({
    entityId: null
  }),
  withFormStates,
  withPublishingDialog(),
  withPreviewDialog
)(ServiceLocationForm)
