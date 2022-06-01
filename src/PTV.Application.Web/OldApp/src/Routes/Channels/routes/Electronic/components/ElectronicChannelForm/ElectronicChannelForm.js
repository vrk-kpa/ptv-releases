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
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import { withChannelQualityAgent } from 'Routes/Channels/hoc'
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
import validate from 'util/redux-form/util/validate'
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { loadConnections, successArchiveAction } from '../../../../actions'
import { commonChannelValidators, urlValidators } from 'Routes/Channels/validators'
import { directQualityEntityCheck, qualityCheckCancelChanges } from 'actions/qualityAgent'
import { collapseExpandMainAccordions } from 'Routes/actions'
import { getMainAccordionItemCountByForm } from 'util/helpers'
import { getChannelAdditionalQualityCheckData } from 'Routes/Channels/selectors'
import { getLanguageAvailabilityCodes } from 'selectors/selections'
import { setPhoneForEChannel } from 'actions/phones'

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

const basicPublishValidators = [
  ...commonChannelValidators,
  ...urlValidators
]
const publishValidators = [
  ...basicPublishValidators
]
const fieldsRequiredForSave = [
  'name',
  'organization'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)

class ElectronicChannelForm extends Component {
  render () {
    const {
      handleSubmit,
      form,
      intl: { formatMessage },
      isCompareMode,
      isInReview
    } = this.props
    const formClass = cx(
      styles.form,
      {
        [styles.compareMode]: isCompareMode
      }
    )
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['attachments', 'emails'])} />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <ReduxAccordion
          isExclusive={false}
          reduxKey={'electronicAccordion'}
          initialIndex={getMainAccordionItemCountByForm(form)}
          alwaysOpen={isInReview}
        >
          <ReduxAccordion.Title
            publishFields={basicPublishFields}
            saveFields={fieldsRequiredForSave}
            title={formatMessage(messages.formTitle)}
          />
          <ReduxAccordion.Content>
            <ElectronicChannelBasic />
          </ReduxAccordion.Content>
          <ReduxAccordion.Title
            validate={false}
            title={formatMessage(messages.formTitle2)}
          />
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
}
ElectronicChannelForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape,
  isCompareMode: PropTypes.bool,
  isInReview: PropTypes.bool
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
  const data = getChannelAdditionalQualityCheckData(state, { formName })
  directQualityEntityCheck(data, store, options)
}

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getElectronicChannel)
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

const onSubmitFail = console.log

const getIsLoading = getIsFormLoading(formTypesEnum.ELECTRONICCHANNELFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getElectronicChannel(state, ownProps)
      return {
        isLoading,
        initialValues,
        templateId: ownProps.templateId,
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
    warn,
    shouldWarn: () => true
  }),
  withBubbling({
    framed: true,
    padded: true
  }),
  withFormStates,
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
    handleNewLanguageResponse: setPhoneForEChannel
  }),
  withPublishingDialog({
    customSuccesPublishCallback: (newId, id) => loadConnections(newId, id)
  }),
  withPreviewDialog,
  withChannelQualityAgent
)(ElectronicChannelForm)
