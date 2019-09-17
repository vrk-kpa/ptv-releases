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
import { injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import {
  getIsGeneralDescriptionAttached,
  getGeneralDescriptionServiceType,
  getGDUserInstructionValue,
  getGDUserInstructionCompareValue,
  getGDConditionOfServiceUsageValue,
  getGDConditionOfServiceUsageCompareValue,
  getGDDescriptionValue,
  getGDDescriptionCompareValue,
  getGeneralDescriptionChargeTypeType,
  getGDBackgroundDescriptionValue,
  getGDBackgroundDescriptionCompareValue,
  getIsAnyGDLawInContentLanguage,
  getIsAnyGDLawInCompareLanguage,
  getGDDeadLineAdditionalInfoValue,
  getGDDeadLineAdditionalInfoCompareValue,
  getGDProcessingTimeAdditionalInfoValue,
  getGDProcessingTimeAdditionalInfoCompareValue,
  getGDValidityTimeAdditionalInfoValue,
  getGDValidityTimeAdditionalInfoCompareValue
} from './selectors'
import asGroup from 'util/redux-form/HOC/asGroup'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
// import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import {
  serviceDescriptionMessages,
  typeProfessionalAdditionalInfoMessagesServices } from '../Messages'
import {
  Description,
  DescriptionGD,
  ServiceType,
  ServiceLawsGD,
  ConditionOfServiceUsage,
  ConditionOfServiceUsageGD,
  UserInstruction,
  UserInstructionGD,
  DeadLineAdditionalInfo,
  DeadLineAdditionalInfoGD,
  ProcessingTimeAdditionalInfo,
  ProcessingTimeAdditionalInfoGD,
  ValidityTimeAdditionalInfo,
  ValidityTimeAdditionalInfoGD
} from 'util/redux-form/fields'
import {
  ServiceChargeType,
  ServiceChargeTypeWithGD,
  Laws
} from 'util/redux-form/sections'
import {
  getServiceTypeCodeSelected
} from '../../selectors'

// SERVICE CHARGE TYPE - start
const ServiceServiceChargeTypeWithGDComponent = () => (<ServiceChargeTypeWithGD />)

const ServiceServiceChargeTypeWithGD = compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })))(ServiceServiceChargeTypeWithGDComponent)

export const ServiceServiceChargeType = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => {
    const value = getGeneralDescriptionChargeTypeType(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showServiceChargeTypeWithGDInfo: isGeneralDescriptionAttached && !!value
    }
  }))(
  ({
    intl: { formatMessage },
    showServiceChargeTypeWithGDInfo
  }) => (
    <div className='form-row'>
      {showServiceChargeTypeWithGDInfo && <ServiceServiceChargeTypeWithGD /> ||
        <ServiceChargeType />}
    </div>
  )
)
// SERVICE CHARGE TYPE - end

// SERVICE TYPE - start
const ServiceServiceTypeWithGDComponent = (
  {
    intl: { formatMessage },
    generalDescriptionServiceType
  }
) => (<ServiceType input={{ value: generalDescriptionServiceType }} nonField radio />)

ServiceServiceTypeWithGDComponent.propTypes = {
  intl: intlShape.isRequired,
  generalDescriptionServiceType: PropTypes.string.isRequired
}

const ServiceServiceTypeWithGD = compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps),
    generalDescriptionServiceType: getGeneralDescriptionServiceType(state, ownProps)
  })))(ServiceServiceTypeWithGDComponent)

export const ServiceServiceType = compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })))(
  ({
    intl: { formatMessage },
    isGeneralDescriptionAttached
  }) => (
    isGeneralDescriptionAttached && <ServiceServiceTypeWithGD /> ||
    <ServiceType radio />
  )
)
// SERVICE TYPE - end

// SERVICE DESCRIPTION - start
const ServiceDescriptionWithGDComponent = ({
  intl: { formatMessage }
}
) => (
  <div>
    <DescriptionGD />
    <div className='form-row'>
      <Description
        label={formatMessage(serviceDescriptionMessages.additionalDescription)}
        placeholder={formatMessage(serviceDescriptionMessages.descriptionPlaceholder)}
        tooltip={formatMessage(serviceDescriptionMessages.descriptionTooltip)}
        useQualityAgent
	qualityAgentCompare
      />
    </div>
  </div>
)

ServiceDescriptionWithGDComponent.propTypes = {
  intl: intlShape.isRequired
}

