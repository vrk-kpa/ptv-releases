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
import { injectIntl } from 'react-intl'

// components
import { PTVTextAreaNotEmpty, PTVLabel } from '../../../../../Components'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'

const ServiceTypeAdditionalInfo = ({
    messages, readOnly, language, translationMode,
    additionalInformationDeadLine,
    additionalInformationProcessingTime,
    additionalInformationValidityTime,
    selectedServiceType,
    actions, intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onGeneralDescriptionTranslatedInputChange(input, value, language, isSet)
  }

  const specificMessages = selectedServiceType === 'PermissionAndObligation'
    ? messages.typeAdditionalInfoMessages
    : messages.typeProfessionalAdditionalInfoMessages
  const translatableAreaRO = readOnly && translationMode === 'none'

  return (
            selectedServiceType !== 'Service'
                ? <div>
                  <div className='row'>
                    <div className='col-xs-12'>
                      <PTVLabel><strong>{ intl.formatMessage(specificMessages.title) }</strong></PTVLabel>
                    </div>
                  </div>
                  <div className={translatableAreaRO ? '' : 'service-description'}>
                    <div className='row form-group'>
                      <PTVTextAreaNotEmpty
                        componentClass='col-xs-12'
                        minRows={3}
                        maxLength={500}
                        label={intl.formatMessage(specificMessages.deadlineTitle)}
                        placeholder={intl.formatMessage(specificMessages.deadlinePlaceholder)}
                        tooltip={intl.formatMessage(specificMessages.deadlineTootltip)}
                        value={additionalInformationDeadLine}
                        name='generalDescriptionDeadline'
                        blurCallback={onInputChange('additionalInformationDeadLine')}
                        order={20}
                        disabled={translationMode === 'view'}
                        readOnly={translatableAreaRO}
                        validatedField={specificMessages.deadlineTitle} />
                    </div>
                    <div className='row form-group'>
                      <PTVTextAreaNotEmpty
                        componentClass='col-xs-12'
                        minRows={3}
                        maxLength={500}
                        label={intl.formatMessage(specificMessages.processingTimeTitle)}
                        placeholder={intl.formatMessage(specificMessages.processingTimePlaceholder)}
                        tooltip={intl.formatMessage(specificMessages.processingTimeTootltip)}
                        value={additionalInformationProcessingTime}
                        name='generalDescriptionProcessingTime'
                        blurCallback={onInputChange('additionalInformationProcessingTime')}
                        order={30}
                        disabled={translationMode === 'view'}
                        readOnly={translatableAreaRO}
                        validatedField={specificMessages.processingTimeTitle} />
                    </div>
                    <div className='row form-group'>
                      <PTVTextAreaNotEmpty
                        componentClass='col-xs-12'
                        minRows={3}
                        maxLength={500}
                        label={intl.formatMessage(specificMessages.validityTimeTitle)}
                        placeholder={intl.formatMessage(specificMessages.validityTimePlaceholder)}
                        tooltip={intl.formatMessage(specificMessages.validityTimeTootltip)}
                        value={additionalInformationValidityTime}
                        name='generalDescriptionValidityTime'
                        blurCallback={onInputChange('additionalInformationValidityTime')}
                        order={40}
                        disabled={translationMode === 'view'}
                        readOnly={translatableAreaRO}
                        validatedField={specificMessages.validityTimeTitle} />
                    </div>
                  </div>
                </div>
            : null
  )
}

function mapStateToProps (state, ownProps) {
  return {
    selectedServiceType:
      GeneralDescriptionSelectors.getSelectedServiceTypeCode(state, ownProps),
    additionalInformationDeadLine:
      GeneralDescriptionSelectors.getGeneralDescriptionAdditionalInformationDeadLine(state, ownProps),
    additionalInformationProcessingTime:
      GeneralDescriptionSelectors.getGeneralDescriptionAdditionalInformationProcessingTime(state, ownProps),
    additionalInformationValidityTime:
      GeneralDescriptionSelectors.getGeneralDescriptionAdditionalInformationValidityTime(state, ownProps)
  }
}

const actions = [
  generealDescriptionActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceTypeAdditionalInfo))

