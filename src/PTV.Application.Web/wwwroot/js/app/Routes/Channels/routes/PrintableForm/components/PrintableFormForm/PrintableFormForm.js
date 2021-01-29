/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import { withChannelQualityAgent } from 'Routes/Channels/hoc'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
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
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { loadConnections, successArchiveAction } from '../../../../actions'
import validate from 'util/redux-form/util/validate'
import { commonChannelValidators } from 'Routes/Channels/validators'
import { directQualityEntityCheck, qualityCheckCancelChanges } from 'actions/qualityAgent'
import { collapseExpandMainAccordions } from 'Routes/actions'
import { getMainAccordionItemCountByForm } from 'util/helpers'
import { getAdditionalQualityCheckData } from 'Routes/Channels/selectors'
import { getLanguageAvailabilityCodes } from 'selectors/selections'
import { setPhoneForPrintableForm } from 'actions/phones'

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

const basicPublishValidators = commonChannelValidators
const publishValidators = [
  ...basicPublishValidators
]
const fieldsRequiredForSave = [
  'name',
  'organization'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)

class PrintableFormForm extends Component {
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
      isCompareMode,
      isInReview,
      intl: { formatMessage }
    } = this.props
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['attachments', 'emails'])} />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <ReduxAccordion
          reduxKey={'printableFormAccordion'}
          isExclusive={false}
          initialIndex={getMainAccordionItemCountByForm(form)}
          alwaysOpen={isInReview}
        >
          <ReduxAccordion.Title
            publishFields={basicPublishFields}
            saveFields={fieldsRequiredForSave}
            title={formatMessage(messages.formTitle)}
          />
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
    )
  }
}
PrintableFormForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  isCompareMode: PropTypes.bool,
  form: PropTypes.string.isRequired,
  isInReview: PropTypes.bool,
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

const entityType = 'channel'

const qualityCheck = (store, formName, languages) => {
  const state = store.getState()
  const options = {
    formName,
    entityType,
    profile: 'VRKak',
    languages
  }
  const data = getAdditionalQualityCheckData(state, { formName })
  directQualityEntityCheck(data, store, options)
}

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getPrintableForm)
  dispatch((store) => {
    qualityCheck(store, props.form, Object.keys(result.name))
  })
  collapseExpandMainAccordions(dispatch, props.form)
}

const onEdit = ({ formName }) => store => {
  qualityCheck(store, formName, getLanguageAvailabilityCodes(store.getState()))
  collapseExpandMainAccordions(store.dispatch, formName)
}

const onCancel = ({ formName }) => store => {
  store.dispatch(qualityCheckCancelChanges({ formName }, entityType))
  var isInReview = getShowReviewBar(store.getState())
  if (isInReview) {
    qualityCheck(store, formName, getLanguageAvailabilityCodes(store.getState()))
  }
  collapseExpandMainAccordions(store.dispatch, formName)
}

const getIsLoading = getIsFormLoading(formTypesEnum.PRINTABLEFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getPrintableForm(state, ownProps)
      return {
        isLoading,
        initialValues,
        templateId: ownProps.templateId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.PRINTABLEFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitSuccess,
    warn,
    shouldWarn: () => true
  }),
  withBubbling({
    framed: true,
    padded: true
  }),
  withAutomaticSave({
    draftJSFields: ['name', 'shortDescription', 'description']
  }),
  withEntityNotification,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  branch(({ isInReview }) => !isInReview, withConnectionStep),
  withEntityButtons({
    getHasErrorSelector: createGetHasError(['attachments', 'emails']),
    onEdit,
    onCancel
  }),
  withEntityHeader({
    entityId: null,
    successArchiveAction: successArchiveAction,
    handleNewLanguageResponse: setPhoneForPrintableForm
  }),
  withFormStates,
  withPublishingDialog({
    customSuccesPublishCallback: (newId, id) => loadConnections(newId, id)
  }),
  withPreviewDialog,
  withChannelQualityAgent
)(PrintableFormForm)