const ServiceDescriptionWithGD = compose(
  injectIntl,
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.descriptionTitle,
    tooltip: <FormattedMessage {...serviceDescriptionMessages.descriptionTooltip} /> }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })))(ServiceDescriptionWithGDComponent)

export const ServiceDescription = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  // withLanguageKey,
  connect((state, ownProps) => {
    const value = getGDDescriptionValue(state, ownProps)
    const compareValue = getGDDescriptionCompareValue(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showDescriptionWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue)
        : !!value
    }
  }))(
  ({
    intl: { formatMessage },
    showDescriptionWithGDInfo,
    required
  }) => (
    showDescriptionWithGDInfo && <ServiceDescriptionWithGD /> ||
      <Description
        label={formatMessage(serviceDescriptionMessages.descriptionTitle)}
        placeholder={formatMessage(serviceDescriptionMessages.descriptionPlaceholder)}
        tooltip={formatMessage(serviceDescriptionMessages.descriptionTooltip)}
        required={required}
        useQualityAgent
        qualityAgentCompare
      />
  )
)
// SERVICE DESCRIPTION - end

// SERVICE CONDITIONS - start
const ServiceConditionOfUsageWithGDComponent = ({
  intl: { formatMessage }
}
) => (
  <div>
    <ConditionOfServiceUsageGD />
    <div className='form-row'>
      <ConditionOfServiceUsage
        label={formatMessage(serviceDescriptionMessages.additionalDescription)}
        useQualityAgent
        qualityAgentCompare
      />
    </div>
  </div>
)

ServiceConditionOfUsageWithGDComponent.propTypes = {
  intl: intlShape.isRequired
}

const ServiceConditionOfUsageWithGD = compose(
  injectIntl,
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.conditionOfServiceUsageTitle }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })))(ServiceConditionOfUsageWithGDComponent)

export const ServiceConditionsOfUsage = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  // withLanguageKey,
  connect((state, ownProps) => {
    const value = getGDConditionOfServiceUsageValue(state, ownProps)
    const compareValue = getGDConditionOfServiceUsageCompareValue(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showServiceConditionsWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue)
        : !!value
    }
  })
)(
  ({
    intl: { formatMessage },
    showServiceConditionsWithGDInfo
  }) => (
    showServiceConditionsWithGDInfo && <ServiceConditionOfUsageWithGD /> ||
      <ConditionOfServiceUsage
        label={formatMessage(serviceDescriptionMessages.conditionOfServiceUsageTitle)}
        useQualityAgent
        qualityAgentCompare
      />
  )
)
// SERVICE CONDITIONS - end

// SERVICE USER INSTRUCTIONS - start
const ServiceUserInstructionWithGDComponent = ({
  intl: { formatMessage }
}
) => (
  <div>
    <UserInstructionGD />
    <div className='form-row'>
      <UserInstruction
        label={formatMessage(serviceDescriptionMessages.additionalDescription)}
        useQualityAgent
        qualityAgentCompare
      />
    </div>
  </div>
)

ServiceUserInstructionWithGDComponent.propTypes = {
  intl: intlShape.isRequired
}

const ServiceUserInstructionWithGD = compose(
  injectIntl,
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.serviceUserInstructionTitle }),
  connect((state, ownProps) => {
    return {
      isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
    }
  }))(ServiceUserInstructionWithGDComponent)

export const ServiceUserInstruction = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  // withLanguageKey,
  connect((state, ownProps) => {
    const value = getGDUserInstructionValue(state, ownProps)
    const compareValue = getGDUserInstructionCompareValue(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showUserInstructionWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue)
        : !!value
    }
  }))(
  ({
    intl: { formatMessage },
    showUserInstructionWithGDInfo
  }) => (
    showUserInstructionWithGDInfo && <ServiceUserInstructionWithGD /> ||
      <UserInstruction
        label={formatMessage(serviceDescriptionMessages.serviceUserInstructionTitle)}
        useQualityAgent
        qualityAgentCompare
      />
  )
)
// SERVICE USER INSTRUCTIONS - end

// SERVICE LAWS - start
const ServiceLawsWithGDComponent = () => (<ServiceLawsGD />)

const ServiceLawsWithGD = compose(
  injectIntl,
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.backgroundAreaTitle }))(ServiceLawsWithGDComponent)

