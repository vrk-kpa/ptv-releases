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
import { injectIntl, FormattedMessage } from 'react-intl'
import {
  getIsGeneralDescriptionAttached,
  getGeneralDescriptionServiceType
 } from './selectors'
import { asGroup, injectFormName } from 'util/redux-form/HOC'
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
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(ServiceServiceChargeTypeWithGDComponent)

export const ServiceServiceChargeType = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached
    }) => (
      <div className='form-row'>
        {isGeneralDescriptionAttached && <ServiceServiceChargeTypeWithGD /> ||
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
  intl: PropTypes.object.isRequired,
  generalDescriptionServiceType: PropTypes.string.isRequired
}

const ServiceServiceTypeWithGD = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps),
    generalDescriptionServiceType: getGeneralDescriptionServiceType(state, ownProps)
  })),
  injectIntl)(ServiceServiceTypeWithGDComponent)

export const ServiceServiceType = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(
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
          />
    </div>
  </div>
  )

ServiceDescriptionWithGDComponent.propTypes = {
  intl: PropTypes.object.isRequired
}

const ServiceDescriptionWithGD = compose(
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.descriptionTitle,
    tooltip: <FormattedMessage {...serviceDescriptionMessages.descriptionTooltip} /> }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(ServiceDescriptionWithGDComponent)

export const ServiceDescription = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached,
      required
    }) => (
      isGeneralDescriptionAttached && <ServiceDescriptionWithGD /> ||
        <Description
          label={formatMessage(serviceDescriptionMessages.descriptionTitle)}
          placeholder={formatMessage(serviceDescriptionMessages.descriptionPlaceholder)}
          tooltip={formatMessage(serviceDescriptionMessages.descriptionTooltip)}
          required={required}
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
        />
    </div>
  </div>
  )

ServiceConditionOfUsageWithGDComponent.propTypes = {
  intl: PropTypes.object.isRequired
}

const ServiceConditionOfUsageWithGD = compose(
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.conditionOfServiceUsageTitle }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(ServiceConditionOfUsageWithGDComponent)

export const ServiceConditionsOfUsage = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached,
      isCondition
    }) => (
      isGeneralDescriptionAttached && <ServiceConditionOfUsageWithGD /> ||
        <ConditionOfServiceUsage
          label={formatMessage(serviceDescriptionMessages.conditionOfServiceUsageTitle)}
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
        />
    </div>
  </div>
  )

ServiceUserInstructionWithGDComponent.propTypes = {
  intl: PropTypes.object.isRequired
}

const ServiceUserInstructionWithGD = compose(
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.serviceUserInstructionTitle }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(ServiceUserInstructionWithGDComponent)

export const ServiceUserInstruction = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached
    }) => (
      isGeneralDescriptionAttached && <ServiceUserInstructionWithGD /> ||
        <UserInstruction
          label={formatMessage(serviceDescriptionMessages.serviceUserInstructionTitle)}
          />
    )
  )
// SERVICE USER INSTRUCTIONS - end

// SERVICE LAWS - start
const ServiceLawsWithGDComponent = () => (<ServiceLawsGD />)

const ServiceLawsWithGD = compose(
  injectFormName,
  asGroup({ title: serviceDescriptionMessages.backgroundAreaTitle }),
  injectIntl)(ServiceLawsWithGDComponent)

export const ServiceLaws = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached
    }) => (
      <div>
        <div className='form-row'>
          {isGeneralDescriptionAttached && <ServiceLawsWithGD /> }
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
        />
  </div>
  )

ServiceDeadlineAdditionalInfoWithGDComponent.propTypes = {
  intl: PropTypes.object.isRequired,
  isQualification: PropTypes.bool.isRequired
}

const ServiceDeadlineAdditionalInfoWithGD = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  asGroup({ title: typeProfessionalAdditionalInfoMessagesServices.deadlineTitle,
    tooltip: <FormattedMessage {...typeProfessionalAdditionalInfoMessagesServices.deadlineTootltip} /> }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(ServiceDeadlineAdditionalInfoWithGDComponent)

export const ServiceDeadlineAdditionalInfo = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps),
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached,
      isQualification
    }) => (
      isGeneralDescriptionAttached && <ServiceDeadlineAdditionalInfoWithGD /> ||
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
        />
    </div>
  )
}

ServiceProcessingTimeAdditionalInfoWithGDComponent.propTypes = {
  intl: PropTypes.object.isRequired,
  isQualification: PropTypes.bool.isRequired
}

const ServiceProcessingTimeAdditionalInfoWithGD = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  asGroup({ title: typeProfessionalAdditionalInfoMessagesServices.processingTimeTitle,
    tooltip: <FormattedMessage {...typeProfessionalAdditionalInfoMessagesServices.processingTimeTootltip} /> }),
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps)
  })),
  injectIntl)(ServiceProcessingTimeAdditionalInfoWithGDComponent)

export const ServiceProcessingTimeAdditionalInfo = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps),
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached,
      isQualification
    }) => (
      isGeneralDescriptionAttached && <ServiceProcessingTimeAdditionalInfoWithGD /> ||
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
        />
  </div>
  )

ServiceValidityTimeAdditionalInfoWithGDComponent.propTypes = {
  intl: PropTypes.object.isRequired,
  isQualification: PropTypes.bool.isRequired
}

const ServiceValidityTimeAdditionalInfoWithGD = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  asGroup({
    title: typeProfessionalAdditionalInfoMessagesServices.validityTimeTitle,
    tooltip: <FormattedMessage {...typeProfessionalAdditionalInfoMessagesServices.validityTimeTootltip} />
  }),
  injectIntl)(ServiceValidityTimeAdditionalInfoWithGDComponent)

export const ServiceValidityTimeAdditionalInfo = compose(
  injectFormName,
  connect((state, ownProps) => ({
    isGeneralDescriptionAttached: getIsGeneralDescriptionAttached(state, ownProps),
    isQualification: getServiceTypeCodeSelected(state, ownProps) === 'qualification'
  })),
  injectIntl)(
    ({
      intl: { formatMessage },
      isGeneralDescriptionAttached,
      isQualification
    }) => (
      isGeneralDescriptionAttached && <ServiceValidityTimeAdditionalInfoWithGD /> ||
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
          />
    )
  )
// SERVICE VALIDITY ADDITIONAL INFO - end
