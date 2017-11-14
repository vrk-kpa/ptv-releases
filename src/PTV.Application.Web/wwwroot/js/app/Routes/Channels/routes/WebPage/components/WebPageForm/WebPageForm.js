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
import { EntitySchemas } from 'schemas'
import {
  withEntityButtons,
  withEntityHeader,
  withFormStates,
  withEntityTitle,
  withBubbling,
  withNotification,
  withPublishingDialog,
  withPreviewDialog,
  withConnectionStep
} from 'util/redux-form/HOC'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { messages as commonFormMessages } from 'Routes/messages'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { getWebPageChannel } from 'Routes/Channels/routes/WebPage/selectors'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { phoneNumbersTransformer, areaInformationTransformer } from 'util/redux-form/submitFilters'
import { webPageChannelTransformer } from 'Routes/Channels/routes/WebPage/submitFilters'

import { formTypesEnum } from 'enums'
import { getChannel } from 'Routes/Channels/routes/WebPage/actions'
import {
  LanguageComparisonSelect,
  ValidationMessages
} from 'appComponents'
import WebPageBasic from '../WebPageBasic'
import cx from 'classnames'
import styles from './styles.scss'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'

const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Channels.AddWebPageChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: verkkosivu'
  },
  formTitle: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Header.Title',
    defaultMessage: 'Vaihe 1/1: Perustiedot'
  },
  urlCheckerTooltip:  {
    id: 'Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Tooltip',
    defaultMessage:
      'Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  }
})

const WebPageForm = ({
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
  return <form onSubmit={handleSubmit} className={formClass}>
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
        <WebPageBasic />
      </Accordion.Content>
    </Accordion>
    <div className='row'>
      <div className='col-lg-12'>
        <ValidationMessages form={form} />
      </div>
    </div>
  </form>
}
WebPageForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  isCompareMode: PropTypes.bool,
  form: PropTypes.string.isRequired,
  intl: intlShape
}

const onSubmit = handleOnSubmit({
  url: 'channel/SaveWebPageChannel',
  transformers: [
    languagesAvailabilitiesTransformer,
    webPageChannelTransformer,
    phoneNumbersTransformer,
    areaInformationTransformer
  ],
  schema: EntitySchemas.CHANNEL
})

const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.WEBPAGEFORM, getWebPageChannel)
}

const getIsLoading = getIsFormLoading(formTypesEnum.WEBPAGEFORM)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getWebPageChannel(state, ownProps)
      return {
        isLoading,
        initialValues
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.WEBPAGEFORM,
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
)(WebPageForm)