export const ServiceLaws = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  // withLanguageKey,
  connect((state, ownProps) => {
    const value = getGDBackgroundDescriptionValue(state, ownProps)
    const compareValue = getGDBackgroundDescriptionCompareValue(state, ownProps)
    const isAnyGDLawInContentLanguage = getIsAnyGDLawInContentLanguage(state, ownProps)
    const isAnyGDLawInCompareLanguage = getIsAnyGDLawInCompareLanguage(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showBackgroundInfoWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue) || isAnyGDLawInContentLanguage || isAnyGDLawInCompareLanguage
        : !!value || isAnyGDLawInContentLanguage
    }
  }))(
  ({
    intl: { formatMessage },
    showBackgroundInfoWithGDInfo
  }) => (
    <div>
      <div className='form-row'>
        {showBackgroundInfoWithGDInfo && <ServiceLawsWithGD /> }
      </div>
      <div className='form-row'>
        <Laws />
      </div>
    </div>
  )
)
// SERVICE LAWS - end

// SERVICE DEADLINE ADDITIONAL INFO - start
const ServiceDeadlineAdditionalInfoWithGDComponent = ({
  intl: { formatMessage },
  isQualification
}
) => (
  <div className='form-row'>
    <div>
      <DeadLineAdditionalInfoGD
        labelPosition='top'
        disabled
        multiline
        rows={3}
        size='full'
        maxLength={500}
        counter
      />
    </div>
    <DeadLineAdditionalInfo
      label={formatMessage(serviceDescriptionMessages.additionalDescription)}
      placeholder={isQualification
        ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationDeadlinePlaceholder)
        : formatMessage(typeProfessionalAdditionalInfoMessagesServices.deadlinePlaceholder)}
      tooltip={isQualification
        ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationDeadlineTootltip)
        : formatMessage(typeProfessionalAdditionalInfoMessagesServices.deadlineTootltip)}
      multiline
      rows={3}
      counter
      maxLength={500}
      useQualityAgent
    />
  </div>
)

ServiceDeadlineAdditionalInfoWithGDComponent.propTypes = {
  intl: intlShape.isRequired,
  isQualification: PropTypes.bool.isRequired
}

const ServiceDeadlineAdditionalInfoWithGD = compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  asGroup({ title: typeProfessionalAdditionalInfoMessagesServices.deadlineTitle,
    tooltip: <FormattedMessage {...typeProfessionalAdditionalInfoMessagesServices.deadlineTootltip} /> }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })))(ServiceDeadlineAdditionalInfoWithGDComponent)

export const ServiceDeadlineAdditionalInfo = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => {
    const value = getGDDeadLineAdditionalInfoValue(state, ownProps)
    const compareValue = getGDDeadLineAdditionalInfoCompareValue(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showDeadlineWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue)
        : !!value,
      isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
    }
  }))(
  ({
    intl: { formatMessage },
    showDeadlineWithGDInfo,
    isQualification
  }) => (
    showDeadlineWithGDInfo && <ServiceDeadlineAdditionalInfoWithGD /> ||
      <DeadLineAdditionalInfo
        label={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationDeadlineTitle)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.deadlineTitle)}
        placeholder={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationDeadlinePlaceholder)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.deadlinePlaceholder)}
        tooltip={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationDeadlineTootltip)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.deadlineTootltip)}
        multiline
        rows={3}
        counter
        maxLength={500}
        useQualityAgent
      />
  )
)
// SERVICE DEADLINE ADDITIONAL INFO - end

// SERVICE PROCESSING ADDITIONAL INFO - start
const ServiceProcessingTimeAdditionalInfoWithGDComponent = ({
  intl: { formatMessage },
  isQualification
}
) => {
  return (
    <div className='form-row'>
      <div>
        <ProcessingTimeAdditionalInfoGD
          labelPosition='top'
          disabled
          multiline
          rows={3}
          size='full'
          maxLength={500}
          counter
        />
      </div>
      <ProcessingTimeAdditionalInfo
        label={formatMessage(serviceDescriptionMessages.additionalDescription)}
        placeholder={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationProcessingTimePlaceholder)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.processingTimePlaceholder)}
        tooltip={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationProcessingTimeTootltip)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.processingTimeTootltip)}
        multiline
        rows={3}
        counter
        maxLength={500}
        useQualityAgent
      />
    </div>
  )
}

ServiceProcessingTimeAdditionalInfoWithGDComponent.propTypes = {
  intl: intlShape.isRequired,
  isQualification: PropTypes.bool.isRequired
}

