/* eslint-disable standard/object-curly-even-spacing */
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
import { connect } from 'react-redux'
import { branch } from 'recompose'
import { reduxForm, formValueSelector, change } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import ServiceBasic from '../ServiceBasic'
import ServiceClassificationAndKeywords from '../ServiceClassificationAndKeywords'
import ServiceProducerInfo from '../ServiceProducerInfo'
import { serviceBasicTransformer } from '../serviceTransformers'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import { ReduxAccordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import withAutomaticSave from 'util/redux-form/HOC/withAutomaticSave'
import withEntityButtons from 'util/redux-form/HOC/withEntityButtons'
import withEntityHeader from 'util/redux-form/HOC/withEntityHeader'
import withEntityNotification from 'util/redux-form/HOC/withEntityNotification'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withConnectionStep from 'util/redux-form/HOC/withConnectionStep'
import withEntityTitle from 'util/redux-form/HOC/withEntityTitle'
import withPublishingDialog from 'util/redux-form/HOC/withPublishingDialog'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import withQualityAgentProvider from 'util/redux-form/HOC/withQualityAgentProvider'
import ValidationMessages from 'appComponents/ValidationMessages'
import LanguageComparisonSelect from 'appComponents/LanguageComparisonSelect'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import validate from 'util/redux-form/util/validate'
import {
  hasMaxBulletCountDraftJS,
  hasValidServiceClasses,
  hasValueDraftJSWithGDCheck,
  isRequired,
  isRequiredDraftJs,
  isNotEmpty,
  isEqual,
  hasMinLengthDraftJS,
  hasMaxItemCountWithGD,
  serviceProducerValidation,
  arrayValidator
} from 'util/redux-form/validators'
import { validationMessageTypes } from 'util/redux-form/validators/types'
import {
  getService,
  getAdditionalQualityCheckData,
  getServiceByGeneralDescription,
  canArchiveAstiEntity
} from 'Routes/Service/selectors'
import { EntitySelectors } from 'selectors'
import { getIsFormLoading } from 'selectors/formStates'
import { EntitySchemas } from 'schemas'
import { messages as commonFormMessages } from 'Routes/messages'
import { customValidationMessages } from 'util/redux-form/validators/messages'
import {
  formTypesEnum,
  formActionsTypesEnum,
  DRAFTJS_MIN_LENGTH,
  DRAFTJS_MAX_BULLET_COUNT,
  SERVICE_CLASSES_MAX_COUNT,
  ONTOLOGY_TERMS_MAX_COUNT
} from 'enums'
import { languagesAvailabilitiesTransformer } from 'util/redux-form/transformers'
import { getGDPublishedLanguages } from 'Routes/Service/components/ServiceComponents/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { Map, List } from 'immutable'
import cx from 'classnames'
import styles from './styles.scss'
import { directQualityEntityCheck, qualityCheckCancelChanges } from 'actions/qualityAgent'
import { collapseExpandMainAccordions } from 'Routes/actions'
import { getMainAccordionItemCountByForm } from 'util/helpers'
import { getLanguageAvailabilityCodes } from 'selectors/selections'

export const messages = defineMessages({
  entityTitleNew: {
    id: 'Containers.Services.AddService.Header.Title',
    defaultMessage: 'Lisää uusi palvelu'
  },
  formTitle: {
    id: 'Containers.Services.AddService.Step1.Header.Title',
    defaultMessage: 'Vaihe 1/4: Palvelun perustiedot'
  },
  formTitle2: {
    id: 'Containers.Services.AddService.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/4: Luokittelu ja ontologiakäsitteet'
  },
  formTitle2InfoText: {
    id: 'Routes.Service.ServiceForm.Section2.InfoText',
    defaultMessage: 'Tämän osion tiedot eivät näy loppukäyttäjille.'
  },
  formTitle3: {
    id: 'Containers.Services.AddService.Step3.Header.Title',
    defaultMessage: 'Vaihe 3/4: Palvelun tuottaminen ja alueelliset tiedot'
  }
})

const handleNewLanguageResponse = (newLanguageCode, { getState, dispatch }) => {
  const state = getState()
  const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)

  const serviceName = getServiceFormSelector(state, 'name') || Map()
  const generalDescriptionId = getServiceFormSelector(state, 'generalDescriptionId')
  const generalDescription = generalDescriptionId && EntitySelectors.generalDescriptions.getEntity(state,
    { id: generalDescriptionId })
  const generalDescriptionName = generalDescription && generalDescription.get('name') || Map()
  const publishedGDLanguages = getGDPublishedLanguages(state, { formName:formTypesEnum.SERVICEFORM })

  let someNameIsOverwritenWithGD = false
  if (generalDescriptionName && generalDescriptionName.size > 0) {
    serviceName.forEach((name, langCode) => {
      if (publishedGDLanguages.contains(langCode)) {
        someNameIsOverwritenWithGD = name === generalDescriptionName.get(langCode)
        if (someNameIsOverwritenWithGD) return
      }
    })

    if (someNameIsOverwritenWithGD && publishedGDLanguages.contains(newLanguageCode)) {
      let nameGD = generalDescriptionName.get(newLanguageCode)
      let newServiceNames = serviceName.set(newLanguageCode, nameGD)

      dispatch(
        change(
          formTypesEnum.SERVICEFORM,
          'name',
          newServiceNames
        )
      )
    }
  }
}

