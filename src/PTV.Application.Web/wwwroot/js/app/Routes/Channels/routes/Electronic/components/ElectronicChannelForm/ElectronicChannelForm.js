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
import { Sticky, StickyContainer } from 'react-sticky'
import { OpeningHours } from 'util/redux-form/sections'
import { getChannel } from 'Routes/Channels/routes/Electronic/actions'
import { ElectronicChannelBasic } from 'Routes/Channels/routes/Electronic/components'
import { Accordion } from 'appComponents/Accordion'
import {
  withBubbling,
  withEntityButtons,
  withEntityHeader,
  withFormStates,
  withConnectionStep,
  withNotification,
  withEntityTitle,
  withPublishingDialog,
  withPreviewDialog
} from 'util/redux-form/HOC'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  OpeningHoursPreview,
  ValidationMessages,
  LanguageComparisonSelect
} from 'appComponents'
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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { messages as commonFormMessages } from 'Routes/messages'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { EntitySchemas } from 'schemas'
import cx from 'classnames'
import styles from './styles.scss'
import { formTypesEnum } from 'enums'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'

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

const ElectronicChannelForm = ({ handleSubmit, form, intl: { formatMessage }, isReadOnly, isCompareMode }) => {
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
        <Accordion.Content>
          <ElectronicChannelBasic />
        </Accordion.Content>
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
ElectronicChannelForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape,
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool
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
const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.ELECTRONICCHANNELFORM, getElectronicChannel)
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
        initialValues
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
    getAction: getChannel
  }),
  withPublishingDialog(),
  withPreviewDialog
  // withAutomaticSave({
  //   period: 10,
  //   draftJSFields: ['description']
  // })
)(ElectronicChannelForm)
