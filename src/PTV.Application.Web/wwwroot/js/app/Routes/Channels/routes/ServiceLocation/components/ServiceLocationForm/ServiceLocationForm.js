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
import { ReduxAccordion } from 'appComponents/Accordion'
import { injectIntl, intlShape, defineMessages, FormattedMessage } from 'util/react-intl'
import { OpeningHours } from 'util/redux-form/sections'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import withAddressSearch from 'util/redux-form/HOC/withAddressSearch'
import { withChannelQualityAgent } from 'Routes/Channels/hoc'
import { EntitySchemas } from 'schemas'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { deleteInUIState, mergeInUIState } from 'reducers/ui'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import {
  getServiceLocationChannel
} from 'Routes/Channels/routes/ServiceLocation/selectors'
import {
  addressesTransformer,
  openingHoursTransformer,
  phoneNumbersTransformer,
  faxNumbersTransformer,
  areaInformationTransformer,
  streetAddressesTransformer
} from 'util/redux-form/submitFilters'
import {
  serviceLocationChannelTransformer,
  accessibilityRegisterTransformer
} from 'Routes/Channels/routes/ServiceLocation/transformers'
import { loadConnections, successArchiveAction } from '../../../../actions'
import { formTypesEnum, formActionsTypesEnum } from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { messages as commonFormMessages } from 'Routes/messages'
import ServiceLocationBasic from '../ServiceLocationBasic'
import cx from 'classnames'
import styles from './styles.scss'
import { createGetHasError } from 'util/redux-form/HOC/asContainer/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import validate from 'util/redux-form/util/validate'
import { commonChannelValidators, languageValidators, visitingAddressValidators } from 'Routes/Channels/validators'
import { qualityEntityCheck, qualityEntityCheckIfNotRun, qualityCheckCancelChanges } from 'actions/qualityAgent'
import { canArchiveAstiEntity } from 'Routes/Service/selectors'

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

const handleActionCancel = (action, actionCallback, { getState, dispatch }) => {
  const state = getState()
  const canArchive = canArchiveAstiEntity(state)
  switch (action) {
    case formActionsTypesEnum.ARCHIVEENTITY:
    case formActionsTypesEnum.ARCHIVELANGUAGE:
      canArchive
        ? actionCallback(action)
        : dispatch(mergeInUIState({
          key: `${formTypesEnum.SERVICELOCATIONFORM}${action}CancelDialog`,
          value: {
            isOpen: true,
            action
          }
        }))
      break
    default:
      actionCallback(action)
  }
}

const basicPublishValidators = [
  ...commonChannelValidators,
  ...languageValidators,
  ...visitingAddressValidators
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

class ServiceLocationForm extends Component {
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
      inTranslation,
      intl: { formatMessage }
    } = this.props
    return (
      <form onSubmit={handleSubmit} className={formClass}>
        <LanguageComparisonSelect getHasErrorSelector={createGetHasError(['webPages', 'emails'])} />
        <ReduxAccordion reduxKey={inTranslation ? 'translationAccordion' : 'locationChannelAccordion'}>
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
            <ServiceLocationBasic />
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
ServiceLocationForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool,
  intl: intlShape,
  inTranslation: PropTypes.bool
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
    areaInformationTransformer,
    accessibilityRegisterTransformer,
    streetAddressesTransformer('visitingAddresses'),
    streetAddressesTransformer('postalAddresses')
  ]
})

const entityType = 'channel'

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getServiceLocationChannel)
  dispatch(qualityEntityCheck(result, entityType))
}

const onEdit = ({ formName }) => store => {
  store.dispatch(deleteInUIState({ key: 'accessibilityRegisterPrompt' }))
  store.dispatch(qualityEntityCheckIfNotRun({ formName }, entityType))
}

const onCancel = ({ formName }) => store => {
  store.dispatch(qualityCheckCancelChanges({ formName }, entityType))
}

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICELOCATIONFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.channels.getEntityIsFetching(state) || getIsLoading(state)
      const initialValues = getServiceLocationChannel(state, ownProps)
      return {
        isLoading,
        initialValues,
        templateId: ownProps.templateId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.SERVICELOCATIONFORM,
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
    getHasErrorSelector: createGetHasError(['webPages', 'emails']),
    onEdit,
    onCancel
  }),
  withEntityHeader({
    entityId: null,
    handleActionCancel: handleActionCancel,
    successArchiveAction: successArchiveAction
  }),
  withFormStates,
  withPublishingDialog({
    customSuccesPublishCallback: (newId, id) => loadConnections(newId, id)
  }),
  withPreviewDialog,
  withAddressSearch,
  withChannelQualityAgent
)(ServiceLocationForm)