const handleActionCancel = (action, actionCallback, { getState, dispatch }) => {
  const state = getState()
  const canArchive = canArchiveAstiEntity(state)
  switch (action) {
    case formActionsTypesEnum.ARCHIVEENTITY:
    case formActionsTypesEnum.ARCHIVELANGUAGE:
      canArchive
        ? actionCallback(action)
        : dispatch(mergeInUIState({
          key: `${formTypesEnum.SERVICEFORM}${action}CancelDialog`,
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

const successArchiveAction = (dispatch, response) => {
  const id = response && response.response && response.response.result && response.response.result.result
  const services = response && response.response && response.response.entities && response.response.entities.services
  const service = id && services && services[id]
  const numberOfConnections = service && service.numberOfConnections

  if (numberOfConnections === 0) {
    dispatch(change(formTypesEnum.CONNECTIONS, 'selectedConnections', List()))
    dispatch(change(formTypesEnum.ASTICONNECTIONS, 'connectionsFlat', List()))
  }
}

const basicPublishValidators = [
  { path: 'fundingType', validate: isRequired() },
  {
    path: 'name',
    validate: isEqual('shortDescription', formTypesEnum.SERVICEFORM)(),
    type: validationMessageTypes.asErrorVisible
  },
  { path: 'shortDescription', validate: isRequired() },
  {
    path: 'shortDescription',
    validate: isEqual('name', formTypesEnum.SERVICEFORM)(),
    type: validationMessageTypes.asErrorVisible
  },
  {
    path: 'description',
    validate: isRequiredDraftJs()
  },
  {
    path: 'description',
    validate: hasMaxBulletCountDraftJS(DRAFTJS_MAX_BULLET_COUNT)(),
    type: validationMessageTypes.visible
  },
  {
    path: 'description',
    validate: hasMinLengthDraftJS(DRAFTJS_MIN_LENGTH)(),
    type: validationMessageTypes.visible
  },
  {
    path: 'userInstruction',
    validate: hasValueDraftJSWithGDCheck('userInstruction')(),
    type: validationMessageTypes.visible
  },
  { path: 'languages', validate: isNotEmpty() }
]
const classificationPublishValidators = [
  { path: 'targetGroups', validate: isNotEmpty() },
  { path: 'serviceClasses', validate: isNotEmpty() },
  {
    path: 'serviceClasses',
    validate: hasValidServiceClasses('serviceClasses')({ fieldProps: {} }),
    type: validationMessageTypes.visible
  },
  { path: 'ontologyTerms', validate: isNotEmpty() },
  {
    path: 'ontologyTerms',
    validate: hasMaxItemCountWithGD(
      ONTOLOGY_TERMS_MAX_COUNT,
      'ontologyTerms',
      customValidationMessages.ontologyTermItemMaxCountReached
    )({ fieldProps: {} }),
    type: validationMessageTypes.visible
  }
]
const provisionMethodsAndProviderValidators = [
  {
    path: 'serviceProducers',
    validate: arrayValidator(
      serviceProducerValidation('serviceProducers')
    )
  },
  {
    path: 'serviceProducers',
    validate: isNotEmpty()
  }
]
const publishValidators = [
  ...basicPublishValidators,
  ...classificationPublishValidators,
  ...provisionMethodsAndProviderValidators
]
const fieldsRequiredForSave = [
  'name',
  'organization'
]
const basicPublishFields = basicPublishValidators
  .map(validator => validator.path)
const classificationPublishFields = classificationPublishValidators
  .map(validator => validator.path)

class ServiceForm extends Component {
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
        <LanguageComparisonSelect />
        <div className='row'>
          <div className='col-lg-12'>
            <ValidationMessages form={form} top />
          </div>
        </div>
        <ReduxAccordion
          reduxKey={'serviceAccordion'}
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
            <ServiceBasic />
          </ReduxAccordion.Content>
          <ReduxAccordion.Title
            publishFields={classificationPublishFields}
            title={formatMessage(messages.formTitle2)}
            helpText={formatMessage(messages.formTitle2InfoText)} />
          <ReduxAccordion.Content>
            <ServiceClassificationAndKeywords />
          </ReduxAccordion.Content>
          <ReduxAccordion.Title
            publishFields={['serviceProducers']}
            title={formatMessage(messages.formTitle3)}
          />
          <ReduxAccordion.Content>
            <ServiceProducerInfo />
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
ServiceForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  isCompareMode: PropTypes.bool,
  isInReview: PropTypes.bool
}

const onSubmit = handleOnSubmit({
  url: 'service/SaveService',
  transformers: [
    languagesAvailabilitiesTransformer,
    serviceBasicTransformer
  ],
  schema: EntitySchemas.SERVICE
})

const entityType = 'service'

const qualityCheck = (store, formName, languages) => {
  const state = store.getState()
  const options = {
    formName,
    entityType,
    profile: 'VRKp',
    languages
  }
  const data = getAdditionalQualityCheckData(state, { formName })
  directQualityEntityCheck(data, store, options)
}

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, props, getService)
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

const onSubmitFail = (...args) => console.log(args)

const getIsLoading = getIsFormLoading(formTypesEnum.SERVICEFORM)

const warn = validate(publishValidators)

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      const isLoading = EntitySelectors.services.getEntityIsFetching(state) || getIsLoading(state)
      const gdId = ownProps.location && ownProps.location.state && ownProps.location.state.gd
      const initialValues = !ownProps.id && gdId
        ? getServiceByGeneralDescription(state, { gdId, ...ownProps })
        : getService(state, ownProps)
      return {
        isLoading,
        initialValues,
        templateId: ownProps.templateId,
        isInReview: getShowReviewBar(state)
      }
    }
  ),
  reduxForm({
    form: formTypesEnum.SERVICEFORM,
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
  withAutomaticSave({
    draftJSFields: [
      'name',
      'shortDescription',
      'description',
      'conditionOfServiceUsage',
      'userInstruction'
    ]
  }),
  withEntityNotification,
  withEntityTitle({
    newEntityTitle: <FormattedMessage {...messages.entityTitleNew} />,
    newLanguageVersionTitle: <FormattedMessage {...commonFormMessages.languageVersionTitleNew} />
  }),
  branch(({ isInReview }) => !isInReview, withConnectionStep),
  withEntityButtons({
    formNameToSubmit: formTypesEnum.SERVICEFORM,
    onEdit,
    onCancel
  }),
  withEntityHeader({
    entityId: null,
    handleNewLanguageResponse: handleNewLanguageResponse,
    successArchiveAction: successArchiveAction,
    handleActionCancel: handleActionCancel
  }),
  withFormStates,
  withPublishingDialog(),
  withPreviewDialog,
  withQualityAgentProvider({
    entityPrefix: 'service',
    dataSelector: getAdditionalQualityCheckData
  })
)(ServiceForm)
