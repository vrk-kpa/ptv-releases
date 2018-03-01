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
import { reduxForm } from 'redux-form/immutable'
import { Accordion } from 'appComponents/Accordion'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import {
  withEntityButtons,
  withEntityHeader,
  withBubbling,
  withFormStates,
  withEntityTitle,
  withNotification,
  withPublishingDialog,
  withPreviewDialog,
  withConnectionStep
} from 'util/redux-form/HOC'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
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
import { loadPrintableFormChannel } from 'Routes/Channels/routes/PrintableForm/actions'
import {
  LanguageComparisonSelect,
  ValidationMessages
} from 'appComponents'
import PrintableFormBasic from '../PrintableFormBasic'
import cx from 'classnames'
import styles from './styles.scss'
import { validateForPublish } from 'util/redux-form/syncValidation/publishing'

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
        <PrintableFormBasic />
      </Accordion.Content>
    </Accordion>
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
  intl: intlShape
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
const onSubmitSuccess = (props, dispatch) => {
  handleOnSubmitSuccess(props, dispatch, formTypesEnum.PRINTABLEFORM, getPrintableForm)
}

const getIsLoading = getIsFormLoading(formTypesEnum.PRINTABLEFORM)

export default compose(
  injectIntl,
  connect(state => ({
    initialValues: getPrintableForm(state),
    isLoading: EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
  })),
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
)(PrintableFormForm)