const ServiceProcessingTimeAdditionalInfoWithGD = compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  asGroup({ title: typeProfessionalAdditionalInfoMessagesServices.processingTimeTitle,
    tooltip: <FormattedMessage {...typeProfessionalAdditionalInfoMessagesServices.processingTimeTootltip} /> }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })))(ServiceProcessingTimeAdditionalInfoWithGDComponent)

export const ServiceProcessingTimeAdditionalInfo = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => {
    const value = getGDProcessingTimeAdditionalInfoValue(state, ownProps)
    const compareValue = getGDProcessingTimeAdditionalInfoCompareValue(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showProcessingTimeWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue)
        : !!value,
      isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
    }
  }))(
  ({
    intl: { formatMessage },
    showProcessingTimeWithGDInfo,
    isQualification
  }) => (
    showProcessingTimeWithGDInfo && <ServiceProcessingTimeAdditionalInfoWithGD /> ||
      <ProcessingTimeAdditionalInfo
        label={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationProcessingTimeTitle)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.processingTimeTitle)}
        placeholder={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationProcessingTimePlaceholder)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.processingTimePlaceholder)}
        tooltip={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationProcessingTimeTootltip)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.processingTimeTootltip)}
        multiline
        rows={3}
        counter
        maxLength={500}
        useQualityAgent
      />
  )
)
// SERVICE PROCESSING ADDITIONAL INFO - end

// SERVICE VALIDITY ADDITIONAL INFO - start
const ServiceValidityTimeAdditionalInfoWithGDComponent = ({
  intl: { formatMessage },
  isQualification
}
) => (
  <div className='form-row'>
    <div>
      <ValidityTimeAdditionalInfoGD
        labelPosition='top'
        disabled
        multiline
        rows={3}
        size='full'
        maxLength={500}
        counter
      />
    </div>
    <ValidityTimeAdditionalInfo
      label={formatMessage(serviceDescriptionMessages.additionalDescription)}
      placeholder={isQualification
        ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationValidityTimePlaceholder)
        : formatMessage(typeProfessionalAdditionalInfoMessagesServices.validityTimePlaceholder)}
      tooltip={isQualification
        ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationValidityTimeTootltip)
        : formatMessage(typeProfessionalAdditionalInfoMessagesServices.validityTimeTootltip)}
      multiline
      rows={3}
      counter
      maxLength={500}
      useQualityAgent
    />
  </div>
)

ServiceValidityTimeAdditionalInfoWithGDComponent.propTypes = {
  intl: intlShape.isRequired,
  isQualification: PropTypes.bool.isRequired
}

const ServiceValidityTimeAdditionalInfoWithGD = compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  asGroup({
    title: typeProfessionalAdditionalInfoMessagesServices.validityTimeTitle,
    tooltip: <FormattedMessage {...typeProfessionalAdditionalInfoMessagesServices.validityTimeTootltip} />
  }))(ServiceValidityTimeAdditionalInfoWithGDComponent)

export const ServiceValidityTimeAdditionalInfo = compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => {
    const value = getGDValidityTimeAdditionalInfoValue(state, ownProps)
    const compareValue = getGDValidityTimeAdditionalInfoCompareValue(state, ownProps)
    const isGeneralDescriptionAttached = getIsGeneralDescriptionAttached(state, ownProps)
    return {
      showValidityTimeWithGDInfo: isGeneralDescriptionAttached && ownProps.isCompareMode
        ? !!(value || compareValue)
        : !!value,
      isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
    }
  }))(
  ({
    intl: { formatMessage },
    showValidityTimeWithGDInfo,
    isQualification
  }) => (
    showValidityTimeWithGDInfo && <ServiceValidityTimeAdditionalInfoWithGD /> ||
      <ValidityTimeAdditionalInfo
        label={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationValidityTimeTitle)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.validityTimeTitle)}
        placeholder={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationValidityTimePlaceholder)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.validityTimePlaceholder)}
        tooltip={isQualification
          ? formatMessage(typeProfessionalAdditionalInfoMessagesServices.qualificationValidityTimeTootltip)
          : formatMessage(typeProfessionalAdditionalInfoMessagesServices.validityTimeTootltip)}
        multiline
        rows={3}
        counter
        maxLength={500}
        useQualityAgent
      />
  )
)
// SERVICE VALIDITY ADDITIONAL INFO - end
