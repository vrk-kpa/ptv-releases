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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { reduxForm } from 'redux-form/immutable'
import { Accordion } from 'appComponents/Accordion'
import {
  OpeningHours
} from 'util/redux-form/sections'
import {
  withBubbling,
  withEntityButtons,
  withEntityHeader,
  withEntityTitle,
  withFormStates,
  withNotification,
  withPublishingDialog,
  withPreviewDialog,
  withConnectionStep
} from 'util/redux-form/HOC'
import { Sticky, StickyContainer } from 'react-sticky'
import {
  LanguageComparisonSelect,
  OpeningHoursPreview,
  ValidationMessages
} from 'appComponents'
import { injectIntl, intlShape, defineMessages, FormattedMessage } from 'react-intl'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import {
  openingHoursTransformer,
  phoneNumbersTransformer,
  areaInformationTransformer
} from 'util/redux-form/submitFilters'
import { EntitySchemas } from 'schemas'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { getPhoneChannel } from 'Routes/Channels/routes/Phone/selectors'
import { phoneChannelTransformer } from 'Routes/Channels/routes/Phone/transformers'
import { formTypesEnum } from 'enums'
import { messages as commonFormMessages } from 'Routes/messages'
import { getChannel } from 'Routes/Channels/routes/Phone/actions'
import PhoneChannelBasic from '../PhoneChannelBasic'
import cx from 'classnames'
import styles from './styles.scss'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'

const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Channels.AddPhoneChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: puhelinasiointi'
  },
  formTitle: {
    id: 'Components.AddPhoneChannel.SubTitle1',
    defaultMessage: 'Vaihe 1/2: Perustiedot'
  },
  formTitle2: {
    id: 'Components.AddPhoneChannel.SubTitle2',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  }
})

const PhoneChannelForm = ({
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
          <PhoneChannelBasic />
        </Accordion.Content>
      </Accordion>
      <Accordion>
        <Accordion.Title title={formatMessage(messages.formTitle2)} />
        <Accordion.Content>
          <StickyContainer>
            <div className='form-row'>
              <OpeningHours>
                <Sticky>
                  <OpeningHoursPreview />
                </Sticky>
              </OpeningHours>
            </div>
          </StickyContainer>
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
PhoneChannelForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool,
  intl: intlShape
}

const onSubmit = handleOnSubmit({
  url: 'channel/SavePhoneChannel',
  schema: EntitySchemas.CHANNEL,
  transformers: [
    phoneChannelTransformer,
    languagesAvailabilitiesTransformer,
    openingHoursTransformer(['openingHours']),
    phoneNumbersTransformer,
    areaInformationTransformer
  ]
  // ,
  // validators: [
  //   electronicChannelBasicValidation(['basicInfo'])
  // ]
})

const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.PHONECHANNELFORM, getPhoneChannel)
}

const getIsLoading = getIsFormLoading(formTypesEnum.PHONECHANNELFORM)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getPhoneChannel(state, ownProps)
      return {
        isLoading,
        initialValues
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.PHONECHANNELFORM,
    enableReinitialize: true,
    onSubmit,
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
  withEntityButtons(),
  withEntityHeader({
    entityId: null,
    getEndpoint: getChannel
  }),
  withPublishingDialog(),
  withPreviewDialog
)(PhoneChannelForm)
