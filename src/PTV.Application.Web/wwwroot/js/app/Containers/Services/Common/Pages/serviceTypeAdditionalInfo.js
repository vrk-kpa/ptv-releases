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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// components
import { PTVLabel } from '../../../../Components'
import ServiceDescription from './serviceDescription'

// actions
import * as serviceActions from '../../Service/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as ServiceSelectors from '../../Service/Selectors'

const ServiceTypeAdditionalInfo = ({
    messages, readOnly, language, translationMode,
    additionalInformationDeadLine,
    deadLineFromGeneralDescription,
    additionalInformationProcessingTime,
    processingTimeFromGeneralDescription,
    additionalInformationValidityTime,
    validityTimeFromGeneralDescription,
    selectedServiceType,
    actions, intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onServiceInputChange(input, value, language, isSet)
  }

  const specificMessages = selectedServiceType === 'PermissionAndObligation'
    ? messages.serviceTypeAdditionalInfoMessages
    : messages.serviceTypeProfessionalAdditionalInfoMessages

  return (
            selectedServiceType !== 'Service'
                ? <div>
                  <div className='form-group'>
                    <div>
                      <PTVLabel><strong>{ intl.formatMessage(specificMessages.title) }</strong></PTVLabel>
                    </div>
                  </div>
                  <div className='form-group'>
                    <ServiceDescription
                      title={intl.formatMessage(specificMessages.deadlineTitle)}
                      tooltip={intl.formatMessage(specificMessages.deadlineTootltip)}
                      placeHolder={intl.formatMessage(specificMessages.deadlinePlaceholder)}
                      readOnly={readOnly}
                      translationMode={translationMode}
                      name='serviceDeadline'
                      value={additionalInformationDeadLine}
                      valueFromGeneralDescription={deadLineFromGeneralDescription}
                      blurCallback={onInputChange('additionalInformationDeadLine')}
                      maxLength={500}
                      minRows={2}
                      language={language}
                        />
                    <ServiceDescription
                      title={intl.formatMessage(specificMessages.processingTimeTitle)}
                      tooltip={intl.formatMessage(specificMessages.processingTimeTootltip)}
                      placeHolder={intl.formatMessage(specificMessages.processingTimePlaceholder)}
                      readOnly={readOnly}
                      translationMode={translationMode}
                      name='serviceProcessingTime'
                      value={additionalInformationProcessingTime}
                      valueFromGeneralDescription={processingTimeFromGeneralDescription}
                      blurCallback={onInputChange('additionalInformationProcessingTime')}
                      maxLength={500}
                      minRows={2}
                      language={language}
                        />
                    <ServiceDescription
                      title={intl.formatMessage(specificMessages.validityTimeTitle)}
                      tooltip={intl.formatMessage(specificMessages.validityTimeTootltip)}
                      placeHolder={intl.formatMessage(specificMessages.validityTimePlaceholder)}
                      readOnly={readOnly}
                      translationMode={translationMode}
                      name='serviceValidityTime'
                      value={additionalInformationValidityTime}
                      valueFromGeneralDescription={validityTimeFromGeneralDescription}
                      blurCallback={onInputChange('additionalInformationValidityTime')}
                      maxLength={500}
                      minRows={2}
                      language={language}
                        />
                  </div>
                </div>
            : null
  )
}

function mapStateToProps (state, ownProps) {
  return {
    selectedServiceType: ServiceSelectors.getSelctedServiceTypeCode(state, ownProps),
    additionalInformationDeadLine: ServiceSelectors.getAdditionalInformationDeadLine(state, ownProps),
    deadLineFromGeneralDescription: ServiceSelectors.getDeadLineFromGeneralDescriptionLocale(state, ownProps),
    additionalInformationProcessingTime: ServiceSelectors.getAdditionalInformationProcessingTime(state, ownProps),
    processingTimeFromGeneralDescription: ServiceSelectors.getProcessingTimeFromGeneralDescriptionLocale(state, ownProps),
    additionalInformationValidityTime: ServiceSelectors.getAdditionalInformationValidityTime(state, ownProps),
    validityTimeFromGeneralDescription: ServiceSelectors.getValidityTimeFromGeneralDescriptionLocale(state, ownProps)
  }
}

const actions = [
  serviceActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceTypeAdditionalInfo))

